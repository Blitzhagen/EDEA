using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EDEA.Models;

/// <summary>
/// Stores and updates planet data parsed from the journal.
/// </summary>
public class JournalPlanetMemory
{
    /// <summary>
    /// The stored planet memory items, keyed by star system and body identifier.
    /// </summary>
    private readonly ConcurrentDictionary<(long, int), JournalPlanetMemoryItem> _items = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="JournalPlanetMemory"/> class.
    /// </summary>
    public JournalPlanetMemory()
    {
    }

    /// <summary>
    /// Adds or updates a planet memory item.
    /// </summary>
    /// <param name="item">The item to add or update.</param>
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

    /// <summary>
    /// Determines whether an item exists for the specified identifiers.
    /// </summary>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="bodyId">The body identifier.</param>
    /// <returns><see langword="true"/> if the item exists; otherwise, <see langword="false"/>.</returns>
    public bool Contains(long starSystemId, int bodyId)
    {
        return _items.ContainsKey((starSystemId, bodyId));
    }

    /// <summary>
    /// Gets the item for the specified identifiers.
    /// </summary>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="bodyId">The body identifier.</param>
    /// <returns>The memory item, or <see langword="null"/> if not found.</returns>
    public JournalPlanetMemoryItem? Get(long starSystemId, int bodyId)
    {
        _items.TryGetValue((starSystemId, bodyId), out var item);
        return item;
    }

    /// <summary>
    /// Clears all stored items.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }
}
