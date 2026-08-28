using System.Text.Json.Nodes;

namespace EDEA.Models;

public class JournalLine
{
    public string Event { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
    public JsonNode? Data { get; set; }

    public static JournalLine? Parse(string json)
    {
        var node = JsonNode.Parse(json);
        if (node is null || node["event"] is null || node["timestamp"] is null)
            return null;

        return new JournalLine
        {
            Event = node["event"]!.GetValue<string>(),
            Timestamp = node["timestamp"]!.GetValue<DateTimeOffset>(),
            Data = node
        };
    }
}
