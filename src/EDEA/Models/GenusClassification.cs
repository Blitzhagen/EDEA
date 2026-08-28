using System.Collections.Generic;

namespace EDEA.Models;

public class IntRange
{
    public int? Min { get; set; }
    public int? Max { get; set; }
}

public class GenusClassification
{
    public string Name { get; }

    public string Species { get; }

    public string Variant { get; set; }

    public string SpeciesShort => Species.Replace(Name, "").Trim();

    public string VariantShort => Variant.Replace(Species, "").Replace("-", "").Trim();

    public int VistaGenomicsBaseValue { get; }

    public int ClonalColonyRange { get; }

    public List<string> PlanetClasses { get; set; }

    public List<string> Atmospheres { get; set; }

    public List<string> Volcanisms { get; set; }

    public GravityRange GravityRange { get; set; }

    public TemperatureRange TemperatureRange { get; set; }

    public DistanceRange DistanceRange { get; set; }

    public List<string> StarClasses { get; set; }

    public IntRange LuminosityRange { get; set; }

    public GenusClassification(string name, string species, int vistaGenomicsBaseValue, int clonalColonyRange)
    {
        Name = name;
        Species = species;
        Variant = "unknown";
        VistaGenomicsBaseValue = vistaGenomicsBaseValue;
        ClonalColonyRange = clonalColonyRange;
        PlanetClasses = new List<string>();
        Atmospheres = new List<string>();
        Volcanisms = new List<string>();
        StarClasses = new List<string>();
        GravityRange = new GravityRange();
        TemperatureRange = new TemperatureRange();
        DistanceRange = new DistanceRange();
        LuminosityRange = new IntRange();
    }
}
