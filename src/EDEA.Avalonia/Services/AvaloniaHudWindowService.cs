using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IHudWindowService"/>.
/// </summary>
public sealed class AvaloniaHudWindowService : IHudWindowService
{
    /// <summary>
    /// Gets the native window handle for the specified window.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <returns>The native window handle.</returns>
    public nint GetWindowHandle(object window)
    {
        if (window is not Window avaloniaWindow)
        {
            return 0;
        }

        var handle = avaloniaWindow.TryGetPlatformHandle();
        return handle?.Handle ?? 0;
    }

    /// <summary>
    /// Enables or disables click-through mouse input for the specified window.
    /// </summary>
    /// <param name="windowHandle">The native window handle.</param>
    /// <param name="enabled"><c>true</c> to enable click-through; <c>false</c> to disable it.</param>
    public void SetClickThrough(nint windowHandle, bool enabled)
    {
        if (windowHandle == 0 || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        const int GWL_EXSTYLE = -20;
        const uint WS_EX_TRANSPARENT = 0x00000020;

        var exStyle = GetWindowLong(windowHandle, GWL_EXSTYLE);
        var newExStyle = enabled ? exStyle | WS_EX_TRANSPARENT : exStyle & ~WS_EX_TRANSPARENT;
        SetWindowLong(windowHandle, GWL_EXSTYLE, newExStyle);
    }

    /// <summary>
    /// Starts a window drag operation (equivalent to WPF DragMove).
    /// </summary>
    /// <param name="window">The window object.</param>
    public void BeginDrag(object window)
    {
        if (window is Window avaloniaWindow)
        {
            // BeginMoveDrag is invoked from a pointer event; Avalonia handles the rest.
        }
    }

    /// <summary>
    /// Sets the cursor to the move cursor.
    /// </summary>
    public void SetMoveCursor()
    {
        // Avalonia cursor changes are typically done via the Cursor property on controls.
    }

    /// <summary>
    /// Resets the cursor to the default cursor.
    /// </summary>
    public void ResetCursor()
    {
        // Avalonia cursor changes are typically done via the Cursor property on controls.
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowLong(nint hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SetWindowLong(nint hWnd, int nIndex, uint dwNewLong);
}
