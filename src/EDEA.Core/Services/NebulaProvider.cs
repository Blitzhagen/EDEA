using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using log4net;

namespace EDEA.Services;

/// <summary>
/// Provides nebula, guardian-zone and tuber-zone lookups. Port of the BioScan nebula_data and bio_data regions data.
/// </summary>
public static class NebulaProvider
{
    /// <summary>The logger for this class.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(NebulaProvider));

    private static Dictionary<string, (double x, double y, double z)> _large = new();
    private static Dictionary<string, (double x, double y, double z)> _planetary = new();
    private static List<string> _sectors = new();
    private static Dictionary<string, (double maxDistance, double x, double y, double z)> _guardianNebulae = new();
    private static Dictionary<string, (double minDistance, double maxDistance, double x, double y, double z)> _tuberZones = new();
    private static bool _loaded;

    /// <summary>
    /// Ensures the nebula data is loaded from nebulae.json.
    /// </summary>
    private static void EnsureLoaded()
    {
        if (_loaded)
        {
            return;
        }
        _loaded = true;
        try
        {
            string file = Path.Combine(Globals.ApplicationFolder, "Resources", "nebulae.json");
            if (!File.Exists(file))
            {
                file = Path.Combine(Globals.ApplicationFolder, "nebulae.json");
            }
            if (!File.Exists(file))
            {
                log.Warn("nebulae.json not found - nebula/guardian/tuber rules will not be evaluated");
                return;
            }
            if (JsonNode.Parse(File.ReadAllText(file)) is not JsonObject root)
            {
                return;
            }
            _large = ReadCoordinates(root["large"] as JsonObject);
            _planetary = ReadCoordinates(root["planetary"] as JsonObject);
            if (root["sectors"] is JsonArray sectors)
            {
                _sectors = sectors.OfType<JsonValue>()
                    .Select(v => v.GetValue<string>())
                    .ToList();
            }
            if (root["guardianNebulae"] is JsonObject guardians)
            {
                foreach (var entry in guardians)
                {
                    if (entry.Value is JsonArray data && data.Count == 2 && data[1] is JsonArray coords)
                    {
                        _guardianNebulae[entry.Key] = (data[0]!.GetValue<double>(), coords[0]!.GetValue<double>(), coords[1]!.GetValue<double>(), coords[2]!.GetValue<double>());
                    }
                }
            }
            if (root["tuberZones"] is JsonObject tubers)
            {
                foreach (var entry in tubers)
                {
                    if (entry.Value is JsonArray data && data.Count == 2
                        && data[0] is JsonArray distances && data[1] is JsonArray coords)
                    {
                        _tuberZones[entry.Key] = (distances[0]!.GetValue<double>(), distances[1]!.GetValue<double>(),
                            coords[0]!.GetValue<double>(), coords[1]!.GetValue<double>(), coords[2]!.GetValue<double>());
                    }
                }
            }
            log.Info($"Nebula data loaded ({_large.Count} large, {_planetary.Count} planetary, {_sectors.Count} sectors)");
        }
        catch (Exception exception)
        {
            log.Error("Error reading nebula data!", exception);
        }
    }

    private static Dictionary<string, (double x, double y, double z)> ReadCoordinates(JsonObject? obj)
    {
        var result = new Dictionary<string, (double x, double y, double z)>();
        if (obj == null)
        {
            return result;
        }
        foreach (var entry in obj)
        {
            if (entry.Value is JsonArray coords && coords.Count == 3)
            {
                result[entry.Key] = (coords[0]!.GetValue<double>(), coords[1]!.GetValue<double>(), coords[2]!.GetValue<double>());
            }
        }
        return result;
    }

    /// <summary>
    /// Calculates the distance between two galactic coordinates.
    /// </summary>
    /// <param name="a">The first coordinate.</param>
    /// <param name="b">The second coordinate.</param>
    /// <returns>The distance in light years.</returns>
    public static double SystemDistance((double x, double y, double z) a, (double x, double y, double z) b)
    {
        double dx = a.x - b.x;
        double dy = a.y - b.y;
        double dz = a.z - b.z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    /// <summary>
    /// Checks whether the given system name is inside a known nebula sector.
    /// </summary>
    /// <param name="systemName">The system name.</param>
    /// <returns><see langword="true"/> when the system name starts with a nebula sector name.</returns>
    public static bool IsInNebulaSector(string systemName)
    {
        EnsureLoaded();
        return _sectors.Any(sector => systemName.StartsWith(sector, StringComparison.Ordinal));
    }

    /// <summary>
    /// Checks the BioScan "nebula" rule: nebula sector by name, then nearest large nebula within 150 ly,
    /// and for "all" additionally nearest planetary nebula within 100 ly.
    /// </summary>
    /// <param name="systemName">The system name.</param>
    /// <param name="position">The system coordinates.</param>
    /// <param name="checkType">The rule value ("all" or "large").</param>
    /// <returns><see langword="true"/> when the system is in or near a nebula.</returns>
    public static bool IsInNebula(string systemName, (double x, double y, double z)? position, string checkType)
    {
        EnsureLoaded();
        if (checkType is not ("all" or "large"))
        {
            return false;
        }
        if (IsInNebulaSector(systemName))
        {
            return true;
        }
        if (!position.HasValue)
        {
            return false;
        }
        var pos = position.Value;
        if (_large.Values.Any(coords => SystemDistance(pos, coords) < 150.0))
        {
            return true;
        }
        if (checkType == "all" && _planetary.Values.Any(coords => SystemDistance(pos, coords) < 100.0))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks whether the given coordinates are inside a known guardian zone.
    /// </summary>
    /// <param name="position">The system coordinates.</param>
    /// <returns><see langword="true"/> when within a guardian zone.</returns>
    public static bool IsInGuardianZone((double x, double y, double z) position)
    {
        EnsureLoaded();
        return _guardianNebulae.Values.Any(zone => SystemDistance(position, (zone.x, zone.y, zone.z)) < zone.maxDistance);
    }

    /// <summary>
    /// Checks whether the given coordinates are inside a tuber zone (for Sinuous Tubers).
    /// </summary>
    /// <param name="position">The system coordinates.</param>
    /// <param name="zoneNames">The zone names to check, or "Any" for all zones.</param>
    /// <returns><see langword="true"/> when inside a matching zone's distance band.</returns>
    public static bool IsInTuberZone((double x, double y, double z) position, JsonNode? zoneNames)
    {
        EnsureLoaded();
        bool any = zoneNames is JsonValue value && value.GetValue<string>() == "Any";
        var wanted = new HashSet<string>();
        if (zoneNames is JsonArray array)
        {
            foreach (JsonNode? node in array)
            {
                if (node is JsonValue v && v.TryGetValue<string>(out string? name))
                {
                    wanted.Add(name);
                }
            }
        }
        if (!any && wanted.Count == 0)
        {
            return false;
        }
        foreach (var zone in _tuberZones)
        {
            if (any || wanted.Contains(zone.Key))
            {
                double distance = SystemDistance(position, (zone.Value.x, zone.Value.y, zone.Value.z));
                if (zone.Value.minDistance <= distance && distance <= zone.Value.maxDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
