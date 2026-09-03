using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using EDEA.Core.Input;
using EDEA.Enums;
using EDEA.Models;
using EDEA.Services;
using log4net;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF implementation of <see cref="IGlobalHotkeyService"/> using user32 RegisterHotKey.
/// </summary>
public sealed class WindowsGlobalHotkeyService : IGlobalHotkeyService
{
    private const int WM_HOTKEY = 0x0312;
    private const int HotkeyBaseId = 171701;

    private static readonly ILog log = LogManager.GetLogger(typeof(WindowsGlobalHotkeyService));

    private nint _windowHandle;
    private HwndSource? _hwndSource;

    /// <summary>
    /// Occurs when a registered hotkey is pressed.
    /// </summary>
    public event Action<HotkeyId>? HotkeyPressed;

    /// <summary>
    /// Attaches the service to the specified native window handle.
    /// </summary>
    /// <param name="windowHandle">The native window handle.</param>
    public void Attach(nint windowHandle)
    {
        Detach();
        _windowHandle = windowHandle;
        _hwndSource = HwndSource.FromHwnd(windowHandle);
        _hwndSource?.AddHook(WndProc);
    }

    /// <summary>
    /// Detaches the service from the current window and unregisters all hotkeys.
    /// </summary>
    public void Detach()
    {
        UnregisterAll();
        if (_hwndSource is not null)
        {
            try
            {
                _hwndSource.RemoveHook(WndProc);
            }
            catch (Exception exception)
            {
                log.Error("Error removing global hotkey window hook", exception);
            }
            _hwndSource = null;
        }
        _windowHandle = 0;
    }

    /// <summary>
    /// Registers a global hotkey.
    /// </summary>
    /// <param name="id">The hotkey identifier.</param>
    /// <param name="modifierKeys">The modifier keys.</param>
    /// <param name="key">The key.</param>
    public void Register(HotkeyId id, ModifierKeys modifierKeys, Key key)
    {
        if (_windowHandle == 0)
        {
            log.Error("Cannot register hotkey: no window handle attached");
            return;
        }

        try
        {
            Unregister(id);
            uint modifiers = (uint)(System.Windows.Input.ModifierKeys)modifierKeys;
            uint virtualKey = (uint)System.Windows.Input.KeyInterop.VirtualKeyFromKey((System.Windows.Input.Key)key);
            if (!RegisterHotKey(_windowHandle, (int)id, modifiers, virtualKey))
            {
                log.Error($"RegisterHotKey failed for hotkey {id} with modifiers {modifiers} and virtual key {virtualKey}");
            }
        }
        catch (Exception exception)
        {
            log.Error($"Could not register hotkey {id}", exception);
        }
    }

    /// <summary>
    /// Unregisters the specified global hotkey.
    /// </summary>
    /// <param name="id">The hotkey identifier.</param>
    public void Unregister(HotkeyId id)
    {
        if (_windowHandle == 0)
        {
            return;
        }

        try
        {
            UnregisterHotKey(_windowHandle, (int)id);
        }
        catch (Exception exception)
        {
            log.Error($"Could not unregister hotkey {id}", exception);
        }
    }

    /// <summary>
    /// Unregisters all global hotkeys.
    /// </summary>
    public void UnregisterAll()
    {
        if (_windowHandle == 0)
        {
            return;
        }

        foreach (HotkeyId id in Enum.GetValues<HotkeyId>())
        {
            try
            {
                UnregisterHotKey(_windowHandle, (int)id);
            }
            catch (Exception exception)
            {
                log.Error($"Could not unregister hotkey {id}", exception);
            }
        }
    }

    /// <summary>
    /// Window procedure hook that listens for WM_HOTKEY messages.
    /// </summary>
    private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY)
        {
            int id = wParam.ToInt32();
            if (Enum.IsDefined(typeof(HotkeyId), id))
            {
                log.Debug($"Hotkey press event with id: {id}");
                HotkeyPressed?.Invoke((HotkeyId)id);
                handled = true;
            }
        }
        return IntPtr.Zero;
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vlc);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(nint hWnd, int id);
}
