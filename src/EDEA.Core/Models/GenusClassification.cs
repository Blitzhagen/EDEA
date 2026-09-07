using System.Collections.Generic;

namespace EDEA.Models;

/// <summary>
/// Represents an integer range with optional minimum and maximum values.
/// </summary>
public class IntRange
{
    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    /// <value>The minimum value, or <see langword="null"/> if not specified.</value>
    public int? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    /// <value>The maximum value, or <see langword="null"/> if not specified.</value>
    public int? Max { get; set; }
}

/// <summary>
/// Represents the classification data for a biological genus.
/// </summary>
public class GenusClassification
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The genus name.</value>
    public string Name { get; }

    /// <summary>
    /// Gets the species.
    /// </summary>
    /// <value>The species name.</value>
    public string Species { get; }

    /// <summary>
    /// Gets or sets the variant.
    /// </summary>
    /// <value>The variant name.</value>
    public string Variant { get; set; }

    /// <summary>
    /// Gets the short species name.
    /// </summary>
    /// <value>The short species name.</value>
    public string SpeciesShort => Species.Replace(Name, "").Trim();

    /// <summary>
    /// Gets the short variant name.
    /// </summary>
    /// <value>The short variant name.</value>
    public string VariantShort => Variant.Replace(Species, "").Replace("-", "").Trim();

    /// <summary>
    /// Gets the base Vista Genomics value.
    /// </summary>
    /// <value>The base Vista Genomics value.</value>
    public int VistaGenomicsBaseValue { get; }

    /// <summary>
    /// Gets the clonal colony range.
    /// </summary>
    /// <value>The clonal colony range.</value>
    public int ClonalColonyRange { get; }

    /// <summary>
    /// Gets or sets the codex genus identifier this classification belongs to.
    /// </summary>
    /// <value>The codex genus key.</value>
    public string GenusKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this species is already recorded in the codex of the current region.
    /// </summary>
    /// <value><see langword="true"/> when a codex entry exists in the region.</value>
    public bool IsInCodex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this species is already recorded in the codex galaxy-wide.
    /// </summary>
    /// <value><see langword="true"/> when a codex entry exists in any region.</value>
    public bool IsInGalaxyCodex { get; set; }

    /// <summary>
    /// Gets or sets the compatible planet classes.
    /// </summary>
    /// <value>The list of planet classes.</value>
    public List<string> PlanetClasses { get; set; }

    /// <summary>
    /// Gets or sets the compatible atmospheres.
    /// </summary>
    /// <value>The list of atmosphere types.</value>
    public List<string> Atmospheres { get; set; }

    /// <summary>
    /// Gets or sets the compatible volcanisms.
    /// </summary>
    /// <value>The list of volcanism types.</value>
    public List<string> Volcanisms { get; set; }

    /// <summary>
    /// Gets or sets the gravity range.
    /// </summary>
    /// <value>The gravity range.</value>
    public GravityRange GravityRange { get; set; }

    /// <summary>
    /// Gets or sets the temperature range.
    /// </summary>
    /// <value>The temperature range.</value>
    public TemperatureRange TemperatureRange { get; set; }

    /// <summary>
    /// Gets or sets the distance range.
    /// </summary>
    /// <value>The distance range.</value>
    public DistanceRange DistanceRange { get; set; }

    /// <summary>
    /// Gets or sets the compatible star classes.
    /// </summary>
    /// <value>The list of star classes.</value>
    public List<string> StarClasses { get; set; }

    /// <summary>
    /// Gets or sets the luminosity range.
    /// </summary>
    /// <value>The luminosity range.</value>
    public IntRange LuminosityRange { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenusClassification"/> class.
    /// </summary>
    /// <param name="name">The genus name.</param>
    /// <param name="species">The species name.</param>
    /// <param name="vistaGenomicsBaseValue">The base Vista Genomics value.</param>
    /// <param name="clonalColonyRange">The clonal colony range.</param>
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
