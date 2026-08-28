using System.Collections.Generic;

namespace EDEA.Models;

public class JournalPlanetMemoryItem
{
    public int BodyId { get; }
    public long StarSystemId { get; }
    public bool SurfaceScanned { get; set; }
    public int BiologicalCount { get; set; }
    public int GeologicalCount { get; set; }
    public List<Genus> Genera { get; set; }
    public bool EfficientlyScanned { get; set; }

    public JournalPlanetMemoryItem(long starSystemId, int bodyId)
    {
        StarSystemId = starSystemId;
        BodyId = bodyId;
        Genera = new List<Genus>();
    }
}
