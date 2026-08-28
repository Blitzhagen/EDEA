namespace EDEA.Enums;

/// <summary>
/// Identifies the hotkeys available in the application.
/// </summary>
public enum HotkeyId
{
    /// <summary>
    /// Toggles the visibility of the HUD window.
    /// </summary>
    ToggleHudWindow = 171701,

    /// <summary>
    /// Toggles the mouse pass-through state of the HUD window.
    /// </summary>
    ToggleHudMousePassThrough,

    /// <summary>
    /// Opens the route tab.
    /// </summary>
    OpenRouteTab,

    /// <summary>
    /// Opens the bodies tab.
    /// </summary>
    OpenBodiesTab,

    /// <summary>
    /// Opens the biologicals tab.
    /// </summary>
    OpenBiologicalsTab,

    /// <summary>
    /// Opens the surroundings tab.
    /// </summary>
    OpenSurroundingsTab,

    /// <summary>
    /// Opens the history tab.
    /// </summary>
    OpenHistoryTab,

    /// <summary>
    /// Attempts to copy the next system in the route to the clipboard.
    /// </summary>
    TryCopyNextSystemToClipboard,

    /// <summary>
    /// Stops the current speech output.
    /// </summary>
    QuitSpeechOutput
}
