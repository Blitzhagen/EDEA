namespace EDEA.Models;

/// <summary>
/// Represents the current Elite Dangerous player status.
/// </summary>
public class Status
{
    /// <summary>
    /// Gets or sets the GUI focus.
    /// </summary>
    /// <value>The GUI focus, or <see langword="null"/> if not specified.</value>
    public int? GuiFocus { get; set; }

    /// <summary>
    /// Gets or sets the status flags.
    /// </summary>
    /// <value>The status flags.</value>
    public ulong Flags { get; set; }

    /// <summary>
    /// Gets or sets the secondary status flags.
    /// </summary>
    /// <value>The secondary status flags.</value>
    public ulong Flags2 { get; set; }

    /// <summary>
    /// Gets or sets the ship identifier.
    /// </summary>
    /// <value>The ship identifier, or <see langword="null"/> if not specified.</value>
    public string? Ship { get; set; }

    /// <summary>
    /// Gets or sets the ship identification string.
    /// </summary>
    /// <value>The ship identification, or <see langword="null"/> if not specified.</value>
    public string? ShipIdent { get; set; }

    /// <summary>
    /// Gets or sets the main fuel amount.
    /// </summary>
    /// <value>The main fuel.</value>
    public double FuelMain { get; set; }

    /// <summary>
    /// Gets or sets the reserve fuel amount.
    /// </summary>
    /// <value>The reserve fuel.</value>
    public double FuelReservoir { get; set; }

    /// <summary>
    /// Gets or sets the legal state.
    /// </summary>
    /// <value>The legal state, or <see langword="null"/> if not specified.</value>
    public string? LegalState { get; set; }

    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    /// <value>The latitude.</value>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    /// <value>The longitude.</value>
    public double Longitude { get; set; }

    /// <summary>
    /// Gets or sets the altitude.
    /// </summary>
    /// <value>The altitude.</value>
    public double Altitude { get; set; }

    /// <summary>
    /// Gets or sets the body name.
    /// </summary>
    /// <value>The body name, or <see langword="null"/> if not specified.</value>
    public string? BodyName { get; set; }

    /// <summary>
    /// Gets or sets the star system name.
    /// </summary>
    /// <value>The system name, or <see langword="null"/> if not specified.</value>
    public string? SystemName { get; set; }
}
