using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using log4net;

namespace EDEA.Services;

/// <summary>
/// Maps galactic coordinates to galactic regions. Port of the ExploData RegionMap.
/// </summary>
public static class GalacticRegionProvider
{
    /// <summary>The logger for this class.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(GalacticRegionProvider));

    private const int X0 = -49985;
    private const int Y0 = -40985;
    private const int Z0 = -24105;

    private static List<string?> _regions = new();
    private static List<List<(int length, int regionId)>> _regionMap = new();
    private static bool _loaded;

    /// <summary>
    /// Ensures the region data is loaded from regions.json.
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
            string file = Path.Combine(Globals.ApplicationFolder, "Resources", "regions.json");
            if (!File.Exists(file))
            {
                file = Path.Combine(Globals.ApplicationFolder, "regions.json");
            }
            if (!File.Exists(file))
            {
                log.Warn("regions.json not found - region rules will not be evaluated");
                return;
            }
            if (JsonNode.Parse(File.ReadAllText(file)) is not JsonObject root)
            {
                return;
            }
            if (root["regions"] is JsonArray regions)
            {
                foreach (JsonNode? node in regions)
                {
                    _regions.Add(node?.GetValue<string>());
                }
            }
            if (root["regionmap"] is JsonArray map)
            {
                foreach (JsonNode? rowNode in map)
                {
                    var row = new List<(int length, int regionId)>();
                    if (rowNode is JsonArray rowArray)
                    {
                        foreach (JsonNode? cellNode in rowArray)
                        {
                            if (cellNode is JsonArray cell && cell.Count == 2)
                            {
                                row.Add((cell[0]!.GetValue<int>(), cell[1]!.GetValue<int>()));
                            }
                        }
                    }
                    _regionMap.Add(row);
                }
            }
            log.Info($"Galactic region map loaded ({_regions.Count} regions, {_regionMap.Count} rows)");
        }
        catch (Exception exception)
        {
            log.Error("Error reading galactic region map!", exception);
        }
    }

    /// <summary>
    /// Finds the galactic region for the given coordinates. Port of RegionMap.findRegion.
    /// </summary>
    /// <param name="x">The galactic X coordinate.</param>
    /// <param name="y">The galactic Y coordinate.</param>
    /// <param name="z">The galactic Z coordinate.</param>
    /// <returns>The region id and name, or <see langword="null"/> id when outside the map.</returns>
    public static (int? id, string? name) FindRegion(double x, double y, double z)
    {
        EnsureLoaded();
        int px = (int)((x - X0) * 83 / 4096);
        int pz = (int)((z - Z0) * 83 / 4096);
        if (px < 0 || pz < 0 || pz >= _regionMap.Count)
        {
            return (null, null);
        }
        int regionId = 0;
        int rx = 0;
        foreach ((int length, int pv) in _regionMap[pz])
        {
            if (px < rx + length)
            {
                regionId = pv;
                break;
            }
            rx += length;
        }
        if (regionId == 0 || regionId >= _regions.Count)
        {
            return (null, null);
        }
        return (regionId, _regions[regionId]);
    }

    /// <summary>
    /// Finds the galactic region for the boxel of a system address (Id64). Port of RegionMap.findRegionForBoxel.
    /// </summary>
    /// <param name="id64">The system address.</param>
    /// <returns>The region id and name, or <see langword="null"/> id when outside the map.</returns>
    public static (int? id, string? name) FindRegionForBoxel(long id64)
    {
        int masscode = (int)(id64 & 7);
        double z = (((id64 >> 3) & (0x3FFF >> masscode)) << masscode) * 10 + Z0;
        double y = (((id64 >> (17 - masscode)) & (0x1FFF >> masscode)) << masscode) * 10 + Y0;
        double x = (((id64 >> (30 - masscode * 2)) & (0x3FFF >> masscode)) << masscode) * 10 + X0;
        return FindRegion(x, y, z);
    }

    /// <summary>
    /// Checks whether the given region identifier is covered by a named region rule.
    /// </summary>
    /// <param name="ruleName">The region rule name (e.g. "orion-cygnus").</param>
    /// <param name="regionId">The galactic region identifier.</param>
    /// <param name="regionMap">The region rule map from the biology catalog.</param>
    /// <returns><see langword="true"/> when the region is in the rule's region list.</returns>
    public static bool IsInRegionRule(string ruleName, int? regionId, IReadOnlyDictionary<string, List<int>> regionMap)
    {
        return regionId.HasValue && regionMap.TryGetValue(ruleName, out List<int>? ids) && ids.Contains(regionId.Value);
    }
}
