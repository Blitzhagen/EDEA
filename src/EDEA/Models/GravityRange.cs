namespace EDEA.Models;

/// <summary>
/// Represents a gravity range with optional minimum and maximum values.
/// </summary>
public class GravityRange
{
    /// <summary>
    /// Gets or sets the minimum gravity.
    /// </summary>
    /// <value>The minimum gravity, or <see langword="null"/> if not specified.</value>
    public double? GravityMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum gravity.
    /// </summary>
    /// <value>The maximum gravity, or <see langword="null"/> if not specified.</value>
    public double? GravityMax { get; set; }
}
