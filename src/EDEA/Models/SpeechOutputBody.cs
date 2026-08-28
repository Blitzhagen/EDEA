namespace EDEA.Models;

public class SpeechOutputBody : SpeechOutput
{
    public SpeechOutputBody(Body body)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.BodyType, Enum.GetName(typeof(BodyType), body.Type) ?? string.Empty);
        Placeholders.Add(SpeechOutputPlaceholderKeys.BodyName, body.ShortName);
    }
}
