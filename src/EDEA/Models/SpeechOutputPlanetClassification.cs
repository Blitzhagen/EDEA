namespace EDEA.Models;

public class SpeechOutputPlanetClassification : SpeechOutput
{
    public SpeechOutputPlanetClassification(PlanetClassification planetClassification)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.PoiCriteriaSetName, planetClassification.Name);
    }
}
