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
using EDEA.Services.Platform;
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

/// <summary>
/// Main application class that configures logging, initializes providers and shows the main window.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Mutex that ensures only one application instance can run at a time.
    /// </summary>
    private static readonly Mutex singletonApp = new(true, "EDEA7062a413-faf0-406d-bae5-cddf0541e295");

    /// <summary>
    /// Shared <see cref="HttpClient"/> used for web API calls.
    /// </summary>
    private static readonly HttpClient httpClient = new();

    /// <summary>
    /// Logger for the application.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(App));

    /// <summary>
    /// Persistent SQLite data store.
    /// </summary>
    private SQLiteStore? _sqliteStore;

    /// <summary>
    /// Provider that manages application settings.
    /// </summary>
    private SettingsProvider? _settingsProvider;

    /// <summary>
    /// Provider that manages the current star system.
    /// </summary>
    private StarSystemProvider? _starSystemProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
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

        PlatformServices.Screen = new WindowsScreenService();
        var screenSize = PlatformServices.Screen.GetVirtualScreenSize();

        JotSettingsProvider.Tracker = new Tracker(new JsonFileStore(Path.Combine(Globals.AppDataFolder, "jot")));
        JotSettingsProvider.Tracker.Configure<Window>()
            .Id((Window w) => w.Name, new { W = screenSize.Width, H = screenSize.Height })
            .Properties((Window w) => new { w.Top, w.Width, w.Height, w.Left, w.WindowState })
            .PersistOn("Closing")
            .StopTrackingOn("Closing");

        PlatformServices.WindowState = new WindowsWindowStateService(JotSettingsProvider.Tracker);
        PlatformServices.ColorTheme = new WindowsColorThemeService();
        PlatformServices.UiTimer = new WindowsUiTimerService();

        if (Process.GetProcessesByName("EDMarketConnector").FirstOrDefault() == null)
        {
            log.Warn("Elite Dangerous Market Connector (EDMC) seems not be running. It is highly recommended to start EDMC");
        }

        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
    }

    /// <summary>
    /// Configures log4net from an embedded configuration string.
    /// </summary>
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

    /// <summary>
    /// Called when the application is starting.
    /// </summary>
    /// <param name="e">The startup event data.</param>
    /// <exception cref="Exception">Re-throws any exception that occurs during startup after logging it.</exception>
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

    /// <summary>
    /// Creates and wires up all application providers, then shows the main window.
    /// </summary>
    private void RunStartup()
    {
        PlatformServices.Dispatcher = new WindowsDispatcher();
        PlatformServices.Clipboard = new WindowsClipboardService();
        PlatformServices.Speech = new WindowsSpeechService();
        PlatformServices.Dialog = new WindowsDialogService();
        PlatformServices.Platform = new WindowsPlatformService();
        PlatformServices.GlobalHotkey = new WindowsGlobalHotkeyService();
        PlatformServices.HudWindow = new WindowsHudWindowService();

        _settingsProvider = new SettingsProvider();
        Preferences.User = _settingsProvider.Settings;
        Preferences.SetSettingsProvider(_settingsProvider);
        PlatformServices.ColorTheme?.ApplyCurrentColors();

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
        var hotkeyProvider = HotkeyProvider.Instance(_starSystemProvider, PlatformServices.GlobalHotkey!);

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
        PlatformServices.WindowState?.Track(mainWindow, "MainWindow");

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

    /// <summary>
    /// Sets the current UI and thread culture based on the configured language.
    /// </summary>
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

    /// <summary>
    /// Called when the application is exiting.
    /// </summary>
    /// <param name="e">The exit event data.</param>
    protected override void OnExit(ExitEventArgs e)
    {
        _starSystemProvider?.HandleApplicationShutdown();
        base.OnExit(e);
    }

    /// <summary>
    /// Logs and shows an error message for unhandled dispatcher exceptions.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The unhandled exception event data.</param>
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

    /// <summary>
    /// Logs unhandled AppDomain exceptions.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The unhandled exception event data.</param>
    private static void OnCurrentDomainUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
            log.Error("AppDomainUnhandled", ex);
    }
}
