using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Platform;
using EDEA.Avalonia.Platform;
using EDEA.Services;
using log4net;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IHudWindowService"/>.
/// </summary>
public sealed class AvaloniaHudWindowService : IHudWindowService
{
    /// <summary>
    /// The logger.
    /// </summary>
    private static readonly ILog Log = LogManager.GetLogger(typeof(AvaloniaHudWindowService));

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
    /// <param name="window">The window object.</param>
    /// <param name="enabled"><c>true</c> to enable click-through; <c>false</c> to disable it.</param>
    public void SetClickThrough(object window, bool enabled)
    {
        if (window is not Window avaloniaWindow)
        {
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SetClickThroughWindows(avaloniaWindow, enabled);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            SetClickThroughX11(avaloniaWindow, enabled);
        }
    }

    /// <summary>
    /// Enables or disables click-through via the Win32 extended window style.
    /// </summary>
    private void SetClickThroughWindows(Window avaloniaWindow, bool enabled)
    {
        var handle = GetWindowHandle(avaloniaWindow);
        if (handle == 0)
        {
            return;
        }

        int exStyle = GetWindowLong(handle, GWL_EXSTYLE);
        if (enabled)
        {
            exStyle |= (int)(WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
        else
        {
            exStyle &= ~(int)WS_EX_TRANSPARENT;
        }

        SetWindowLong(handle, GWL_EXSTYLE, exStyle);

        // Constant alpha 255 means the per-pixel alpha from the layered surface is used unchanged.
        SetLayeredWindowAttributes(handle, 0, 255, LWA_ALPHA);

        // Force Windows to re-evaluate the extended window style.
        SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
    }

    /// <summary>
    /// Enables or disables click-through via the X11 XShape extension input region.
    /// Works on X11 sessions and on Wayland through XWayland (XWayland translates
    /// the input region to an empty wl_surface input region).
    /// </summary>
    private static void SetClickThroughX11(Window avaloniaWindow, bool enabled)
    {
        var platformHandle = avaloniaWindow.TryGetPlatformHandle();
        if (platformHandle == null || platformHandle.Handle == 0)
        {
            return;
        }

        // Only meaningful while Avalonia runs on its X11 backend (incl. XWayland).
        if (platformHandle.HandleDescriptor != "XID")
        {
            Log.Debug($"Skipping X11 click-through: platform handle is '{platformHandle.HandleDescriptor}'");
            return;
        }

        try
        {
            var display = X11Native.XOpenDisplay(null);
            if (display == IntPtr.Zero)
            {
                Log.Warn("Cannot apply HUD click-through: no X11 display available");
                return;
            }

            try
            {
                if (!X11Native.XShapeQueryExtension(display, out _, out _))
                {
                    Log.Warn("Cannot apply HUD click-through: XShape extension not available");
                    return;
                }

                ulong xid = (ulong)platformHandle.Handle;
                if (enabled)
                {
                    // Empty input region: all pointer events fall through to the window below.
                    X11Native.XShapeCombineRectangles(display, xid, X11Native.ShapeInput,
                        0, 0, IntPtr.Zero, 0, X11Native.ShapeSet, 0);
                }
                else
                {
                    // Reset to the default input region (whole window).
                    X11Native.XShapeCombineMask(display, xid, X11Native.ShapeInput,
                        0, 0, 0, X11Native.ShapeSet);
                }

                X11Native.XSync(display, 0);
            }
            finally
            {
                X11Native.XCloseDisplay(display);
            }
        }
        catch (Exception ex)
        {
            Log.Warn("Failed to apply X11 click-through", ex);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(nint hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetLayeredWindowAttributes(nint hWnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    private const int GWL_EXSTYLE = -20;
    private const uint WS_EX_TRANSPARENT = 0x00000020;
    private const uint WS_EX_LAYERED = 0x00080000;

    private const uint LWA_ALPHA = 0x00000002;

    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_FRAMECHANGED = 0x0020;

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
}
