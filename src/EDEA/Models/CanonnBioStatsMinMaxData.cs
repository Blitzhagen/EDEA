namespace EDEA.Models;

public class CanonnBioStatsMinMaxData
{
    public double? Min { get; set; }
    public double? Max { get; set; }
    public int? Value { get; set; }

    public CanonnBioStatsMinMaxData()
    {
    }

    public CanonnBioStatsMinMaxData(double min, double max, int value)
    {
        Min = min;
        Max = max;
        Value = value;
    }
}
