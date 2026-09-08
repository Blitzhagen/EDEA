using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using EDEA.Services;
using log4net;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IWindowStateService"/>.
/// </summary>
public sealed class AvaloniaWindowStateService : IWindowStateService
{
    private static readonly ILog log = LogManager.GetLogger(typeof(AvaloniaWindowStateService));
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    private readonly string _stateFilePath;
    private readonly Dictionary<Window, WindowTrackingContext> _trackedWindows = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaWindowStateService"/> class.
    /// </summary>
    public AvaloniaWindowStateService()
    {
        var directory = Globals.AppDataFolder;
        Directory.CreateDirectory(directory);
        _stateFilePath = Path.Combine(directory, "windowstate.json");

        var legacyPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EDEA.Core",
            "windowstate.json");
        if (File.Exists(legacyPath)
            && !File.Exists(_stateFilePath)
            && !string.Equals(Path.GetFullPath(legacyPath), Path.GetFullPath(_stateFilePath), StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                File.Move(legacyPath, _stateFilePath);
            }
            catch (Exception)
            {
                // Best effort: state is re-saved on next window change anyway.
            }
        }
    }

    /// <summary>
    /// Starts tracking the specified window and applies any previously saved state.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <param name="windowId">An identifier for the window.</param>
    public void Track(object window, string windowId)
    {
        if (window is not Window avaloniaWindow)
        {
            return;
        }

        var context = new WindowTrackingContext(windowId);
        _trackedWindows[avaloniaWindow] = context;

        avaloniaWindow.Opened += TrackedWindow_Opened;
        avaloniaWindow.Closing += TrackedWindow_Closing;
        avaloniaWindow.Resized += TrackedWindow_Resized;
        avaloniaWindow.PositionChanged += TrackedWindow_PositionChanged;

        ApplyState(avaloniaWindow, windowId);
    }

    /// <summary>
    /// Stops tracking the specified window and optionally persists its current state.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <param name="persist"><c>true</c> to persist the current state; otherwise, <c>false</c>.</param>
    public void StopTracking(object window, bool persist = true)
    {
        if (window is not Window avaloniaWindow)
        {
            return;
        }

        avaloniaWindow.Opened -= TrackedWindow_Opened;
        avaloniaWindow.Closing -= TrackedWindow_Closing;
        avaloniaWindow.Resized -= TrackedWindow_Resized;
        avaloniaWindow.PositionChanged -= TrackedWindow_PositionChanged;

        if (persist)
        {
            SaveState(avaloniaWindow);
        }

        _trackedWindows.Remove(avaloniaWindow);
    }

    /// <summary>
    /// Handles the window opened event and applies the saved state.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void TrackedWindow_Opened(object? sender, EventArgs e)
    {
        if (sender is Window window && _trackedWindows.TryGetValue(window, out var context))
        {
            ApplyState(window, context.WindowId);
        }
    }

    /// <summary>
    /// Handles the window closing event and persists the state.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void TrackedWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (sender is Window window)
        {
            SaveState(window);
        }
    }

    /// <summary>
    /// Handles the window resized event and updates the normal (non-maximized) bounds.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void TrackedWindow_Resized(object? sender, WindowResizedEventArgs e)
    {
        if (sender is Window window && _trackedWindows.TryGetValue(window, out var context))
        {
            UpdateNormalBounds(window, context);
        }
    }

    /// <summary>
    /// Handles the window position changed event and updates the normal (non-maximized) bounds.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void TrackedWindow_PositionChanged(object? sender, PixelPointEventArgs e)
    {
        if (sender is Window window && _trackedWindows.TryGetValue(window, out var context))
        {
            UpdateNormalBounds(window, context);
        }
    }

    /// <summary>
    /// Updates the stored normal bounds for the window when it is not maximized or minimized.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <param name="context">The tracking context.</param>
    private void UpdateNormalBounds(Window window, WindowTrackingContext context)
    {
        if (window.WindowState == WindowState.Normal)
        {
            context.NormalBounds = new WindowStateDto
            {
                X = window.Position.X,
                Y = window.Position.Y,
                Width = (int)window.Width,
                Height = (int)window.Height,
            };
        }
    }

    /// <summary>
    /// Loads the stored window states from disk.
    /// </summary>
    /// <returns>The stored window states.</returns>
    private Dictionary<string, WindowStateDto> LoadStates()
    {
        try
        {
            if (File.Exists(_stateFilePath))
            {
                var json = File.ReadAllText(_stateFilePath);
                return JsonSerializer.Deserialize<Dictionary<string, WindowStateDto>>(json, SerializerOptions) ?? new Dictionary<string, WindowStateDto>();
            }
        }
        catch (Exception exception)

        {
            log.Error("Error loading window states", exception);
        }

        return new Dictionary<string, WindowStateDto>();
    }

    /// <summary>
    /// Saves the specified window state to disk.
    /// </summary>
    /// <param name="window">The window to save.</param>
    private void SaveState(Window window)
    {
        if (!_trackedWindows.TryGetValue(window, out var context))
        {
            return;
        }

        try
        {
            var bounds = context.NormalBounds ?? new WindowStateDto
            {
                X = window.Position.X,
                Y = window.Position.Y,
                Width = (int)window.Width,
                Height = (int)window.Height,
            };

            var states = LoadStates();
            states[context.WindowId] = new WindowStateDto
            {
                X = bounds.X,
                Y = bounds.Y,
                Width = bounds.Width,
                Height = bounds.Height,
                WindowState = window.WindowState.ToString(),
            };

            var json = JsonSerializer.Serialize(states, SerializerOptions);
            File.WriteAllText(_stateFilePath, json);
        }
        catch (Exception exception)
        {
            log.Error($"Error saving window state for {context.WindowId}", exception);
        }
    }

    /// <summary>
    /// Applies the saved state to the specified window.
    /// </summary>
    /// <param name="window">The window to apply the state to.</param>
    /// <param name="windowId">The window identifier.</param>
    private void ApplyState(Window window, string windowId)
    {
        try
        {
            var states = LoadStates();
            if (!states.TryGetValue(windowId, out var state))
            {
                return;
            }

            if (state.Width > 0 && state.Height > 0)
            {
                window.Width = state.Width;
                window.Height = state.Height;
            }

            if (state.X != 0 || state.Y != 0)
            {
                window.Position = new PixelPoint(state.X, state.Y);
            }

            if (Enum.TryParse<WindowState>(state.WindowState, out var windowState))
            {
                // Restore minimized windows as normal; otherwise the window would start hidden.
                if (windowState == WindowState.Minimized)
                {
                    windowState = WindowState.Normal;
                }

                window.WindowState = windowState;
            }
        }
        catch (Exception exception)
        {
            log.Error($"Error applying window state for {windowId}", exception);
        }
    }

    /// <summary>
    /// Holds tracking information for a window.
    /// </summary>
    private sealed class WindowTrackingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowTrackingContext"/> class.
        /// </summary>
        /// <param name="windowId">The window identifier.</param>
        public WindowTrackingContext(string windowId)
        {
            WindowId = windowId;
        }

        /// <summary>
        /// Gets the window identifier.
        /// </summary>
        public string WindowId { get; }

        /// <summary>
        /// Gets or sets the last known normal bounds of the window.
        /// </summary>
        public WindowStateDto? NormalBounds { get; set; }
    }

    /// <summary>
    /// Represents the persisted state of a window.
    /// </summary>
    private sealed class WindowStateDto
    {
        /// <summary>
        /// The horizontal screen position.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The vertical screen position.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The window width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The window height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The window state (Normal, Minimized, Maximized, FullScreen).
        /// </summary>
        public string? WindowState { get; set; }
    }
}
