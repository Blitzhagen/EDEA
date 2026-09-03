namespace EDEA.Enums;

/// <summary>
/// Defines the routing algorithms supported by Spansh.
/// </summary>
public enum SpanshRoutingAlgorithm
{
    /// <summary>
    /// Standard fuel-efficient routing.
    /// </summary>
    Fuel,

    /// <summary>
    /// Routing optimized for fuel and jump count.
    /// </summary>
    Fuel_Jumps,

    /// <summary>
    /// Guided routing with step-by-step waypoints.
    /// </summary>
    Guided,

    /// <summary>
    /// Optimistic routing assuming ideal conditions.
    /// </summary>
    Optimistic,

    /// <summary>
    /// Pessimistic routing assuming conservative conditions.
    /// </summary>
    Pessimistic
}
