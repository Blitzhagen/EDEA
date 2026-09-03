using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    private readonly Dictionary<Window, string> _trackedWindows = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaWindowStateService"/> class.
    /// </summary>
    public AvaloniaWindowStateService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var directory = Path.Combine(appData, "EDEA");
        Directory.CreateDirectory(directory);
        _stateFilePath = Path.Combine(directory, "windowstate.json");
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

        _trackedWindows[avaloniaWindow] = windowId;
        ApplyState(avaloniaWindow, windowId);

        avaloniaWindow.Opened += TrackedWindow_Opened;
        avaloniaWindow.Closing += TrackedWindow_Closing;
    }

    /// <summary>
    /// Stops tracking the specified window and persists its current state.
    /// </summary>
    /// <param name="window">The window object.</param>
    public void StopTracking(object window)
    {
        if (window is not Window avaloniaWindow)
        {
            return;
        }

        avaloniaWindow.Opened -= TrackedWindow_Opened;
        avaloniaWindow.Closing -= TrackedWindow_Closing;
        SaveState(avaloniaWindow);
        _trackedWindows.Remove(avaloniaWindow);
    }

    /// <summary>
    /// Handles the window opened event and applies the saved state.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void TrackedWindow_Opened(object? sender, EventArgs e)
    {
        if (sender is Window window && _trackedWindows.TryGetValue(window, out var windowId))
        {
            ApplyState(window, windowId);
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
        if (!_trackedWindows.TryGetValue(window, out var windowId))
        {
            return;
        }

        try
        {
            var states = LoadStates();
            states[windowId] = new WindowStateDto
            {
                X = window.Position.X,
                Y = window.Position.Y,
                Width = (int)window.Width,
                Height = (int)window.Height,
                WindowState = window.WindowState.ToString(),
            };

            var json = JsonSerializer.Serialize(states, SerializerOptions);
            File.WriteAllText(_stateFilePath, json);
        }
        catch (Exception exception)
        {
            log.Error($"Error saving window state for {windowId}", exception);
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
                window.WindowState = windowState;
            }
        }
        catch (Exception exception)
        {
            log.Error($"Error applying window state for {windowId}", exception);
        }
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
