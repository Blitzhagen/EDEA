namespace EDEA.Models;

/// <summary>
/// Represents a single star system's data stored during journal processing.
/// </summary>
public class JournalSystemMemoryItem
{
    /// <summary>
    /// Gets the system identifier.
    /// </summary>
    /// <value>The system identifier.</value>
    public long Id { get; }

    /// <summary>
    /// Gets or sets the star class.
    /// </summary>
    /// <value>The star class.</value>
    public string StarClass { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="JournalSystemMemoryItem"/> class.
    /// </summary>
    /// <param name="systemId">The system identifier.</param>
    public JournalSystemMemoryItem(long systemId)
    {
        Id = systemId;
    }
}
