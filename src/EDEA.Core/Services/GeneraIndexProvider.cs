using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

/// <summary>Provides GeneraIndexProvider functionality.</summary>
public static class GeneraIndexProvider
{
    /// <summary>The log field.</summary>
    private static readonly ILog log;

    /// <summary>The _genusClassifications field.</summary>
    private static List<GenusClassification> _genusClassifications;

    /// <summary>Initializes the GeneraIndexProvider class.</summary>
    static GeneraIndexProvider()
    {
        log = LogManager.GetLogger(typeof(GeneraIndexProvider));
        _genusClassifications = new List<GenusClassification>();
        try
        {
            string gcFile = Path.Combine(Globals.ApplicationFolder, "gc.json");
            if (!File.Exists(gcFile))
            {
                gcFile = Path.Combine(Globals.ApplicationFolder, "Resources", "gc.json");
            }

            string value = File.ReadAllText(gcFile);
            if (JsonNode.Parse(value) is JsonArray array)
            {
                foreach (JsonNode? node in array)
                {
                    if (node is not JsonObject obj)
                    {
                        continue;
                    }

                    string name = obj["Name"]?.GetValue<string>() ?? string.Empty;
                    string species = obj["Species"]?.GetValue<string>() ?? string.Empty;
                    int vista = obj["VistaGenomicsBaseValue"]?.GetValue<int>() ?? 0;
                    int clonal = obj["ClonalColonyRange"]?.GetValue<int>() ?? 0;

                    var item = new GenusClassification(name, species, vista, clonal)
                    {
                        Variant = obj["Variant"]?.GetValue<string>() ?? "unknown",
                        PlanetClasses = ReadStringList(obj, "PlanetClasses"),
                        Atmospheres = ReadStringList(obj, "Atmospheres"),
                        Volcanisms = ReadStringList(obj, "Volcanisms"),
                        StarClasses = ReadStringList(obj, "StarClasses"),
                        GravityRange = ReadGravityRange(obj, "GravityRange"),
                        TemperatureRange = ReadTemperatureRange(obj, "TemperatureRange"),
                        DistanceRange = ReadDistanceRange(obj, "DistanceRange"),
                        LuminosityRange = ReadLuminosityRange(obj, "LuminosityRange")
                    };

                    _genusClassifications.Add(item);
                }
            }
        }
        catch (Exception exception)
        {
            log.Error("Error reading genus classification data!", exception);
        }
    }

    /// <summary>Retrieves StringList.</summary>
    /// <param name="obj">The JsonObject? value of the obj parameter.</param>
    /// <param name="propertyName">The string value of the propertyName parameter.</param>
    /// <returns>A List<string> result.</returns>
    private static List<string> ReadStringList(JsonObject? obj, string propertyName)
    {
        var result = new List<string>();
        if (obj?[propertyName] is not JsonArray array)
        {
            return result;
        }

        foreach (JsonNode? node in array)
        {
            if (node is JsonValue value && value.TryGetValue<string>(out string? s) && s is not null)
            {
                result.Add(s);
            }
        }

        return result;
    }

    /// <summary>Retrieves GravityRange.</summary>
    /// <param name="obj">The JsonObject? value of the obj parameter.</param>
    /// <param name="propertyName">The string value of the propertyName parameter.</param>
    /// <returns>A GravityRange result.</returns>
    private static GravityRange ReadGravityRange(JsonObject? obj, string propertyName)
    {
        var range = new GravityRange();
        if (obj?[propertyName] is not JsonObject rangeObj)
        {
            return range;
        }

        if (rangeObj["Item1"] is JsonValue minJsonValue && minJsonValue.TryGetValue<double>(out double min))
        {
            range.GravityMin = min;
        }
        if (rangeObj["Item2"] is JsonValue maxJsonValue && maxJsonValue.TryGetValue<double>(out double max))
        {
            range.GravityMax = max;
        }

        return range;
    }

    /// <summary>Retrieves TemperatureRange.</summary>
    /// <param name="obj">The JsonObject? value of the obj parameter.</param>
    /// <param name="propertyName">The string value of the propertyName parameter.</param>
    /// <returns>A TemperatureRange result.</returns>
    private static TemperatureRange ReadTemperatureRange(JsonObject? obj, string propertyName)
    {
        var range = new TemperatureRange();
        if (obj?[propertyName] is not JsonObject rangeObj)
        {
            return range;
        }

        if (rangeObj["Item1"] is JsonValue minJsonValue && minJsonValue.TryGetValue<double>(out double min))
        {
            range.TemperatureMin = min;
        }
        if (rangeObj["Item2"] is JsonValue maxJsonValue && maxJsonValue.TryGetValue<double>(out double max))
        {
            range.TemperatureMax = max;
        }

        return range;
    }

    /// <summary>Retrieves DistanceRange.</summary>
    /// <param name="obj">The JsonObject? value of the obj parameter.</param>
    /// <param name="propertyName">The string value of the propertyName parameter.</param>
    /// <returns>A DistanceRange result.</returns>
    private static DistanceRange ReadDistanceRange(JsonObject? obj, string propertyName)
    {
        var range = new DistanceRange();
        if (obj?[propertyName] is not JsonObject rangeObj)
        {
            return range;
        }

        if (rangeObj["Item1"] is JsonValue minJsonValue && minJsonValue.TryGetValue<double>(out double min))
        {
            range.DistanceMin = min;
        }
        if (rangeObj["Item2"] is JsonValue maxJsonValue && maxJsonValue.TryGetValue<double>(out double max))
        {
            range.DistanceMax = max;
        }

        return range;
    }

    /// <summary>Retrieves LuminosityRange.</summary>
    /// <param name="obj">The JsonObject? value of the obj parameter.</param>
    /// <param name="propertyName">The string value of the propertyName parameter.</param>
    /// <returns>A IntRange result.</returns>
    private static IntRange ReadLuminosityRange(JsonObject? obj, string propertyName)
    {
        var range = new IntRange();
        if (obj?[propertyName] is not JsonObject rangeObj)
        {
            return range;
        }

        if (rangeObj["Item1"] is JsonValue minJsonValue && minJsonValue.TryGetValue<int>(out int min))
        {
            range.Min = min;
        }
        if (rangeObj["Item2"] is JsonValue maxJsonValue && maxJsonValue.TryGetValue<int>(out int max))
        {
            range.Max = max;
        }

        return range;
    }

    /// <summary>Retrieves VistaGenomicsValueForSpecies.</summary>
    /// <param name="species">The string value of the species parameter.</param>
    /// <returns>A int result.</returns>
    public static int GetVistaGenomicsValueForSpecies(string species)
    {
        if (!string.IsNullOrEmpty(species) && _genusClassifications.Any(genusClassification => genusClassification.Species == species))
        {
            return _genusClassifications.FirstOrDefault(genusClassification => genusClassification.Species == species)?.VistaGenomicsBaseValue ?? 0;
        }
        return 0;
    }

    /// <summary>Retrieves ClonalColonyRangeForGenus.</summary>
    /// <param name="name">The string value of the name parameter.</param>
    /// <returns>A int result.</returns>
    public static int GetClonalColonyRangeForGenus(string name)
    {
        if (!string.IsNullOrEmpty(name) && _genusClassifications.Any(genusClassification => genusClassification.Name == name))
        {
            return _genusClassifications.FirstOrDefault(genusClassification => genusClassification.Name == name)?.ClonalColonyRange ?? 0;
        }
        return 0;
    }

    /// <summary>Performs the PredictOccurrenceOfSpecies operation.</summary>
    /// <param name="planet">The Planet value of the planet parameter.</param>
    public static void PredictOccurrenceOfSpecies(Planet planet)
    {
        if (planet.BiologicalCount < 1 || planet.StarSystem == null)
        {
            return;
        }
        if (planet.PredictedSpecies.Count() > 0)
        {
            planet.InitialPredictionOfSpecies = false;
            limitOccurrenceOfSpecies(planet);
            return;
        }
        log.Debug("-------------->>> Predicted Species for '" + planet.Name + "' <<<--------------");
        foreach (GenusClassification genusClassification in _genusClassifications)
        {
            if ((genusClassification.Atmospheres.Count <= 0 || genusClassification.Atmospheres.Any(planet.Atmosphere.Equals)) &&
                (genusClassification.PlanetClasses.Count <= 0 || genusClassification.PlanetClasses.Any(planet.PlanetClass.Equals)) &&
                (!genusClassification.GravityRange.GravityMin.HasValue || !(genusClassification.GravityRange.GravityMin > planet.Gravity)) &&
                (!genusClassification.GravityRange.GravityMax.HasValue || !(genusClassification.GravityRange.GravityMax < planet.Gravity)) &&
                (!genusClassification.TemperatureRange.TemperatureMin.HasValue || !(genusClassification.TemperatureRange.TemperatureMin > planet.SurfaceTemperature)) &&
                (!genusClassification.TemperatureRange.TemperatureMax.HasValue || !(genusClassification.TemperatureRange.TemperatureMax < planet.SurfaceTemperature)) &&
                (!genusClassification.DistanceRange.DistanceMin.HasValue || !(genusClassification.DistanceRange.DistanceMin > planet.Distance)) &&
                (!genusClassification.DistanceRange.DistanceMax.HasValue || !(genusClassification.DistanceRange.DistanceMax < planet.Distance)) &&
                (genusClassification.Volcanisms.Count <= 0 || genusClassification.Volcanisms.Any(planet.Volcanism.Equals)) &&
                !string.IsNullOrEmpty(planet.StarSystem.StarClass) &&
                (genusClassification.StarClasses.Count <= 0 || genusClassification.StarClasses.Any(planet.StarSystem.StarClass!.Equals)))
            {
                planet.PredictedSpecies.Add(genusClassification);
                planet.InitialPredictionOfSpecies = true;
                log.Debug($"\n{genusClassification.Species}: \n   {genusClassification.PlanetClasses.Count} planet classes: {string.Join(", ", genusClassification.PlanetClasses)} \n   {genusClassification.Atmospheres.Count} atmospheres: {string.Join(", ", genusClassification.Atmospheres)} \n   {genusClassification.Volcanisms.Count} volcanisms: {string.Join(", ", genusClassification.Volcanisms)} \n   {genusClassification.StarClasses.Count} star classes: {string.Join(", ", genusClassification.StarClasses)} \n   {genusClassification.TemperatureRange.TemperatureMin?.ToString("n0")}-{genusClassification.TemperatureRange.TemperatureMax?.ToString("n0")} K \n   {genusClassification.GravityRange.GravityMin?.ToString("n7")}-{genusClassification.GravityRange.GravityMax?.ToString("n7")} g \n   {genusClassification.DistanceRange.DistanceMin?.ToString("n0")}-{genusClassification.DistanceRange.DistanceMax?.ToString("n0")} Ls");
            }
        }
        limitOccurrenceOfSpecies(planet);
    }

    /// <summary>Performs the PredictOccurrenceOfSpecies operation.</summary>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
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

    /// <summary>Performs the limitOccurrenceOfSpecies operation.</summary>
    /// <param name="planet">The Planet value of the planet parameter.</param>
    private static void limitOccurrenceOfSpecies(Planet planet)
    {
        foreach (GenusClassification predictedSpecies in planet.PredictedSpecies.ToList())
        {
            if (planet.Genuses.Count > 0)
            {
                log.Debug("-------------->>> Limiting Species for '" + planet.Name + "' <<<--------------");
                if (planet.Genuses.Count == planet.BiologicalCount && !planet.Genuses.ContainsKey(predictedSpecies.Name))
                {
                    planet.PredictedSpecies.Remove(predictedSpecies);
                    log.Debug("-------------->>> Removed Species: '" + predictedSpecies.Species + "' <<<--------------");
                }
                else if (planet.Genuses.ContainsKey(predictedSpecies.Name) && planet.Genuses[predictedSpecies.Name].SpeciesSet)
                {
                    planet.PredictedSpecies.Remove(predictedSpecies);
                    log.Debug("-------------->>> Removed Species: '" + predictedSpecies.Species + "' <<<--------------");
                }
            }
        }
    }

    /// <summary>Performs the generateGenusClassificationData operation.</summary>
    /// <param name="bioStatsFilePath">The string value of the bioStatsFilePath parameter.</param>
    private static void generateGenusClassificationData(string bioStatsFilePath)
    {
        // Canonn bio stats based generation is not currently triggered in this app.
        // The pre-generated gc.json provides the required genus classification data.
        // This method is kept for parity with the decompiled reference.
    }
}
