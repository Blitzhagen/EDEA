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

/// <summary>Represents the HistoryProvider class.</summary>
public class HistoryProvider
{
    /// <summary>The _instance field.</summary>
    private static HistoryProvider? _instance;

    /// <summary>The _store field.</summary>
    private readonly SQLiteStore _store;
    /// <summary>The _history field.</summary>
    private readonly ConcurrentDictionary<long, StarSystem> _history;
    /// <summary>The _historyLock field.</summary>
    private readonly object _historyLock = new object();

    /// <summary>The log field.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(HistoryProvider));

    /// <summary>Occurs when the HistoryUpdated event is raised.</summary>
    public event EventHandler? HistoryUpdated;

    /// <summary>Initializes a new instance of the HistoryProvider class.</summary>
    /// <param name="store">The SQLiteStore value of the store parameter.</param>
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

    /// <summary>Performs the _store_DatabaseStarSystemTableUpdated operation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void _store_DatabaseStarSystemTableUpdated(object? sender, EventArgs e)
    {
        HistoryUpdated?.Invoke(this, e);
    }

    /// <summary>Performs the Instance operation.</summary>
    /// <param name="store">The SQLiteStore value of the store parameter.</param>
    /// <returns>A HistoryProvider result.</returns>
    public static HistoryProvider Instance(SQLiteStore store)
    {
        _instance ??= new HistoryProvider(store);
        return _instance;
    }

    /// <summary>Determines whether IsStarSystemExisting.</summary>
    /// <param name="starSystemId">The long value of the starSystemId parameter.</param>
    /// <returns>A bool result.</returns>
    public bool IsStarSystemExisting(long starSystemId)
    {
        if (_history.ContainsKey(starSystemId))
        {
            return true;
        }
        return false;
    }

    /// <summary>Performs the TryGetStarSystem operation.</summary>
    /// <param name="starSystemId">The long value of the starSystemId parameter.</param>
    /// <param name="starSystem">The StarSystem? value of the starSystem parameter.</param>
    /// <returns>A bool result.</returns>
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

    /// <summary>Retrieves AllStarSystems.</summary>
    /// <returns>A IEnumerable<StarSystem> result.</returns>
    public IEnumerable<StarSystem> GetAllStarSystems()
    {
        return _history.Values;
    }

    /// <summary>Performs the AddOrUpdateStarSystem operation.</summary>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    /// <param name="raiseDatabaseStarSystemTableUpdatedEvent">The bool value of the raiseDatabaseStarSystemTableUpdatedEvent parameter.</param>
    /// <returns>A int result.</returns>
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

    /// <summary>Retrieves Count.</summary>
    /// <param name="tripOnly">The bool value of the tripOnly parameter.</param>
    /// <returns>A int result.</returns>
    public int GetCount(bool tripOnly = false)
    {
        if (tripOnly)
        {
            return _history.Count(entry => entry.Value.IsTripHistory);
        }
        return _history.Count;
    }

    /// <summary>Retrieves the journal files that have already been imported.</summary>
    /// <returns>The imported journal files.</returns>
    public IEnumerable<ImportedJournalFile> GetImportedJournalFiles()
    {
        return _store.GetImportedJournalFiles();
    }

    /// <summary>Records that journal files have been imported.</summary>
    /// <param name="files">The imported journal files.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RecordImportedJournalFilesAsync(IEnumerable<ImportedJournalFile> files)
    {
        await _store.RecordImportedJournalFilesAsync(files);
    }

    /// <summary>Removes All.</summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task ClearAll()
    {
        _history.Clear();
        await _store.ClearAllStarSystems();
    }

    /// <summary>Performs the ResetTripData operation.</summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task ResetTripData()
    {
        foreach (var tripHistoryEntry in _history.Where(entry => entry.Value.IsTripHistory).ToList())
        {
            tripHistoryEntry.Value.IsTripHistory = false;
        }
        await _store.ResetTripHistory();
    }

    /// <summary>Retrieves HistoryData.</summary>
    /// <returns>A Task<HistoryData> representing the asynchronous operation.</returns>
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

    /// <summary>Performs the ResetIncompleteAnalysisForGenera operation.</summary>
    /// <param name="currentStarSystemId">The long value of the currentStarSystemId parameter.</param>
    /// <param name="currentBodyId">The int value of the currentBodyId parameter.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
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

    /// <summary>Performs the isStarSystemValid operation.</summary>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    /// <returns>A bool result.</returns>
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
