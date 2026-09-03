namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for a celestial body.
/// </summary>
public class SpeechOutputBody : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputBody"/> class.
    /// </summary>
    /// <param name="body">The body to describe.</param>
    public SpeechOutputBody(Body body)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.BodyType, Enum.GetName(typeof(BodyType), body.Type) ?? string.Empty);
        Placeholders.Add(SpeechOutputPlaceholderKeys.BodyName, body.ShortName);
    }
}
