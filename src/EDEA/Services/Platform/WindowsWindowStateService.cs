using System;
using System.Windows;
using EDEA.Services;
using Jot;
using log4net;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF/Jot implementation of <see cref="IWindowStateService"/>.
/// </summary>
public sealed class WindowsWindowStateService : IWindowStateService
{
    private static readonly ILog log = LogManager.GetLogger(typeof(WindowsWindowStateService));

    private readonly Tracker _tracker;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsWindowStateService"/> class.
    /// </summary>
    /// <param name="tracker">The Jot tracker.</param>
    public WindowsWindowStateService(Tracker tracker)
    {
        _tracker = tracker;
    }

    /// <summary>
    /// Starts tracking the specified window and applies any previously saved state.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <param name="windowId">An identifier for the window.</param>
    public void Track(object window, string windowId)
    {
        try
        {
            if (window is Window wpfWindow)
            {
                if (string.IsNullOrEmpty(wpfWindow.Name))
                {
                    wpfWindow.Name = windowId;
                }
                _tracker.Track(wpfWindow);
            }
        }
        catch (Exception exception)
        {
            log.Error($"Error tracking window {windowId}", exception);
        }
    }

    /// <summary>
    /// Stops tracking the specified window and persists its current state.
    /// </summary>
    /// <param name="window">The window object.</param>
    public void StopTracking(object window)
    {
        // Jot persists automatically on window close; no explicit action required.
    }
}
