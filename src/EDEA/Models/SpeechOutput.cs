using System.Collections.Generic;

namespace EDEA.Models;

/// <summary>
/// Represents a speech output with placeholder values.
/// </summary>
public class SpeechOutput
{
    /// <summary>
    /// Gets or sets the placeholder values.
    /// </summary>
    /// <value>A dictionary of placeholders and their replacement text.</value>
    public Dictionary<SpeechOutputPlaceholderKeys, string> Placeholders { get; set; } = new();
}
