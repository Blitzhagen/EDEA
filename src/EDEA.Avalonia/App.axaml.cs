using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EDEA;
using EDEA.Avalonia.Services;
using EDEA.Avalonia.ViewModels;
using EDEA.Avalonia.Views;
using EDEA.Services;
using EDEA.Stores;
using log4net;
using log4net.Appender;
using log4net.Config;

namespace EDEA.Avalonia;

/// <summary>
/// Avalonia application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Logger for the application.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(App));

    /// <summary>
    /// The settings provider.
    /// </summary>
    private SettingsProvider? _settingsProvider;

    /// <summary>
    /// The star system provider.
    /// </summary>
    private StarSystemProvider? _starSystemProvider;

    /// <summary>
    /// The history provider.
    /// </summary>
    private HistoryProvider? _historyProvider;

    /// <summary>
    /// The route provider.
    /// </summary>
    private RouteProvider? _routeProvider;

    /// <summary>
    /// Shared <see cref="HttpClient"/> used for web API calls.
    /// </summary>
    private static readonly HttpClient httpClient = new();

    /// <summary>
    /// Static constructor that configures the shared <see cref="HttpClient"/> and logging.
    /// </summary>
    static App()
    {
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd($"EDEA/{Assembly.GetExecutingAssembly().GetName().Version}");
        ConfigureLog4Net();
    }

    /// <summary>
    /// Configures log4net from an embedded configuration string.
    /// </summary>
    private static void ConfigureLog4Net()
    {
        string logFileFullPath = Path.Combine(Globals.AppDataFolder, "log", "EDEA.Avalonia.log");
        Directory.CreateDirectory(Path.GetDirectoryName(logFileFullPath)!);
        GlobalContext.Properties["LogFileFullPath"] = logFileFullPath;

        string log4netConfig = $@"
<log4net>
  <appender name=""TraceAppender"" type=""log4net.Appender.TraceAppender"">
    <layout type=""log4net.Layout.PatternLayout"">
      <conversionPattern value=""%date{{HH:mm:ss,fff}} %-5level | %message (%logger)%newline%exception"" />
    </layout>
  </appender>
  <appender name=""RollingFileAppender"" type=""log4net.Appender.RollingFileAppender"">
    <filter type=""log4net.Filter.LevelRangeFilter"">
      <levelMin value=""DEBUG"" />
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
    <level value=""DEBUG"" />
    <appender-ref ref=""TraceAppender"" />
    <appender-ref ref=""RollingFileAppender"" />
  </root>
</log4net>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(log4netConfig));
        XmlConfigurator.Configure(stream);
    }

    /// <summary>
    /// Initializes the XAML components.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Called when the application framework has been initialized.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            RegisterPlatformServices();
            InitializeSettings();
            RunStartup();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow { DataContext = new MainViewModel(_starSystemProvider!, _historyProvider!, _routeProvider!) };
                PlatformServices.WindowState?.Track(mainWindow, "MainWindow");
                desktop.MainWindow = mainWindow;
                mainWindow.Show();
            }
        }
        catch (Exception exception)
        {
            log.Error("Application initialization failed", exception);
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Initializes the settings provider, language and color theme.
    /// </summary>
    private void InitializeSettings()
    {
        _settingsProvider = new SettingsProvider();
        Preferences.SetSettingsProvider(_settingsProvider);
        ApplyLanguage();
        PlatformServices.ColorTheme?.ApplyCurrentColors();
    }

    /// <summary>
    /// Creates and wires up all application providers.
    /// </summary>
    private void RunStartup()
    {
        log.Info("################# Application started #################");
        log.Info($"ED saved game path: {Preferences.Other.EdSavedGamePath}");
        log.Info($"App data folder: {Globals.AppDataFolder}");

        var fileWatcher = EDFileWatcher.Instance();
        var journalStore = JournalStore.Instance(fileWatcher);
        var sqliteStore = SQLiteStore.Instance(Path.Combine(Globals.AppDataFolder, "db", "EDEA.db"));
        _historyProvider = HistoryProvider.Instance(sqliteStore);
        _starSystemProvider = StarSystemProvider.Instance(_historyProvider);
        var webApiProvider = WebApiProvider.Instance(httpClient, _starSystemProvider);
        var journalProvider = JournalProvider.Instance(journalStore, _starSystemProvider);
        _routeProvider = RouteProvider.Instance(fileWatcher, _starSystemProvider, journalProvider);
        var statusProvider = StatusProvider.Instance(fileWatcher, _starSystemProvider);
        var planetsOfInterestProvider = PlanetsOfInterestProvider.Instance(_starSystemProvider);
        var journalHistoryImporter = JournalHistoryImporter.Instance(_historyProvider, journalProvider, _starSystemProvider);

        journalProvider.Initialize();
    }

    /// <summary>
    /// Sets the current UI and thread culture based on the configured language.
    /// </summary>
    private static void ApplyLanguage()
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
        EDEA.Properties.Resources.Culture = culture;
    }

    /// <summary>
    /// Registers Avalonia-specific platform service implementations.
    /// </summary>
    private static void RegisterPlatformServices()
    {
        PlatformServices.Dispatcher = new AvaloniaDispatcher();
        PlatformServices.Clipboard = new AvaloniaClipboardService();
        PlatformServices.Speech = new AvaloniaSpeechService();
        PlatformServices.Dialog = new AvaloniaDialogService();
        PlatformServices.Platform = new AvaloniaPlatformService();
        PlatformServices.HudWindow = new AvaloniaHudWindowService();
        PlatformServices.WindowState = new AvaloniaWindowStateService();
        PlatformServices.ColorTheme = new AvaloniaColorThemeService();
        PlatformServices.UiTimer = new AvaloniaUiTimerService();
        PlatformServices.Screen = new AvaloniaScreenService();
        PlatformServices.GlobalHotkey = new AvaloniaGlobalHotkeyService();
    }
}
