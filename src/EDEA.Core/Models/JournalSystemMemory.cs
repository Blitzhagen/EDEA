using System.Collections.Concurrent;

namespace EDEA.Models;

/// <summary>
/// Stores and updates star system data parsed from the journal.
/// </summary>
public class JournalSystemMemory
{
    /// <summary>
    /// The stored system memory items, keyed by system identifier.
    /// </summary>
    private readonly ConcurrentDictionary<long, JournalSystemMemoryItem> _items = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="JournalSystemMemory"/> class.
    /// </summary>
    public JournalSystemMemory()
    {
    }

    /// <summary>
    /// Adds or updates a system memory item.
    /// </summary>
    /// <param name="item">The item to add or update.</param>
    public void AddOrUpdate(JournalSystemMemoryItem item)
    {
        if (_items.TryGetValue(item.Id, out var existing))
        {
            if (!string.IsNullOrEmpty(item.StarClass))
            {
                existing.StarClass = item.StarClass;
            }
        }
        else
        {
            _items.TryAdd(item.Id, item);
        }
    }

    /// <summary>
    /// Determines whether an item exists for the specified identifier.
    /// </summary>
    /// <param name="id">The system identifier.</param>
    /// <returns><see langword="true"/> if the item exists; otherwise, <see langword="false"/>.</returns>
    public bool Contains(long id)
    {
        return _items.ContainsKey(id);
    }

    /// <summary>
    /// Gets the item for the specified identifier.
    /// </summary>
    /// <param name="id">The system identifier.</param>
    /// <returns>The memory item, or <see langword="null"/> if not found.</returns>
    public JournalSystemMemoryItem? Get(long id)
    {
        _items.TryGetValue(id, out var item);
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
