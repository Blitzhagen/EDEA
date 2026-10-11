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
    /// Makes the window render above fullscreen applications, if the platform
    /// supports it. Must be called before the window is shown. On X11 this marks
    /// the window as override-redirect so the compositor puts it in the
    /// unmanaged layer, which stacks above active fullscreen windows.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <returns><c>true</c> when the window was made unmanaged/override-redirect
    /// and therefore needs client-side move handling; otherwise <c>false</c>.</returns>
    bool SetAboveFullscreen(object window);

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
