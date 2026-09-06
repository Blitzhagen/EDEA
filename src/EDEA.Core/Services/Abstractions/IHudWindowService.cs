namespace EDEA.Services;

/// <summary>
/// Abstraction for platform-specific HUD window operations.
/// </summary>
public interface IHudWindowService
{
    /// <summary>
    /// Gets the native window handle for the specified window.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <returns>The native window handle.</returns>
    nint GetWindowHandle(object window);

    /// <summary>
    /// Enables or disables click-through mouse input for the specified window.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <param name="enabled"><c>true</c> to enable click-through; <c>false</c> to disable it.</param>
    void SetClickThrough(object window, bool enabled);

    /// <summary>
    /// Starts a window drag operation (equivalent to WPF DragMove).
    /// </summary>
    /// <param name="window">The window object.</param>
    void BeginDrag(object window);

    /// <summary>
    /// Sets the cursor to the move cursor.
    /// </summary>
    void SetMoveCursor();

    /// <summary>
    /// Resets the cursor to the default cursor.
    /// </summary>
    void ResetCursor();
}
