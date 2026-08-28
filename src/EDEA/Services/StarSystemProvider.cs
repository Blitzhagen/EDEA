#nullable disable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using EDEA;
using EDEA.Models;
using EDEA.Stores;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Services;

public enum Activity
{
    None,
    ExploreSystem,
    GalaxyMap,
    Jump,
    ExplorePlanet,
    Other
}

public delegate void RouteLoadingStatusChangedEventHandler(object sender, string statusText);
public delegate void SurroundingsLoadingStatusChangedEventHandler(object sender, string statusText);

public class StarSystemProvider
{
    private static StarSystemProvider instance;

    private static readonly ILog log = LogManager.GetLogger(typeof(StarSystemProvider));

    private readonly HistoryProvider _historyProvider;

    private WebApiProvider _WebApiProvider;
    private RouteProvider _routeProvider;
    private JournalProvider _journalProvider;
    private PlanetsOfInterestProvider _planetsOfInterestProvider;

    private readonly int _guiDataUpdateTriggerThreshold = 700;

    private bool guiDataUpdateTriggerBlocked;
    private bool guiDataUpdateTriggerPending;
    private long surroundingsSystemId;
    private bool surroundingsVisible;

    public ConcurrentDictionary<long, StarSystem> StarSystemsOnRoute { get; }
    public ConcurrentDictionary<long, StarSystem> SurroundingStarSystems { get; private set; }

    public StarSystem CurrentSystem { get; private set; }
    public StarSystem DestinationSystem { get; private set; }
    public Activity CurrentActivity { get; private set; }
    public Planet CurrentPlanet { get; private set; }
    public LocationOnPlanet LocationOnCurrentPlanet { get; private set; }
    public bool IsInTeam { get; private set; }
    public string CommanderName { get; set; }
    public List<string> TeammateNames { get; set; }

    public bool IsCurrentSystemInRoute => isSystemInRoute(CurrentSystem, StarSystemsOnRoute);
    public bool RouteIsLoading { get; private set; }
    public bool SurroundingsAreLoading { get; private set; }
    public Ship CurrentShip { get; set; }

    public event EventHandler GuiDataUpdated = delegate { };
    public event EventHandler GuiLocationDataUpdated = delegate { };
    public event EventHandler GuiShipFuelDataUpdated = delegate { };
    public event EventHandler CurrentSystemInRouteChanged = delegate { };
    public event EventHandler CurrentPlanetChanged = delegate { };
    public event RouteLoadingStatusChangedEventHandler RouteLoadingStatusChanged = delegate { };
    public event SurroundingsLoadingStatusChangedEventHandler SurroundingsLoadingStatusChanged = delegate { };

    public static StarSystemProvider Instance(HistoryProvider historyProvider)
    {
        if (instance == null)
        {
            instance = new StarSystemProvider(historyProvider);
        }
        return instance;
    }

    private StarSystemProvider(HistoryProvider historyProvider)
    {
        StarSystemsOnRoute = new ConcurrentDictionary<long, StarSystem>();
        SurroundingStarSystems = new ConcurrentDictionary<long, StarSystem>();
        CurrentSystem = new StarSystem(0L, string.Empty);
        CurrentPlanet = null;
        CommanderName = string.Empty;
        TeammateNames = new List<string>();
        RouteIsLoading = true;
        SurroundingsAreLoading = false;
        _historyProvider = historyProvider;
    }

    public void RegisterProvider(object provider)
    {
        if (provider.GetType() == typeof(PlanetsOfInterestProvider))
        {
            _planetsOfInterestProvider = (PlanetsOfInterestProvider)provider;
        }
        else if (provider.GetType() == typeof(StatusProvider))
        {
            ((StatusProvider)provider).StatusUpdated += _statusProvider_StatusUpdated;
            ((StatusProvider)provider).LocationUpdated += _statusProvider_LocationUpdated;
            ((StatusProvider)provider).ShipFuelUpdated += _starSystemProvider_ShipFuelUpdated;
        }
        else if (provider.GetType() == typeof(RouteProvider))
        {
            _routeProvider = (RouteProvider)provider;
            _routeProvider.SystemsOnRouteChanged += _navRouteProvider_SystemsOnRouteChanged;
        }
        else if (provider.GetType() == typeof(JournalProvider))
        {
            _journalProvider = (JournalProvider)provider;
            _journalProvider.ParsedJournalDataUpdated += _journalProvider_ParsedJournalDataUpdated;
        }
        else if (provider.GetType() == typeof(WebApiProvider))
        {
            _WebApiProvider = (WebApiProvider)provider;
        }
    }

    public void HandleLoadEdsmSystemDataCommand(bool forceUpdate)
    {
        _WebApiProvider.EdsmCheckAndRequestStarSystemInformation(new WebApiParameterEdsmStarystem(CurrentSystem), onRequestedStarSystemInformation, forceUpdate, ignoreSpeechOutput: true);
    }

    public void HandleApplicationShutdown()
    {
        SpeechProvider.ShutUp();
        if (!string.IsNullOrEmpty(CommanderName))
        {
            SpeechProvider.SpeakGoodbye(new SpeechOutputCommander(CommanderName.Remove(0, "CMDR ".Length)));
        }
        int addOrUpdateResult = _historyProvider.AddOrUpdateStarSystem(CurrentSystem);
        if (addOrUpdateResult > 0)
        {
            log.Info($"Application shutdown detected, {((addOrUpdateResult == 1) ? "added" : "updated")} current system '{CurrentSystem.Name}' ({CurrentSystem.Id}) {((addOrUpdateResult == 1) ? "to" : "in")} history, history count: {_historyProvider.GetCount()}");
            log.Info($"Application session had {_WebApiProvider.AbsoluteRequestsInSession} EDSM requests");
        }
        else
        {
            log.Warn($"Application shutdown detected, but current system '{CurrentSystem.Name}' ({CurrentSystem.Id}) could not be added to history, history count: {_historyProvider.GetCount()}");
        }
    }

    public async Task HandleCurrentSystemChange(StarSystem potentialNewSystem)
    {
        StarSystem currentSystem = CurrentSystem;
        log.Info($"Current system changed to '{potentialNewSystem.Name}', previous system was '{currentSystem.Name}' ");
        if (currentSystem.Id != 0L && currentSystem.Id != potentialNewSystem.Id)
        {
            _ = _historyProvider.AddOrUpdateStarSystem(currentSystem);
        }
        if (!_journalProvider.JournalFirstParse && currentSystem.Id != 0L && currentSystem.Id != potentialNewSystem.Id)
        {
            log.Info($"Requesting EDSM update for previous system {currentSystem.Name} ({currentSystem.Id})");
            _WebApiProvider.EdsmCheckAndRequestStarSystemInformation(new WebApiParameterEdsmStarystem(currentSystem), addToOrUpdateInHistory, forceUpdate: true, ignoreSpeechOutput: true);
        }
        if (_historyProvider.TryGetStarSystem(potentialNewSystem.Id, out var starSystem))
        {
            starSystem.WasReadFromJournal |= potentialNewSystem.WasReadFromJournal;
            CurrentSystem = starSystem;
            log.Debug($"Restored current system '{CurrentSystem.Name}' ({CurrentSystem.Id}) from the history");
            if (!_journalProvider.JournalFirstParse)
            {
                PredictOccurrenceOfSpeciesForCurrentSystem();
                FindMatchingClassificationsForSystem(CurrentSystem, ignoreSpeechOutput: false);
                calculateCartographicValues(CurrentSystem);
            }
        }
        else
        {
            CurrentSystem = potentialNewSystem;
            log.Debug($"Current system '{CurrentSystem.Name}' ({CurrentSystem.Id}) is a new system with primary star '{CurrentSystem.PrimaryStarName}'");
        }

        int historyResult = _historyProvider.AddOrUpdateStarSystem(CurrentSystem);
        if (historyResult > 0)
        {
            log.Info($"{(historyResult == 1 ? "Added" : "Updated")} current system '{CurrentSystem.Name}' ({CurrentSystem.Id}) to history, history count: {_historyProvider.GetCount()}");
        }
        if (IsCurrentSystemInRoute)
        {
            try
            {
                await replaceStarSystemInRoute(CurrentSystem, isCurrentSystem: true);
            }
            catch (Exception exception)
            {
                log.Error($"Unable to replace current system '{CurrentSystem.Name}' in route", exception);
            }
            if (_routeProvider.IsCustomRoute)
            {
                StarSystem nextJumpSystem = (from system in StarSystemsOnRoute
                                             where system.Value.JumpDistance == CurrentSystem.JumpDistance + 1
                                             select system.Value).FirstOrDefault();
                if (nextJumpSystem != null)
                {
                    copySystemNameToClipboard(nextJumpSystem.Name);
                }
            }
        }
        if (isSystemInRoute(currentSystem, StarSystemsOnRoute))
        {
            try
            {
                await replaceStarSystemInRoute(currentSystem, isCurrentSystem: false);
            }
            catch (Exception exception)
            {
                log.Error($"Unable to replace previous system '{currentSystem.Name}' in route", exception);
            }
        }
        RequestSurroundingStarSystemsForCurrentSystem();
    }

    public void MainViewModel_SelectedTabIndexChanged(object sender, EventArgs e)
    {
        surroundingsVisible = false;
        try
        {
            if (((MainViewModel)sender).IsSurroundingsTabSelected)
            {
                surroundingsVisible = true;
                RequestSurroundingStarSystemsForCurrentSystem();
            }
        }
        catch (Exception exception)
        {
            log.Error("Could not get selection status of Surroundings Tab - systems will not be loaded!", exception);
        }
    }

    public void RequestSurroundingStarSystemsForCurrentSystem()
    {
        if (surroundingsVisible && CurrentSystem != null && CurrentSystem.Id > 0 && !_journalProvider.JournalFirstParse)
        {
            if (CurrentSystem.Id == surroundingsSystemId && SurroundingStarSystems.Count > 0)
            {
                log.Debug($"Already requested surrounding systems for current star system '{CurrentSystem.Name}' ({CurrentSystem.Id}) from EDSM, ignoring request");
            }
            else
            {
                SetSurroundingsAreLoadingStatus(status: true, "Requesting surrounding systems, please wait ...");
                _WebApiProvider.EdsmRequestSurroundingStarSystemsInformation(CurrentSystem, onRequestedSurroundingStarSystemsForCurrentSystem);
            }
        }
    }

    private async void onRequestedSurroundingStarSystemsForCurrentSystem(WebApiParameter webEdsmRequestParameter)
    {
        try
        {

            WebApiParameterEdsmSurroundings webEdsmRequestParameterSurroundings = webEdsmRequestParameter as WebApiParameterEdsmSurroundings;
            try
            {
                await Task.Run(delegate
                {
                    ConcurrentDictionary<long, StarSystem> concurrentDictionary = new ConcurrentDictionary<long, StarSystem>();
                    foreach (StarSystem surroundingSystem in webEdsmRequestParameterSurroundings.StarSystems)
                    {
                        long id = surroundingSystem.Id;
                        if (_historyProvider.TryGetStarSystem(id, out var historySystem) && concurrentDictionary.TryAdd(id, historySystem))
                        {
                            concurrentDictionary[id].JumpDistanceLy = surroundingSystem.JumpDistanceLy;
                            concurrentDictionary[id].Population = surroundingSystem.Population;
                            log.Debug($"Restored surrounding star system '{concurrentDictionary[id].Name}' ({id}) from the history");
                            calculateCartographicValues(concurrentDictionary[id]);
                            FindMatchingClassificationsForSystem(concurrentDictionary[id], ignoreSpeechOutput: true);
                        }
                        else if (SurroundingStarSystems.ContainsKey(id) && concurrentDictionary.TryAdd(id, SurroundingStarSystems[id]))
                        {
                            concurrentDictionary[id].JumpDistanceLy = surroundingSystem.JumpDistanceLy;
                            log.Debug($"Restored surrounding star system '{concurrentDictionary[id].Name}' ({id}) from previous surrounding systems");
                        }
                        else
                        {
                            concurrentDictionary.TryAdd(id, surroundingSystem);
                            log.Debug($"Surrounding star system '{concurrentDictionary[id].Name}' ({id}) is a new system");
                        }
                        _WebApiProvider.EdsmCheckAndRequestStarSystemInformation(new WebApiParameterEdsmStarystem(concurrentDictionary[id]), onRequestedStarSystemInformation, forceUpdate: false, ignoreSpeechOutput: true);
                    }
                    SurroundingStarSystems = concurrentDictionary;
                    surroundingsSystemId = CurrentSystem.Id;
                });
            }
            catch (Exception exception)
            {
                SurroundingStarSystems.Clear();
                surroundingsSystemId = 0L;
                log.Error($"Cannot set surrounding star systems for {CurrentSystem?.Name} ({CurrentSystem?.Id})", exception);
            }
            SetSurroundingsAreLoadingStatus(status: false, string.Empty);
            triggerGuiDataUpdateEvent();

        }
        catch (Exception exception)
        {
            log.Error("Error in onRequestedSurroundingStarSystemsForCurrentSystem", exception);
        }
    }

    public async Task SetPastStarSystemsOnRouteAndRequestEDSMDataForUpcomingStarSystemsOnRoute()
    {
        if (!IsCurrentSystemInRoute)
        {
            return;
        }
        int jd = CurrentSystem.JumpDistance;
        await Task.Run(delegate
        {
            foreach (StarSystem pastSystem in (from sysItem in StarSystemsOnRoute
                                               where sysItem.Value.JumpDistance < jd
                                               select sysItem.Value into sysItem
                                               orderby sysItem.JumpDistance
                                               select sysItem).ToList())
            {
                pastSystem.IsPastSystemInRoute = true;
            }
            List<StarSystem> upcomingSystems = (from sysItem in StarSystemsOnRoute
                                                where sysItem.Value.JumpDistance > jd && sysItem.Value.NeedsEdsmSystemUpdate
                                                select sysItem.Value into sysItem
                                                orderby sysItem.JumpDistance
                                                select sysItem).ToList();
            if (upcomingSystems.Count > 25)
            {
                upcomingSystems = upcomingSystems.GetRange(0, 25);
            }
            foreach (StarSystem upcomingSystem in upcomingSystems)
            {
                _WebApiProvider.EdsmCheckAndRequestStarSystemInformation(new WebApiParameterEdsmStarystem(upcomingSystem), onRequestedStarSystemInformation, forceUpdate: false, ignoreSpeechOutput: true);
            }
            triggerGuiDataUpdateEvent();
        });
    }

    public void CopyNextSystemNametoClipboard()
    {
        if (IsCurrentSystemInRoute && _routeProvider.IsCustomRoute)
        {
            StarSystem starSystem = (from system in StarSystemsOnRoute
                                     where system.Value.JumpDistance == CurrentSystem.JumpDistance + 1
                                     select system.Value).FirstOrDefault();
            if (starSystem != null)
            {
                copySystemNameToClipboard(starSystem.Name);
            }
        }
    }

    public void PredictOccurrenceOfSpeciesForCurrentSystem()
    {
        GeneraIndexProvider.PredictOccurrenceOfSpecies(CurrentSystem);
        foreach (Body body in CurrentSystem.Bodies.Values)
        {
            if (body.Type == BodyType.Planet)
            {
                announcePredictedOccurrenceOfSpecies((Planet)body);
                ((Planet)body).InitialPredictionOfSpecies = false;
            }
        }
        triggerGuiDataUpdateEvent();
    }

    public void PredictOccurrenceOfSpeciesForPlanet(Planet planet)
    {
        GeneraIndexProvider.PredictOccurrenceOfSpecies(planet);
        announcePredictedOccurrenceOfSpecies(planet);
        planet.InitialPredictionOfSpecies = false;
        triggerGuiDataUpdateEvent();
    }

    public async void FindMatchingClassificationsForCurrentRouteAndSurroundings()
    {
        try
        {

            foreach (StarSystem routeSystem in StarSystemsOnRoute.Values)
            {
                await _planetsOfInterestProvider.FindMatchingPlanetClassifications(routeSystem);
            }
            foreach (StarSystem surroundingSystem in SurroundingStarSystems.Values)
            {
                await _planetsOfInterestProvider.FindMatchingPlanetClassifications(surroundingSystem);
            }
            triggerGuiDataUpdateEvent();

        }
        catch (Exception exception)
        {
            log.Error("Error in FindMatchingClassificationsForCurrentRouteAndSurroundings", exception);
        }
    }

    public async void FindMatchingClassificationsForSystem(StarSystem starSystem, bool ignoreSpeechOutput)
    {
        try
        {

            await _planetsOfInterestProvider.FindMatchingPlanetClassifications(starSystem);
            triggerGuiDataUpdateEvent();
            if (ignoreSpeechOutput)
            {
                return;
            }
            foreach (Body body in CurrentSystem.Bodies.Values)
            {
                if (body.Type == BodyType.Planet)
                {
                    announceFoundMatchingClassifications((Planet)body);
                }
            }

        }
        catch (Exception exception)
        {
            log.Error("Error in FindMatchingClassificationsForSystem", exception);
        }
    }

    public async void FindMatchingClassificationsForPlanet(Planet planet)
    {
        try
        {

            await _planetsOfInterestProvider.FindAndSetMatchingPlanetClassifications(planet);
            announceFoundMatchingClassifications(planet);
            triggerGuiDataUpdateEvent();

        }
        catch (Exception exception)
        {
            log.Error("Error in FindMatchingClassificationsForPlanet", exception);
        }
    }

    public void InitializeCurrentSystem()
    {
        if (IsCurrentSystemInRoute)
        {
            CurrentSystemInRouteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ResetIncompleteGenera()
    {
        List<Genus> incompleteGenera = CurrentPlanet?.Genuses?.Where((KeyValuePair<string, Genus> genus) => !genus.Value.AnalysisComplete && genus.Value.ScanCount > 0).Select((KeyValuePair<string, Genus> genus) => genus.Value)?.ToList();
        if (incompleteGenera != null && incompleteGenera.Count > 0)
        {
            foreach (Genus genus in incompleteGenera)
            {
                genus.ResetAnalysisData();
            }
        }
        if (!_journalProvider.JournalFirstParse && CurrentPlanet != null)
        {
            Task _ = _historyProvider.ResetIncompleteAnalysisForGenera(CurrentPlanet.StarSystemId, CurrentPlanet.Id);
        }
    }

    private async void calculateCartographicValues(StarSystem starSystem)
    {
        try
        {

            await Task.Run(delegate
            {
                log.Debug($"Calculating Cartographic Values for Starsystem {starSystem.Name} ({starSystem.Id})");
                foreach (Body body in starSystem.Bodies.Values)
                {
                    if (body.CartographicBaseValue == 0)
                    {
                        body.CalculateCartographicValue(skipSpeechOutput: true);
                    }
                }
            });

        }
        catch (Exception exception)
        {
            log.Error("Error in calculateCartographicValues", exception);
        }
    }

    private class GenusClassificationViewModel
    {
        public GenusClassification GenusClassification { get; }
        public Planet Planet { get; }
        public int VistaGenomicsMaxValueSort => GenusClassification.VistaGenomicsBaseValue * 5;

        public GenusClassificationViewModel(GenusClassification genusClassification, Planet planet)
        {
            GenusClassification = genusClassification;
            Planet = planet;
        }
    }

    private void announcePredictedOccurrenceOfSpecies(Planet planet)
    {
        if (_journalProvider.JournalFirstParse || !planet.InitialPredictionOfSpecies)
        {
            return;
        }
        SpeechOutputPlanet speechOutputPlanet = new SpeechOutputPlanet(planet);
        foreach (GenusClassificationViewModel viewModel in planet.PredictedSpecies.Select((GenusClassification genusClassification) => new GenusClassificationViewModel(genusClassification, planet)).ToList())
        {
            if (viewModel.VistaGenomicsMaxValueSort > Preferences.Other.ValuableGenusThreshold)
            {
                SpeechProvider.SpeakValuableGenusPredicted(speechOutputPlanet, new SpeechOutputSpecies(viewModel.GenusClassification));
            }
        }
        int valuablePredictedCount = (from g in planet.PredictedSpecies
                                      select new GenusClassificationViewModel(g, planet) into x
                                      where x.VistaGenomicsMaxValueSort > Preferences.Other.ValuableGenusThreshold
                                      select x).Count();
        if (valuablePredictedCount > 0)
        {
            SpeechProvider.SpeakValuableGeneraPredicted(speechOutputPlanet, new SpeechOutputValuableSpeciesCount(valuablePredictedCount));
        }
    }

    private void announceFoundMatchingClassifications(Planet planet)
    {
        if (planet.MatchingPlanetClassificationsAnnounced)
        {
            return;
        }
        SpeechOutputPlanet speechOutputPlanet = new SpeechOutputPlanet(planet);
        foreach (PlanetClassification matchingPlanetClassification in planet.MatchingPlanetClassifications)
        {
            SpeechProvider.SpeakMatchingClassificationFound(speechOutputPlanet, new SpeechOutputPlanetClassification(matchingPlanetClassification));
        }
        if (planet.MatchingPlanetClassifications.Count > 0)
        {
            SpeechProvider.SpeakMatchingClassificationsFound(speechOutputPlanet, new SpeechOutputMatchingClassificationsCount(planet.MatchingPlanetClassifications.Count));
        }
        planet.MatchingPlanetClassificationsAnnounced = true;
    }

    private void copySystemNameToClipboard(string systemName)
    {
        if (_journalProvider.JournalFirstParse || !_routeProvider.IsCustomRoute)
        {
            return;
        }
        Thread thread = new Thread((ThreadStart)delegate
        {
            try
            {
                Clipboard.SetText(systemName, TextDataFormat.UnicodeText);
            }
            catch (Exception exception)
            {
                log.Error("Unable to copy next system name '" + systemName + "' to clipboard", exception);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        log.Debug($"Copied next system name '{systemName}' to clipboard, mode: {_journalProvider.JournalFirstParse}");
    }

    private void addToOrUpdateInHistory(WebApiParameter webEdsmRequestParameter)
    {
        WebApiParameterEdsmStarystem webApiParameterEdsmStarystem = (WebApiParameterEdsmStarystem)webEdsmRequestParameter;
        int addOrUpdateResult = _historyProvider.AddOrUpdateStarSystem(webApiParameterEdsmStarystem.StarSystem);
        if (addOrUpdateResult > 0)
        {
            log.Info($"{((addOrUpdateResult == 1) ? "Added" : "Updated")} previous system '{webApiParameterEdsmStarystem.StarSystem.Name}' ({webApiParameterEdsmStarystem.StarSystem.Id}) {((addOrUpdateResult == 1) ? "to" : "in")} history, history count: {_historyProvider.GetCount()}");
        }
        else
        {
            log.Warn($"Previous system '{webApiParameterEdsmStarystem.StarSystem.Name}' ({webApiParameterEdsmStarystem.StarSystem.Id}) could not be {((addOrUpdateResult == 1) ? "added to" : "updated in")} history");
        }
    }

    private void onRequestedStarSystemInformation(WebApiParameter webEdsmRequestParameter = null)
    {
        triggerGuiDataUpdateEvent();
    }

    public void triggerGuiDataUpdateEvent(bool force = false)
    {
        if (force)
        {
            GuiDataUpdated?.Invoke(this, EventArgs.Empty);
        }
        else if (!guiDataUpdateTriggerBlocked)
        {
            guiDataUpdateTriggerBlocked = true;
            GuiDataUpdated?.Invoke(this, EventArgs.Empty);
            log.Debug("GUI updated by GUI trigger");
            Task.Run(async delegate
            {
                await Task.Delay(_guiDataUpdateTriggerThreshold);
                if (guiDataUpdateTriggerPending)
                {
                    GuiDataUpdated?.Invoke(this, EventArgs.Empty);
                    log.Debug("GUI updated by pending GUI trigger");
                    guiDataUpdateTriggerPending = false;
                }
                guiDataUpdateTriggerBlocked = false;
            });
        }
        else
        {
            guiDataUpdateTriggerPending = true;
            log.Debug("GUI trigger called, but last trigger below threshold, GUI not updated, pending");
        }
    }

    public void SetRouteIsLoadingStatus(bool status, string text)
    {
        RouteIsLoading = status;
        RouteLoadingStatusChanged?.Invoke(this, text);
    }

    public void SetSurroundingsAreLoadingStatus(bool status, string text)
    {
        SurroundingsAreLoading = status;
        SurroundingsLoadingStatusChanged?.Invoke(this, text);
    }

    private async void _navRouteProvider_SystemsOnRouteChanged(object sender, ConcurrentDictionary<long, StarSystem> systemsOnRoute, bool firstRead)
    {
        StarSystemsOnRoute.Clear();
        try
        {
            await setRoute(systemsOnRoute, firstRead).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            log.Error("Unable to set route", exception);
        }
    }

    private async Task setRoute(ConcurrentDictionary<long, StarSystem> systemsOnNewRoute, bool firstRead = false)
    {
        bool currentSystemInNewRoute = isSystemInRoute(CurrentSystem, systemsOnNewRoute);
        int currentSystemJumpDistance = 0;
        if (currentSystemInNewRoute)
        {
            currentSystemJumpDistance = systemsOnNewRoute[CurrentSystem.Id].JumpDistance;
        }
        await Task.Run(delegate
        {
            double routeProgressCounter = 0.0;
            foreach (KeyValuePair<long, StarSystem> routeEntry in systemsOnNewRoute)
            {
                StarSystem historyStarSystem;
                if (routeEntry.Key == CurrentSystem.Id && StarSystemsOnRoute.TryAdd(routeEntry.Key, CurrentSystem))
                {
                    StarSystemsOnRoute[routeEntry.Key].JumpDistance = routeEntry.Value.JumpDistance;
                    StarSystemsOnRoute[routeEntry.Key].JumpDistanceLy = routeEntry.Value.JumpDistanceLy;
                    log.Debug($"Star system on route '{routeEntry.Key}' is the current system");
                }
                else if (_historyProvider.TryGetStarSystem(routeEntry.Key, out historyStarSystem) && StarSystemsOnRoute.TryAdd(routeEntry.Key, historyStarSystem))
                {
                    StarSystemsOnRoute[routeEntry.Key].JumpDistance = routeEntry.Value.JumpDistance;
                    StarSystemsOnRoute[routeEntry.Key].JumpDistanceLy = routeEntry.Value.JumpDistanceLy;
                    log.Debug($"Restored star system on route '{routeEntry.Key}' from the history");
                    calculateCartographicValues(StarSystemsOnRoute[routeEntry.Key]);
                    FindMatchingClassificationsForSystem(StarSystemsOnRoute[routeEntry.Key], ignoreSpeechOutput: true);
                }
                else
                {
                    StarSystemsOnRoute.TryAdd(routeEntry.Key, routeEntry.Value);
                    log.Debug($"Star system on route '{routeEntry.Key}' is a new system");
                }
                if ((!_journalProvider.JournalFirstParse && currentSystemInNewRoute) && StarSystemsOnRoute[routeEntry.Key].JumpDistance >= currentSystemJumpDistance && StarSystemsOnRoute[routeEntry.Key].JumpDistance <= currentSystemJumpDistance + 25)
                {
                    _WebApiProvider.EdsmCheckAndRequestStarSystemInformation(new WebApiParameterEdsmStarystem(StarSystemsOnRoute[routeEntry.Key]), onRequestedStarSystemInformation, forceUpdate: false, ignoreSpeechOutput: true);
                }
                if (currentSystemInNewRoute && StarSystemsOnRoute[routeEntry.Key].JumpDistance < currentSystemJumpDistance)
                {
                    StarSystemsOnRoute[routeEntry.Key].IsPastSystemInRoute = true;
                }
                else if (currentSystemInNewRoute && StarSystemsOnRoute[routeEntry.Key].JumpDistance == currentSystemJumpDistance)
                {
                    StarSystemsOnRoute[routeEntry.Key].IsCurrentSystemInRoute = true;
                }
                else if (currentSystemInNewRoute && StarSystemsOnRoute[routeEntry.Key].JumpDistance == currentSystemJumpDistance + 1 && _routeProvider.IsCustomRoute)
                {
                    copySystemNameToClipboard(routeEntry.Value.Name);
                }
                SetRouteIsLoadingStatus(status: true, $"Setting up route: {Math.Round(routeProgressCounter / (double)systemsOnNewRoute.Count * 100.0)} %");
                routeProgressCounter++;
            }
            triggerGuiDataUpdateEvent(force: true);
            if (firstRead)
            {
                _journalProvider.Initialize();
            }
            if (!firstRead && currentSystemInNewRoute)
            {
                CurrentSystemInRouteChanged?.Invoke(this, EventArgs.Empty);
            }
            SetRouteIsLoadingStatus(status: false, string.Empty);
        });
    }

    private async Task replaceStarSystemInRoute(StarSystem starSystem, bool isCurrentSystem)
    {
        if (!StarSystemsOnRoute.ContainsKey(starSystem.Id))
        {
            log.Warn($"Could not replace star system '{starSystem.Name}' ({starSystem.Id}) in route because it is not part of the route");
            return;
        }
        int jumpDistance = StarSystemsOnRoute[starSystem.Id].JumpDistance;
        double jumpDistanceLy = StarSystemsOnRoute[starSystem.Id].JumpDistanceLy;
        StarSystemsOnRoute[starSystem.Id] = starSystem;
        StarSystemsOnRoute[starSystem.Id].JumpDistance = jumpDistance;
        StarSystemsOnRoute[starSystem.Id].JumpDistanceLy = jumpDistanceLy;
        StarSystemsOnRoute[starSystem.Id].IsCurrentSystemInRoute = isCurrentSystem;
        if (isCurrentSystem && !_journalProvider.JournalFirstParse)
        {
            await SetPastStarSystemsOnRouteAndRequestEDSMDataForUpcomingStarSystemsOnRoute();
            CurrentSystemInRouteChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            triggerGuiDataUpdateEvent();
        }
    }

    private void _journalProvider_ParsedJournalDataUpdated(object sender, bool shutdown)
    {
        if (CurrentSystem.EdsmTotalBodyCount.HasValue && CurrentSystem.EdsmTotalBodyCount > CurrentSystem.Bodies.Count && IsInTeam)
        {
            log.Info($"Exploring in a team and less bodies discovered than EDSM knows about: forcing EDSM update for current system {CurrentSystem.Name} ({CurrentSystem.Id}) trying to get bodies discovered by other commanders in the team");
            _WebApiProvider.EdsmCheckAndRequestStarSystemInformation(new WebApiParameterEdsmStarystem(CurrentSystem), onRequestedStarSystemInformation, forceUpdate: true, _journalProvider.JournalFirstParse);
        }
        else
        {
            _WebApiProvider.EdsmCheckAndRequestStarSystemInformation(new WebApiParameterEdsmStarystem(CurrentSystem), onRequestedStarSystemInformation, forceUpdate: false, _journalProvider.JournalFirstParse);
        }
        if (shutdown)
        {
            int addOrUpdateResult = _historyProvider.AddOrUpdateStarSystem(CurrentSystem);
            if (addOrUpdateResult > 0)
            {
                log.Info($"Journal shutdown detected, {((addOrUpdateResult == 1) ? "added" : "updated")} current system '{CurrentSystem.Name}' ({CurrentSystem.Id}) {((addOrUpdateResult == 1) ? "to" : "in")} history, history count: {_historyProvider.GetCount()}");
            }
            else
            {
                log.Warn($"Journal shutdown detected, but current system '{CurrentSystem.Name}' ({CurrentSystem.Id}) could not be added to history, history count: {_historyProvider.GetCount()}");
            }
        }
    }

    private void _statusProvider_StatusUpdated(object sender, Activity nextActivity, bool isInTeam, string planetNameExploring, long? destinationSystemId)
    {
        IsInTeam = isInTeam;
        if (planetNameExploring == null && CurrentPlanet != null)
        {
            CurrentPlanet.IsCurrentPlanetInSystem = false;
            CurrentPlanet = null;
            CurrentPlanetChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (CurrentPlanet?.Name != planetNameExploring)
        {
            Planet planet = (Planet)CurrentSystem.Bodies.FirstOrDefault((KeyValuePair<int, Body> body) => body.Value.Name == planetNameExploring && body.Value.GetType() == typeof(Planet)).Value;
            if (planet != null)
            {
                if (CurrentPlanet != null)
                {
                    CurrentPlanet.IsCurrentPlanetInSystem = false;
                }
                CurrentPlanet = planet;
                CurrentPlanet.IsCurrentPlanetInSystem = true;
                CurrentPlanetChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        if (CurrentActivity != nextActivity)
        {
            if (nextActivity == Activity.Jump && destinationSystemId.HasValue && StarSystemsOnRoute.ContainsKey(destinationSystemId.Value))
            {
                DestinationSystem = StarSystemsOnRoute[destinationSystemId.Value];
                DestinationSystem.IsJumpDestinationSystemInRoute = true;
            }
            else if (CurrentActivity == Activity.Jump && DestinationSystem != null)
            {
                DestinationSystem.IsJumpDestinationSystemInRoute = false;
                DestinationSystem = null;
            }
            log.Info($"Current activity changing from {CurrentActivity} to {nextActivity}");
            CurrentActivity = nextActivity;
        }
        triggerGuiDataUpdateEvent();
    }

    private void _statusProvider_LocationUpdated(object sender, double? longitude, double? latitude, double? radius)
    {
        if (longitude.HasValue && latitude.HasValue && radius.HasValue && CurrentPlanet != null)
        {
            if (LocationOnCurrentPlanet == null)
            {
                LocationOnCurrentPlanet = new LocationOnPlanet(longitude.Value, latitude.Value, radius.Value);
            }
            else
            {
                LocationOnCurrentPlanet.Longitude = longitude.Value;
                LocationOnCurrentPlanet.Latitude = latitude.Value;
                LocationOnCurrentPlanet.PlanetRadius = radius.Value;
            }
        }
        else
        {
            LocationOnCurrentPlanet = null;
        }
        GuiLocationDataUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void _starSystemProvider_ShipFuelUpdated(object sender, double main, double reservoir)
    {
        if (CurrentShip != null)
        {
            CurrentShip.MainFuel = main;
            CurrentShip.ReserveFuel = reservoir;
            CurrentShip.UpdateJumpRange();
            log.Debug($"Fuel update for current ship '{CurrentShip.Name}': main fuel is {CurrentShip.MainFuel} t, reserve fuel is {CurrentShip.ReserveFuel} ");
            GuiShipFuelDataUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool isSystemInRoute(StarSystem starSystem, ConcurrentDictionary<long, StarSystem> route)
    {
        if (starSystem.Id != 0L)
        {
            return route.ContainsKey(starSystem.Id);
        }
        return false;
    }
}
