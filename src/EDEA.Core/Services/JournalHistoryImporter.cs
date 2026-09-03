using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using EDEA;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

/// <summary>Represents the JournalHistoryImporter class.</summary>
public class JournalHistoryImporter
{
    /// <summary>The instance field.</summary>
    private static JournalHistoryImporter? instance;

    /// <summary>The log field.</summary>
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(JournalHistoryImporter));

    /// <summary>The _historyProvider field.</summary>
    private readonly HistoryProvider _historyProvider;

    /// <summary>The _journalProvider field.</summary>
    private readonly JournalProvider _journalProvider;

    /// <summary>The _starSystemProvider field.</summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>The _journalPlanetMemory field.</summary>
    private readonly JournalPlanetMemory _journalPlanetMemory;

    /// <summary>The _journalSystemMemory field.</summary>
    private readonly JournalSystemMemory _journalSystemMemory;

    /// <summary>The _memorizedStarSystems field.</summary>
    private readonly ConcurrentDictionary<long, StarSystem> _memorizedStarSystems;

    /// <summary>The journalFiles field.</summary>
    private List<FileInfo>? journalFiles;

    /// <summary>The newStarSystemCount field.</summary>
    private int newStarSystemCount;

    /// <summary>The updatedStarSystemCount field.</summary>
    private int updatedStarSystemCount;

    /// <summary>The ignoredStarSystemCount field.</summary>
    private int ignoredStarSystemCount;

    /// <summary>The importCanceled field.</summary>
    private bool importCanceled;

    /// <summary>The importWorker field.</summary>
    private BackgroundWorker? importWorker;

    /// <summary>Gets or sets the StatusData.</summary>
    /// <value>A JournalImportReportData value.</value>
    public JournalImportReportData StatusData { get; private set; }

    /// <summary>Gets or sets the StatusPercentage.</summary>
    /// <value>A int value.</value>
    public int StatusPercentage { get; private set; }

    /// <summary>Occurs when the JournalHistoryImportProgressChanged event is raised.</summary>
    public event EventHandler JournalHistoryImportProgressChanged = delegate
    {
    };

    /// <summary>Initializes a new instance of the JournalHistoryImporter class.</summary>
    /// <param name="historyProvider">The HistoryProvider value of the historyProvider parameter.</param>
    /// <param name="journalProvider">The JournalProvider value of the journalProvider parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    private JournalHistoryImporter(HistoryProvider historyProvider, JournalProvider journalProvider, StarSystemProvider starSystemProvider)
    {
        _historyProvider = historyProvider;
        _journalProvider = journalProvider;
        _starSystemProvider = starSystemProvider;
        _journalPlanetMemory = new JournalPlanetMemory();
        _journalSystemMemory = new JournalSystemMemory();
        _memorizedStarSystems = new ConcurrentDictionary<long, StarSystem>();
        journalFiles = null;
        StatusData = new JournalImportReportData();
    }

    /// <summary>Performs the Instance operation.</summary>
    /// <param name="historyProvider">The HistoryProvider value of the historyProvider parameter.</param>
    /// <param name="journalProvider">The JournalProvider value of the journalProvider parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    /// <returns>A JournalHistoryImporter result.</returns>
    public static JournalHistoryImporter Instance(HistoryProvider historyProvider, JournalProvider journalProvider, StarSystemProvider starSystemProvider)
    {
        if (instance == null)
        {
            instance = new JournalHistoryImporter(historyProvider, journalProvider, starSystemProvider);
        }
        return instance;
    }

    /// <summary>Performs the StartJournalImport operation.</summary>
    public void StartJournalImport()
    {
        if (importWorker == null)
        {
            importWorker = new BackgroundWorker();
            importWorker.WorkerReportsProgress = true;
            importWorker.WorkerSupportsCancellation = true;
            importWorker.DoWork += importWorker_DoWork;
            importWorker.ProgressChanged += importWorker_ProgressChanged;
            importWorker.RunWorkerCompleted += importWorker_RunWorkerCompleted;
            log.Debug("Import worker initialised");
            importWorker.RunWorkerAsync();
        }
    }

    /// <summary>Retrieves JournalFiles.</summary>
    /// <returns>A int result.</returns>
    public int ReadJournalFiles()
    {
        journalFiles = (from f in Helpsters.GetJournalFiles(Preferences.Other.EdSavedGamePath)
                        orderby f.LastWriteTime
                        select f).ToList();
        if (journalFiles != null && journalFiles.Count > 1)
        {
            journalFiles.RemoveAt(journalFiles.Count - 1);
            return journalFiles.Count;
        }
        return 0;
    }

    /// <summary>Performs the importWorker_RunWorkerCompleted operation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void importWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        importWorker?.Dispose();
        importWorker = null;
        log.Debug("Import worker disposed");
        _journalPlanetMemory.Clear();
        _journalSystemMemory.Clear();
        _memorizedStarSystems.Clear();
        journalFiles = null;
        StatusData.ResetData();
        StatusPercentage = 0;
        newStarSystemCount = 0;
        updatedStarSystemCount = 0;
        ignoredStarSystemCount = 0;
        importCanceled = false;
    }

    /// <summary>Determines whether CancelJournalImport.</summary>
    public void CancelJournalImport()
    {
        importWorker?.CancelAsync();
    }

    /// <summary>Performs the importWorker_ProgressChanged operation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void importWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        StatusPercentage = e.ProgressPercentage;
        JournalHistoryImportProgressChanged(this, EventArgs.Empty);
    }

    /// <summary>Performs the importWorker_DoWork operation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void importWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        BackgroundWorker? backgroundWorker = sender as BackgroundWorker;
        if (journalFiles == null || journalFiles.Count < 1)
        {
            return;
        }
        log.Info("Starting to import historical journal files ...");
        Stopwatch stopwatch = Stopwatch.StartNew();
        int phaseCount = 4;
        int currentPhaseProgress = 0;
        log.Info("Import historical journal files phase 1, scanning for systems ...");
        for (int i = 0; i < journalFiles.Count; i++)
        {
            if (backgroundWorker?.CancellationPending == true)
            {
                log.Warn($"Canceled importing journal files in phase 1 after {stopwatch.ElapsedMilliseconds}ms, processed {i} journal files so far");
                importCanceled = true;
                break;
            }
            try
            {
                string[] lines = File.ReadLines(journalFiles[i].FullName).ToArray();
                log.Debug($"Processing journal file {journalFiles[i].Name} ({lines.Length} lines):");
                for (int j = 0; j < lines.Length; j++)
                {
                    JsonNode? jNode = JsonNode.Parse(lines[j]);
                    if (jNode is not JsonObject jObject)
                    {
                        continue;
                    }
                    try
                    {
                        string? eventName = Helpsters.ConvertJObjectValue<string>(jObject, "event");
                        if (eventName == "StartJump" && jObject.ContainsKey("StarSystem") && jObject.ContainsKey("StarClass"))
                        {
                            _journalProvider.processJournalStartJumpEvent(jObject, _journalSystemMemory);
                        }
                        else if (eventName == "FSDJump" || eventName == "Location" || eventName == "CarrierJump")
                        {
                            StarSystem? starSystem = _journalProvider.processJournalFSDJumpEvent(jObject, _journalSystemMemory);
                            if (starSystem != null)
                            {
                                starSystem.IsTripHistory = false;
                                if (starSystem.Id != 0L && !_memorizedStarSystems.ContainsKey(starSystem.Id))
                                {
                                    if (_memorizedStarSystems.TryAdd(starSystem.Id, starSystem))
                                    {
                                        log.Debug($"Memorized star system {starSystem.Name} ({starSystem.Id})");
                                    }
                                    else
                                    {
                                        log.Warn($"Could not memorize star system {starSystem.Name} ({starSystem.Id})");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Error parsing journal while importing journal files in phase 1, JSON object: {jObject}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn("Journal file " + journalFiles[i].FullName + " is locked or missing and will be ignored!", ex);
            }
            currentPhaseProgress = (i + 1) * 100 / journalFiles.Count;
            StatusData.SystemsScanProgress = currentPhaseProgress;
            StatusData.JournalFilesProcessed = i + 1;
            backgroundWorker?.ReportProgress(currentPhaseProgress / phaseCount);
        }
        log.Info($"Import historical journal files phase 1 completed, memorized {_memorizedStarSystems.Count} star systems");
        log.Info("Import historical journal files phase 2, scanning for bodies ...");
        for (int k = 0; k < journalFiles.Count; k++)
        {
            if (backgroundWorker?.CancellationPending == true)
            {
                log.Warn($"Canceled importing journal files in phase 2 after {stopwatch.ElapsedMilliseconds}ms, processed {k} journal files so far");
                importCanceled = true;
                break;
            }
            try
            {
                string[] lines = File.ReadLines(journalFiles[k].FullName).ToArray();
                log.Debug($"Processing journal file {journalFiles[k].Name} ({lines.Length} lines):");
                for (int j = 0; j < lines.Length; j++)
                {
                    JsonNode? jNode = JsonNode.Parse(lines[j]);
                    if (jNode is not JsonObject jObject)
                    {
                        continue;
                    }
                    try
                    {
                        string? eventName = Helpsters.ConvertJObjectValue<string>(jObject, "event");
                        if (eventName == "Scan" && jObject.ContainsKey("BodyID") && jObject.ContainsKey("BodyName"))
                        {
                            long jSystemAddress = Helpsters.ConvertJObjectValue<long>(jObject, "SystemAddress", 0L);
                            if (_memorizedStarSystems.TryGetValue(jSystemAddress, out var memorizedScanStarSystem))
                            {
                                _journalProvider.processJournalScanEvent(jObject, _journalPlanetMemory, memorizedScanStarSystem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Error parsing journal while importing journal files in phase 2, JSON object: {jObject}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn("Journal file " + journalFiles[k].FullName + " is locked or missing and will be ignored!", ex);
            }
            currentPhaseProgress = (k + 1) * 100 / journalFiles.Count;
            StatusData.BodiesScanProgress = currentPhaseProgress;
            backgroundWorker?.ReportProgress(100 / phaseCount + currentPhaseProgress / phaseCount);
        }
        int totalBodyCount = 0;
        foreach (StarSystem memorizedStarSystem in _memorizedStarSystems.Values)
        {
            int bodiesInSystem = memorizedStarSystem.Bodies.Count(body => body.Value.Type != BodyType.Unknown);
            totalBodyCount += bodiesInSystem;
            log.Debug($"Memorized {bodiesInSystem} in star system {memorizedStarSystem.Name}");
        }
        log.Info($"Import historical journal files phase 2 completed, memorized {totalBodyCount} bodies");
        log.Info("Import historical journal files phase 3, scanning for additional data ...");
        for (int phase3FileIndex = 0; phase3FileIndex < journalFiles.Count; phase3FileIndex++)
        {
            if (backgroundWorker?.CancellationPending == true)
            {
                log.Warn($"Canceled importing journal files in phase 3 after {stopwatch.ElapsedMilliseconds}ms, processed {phase3FileIndex} journal files so far");
                importCanceled = true;
                break;
            }
            try
            {
                string[] lines = File.ReadLines(journalFiles[phase3FileIndex].FullName).ToArray();
                log.Debug($"Processing journal file {journalFiles[phase3FileIndex].Name} ({lines.Length} lines):");
                for (int j = 0; j < lines.Length; j++)
                {
                    JsonNode? jNode = JsonNode.Parse(lines[j]);
                    if (jNode is not JsonObject jObject)
                    {
                        continue;
                    }
                    try
                    {
                        string? eventName = Helpsters.ConvertJObjectValue<string>(jObject, "event");
                        long jSystemAddress = Helpsters.ConvertJObjectValue<long>(jObject, "SystemAddress", 0L);
                        if (_memorizedStarSystems.TryGetValue(jSystemAddress, out var memorizedEventStarSystem))
                        {
                            switch (eventName)
                            {
                                case "FSSDiscoveryScan":
                                    _journalProvider.processJournalFSSDiscoveryScanEvent(jObject, memorizedEventStarSystem);
                                    break;
                                case "NavBeaconScan":
                                    _journalProvider.processJournalNavBeaconScanEvent(jObject, memorizedEventStarSystem);
                                    break;
                                case "FSSAllBodiesFound":
                                    _journalProvider.processJournalFSSAllBodiesFoundEvent(jObject, memorizedEventStarSystem);
                                    break;
                                case "SAAScanComplete":
                                    _journalProvider.processJournalSAAScanCompleteEvent(jObject, memorizedEventStarSystem, _journalPlanetMemory);
                                    break;
                                case "FSSBodySignals":
                                case "SAASignalsFound":
                                    _journalProvider.processJournalFSSBodySignalsEvent(jObject, _journalPlanetMemory, memorizedEventStarSystem);
                                    break;
                                case "Touchdown" when jObject.ContainsKey("SystemAddress"):
                                    _journalProvider.processJournalTouchdownEvent(jObject, memorizedEventStarSystem);
                                    break;
                                case "ScanOrganic":
                                    _journalProvider.processJournalScanOrganicEvent(jObject, memorizedEventStarSystem);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Error parsing journal while importing journal files in phase 3, JSON object: {jObject}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn("Journal file " + journalFiles[phase3FileIndex].FullName + " is locked or missing and will be ignored!", ex);
            }
            currentPhaseProgress = (phase3FileIndex + 1) * 100 / journalFiles.Count;
            StatusData.AdditionalDataScanProgress = currentPhaseProgress;
            backgroundWorker?.ReportProgress(200 / phaseCount + currentPhaseProgress / phaseCount);
        }
        log.Info("Import historical journal files phase 3 completed");
        log.Info("Import historical journal files phase 4, updating history ...");
        bool shouldReport = false;
        for (int historySystemIndex = 0; historySystemIndex < _memorizedStarSystems.Count; historySystemIndex++)
        {
            if (backgroundWorker?.CancellationPending == true)
            {
                log.Warn($"Canceled importing journal files in phase 4 after {stopwatch.ElapsedMilliseconds}ms");
                importCanceled = true;
                break;
            }
            shouldReport = historySystemIndex == _memorizedStarSystems.Count - 1 || historySystemIndex % 10 == 0;
            StarSystem memorizedStarSystem = _memorizedStarSystems.ElementAt(historySystemIndex).Value;
            if (string.IsNullOrEmpty(memorizedStarSystem.Name) || memorizedStarSystem.Id == 0L)
            {
                continue;
            }
            if (memorizedStarSystem.Id == _starSystemProvider.CurrentSystem.Id)
            {
                foreach (Body memorizedBody in memorizedStarSystem.Bodies.Values)
                {
                    _starSystemProvider.CurrentSystem.TryAddOrUpdateBody(memorizedBody, ignoreSpeechOutput: true, DataSource.Journal, out var _);
                    _starSystemProvider.triggerGuiDataUpdateEvent();
                }
            }
            switch (_historyProvider.AddOrUpdateStarSystem(memorizedStarSystem, shouldReport))
            {
                case 1:
                    newStarSystemCount++;
                    log.Debug($"Added star system {memorizedStarSystem.Name} ({memorizedStarSystem.Id}) to the history");
                    break;
                case 2:
                    updatedStarSystemCount++;
                    log.Debug($"Updated star system {memorizedStarSystem.Name} ({memorizedStarSystem.Id}) in the history");
                    break;
                case 0:
                    ignoredStarSystemCount++;
                    log.Warn($"Could not add or update star system {memorizedStarSystem.Name} ({memorizedStarSystem.Id}) to/in the history");
                    break;
            }
            currentPhaseProgress = (historySystemIndex + 1) * 100 / _memorizedStarSystems.Count;
            StatusData.AddedToHistoryCount = newStarSystemCount;
            StatusData.UpdatedInHistoryCount = updatedStarSystemCount;
            StatusData.IgnoredSystemsCount = ignoredStarSystemCount;
            backgroundWorker?.ReportProgress(300 / phaseCount + currentPhaseProgress / phaseCount);
        }
        if (!importCanceled)
        {
            log.Info($"Imported journal files to history in {stopwatch.ElapsedMilliseconds}ms, processed {journalFiles.Count} journal files, added {newStarSystemCount} star systems and updated {updatedStarSystemCount} star systems");
        }
        if (!importCanceled && _starSystemProvider.CurrentSystem.Id != 0L)
        {
            _starSystemProvider.HandleCurrentSystemChange(_starSystemProvider.CurrentSystem).GetAwaiter().GetResult();
        }
        stopwatch.Stop();
    }
}
