namespace EDEA.Services;

/// <summary>
/// Provides access to platform-specific services from platform-independent code.
/// This is a service locator that is populated by the concrete UI project (WPF/Avalonia) at startup.
/// </summary>
public static class PlatformServices
{
    /// <summary>
    /// Gets or sets the dispatcher used to marshal calls to the UI thread.
    /// </summary>
    public static IDispatcher? Dispatcher { get; set; }

    /// <summary>
    /// Gets or sets the clipboard service.
    /// </summary>
    public static IClipboardService? Clipboard { get; set; }

    /// <summary>
    /// Gets or sets the speech service.
    /// </summary>
    public static ISpeechService? Speech { get; set; }

    /// <summary>
    /// Gets or sets the dialog service.
    /// </summary>
    public static IDialogService? Dialog { get; set; }

    /// <summary>
    /// Gets or sets the platform service.
    /// </summary>
    public static IPlatformService? Platform { get; set; }

    /// <summary>
    /// Gets or sets the global hotkey service.
    /// </summary>
    public static IGlobalHotkeyService? GlobalHotkey { get; set; }

    /// <summary>
    /// Gets or sets the HUD window service.
    /// </summary>
    public static IHudWindowService? HudWindow { get; set; }

    /// <summary>
    /// Gets or sets the window state service.
    /// </summary>
    public static IWindowStateService? WindowState { get; set; }

    /// <summary>
    /// Gets or sets the color theme service.
    /// </summary>
    public static IColorThemeService? ColorTheme { get; set; }

    /// <summary>
    /// Gets or sets the UI timer service.
    /// </summary>
    public static IUiTimerService? UiTimer { get; set; }

    /// <summary>
    /// Gets or sets the screen service.
    /// </summary>
    public static IScreenService? Screen { get; set; }
}
