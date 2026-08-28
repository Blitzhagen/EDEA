namespace EDEA.Enums;

/// <summary>
/// Represents the exploration status of a star system.
/// </summary>
public enum StarSystemExplorationStatus
{
    /// <summary>
    /// The exploration status is not known.
    /// </summary>
    Unknown,

    /// <summary>
    /// The system has not been explored.
    /// </summary>
    Unexplored,

    /// <summary>
    /// The system has not been scanned.
    /// </summary>
    Unscanned,

    /// <summary>
    /// The exploration of the system is incomplete.
    /// </summary>
    Incomplete,

    /// <summary>
    /// The exploration of the system is complete.
    /// </summary>
    Complete
}
