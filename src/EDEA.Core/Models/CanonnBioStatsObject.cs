namespace EDEA.Models;

/// <summary>
/// Represents a Canonn biological statistics object.
/// </summary>
public class CanonnBioStatsObject
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name of the biological object.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HUD category.
    /// </summary>
    /// <value>The HUD category of the biological object.</value>
    public string HudCategory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reward.
    /// </summary>
    /// <value>The reward value, or <see langword="null"/> if not specified.</value>
    public int? Reward { get; set; }

    /// <summary>
    /// Gets or sets the histogram data.
    /// </summary>
    /// <value>The histogram data, or <see langword="null"/> if not specified.</value>
    public CanonnBioStatsHistograms? Histograms { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonnBioStatsObject"/> class.
    /// </summary>
    public CanonnBioStatsObject()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonnBioStatsObject"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="hud_category">The HUD category.</param>
    /// <param name="_bodies">The list of bodies.</param>
    /// <param name="ming">The minimum gravity.</param>
    /// <param name="maxg">The maximum gravity.</param>
    /// <param name="mint">The minimum temperature.</param>
    /// <param name="maxt">The maximum temperature.</param>
    /// <param name="minp">The minimum pressure.</param>
    /// <param name="maxp">The maximum pressure.</param>
    /// <param name="mind">The minimum distance.</param>
    /// <param name="maxd">The maximum distance.</param>
    /// <param name="atmosphereType">The list of atmosphere types.</param>
    /// <param name="volcanism">The list of volcanism types.</param>
    /// <param name="reward">The reward value.</param>
    /// <param name="primaryStars">The list of primary star types.</param>
    /// <param name="histograms">The histogram data.</param>
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
