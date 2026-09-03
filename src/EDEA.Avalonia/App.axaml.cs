using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EDEA.Avalonia.Services;
using EDEA.Avalonia.Views;
using EDEA.Services;
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
