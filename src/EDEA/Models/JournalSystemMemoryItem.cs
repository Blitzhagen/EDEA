namespace EDEA.Models;

public class JournalSystemMemoryItem
{
    public long Id { get; }
    public string StarClass { get; set; } = string.Empty;

    public JournalSystemMemoryItem(long systemId)
    {
        Id = systemId;
    }
}
