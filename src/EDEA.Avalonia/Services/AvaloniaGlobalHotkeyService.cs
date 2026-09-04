using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia.Threading;
using EDEA.Core.Input;
using EDEA.Enums;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IGlobalHotkeyService"/> for Windows.
/// </summary>
/// <remarks>
/// Registers system-wide hotkeys via the Win32 <c>RegisterHotKey</c> API and
/// dispatches <c>WM_HOTKEY</c> messages by subclassing the attached window.
/// </remarks>
public sealed class AvaloniaGlobalHotkeyService : IGlobalHotkeyService
{
    private const uint WM_HOTKEY = 0x0312;

    /// <summary>
    /// Maps a hotkey identifier to the Win32 id used during registration.
    /// </summary>
    private readonly Dictionary<HotkeyId, int> _registeredIds = new();

    /// <summary>
    /// Maps a Win32 local hotkey id back to the application hotkey identifier.
    /// </summary>
    private readonly Dictionary<int, HotkeyId> _idToHotkeyId = new();

    /// <summary>
    /// The native window handle to attach to.
    /// </summary>
    private nint _windowHandle;

    /// <summary>
    /// The next local hotkey id to assign.
    /// </summary>
    private int _nextId = 1;

    /// <summary>
    /// The subclass callback delegate. Kept alive to prevent GC.
    /// </summary>
    private SubclassProc? _subclassProc;

    /// <inheritdoc />
    public event Action<HotkeyId>? HotkeyPressed;

    /// <inheritdoc />
    public void Attach(nint windowHandle)
    {
        if (_windowHandle != 0)
        {
            Detach();
        }

        _windowHandle = windowHandle;

        if (_windowHandle == 0 || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        _subclassProc = SubclassWndProc;
        SetWindowSubclass(_windowHandle, _subclassProc, 0, 0);
    }

    /// <inheritdoc />
    public void Detach()
    {
        if (_windowHandle != 0 && _subclassProc != null && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            UnregisterAll();
            RemoveWindowSubclass(_windowHandle, _subclassProc, 0);
        }

        _subclassProc = null;
        _windowHandle = 0;
    }

    /// <summary>
    /// Registers all valid hotkeys from the current preferences.
    /// </summary>
    public void RegisterAllHotkeys()
    {
        if (_windowHandle == 0 || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        UnregisterAll();

        foreach (var property in Preferences.Hotkeys.GetType().GetProperties())
        {
            if (property.PropertyType != typeof(EDEA.Models.Hotkey))
            {
                continue;
            }

            var hotkey = (EDEA.Models.Hotkey?)property.GetValue(Preferences.Hotkeys);
            if (hotkey is { IsValid: true })
            {
                Register(hotkey.Id, hotkey.Modifier, hotkey.Key);
            }
        }
    }

    /// <inheritdoc />
    public void Register(HotkeyId id, ModifierKeys modifierKeys, Key key)
    {
        if (_windowHandle == 0 || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        if (modifierKeys == ModifierKeys.None || key == Key.None)
        {
            return;
        }

        if (_registeredIds.ContainsKey(id))
        {
            return;
        }

        var localId = _nextId++;
        var modifiers = ToWin32Modifiers(modifierKeys);
        var virtualKey = ToVirtualKey(key);

        if (RegisterHotKey(_windowHandle, localId, modifiers, virtualKey))
        {
            _registeredIds[id] = localId;
            _idToHotkeyId[localId] = id;
        }
    }

    /// <inheritdoc />
    public void Unregister(HotkeyId id)
    {
        if (_registeredIds.TryGetValue(id, out var localId))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                UnregisterHotKey(_windowHandle, localId);
            }

            _registeredIds.Remove(id);
            _idToHotkeyId.Remove(localId);
        }
    }

    /// <inheritdoc />
    public void UnregisterAll()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            foreach (var localId in _registeredIds.Values)
            {
                UnregisterHotKey(_windowHandle, localId);
            }
        }

        _registeredIds.Clear();
        _idToHotkeyId.Clear();
    }

    /// <summary>
    /// Handles subclassed window messages.
    /// </summary>
    private nint SubclassWndProc(nint hWnd, uint uMsg, nint wParam, nint lParam, nint uIdSubclass, nint dwRefData)
    {
        if (uMsg == WM_HOTKEY)
        {
            var localId = (int)wParam;
            if (_idToHotkeyId.TryGetValue(localId, out var hotkeyId))
            {
                var handler = HotkeyPressed;
                if (handler != null)
                {
                    Dispatcher.UIThread.Post(() => handler(hotkeyId));
                }
            }
        }

        return DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    /// <summary>
    /// Converts <see cref="ModifierKeys"/> to Win32 modifiers.
    /// </summary>
    private static uint ToWin32Modifiers(ModifierKeys modifierKeys)
    {
        uint modifiers = 0;
        if ((modifierKeys & ModifierKeys.Control) != 0) modifiers |= 0x0002;
        if ((modifierKeys & ModifierKeys.Shift) != 0) modifiers |= 0x0004;
        if ((modifierKeys & ModifierKeys.Alt) != 0) modifiers |= 0x0001;
        if ((modifierKeys & ModifierKeys.Windows) != 0) modifiers |= 0x0008;
        return modifiers;
    }

    /// <summary>
    /// Converts a platform-independent <see cref="Key"/> to a Win32 virtual-key code.
    /// </summary>
    private static uint ToVirtualKey(Key key)
    {
        if (key == Key.None)
        {
            return 0;
        }

        return key switch
        {
            Key.Back => 0x08,
            Key.Tab => 0x09,
            Key.Enter => 0x0D,
            Key.Pause => 0x13,
            Key.Escape => 0x1B,
            Key.Space => 0x20,
            Key.PageUp => 0x21,
            Key.PageDown => 0x22,
            Key.End => 0x23,
            Key.Home => 0x24,
            Key.Left => 0x25,
            Key.Up => 0x26,
            Key.Right => 0x27,
            Key.Down => 0x28,
            Key.Insert => 0x2D,
            Key.Delete => 0x2E,
            Key.LWin => 0x5B,
            Key.RWin => 0x5C,
            Key.Apps => 0x5D,
            Key.Multiply => 0x6A,
            Key.Add => 0x6B,
            Key.Subtract => 0x6D,
            Key.Decimal => 0x6E,
            Key.Divide => 0x6F,
            Key.NumLock => 0x90,
            Key.Scroll => 0x91,
            Key.LeftShift => 0xA0,
            Key.RightShift => 0xA1,
            Key.LeftCtrl => 0xA2,
            Key.RightCtrl => 0xA3,
            Key.LeftAlt => 0xA4,
            Key.RightAlt => 0xA5,
            Key.OemSemicolon => 0xBA,
            Key.OemPlus => 0xBB,
            Key.OemComma => 0xBC,
            Key.OemMinus => 0xBD,
            Key.OemPeriod => 0xBE,
            Key.OemQuestion => 0xBF,
            Key.OemTilde => 0xC0,
            Key.OemOpenBrackets => 0xDB,
            Key.OemPipe => 0xDC,
            Key.OemCloseBrackets => 0xDD,
            Key.OemQuotes => 0xDE,
            Key.OemBackslash => 0xE2,
            Key.OemClear => 0xFE,
            var k when k >= Key.D0 && k <= Key.D9 => (uint)((int)k - 34 + 0x30),
            var k when k >= Key.A && k <= Key.Z => (uint)((int)k - 44 + 0x41),
            var k when k >= Key.NumPad0 && k <= Key.NumPad9 => (uint)((int)k - 74 + 0x60),
            var k when k >= Key.F1 && k <= Key.F12 => (uint)((int)k - 90 + 0x70),
            _ => 0,
        };
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate nint SubclassProc(nint hWnd, uint uMsg, nint wParam, nint lParam, nint uIdSubclass, nint dwRefData);

    [DllImport("comctl32.dll", SetLastError = true)]
    private static extern bool SetWindowSubclass(nint hWnd, SubclassProc pfnSubclass, nint uIdSubclass, nint dwRefData);

    [DllImport("comctl32.dll", SetLastError = true)]
    private static extern bool RemoveWindowSubclass(nint hWnd, SubclassProc pfnSubclass, nint uIdSubclass);

    [DllImport("comctl32.dll")]
    private static extern nint DefSubclassProc(nint hWnd, uint uMsg, nint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(nint hWnd, int id);
}
