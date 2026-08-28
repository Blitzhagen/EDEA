namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for the commander name.
/// </summary>
public class SpeechOutputCommander : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputCommander"/> class.
    /// </summary>
    /// <param name="commanderName">The commander name.</param>
    public SpeechOutputCommander(string commanderName)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.CommanderName, commanderName);
    }
}
