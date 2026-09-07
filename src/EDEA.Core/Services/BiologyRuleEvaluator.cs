using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using EDEA.Models;
using EDEA.Models.Biology;
using log4net;

namespace EDEA.Services;

/// <summary>
/// Evaluates BioScan-derived biology rulesets against a planet/system context.
/// Port of the ruleset matching in BioScan's value_estimate.
/// </summary>
public static class BiologyRuleEvaluator
{
    /// <summary>The logger for this class.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(BiologyRuleEvaluator));

    /// <summary>
    /// The luminosity qualifier flags appended when comparing tuple luminosity values.
    /// </summary>
    private static readonly string[] LuminosityFlags = { "", "a", "b", "ab", "z" };

    /// <summary>
    /// Checks whether the star query matches the given ED journal star type.
    /// Port of BioScan's star_check.
    /// </summary>
    /// <param name="starQuery">The simple star type identifier (A, F, K, O, D, H, etc.).</param>
    /// <param name="starType">The ED journal star type string.</param>
    /// <returns><see langword="true"/> when the query matches.</returns>
    public static bool StarCheck(string starQuery, string starType)
    {
        return starQuery switch
        {
            "A" => starType is "A" or "A_BlueWhiteSuperGiant",
            "B" => starType is "B" or "B_BlueWhiteSuperGiant",
            "F" => starType is "F" or "F_WhiteSuperGiant",
            "G" => starType is "G" or "G_WhiteSuperGiant",
            "K" => starType is "K" or "K_OrangeGiant",
            "M" => starType is "M" or "M_RedGiant" or "M_RedSuperGiant",
            "D" or "C" or "W" => starType.StartsWith(starQuery, StringComparison.Ordinal),
            _ => starType == starQuery,
        };
    }

    /// <summary>
    /// Checks a star list entry which may be a plain star type string or a (type, luminosity) tuple.
    /// </summary>
    /// <param name="entry">The JSON entry (string or two-element array).</param>
    /// <param name="star">The star to check.</param>
    /// <returns><see langword="true"/> when the entry matches.</returns>
    private static bool StarEntryCheck(JsonNode? entry, Star star)
    {
        if (entry is JsonArray tuple && tuple.Count == 2)
        {
            string type = tuple[0]!.GetValue<string>();
            string luminosity = tuple[1]!.GetValue<string>();
            if (!StarCheck(type, star.StarType))
            {
                return false;
            }
            return LuminosityFlags.Any(flag => luminosity + flag == star.Luminosity);
        }
        if (entry is JsonValue value && value.TryGetValue<string>(out string? starType))
        {
            return StarCheck(starType, star.StarType);
        }
        return false;
    }

    /// <summary>
    /// Evaluates a single ruleset against the planet context.
    /// A ruleset matches when all present criteria are fulfilled; missing criteria mean no restriction.
    /// </summary>
    /// <param name="ruleset">The ruleset JSON object.</param>
    /// <param name="ctx">The evaluation context.</param>
    /// <param name="regionMap">The catalog region rule map.</param>
    /// <returns><see langword="true"/> when the ruleset matches.</returns>
    public static bool Matches(JsonObject ruleset, BioEvaluationContext ctx, IReadOnlyDictionary<string, List<int>> regionMap)
    {
        Planet planet = ctx.Planet;
        foreach (var rule in ruleset)
        {
            JsonNode? value = rule.Value;
            if (value == null)
            {
                continue;
            }
            switch (rule.Key)
            {
                case "atmosphere":
                    if (value is JsonValue atmosValue && atmosValue.TryGetValue<string>(out string? atmos) && atmos == "Any")
                    {
                        if (planet.AtmosphereType is "" or "None")
                        {
                            return false;
                        }
                    }
                    else if (!StringListContains(value, planet.AtmosphereType))
                    {
                        return false;
                    }
                    break;
                case "atmosphere_component":
                    if (value is JsonObject components)
                    {
                        foreach (var component in components)
                        {
                            double required = component.Value!.GetValue<double>();
                            if (!planet.AtmosphereComposition.TryGetValue(component.Key, out double percent) || percent < required)
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case "max_gravity":
                    if (planet.Gravity > value.GetValue<double>())
                    {
                        return false;
                    }
                    break;
                case "min_gravity":
                    if (planet.Gravity < value.GetValue<double>())
                    {
                        return false;
                    }
                    break;
                case "max_temperature":
                    if (planet.SurfaceTemperature != 0.0 && planet.SurfaceTemperature > value.GetValue<double>())
                    {
                        return false;
                    }
                    break;
                case "min_temperature":
                    if (planet.SurfaceTemperature != 0.0 && planet.SurfaceTemperature < value.GetValue<double>())
                    {
                        return false;
                    }
                    break;
                case "min_pressure":
                    if (planet.SurfacePressure != 0.0 && planet.SurfacePressure < value.GetValue<double>())
                    {
                        return false;
                    }
                    break;
                case "max_pressure":
                    if (planet.SurfacePressure != 0.0 && planet.SurfacePressure >= value.GetValue<double>())
                    {
                        return false;
                    }
                    break;
                case "max_orbital_period":
                    if (planet.OrbitalPeriod >= value.GetValue<double>())
                    {
                        return false;
                    }
                    break;
                case "volcanism":
                    if (!CheckVolcanism(value, planet.Volcanism.ToLowerInvariant()))
                    {
                        return false;
                    }
                    break;
                case "body_type":
                    if (!StringListContains(value, planet.PlanetClass))
                    {
                        return false;
                    }
                    break;
                case "regions":
                    if (ctx.Region.HasValue)
                    {
                        if (!CheckRegions(value, ctx.Region.Value, regionMap))
                        {
                            return false;
                        }
                    }
                    break;
                case "guardian":
                    if (IsTruthy(value)
                        && (!ctx.X.HasValue || !ctx.Y.HasValue || !ctx.Z.HasValue
                            || !NebulaProvider.IsInGuardianZone((ctx.X.Value, ctx.Y.Value, ctx.Z.Value))))
                    {
                        log.Debug($"Ruleset rejected by 'guardian' criterion for '{planet.Name}' (coords: {ctx.X}, {ctx.Y}, {ctx.Z})");
                        return false;
                    }
                    break;
                case "tuber":
                    if (!ctx.X.HasValue || !ctx.Y.HasValue || !ctx.Z.HasValue
                        || !NebulaProvider.IsInTuberZone((ctx.X.Value, ctx.Y.Value, ctx.Z.Value), value))
                    {
                        log.Debug($"Ruleset rejected by 'tuber' criterion for '{planet.Name}' (coords: {ctx.X}, {ctx.Y}, {ctx.Z})");
                        return false;
                    }
                    break;
                case "bodies":
                    if (value is JsonArray bodyTypes)
                    {
                        var wanted = new List<string>();
                        foreach (JsonNode? node in bodyTypes)
                        {
                            if (node is JsonValue v && v.TryGetValue<string>(out string? s))
                            {
                                wanted.Add(s);
                            }
                        }
                        if (!ctx.System.HasPlanetClass(wanted))
                        {
                            return false;
                        }
                    }
                    break;
                case "main_star":
                    if (value is JsonArray mainList)
                    {
                        if (!mainList.Any(entry => StarEntryCheck(entry, new Star { StarType = ctx.MainStarType, Luminosity = ctx.MainStarLuminosity })))
                        {
                            return false;
                        }
                    }
                    else if (value is JsonValue mainValue && mainValue.TryGetValue<string>(out string? mainQuery))
                    {
                        if (!StarCheck(mainQuery, ctx.MainStarType))
                        {
                            return false;
                        }
                    }
                    break;
                case "parent_star":
                    {
                        bool match = value is JsonArray parentList
                            && parentList.Any(entry => entry is JsonValue v && v.TryGetValue<string>(out string? q) && StarCheck(q, ctx.MainStarType));
                        if (!match && value is JsonArray parentTypes)
                        {
                            foreach (Star parentStar in ctx.ParentStars)
                            {
                                if (parentTypes.Any(entry => entry is JsonValue v && v.TryGetValue<string>(out string? q) && StarCheck(q, parentStar.StarType)))
                                {
                                    match = true;
                                    break;
                                }
                            }
                        }
                        if (!match)
                        {
                            return false;
                        }
                    }
                    break;
                case "star":
                    if (value is JsonArray starList)
                    {
                        if (!ctx.Stars.Any(star => starList.Any(entry => StarEntryCheck(entry, star))))
                        {
                            return false;
                        }
                    }
                    else if (value is JsonValue starValue && starValue.TryGetValue<string>(out string? starQuery))
                    {
                        if (!ctx.Stars.Any(star => StarCheck(starQuery, star.StarType)))
                        {
                            return false;
                        }
                    }
                    break;
                case "nebula":
                    if (value is JsonValue nebulaValue && nebulaValue.TryGetValue<string>(out string? nebulaType))
                    {
                        (double x, double y, double z)? position = ctx.X.HasValue && ctx.Y.HasValue && ctx.Z.HasValue
                            ? (ctx.X.Value, ctx.Y.Value, ctx.Z.Value)
                            : null;
                        if (!NebulaProvider.IsInNebula(ctx.SystemName, position, nebulaType))
                        {
                            return false;
                        }
                    }
                    break;
                case "distance":
                    if (planet.Distance < value.GetValue<double>())
                    {
                        return false;
                    }
                    break;
                case "system":
                    if (value is JsonValue systemValue && systemValue.TryGetValue<string>(out string? systemName)
                        && ctx.SystemName != systemName)
                    {
                        return false;
                    }
                    break;
                case "region":
                    // BioScan ignores this key (no match arm in load.py); the
                    // 'guardian' criterion already restricts to the same zones.
                    break;
                default:
                    if (_unknownRuleKeys.Add(rule.Key))
                    {
                        log.Warn($"Unknown ruleset criterion '{rule.Key}' ignored");
                    }
                    break;
            }
        }
        return true;
    }

    private static readonly System.Collections.Generic.HashSet<string> _unknownRuleKeys = new();

    /// <summary>
    /// Checks whether the JSON value is a list of strings containing the given item.
    /// </summary>
    private static bool StringListContains(JsonNode? value, string item)
    {
        if (value is not JsonArray array)
        {
            return false;
        }
        return array.Any(node => node is JsonValue v && v.TryGetValue<string>(out string? s) && s == item);
    }

    /// <summary>
    /// Checks whether a JSON value is truthy (non-empty string, non-zero number, or true).
    /// </summary>
    private static bool IsTruthy(JsonNode? value)
    {
        return value switch
        {
            JsonValue v when v.TryGetValue<bool>(out bool b) => b,
            JsonValue v when v.TryGetValue<string>(out string? s) => !string.IsNullOrEmpty(s),
            JsonValue v when v.TryGetValue<int>(out int i) => i != 0,
            _ => value != null,
        };
    }

    /// <summary>
    /// Checks the BioScan "volcanism" rule against a lowercased volcanism string.
    /// </summary>
    /// <param name="value">The rule value (list, "Any", "None", or "!"-negation).</param>
    /// <param name="volcanism">The lowercased journal volcanism string.</param>
    /// <returns><see langword="true"/> when the rule matches.</returns>
    private static bool CheckVolcanism(JsonNode value, string volcanism)
    {
        if (value is JsonArray list)
        {
            foreach (JsonNode? node in list)
            {
                if (node is not JsonValue v || !v.TryGetValue<string>(out string? volcType))
                {
                    continue;
                }
                if (volcType.StartsWith('='))
                {
                    if (volcanism == volcType[1..])
                    {
                        return true;
                    }
                }
                else if (volcanism.Contains(volcType))
                {
                    return true;
                }
            }
            return false;
        }
        if (value is not JsonValue single || !single.TryGetValue<string>(out string? rule))
        {
            return true;
        }
        if (rule == "Any")
        {
            return volcanism != "";
        }
        if (rule == "None")
        {
            return volcanism == "";
        }
        if (rule.StartsWith('!'))
        {
            // 'not' values assume there must be some volcanism
            return volcanism != "" && !volcanism.Contains(rule[1..]);
        }
        return volcanism.Contains(rule);
    }

    /// <summary>
    /// Checks the BioScan "regions" rule: '!' entries eliminate, positive entries require membership.
    /// </summary>
    /// <param name="value">The list of region rule names.</param>
    /// <param name="region">The current region identifier.</param>
    /// <param name="regionMap">The catalog region map.</param>
    /// <returns><see langword="true"/> when the region rule matches.</returns>
    private static bool CheckRegions(JsonNode value, int region, IReadOnlyDictionary<string, List<int>> regionMap)
    {
        if (value is not JsonArray list)
        {
            return true;
        }
        var positives = new List<string>();
        foreach (JsonNode? node in list)
        {
            if (node is not JsonValue v || !v.TryGetValue<string>(out string? name))
            {
                continue;
            }
            if (name.StartsWith('!'))
            {
                if (GalacticRegionProvider.IsInRegionRule(name[1..], region, regionMap))
                {
                    return false;
                }
            }
            else
            {
                positives.Add(name);
            }
        }
        if (positives.Count > 0)
        {
            return positives.Any(name => GalacticRegionProvider.IsInRegionRule(name, region, regionMap));
        }
        return true;
    }

    /// <summary>
    /// Checks whether a star qualifies as a color source for the given body:
    /// the star has distance zero or is a black-hole parent orbited by the body (parent_is_H).
    /// </summary>
    /// <param name="star">The star to check.</param>
    /// <param name="ctx">The evaluation context.</param>
    /// <returns><see langword="true"/> when the star may provide a color.</returns>
    public static bool ParentIsH(Star star, BioEvaluationContext ctx)
    {
        Planet body = ctx.Planet;
        StarSystem system = ctx.System;
        if (star.Name == system.Name || !body.Name.StartsWith(star.Name + " "))
        {
            return false;
        }
        Star? mainStar = system.MainStar;
        if (mainStar != null && mainStar.Name == system.Name)
        {
            return ctx.MainStarType == "H";
        }
        string[] starParts = system.GetBodyShortName(star.Name).Split(' ');
        if (starParts[0].Length > 1)
        {
            foreach (char letter in starParts[0])
            {
                Star? part = system.GetStarByShortName(letter.ToString());
                if (part?.StarType == "H")
                {
                    return true;
                }
            }
            return false;
        }
        if (starParts.Length == 1)
        {
            return star.StarType == "H";
        }
        Star? headStar = system.GetStarByShortName(starParts[0]);
        return headStar?.StarType == "H";
    }

    /// <summary>
    /// Resolves the possible color variants for a species of a genus.
    /// Port of the color checks in BioScan's value_estimate.
    /// </summary>
    /// <param name="genus">The genus catalog entry.</param>
    /// <param name="speciesKey">The species codex key.</param>
    /// <param name="ctx">The evaluation context.</param>
    /// <returns>The sorted list of possible colors; empty when the genus has no color rules or nothing matched.</returns>
    public static List<string> ResolveColors(BiologyGenus genus, string speciesKey, BioEvaluationContext ctx)
    {
        var foundColors = new HashSet<string>();
        if (genus.SpeciesColors != null)
        {
            // Per-species color rules
            if (!genus.SpeciesColors.TryGetValue(speciesKey, out BiologySpeciesColors? speciesColors) || speciesColors == null)
            {
                return new List<string>();
            }
            if (speciesColors.Star != null)
            {
                bool parentMatched = false;
                foreach (Star parentStar in ctx.ParentStars)
                {
                    if (parentMatched)
                    {
                        break;
                    }
                    foreach (var colorRule in speciesColors.Star)
                    {
                        if (StarCheck(colorRule.Key, parentStar.StarType))
                        {
                            foundColors.Add(colorRule.Value);
                            parentMatched = true;
                            break;
                        }
                    }
                }
                foreach (Star star in ctx.Stars)
                {
                    if (ctx.ParentStars.Contains(star))
                    {
                        continue;
                    }
                    if (star.Distance == 0.0 || ParentIsH(star, ctx))
                    {
                        foreach (var colorRule in speciesColors.Star)
                        {
                            if (StarCheck(colorRule.Key, star.StarType))
                            {
                                foundColors.Add(colorRule.Value);
                                break;
                            }
                        }
                    }
                }
            }
            else if (speciesColors.Element != null)
            {
                foreach (var colorRule in speciesColors.Element)
                {
                    if (ctx.Planet.Materials.Contains(colorRule.Key))
                    {
                        foundColors.Add(colorRule.Value);
                    }
                }
            }
            return foundColors.OrderBy(c => c, StringComparer.Ordinal).ToList();
        }
        if (genus.StarColors == null)
        {
            return new List<string>();
        }
        // Genus-level star color rules
        bool parentFound = false;
        foreach (Star parentStar in ctx.ParentStars)
        {
            if (parentFound)
            {
                break;
            }
            foreach (var colorRule in genus.StarColors)
            {
                if (StarCheck(colorRule.Key, parentStar.StarType))
                {
                    foundColors.Add(colorRule.Value);
                    parentFound = true;
                    break;
                }
            }
        }
        foreach (Star star in ctx.Stars)
        {
            if (ctx.ParentStars.Contains(star))
            {
                continue;
            }
            if (star.Distance == 0.0 || ParentIsH(star, ctx))
            {
                foreach (var colorRule in genus.StarColors)
                {
                    if (StarCheck(colorRule.Key, star.StarType))
                    {
                        foundColors.Add(colorRule.Value);
                        break;
                    }
                }
            }
        }
        return foundColors.OrderBy(c => c, StringComparer.Ordinal).ToList();
    }
}
