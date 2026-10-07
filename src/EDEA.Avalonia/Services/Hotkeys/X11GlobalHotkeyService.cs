using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Threading;
using EDEA.Avalonia.Platform;
using EDEA.Core.Input;
using EDEA.Enums;
using EDEA.Services;
using log4net;

namespace EDEA.Avalonia.Services.Hotkeys;

/// <summary>
/// X11 implementation of <see cref="IGlobalHotkeyService"/> using <c>XGrabKey</c> passive grabs.
/// Also works on Wayland sessions through XWayland: while an X11/XWayland window (e.g. Elite
/// Dangerous running via Proton) has focus, the grabs are delivered to this connection.
/// </summary>
internal sealed class X11GlobalHotkeyService : IGlobalHotkeyService, IDisposable
{
    /// <summary>
    /// The logger.
    /// </summary>
    private static readonly ILog Log = LogManager.GetLogger(typeof(X11GlobalHotkeyService));

    /// <summary>
    /// The X error handler. Static so the native callback stays alive for the process lifetime.
    /// </summary>
    private static readonly X11Native.XErrorHandler ErrorHandlerDelegate = HandleXError;

    /// <summary>
    /// The last X error code observed (reset before each batch of requests).
    /// </summary>
    private static volatile int _lastXError;

    /// <summary>
    /// Guards all mutable state below.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// Maps (keycode, normalized modifier mask) to the registered hotkey identifier.
    /// </summary>
    private readonly Dictionary<(uint Keycode, uint Modifiers), HotkeyId> _grabToHotkeyId = new();

    /// <summary>
    /// Maps hotkey identifier to its grabbed (keycode, base modifier mask) for ungrabbing.
    /// </summary>
    private readonly Dictionary<HotkeyId, (uint Keycode, uint Modifiers)> _registeredGrabs = new();

    /// <summary>
    /// Our own X11 display connection (separate from Avalonia's).
    /// </summary>
    private IntPtr _display;

    /// <summary>
    /// The root window the passive grabs are established on.
    /// </summary>
    private ulong _rootWindow;

    /// <summary>
    /// The background thread reading X events.
    /// </summary>
    private Thread? _eventThread;

    /// <summary>
    /// Whether the service is attached and listening.
    /// </summary>
    private volatile bool _running;

    /// <inheritdoc />
    public event Action<HotkeyId>? HotkeyPressed;

    /// <inheritdoc />
    public void Attach(nint windowHandle)
    {
        lock (_lock)
        {
            if (_display != IntPtr.Zero)
            {
                return;
            }

            try
            {
                X11Native.XInitThreads();
                _display = X11Native.XOpenDisplay(null);
                if (_display == IntPtr.Zero)
                {
                    Log.Info("X11 global hotkeys unavailable: no X11 display (DISPLAY not set). " +
                             "Wayland-only desktops may still be covered by the portal service.");
                    return;
                }

                X11Native.XSetErrorHandler(ErrorHandlerDelegate);
                _rootWindow = X11Native.XDefaultRootWindow(_display);
                _running = true;
                _eventThread = new Thread(EventLoop)
                {
                    IsBackground = true,
                    Name = "EDEA X11 hotkey listener"
                };
                _eventThread.Start();
                Log.Info("X11 global hotkey service initialized.");
            }
            catch (Exception ex)
            {
                Log.Warn("X11 global hotkey service initialization failed", ex);
                if (_display != IntPtr.Zero)
                {
                    X11Native.XCloseDisplay(_display);
                    _display = IntPtr.Zero;
                }
            }
        }
    }

    /// <inheritdoc />
    public void Detach()
    {
        lock (_lock)
        {
            UnregisterAll();
            _running = false;
        }

        // Wait for the event loop to finish touching the display before closing it.
        _eventThread?.Join(2000);
        _eventThread = null;

        lock (_lock)
        {
            if (_display != IntPtr.Zero)
            {
                X11Native.XCloseDisplay(_display);
                _display = IntPtr.Zero;
            }
        }
    }

    /// <inheritdoc />
    public void Register(HotkeyId id, ModifierKeys modifierKeys, Key key)
    {
        if (key == Key.None || _registeredGrabs.ContainsKey(id))
        {
            return;
        }

        var keySymName = KeySymMap.GetKeySymName(key);
        if (keySymName == null)
        {
            Log.Warn($"Cannot register X11 hotkey '{id}': key '{key}' cannot be mapped to a keysym");
            return;
        }

        var display = _display;
        if (display == IntPtr.Zero)
        {
            return;
        }

        var keysym = X11Native.XStringToKeysym(keySymName);
        if (keysym == 0)
        {
            Log.Warn($"Cannot register X11 hotkey '{id}': unknown keysym name '{keySymName}'");
            return;
        }

        var keycode = X11Native.XKeysymToKeycode(display, keysym);
        if (keycode == 0)
        {
            Log.Warn($"Cannot register X11 hotkey '{id}': no keycode for '{keySymName}' in the current keyboard layout");
            return;
        }

        uint baseModifiers = ToX11Modifiers(modifierKeys);
        bool grabbed = false;
        _lastXError = 0;
        lock (_lock)
        {
            foreach (var combo in GetLockVariants(baseModifiers))
            {
                X11Native.XGrabKey(display, keycode, combo, _rootWindow, 0,
                    X11Native.GrabModeAsync, X11Native.GrabModeAsync);
            }

            X11Native.XSync(display, 0);
            grabbed = _lastXError == 0;
            if (grabbed)
            {
                _grabToHotkeyId[(keycode, baseModifiers)] = id;
                _registeredGrabs[id] = (keycode, baseModifiers);
            }
        }

        if (!grabbed)
        {
            Log.Warn($"X11 grab for hotkey '{id}' failed (X error {_lastXError}) - the combination is probably already grabbed by another application");
        }
    }

    /// <inheritdoc />
    public void Unregister(HotkeyId id)
    {
        var display = _display;
        if (display == IntPtr.Zero)
        {
            _registeredGrabs.Remove(id);
            return;
        }

        lock (_lock)
        {
            if (!_registeredGrabs.TryGetValue(id, out var grab))
            {
                return;
            }

            foreach (var combo in GetLockVariants(grab.Modifiers))
            {
                X11Native.XUngrabKey(display, (int)grab.Keycode, combo, _rootWindow);
            }

            _grabToHotkeyId.Remove((grab.Keycode, grab.Modifiers));
            _registeredGrabs.Remove(id);
        }
    }

    /// <inheritdoc />
    public void UnregisterAll()
    {
        var display = _display;
        lock (_lock)
        {
            if (display != IntPtr.Zero)
            {
                foreach (var grab in _registeredGrabs.Values)
                {
                    foreach (var combo in GetLockVariants(grab.Modifiers))
                    {
                        X11Native.XUngrabKey(display, (int)grab.Keycode, combo, _rootWindow);
                    }
                }
            }

            _grabToHotkeyId.Clear();
            _registeredGrabs.Clear();
        }
    }

    /// <inheritdoc />
    public void Dispose() => Detach();

    /// <summary>
    /// Enumerates the given modifier mask in all CapsLock/NumLock combinations,
    /// so a grabbed hotkey fires regardless of lock states.
    /// </summary>
    private static IEnumerable<uint> GetLockVariants(uint baseModifiers)
    {
        yield return baseModifiers;
        yield return baseModifiers | X11Native.LockMask;
        yield return baseModifiers | X11Native.Mod2Mask;
        yield return baseModifiers | X11Native.LockMask | X11Native.Mod2Mask;
    }

    /// <summary>
    /// Converts <see cref="ModifierKeys"/> to X11 modifier masks.
    /// </summary>
    private static uint ToX11Modifiers(ModifierKeys modifierKeys)
    {
        uint modifiers = 0;
        if ((modifierKeys & ModifierKeys.Shift) != 0)
        {
            modifiers |= X11Native.ShiftMask;
        }

        if ((modifierKeys & ModifierKeys.Control) != 0)
        {
            modifiers |= X11Native.ControlMask;
        }

        if ((modifierKeys & ModifierKeys.Alt) != 0)
        {
            modifiers |= X11Native.Mod1Mask;
        }

        if ((modifierKeys & ModifierKeys.Windows) != 0)
        {
            modifiers |= X11Native.Mod4Mask;
        }

        return modifiers;
    }

    /// <summary>
    /// Polls the X connection for grabbed key press events.
    /// </summary>
    private void EventLoop()
    {
        var ev = new X11Native.XEventUnion();
        while (_running)
        {
            var display = _display;
            if (display == IntPtr.Zero)
            {
                break;
            }

            while (X11Native.XPending(display) > 0)
            {
                if (X11Native.XNextEvent(display, ref ev) != 0)
                {
                    continue;
                }

                if (ev.Type == X11Native.KeyPress)
                {
                    HandleKeyPress(ev.KeyCode, ev.KeyState);
                }
            }

            Thread.Sleep(20);
        }
    }

    /// <summary>
    /// Maps a grabbed key press back to the hotkey identifier and raises <see cref="HotkeyPressed"/>.
    /// </summary>
    private void HandleKeyPress(uint keycode, uint state)
    {
        HotkeyId id;
        lock (_lock)
        {
            if (!_grabToHotkeyId.TryGetValue((keycode, state & X11Native.HotkeyModifierMask), out id))
            {
                return;
            }
        }

        var handler = HotkeyPressed;
        if (handler != null)
        {
            Dispatcher.UIThread.Post(() => handler(id));
        }
    }

    /// <summary>
    /// X error callback. Records the error code instead of aborting the process
    /// (the default X behavior on e.g. BadAccess for conflicting grabs).
    /// </summary>
    private static int HandleXError(IntPtr display, ref X11Native.XErrorEvent errorEvent)
    {
        _lastXError = errorEvent.ErrorCode;
        Log.Debug($"X11 error {errorEvent.ErrorCode} (request {errorEvent.RequestCode})");
        return 0;
    }
}
