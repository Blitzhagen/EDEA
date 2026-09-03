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
}
