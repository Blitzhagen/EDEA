namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for a star system.
/// </summary>
public class SpeechOutputSystem : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputSystem"/> class.
    /// </summary>
    /// <param name="system">The star system to describe.</param>
    public SpeechOutputSystem(StarSystem system)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.SystemName, system.Name);
        Placeholders.Add(SpeechOutputPlaceholderKeys.SystemStarType, system.StarClass ?? string.Empty);
    }
}
