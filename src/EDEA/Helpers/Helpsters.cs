using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Documents;
using EDEA.Models;
using EDEA.Properties;
using log4net;

namespace EDEA;

public static class Helpsters
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Helpsters));

    private static readonly ImmutableList<string> _atmosphereAndVolcanismDescriptors;

    private static readonly ImmutableList<string> _scoopableStarClasses;

    public static string FirstLetterToUpperCase(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= 1)
        {
            return string.Empty;
        }
        return char.ToUpper(input[0]) + input.Substring(1, input.Length - 1);
    }

    public static string RemoveAtmosphereAndVolcanismDescriptors(string input)
    {
        foreach (string atmosphereAndVolcanismDescriptor in _atmosphereAndVolcanismDescriptors)
        {
            if (input.StartsWith(atmosphereAndVolcanismDescriptor, StringComparison.CurrentCultureIgnoreCase))
            {
                int length = atmosphereAndVolcanismDescriptor.Length;
                return input.Substring(length, input.Length - length).TrimStart();
            }
        }
        return input;
    }

    public static string DoubleToHumanRounded(double journalValue)
    {
        double absoluteValue = Math.Abs(journalValue);
        if (absoluteValue < 1.0)
        {
            return Convert.ToString(Math.Round(absoluteValue, 2), CultureInfo.CurrentCulture);
        }
        if (absoluteValue < 20.0)
        {
            return Convert.ToString(Math.Round(absoluteValue, 1), CultureInfo.CurrentCulture);
        }
        if (absoluteValue < 100.0)
        {
            return Convert.ToString(Math.Floor(absoluteValue), CultureInfo.CurrentCulture);
        }
        if (absoluteValue < 1000.0)
        {
            return Convert.ToString(Math.Floor(absoluteValue / 10.0) * 10.0, CultureInfo.CurrentCulture);
        }
        if (absoluteValue < 10000.0)
        {
            return Convert.ToString(Math.Floor(absoluteValue / 100.0) * 100.0, CultureInfo.CurrentCulture);
        }
        if (absoluteValue < 100000.0)
        {
            return Convert.ToString(Math.Floor(absoluteValue / 1000.0) * 1000.0, CultureInfo.CurrentCulture);
        }
        if (absoluteValue < 1000000.0)
        {
            return Convert.ToString(Math.Floor(absoluteValue / 10000.0) * 10000.0, CultureInfo.CurrentCulture);
        }
        if (absoluteValue < 10000000.0)
        {
            return Convert.ToString(Math.Floor(absoluteValue / 100000.0) * 100000.0, CultureInfo.CurrentCulture);
        }
        if (absoluteValue < 100000000.0)
        {
            return Convert.ToString(Math.Floor(absoluteValue / 1000000.0) * 1000000.0, CultureInfo.CurrentCulture);
        }
        return Convert.ToString(Math.Floor(absoluteValue / 10000000.0) * 10000000.0, CultureInfo.CurrentCulture);
    }

    public static FileInfo? GetLatestJournalFile(string? path)
    {
        return GetJournalFiles(path)?.FirstOrDefault();
    }

    public static FileInfo[]? GetJournalFiles(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }
        if (!Directory.Exists(path))
        {
            return null;
        }
        FileInfo[] files = new DirectoryInfo(path).GetFiles("Journal.*.log");
        if (files.Length < 1)
        {
            return null;
        }
        return files.OrderByDescending((FileInfo f) => f.LastWriteTime).ToArray();
    }

    public static T ConvertJObjectValue<T>(JsonNode? jObject, string journalValueName, T journalValueForNull = default!)
    {
        try
        {
            if (jObject is JsonObject obj && obj.TryGetPropertyValue(journalValueName, out var journalValue) && journalValue != null)
            {
                if (typeof(T) == typeof(JsonObject))
                {
                    return (T)(object)journalValue.AsObject();
                }
                if (typeof(T) == typeof(JsonArray))
                {
                    return (T)(object)journalValue.AsArray();
                }
                if (IsIntegerType(typeof(T)) && journalValue is JsonValue numberValue && numberValue.TryGetValue<double>(out var doubleValue))
                {
                    return (T)Convert.ChangeType(doubleValue, typeof(T));
                }
                var result = JsonSerializer.Deserialize<T>(journalValue.ToJsonString());
                if (result != null)
                {
                    return result;
                }
                log.Debug($"Value is null for '{journalValueName}' of type {typeof(T)} while converting JSON: {jObject}, returning replacement/default journalValue '{journalValueForNull}' instead");
            }
        }
        catch (Exception exception)
        {
            log.Warn($"Cannot convert journalValue with name '{journalValueName}' to {typeof(T)} journalValue, JSON: {jObject}, returning replacement/default journalValue '{journalValueForNull}' instead: {exception.Message}");
        }
        return journalValueForNull;
    }

    private static bool IsIntegerType(Type type)
    {
        return type == typeof(long) || type == typeof(ulong)
            || type == typeof(int) || type == typeof(uint)
            || type == typeof(short) || type == typeof(ushort)
            || type == typeof(byte) || type == typeof(sbyte);
    }

    public static (int? parentStarId, int? parentPlanetId) DetermineParentIdsOfBody(JsonNode? jObject, DataSource dataSource)
    {
        int? parentStarId = null;
        int? parentPlanetId = null;
        string journalValueName = (dataSource == DataSource.Journal) ? "Parents" : "parents";
        try
        {
            foreach (JsonObject? parentObject in ConvertJObjectValue(jObject, journalValueName, new JsonArray()).OfType<JsonObject>())
            {
                if (parentObject == null)
                {
                    continue;
                }
                if (!parentStarId.HasValue)
                {
                    parentStarId = ConvertJObjectValue<int?>(parentObject, "Star");
                }
                if (!parentPlanetId.HasValue)
                {
                    parentPlanetId = ConvertJObjectValue<int?>(parentObject, "Planet");
                }
                if (parentStarId.HasValue && parentPlanetId.HasValue)
                {
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            log.Debug($"Cannot determine parent IDs for JSON token {jObject}", exception);
        }
        return (parentStarId: parentStarId, parentPlanetId: parentPlanetId);
    }

    public static bool CheckStarClassForScoopable(string? starClass)
    {
        if (!string.IsNullOrEmpty(starClass))
        {
            string[] starClassParts = starClass.Split(' ');
            if (starClassParts.Length != 0 && _scoopableStarClasses.Contains(starClassParts[0]))
            {
                return true;
            }
        }
        return false;
    }

    public static bool CalculateDistanceBetweenSystems(StarSystem firstStarSystem, StarSystem secondStarSystem, out double distanceInLightYears)
    {
        try
        {
            double dx = (firstStarSystem.StarPositionX - secondStarSystem.StarPositionX)!.Value;
            double dy = (firstStarSystem.StarPositionY - secondStarSystem.StarPositionY)!.Value;
            double dz = (firstStarSystem.StarPositionZ - secondStarSystem.StarPositionZ)!.Value;
            distanceInLightYears = Math.Sqrt(Math.Pow(dx, 2.0) + Math.Pow(dy, 2.0) + Math.Pow(dz, 2.0));
            return true;
        }
        catch (Exception exception)
        {
            log.Error($"Cannot calculate distance between starsystems {firstStarSystem} and {secondStarSystem}", exception);
            distanceInLightYears = 0.0;
            return false;
        }
    }

    public static List<string> GetEdsmValuesFromJournalValue(string journalValue, ImmutableDictionary<string, string> edsmToJournalDictionary)
    {
        return (from edsmPair in edsmToJournalDictionary
                where edsmPair.Value == journalValue
                select edsmPair.Key).ToList();
    }

    public static List<string> GetUniqueJournalValues(ImmutableDictionary<string, string> edsmToJournalDictionary)
    {
        List<string> uniqueValues = new List<string>();
        foreach (string journalValue in edsmToJournalDictionary.Values)
        {
            if (!uniqueValues.Contains(journalValue))
            {
                uniqueValues.Add(journalValue);
            }
        }
        uniqueValues.Sort();
        return uniqueValues;
    }

    public static RingType GetRingType(string? dataSourceDescription)
    {
        if (string.IsNullOrEmpty(dataSourceDescription))
        {
            return RingType.Unknown;
        }
        string clean = dataSourceDescription;
        if (clean.StartsWith("eRingClass_", StringComparison.OrdinalIgnoreCase))
        {
            clean = clean.Substring("eRingClass_".Length);
        }
        return clean switch
        {
            "MetalRich" => RingType.MetalRich,
            "Metalic" or "Metallic" => RingType.Metallic,
            "Rocky" => RingType.Rocky,
            "Icy" => RingType.Icy,
            _ => RingType.Unknown
        };
    }

    public static string GetRingTypeSourceDesciption(DataSource dataSource, RingType ringType)
    {
        if (Globals.RingTypeDataSourceDescriptions.TryGetValue(dataSource, out var descriptions) && descriptions != null && descriptions.TryGetValue(ringType, out var description) && !string.IsNullOrEmpty(description))
        {
            return description;
        }
        return string.Empty;
    }

    public static string GetRingTypeName(RingType ringType)
    {
        return ringType switch
        {
            RingType.MetalRich => Resources.RingType_MetalRich,
            RingType.Metallic => Resources.RingType_Metallic,
            RingType.Rocky => Resources.RingType_Rocky,
            RingType.Icy => Resources.RingType_Icy,
            _ => Resources.RingType_Unknown
        };
    }

    public static string GetRingReserveLevelName(RingReserveLevel ringReserveLevel)
    {
        return ringReserveLevel switch
        {
            RingReserveLevel.Pristine => Resources.RingReserveLevel_Pristine,
            RingReserveLevel.Major => Resources.RingReserveLevel_Major,
            RingReserveLevel.Common => Resources.RingReserveLevel_Common,
            RingReserveLevel.Low => Resources.RingReserveLevel_Low,
            RingReserveLevel.Depleted => Resources.RingReserveLevel_Depleted,
            _ => Resources.RingReserveLevel_Unknown
        };
    }

    public static RingReserveLevel GetRingReserveLevel(string? dataSourceDescription)
    {
        if (string.IsNullOrEmpty(dataSourceDescription))
        {
            return RingReserveLevel.Unknown;
        }
        try
        {
            return (RingReserveLevel)Enum.Parse(typeof(RingReserveLevel), dataSourceDescription.Replace("Resources", string.Empty));
        }
        catch (Exception exception)
        {
            log.Debug("Could not get RingReserveLevel for " + dataSourceDescription + ", returning 'Unknown'", exception);
        }
        return RingReserveLevel.Unknown;
    }

    public static bool OpenHyperlink(Hyperlink? hyperlink)
    {
        try
        {
            if (hyperlink?.NavigateUri != null)
            {
                Process.Start(new ProcessStartInfo(hyperlink.NavigateUri.ToString())
                {
                    UseShellExecute = true
                });
                return true;
            }
        }
        catch (Exception exception)
        {
            log.Error("Error on opening hyperlink.", exception);
        }
        return false;
    }

    static Helpsters()
    {
        _atmosphereAndVolcanismDescriptors = new[]
        {
            "Thin ",
            "Thick ",
            "Hot thin",
            "Hot thick",
            "Hot",
            "Minor ",
            "Major "
        }.ToImmutableList();

        _scoopableStarClasses = new[]
        {
            "O",
            "B",
            "A",
            "F",
            "G",
            "K",
            "M",
            "Refuel"
        }.ToImmutableList();
    }
}
