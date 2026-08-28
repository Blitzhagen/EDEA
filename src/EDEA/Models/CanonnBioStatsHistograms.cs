using System.Collections.Generic;

namespace EDEA.Models;

public class CanonnBioStatsHistograms
{
    public Dictionary<string, int> BodyTypes { get; set; } = new();
    public Dictionary<string, int> VolcanicBodyTypes { get; set; } = new();
    public Dictionary<string, int> PrimaryStars { get; set; } = new();
    public Dictionary<string, int> AtmosTypes { get; set; } = new();
    public List<CanonnBioStatsMinMaxData> Distance { get; set; } = new();
    public List<CanonnBioStatsMinMaxData> Gravity { get; set; } = new();
    public List<CanonnBioStatsMinMaxData> Temperature { get; set; } = new();

    public CanonnBioStatsHistograms()
    {
    }

    public CanonnBioStatsHistograms(
        Dictionary<string, int> body_types,
        Dictionary<string, int> volcanic_body_types,
        Dictionary<string, int> primary_stars,
        Dictionary<string, int> atmos_types,
        List<CanonnBioStatsMinMaxData> distance,
        List<CanonnBioStatsMinMaxData> gravity,
        List<CanonnBioStatsMinMaxData> temperature)
    {
        BodyTypes = body_types;
        VolcanicBodyTypes = volcanic_body_types;
        PrimaryStars = primary_stars;
        AtmosTypes = atmos_types;
        Distance = distance;
        Gravity = gravity;
        Temperature = temperature;
    }
}
