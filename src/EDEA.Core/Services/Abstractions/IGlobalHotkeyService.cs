using System;
using EDEA.Core.Input;
using EDEA.Enums;
using EDEA.Models;

namespace EDEA.Services;

/// <summary>
/// Abstraction for registering and receiving global (system-wide) hotkeys.
/// </summary>
public interface IGlobalHotkeyService
{
    /// <summary>
    /// Occurs when a registered hotkey is pressed.
    /// </summary>
    event Action<HotkeyId>? HotkeyPressed;

    /// <summary>
    /// Attaches the service to the specified native window handle.
    /// </summary>
    /// <param name="windowHandle">The native window handle.</param>
    void Attach(nint windowHandle);

    /// <summary>
    /// Detaches the service from the current window and unregisters all hotkeys.
    /// </summary>
    void Detach();

    /// <summary>
    /// Registers a global hotkey.
    /// </summary>
    /// <param name="id">The hotkey identifier.</param>
    /// <param name="modifierKeys">The modifier keys.</param>
    /// <param name="key">The key.</param>
    void Register(HotkeyId id, ModifierKeys modifierKeys, Key key);

    /// <summary>
    /// Unregisters the specified global hotkey.
    /// </summary>
    /// <param name="id">The hotkey identifier.</param>
    void Unregister(HotkeyId id);

    /// <summary>
    /// Unregisters all global hotkeys.
    /// </summary>
    void UnregisterAll();
}
