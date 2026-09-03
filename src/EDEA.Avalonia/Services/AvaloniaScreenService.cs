using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using EDEA.Core.Windowing;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IScreenService"/>.
/// </summary>
public sealed class AvaloniaScreenService : IScreenService
{
    /// <summary>
    /// Gets the virtual screen size spanning all displays.
    /// </summary>
    /// <returns>The virtual screen size.</returns>
    public ScreenSize GetVirtualScreenSize()
    {
        var screens = GetScreens();
        if (screens == null || screens.ScreenCount == 0)
        {
            return new ScreenSize(1920, 1080);
        }

        double minX = double.MaxValue;
        double minY = double.MaxValue;
        double maxX = double.MinValue;
        double maxY = double.MinValue;

        foreach (var screen in screens.All)
        {
            var bounds = screen.Bounds;
            minX = Math.Min(minX, bounds.X);
            minY = Math.Min(minY, bounds.Y);
            maxX = Math.Max(maxX, bounds.X + bounds.Width);
            maxY = Math.Max(maxY, bounds.Y + bounds.Height);
        }

        return new ScreenSize(maxX - minX, maxY - minY);
    }

    /// <summary>
    /// Gets the primary screen size.
    /// </summary>
    /// <returns>The primary screen size.</returns>
    public ScreenSize GetPrimaryScreenSize()
    {
        var screens = GetScreens();
        var primary = screens?.Primary;
        if (primary != null)
        {
            return new ScreenSize(primary.Bounds.Width, primary.Bounds.Height);
        }

        return new ScreenSize(1920, 1080);
    }

    /// <summary>
    /// Gets the primary screen working area.
    /// </summary>
    /// <returns>The primary screen working area.</returns>
    public ScreenRect GetWorkingArea()
    {
        var screens = GetScreens();
        var primary = screens?.Primary;
        if (primary != null)
        {
            var workingArea = primary.WorkingArea;
            return new ScreenRect(workingArea.X, workingArea.Y, workingArea.Width, workingArea.Height);
        }

        return new ScreenRect(0, 0, 1920, 1080);
    }

    /// <summary>
    /// Gets the <see cref="Screens"/> instance from the main window, if available.
    /// </summary>
    /// <returns>The screens instance, or <see langword="null"/>.</returns>
    private static Screens? GetScreens()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
        {
            return desktop.MainWindow.Screens;
        }

        return null;
    }
}
