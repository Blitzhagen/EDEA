namespace EDEA.Models;

/// <summary>
/// Identifies the resource reserve level of a ring.
/// </summary>
public enum RingReserveLevel
{
    /// <summary>
    /// The reserve level is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// Pristine reserves.
    /// </summary>
    Pristine,

    /// <summary>
    /// Major reserves.
    /// </summary>
    Major,

    /// <summary>
    /// Common reserves.
    /// </summary>
    Common,

    /// <summary>
    /// Low reserves.
    /// </summary>
    Low,

    /// <summary>
    /// Depleted reserves.
    /// </summary>
    Depleted
}
