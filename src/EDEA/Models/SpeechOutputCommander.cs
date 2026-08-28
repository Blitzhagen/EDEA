namespace EDEA.Models;

public class SpeechOutputCommander : SpeechOutput
{
    public SpeechOutputCommander(string commanderName)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.CommanderName, commanderName);
    }
}
