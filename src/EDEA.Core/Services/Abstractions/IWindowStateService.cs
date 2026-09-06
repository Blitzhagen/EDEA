namespace EDEA.Services;

/// <summary>
/// Abstraction for saving and restoring window geometry and state.
/// </summary>
public interface IWindowStateService
{
    /// <summary>
    /// Starts tracking the specified window and applies any previously saved state.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <param name="windowId">An identifier for the window.</param>
    void Track(object window, string windowId);

    /// <summary>
    /// Stops tracking the specified window and optionally persists its current state.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <param name="persist"><c>true</c> to persist the current state; otherwise, <c>false</c>.</param>
    void StopTracking(object window, bool persist = true);
}
