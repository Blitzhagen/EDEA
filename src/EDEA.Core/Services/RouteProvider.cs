using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EDEA.Models;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Services;

/// <summary>Represents a method that handles the SystemsOnRouteChanged event.</summary>
/// <param name="sender">The source of the event.</param>
/// <param name="systemsOnRoute">The ConcurrentDictionary<long, StarSystem> value of the systemsOnRoute parameter.</param>
/// <param name="firstRead">The bool value of the firstRead parameter.</param>
public delegate void SystemsOnRouteChangedEventHandler(object? sender, ConcurrentDictionary<long, StarSystem> systemsOnRoute, bool firstRead);

/// <summary>Represents the RouteProvider class.</summary>
public class RouteProvider
{
    /// <summary>The instance field.</summary>
    private static RouteProvider? instance;

    /// <summary>The log field.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(RouteProvider));

    /// <summary>The _starSystemProvider field.</summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>The _fileWatcher field.</summary>
    private readonly EDFileWatcher _fileWatcher;

    /// <summary>The _journalProvider field.</summary>
    private readonly JournalProvider _journalProvider;

    /// <summary>The readStarsSystemsTask field.</summary>
    private Task? readStarsSystemsTask;

    /// <summary>The _plotterJumps field.</summary>
    private JsonArray? _plotterJumps;
    /// <summary>The _isLocked field.</summary>
    private bool _isLocked;
    /// <summary>The _lockedRouteFilePath field.</summary>
    private readonly string _lockedRouteFilePath;

    /// <summary>Gets the Route.</summary>
    /// <value>A List<RouteView> value.</value>
    public List<RouteView> Route { get; } = new List<RouteView>();

    /// <summary>Gets the IsCustomRoute.</summary>
    /// <value>A bool value.</value>
    public bool IsCustomRoute => _plotterJumps != null && _plotterJumps.Count > 1;

    /// <summary>Gets the IsLocked.</summary>
    /// <value>A bool value.</value>
    public bool IsLocked
    {
        get => _isLocked;
        private set
        {
            _isLocked = value;
            log.Info($"Route {(value ? "locked" : "unlocked")}");
        }
    }

    /// <summary>Performs the saveLockedRoute operation.</summary>
    private void saveLockedRoute()
    {
        try
        {
            if (_plotterJumps == null)
            {
                return;
            }

            JsonObject lockedRoute = new JsonObject
            {
                ["isLocked"] = true,
                ["jumps"] = _plotterJumps
            };
            Directory.CreateDirectory(Path.GetDirectoryName(_lockedRouteFilePath)!);
            File.WriteAllText(_lockedRouteFilePath, lockedRoute.ToJsonString());
            log.Debug($"Saved locked route to {_lockedRouteFilePath}");
        }
        catch (Exception exception)
        {
            log.Error($"Cannot save locked route to {_lockedRouteFilePath}", exception);
        }
    }

    /// <summary>Performs the deleteLockedRouteFile operation.</summary>
    private void deleteLockedRouteFile()
    {
        try
        {
            if (File.Exists(_lockedRouteFilePath))
            {
                File.Delete(_lockedRouteFilePath);
                log.Debug($"Deleted locked route file {_lockedRouteFilePath}");
            }
        }
        catch (Exception exception)
        {
            log.Error($"Cannot delete locked route file {_lockedRouteFilePath}", exception);
        }
    }

    /// <summary>Performs the loadLockedRoute operation.</summary>
    private void loadLockedRoute()
    {
        try
        {
            if (!File.Exists(_lockedRouteFilePath))
            {
                return;
            }

            string content = File.ReadAllText(_lockedRouteFilePath);
            JsonNode? node = JsonNode.Parse(content);
            if (node is not JsonObject routeObject || !routeObject.ContainsKey("jumps"))
            {
                return;
            }

            if (routeObject["jumps"] is not JsonArray jumps)
            {
                return;
            }

            _plotterJumps = jumps;
            IsLocked = true;
            log.Info($"Loaded locked route from {_lockedRouteFilePath}");
        }
        catch (Exception exception)
        {
            log.Error($"Cannot load locked route from {_lockedRouteFilePath}", exception);
        }
    }

    /// <summary>Occurs when the RouteChanged event is raised.</summary>
    public event Action? RouteChanged = delegate { };

    /// <summary>Occurs when the SystemsOnRouteChanged event is raised.</summary>
    public event SystemsOnRouteChangedEventHandler SystemsOnRouteChanged = delegate { };

    /// <summary>Initializes a new instance of the RouteProvider class.</summary>
    /// <param name="eDFileWatcher">The EDFileWatcher value of the eDFileWatcher parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    /// <param name="journalProvider">The JournalProvider value of the journalProvider parameter.</param>
    private RouteProvider(EDFileWatcher eDFileWatcher, StarSystemProvider starSystemProvider, JournalProvider journalProvider)
    {
        _fileWatcher = eDFileWatcher;
        _starSystemProvider = starSystemProvider;
        _starSystemProvider.RegisterProvider(this);
        _journalProvider = journalProvider;
        _lockedRouteFilePath = Path.Combine(Globals.AppDataFolder, "lockedroute.json");
        readStarsSystemsTask = firstRead();
    }

    /// <summary>Performs the Instance operation.</summary>
    /// <param name="eDFileWatcher">The EDFileWatcher value of the eDFileWatcher parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    /// <param name="journalProvider">The JournalProvider value of the journalProvider parameter.</param>
    /// <returns>A RouteProvider result.</returns>
    public static RouteProvider Instance(EDFileWatcher eDFileWatcher, StarSystemProvider starSystemProvider, JournalProvider journalProvider)
    {
        if (instance == null)
        {
            instance = new RouteProvider(eDFileWatcher, starSystemProvider, journalProvider);
        }
        return instance;
    }

    /// <summary>Removes PlotterRoute.</summary>
    public void DeletePlotterRoute()
    {
        if (IsLocked)
        {
            log.Warn("Cannot delete plotter route while it is locked");
            return;
        }
        _plotterJumps = null;
        readStarsSystemsTask = ReadStarsSystems();
    }

    /// <summary>Imports PlotterRoute.</summary>
    /// <param name="plotterRouteJumps">The JsonArray value of the plotterRouteJumps parameter.</param>
    /// <returns>A bool result.</returns>
    public bool ImportPlotterRoute(JsonArray plotterRouteJumps)
    {
        try
        {
            if (IsLocked)
            {
                log.Warn("Cannot import plotter route while it is locked");
                return false;
            }
            if (plotterRouteJumps == null || plotterRouteJumps.Count < 2)
            {
                log.Warn("Invalid plotter route, at least two jumps required");
                return false;
            }

            log.Info("Importing plotter route in memory");
            _plotterJumps = plotterRouteJumps;
            int jumpIndex = 0;
            foreach (JsonNode? node in plotterRouteJumps)
            {
                if (node is JsonObject jump)
                {
                    log.Info($"  Jump {jumpIndex}: {Helpsters.ConvertJObjectValue<string>(jump, "name")}, " +
                        $"{Helpsters.ConvertJObjectValue(jump, "distance", 0.0):F2} Ly" +
                        (Helpsters.ConvertJObjectValue(jump, "has_neutron", false) ? ", neutron" : string.Empty) +
                        (Helpsters.ConvertJObjectValue(jump, "is_refuel", false) ? ", refuel" : string.Empty) +
                        (Helpsters.ConvertJObjectValue(jump, "is_scoopable", false) ? ", scoopable" : string.Empty));
                }
                jumpIndex++;
            }
            readStarsSystemsTask = ReadStarsSystems();
            return true;
        }
        catch (Exception exception)
        {
            log.Error("Can not import plotter route", exception);
        }
        return false;
    }

    /// <summary>Performs the LockRoute operation.</summary>
    public void LockRoute()
    {
        IsLocked = true;
        saveLockedRoute();
    }

    /// <summary>Unlocks the route by deleting the locked route file without clearing the in-memory plotter route.</summary>
    public void UnlockRoute()
    {
        IsLocked = false;
        deleteLockedRouteFile();
    }

    /// <summary>Imports SpanshRouteFile.</summary>
    /// <param name="filePath">The string value of the filePath parameter.</param>
    /// <returns>A bool result.</returns>
    public bool ImportSpanshRouteFile(string filePath)
    {
        try
        {
            if (IsLocked)
            {
                log.Warn("Cannot import route while it is locked");
                return false;
            }
            if (!File.Exists(filePath))
            {
                log.Warn($"Spansh route file not found: {filePath}");
                return false;
            }

            string fileContent = File.ReadAllText(filePath);
            JsonArray? jumps = null;
            if (string.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase))
            {
                jumps = ParseSpanshJsonRoute(fileContent);
            }
            else if (string.Equals(Path.GetExtension(filePath), ".csv", StringComparison.OrdinalIgnoreCase))
            {
                jumps = ParseSpanshCsvRoute(fileContent);
            }
            else
            {
                log.Warn($"Unsupported file type for Spansh route import: {Path.GetExtension(filePath)}");
                return false;
            }

            return jumps != null && ImportPlotterRoute(jumps);
        }
        catch (Exception exception)
        {
            log.Error($"Can not import Spansh route from {filePath}", exception);
            return false;
        }
    }

    /// <summary>Performs the ParseSpanshJsonRoute operation.</summary>
    /// <param name="fileContent">The string value of the fileContent parameter.</param>
    /// <returns>A JsonArray? result.</returns>
    private static JsonArray? ParseSpanshJsonRoute(string fileContent)
    {
        try
        {
            JsonNode? routeJson = JsonNode.Parse(fileContent);
            if (routeJson is not JsonObject routeObj)
            {
                return null;
            }

            JsonNode? resultNode = Helpsters.ConvertJObjectValue<JsonNode?>(routeObj, "result");
            if (resultNode is not JsonObject resultObj)
            {
                return null;
            }

            JsonNode? jumpsNode = Helpsters.ConvertJObjectValue<JsonNode?>(resultObj, "system_jumps");
            if (jumpsNode is not JsonArray jumps)
            {
                return null;
            }

            JsonArray plotterJumps = new JsonArray();
            foreach (JsonNode? node in jumps)
            {
                if (node is not JsonObject item)
                {
                    continue;
                }

                long systemId = Helpsters.ConvertJObjectValue(item, "id64", 0L);
                string? systemName = Helpsters.ConvertJObjectValue<string?>(item, "system");
                if (systemId == 0L || string.IsNullOrEmpty(systemName))
                {
                    continue;
                }

                double distance = Helpsters.ConvertJObjectValue(item, "distance_jumped", 0.0);
                bool isNeutron = Helpsters.ConvertJObjectValue(item, "neutron_star", false);

                plotterJumps.Add(new JsonObject
                {
                    ["id64"] = systemId,
                    ["name"] = systemName,
                    ["distance"] = distance,
                    ["has_neutron"] = isNeutron,
                    ["is_refuel"] = false,
                    ["is_scoopable"] = false
                });
            }

            return plotterJumps.Count > 1 ? plotterJumps : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>Performs the ParseSpanshCsvRoute operation.</summary>
    /// <param name="fileContent">The string value of the fileContent parameter.</param>
    /// <returns>A JsonArray? result.</returns>
    private static JsonArray? ParseSpanshCsvRoute(string fileContent)
    {
        try
        {
            using var reader = new StringReader(fileContent);
            string? header = reader.ReadLine();
            if (string.IsNullOrEmpty(header))
            {
                return null;
            }

            JsonArray plotterJumps = new JsonArray();
            double previousDistanceToArrival = 0.0;
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] columns = line.Split(',');
                if (columns.Length < 5)
                {
                    continue;
                }

                for (int i = 0; i < columns.Length; i++)
                {
                    columns[i] = columns[i].Trim('"');
                }

                string systemName = columns[0];
                if (string.IsNullOrEmpty(systemName))
                {
                    continue;
                }

                if (!double.TryParse(columns[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double distanceToArrival))
                {
                    continue;
                }

                double distance = plotterJumps.Count == 0 ? 0.0 : distanceToArrival - previousDistanceToArrival;
                previousDistanceToArrival = distanceToArrival;
                bool isNeutron = string.Equals(columns[3], "Yes", StringComparison.OrdinalIgnoreCase);

                plotterJumps.Add(new JsonObject
                {
                    ["id64"] = (long)systemName.GetHashCode(),
                    ["name"] = systemName,
                    ["distance"] = distance,
                    ["has_neutron"] = isNeutron,
                    ["is_refuel"] = false,
                    ["is_scoopable"] = false
                });
            }

            return plotterJumps.Count > 1 ? plotterJumps : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>Performs the firstRead operation.</summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task firstRead()
    {
        loadLockedRoute();
        await ReadStarsSystems(firstRead: true);
        _fileWatcher.NavRouteFileChanged += FileWatcher_NavRouteFileChanged;
    }

    /// <summary>Retrieves StarsSystems.</summary>
    /// <param name="firstRead">The bool value of the firstRead parameter.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task ReadStarsSystems(bool firstRead = false)
    {
        _starSystemProvider.SetRouteIsLoadingStatus(status: true, "Preparing ...");
        ConcurrentDictionary<long, StarSystem> starSystemsOnRoute = new ConcurrentDictionary<long, StarSystem>();
        if (IsCustomRoute)
        {
            log.Info("In-memory plotter route active, using plotter route mode");
            starSystemsOnRoute = await appendStarSystemsFromPlotterJumps(starSystemsOnRoute);
        }
        else
        {
            starSystemsOnRoute = await appendStarSystemsFromNavRouteFile(starSystemsOnRoute);
        }
        BuildRoute(starSystemsOnRoute);
        SystemsOnRouteChanged?.Invoke(this, starSystemsOnRoute, firstRead);
        readStarsSystemsTask = null;
    }

    /// <summary>Creates Route.</summary>
    /// <param name="starSystemsOnRoute">The ConcurrentDictionary<long, StarSystem> value of the starSystemsOnRoute parameter.</param>
    private void BuildRoute(ConcurrentDictionary<long, StarSystem> starSystemsOnRoute)
    {
        Route.Clear();
        StarSystem? previous = null;
        foreach (StarSystem star in starSystemsOnRoute.Values.OrderBy(x => x.JumpDistance))
        {
            var view = new RouteView
            {
                Jump = star.JumpDistance + 1,
                SystemName = star.Name,
                StarClass = star.StarClass ?? string.Empty,
                PrimaryStarIsScoopable = Helpsters.CheckStarClassForScoopable(star.StarClass),
                DiscoveryStatus = string.Empty,
                X = star.StarPositionX,
                Y = star.StarPositionY,
                Z = star.StarPositionZ
            };
            if (previous != null && view.X.HasValue && previous.StarPositionX.HasValue)
            {
                double dx = view.X.Value - previous.StarPositionX.Value;
                double dy = view.Y!.Value - previous.StarPositionY!.Value;
                double dz = view.Z!.Value - previous.StarPositionZ!.Value;
                view.Distance = Math.Sqrt(dx * dx + dy * dy + dz * dz).ToString("F2");
            }
            else if (star.JumpDistanceLy > 0)
            {
                view.Distance = star.JumpDistanceLy.ToString("F2");
            }
            Route.Add(view);
            previous = star;
        }
        RouteChanged?.Invoke();
    }

    /// <summary>Performs the FileWatcher_NavRouteFileChanged operation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void FileWatcher_NavRouteFileChanged(object? sender, EdFileEvent e)
    {
        log.Info("NavRoute.json changed, reloading route");
        if (!IsCustomRoute && !IsLocked)
        {
            readStarsSystemsTask = ReadStarsSystems();
        }
    }

    /// <summary>Performs the appendStarSystemsFromPlotterJumps operation.</summary>
    /// <param name="starSystemsOnRoute">The ConcurrentDictionary<long, StarSystem> value of the starSystemsOnRoute parameter.</param>
    /// <returns>A Task<ConcurrentDictionary<long, StarSystem>> representing the asynchronous operation.</returns>
    private async Task<ConcurrentDictionary<long, StarSystem>> appendStarSystemsFromPlotterJumps(ConcurrentDictionary<long, StarSystem> starSystemsOnRoute)
    {
        await Task.Run(delegate
        {
            try
            {
                if (_plotterJumps == null)
                {
                    return;
                }

                int jumpIndex = 0;
                foreach (JsonNode? node in _plotterJumps)
                {
                    if (node is not JsonObject item)
                    {
                        continue;
                    }

                    long systemId = Helpsters.ConvertJObjectValue(item, "id64", 0L);
                    string? systemName = Helpsters.ConvertJObjectValue<string?>(item, "name");
                    if (systemId == 0L || string.IsNullOrEmpty(systemName))
                    {
                        continue;
                    }

                    string starClass = Globals.PlotterStarClasses["Unknown"];
                    if (Helpsters.ConvertJObjectValue(item, "is_refuel", false))
                    {
                        starClass = Globals.PlotterStarClasses["Refuel"];
                    }
                    else if (Helpsters.ConvertJObjectValue(item, "has_neutron", false))
                    {
                        starClass = Globals.PlotterStarClasses["Neutron"];
                    }
                    else if (Helpsters.ConvertJObjectValue(item, "is_scoopable", false))
                    {
                        starClass = Globals.PlotterStarClasses["Refuel"];
                    }

                    StarSystem starSystem = new StarSystem(systemId, systemName)
                    {
                        JumpDistanceLy = Helpsters.ConvertJObjectValue(item, "distance", 0.0),
                        StarClass = starClass,
                        JumpDistance = jumpIndex
                    };
                    starSystemsOnRoute.TryAdd(starSystem.Id, starSystem);
                    log.Debug($"system {starSystem.Name} ({starSystem.Id}) added to route");
                    jumpIndex++;
                    _starSystemProvider.SetRouteIsLoadingStatus(status: true, $"Reading from route: {jumpIndex} systems");
                }
            }
            catch (Exception exception)
            {
                log.Error("Can not parse in-memory plotter route", exception);
                _plotterJumps = null;
            }
        });
        return starSystemsOnRoute;
    }

    /// <summary>Performs the appendStarSystemsFromNavRouteFile operation.</summary>
    /// <param name="starSystemsOnRoute">The ConcurrentDictionary<long, StarSystem> value of the starSystemsOnRoute parameter.</param>
    /// <returns>A Task<ConcurrentDictionary<long, StarSystem>> representing the asynchronous operation.</returns>
    private async Task<ConcurrentDictionary<long, StarSystem>> appendStarSystemsFromNavRouteFile(ConcurrentDictionary<long, StarSystem> starSystemsOnRoute)
    {
        if (string.IsNullOrEmpty(_fileWatcher.NavRouteFilePath) || !File.Exists(_fileWatcher.NavRouteFilePath))
        {
            return starSystemsOnRoute;
        }

        for (int i = 0; i < 10; i++)
        {
            try
            {
                string routeFileContent;
                using (var stream = File.Open(_fileWatcher.NavRouteFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    routeFileContent = await reader.ReadToEndAsync();
                }
                if (string.IsNullOrEmpty(routeFileContent))
                {
                    break;
                }
                JsonNode? routeJson = JsonNode.Parse(routeFileContent);
                if (routeJson is not JsonObject obj || !obj.ContainsKey("Route"))
                {
                    break;
                }
                log.Info("Route file " + _fileWatcher.NavRouteFilePath + " read");
                JsonArray? routeArray = Helpsters.ConvertJObjectValue<JsonArray?>(obj, "Route");
                if (routeArray == null || routeArray.Count < 2)
                {
                    log.Info("No route found in route file " + _fileWatcher.NavRouteFilePath);
                    break;
                }
                StarSystem? previousSystem = null;
                for (int j = 0; j < routeArray.Count; j++)
                {
                    JsonNode? routeItemNode = routeArray[j];
                    if (routeItemNode is not JsonObject item)
                    {
                        continue;
                    }
                    StarSystem newSystem = new StarSystem(Helpsters.ConvertJObjectValue(item, "SystemAddress", 0L), Helpsters.ConvertJObjectValue<string?>(item, "StarSystem") ?? string.Empty);
                    JsonArray? starPos = Helpsters.ConvertJObjectValue<JsonArray?>(item, "StarPos");
                    if (starPos != null && starPos.Count >= 3)
                    {
                        newSystem.StarPositionX = starPos[0]?.GetValue<double>();
                        newSystem.StarPositionY = starPos[1]?.GetValue<double>();
                        newSystem.StarPositionZ = starPos[2]?.GetValue<double>();
                    }
                    newSystem.StarClass = Helpsters.ConvertJObjectValue<string?>(item, "StarClass") ?? string.Empty;
                    newSystem.JumpDistance = j;
                    StarSystem systemToAdd = newSystem;
                    if (systemToAdd.JumpDistance > 0 && previousSystem != null && Helpsters.CalculateDistanceBetweenSystems(systemToAdd, previousSystem, out double distanceInLightYears))
                    {
                        systemToAdd.JumpDistanceLy = distanceInLightYears;
                    }
                    starSystemsOnRoute.TryAdd(systemToAdd.Id, systemToAdd);
                    previousSystem = systemToAdd;
                    _starSystemProvider.SetRouteIsLoadingStatus(status: true, $"Reading from route file: {j} systems");
                }
            }
            catch (Exception exception)
            {
                if (10 - i > 1)
                {
                    log.Warn($"Route file {_fileWatcher.NavRouteFilePath} missing or locked, waiting for {500}ms, retry {i + 1} of {10}", exception);
                }
                else
                {
                    log.Error($"Route file {_fileWatcher.NavRouteFilePath} missing or locked, giving up after {i + 1} retries", exception);
                }
                await Task.Delay(500);
                continue;
            }
            break;
        }
        return starSystemsOnRoute;
    }
}
