namespace EDEA.Models;

/// <summary>
/// Represents the state of a fleet carrier as reported by the game journal.
/// </summary>
public class FleetCarrier
{
    /// <summary>
    /// Gets or sets the carrier identifier (market ID).
    /// </summary>
    /// <value>The carrier identifier.</value>
    public long CarrierId { get; set; }

    /// <summary>
    /// Gets or sets the carrier callsign.
    /// </summary>
    /// <value>The callsign.</value>
    public string Callsign { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the carrier name.
    /// </summary>
    /// <value>The carrier name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the carrier type (e.g. FleetCarrier or SquadronCarrier).
    /// </summary>
    /// <value>The carrier type.</value>
    public string CarrierType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the amount of tritium in the carrier fuel depot.
    /// </summary>
    /// <value>The tritium fuel level in tonnes.</value>
    public int FuelLevel { get; set; }

    /// <summary>
    /// Gets or sets the current jump range.
    /// </summary>
    /// <value>The current jump range in light years.</value>
    public double JumpRangeCurr { get; set; }

    /// <summary>
    /// Gets or sets the maximum jump range.
    /// </summary>
    /// <value>The maximum jump range in light years.</value>
    public double JumpRangeMax { get; set; } = 500.0;

    /// <summary>
    /// Gets or sets the total capacity of the carrier.
    /// </summary>
    /// <value>The total capacity in tonnes.</value>
    public int TotalCapacity { get; set; } = 25000;

    /// <summary>
    /// Gets or sets the free space on the carrier.
    /// </summary>
    /// <value>The free space in tonnes.</value>
    public int FreeSpace { get; set; }

    /// <summary>
    /// Gets the used capacity of the carrier.
    /// </summary>
    /// <value>The used capacity in tonnes.</value>
    public int CapacityUsed => TotalCapacity - FreeSpace;

    /// <summary>
    /// Gets or sets the identifier of the star system the carrier is located in.
    /// </summary>
    /// <value>The star system identifier.</value>
    public long StarSystemId { get; set; }

    /// <summary>
    /// Gets or sets the name of the star system the carrier is located in.
    /// </summary>
    /// <value>The star system name.</value>
    public string StarSystemName { get; set; } = string.Empty;
}
