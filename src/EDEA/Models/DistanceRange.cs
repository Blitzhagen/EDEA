namespace EDEA.Models;

/// <summary>
/// Represents a distance range with optional minimum and maximum values.
/// </summary>
public class DistanceRange
{
    /// <summary>
    /// Gets or sets the minimum distance.
    /// </summary>
    /// <value>The minimum distance, or <see langword="null"/> if not specified.</value>
    public double? DistanceMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum distance.
    /// </summary>
    /// <value>The maximum distance, or <see langword="null"/> if not specified.</value>
    public double? DistanceMax { get; set; }
}
