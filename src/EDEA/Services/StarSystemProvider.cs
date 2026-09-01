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

/// <summary>Defines the activities the commander can currently perform.</summary>
public enum Activity
{
    None,
    ExploreSystem,
    GalaxyMap,
    Jump,
    ExplorePlanet,
    Other
}

/// <summary>Delegate for the <see cref="StarSystemProvider.RouteLoadingStatusChanged"/> event.</summary>
/// <param name="sender">The source of the event.</param>
/// <param name="statusText">The current loading status text.</param>
public delegate void RouteLoadingStatusChangedEventHandler(object sender, string statusText);
/// <summary>Delegate for the <see cref="StarSystemProvider.SurroundingsLoadingStatusChanged"/> event.</summary>
/// <param name="sender">The source of the event.</param>
/// <param name="statusText">The current loading status text.</param>
public delegate void SurroundingsLoadingStatusChangedEventHandler(object sender, string statusText);

/// <summary>
/// Central service that provides information about the current and surrounding star systems,
/// manages the route, and coordinates communication between providers and the GUI.
/// </summary>
public class StarSystemProvider
{
    /// <summary>The singleton instance of the <see cref="StarSystemProvider"/>.</summary>
    private static StarSystemProvider instance;

    /// <summary>The logger used by this provider.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(StarSystemProvider));

    /// <summary>Provides access to the persisted star system history.</summary>
    private readonly HistoryProvider _historyProvider;

    /// <summary>Provides access to the EDSM Web API.</summary>
    private WebApiProvider _WebApiProvider;
    /// <summary>Provides the current navigation route.</summary>
    private RouteProvider _routeProvider;
    /// <summary>Provides parsed journal data.</summary>
    private JournalProvider _journalProvider;
    /// <summary>Provides planet-of-interest matching data.</summary>
    private PlanetsOfInterestProvider _planetsOfInterestProvider;

    /// <summary>Threshold in milliseconds for throttling GUI data update events.</summary>
    private readonly int _guiDataUpdateTriggerThreshold = 200;

    /// <summary>Indicates whether a GUI data update is currently blocked.</summary>
    private bool guiDataUpdateTriggerBlocked;
    /// <summary>Indicates whether a GUI data update is pending while throttled.</summary>
    private bool guiDataUpdateTriggerPending;
    /// <summary>Identifier of the star system for which surroundings have been loaded.</summary>
    private long surroundingsSystemId;
    /// <summary>Indicates whether the surroundings view is currently visible.</summary>
    private bool surroundingsVisible;

    /// <summary>Gets the star systems that are part of the current route.</summary>
    /// <value>A <see cref="ConcurrentDictionary{TKey, TValue}"/> keyed by system id.</value>
    public ConcurrentDictionary<long, StarSystem> StarSystemsOnRoute { get; }
    /// <summary>Gets or sets the star systems surrounding the current system.</summary>
    /// <value>A <see cref="ConcurrentDictionary{TKey, TValue}"/> keyed by system id.</value>
    public ConcurrentDictionary<long, StarSystem> SurroundingStarSystems { get; private set; }

    /// <summary>Gets or sets the currently selected star system.</summary>
    /// <value>The current <see cref="StarSystem"/>.</value>
    public StarSystem CurrentSystem { get; private set; }
    /// <summary>Gets or sets the destination system when a jump is in progress.</summary>
    /// <value>The destination <see cref="StarSystem"/>.</value>
    public StarSystem DestinationSystem { get; private set; }
    /// <summary>Gets or sets the current commander activity.</summary>
    /// <value>The current <see cref="Activity"/>.</value>
    public Activity CurrentActivity { get; private set; }
    /// <summary>Gets or sets the currently explored planet.</summary>
    /// <value>The current <see cref="Planet"/>.</value>
    public Planet CurrentPlanet { get; private set; }
    /// <summary>Gets or sets the surface location on the current planet.</summary>
    /// <value>The current <see cref="LocationOnPlanet"/>.</value>
    public LocationOnPlanet LocationOnCurrentPlanet { get; private set; }
    /// <summary>Gets or sets a value indicating whether the commander is in a team.</summary>
    /// <value><c>true</c> if the commander is in a team; otherwise, <c>false</c>.</value>
    public bool IsInTeam { get; private set; }
    /// <summary>Gets or sets the commander name.</summary>
    /// <value>The name of the commander.</value>
    public string CommanderName { get; set; }
    /// <summary>Gets or sets the names of the commanders teammates.</summary>
    /// <value>A list of teammate names.</value>
    public List<string> TeammateNames { get; set; }

    /// <summary>Gets a value indicating whether the <see cref="CurrentSystem"/> is part of the route.</summary>
    /// <value><c>true</c> if the current system is on the route; otherwise, <c>false</c>.</value>
    public bool IsCurrentSystemInRoute => isSystemInRoute(CurrentSystem, StarSystemsOnRoute);
    /// <summary>Gets or sets a value indicating whether route data is loading.</summary>
    /// <value><c>true</c> if the route is loading; otherwise, <c>false</c>.</value>
    public bool RouteIsLoading { get; private set; }
    /// <summary>Gets or sets a value indicating whether surrounding systems are loading.</summary>
    /// <value><c>true</c> if the surroundings are loading; otherwise, <c>false</c>.</value>
    public bool SurroundingsAreLoading { get; private set; }
    /// <summary>Gets or sets the currently used ship.</summary>
    /// <value>The current <see cref="Ship"/>.</value>
    public Ship CurrentShip { get; set; }

    /// <summary>Raised when general GUI data has been updated.</summary>
    public event EventHandler GuiDataUpdated = delegate { };
    /// <summary>Raised when planetary location data has been updated.</summary>
    public event EventHandler GuiLocationDataUpdated = delegate { };
    /// <summary>Raised when ship fuel data has been updated.</summary>
    public event EventHandler GuiShipFuelDataUpdated = delegate { };
    /// <summary>Raised when the current system membership in the route changes.</summary>
    public event EventHandler CurrentSystemInRouteChanged = delegate { };
    /// <summary>Raised when the current planet changes.</summary>
    public event EventHandler CurrentPlanetChanged = delegate { };
    /// <summary>Raised when the route loading status text changes.</summary>
    public event RouteLoadingStatusChangedEventHandler RouteLoadingStatusChanged = delegate { };
    /// <summary>Raised when the surroundings loading status text changes.</summary>
    public event SurroundingsLoadingStatusChangedEventHandler SurroundingsLoadingStatusChanged = delegate { };

    /// <summary>Returns the singleton <see cref="StarSystemProvider"/> instance.</summary>
    /// <param name="historyProvider">The <see cref="HistoryProvider"/> used for history persistence.</param>
    /// <returns>The singleton <see cref="StarSystemProvider"/> instance.</returns>
    public static StarSystemProvider Instance(HistoryProvider historyProvider)
    {
        if (instance == null)
        {
            instance = new StarSystemProvider(historyProvider);
        }
        return instance;
    }

    /// <summary>Initializes a new instance of the <see cref="StarSystemProvider"/> class.</summary>
    /// <param name="historyProvider">The <see cref="HistoryProvider"/> used for history persistence.</param>
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

    /// <summary>Registers a provider with this <see cref="StarSystemProvider"/>.</summary>
    /// <param name="provider">The <see cref="object"/> representing the provider to register.</param>
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

    /// <summary>Requests EDSM data for the current star system.</summary>
    /// <param name="forceUpdate">A <see cref="Boolean"/> indicating whether to force an update.</param>
    public void HandleLoadEdsmSystemDataCommand(bool forceUpdate)
    {
        _WebApiProvider.EdsmCheckAndRequestStarSystemInformation(new WebApiParameterEdsmStarystem(CurrentSystem), onRequestedStarSystemInformation, forceUpdate, ignoreSpeechOutput: true);
    }

    /// <summary>Handles the application shutdown and persists the current system.</summary>
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

    /// <summary>Handles a change of the current star system, updates history and requests additional data.</summary>
    /// <param name="potentialNewSystem">The <see cref="StarSystem"/> that may become the current system.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
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

    /// <summary>Handles the selected tab index change in the main view model.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> containing the event data.</param>
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

    /// <summary>Requests the surrounding star systems for the current system from EDSM.</summary>
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

    /// <summary>Marks past star systems on the route and requests EDSM data for upcoming systems.</summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
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
                                                where sysItem.Value.JumpDistance > jd && (sysItem.Value.NeedsEdsmSystemUpdate || sysItem.Value.NeedsEdsmBodiesUpdate)
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

    /// <summary>Copies the name of the next route system to the clipboard.</summary>
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

    /// <summary>Predicts the occurrence of valuable species for all planets in the current system.</summary>
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

    /// <summary>Predicts the occurrence of valuable species for the specified planet.</summary>
    /// <param name="planet">The <see cref="Planet"/> for which to predict species occurrence.</param>
    public void PredictOccurrenceOfSpeciesForPlanet(Planet planet)
    {
        GeneraIndexProvider.PredictOccurrenceOfSpecies(planet);
        announcePredictedOccurrenceOfSpecies(planet);
        planet.InitialPredictionOfSpecies = false;
        triggerGuiDataUpdateEvent();
    }

    /// <summary>Finds matching planet classifications for the current route and surroundings.</summary>
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

    /// <summary>Finds matching planet classifications for the specified star system.</summary>
    /// <param name="starSystem">The <see cref="StarSystem"/> to check for classifications.</param>
    /// <param name="ignoreSpeechOutput">A <see cref="Boolean"/> indicating whether to suppress speech output.</param>
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

    /// <summary>Finds matching planet classifications for the specified planet.</summary>
    /// <param name="planet">The <see cref="Planet"/> to check for classifications.</param>
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

    /// <summary>Initializes the current system by raising the route membership event if applicable.</summary>
    public void InitializeCurrentSystem()
    {
        if (IsCurrentSystemInRoute)
        {
            CurrentSystemInRouteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Resets analysis data for incomplete genera on the current planet.</summary>
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

    /// <summary>View model that combines a genus classification with its planet for sorting.</summary>
    private class GenusClassificationViewModel
    {
        /// <summary>Gets the genus classification.</summary>
        /// <value>The <see cref="GenusClassification"/> value.</value>
        public GenusClassification GenusClassification { get; }
        /// <summary>Gets the planet associated with the classification.</summary>
        /// <value>The <see cref="Planet"/> instance.</value>
        public Planet Planet { get; }
        /// <summary>Gets the maximum Vista Genomics value for sorting purposes.</summary>
        /// <value>The calculated sort value.</value>
        public int VistaGenomicsMaxValueSort => GenusClassification.VistaGenomicsBaseValue * 5;

        /// <summary>Initializes a new instance of the <see cref="GenusClassificationViewModel"/> class.</summary>
        /// <param name="genusClassification">The <see cref="GenusClassification"/> to wrap.</param>
        /// <param name="planet">The <see cref="Planet"/> associated with the classification.</param>
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

    /// <summary>Triggers a GUI data update event, optionally forcing an immediate update.</summary>
    /// <param name="force">A <see cref="Boolean"/> indicating whether to force the update immediately.</param>
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

    /// <summary>Sets the route loading status and raises the corresponding event.</summary>
    /// <param name="status">A <see cref="Boolean"/> indicating whether the route is loading.</param>
    /// <param name="text">The loading status text.</param>
    public void SetRouteIsLoadingStatus(bool status, string text)
    {
        RouteIsLoading = status;
        RouteLoadingStatusChanged?.Invoke(this, text);
    }

    /// <summary>Sets the surroundings loading status and raises the corresponding event.</summary>
    /// <param name="status">A <see cref="Boolean"/> indicating whether the surroundings are loading.</param>
    /// <param name="text">The loading status text.</param>
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
