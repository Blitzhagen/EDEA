using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using EDEA.Services;
using EDEA.Stores;
using EDEA.ViewModels;
using EDEA.Windows;
using Jot;
using Jot.Storage;
using log4net;
using log4net.Appender;
using log4net.Config;
using Microsoft.Win32;

namespace EDEA;

public partial class App : Application
{
    private static readonly Mutex singletonApp = new(true, "EDEA7062a413-faf0-406d-bae5-cddf0541e295");
    private static readonly HttpClient httpClient = new();
    private static readonly ILog log = LogManager.GetLogger(typeof(App));

    private SQLiteStore? _sqliteStore;
    private SettingsProvider? _settingsProvider;
    private StarSystemProvider? _starSystemProvider;

    public App()
    {
        ConfigureLog4Net();

        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(GetType().Assembly.GetName().Name + "/" + Globals.AppVersionString);

        log.Info($"################# Application started on {DateTime.Now.ToString(CultureInfo.CurrentCulture)}, version {Globals.AppVersionString} #################");

        bool isNotRelease = GetType().Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration != "";
        if (isNotRelease)
        {
            IAppender[] appenders = LogManager.GetRepository().GetAppenders();
            foreach (IAppender appender in appenders)
            {
                if (appender is RollingFileAppender rollingFileAppender)
                {
                    rollingFileAppender.ClearFilters();
                }
            }
        }

        if (!singletonApp.WaitOne(TimeSpan.Zero, true))
        {
            log.Warn("Another instance of EDEA is already running, shutting down ...");
            Current?.Shutdown();
            return;
        }

        ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(60000));
        ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(100));

        JotSettingsProvider.Tracker = new Tracker(new JsonFileStore(Path.Combine(Globals.AppDataFolder, "jot")));
        JotSettingsProvider.Tracker.Configure<Window>()
            .Id((Window w) => w.Name, new { W = SystemParameters.VirtualScreenWidth, H = SystemParameters.VirtualScreenHeight })
            .Properties((Window w) => new { w.Top, w.Width, w.Height, w.Left, w.WindowState })
            .PersistOn("Closing")
            .StopTrackingOn("Closing");

        if (Process.GetProcessesByName("EDMarketConnector").FirstOrDefault() == null)
        {
            log.Warn("Elite Dangerous Market Connector (EDMC) seems not be running. It is highly recommended to start EDMC");
        }

        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
    }

    private static void ConfigureLog4Net()
    {
        string logFileFullPath = Path.Combine(Globals.AppDataFolder, "log", (Assembly.GetEntryAssembly()?.GetName().Name ?? "EDEA") + ".log");
        Directory.CreateDirectory(Path.GetDirectoryName(logFileFullPath)!);
        GlobalContext.Properties["LogFileFullPath"] = logFileFullPath;

        string rootLevel = "INFO";
        string fileLevelMin = "INFO";
#if DEBUG
        rootLevel = "DEBUG";
        fileLevelMin = "DEBUG";
#endif

        string log4netConfig = $@"
<log4net>
  <appender name=""TraceAppender"" type=""log4net.Appender.TraceAppender"">
    <layout type=""log4net.Layout.PatternLayout"">
      <conversionPattern value=""%date{{HH:mm:ss,fff}} %-5level | %message (%logger)%newline%exception"" />
    </layout>
  </appender>
  <appender name=""RollingFileAppender"" type=""log4net.Appender.RollingFileAppender"">
    <filter type=""log4net.Filter.LevelRangeFilter"">
      <levelMin value=""{fileLevelMin}"" />
    </filter>
    <file type=""log4net.Util.PatternString"" value=""%property{{LogFileFullPath}}"" />
    <appendToFile value=""true"" />
    <rollingStyle value=""Size"" />
    <maximumFileSize value=""5MB"" />
    <maxSizeRollBackups value=""5"" />
    <staticLogFileName value=""true"" />
    <layout type=""log4net.Layout.PatternLayout"">
      <conversionPattern value=""%date{{dd MMM yyyy HH:mm:ss,fff}} %-5level | %message (%logger)%newline%exception"" />
    </layout>
  </appender>
  <root>
    <level value=""{rootLevel}"" />
    <appender-ref ref=""TraceAppender"" />
    <appender-ref ref=""RollingFileAppender"" />
  </root>
</log4net>";

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(log4netConfig));
        XmlConfigurator.Configure(stream);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        try
        {
            RunStartup();
        }
        catch (Exception ex)
        {
            log.Error("OnStartup", ex);
            throw;
        }
    }

    private void RunStartup()
    {
        _settingsProvider = new SettingsProvider();
        Preferences.User = _settingsProvider.Settings;
        Preferences.SetSettingsProvider(_settingsProvider);
        Helpers.ColorThemeHelper.ApplyCurrentColors();

        ApplyLanguage();

        _sqliteStore = SQLiteStore.Instance(Path.Combine(Globals.AppDataFolder, "db", "EDEA.db"));
        var fileWatcher = EDFileWatcher.Instance();
        var journalStore = JournalStore.Instance(fileWatcher);
        var historyProvider = HistoryProvider.Instance(_sqliteStore);
        _starSystemProvider = StarSystemProvider.Instance(historyProvider);
        var webApiProvider = WebApiProvider.Instance(httpClient, _starSystemProvider);
        var journalProvider = JournalProvider.Instance(journalStore, _starSystemProvider);
        var routeProvider = RouteProvider.Instance(fileWatcher, _starSystemProvider, journalProvider);
        var statusProvider = StatusProvider.Instance(fileWatcher, _starSystemProvider);
        var planetsOfInterestProvider = PlanetsOfInterestProvider.Instance(_starSystemProvider);
        var journalHistoryImporter = JournalHistoryImporter.Instance(historyProvider, journalProvider, _starSystemProvider);
        var hotkeyProvider = HotkeyProvider.Instance(_starSystemProvider);

        var viewModel = new MainViewModel(
            hotkeyProvider,
            webApiProvider,
            _starSystemProvider,
            historyProvider,
            routeProvider,
            statusProvider,
            planetsOfInterestProvider,
            journalHistoryImporter);

        var mainWindow = new MainWindow { DataContext = viewModel, Name = "MainWindow" };
        mainWindow.Closed += viewModel.OnMainWindowClosed;
        mainWindow.Show();
        mainWindow.Activate();
        JotSettingsProvider.Tracker.Track(mainWindow);

        viewModel.RestoreWindows();
        journalProvider.Initialize();

        try
        {
            hotkeyProvider.AttachHotkeyListener(viewModel, mainWindow);
            viewModel.RegisterHotkeys(hotkeyProvider);
        }
        catch (Exception ex)
        {
            log.Error("Hotkey registration failed", ex);
        }

    }

    private void ApplyLanguage()
    {
        var supported = new[] { "en", "de", "es", "fr", "ru", "pt-BR" };
        var setting = Preferences.Application.Language;
        var uiCulture = CultureInfo.CurrentUICulture;
        CultureInfo culture;

        if (string.IsNullOrEmpty(setting) || setting.Equals("Auto", StringComparison.OrdinalIgnoreCase))
        {
            var match = supported.FirstOrDefault(s =>
                uiCulture.Name.Equals(s, StringComparison.OrdinalIgnoreCase) ||
                uiCulture.TwoLetterISOLanguageName.Equals(s, StringComparison.OrdinalIgnoreCase));
            culture = match != null ? new CultureInfo(match) : new CultureInfo("en");
        }
        else
        {
            culture = supported.Any(s => setting.Equals(s, StringComparison.OrdinalIgnoreCase))
                ? new CultureInfo(setting)
                : new CultureInfo("en");
        }

        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        global::EDEA.Properties.Resources.Culture = culture;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _starSystemProvider?.HandleApplicationShutdown();
        base.OnExit(e);
    }

    private static void OnDispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
    {
        log.Error("An unhandled exception occurred!", e.Exception);
        try
        {
            MessageBox.Show(e.Exception.ToString(), global::EDEA.Properties.Resources.MessageBoxTitle_Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch
        {
            // MessageBox can fail too
        }
        e.Handled = true;
        Current?.Shutdown(1);
    }

    private static void OnCurrentDomainUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
            log.Error("AppDomainUnhandled", ex);
    }
}
