using System.Collections.Concurrent;

namespace EDEA.Models;

public class JournalSystemMemory
{
    private readonly ConcurrentDictionary<long, JournalSystemMemoryItem> _items = new();

    public JournalSystemMemory()
    {
    }

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

    public bool Contains(long id)
    {
        return _items.ContainsKey(id);
    }

    public JournalSystemMemoryItem? Get(long id)
    {
        _items.TryGetValue(id, out var item);
        return item;
    }

    public void Clear()
    {
        _items.Clear();
    }
}
