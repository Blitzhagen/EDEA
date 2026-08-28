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

/// <summary>
/// Provides general utility helper methods used across the application.
/// </summary>
public static class Helpsters
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Helpsters));

    /// <summary>
    /// The list of atmosphere and volcanism descriptors to remove from input strings.
    /// </summary>
    private static readonly ImmutableList<string> _atmosphereAndVolcanismDescriptors;

    /// <summary>
    /// The list of star classes that are considered scoopable for fuel.
    /// </summary>
    private static readonly ImmutableList<string> _scoopableStarClasses;

    /// <summary>
    /// Converts the first letter of a string to uppercase.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The input string with the first letter in uppercase, or <see cref="string.Empty"/> when the input is too short.</returns>
    public static string FirstLetterToUpperCase(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= 1)
        {
            return string.Empty;
        }
        return char.ToUpper(input[0]) + input.Substring(1, input.Length - 1);
    }

    /// <summary>
    /// Removes known atmosphere and volcanism descriptors from the start of the input string.
    /// </summary>
    /// <param name="input">The string to clean.</param>
    /// <returns>The input string without the leading descriptor, or the original input when no descriptor matches.</returns>
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

    /// <summary>
    /// Rounds a double value to a human-readable precision depending on its magnitude.
    /// </summary>
    /// <param name="journalValue">The double value to round.</param>
    /// <returns>A rounded string representation of the value in the current culture.</returns>
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

    /// <summary>
    /// Returns the most recently written journal file in the specified folder.
    /// </summary>
    /// <param name="path">The folder to search for journal files.</param>
    /// <returns>The latest <see cref="FileInfo"/> or <c>null</c> when no journal files are found.</returns>
    public static FileInfo? GetLatestJournalFile(string? path)
    {
        return GetJournalFiles(path)?.FirstOrDefault();
    }

    /// <summary>
    /// Returns all journal files in the specified folder ordered by last write time.
    /// </summary>
    /// <param name="path">The folder to search for journal files.</param>
    /// <returns>An array of <see cref="FileInfo"/> objects or <c>null</c> when no journal files are found.</returns>
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

    /// <summary>
    /// Converts a JSON node value to the specified type.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="jObject">The JSON object containing the value.</param>
    /// <param name="journalValueName">The name of the property to read.</param>
    /// <param name="journalValueForNull">The fallback value when the property is missing or cannot be converted.</param>
    /// <returns>The converted value, or <paramref name="journalValueForNull"/> when conversion fails.</returns>
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

    /// <summary>
    /// Determines whether the specified type is an integer type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is an integer type; otherwise, <c>false</c>.</returns>
    private static bool IsIntegerType(Type type)
    {
        return type == typeof(long) || type == typeof(ulong)
            || type == typeof(int) || type == typeof(uint)
            || type == typeof(short) || type == typeof(ushort)
            || type == typeof(byte) || type == typeof(sbyte);
    }

    /// <summary>
    /// Determines the parent star and planet IDs of a body from JSON data.
    /// </summary>
    /// <param name="jObject">The JSON object containing the body data.</param>
    /// <param name="dataSource">The data source that indicates the parent property name format.</param>
    /// <returns>A tuple with the parent star ID and parent planet ID.</returns>
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

    /// <summary>
    /// Checks whether the given star class is scoopable.
    /// </summary>
    /// <param name="starClass">The star class to check.</param>
    /// <returns><c>true</c> when the first part of the star class is in the scoopable star class list; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Calculates the distance in light years between two star systems.
    /// </summary>
    /// <param name="firstStarSystem">The first star system.</param>
    /// <param name="secondStarSystem">The second star system.</param>
    /// <param name="distanceInLightYears">When the method returns, contains the calculated distance.</param>
    /// <returns><c>true</c> when the distance could be calculated; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Returns the EDSM keys that map to the specified journal value.
    /// </summary>
    /// <param name="journalValue">The journal value to look up.</param>
    /// <param name="edsmToJournalDictionary">The dictionary mapping EDSM values to journal values.</param>
    /// <returns>A list of EDSM keys matching the journal value.</returns>
    public static List<string> GetEdsmValuesFromJournalValue(string journalValue, ImmutableDictionary<string, string> edsmToJournalDictionary)
    {
        return (from edsmPair in edsmToJournalDictionary
                where edsmPair.Value == journalValue
                select edsmPair.Key).ToList();
    }

    /// <summary>
    /// Returns a sorted list of unique journal values from the dictionary.
    /// </summary>
    /// <param name="edsmToJournalDictionary">The dictionary mapping EDSM values to journal values.</param>
    /// <returns>A sorted list of unique journal values.</returns>
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

    /// <summary>
    /// Parses a data source ring type description into a <see cref="RingType"/>.
    /// </summary>
    /// <param name="dataSourceDescription">The ring type description to parse.</param>
    /// <returns>The corresponding <see cref="RingType"/> or <see cref="RingType.Unknown"/> when unknown.</returns>
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

    /// <summary>
    /// Returns the data source description for the specified ring type.
    /// </summary>
    /// <param name="dataSource">The data source to use for the lookup.</param>
    /// <param name="ringType">The ring type to describe.</param>
    /// <returns>The data source description, or <see cref="string.Empty"/> when not found.</returns>
    public static string GetRingTypeSourceDesciption(DataSource dataSource, RingType ringType)
    {
        if (Globals.RingTypeDataSourceDescriptions.TryGetValue(dataSource, out var descriptions) && descriptions != null && descriptions.TryGetValue(ringType, out var description) && !string.IsNullOrEmpty(description))
        {
            return description;
        }
        return string.Empty;
    }

    /// <summary>
    /// Returns the localized name for the specified ring type.
    /// </summary>
    /// <param name="ringType">The ring type to name.</param>
    /// <returns>The localized ring type name.</returns>
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

    /// <summary>
    /// Returns the localized name for the specified ring reserve level.
    /// </summary>
    /// <param name="ringReserveLevel">The ring reserve level to name.</param>
    /// <returns>The localized ring reserve level name.</returns>
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

    /// <summary>
    /// Parses a data source ring reserve level description into a <see cref="RingReserveLevel"/>.
    /// </summary>
    /// <param name="dataSourceDescription">The ring reserve level description to parse.</param>
    /// <returns>The corresponding <see cref="RingReserveLevel"/> or <see cref="RingReserveLevel.Unknown"/> when unknown.</returns>
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

    /// <summary>
    /// Opens the URI associated with a hyperlink using the default browser.
    /// </summary>
    /// <param name="hyperlink">The hyperlink to open.</param>
    /// <returns><c>true</c> when the hyperlink was opened; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Initializes the static descriptor and star class lists.
    /// </summary>
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
