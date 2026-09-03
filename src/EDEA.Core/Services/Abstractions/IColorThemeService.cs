using EDEA.Core.Drawing;

namespace EDEA.Services;

/// <summary>
/// Abstraction for applying color theme settings to the application UI.
/// </summary>
public interface IColorThemeService
{
    /// <summary>
    /// Applies all currently configured colors to the application resources.
    /// </summary>
    void ApplyCurrentColors();

    /// <summary>
    /// Applies the specified color to all matching application resource keys.
    /// </summary>
    /// <param name="propertyName">The name of the color property to apply.</param>
    /// <param name="color">The color to apply.</param>
    void ApplyColor(string propertyName, Color color);
}
