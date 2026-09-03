using System.Text.Json.Nodes;

namespace EDEA.Models;

/// <summary>
/// Represents a single line from the Elite Dangerous journal.
/// </summary>
public class JournalLine
{
    /// <summary>
    /// Gets or sets the journal event name.
    /// </summary>
    /// <value>The event name.</value>
    public string Event { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event timestamp.
    /// </summary>
    /// <value>The event timestamp.</value>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the raw JSON data.
    /// </summary>
    /// <value>The raw JSON node, or <see langword="null"/> if not set.</value>
    public JsonNode? Data { get; set; }

    /// <summary>
    /// Parses a JSON string into a <see cref="JournalLine"/>.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The parsed journal line, or <see langword="null"/> if the input is invalid.</returns>
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
