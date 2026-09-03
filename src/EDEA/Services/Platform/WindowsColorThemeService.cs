using EDEA.Core.Drawing;
using EDEA.Helpers;
using EDEA.Services;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF implementation of <see cref="IColorThemeService"/>.
/// </summary>
public sealed class WindowsColorThemeService : IColorThemeService
{
    /// <summary>
    /// Applies all currently configured colors to the application resources.
    /// </summary>
    public void ApplyCurrentColors()
    {
        ColorThemeHelper.ApplyCurrentColors();
    }

    /// <summary>
    /// Applies the specified color to all matching application resource keys.
    /// </summary>
    /// <param name="propertyName">The name of the color property to apply.</param>
    /// <param name="color">The color to apply.</param>
    public void ApplyColor(string propertyName, Color color)
    {
        var wpfColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        ColorThemeHelper.ApplyColor(propertyName, wpfColor);
    }
}
