using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using EDEA.Core.Input;
using EDEA.Enums;
using EDEA.Services;
using log4net;
using Tmds.DBus.Protocol;

namespace EDEA.Avalonia.Services.Hotkeys;

/// <summary>
/// XDG Desktop Portal implementation of <see cref="IGlobalHotkeyService"/> using the
/// <c>org.freedesktop.portal.GlobalShortcuts</c> interface. This is the Wayland-native way to
/// receive global hotkeys; support depends on the compositor's portal backend (strong on KDE
/// Plasma 6.1+, limited or missing on GNOME and other desktops).
/// </summary>
internal sealed class PortalGlobalHotkeyService : IGlobalHotkeyService, IDisposable
{
    /// <summary>
    /// The logger.
    /// </summary>
    private static readonly ILog Log = LogManager.GetLogger(typeof(PortalGlobalHotkeyService));

    /// <summary>
    /// The portal service destination and object path.
    /// </summary>
    private const string PortalBusName = "org.freedesktop.portal.Desktop";
    private const string PortalObjectPath = "/org/freedesktop/portal/desktop";
    private const string GlobalShortcutsInterface = "org.freedesktop.portal.GlobalShortcuts";
    private const string RequestInterface = "org.freedesktop.portal.Request";
    private const string SessionInterface = "org.freedesktop.portal.Session";

    /// <summary>
    /// Debounce interval for batching Register/Unregister calls into one BindShortcuts call
    /// (the portal only allows one bind per session, so every change creates a fresh session).
    /// </summary>
    private static readonly TimeSpan BindDelay = TimeSpan.FromMilliseconds(300);

    /// <summary>
    /// Guards all mutable state below.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// The currently registered hotkeys by identifier.
    /// </summary>
    private readonly Dictionary<HotkeyId, (ModifierKeys Modifiers, Key Key)> _registered = new();

    /// <summary>
    /// Pending portal request completions, keyed by the predicted request object path.
    /// </summary>
    private readonly Dictionary<string, TaskCompletionSource<(uint Response, Dictionary<string, VariantValue> Results)>> _pendingRequests = new();

    /// <summary>
    /// The session bus connection.
    /// </summary>
    private DBusConnection? _connection;

    /// <summary>
    /// The subscription to Request.Response signals.
    /// </summary>
    private IDisposable? _requestObserver;

    /// <summary>
    /// The subscription to GlobalShortcuts.Activated signals.
    /// </summary>
    private IDisposable? _activatedObserver;

    /// <summary>
    /// The debounce timer for rebinds.
    /// </summary>
    private Timer? _bindTimer;

    /// <summary>
    /// The currently bound portal session handle, if any.
    /// </summary>
    private string? _sessionHandle;

    /// <summary>
    /// The X11 window id used as parent_window (0 if unavailable).
    /// </summary>
    private nint _windowHandle;

    /// <summary>
    /// Whether the registered set changed since the last successful bind.
    /// </summary>
    private bool _bindDirty;

    /// <summary>
    /// Whether a bind operation is currently in flight.
    /// </summary>
    private bool _bindInFlight;

    /// <summary>
    /// Counter used to generate unique portal handle tokens.
    /// </summary>
    private int _tokenCounter;

    /// <inheritdoc />
    public event Action<HotkeyId>? HotkeyPressed;

    /// <inheritdoc />
    public void Attach(nint windowHandle)
    {
        _windowHandle = windowHandle;
        _ = EnsureConnectedAsync();
    }

    /// <inheritdoc />
    public void Detach()
    {
        lock (_lock)
        {
            _registered.Clear();
            _bindDirty = false;
            _bindTimer?.Dispose();
            _bindTimer = null;
        }

        _activatedObserver?.Dispose();
        _activatedObserver = null;
        _requestObserver?.Dispose();
        _requestObserver = null;
        _connection?.Dispose();
        _connection = null;
        _sessionHandle = null;
    }

    /// <inheritdoc />
    public void Register(HotkeyId id, ModifierKeys modifierKeys, Key key)
    {
        if (key == Key.None)
        {
            return;
        }

        lock (_lock)
        {
            _registered[id] = (modifierKeys, key);
            _bindDirty = true;
            ScheduleBind();
        }
    }

    /// <inheritdoc />
    public void Unregister(HotkeyId id)
    {
        lock (_lock)
        {
            if (_registered.Remove(id))
            {
                _bindDirty = true;
                ScheduleBind();
            }
        }
    }

    /// <inheritdoc />
    public void UnregisterAll()
    {
        lock (_lock)
        {
            if (_registered.Count == 0)
            {
                return;
            }

            _registered.Clear();
            _bindDirty = true;
            ScheduleBind();
        }
    }

    /// <inheritdoc />
    public void Dispose() => Detach();

    /// <summary>
    /// Connects to the session bus and subscribes to the portal signals, once.
    /// </summary>
    private async Task EnsureConnectedAsync()
    {
        if (_connection != null)
        {
            return;
        }

        try
        {
            var address = DBusAddress.Session;
            if (address == null)
            {
                Log.Info("GlobalShortcuts portal unavailable: no session bus address (DBUS_SESSION_BUS_ADDRESS not set)");
                return;
            }

            var connection = new DBusConnection(address);
            await connection.ConnectAsync();

            _requestObserver = await connection.AddMatchAsync(
                new MatchRule
                {
                    Type = MessageType.Signal,
                    Sender = PortalBusName,
                    Interface = RequestInterface,
                    Member = "Response"
                },
                static (Message m, object? s) => ReadResponseSignal(m),
                (Notification<(string Path, uint Response, Dictionary<string, VariantValue> Results)> n) =>
                {
                    if (n.HasValue)
                    {
                        OnResponse(n.Value);
                    }
                },
                emitOnCapturedContext: false);

            _activatedObserver = await connection.AddMatchAsync(
                new MatchRule
                {
                    Type = MessageType.Signal,
                    Sender = PortalBusName,
                    Interface = GlobalShortcutsInterface,
                    Member = "Activated"
                },
                static (Message m, object? s) =>
                {
                    var reader = m.GetBodyReader();
                    reader.ReadObjectPathAsString(); // session_handle, not needed
                    return reader.ReadString();      // shortcut_id
                },
                (Notification<string> n) =>
                {
                    if (n.HasValue)
                    {
                        OnActivated(n.Value);
                    }
                },
                emitOnCapturedContext: false);

            lock (_lock)
            {
                _connection = connection;
                _bindDirty = true;
                ScheduleBind();
            }

            Log.Info("XDG GlobalShortcuts portal connected.");
        }
        catch (Exception ex)
        {
            Log.Info($"GlobalShortcuts portal unavailable: {ex.Message}");
            _connection?.Dispose();
        }
    }

    /// <summary>
    /// Starts the debounce timer for a rebind.
    /// </summary>
    private void ScheduleBind()
    {
        _bindTimer ??= new Timer(_ => _ = RebindAsync(), null, Timeout.Infinite, Timeout.Infinite);
        _bindTimer.Change(BindDelay, Timeout.InfiniteTimeSpan);
    }

    /// <summary>
    /// Creates a fresh portal session and binds all registered shortcuts to it.
    /// BindShortcuts may only be called once per session, so every change re-creates it.
    /// </summary>
    private async Task RebindAsync()
    {
        List<KeyValuePair<HotkeyId, (ModifierKeys Modifiers, Key Key)>>? shortcuts;
        DBusConnection? connection;
        lock (_lock)
        {
            if (_connection == null || !_bindDirty || _bindInFlight)
            {
                return;
            }

            _bindInFlight = true;
            _bindDirty = false;
            connection = _connection;
            shortcuts = new List<KeyValuePair<HotkeyId, (ModifierKeys, Key)>>(_registered);
        }

        try
        {
            if (shortcuts.Count == 0)
            {
                lock (_lock)
                {
                    _bindInFlight = false;
                }

                return;
            }

            var token = $"edea{Interlocked.Increment(ref _tokenCounter)}";
            var requestPath = BuildRequestPath(connection, token);

            var createTcs = new TaskCompletionSource<(uint, Dictionary<string, VariantValue>)>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (_lock)
            {
                _pendingRequests[requestPath] = createTcs;
            }

            try
            {
                var createMsg = BuildCreateSessionMessage(connection, token);
                var returnedPath = await connection.CallMethodAsync(createMsg,
                    static (Message m, object? s) => m.GetBodyReader().ReadObjectPathAsString());

                var createResponse = await WaitForResponseAsync(createTcs, requestPath);
                if (createResponse.Response != 0 || !createResponse.Results.TryGetValue("session_handle", out var handleValue))
                {
                    Log.Warn($"GlobalShortcuts CreateSession failed (response {createResponse.Response})");
                    return;
                }

                var sessionHandle = handleValue.GetString();

                var bindToken = $"edea{Interlocked.Increment(ref _tokenCounter)}";
                var bindPath = BuildRequestPath(connection, bindToken);
                var bindTcs = new TaskCompletionSource<(uint, Dictionary<string, VariantValue>)>(TaskCreationOptions.RunContinuationsAsynchronously);
                lock (_lock)
                {
                    _pendingRequests[bindPath] = bindTcs;
                }

                try
                {
                    var bindMsg = BuildBindShortcutsMessage(connection, sessionHandle, bindToken, shortcuts);
                    await connection.CallMethodAsync(bindMsg,
                        static (Message m, object? s) => m.GetBodyReader().ReadObjectPathAsString());

                    var bindResponse = await WaitForResponseAsync(bindTcs, bindPath);
                    if (bindResponse.Response == 0)
                    {
                        _sessionHandle = sessionHandle;
                        Log.Info($"Bound {shortcuts.Count} global shortcut(s) via XDG portal.");
                    }
                    else
                    {
                        Log.Warn($"GlobalShortcuts BindShortcuts failed (response {bindResponse.Response}) - the desktop portal may not support global shortcuts");
                    }
                }
                finally
                {
                    lock (_lock)
                    {
                        _pendingRequests.Remove(bindPath);
                    }
                }
            }
            finally
            {
                lock (_lock)
                {
                    _pendingRequests.Remove(requestPath);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warn($"GlobalShortcuts portal bind failed: {ex.Message}");
        }
        finally
        {
            lock (_lock)
            {
                _bindInFlight = false;
                if (_bindDirty)
                {
                    ScheduleBind();
                }
            }
        }
    }

    /// <summary>
    /// Waits for a portal request response with a timeout.
    /// </summary>
    private static async Task<(uint Response, Dictionary<string, VariantValue> Results)> WaitForResponseAsync(
        TaskCompletionSource<(uint Response, Dictionary<string, VariantValue> Results)> tcs, string requestPath)
    {
        var delay = Task.Delay(TimeSpan.FromSeconds(10));
        var completed = await Task.WhenAny(tcs.Task, delay);
        if (completed != tcs.Task)
        {
            throw new TimeoutException($"No portal response for request {requestPath}");
        }

        return tcs.Task.Result;
    }

    /// <summary>
    /// Builds the CreateSession method call message. Kept synchronous because
    /// <see cref="MessageWriter"/> is a ref struct that cannot cross await boundaries.
    /// </summary>
    private static MessageBuffer BuildCreateSessionMessage(DBusConnection connection, string token)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: PortalBusName,
            path: PortalObjectPath,
            @interface: GlobalShortcutsInterface,
            signature: "a{sv}",
            member: "CreateSession");
        writer.WriteDictionary(new Dictionary<string, VariantValue>
        {
            ["handle_token"] = VariantValue.String(token),
            ["session_handle_token"] = VariantValue.String($"s{token}")
        });
        return writer.CreateMessage();
    }

    /// <summary>
    /// Builds the BindShortcuts method call message for all given hotkeys.
    /// </summary>
    private MessageBuffer BuildBindShortcutsMessage(DBusConnection connection, string sessionHandle,
        string bindToken, List<KeyValuePair<HotkeyId, (ModifierKeys Modifiers, Key Key)>> shortcuts)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: PortalBusName,
            path: PortalObjectPath,
            @interface: GlobalShortcutsInterface,
            signature: "oa(sa{sv})sa{sv}",
            member: "BindShortcuts");
        writer.WriteObjectPath(sessionHandle);

        var arrayStart = writer.WriteArrayStart(DBusType.Struct);
        foreach (var (id, spec) in shortcuts)
        {
            var trigger = KeySymMap.GetPortalTrigger(spec.Modifiers, spec.Key);
            if (trigger == null)
            {
                continue;
            }

            writer.WriteStructureStart();
            writer.WriteString(((int)id).ToString(System.Globalization.CultureInfo.InvariantCulture));

            var dictStart = writer.WriteDictionaryStart();
            writer.WriteDictionaryEntryStart();
            writer.WriteString("description");
            writer.WriteVariantString($"EDEA {id}");
            writer.WriteDictionaryEntryStart();
            writer.WriteString("preferred_trigger");
            writer.WriteVariantString(trigger);
            writer.WriteDictionaryEnd(dictStart);
        }

        writer.WriteArrayEnd(arrayStart);
        writer.WriteString(_windowHandle != 0 ? $"x11:{_windowHandle:x}" : "");
        writer.WriteDictionary(new Dictionary<string, VariantValue>
        {
            ["handle_token"] = VariantValue.String(bindToken)
        });
        return writer.CreateMessage();
    }

    /// <summary>
    /// Computes the deterministic request object path for the given handle token,
    /// based on our unique bus name (":1.234" -> "1_234").
    /// </summary>
    private static string BuildRequestPath(DBusConnection connection, string token)
    {
        var senderElement = (connection.UniqueName ?? "x").TrimStart(':').Replace('.', '_');
        return $"{PortalObjectPath}/request/{senderElement}/{token}";
    }

    /// <summary>
    /// Reads a Request.Response signal body (u response, a{sv} results) plus the object path.
    /// </summary>
    private static (string Path, uint Response, Dictionary<string, VariantValue> Results) ReadResponseSignal(Message message)
    {
        var reader = message.GetBodyReader();
        uint response = reader.ReadUInt32();
        var results = new Dictionary<string, VariantValue>();
        var dictEnd = reader.ReadDictionaryStart();
        while (reader.HasNext(dictEnd))
        {
            var key = reader.ReadString();
            results[key] = reader.ReadVariantValue();
        }

        return (message.PathAsString ?? string.Empty, response, results);
    }

    /// <summary>
    /// Dispatches a portal response to the waiting request.
    /// </summary>
    private void OnResponse((string Path, uint Response, Dictionary<string, VariantValue> Results) signal)
    {
        lock (_lock)
        {
            if (_pendingRequests.TryGetValue(signal.Path, out var tcs))
            {
                tcs.TrySetResult((signal.Response, signal.Results));
            }
        }
    }

    /// <summary>
    /// Handles a GlobalShortcuts.Activated signal.
    /// </summary>
    private void OnActivated(string shortcutId)
    {
        if (!int.TryParse(shortcutId, System.Globalization.CultureInfo.InvariantCulture, out var idValue) ||
            !Enum.IsDefined(typeof(HotkeyId), idValue))
        {
            return;
        }

        var id = (HotkeyId)idValue;
        var handler = HotkeyPressed;
        if (handler != null)
        {
            Dispatcher.UIThread.Post(() => handler(id));
        }
    }
}
