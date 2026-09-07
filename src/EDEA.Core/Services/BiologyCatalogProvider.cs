using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using EDEA.Models;
using EDEA.Models.Biology;
using log4net;

namespace EDEA.Services;

/// <summary>
/// Loads the BioScan-derived biology catalog (bio_catalog.json) and predicts possible
/// biological genera and species for planets. Replaces the former GeneraIndexProvider/gc.json engine.
/// </summary>
public static class BiologyCatalogProvider
{
    /// <summary>The logger for this class.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(BiologyCatalogProvider));

    /// <summary>The loaded biology catalog.</summary>
    private static readonly BiologyCatalog _catalog = new();

    /// <summary>Lookup from localized genus name to catalog genus.</summary>
    private static readonly Dictionary<string, BiologyGenus> _genusByName = new(StringComparer.Ordinal);

    /// <summary>Lookup from localized species name to catalog species.</summary>
    private static readonly Dictionary<string, BiologySpecies> _speciesByName = new(StringComparer.Ordinal);

    /// <summary>Lookup from codex species key to catalog species.</summary>
    private static readonly Dictionary<string, BiologySpecies> _speciesByKey = new(StringComparer.Ordinal);

    /// <summary>Initializes the BiologyCatalogProvider class.</summary>
    static BiologyCatalogProvider()
    {
        try
        {
            string catalogFile = Path.Combine(Globals.ApplicationFolder, "bio_catalog.json");
            if (!File.Exists(catalogFile))
            {
                catalogFile = Path.Combine(Globals.ApplicationFolder, "Resources", "bio_catalog.json");
            }
            if (!File.Exists(catalogFile))
            {
                log.Error("bio_catalog.json not found - biology prediction is disabled");
                return;
            }
            if (JsonNode.Parse(File.ReadAllText(catalogFile)) is not JsonObject root)
            {
                return;
            }
            if (root["genera"] is JsonObject genera)
            {
                foreach (var genusEntry in genera)
                {
                    if (genusEntry.Value is not JsonObject genusObject)
                    {
                        continue;
                    }
                    var genus = new BiologyGenus
                    {
                        Key = genusEntry.Key,
                        Name = genusObject["name"]?.GetValue<string>() ?? string.Empty,
                        Distance = genusObject["distance"]?.GetValue<int>() ?? 0,
                        Multiple = genusObject["multiple"]?.GetValue<bool>() ?? false
                    };
                    if (genusObject["colors"] is JsonObject colors)
                    {
                        if (colors["star"] is JsonObject starColors)
                        {
                            genus.StarColors = ReadStringMap(starColors);
                        }
                        if (colors["species"] is JsonObject speciesColors)
                        {
                            genus.SpeciesColors = new Dictionary<string, BiologySpeciesColors>();
                            foreach (var speciesColorEntry in speciesColors)
                            {
                                if (speciesColorEntry.Value is not JsonObject scObj)
                                {
                                    continue;
                                }
                                var sc = new BiologySpeciesColors();
                                if (scObj["star"] is JsonObject starMap)
                                {
                                    sc.Star = ReadStringMap(starMap);
                                }
                                if (scObj["element"] is JsonObject elementMap)
                                {
                                    sc.Element = ReadStringMap(elementMap);
                                }
                                genus.SpeciesColors[speciesColorEntry.Key] = sc;
                            }
                        }
                    }
                    _catalog.Genera[genus.Key] = genus;
                    if (!string.IsNullOrEmpty(genus.Name))
                    {
                        _genusByName[genus.Name] = genus;
                    }
                }
            }
            if (root["species"] is JsonObject speciesRoot)
            {
                foreach (var genusEntry in speciesRoot)
                {
                    var speciesMap = new Dictionary<string, BiologySpecies>();
                    if (genusEntry.Value is JsonObject speciesObject)
                    {
                        foreach (var speciesEntry in speciesObject)
                        {
                            if (speciesEntry.Value is not JsonObject data)
                            {
                                continue;
                            }
                            var species = new BiologySpecies
                            {
                                Key = speciesEntry.Key,
                                Name = data["name"]?.GetValue<string>() ?? string.Empty,
                                Value = data["value"]?.GetValue<int>() ?? 0
                            };
                            if (data["rulesets"] is JsonArray rulesets)
                            {
                                foreach (JsonNode? ruleset in rulesets)
                                {
                                    if (ruleset is JsonObject rulesetObject)
                                    {
                                        species.Rulesets.Add((JsonObject)rulesetObject.DeepClone());
                                    }
                                }
                            }
                            speciesMap[species.Key] = species;
                            _speciesByKey[species.Key] = species;
                            if (!string.IsNullOrEmpty(species.Name))
                            {
                                _speciesByName.TryAdd(species.Name, species);
                            }
                        }
                    }
                    _catalog.Species[genusEntry.Key] = speciesMap;
                }
            }
            if (root["codexMap"] is JsonObject codexMap)
            {
                foreach (var entry in codexMap)
                {
                    var list = new List<string>();
                    if (entry.Value is JsonArray array)
                    {
                        list.AddRange(array.OfType<JsonValue>().Select(v => v.GetValue<string>()));
                    }
                    _catalog.CodexMap[entry.Key] = list;
                }
            }
            if (root["colorSuffixMap"] is JsonObject suffixMap)
            {
                _catalog.ColorSuffixMap = ReadStringMap(suffixMap);
            }
            if (root["regionMap"] is JsonObject regionMap)
            {
                foreach (var entry in regionMap)
                {
                    var ids = new List<int>();
                    if (entry.Value is JsonArray array)
                    {
                        ids.AddRange(array.OfType<JsonValue>().Select(v => v.GetValue<int>()));
                    }
                    _catalog.RegionMap[entry.Key] = ids;
                }
            }
            log.Info($"Biology catalog loaded ({_catalog.Genera.Count} genera)");
        }
        catch (Exception exception)
        {
            log.Error("Error reading biology catalog data!", exception);
        }
    }

    /// <summary>
    /// Reads a JSON object into a string dictionary.
    /// </summary>
    private static Dictionary<string, string> ReadStringMap(JsonObject obj)
    {
        var map = new Dictionary<string, string>();
        foreach (var entry in obj)
        {
            if (entry.Value is JsonValue value && value.TryGetValue<string>(out string? s))
            {
                map[entry.Key] = s;
            }
        }
        return map;
    }

    /// <summary>Gets the biology catalog.</summary>
    /// <value>The loaded catalog.</value>
    public static BiologyCatalog Catalog => _catalog;

    /// <summary>Retrieves the Vista Genomics base value for a species name or codex key.</summary>
    /// <param name="species">The localized species name or codex species key.</param>
    /// <returns>The base value in credits, or 0 when unknown.</returns>
    public static int GetVistaGenomicsValueForSpecies(string species)
    {
        if (string.IsNullOrEmpty(species))
        {
            return 0;
        }
        if (_speciesByKey.TryGetValue(species, out BiologySpecies? byKey))
        {
            return byKey.Value;
        }
        if (_speciesByName.TryGetValue(species, out BiologySpecies? entry))
        {
            return entry.Value;
        }
        return 0;
    }

    /// <summary>Retrieves the clonal colony range for a genus name or codex key.</summary>
    /// <param name="name">The localized genus name or codex genus key.</param>
    /// <returns>The clonal colony range in meters, or 0 when unknown.</returns>
    public static int GetClonalColonyRangeForGenus(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return 0;
        }
        if (_catalog.Genera.TryGetValue(name, out BiologyGenus? byKey))
        {
            return byKey.Distance;
        }
        if (_genusByName.TryGetValue(name, out BiologyGenus? genus))
        {
            return genus.Distance;
        }
        return 0;
    }

    /// <summary>
    /// Finds the catalog genus for a localized genus name.
    /// </summary>
    /// <param name="name">The localized genus name.</param>
    /// <returns>The genus catalog entry, or <see langword="null"/>.</returns>
    public static BiologyGenus? GetGenusByName(string name)
    {
        return !string.IsNullOrEmpty(name) && _genusByName.TryGetValue(name, out BiologyGenus? genus) ? genus : null;
    }

    /// <summary>
    /// Builds the evaluation context for a planet in its star system.
    /// </summary>
    /// <param name="planet">The planet.</param>
    /// <returns>The evaluation context.</returns>
    public static BioEvaluationContext BuildContext(Planet planet)
    {
        StarSystem system = planet.StarSystem!;
        system.UpdateRegion();
        return new BioEvaluationContext
        {
            Planet = planet,
            System = system,
            ParentStars = system.GetParentStars(planet),
            Stars = system.Stars.ToList(),
            MainStarType = system.MainStarType,
            MainStarLuminosity = system.MainStarLuminosity,
            Region = system.Region,
            SystemName = system.Name,
            X = system.StarPositionX,
            Y = system.StarPositionY,
            Z = system.StarPositionZ
        };
    }

    /// <summary>
    /// Predicts the possible species of a single genus on a planet. Port of BioScan's value_estimate.
    /// </summary>
    /// <param name="genus">The catalog genus.</param>
    /// <param name="ctx">The evaluation context.</param>
    /// <returns>The prediction, or <see langword="null"/> when no species match.</returns>
    public static BiologyPrediction? PredictGenus(BiologyGenus genus, BioEvaluationContext ctx)
    {
        if (!_catalog.Species.TryGetValue(genus.Key, out Dictionary<string, BiologySpecies>? speciesMap))
        {
            return null;
        }
        var possible = new Dictionary<string, List<string>>();
        foreach (var speciesEntry in speciesMap)
        {
            BiologySpecies species = speciesEntry.Value;
            if (species.Rulesets.Any(ruleset => SafeMatches(ruleset, ctx)))
            {
                possible[speciesEntry.Key] = new List<string>();
            }
        }
        // Color/variant checks for genera with color rules
        var eliminated = new HashSet<string>();
        if (genus.SpeciesColors != null)
        {
            foreach (string speciesKey in possible.Keys)
            {
                possible[speciesKey] = BiologyRuleEvaluator.ResolveColors(genus, speciesKey, ctx);
                if (possible[speciesKey].Count == 0)
                {
                    eliminated.Add(speciesKey);
                }
            }
        }
        else if (genus.StarColors != null)
        {
            // Genus-level color rules apply to all candidate species equally.
            List<string> colors = new List<string>();
            foreach (string speciesKey in possible.Keys)
            {
                List<string> speciesColors = BiologyRuleEvaluator.ResolveColors(genus, speciesKey, ctx);
                foreach (string color in speciesColors)
                {
                    if (!colors.Contains(color))
                    {
                        colors.Add(color);
                    }
                }
            }
            if (possible.Count > 0 && colors.Count == 0)
            {
                possible.Clear();
            }
            else
            {
                foreach (string speciesKey in possible.Keys.ToList())
                {
                    possible[speciesKey] = colors.OrderBy(c => c, StringComparer.Ordinal).ToList();
                }
            }
        }
        var sorted = possible
            .Where(entry => !eliminated.Contains(entry.Key))
            .OrderBy(entry => speciesMap[entry.Key].Value)
            .ToList();
        if (sorted.Count == 0)
        {
            return null;
        }
        var prediction = new BiologyPrediction
        {
            GenusKey = genus.Key,
            GenusName = genus.Name,
            ClonalColonyRange = genus.Distance,
            MinValue = speciesMap[sorted[0].Key].Value,
            MaxValue = speciesMap[sorted[^1].Key].Value
        };
        foreach (var entry in sorted)
        {
            BiologySpecies species = speciesMap[entry.Key];
            var match = new BiologySpeciesMatch
            {
                SpeciesKey = entry.Key,
                Name = species.Name,
                Value = species.Value,
                Colors = entry.Value
            };
            // Codex status: unknown in galaxy, or unknown in the current region
            if (match.Colors.Count > 0)
            {
                match.CodexInGalaxy = match.Colors.All(color => CheckCodex(null, genus.Key, entry.Key, color));
                match.CodexInRegion = ctx.Region.HasValue
                    && match.Colors.All(color => CheckCodex(ctx.Region, genus.Key, entry.Key, color));
            }
            else
            {
                match.CodexInGalaxy = CheckCodex(null, genus.Key, entry.Key);
                match.CodexInRegion = ctx.Region.HasValue && CheckCodex(ctx.Region, genus.Key, entry.Key);
            }
            prediction.Species.Add(match);
        }
        return prediction;
    }

    /// <summary>
    /// Resolves the codex biological identifier for a species and optional color variant.
    /// Port of ExploData check_codex's identifier reconstruction.
    /// </summary>
    /// <param name="genusKey">The codex genus key.</param>
    /// <param name="speciesKey">The codex species key.</param>
    /// <param name="variant">The color variant name.</param>
    /// <returns>The codex entry name to look up.</returns>
    public static string CodexBiologicalId(string genusKey, string speciesKey, string variant = "")
    {
        string biological = speciesKey;
        if (string.IsNullOrEmpty(variant) || !_catalog.Genera.TryGetValue(genusKey, out BiologyGenus? genus))
        {
            return biological;
        }
        Dictionary<string, string>? colorData = null;
        bool element = false;
        if (_catalog.Genera.ContainsKey(speciesKey))
        {
            // Species key is itself a genus with star colors (e.g. genus-level codex entries)
            colorData = _catalog.Genera[speciesKey].StarColors;
        }
        else if (genus.SpeciesColors != null)
        {
            if (genus.SpeciesColors.TryGetValue(speciesKey, out BiologySpeciesColors? speciesColors))
            {
                if (speciesColors?.Star != null)
                {
                    colorData = speciesColors.Star;
                }
                else if (speciesColors?.Element != null)
                {
                    colorData = speciesColors.Element;
                    element = true;
                }
            }
        }
        else if (genus.StarColors != null)
        {
            colorData = genus.StarColors;
        }
        string code = string.Empty;
        if (colorData != null)
        {
            foreach (var colorEntry in colorData)
            {
                if (colorEntry.Value == variant)
                {
                    code = element ? char.ToUpper(colorEntry.Key[0]) + colorEntry.Key[1..] : colorEntry.Key;
                    break;
                }
            }
        }
        if (!string.IsNullOrEmpty(code))
        {
            const string suffix = "_Name;";
            if (speciesKey.EndsWith(suffix, StringComparison.Ordinal))
            {
                biological = speciesKey[..^suffix.Length] + "_" + code + suffix;
            }
        }
        return biological;
    }

    /// <summary>
    /// Checks whether a species/variant is already recorded in the codex database.
    /// </summary>
    /// <param name="region">The region identifier, or <see langword="null"/> for galaxy-wide.</param>
    /// <param name="genusKey">The codex genus key.</param>
    /// <param name="speciesKey">The codex species key.</param>
    /// <param name="variant">The optional color variant.</param>
    /// <returns><see langword="true"/> when the codex entry exists.</returns>
    public static bool CheckCodex(int? region, string genusKey, string speciesKey, string variant = "")
    {
        if (!_catalog.Genera.ContainsKey(genusKey))
        {
            return false;
        }
        return CodexTracker.CheckCodex(region, CodexBiologicalId(genusKey, speciesKey, variant));
    }

    /// <summary>
    /// Parses a codex entry name into genus, species, and color variant. Port of ExploData parse_variant.
    /// </summary>
    /// <param name="name">The codex entry name.</param>
    /// <returns>A tuple of genus key, species key, and color name.</returns>
    public static (string genus, string species, string color) ParseVariant(string name)
    {
        foreach (var codexEntry in _catalog.CodexMap)
        {
            string genus = codexEntry.Key;
            if (!_catalog.Species.TryGetValue(genus, out Dictionary<string, BiologySpecies>? speciesMap))
            {
                continue;
            }
            if (speciesMap.ContainsKey(name))
            {
                return (genus, name, string.Empty);
            }
            foreach (string search in codexEntry.Value)
            {
                if (!name.StartsWith(search, StringComparison.Ordinal))
                {
                    continue;
                }
                foreach (string speciesKey in speciesMap.Keys)
                {
                    if (!speciesKey.StartsWith(search, StringComparison.Ordinal))
                    {
                        continue;
                    }
                    string colorType = name[search.Length..].Split("_Name")[0];
                    if (string.IsNullOrEmpty(colorType) || !_catalog.ColorSuffixMap.TryGetValue(colorType, out string? colorKind))
                    {
                        continue;
                    }
                    string color = string.Empty;
                    if (colorKind == "star")
                    {
                        if (_catalog.Genera.TryGetValue(speciesKey, out BiologyGenus? speciesGenus) && speciesGenus.StarColors != null)
                        {
                            speciesGenus.StarColors.TryGetValue(colorType, out color!);
                        }
                        else if (_catalog.Genera.TryGetValue(genus, out BiologyGenus? genusEntry))
                        {
                            if (genusEntry.SpeciesColors != null
                                && genusEntry.SpeciesColors.TryGetValue(speciesKey, out BiologySpeciesColors? sc)
                                && sc?.Star != null)
                            {
                                sc.Star.TryGetValue(colorType, out color!);
                            }
                            else if (genusEntry.StarColors != null)
                            {
                                genusEntry.StarColors.TryGetValue(colorType, out color!);
                            }
                        }
                    }
                    else if (colorKind == "element"
                        && _catalog.Genera.TryGetValue(genus, out BiologyGenus? genusElement)
                        && genusElement.SpeciesColors != null
                        && genusElement.SpeciesColors.TryGetValue(speciesKey, out BiologySpeciesColors? scElem)
                        && scElem?.Element != null)
                    {
                        scElem.Element.TryGetValue(colorType.ToLowerInvariant(), out color!);
                    }
                    return (genus, speciesKey, color);
                }
            }
        }
        return (string.Empty, string.Empty, string.Empty);
    }

    /// <summary>
    /// Evaluates a ruleset defensively: a malformed catalog entry counts as no match instead of aborting the prediction.
    /// </summary>
    private static bool SafeMatches(JsonObject ruleset, BioEvaluationContext ctx)
    {
        try
        {
            return BiologyRuleEvaluator.Matches(ruleset, ctx, _catalog.RegionMap);
        }
        catch (Exception ex)
        {
            log.Warn($"Malformed ruleset skipped for '{ctx.Planet.Name}': {ruleset.ToJsonString()}", ex);
            return false;
        }
    }

    /// <summary>
    /// Predicts the possible species for a planet and fills <see cref="Planet.PredictedSpecies"/>.
    /// </summary>
    /// <param name="planet">The planet to evaluate.</param>
    public static void PredictOccurrenceOfSpecies(Planet planet)
    {
        if (planet.BiologicalCount < 1 || planet.StarSystem == null)
        {
            return;
        }
        if (planet.PredictedSpecies.Count > 0)
        {
            planet.InitialPredictionOfSpecies = false;
            LimitOccurrenceOfSpecies(planet, planet.PredictedSpecies);
            return;
        }
        log.Debug("-------------->>> Predicted Species for '" + planet.Name + "' (BioScan engine) <<<--------------");
        BioEvaluationContext ctx = BuildContext(planet);
        // Build the list locally and assign it atomically: the UI thread materializes
        // PredictedSpecies for tooltips while this loop runs on the journal thread.
        var predicted = new List<GenusClassification>();
        foreach (var genusEntry in _catalog.Genera)
        {
            BiologyGenus genus = genusEntry.Value;
            BiologyPrediction? prediction = PredictGenus(genus, ctx);
            if (prediction == null)
            {
                continue;
            }
            planet.InitialPredictionOfSpecies = true;
            foreach (BiologySpeciesMatch match in prediction.Species)
            {
                var classification = new GenusClassification(prediction.GenusName, match.Name, match.Value, prediction.ClonalColonyRange)
                {
                    GenusKey = prediction.GenusKey,
                    Variant = match.Colors.Count > 0 ? string.Join("/", match.Colors) : "unknown",
                    IsInCodex = match.CodexInRegion,
                    IsInGalaxyCodex = match.CodexInGalaxy
                };
                predicted.Add(classification);
            }
            log.Debug($"\n{prediction.GenusName}: {prediction.Species.Count} candidate species, value {prediction.MinValue}-{prediction.MaxValue}");
        }
        LimitOccurrenceOfSpecies(planet, predicted);
        planet.PredictedSpecies = predicted;
        log.Debug($"PredictedSpecies now {predicted.Count} entries for '{planet.Name}': {string.Join(", ", predicted.Select(p => p.Name + " " + p.Species))}");
    }

    /// <summary>
    /// Predicts the possible species for all planets in a star system.
    /// </summary>
    /// <param name="starSystem">The star system.</param>
    public static void PredictOccurrenceOfSpecies(StarSystem starSystem)
    {
        if (starSystem == null)
        {
            return;
        }
        foreach (Body body in starSystem.Bodies.Values)
        {
            if (body.Type == BodyType.Planet)
            {
                PredictOccurrenceOfSpecies((Planet)body);
            }
        }
    }

    /// <summary>
    /// Removes predicted species that conflict with the already observed genera and signal count.
    /// </summary>
    /// <param name="planet">The planet to limit.</param>
    private static void LimitOccurrenceOfSpecies(Planet planet, List<GenusClassification> predictedSpecies)
    {
        foreach (GenusClassification predicted in predictedSpecies.ToList())
        {
            if (planet.Genuses.Count > 0)
            {
                // Match observed genera by codex key (language-independent) or localized name.
                Genus? observed = planet.Genuses.Values.FirstOrDefault(genus =>
                    (!string.IsNullOrEmpty(predicted.GenusKey) && genus.CodexKey == predicted.GenusKey)
                    || genus.Name == predicted.Name);
                log.Debug("-------------->>> Limiting Species for '" + planet.Name + "' <<<--------------");
                if (planet.Genuses.Count == planet.BiologicalCount && observed == null)
                {
                    predictedSpecies.Remove(predicted);
                    log.Debug("-------------->>> Removed Species: '" + predicted.Species + "' <<<--------------");
                }
                else if (observed != null && observed.SpeciesSet)
                {
                    predictedSpecies.Remove(predicted);
                    log.Debug("-------------->>> Removed Species: '" + predicted.Species + "' <<<--------------");
                }
            }
        }
    }

    /// <summary>
    /// Clears the prediction cache of a planet so it is recalculated on the next call.
    /// </summary>
    /// <param name="planet">The planet to invalidate.</param>
    public static void InvalidatePrediction(Planet planet)
    {
        planet.PredictedSpecies = new List<GenusClassification>();
        planet.InitialPredictionOfSpecies = false;
    }
}
