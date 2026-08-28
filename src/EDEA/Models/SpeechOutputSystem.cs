namespace EDEA.Models;

public class SpeechOutputSystem : SpeechOutput
{
    public SpeechOutputSystem(StarSystem system)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.SystemName, system.Name);
        Placeholders.Add(SpeechOutputPlaceholderKeys.SystemStarType, system.StarClass ?? string.Empty);
    }
}
