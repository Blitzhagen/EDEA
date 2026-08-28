namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for a planet classification match.
/// </summary>
public class SpeechOutputPlanetClassification : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputPlanetClassification"/> class.
    /// </summary>
    /// <param name="planetClassification">The matching planet classification.</param>
    public SpeechOutputPlanetClassification(PlanetClassification planetClassification)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.PoiCriteriaSetName, planetClassification.Name);
    }
}
