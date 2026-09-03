using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EDEA.Avalonia.Services;
using EDEA.Avalonia.Views;
using EDEA.Services;
using EDEA.Stores;
using log4net;

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
    /// Shared <see cref="HttpClient"/> used for web API calls.
    /// </summary>
    private static readonly HttpClient httpClient = new();

    /// <summary>
    /// Static constructor that configures the shared <see cref="HttpClient"/>.
    /// </summary>
    static App()
    {
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd($"EDEA/{Assembly.GetExecutingAssembly().GetName().Version}");
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
                var mainWindow = new MainWindow();
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
    private static void RunStartup()
    {
        var fileWatcher = EDFileWatcher.Instance();
        var journalStore = JournalStore.Instance(fileWatcher);
        var sqliteStore = SQLiteStore.Instance(Path.Combine(Globals.AppDataFolder, "db", "EDEA.db"));
        var historyProvider = HistoryProvider.Instance(sqliteStore);
        var starSystemProvider = StarSystemProvider.Instance(historyProvider);
        var webApiProvider = WebApiProvider.Instance(httpClient, starSystemProvider);
        var journalProvider = JournalProvider.Instance(journalStore, starSystemProvider);
        var routeProvider = RouteProvider.Instance(fileWatcher, starSystemProvider, journalProvider);
        var statusProvider = StatusProvider.Instance(fileWatcher, starSystemProvider);
        var planetsOfInterestProvider = PlanetsOfInterestProvider.Instance(starSystemProvider);
        var journalHistoryImporter = JournalHistoryImporter.Instance(historyProvider, journalProvider, starSystemProvider);

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
