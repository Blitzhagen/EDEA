namespace EDEA.Models;

/// <summary>
/// Represents the classification of a hyperdrive module.
/// </summary>
public class HyperdriveClassification : ModuleClassification
{
    /// <summary>
    /// Gets or sets the fuel power.
    /// </summary>
    /// <value>The fuel power.</value>
    public double FuelPower { get; set; }

    /// <summary>
    /// Gets or sets the fuel multiplier.
    /// </summary>
    /// <value>The fuel multiplier.</value>
    public double FuelMultiplier { get; set; }

    /// <summary>
    /// Gets or sets the optimal mass.
    /// </summary>
    /// <value>The optimal mass.</value>
    public double OptimalMass { get; set; }

    /// <summary>
    /// Gets or sets the maximum fuel consumed per jump.
    /// </summary>
    /// <value>The maximum fuel per jump.</value>
    public double MaxFuelPerJump { get; set; }

    /// <summary>
    /// Gets or sets the jump boost multiplier.
    /// </summary>
    /// <value>The jump boost multiplier.</value>
    public double JumpBoostMultiplier { get; set; } = 1.0;
}
