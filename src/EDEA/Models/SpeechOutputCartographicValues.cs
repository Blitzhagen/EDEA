namespace EDEA.Models;

public class SpeechOutputCartographicValues : SpeechOutput
{
    public SpeechOutputCartographicValues(double cartographicMaxValue, double cartographicValue)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.BodyAchievableValue, cartographicMaxValue.ToString("0.##"));
        Placeholders.Add(SpeechOutputPlaceholderKeys.BodyAchievedValue, cartographicValue.ToString("0.##"));
    }
}
