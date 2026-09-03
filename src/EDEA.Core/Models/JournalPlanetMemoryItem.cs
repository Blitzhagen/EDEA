using System.Collections.Generic;

namespace EDEA.Models;

/// <summary>
/// Represents a single planet's data stored during journal processing.
/// </summary>
public class JournalPlanetMemoryItem
{
    /// <summary>
    /// Gets the body identifier.
    /// </summary>
    /// <value>The body identifier.</value>
    public int BodyId { get; }

    /// <summary>
    /// Gets the star system identifier.
    /// </summary>
    /// <value>The star system identifier.</value>
    public long StarSystemId { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the planet was surface scanned.
    /// </summary>
    /// <value><see langword="true"/> if surface scanned; otherwise, <see langword="false"/>.</value>
    public bool SurfaceScanned { get; set; }

    /// <summary>
    /// Gets or sets the number of biological signals.
    /// </summary>
    /// <value>The biological signal count.</value>
    public int BiologicalCount { get; set; }

    /// <summary>
    /// Gets or sets the number of geological signals.
    /// </summary>
    /// <value>The geological signal count.</value>
    public int GeologicalCount { get; set; }

    /// <summary>
    /// Gets or sets the list of genera.
    /// </summary>
    /// <value>The list of genera.</value>
    public List<Genus> Genera { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the planet was efficiently scanned.
    /// </summary>
    /// <value><see langword="true"/> if efficiently scanned; otherwise, <see langword="false"/>.</value>
    public bool EfficientlyScanned { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JournalPlanetMemoryItem"/> class.
    /// </summary>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="bodyId">The body identifier.</param>
    public JournalPlanetMemoryItem(long starSystemId, int bodyId)
    {
        StarSystemId = starSystemId;
        BodyId = bodyId;
        Genera = new List<Genus>();
    }
}
