namespace EDEA.Models;

/// <summary>
/// Represents a temperature range with optional minimum and maximum values.
/// </summary>
public class TemperatureRange
{
    /// <summary>
    /// Gets or sets the minimum temperature.
    /// </summary>
    /// <value>The minimum temperature, or <see langword="null"/> if not specified.</value>
    public double? TemperatureMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum temperature.
    /// </summary>
    /// <value>The maximum temperature, or <see langword="null"/> if not specified.</value>
    public double? TemperatureMax { get; set; }
}
