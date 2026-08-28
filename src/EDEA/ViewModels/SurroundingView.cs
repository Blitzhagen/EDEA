namespace EDEA.ViewModels;

/// <summary>
/// Simple data transfer object for surrounding system information used in the UI.
/// </summary>
public class SurroundingView
{
    /// <summary>
    /// Gets or sets the name of the surrounding system.
    /// </summary>
    /// <value>The system name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the star class of the surrounding system.
    /// </summary>
    /// <value>The star class.</value>
    public string StarClass { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the discovery status of the surrounding system.
    /// </summary>
    /// <value>The discovery status.</value>
    public string DiscoveryStatus { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cartographic value of the surrounding system.
    /// </summary>
    /// <value>The cartographic value.</value>
    public string CartographicValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the exploration progress of the surrounding system.
    /// </summary>
    /// <value>The progress.</value>
    public string Progress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the EDSM discoverer of the surrounding system.
    /// </summary>
    /// <value>The EDSM discoverer.</value>
    public string EdsmDiscoverer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the distance to the surrounding system.
    /// </summary>
    /// <value>The distance.</value>
    public string Distance { get; set; } = string.Empty;
}
