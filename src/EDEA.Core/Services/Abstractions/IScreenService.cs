using EDEA.Core.Windowing;

namespace EDEA.Services;

/// <summary>
/// Abstraction for querying screen geometry.
/// </summary>
public interface IScreenService
{
    /// <summary>
    /// Gets the virtual screen size spanning all displays.
    /// </summary>
    /// <returns>The virtual screen size.</returns>
    ScreenSize GetVirtualScreenSize();

    /// <summary>
    /// Gets the primary screen size.
    /// </summary>
    /// <returns>The primary screen size.</returns>
    ScreenSize GetPrimaryScreenSize();

    /// <summary>
    /// Gets the primary screen working area.
    /// </summary>
    /// <returns>The primary screen working area.</returns>
    ScreenRect GetWorkingArea();
}
