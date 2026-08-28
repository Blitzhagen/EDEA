namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for cartographic values.
/// </summary>
public class SpeechOutputCartographicValues : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputCartographicValues"/> class.
    /// </summary>
    /// <param name="cartographicMaxValue">The achievable maximum cartographic value.</param>
    /// <param name="cartographicValue">The currently achieved cartographic value.</param>
    public SpeechOutputCartographicValues(double cartographicMaxValue, double cartographicValue)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.BodyAchievableValue, cartographicMaxValue.ToString("0.##"));
        Placeholders.Add(SpeechOutputPlaceholderKeys.BodyAchievedValue, cartographicValue.ToString("0.##"));
    }
}
