using System.Collections.Generic;

namespace EDEA.Models;

/// <summary>
/// Represents histogram data for biological statistics provided by Canonn.
/// </summary>
public class CanonnBioStatsHistograms
{
    /// <summary>
    /// Gets or sets the count of body types.
    /// </summary>
    /// <value>A dictionary mapping body type names to their occurrence counts.</value>
    public Dictionary<string, int> BodyTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets the count of volcanic body types.
    /// </summary>
    /// <value>A dictionary mapping volcanic body type names to their occurrence counts.</value>
    public Dictionary<string, int> VolcanicBodyTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets the count of primary star types.
    /// </summary>
    /// <value>A dictionary mapping primary star type names to their occurrence counts.</value>
    public Dictionary<string, int> PrimaryStars { get; set; } = new();

    /// <summary>
    /// Gets or sets the count of atmosphere types.
    /// </summary>
    /// <value>A dictionary mapping atmosphere type names to their occurrence counts.</value>
    public Dictionary<string, int> AtmosTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets the distance histogram data.
    /// </summary>
    /// <value>A list of minimum, maximum and value data points for distance.</value>
    public List<CanonnBioStatsMinMaxData> Distance { get; set; } = new();

    /// <summary>
    /// Gets or sets the gravity histogram data.
    /// </summary>
    /// <value>A list of minimum, maximum and value data points for gravity.</value>
    public List<CanonnBioStatsMinMaxData> Gravity { get; set; } = new();

    /// <summary>
    /// Gets or sets the temperature histogram data.
    /// </summary>
    /// <value>A list of minimum, maximum and value data points for temperature.</value>
    public List<CanonnBioStatsMinMaxData> Temperature { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonnBioStatsHistograms"/> class.
    /// </summary>
    public CanonnBioStatsHistograms()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonnBioStatsHistograms"/> class.
    /// </summary>
    /// <param name="body_types">The count of body types.</param>
    /// <param name="volcanic_body_types">The count of volcanic body types.</param>
    /// <param name="primary_stars">The count of primary star types.</param>
    /// <param name="atmos_types">The count of atmosphere types.</param>
    /// <param name="distance">The distance histogram data.</param>
    /// <param name="gravity">The gravity histogram data.</param>
    /// <param name="temperature">The temperature histogram data.</param>
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
