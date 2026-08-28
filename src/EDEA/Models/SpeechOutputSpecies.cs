using System;

namespace EDEA.Models;

public class SpeechOutputSpecies : SpeechOutput
{
    public SpeechOutputSpecies(Genus genus)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesName, genus.Species);
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesVariant, genus.VariantShort);
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesValue, genus.VistaGenomicsMaxValue.ToString("0.##"));
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesClonColRng, Convert.ToString(genus.ClonalColonyRange));
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesScanCount, Convert.ToString(genus.ScanCount));
    }

    public SpeechOutputSpecies(GenusClassification genusClassification)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesName, genusClassification.Species);
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesVariant, genusClassification.VariantShort);
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesValue, genusClassification.VistaGenomicsBaseValue.ToString("0.##"));
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesClonColRng, Convert.ToString(genusClassification.ClonalColonyRange));
        Placeholders.Add(SpeechOutputPlaceholderKeys.SpeciesScanCount, "unknown");
    }
}
