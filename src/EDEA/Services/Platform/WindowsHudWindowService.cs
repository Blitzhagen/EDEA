using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using EDEA.Services;
using log4net;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF implementation of <see cref="IHudWindowService"/>.
/// </summary>
public sealed class WindowsHudWindowService : IHudWindowService
{
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int GWL_EXSTYLE = -20;

    private static readonly ILog log = LogManager.GetLogger(typeof(WindowsHudWindowService));

    /// <summary>
    /// The original extended window style captured when click-through was enabled.
    /// </summary>
    private int _originalExtendedStyle;

    /// <summary>
    /// Gets the native window handle for the specified window.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <returns>The native window handle.</returns>
    public nint GetWindowHandle(object window)
    {
        if (window is Window wpfWindow)
        {
            return new WindowInteropHelper(wpfWindow).Handle;
        }
        return 0;
    }

    /// <summary>
    /// Enables or disables click-through mouse input for the specified window.
    /// </summary>
    /// <param name="windowHandle">The native window handle.</param>
    /// <param name="enabled"><c>true</c> to enable click-through; <c>false</c> to disable it.</param>
    public void SetClickThrough(nint windowHandle, bool enabled)
    {
        if (windowHandle == 0)
        {
            return;
        }

        try
        {
            if (enabled)
            {
                _originalExtendedStyle = GetWindowLong(windowHandle, GWL_EXSTYLE);
                SetWindowLong(windowHandle, GWL_EXSTYLE, _originalExtendedStyle | WS_EX_TRANSPARENT);
            }
            else if (_originalExtendedStyle != 0)
            {
                SetWindowLong(windowHandle, GWL_EXSTYLE, _originalExtendedStyle);
            }
        }
        catch (Exception exception)
        {
            log.Error("Error setting HUD window click-through state", exception);
        }
    }

    /// <summary>
    /// Starts a window drag operation.
    /// </summary>
    /// <param name="window">The window object.</param>
    public void BeginDrag(object window)
    {
        if (window is Window wpfWindow && Mouse.LeftButton == MouseButtonState.Pressed)
        {
            wpfWindow.DragMove();
        }
    }

    /// <summary>
    /// Sets the cursor to the move cursor.
    /// </summary>
    public void SetMoveCursor()
    {
        Mouse.OverrideCursor = Cursors.SizeAll;
    }

    /// <summary>
    /// Resets the cursor to the default cursor.
    /// </summary>
    public void ResetCursor()
    {
        Mouse.OverrideCursor = null;
    }

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(nint hwnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(nint hwnd, int index, int newStyle);
}
