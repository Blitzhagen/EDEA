namespace EDEA.Models;

/// <summary>
/// Identifies the source of data used to populate a model.
/// </summary>
public enum DataSource
{
    /// <summary>
    /// Data originates from the game journal.
    /// </summary>
    Journal,

    /// <summary>
    /// Data originates from the Elite Dangerous Star Map.
    /// </summary>
    Edsm,

    /// <summary>
    /// Data originates from the application history.
    /// </summary>
    History
}
