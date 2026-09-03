namespace EDEA.Models;

/// <summary>
/// Represents the classification of a Guardian frame shift drive booster.
/// </summary>
public class GuardianFsdBoosterClassification : ModuleClassification
{
    /// <summary>
    /// Gets or sets the jump range boost in light years.
    /// </summary>
    /// <value>The jump range boost.</value>
    public double JumpBoost { get; set; }
}
