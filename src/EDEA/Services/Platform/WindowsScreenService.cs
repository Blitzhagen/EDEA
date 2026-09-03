using System.Windows;
using EDEA.Core.Windowing;
using EDEA.Services;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF implementation of <see cref="IScreenService"/>.
/// </summary>
public sealed class WindowsScreenService : IScreenService
{
    /// <summary>
    /// Gets the primary screen size.
    /// </summary>
    /// <returns>The primary screen size.</returns>
    public ScreenSize GetPrimaryScreenSize()
    {
        return new ScreenSize(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
    }

    /// <summary>
    /// Gets the primary screen working area.
    /// </summary>
    /// <returns>The primary screen working area.</returns>
    public ScreenRect GetWorkingArea()
    {
        return new ScreenRect(SystemParameters.WorkArea.Left, SystemParameters.WorkArea.Top, SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);
    }
}
