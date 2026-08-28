namespace EDEA.ViewModels;

/// <summary>
/// Simple data transfer object for ring information used in the UI.
/// </summary>
public class RingView
{
    /// <summary>
    /// Gets or sets the name of the body the ring belongs to.
    /// </summary>
    /// <value>The body name.</value>
    public string BodyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ring name.
    /// </summary>
    /// <value>The ring name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ring type.
    /// </summary>
    /// <value>The ring type.</value>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ring reserve level.
    /// </summary>
    /// <value>The reserve level.</value>
    public string ReserveLevel { get; set; } = string.Empty;
}
