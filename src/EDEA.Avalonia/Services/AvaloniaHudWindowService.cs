using System;
using System.Collections.Generic;
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
    /// Holds the click-through state and callback for a HUD window.
    /// </summary>
    private sealed class ClickThroughState
    {
        /// <summary>
        /// Gets or sets a value indicating whether click-through is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the registered window-styles callback.
        /// </summary>
        public Win32Properties.CustomWindowStylesCallback? Callback { get; set; }
    }

    private static readonly Dictionary<Window, ClickThroughState> _clickThroughStates = new();

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
        if (window is not Window avaloniaWindow || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        if (!_clickThroughStates.TryGetValue(avaloniaWindow, out var state))
        {
            state = new ClickThroughState { Enabled = enabled };
            state.Callback = (style, exStyle) =>
            {
                const uint WS_EX_TRANSPARENT = 0x00000020;
                const uint WS_EX_LAYERED = 0x00080000;

                if (state.Enabled)
                {
                    exStyle |= WS_EX_TRANSPARENT | WS_EX_LAYERED;
                }
                else
                {
                    exStyle &= ~(WS_EX_TRANSPARENT | WS_EX_LAYERED);
                }

                return (style, exStyle);
            };

            _clickThroughStates[avaloniaWindow] = state;
            Win32Properties.AddWindowStylesCallback(avaloniaWindow, state.Callback);

            avaloniaWindow.Closed += (_, _) =>
            {
                if (_clickThroughStates.TryGetValue(avaloniaWindow, out var toRemove))
                {
                    if (toRemove.Callback != null)
                    {
                        Win32Properties.RemoveWindowStylesCallback(avaloniaWindow, toRemove.Callback);
                    }

                    _clickThroughStates.Remove(avaloniaWindow);
                }
            };
        }
        else
        {
            state.Enabled = enabled;
        }

        // Trigger Avalonia to recompute the Win32 window styles so the callback is applied.
        var originalCanResize = avaloniaWindow.CanResize;
        avaloniaWindow.SetValue(Window.CanResizeProperty, !originalCanResize);
        avaloniaWindow.SetValue(Window.CanResizeProperty, originalCanResize);

        var handle = GetWindowHandle(avaloniaWindow);
        if (handle != 0)
        {
            // Force Windows to re-evaluate the extended window style.
            SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);

            // Set a constant alpha of 255 so the layered window stays fully visible
            // while still honoring WS_EX_TRANSPARENT for click-through.
            SetLayeredWindowAttributes(handle, 0, 255, LWA_ALPHA);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetLayeredWindowAttributes(nint hWnd, uint crKey, byte bAlpha, uint dwFlags);

    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint LWA_ALPHA = 0x00000002;

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
