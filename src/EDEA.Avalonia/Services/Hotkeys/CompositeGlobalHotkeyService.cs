using System;
using System.Collections.Generic;
using EDEA.Core.Input;
using EDEA.Enums;
using EDEA.Services;

namespace EDEA.Avalonia.Services.Hotkeys;

/// <summary>
/// Combines multiple <see cref="IGlobalHotkeyService"/> implementations (X11 grabs and the
/// XDG portal) into one. A press coming through both mechanisms within a short window is
/// de-duplicated so each physical key press raises <see cref="HotkeyPressed"/> only once.
/// </summary>
internal sealed class CompositeGlobalHotkeyService : IGlobalHotkeyService, IDisposable
{
    /// <summary>
    /// Time window in which a second event for the same hotkey is treated as a duplicate.
    /// </summary>
    private static readonly TimeSpan DedupWindow = TimeSpan.FromMilliseconds(300);

    /// <summary>
    /// The composed services.
    /// </summary>
    private readonly IReadOnlyList<IGlobalHotkeyService> _services;

    /// <summary>
    /// Guards the de-duplication state below.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// The last hotkey that fired.
    /// </summary>
    private HotkeyId _lastId;

    /// <summary>
    /// The timestamp of the last fired hotkey.
    /// </summary>
    private DateTime _lastTimestamp;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeGlobalHotkeyService"/> class.
    /// </summary>
    public CompositeGlobalHotkeyService(params IGlobalHotkeyService[] services)
    {
        _services = services;
        foreach (var service in _services)
        {
            service.HotkeyPressed += OnInnerHotkeyPressed;
        }
    }

    /// <inheritdoc />
    public event Action<HotkeyId>? HotkeyPressed;

    /// <inheritdoc />
    public void Attach(nint windowHandle)
    {
        foreach (var service in _services)
        {
            service.Attach(windowHandle);
        }
    }

    /// <inheritdoc />
    public void Detach()
    {
        foreach (var service in _services)
        {
            service.Detach();
        }
    }

    /// <inheritdoc />
    public void Register(HotkeyId id, ModifierKeys modifierKeys, Key key)
    {
        foreach (var service in _services)
        {
            service.Register(id, modifierKeys, key);
        }
    }

    /// <inheritdoc />
    public void Unregister(HotkeyId id)
    {
        foreach (var service in _services)
        {
            service.Unregister(id);
        }
    }

    /// <inheritdoc />
    public void UnregisterAll()
    {
        foreach (var service in _services)
        {
            service.UnregisterAll();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var service in _services)
        {
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Forwards a hotkey press from an inner service, suppressing duplicates within
    /// <see cref="DedupWindow"/>.
    /// </summary>
    private void OnInnerHotkeyPressed(HotkeyId id)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            if (id == _lastId && now - _lastTimestamp < DedupWindow)
            {
                return;
            }

            _lastId = id;
            _lastTimestamp = now;
        }

        HotkeyPressed?.Invoke(id);
    }
}
