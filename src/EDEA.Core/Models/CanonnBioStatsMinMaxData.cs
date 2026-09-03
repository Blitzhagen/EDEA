namespace EDEA.Models;

/// <summary>
/// Represents a single minimum, maximum and value data point for biological statistics.
/// </summary>
public class CanonnBioStatsMinMaxData
{
    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    /// <value>The minimum value, or <see langword="null"/> if not specified.</value>
    public double? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    /// <value>The maximum value, or <see langword="null"/> if not specified.</value>
    public double? Max { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value, or <see langword="null"/> if not specified.</value>
    public int? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonnBioStatsMinMaxData"/> class.
    /// </summary>
    public CanonnBioStatsMinMaxData()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonnBioStatsMinMaxData"/> class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="value">The value.</param>
    public CanonnBioStatsMinMaxData(double min, double max, int value)
    {
        Min = min;
        Max = max;
        Value = value;
    }
}
