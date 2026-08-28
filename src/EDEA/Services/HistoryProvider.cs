using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EDEA.Models;
using EDEA.Stores;
using log4net;

namespace EDEA.Services;

public class HistoryProvider
{
    private static HistoryProvider? _instance;

    private readonly SQLiteStore _store;
    private readonly ConcurrentDictionary<long, StarSystem> _history;
    private readonly object _historyLock = new object();

    private static readonly ILog log = LogManager.GetLogger(typeof(HistoryProvider));

    public event EventHandler? HistoryUpdated;

    public HistoryProvider(SQLiteStore store)
    {
        _store = store;
        _store.DatabaseStarSystemTableUpdated += _store_DatabaseStarSystemTableUpdated;

        Stopwatch stopwatch = Stopwatch.StartNew();
        List<StarSystem>? starSystems = _store.ReadAllStarSystemBasicData();
        if (starSystems != null)
        {
            _history = new ConcurrentDictionary<long, StarSystem>(starSystems.ToDictionary((StarSystem starSystem) => starSystem.Id, (StarSystem starSystem) => starSystem));
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            log.Info($"Read basic data for {_history.Count} star system(s) from database in {elapsedMilliseconds}ms");
        }
        else
        {
            _history = new ConcurrentDictionary<long, StarSystem>();
            log.Info("Could not read basic data for star system(s) from database, using an empty history instead");
        }
    }

    private void _store_DatabaseStarSystemTableUpdated(object? sender, EventArgs e)
    {
        HistoryUpdated?.Invoke(this, e);
    }

    public static HistoryProvider Instance(SQLiteStore store)
    {
        _instance ??= new HistoryProvider(store);
        return _instance;
    }

    public bool IsStarSystemExisting(long starSystemId)
    {
        if (_history.ContainsKey(starSystemId))
        {
            return true;
        }
        return false;
    }

    public bool TryGetStarSystem(long starSystemId, out StarSystem? starSystem)
    {
        lock (_historyLock)
        {
            if (_history.TryGetValue(starSystemId, out var historySystem))
            {
                StarSystem? storedSystem = _store.ReadStarSystem(starSystemId);
                if (storedSystem != null && storedSystem.Bodies.Count > historySystem.Bodies.Count)
                {
                    _history[starSystemId] = storedSystem;
                }
                starSystem = _history[starSystemId];
                return true;
            }
        }
        starSystem = null;
        return false;
    }

    public IEnumerable<StarSystem> GetAllStarSystems()
    {
        return _history.Values;
    }

    public int AddOrUpdateStarSystem(StarSystem starSystem, bool raiseDatabaseStarSystemTableUpdatedEvent = true)
    {
        if (isStarSystemValid(starSystem))
        {
            int result;
            lock (_historyLock)
            {
                if (_history.ContainsKey(starSystem.Id))
                {
                    _history[starSystem.Id] = starSystem;
                    result = 2;
                }
                else
                {
                    _history[starSystem.Id] = starSystem;
                    result = 1;
                }
            }
            _ = _store.InsertOrUpdateStarSystemAsync(starSystem, raiseDatabaseStarSystemTableUpdatedEvent);
            return result;
        }
        else
        {
            log.Warn($"Star system '{starSystem.Name}' ({starSystem.Id}) with main star class '{starSystem.StarClass}' and total body count of {starSystem.TotalBodyCount} has invalid attributes for storing to history and was ignored");
        }
        return 0;
    }

    public int GetCount(bool tripOnly = false)
    {
        if (tripOnly)
        {
            return _history.Count(entry => entry.Value.IsTripHistory);
        }
        return _history.Count;
    }

    public async Task ClearAll()
    {
        _history.Clear();
        await _store.ClearAllStarSystems();
    }

    public async Task ResetTripData()
    {
        foreach (var tripHistoryEntry in _history.Where(entry => entry.Value.IsTripHistory).ToList())
        {
            tripHistoryEntry.Value.IsTripHistory = false;
        }
        await _store.ResetTripHistory();
    }

    public async Task<HistoryData> GetHistoryData()
    {
        Stopwatch watch = Stopwatch.StartNew();
        HistoryData historyData = new HistoryData
        {
            SystemCount = GetCount(),
            TripSystemCount = GetCount(tripOnly: true)
        };

        historyData.SystemFirstDiscoveryCount = await _store.GetSystemFirstDiscoveryCount();
        historyData.TripSystemFirstDiscoveryCount = await _store.GetSystemFirstDiscoveryCount(tripOnly: true);
        historyData.BodyCartographicBaseValueSum = await _store.GetBodyCartographicBaseValueSum();
        historyData.TripBodyCartographicBaseValueSum = await _store.GetBodyCartographicBaseValueSum(tripOnly: true);
        historyData.SystemStarClassStatistics = await _store.GetSystemStarClassStatistics();
        historyData.TripSystemStarClassStatistics = await _store.GetSystemStarClassStatistics(tripOnly: true);
        historyData.BodySignalSum = await _store.GetBodySignalSum();
        historyData.TripBodySignalSum = await _store.GetBodySignalSum(tripOnly: true);
        historyData.BodyCount = await _store.GetBodyCount();
        historyData.TripBodyCount = await _store.GetBodyCount(tripOnly: true);
        historyData.BodyFirstDiscoveryCount = await _store.GetBodyFirstDiscoveryCount();
        historyData.TripBodyFirstDiscoveryCount = await _store.GetBodyFirstDiscoveryCount(tripOnly: true);
        historyData.BodyTerraformableCount = await _store.GetBodyTerraformableCount();
        historyData.TripBodyTerraformableCount = await _store.GetBodyTerraformableCount(tripOnly: true);
        historyData.BodyValuableCount = await _store.GetBodyValuableBodyCount();
        historyData.TripBodyValuableCount = await _store.GetBodyValuableBodyCount(tripOnly: true);
        historyData.BodySurfaceScanCount = await _store.GetBodySurfaceScanCount();
        historyData.TripBodySurfaceScanCount = await _store.GetBodySurfaceScanCount(tripOnly: true);
        historyData.BodyCartographicSurfaceScanValueSum = await _store.GetBodyCartographicSurfaceScanValueSum();
        historyData.TripBodyCartographicSurfaceScanValueSum = await _store.GetBodyCartographicSurfaceScanValueSum(tripOnly: true);
        historyData.BodyTouchdownCount = await _store.GetBodyTouchdownCount();
        historyData.TripBodyTouchdownCount = await _store.GetBodyTouchdownCount(tripOnly: true);
        historyData.BodyRingBodyCount = await _store.GetBodyRingBodyCount();
        historyData.TripBodyRingBodyCount = await _store.GetBodyRingBodyCount(tripOnly: true);
        historyData.GenusSignalSum = await _store.GetGenusSignalSum();
        historyData.TripGenusSignalSum = await _store.GetGenusSignalSum(tripOnly: true);
        historyData.GenusCount = await _store.GetGenusCount();
        historyData.TripGenusCount = await _store.GetGenusCount(tripOnly: true);
        historyData.GenusAnalysisCompleteCount = await _store.GetGenusAnalysisCompleteCount();
        historyData.TripGenusAnalysisCompleteCount = await _store.GetGenusAnalysisCompleteCount(tripOnly: true);
        historyData.GenusVistaGenomicsValueSum = await _store.GetGenusVistaGenomicsValueSum();
        historyData.TripGenusVistaGenomicsValueSum = await _store.GetGenusVistaGenomicsValueSum(tripOnly: true);
        historyData.GenusSpeciesStatistics = await _store.GetGenusStatistics();
        historyData.TripGenusSpeciesStatistics = await _store.GetGenusStatistics(tripOnly: true);
        historyData.BodyCartographicValueSum = await _store.GetBodyCartographicValueSum();
        historyData.TripBodyCartographicValueSum = await _store.GetBodyCartographicValueSum(tripOnly: true);
        historyData.GenusVistaGenomicsBaseValueSum = await _store.GetGenusVistaGenomicsBaseValueSum();
        historyData.TripGenusVistaGenomicsBaseValueSum = await _store.GetGenusVistaGenomicsBaseValueSum(tripOnly: true);
        historyData.GenusFirstDiscoveryCount = await _store.GetGenusFirstDiscoveryCount();
        historyData.TripGenusFirstDiscoveryCount = await _store.GetGenusFirstDiscoveryCount(tripOnly: true);
        historyData.RingCount = await _store.GetRingCount();
        historyData.TripRingCount = await _store.GetRingCount(tripOnly: true);
        historyData.RingFirstDiscoveryCount = await _store.GetRingFirstDiscoveryCount();
        historyData.TripRingFirstDiscoveryCount = await _store.GetRingFirstDiscoveryCount(tripOnly: true);
        historyData.RingTypeStatistics = await _store.GetRingStatistics();
        historyData.TripRingTypeStatistics = await _store.GetRingStatistics(tripOnly: true);

        long elapsedMilliseconds = watch.ElapsedMilliseconds;
        log.Debug($"Read history data from database in {elapsedMilliseconds}ms");
        return historyData;
    }

    public async Task ResetIncompleteAnalysisForGenera(long currentStarSystemId, int currentBodyId)
    {
        List<Genus> incompleteGenera = new List<Genus>();
        foreach (StarSystem starSystem in _history.Values)
        {
            IReadOnlyDictionary<int, Body> bodies = starSystem.Bodies;
            if (bodies == null || bodies.Count <= 0)
            {
                continue;
            }
            foreach (Body body in starSystem.Bodies.Values)
            {
                if ((body.Id == currentBodyId && body.StarSystemId == currentStarSystemId) || body.Type != BodyType.Planet)
                {
                    continue;
                }
                IReadOnlyDictionary<string, Genus> genuses = ((Planet)body).Genuses;
                if (genuses == null || genuses.Count <= 0)
                {
                    continue;
                }
                foreach (Genus genus in ((Planet)body).Genuses.Values)
                {
                    if (!genus.AnalysisComplete && genus.ScanCount > 0)
                    {
                        incompleteGenera.Add(genus);
                    }
                }
            }
        }
        if (incompleteGenera.Count > 0)
        {
            foreach (Genus genus in incompleteGenera)
            {
                genus.ResetAnalysisData();
            }
        }
        int resetCount = await _store.ResetIncompleteAnalysisForGenera();
        if (resetCount > 0)
        {
            log.Info($"Reset {resetCount} incomplete {((resetCount > 1) ? "analyses" : "analysis")} for genera in database.");
        }
    }

    private bool isStarSystemValid(StarSystem starSystem)
    {
        if (string.IsNullOrEmpty(starSystem.Name) || starSystem.Id <= 0 || string.IsNullOrEmpty(starSystem.StarClass))
        {
            log.Info($"Star system has an invalid name ({starSystem.Name}), id ({starSystem.Id}) or star class {starSystem.StarClass}");
            return false;
        }
        if (starSystem.TotalBodyCount == 0)
        {
            log.Debug("Star system '" + starSystem.Name + "' has a total body count of 0 (scan missing?)");
            if (starSystem.NavBeaconScanBodyCount > 0)
            {
                starSystem.TotalBodyCount = starSystem.Bodies.Values.Where((Body body) => body.IsPlanetOrStar).Count();
                starSystem.TotalNonBodyCount = starSystem.NavBeaconScanBodyCount - starSystem.TotalBodyCount;
                log.Info($"Star system '{starSystem.Name}' ({starSystem.Id}) was scanned by nav beacon only (NumBodies = {starSystem.NavBeaconScanBodyCount}), calculated and set total body count to {starSystem.TotalBodyCount} and total non-body count to {starSystem.TotalNonBodyCount}");
            }
        }
        else if (starSystem.Bodies.Values.Where((Body body) => body.IsPlanetOrStar).Count() > starSystem.TotalBodyCount)
        {
            log.Info($"Star system '{starSystem.Name}' ({starSystem.Id}) has in invalid number of bodies ({starSystem.Bodies.Values.Where((Body body) => body.IsPlanetOrStar).Count()}) in comparision to its TotalBodyCount of {starSystem.TotalBodyCount}.");
            return false;
        }
        return true;
    }
}
