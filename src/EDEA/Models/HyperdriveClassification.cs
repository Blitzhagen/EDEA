namespace EDEA.Models;

public class HyperdriveClassification : ModuleClassification
{
    public double FuelPower { get; set; }

    public double FuelMultiplier { get; set; }

    public double OptimalMass { get; set; }

    public double MaxFuelPerJump { get; set; }

    public double JumpBoostMultiplier { get; set; } = 1.0;
}
