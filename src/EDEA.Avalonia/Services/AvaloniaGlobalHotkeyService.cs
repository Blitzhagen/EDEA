using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EDEA.Core.Input;
using EDEA.Enums;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IGlobalHotkeyService"/> for Windows.
/// </summary>
/// <remarks>
/// Registers system-wide hotkeys via the Win32 <c>RegisterHotKey</c> API.
/// Actual key press delivery (WM_HOTKEY dispatch) is not yet wired to Avalonia.
/// </remarks>
public sealed class AvaloniaGlobalHotkeyService : IGlobalHotkeyService
{
    /// <summary>
    /// Maps a hotkey identifier to the Win32 id used during registration.
    /// </summary>
    private readonly Dictionary<HotkeyId, int> _registeredIds = new();

    /// <summary>
    /// The native window handle to attach to.
    /// </summary>
    private nint _windowHandle;

    /// <summary>
    /// The next local hotkey id to assign.
    /// </summary>
    private int _nextId = 1;

    /// <inheritdoc />
#pragma warning disable CS0067
    public event Action<HotkeyId>? HotkeyPressed;
#pragma warning restore CS0067

    /// <inheritdoc />
    public void Attach(nint windowHandle)
    {
        _windowHandle = windowHandle;
    }

    /// <inheritdoc />
    public void Detach()
    {
        UnregisterAll();
        _windowHandle = 0;
    }

    /// <inheritdoc />
    public void Register(HotkeyId id, ModifierKeys modifierKeys, Key key)
    {
        if (_windowHandle == 0)
        {
            return;
        }

        if (_registeredIds.ContainsKey(id))
        {
            return;
        }

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        var localId = _nextId++;
        var modifiers = ToWin32Modifiers(modifierKeys);
        var virtualKey = ToVirtualKey(key);

        if (RegisterHotKey(_windowHandle, localId, modifiers, virtualKey))
        {
            _registeredIds[id] = localId;
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
    /// Converts an Avalonia-independent <see cref="Key"/> to a Win32 virtual-key code.
    /// </summary>
    private static uint ToVirtualKey(Key key)
    {
        return (uint)key;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(nint hWnd, int id);
}
