namespace EDEA.Models;

/// <summary>
/// Identifies the current Elite Dangerous GUI focus.
/// </summary>
public enum EdGuiFocus
{
    /// <summary>
    /// No panel is focused.
    /// </summary>
    NoFocus,

    /// <summary>
    /// The internal (right) panel is focused.
    /// </summary>
    InternalPanel,

    /// <summary>
    /// The external (left) panel is focused.
    /// </summary>
    ExternalPanel,

    /// <summary>
    /// The communications panel is focused.
    /// </summary>
    CommsPanel,

    /// <summary>
    /// The role panel is focused.
    /// </summary>
    RolePanel,

    /// <summary>
    /// The station services panel is focused.
    /// </summary>
    StationServices,

    /// <summary>
    /// The galaxy map is focused.
    /// </summary>
    GalaxyMap,

    /// <summary>
    /// The system map is focused.
    /// </summary>
    SystemMap,

    /// <summary>
    /// The orrery view is focused.
    /// </summary>
    Orrery,

    /// <summary>
    /// The full spectrum system scanner (FSS) mode is focused.
    /// </summary>
    FSSmode,

    /// <summary>
    /// The surface area analysis (SAA) mode is focused.
    /// </summary>
    SAAmode,

    /// <summary>
    /// The codex is focused.
    /// </summary>
    Codex
}
