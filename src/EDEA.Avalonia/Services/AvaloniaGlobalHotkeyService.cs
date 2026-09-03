using System;
using EDEA.Core.Input;
using EDEA.Enums;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia/cross-platform stub implementation of <see cref="IGlobalHotkeyService"/>.
/// </summary>
/// <remarks>
/// Avalonia has no built-in global hotkey support. A platform-specific implementation
/// using RegisterHotKey on Windows or X11 grabs on Linux can be added later.
/// </remarks>
public sealed class AvaloniaGlobalHotkeyService : IGlobalHotkeyService
{
#pragma warning disable CS0067
    /// <inheritdoc />
    public event Action<HotkeyId>? HotkeyPressed;
#pragma warning restore CS0067

    /// <inheritdoc />
    public void Attach(nint windowHandle)
    {
    }

    /// <inheritdoc />
    public void Detach()
    {
    }

    /// <inheritdoc />
    public void Register(HotkeyId id, ModifierKeys modifierKeys, Key key)
    {
    }

    /// <inheritdoc />
    public void Unregister(HotkeyId id)
    {
    }

    /// <inheritdoc />
    public void UnregisterAll()
    {
    }
}
