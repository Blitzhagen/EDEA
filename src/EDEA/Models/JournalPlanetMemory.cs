using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EDEA.Models;

public class JournalPlanetMemory
{
    private readonly ConcurrentDictionary<(long, int), JournalPlanetMemoryItem> _items = new();

    public JournalPlanetMemory()
    {
    }

    public void AddOrUpdate(JournalPlanetMemoryItem item)
    {
        var key = (item.StarSystemId, item.BodyId);
        if (_items.TryGetValue(key, out var existing))
        {
            if (item.SurfaceScanned)
                existing.SurfaceScanned = item.SurfaceScanned;

            if (item.BiologicalCount > 0)
                existing.BiologicalCount = item.BiologicalCount;

            if (item.GeologicalCount > 0)
                existing.GeologicalCount = item.GeologicalCount;

            if (item.Genera?.Count > 0)
                existing.Genera = item.Genera;

            if (item.EfficientlyScanned)
                existing.EfficientlyScanned = item.EfficientlyScanned;
        }
        else
        {
            _items.TryAdd(key, item);
        }
    }

    public bool Contains(long starSystemId, int bodyId)
    {
        return _items.ContainsKey((starSystemId, bodyId));
    }

    public JournalPlanetMemoryItem? Get(long starSystemId, int bodyId)
    {
        _items.TryGetValue((starSystemId, bodyId), out var item);
        return item;
    }

    public void Clear()
    {
        _items.Clear();
    }
}
