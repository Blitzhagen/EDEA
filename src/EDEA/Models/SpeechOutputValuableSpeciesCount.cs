using System;

namespace EDEA.Models;

public class SpeechOutputValuableSpeciesCount : SpeechOutput
{
    public SpeechOutputValuableSpeciesCount(int speciesCount)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.ValuableSpeciesCount, Convert.ToString(speciesCount));
    }
}
