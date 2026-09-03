using System;

namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for a biological species.
/// </summary>
public class SpeechOutputSpecies : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputSpecies"/> class from a genus.
    /// </summary>
    /// <param name="genus">The genus to describe.</param>
    public SpeechOutputSpecies(Genus genus)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesName, genus.Species);
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesVariant, genus.VariantShort);
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesValue, genus.VistaGenomicsMaxValue.ToString("0.##"));
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesClonColRng, Convert.ToString(genus.ClonalColonyRange));
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesScanCount, Convert.ToString(genus.ScanCount));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputSpecies"/> class from a classification.
    /// </summary>
    /// <param name="genusClassification">The genus classification to describe.</param>
    public SpeechOutputSpecies(GenusClassification genusClassification)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesName, genusClassification.Species);
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesVariant, genusClassification.VariantShort);
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesValue, genusClassification.VistaGenomicsBaseValue.ToString("0.##"));
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesClonColRng, Convert.ToString(genusClassification.ClonalColonyRange));
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesScanCount, "unknown");
    }
}
