using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace EDEA.Models.Biology;

/// <summary>
/// A biological genus from the BioScan-derived catalog (bio_catalog.json "genera").
/// </summary>
public class BiologyGenus
{
    /// <summary>Gets or sets the codex genus key (e.g. "$Codex_Ent_Aleoids_Genus_Name;").</summary>
    /// <value>The codex key.</value>
    public string Key { get; set; } = string.Empty;

    /// <summary>Gets or sets the localized genus name (e.g. "Aleoida").</summary>
    /// <value>The genus name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the clonal colony range in meters.</summary>
    /// <value>The clonal colony range.</value>
    public int Distance { get; set; }

    /// <summary>Gets or sets a value indicating whether multiple colonies of this genus can exist on one body.</summary>
    /// <value><see langword="true"/> if multiple colonies are possible; otherwise, <see langword="false"/>.</value>
    public bool Multiple { get; set; }

    /// <summary>Gets or sets the genus-level star color map (star class to color name).</summary>
    /// <value>The star color map, or <see langword="null"/> when the genus has no color variants.</value>
    public Dictionary<string, string>? StarColors { get; set; }

    /// <summary>Gets or sets the per-species color rules.</summary>
    /// <value>Species key to color rules, or <see langword="null"/>.</value>
    public Dictionary<string, BiologySpeciesColors>? SpeciesColors { get; set; }
}

/// <summary>
/// Per-species color rules of a genus.
/// </summary>
public class BiologySpeciesColors
{
    /// <summary>Gets or sets the star class to color map.</summary>
    /// <value>Star class to color name.</value>
    public Dictionary<string, string>? Star { get; set; }

    /// <summary>Gets or sets the surface material to color map.</summary>
    /// <value>Material name to color name.</value>
    public Dictionary<string, string>? Element { get; set; }
}

/// <summary>
/// A biological species from the BioScan-derived catalog (bio_catalog.json "species").
/// </summary>
public class BiologySpecies
{
    /// <summary>Gets or sets the codex species key (e.g. "$Codex_Ent_Aleoids_01_Name;").</summary>
    /// <value>The codex key.</value>
    public string Key { get; set; } = string.Empty;

    /// <summary>Gets or sets the localized species name (e.g. "Aleoida Arcus").</summary>
    /// <value>The species name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the Vista Genomics base value.</summary>
    /// <value>The base value in credits.</value>
    public int Value { get; set; }

    /// <summary>Gets or sets the alternative rulesets; a species stays a candidate when at least one ruleset matches.</summary>
    /// <value>The rulesets as raw JSON objects (keys as used by the BioScan rules).</value>
    public List<JsonObject> Rulesets { get; set; } = new();
}

/// <summary>
/// The complete biology catalog loaded from bio_catalog.json.
/// </summary>
public class BiologyCatalog
{
    /// <summary>Gets or sets the genera keyed by codex genus key.</summary>
    /// <value>The genera dictionary.</value>
    public Dictionary<string, BiologyGenus> Genera { get; set; } = new();

    /// <summary>Gets or sets the species rules keyed by codex genus key, then codex species key.</summary>
    /// <value>The species rules dictionary.</value>
    public Dictionary<string, Dictionary<string, BiologySpecies>> Species { get; set; } = new();

    /// <summary>Gets or sets the codex genus key to list of codex species name prefixes.</summary>
    /// <value>The codex map.</value>
    public Dictionary<string, List<string>> CodexMap { get; set; } = new();

    /// <summary>Gets or sets the color suffix key to source type ("star" or "element").</summary>
    /// <value>The color suffix map.</value>
    public Dictionary<string, string> ColorSuffixMap { get; set; } = new();

    /// <summary>Gets or sets the region rule name to list of galactic region identifiers.</summary>
    /// <value>The region map.</value>
    public Dictionary<string, List<int>> RegionMap { get; set; } = new();
}

/// <summary>
/// A single matching species candidate within a genus prediction.
/// </summary>
public class BiologySpeciesMatch
{
    /// <summary>Gets or sets the codex species key.</summary>
    /// <value>The codex key.</value>
    public string SpeciesKey { get; set; } = string.Empty;

    /// <summary>Gets or sets the localized species name.</summary>
    /// <value>The species name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the Vista Genomics base value.</summary>
    /// <value>The base value.</value>
    public int Value { get; set; }

    /// <summary>Gets or sets the possible color variants.</summary>
    /// <value>The list of color names.</value>
    public List<string> Colors { get; set; } = new();

    /// <summary>Gets or sets a value indicating whether any variant of this species is already known in the codex of the current region.</summary>
    /// <value><see langword="true"/> when a codex entry exists in the region.</value>
    public bool CodexInRegion { get; set; }

    /// <summary>Gets or sets a value indicating whether any variant of this species is already known in the codex galaxy-wide.</summary>
    /// <value><see langword="true"/> when a codex entry exists in any region.</value>
    public bool CodexInGalaxy { get; set; }
}

/// <summary>
/// The prediction result for one genus on one planet.
/// </summary>
public class BiologyPrediction
{
    /// <summary>Gets or sets the codex genus key.</summary>
    /// <value>The codex key.</value>
    public string GenusKey { get; set; } = string.Empty;

    /// <summary>Gets or sets the localized genus name.</summary>
    /// <value>The genus name.</value>
    public string GenusName { get; set; } = string.Empty;

    /// <summary>Gets or sets the clonal colony range in meters.</summary>
    /// <value>The clonal colony range.</value>
    public int ClonalColonyRange { get; set; }

    /// <summary>Gets or sets the minimum possible species value.</summary>
    /// <value>The minimum value.</value>
    public int MinValue { get; set; }

    /// <summary>Gets or sets the maximum possible species value.</summary>
    /// <value>The maximum value.</value>
    public int MaxValue { get; set; }

    /// <summary>Gets or sets the matching species candidates ordered by value.</summary>
    /// <value>The species candidates.</value>
    public List<BiologySpeciesMatch> Species { get; set; } = new();
}

/// <summary>
/// The evaluation context for a planet within its star system, carrying all data required by the rule engine.
/// </summary>
public class BioEvaluationContext
{
    /// <summary>Gets or sets the planet under evaluation.</summary>
    /// <value>The planet.</value>
    public Planet Planet { get; set; } = null!;

    /// <summary>Gets or sets the star system containing the planet.</summary>
    /// <value>The star system.</value>
    public StarSystem System { get; set; } = null!;

    /// <summary>Gets or sets the resolved parent stars of the planet.</summary>
    /// <value>The parent stars.</value>
    public List<Star> ParentStars { get; set; } = new();

    /// <summary>Gets or sets all stars known in the system.</summary>
    /// <value>All stars.</value>
    public List<Star> Stars { get; set; } = new();

    /// <summary>Gets or sets the main star type.</summary>
    /// <value>The main star type.</value>
    public string MainStarType { get; set; } = string.Empty;

    /// <summary>Gets or sets the main star luminosity.</summary>
    /// <value>The main star luminosity.</value>
    public string MainStarLuminosity { get; set; } = string.Empty;

    /// <summary>Gets or sets the galactic region identifier.</summary>
    /// <value>The region identifier, or <see langword="null"/> when unknown.</value>
    public int? Region { get; set; }

    /// <summary>Gets or sets the system name.</summary>
    /// <value>The system name.</value>
    public string SystemName { get; set; } = string.Empty;

    /// <summary>Gets or sets the system X coordinate.</summary>
    /// <value>The X coordinate, or <see langword="null"/> when unknown.</value>
    public double? X { get; set; }

    /// <summary>Gets or sets the system Y coordinate.</summary>
    /// <value>The Y coordinate, or <see langword="null"/> when unknown.</value>
    public double? Y { get; set; }

    /// <summary>Gets or sets the system Z coordinate.</summary>
    /// <value>The Z coordinate, or <see langword="null"/> when unknown.</value>
    public double? Z { get; set; }
}
