using System;

namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for the count of valuable species.
/// </summary>
public class SpeechOutputValuableSpeciesCount : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputValuableSpeciesCount"/> class.
    /// </summary>
    /// <param name="speciesCount">The number of valuable species.</param>
    public SpeechOutputValuableSpeciesCount(int speciesCount)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.ValuableSpeciesCount, Convert.ToString(speciesCount));
    }
}
