namespace EDEA.Models;

public class CanonnBioStatsObject
{
    public string Name { get; set; } = string.Empty;
    public string HudCategory { get; set; } = string.Empty;
    public int? Reward { get; set; }
    public CanonnBioStatsHistograms? Histograms { get; set; }

    public CanonnBioStatsObject()
    {
    }

    public CanonnBioStatsObject(
        string name,
        string hud_category,
        System.Collections.Generic.List<string>? _bodies,
        double? ming,
        double? maxg,
        double? mint,
        double? maxt,
        double? minp,
        double? maxp,
        double? mind,
        double? maxd,
        System.Collections.Generic.List<string>? atmosphereType,
        System.Collections.Generic.List<string>? volcanism,
        int? reward,
        System.Collections.Generic.List<string>? primaryStars,
        CanonnBioStatsHistograms? histograms)
    {
        Name = name;
        HudCategory = hud_category;
        Reward = reward;
        Histograms = histograms;
    }
}
