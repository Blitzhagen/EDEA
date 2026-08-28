using System;
using System.Collections.Concurrent;
using System.Reflection;
using EDEA.Models;
using EDEA.Properties;
using log4net;

using EDEA.Enums;

namespace EDEA.ViewModels;

/// <summary>
/// View model that wraps a <see cref="StarSystem"/> for display in route and surroundings tables.
/// </summary>
public class StarSystemViewModel : ViewModelBase
{
    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(StarSystemViewModel));

    /// <summary>
    /// The underlying star system model.
    /// </summary>
    private readonly StarSystem _starSystem;

    /// <summary>
    /// The journal-based exploration status.
    /// </summary>
    private StarSystemExplorationStatus journalExplorationStatus { get; }

    /// <summary>
    /// The EDSM exploration status.
    /// </summary>
    private StarSystemExplorationStatus edsmExplorationStatus { get; }

    /// <summary>
    /// The combined exploration status.
    /// </summary>
    private StarSystemExplorationStatus explorationStatus { get; }

    /// <summary>
    /// Gets the name of the star system.
    /// </summary>
    /// <value>The system name.</value>
    public string Name => _starSystem.Name;

    /// <summary>
    /// Gets the star class.
    /// </summary>
    /// <value>The star class.</value>
    public string StarClass { get; }

    /// <summary>
    /// Gets the name of the primary star.
    /// </summary>
    /// <value>The primary star name.</value>
    public string PrimaryStarName => _starSystem.PrimaryStarName ?? string.Empty;

    /// <summary>
    /// Gets a value indicating whether the primary star is scoopable.
    /// </summary>
    /// <value><c>true</c> if the primary star is scoopable; otherwise, <c>false</c>.</value>
    public bool PrimaryStarIsScoopable => _starSystem.PrimaryStarIsScoopable;

    /// <summary>
    /// Gets the jump distance.
    /// </summary>
    /// <value>The jump distance.</value>
    public int JumpDistance => _starSystem.JumpDistance;

    /// <summary>
    /// Gets the jump distance in light years used for sorting.
    /// </summary>
    /// <value>The sortable jump distance.</value>
    public double JumpDistanceLySort => _starSystem.JumpDistanceLy;

    /// <summary>
    /// Gets the formatted jump distance in light years.
    /// </summary>
    /// <value>The jump distance string.</value>
    public string JumpDistanceLy { get; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this system has been passed in the route.
    /// </summary>
    /// <value><c>true</c> if the system has been passed; otherwise, <c>false</c>.</value>
    public bool IsPastSystemInRoute => _starSystem.IsPastSystemInRoute;

    /// <summary>
    /// Gets a value indicating whether this system is the jump destination in the route.
    /// </summary>
    /// <value><c>true</c> if the system is the jump destination; otherwise, <c>false</c>.</value>
    public bool IsJumpDestinationSystemInRoute => _starSystem.IsJumpDestinationSystemInRoute;

    /// <summary>
    /// Gets a value indicating whether this system is the current system in the route.
    /// </summary>
    /// <value><c>true</c> if the system is the current system; otherwise, <c>false</c>.</value>
    public bool IsCurrentSystemInRoute => _starSystem.IsCurrentSystemInRoute;

    /// <summary>
    /// Gets a value indicating whether this system is ahead in the route.
    /// </summary>
    /// <value><c>true</c> if the system is ahead; otherwise, <c>false</c>.</value>
    public bool IsSystemInRouteAhead => _starSystem.IsSystemInRouteAhead;

    /// <summary>
    /// Gets the EDSM name of the system.
    /// </summary>
    /// <value>The EDSM name.</value>
    public string EdsmName => _starSystem.EdsmName ?? string.Empty;

    /// <summary>
    /// Gets the EDSM primary star name.
    /// </summary>
    /// <value>The EDSM primary star name.</value>
    public string EdsmPrimaryStarName => _starSystem.EdsmPrimaryStarName ?? string.Empty;

    /// <summary>
    /// Gets the EDSM primary star type.
    /// </summary>
    /// <value>The EDSM primary star type.</value>
    public string EdsmPrimaryStarType => _starSystem.EdsmPrimaryStarType ?? string.Empty;

    /// <summary>
    /// Gets a value indicating whether the EDSM primary star is scoopable.
    /// </summary>
    /// <value><c>true</c> if the EDSM primary star is scoopable; otherwise, <c>false</c>.</value>
    public bool EdsmPrimaryStarIsScoopable => _starSystem.EdsmPrimaryStarIsScoopable;

    /// <summary>
    /// Gets a value indicating whether the system was read from EDSM.
    /// </summary>
    /// <value><c>true</c> if the system was read from EDSM; otherwise, <c>false</c>.</value>
    public bool WasReadFromEdsm => _starSystem.WasReadFromEdsm;

    /// <summary>
    /// Gets a value indicating whether the system was read from EDSM only.
    /// </summary>
    /// <value><c>true</c> if the system was read from EDSM only; otherwise, <c>false</c>.</value>
    public bool WasReadFromEdsmOnly => _starSystem.WasReadFromEdsmOnly;

    /// <summary>
    /// Gets a value indicating whether the system was completely read from the journal.
    /// </summary>
    /// <value><c>true</c> if the system was completely read from the journal; otherwise, <c>false</c>.</value>
    public bool WasCompletelyReadFromJournal => journalExplorationStatus == StarSystemExplorationStatus.Complete;

    /// <summary>
    /// Gets the dictionary of matching planet classifications.
    /// </summary>
    /// <value>The matching planet classifications.</value>
    public ConcurrentDictionary<string, int> MatchingPlanetClassifications { get; }

    /// <summary>
    /// Gets the matching planet classifications count used for sorting.
    /// </summary>
    /// <value>The sortable matching planet classifications count.</value>
    public int MatchingPlanetClassificationsSort { get; }

    /// <summary>
    /// Gets the formatted matching planet classifications count.
    /// </summary>
    /// <value>The matching planet classifications count string.</value>
    public string MatchingPlanetClassificationsCount { get; }

    /// <summary>
    /// Gets a value indicating whether matching planet classifications are present.
    /// </summary>
    /// <value><c>true</c> if matching planet classifications are present; otherwise, <c>false</c>.</value>
    public bool HasMatchingPlanetClassifications { get; }

    /// <summary>
    /// Gets the formatted count of planets with matching planet classifications.
    /// </summary>
    /// <value>The planets with matching classifications count string.</value>
    public string PlanetsWithMatchingPlanetClassificationsCount { get; }

    /// <summary>
    /// Gets the EDSM discovery commander.
    /// </summary>
    /// <value>The EDSM discovery commander.</value>
    public string EdsmDiscoveryCommander { get; } = string.Empty;

    /// <summary>
    /// Gets the total body count reported by the journal.
    /// </summary>
    /// <value>The total body count.</value>
    public int TotalBodyCount => _starSystem.TotalBodyCount;

    /// <summary>
    /// Gets a value indicating whether there is a known total body count.
    /// </summary>
    /// <value><c>true</c> if a total body count is known; otherwise, <c>false</c>.</value>
    public bool HasTotalBodyCount => TotalBodyCount > 0;

    /// <summary>
    /// Gets a value indicating whether no total body count is known.
    /// </summary>
    /// <value><c>true</c> if no total body count is known; otherwise, <c>false</c>.</value>
    public bool HasNoTotalBodyCount => !HasTotalBodyCount;

    /// <summary>
    /// Gets the total non-body count.
    /// </summary>
    /// <value>The total non-body count.</value>
    public int TotalNonBodyCount => _starSystem.TotalNonBodyCount;

    /// <summary>
    /// Gets a value indicating whether all bodies were found.
    /// </summary>
    /// <value><c>true</c> if all bodies were found; otherwise, <c>false</c>.</value>
    public bool AllBodiesFound
    {
        get
        {
            if (!_starSystem.AllBodiesFound)
            {
                if (TotalBodyCount > 0)
                {
                    return TotalBodyCount == ExploredBodyCount;
                }
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Gets the number of explored bodies.
    /// </summary>
    /// <value>The explored body count.</value>
    public int ExploredBodyCount { get; }

    /// <summary>
    /// Gets the EDSM total body count, if known.
    /// </summary>
    /// <value>The EDSM total body count, or <c>null</c>.</value>
    public int? EdsmTotalBodyCount => _starSystem.EdsmTotalBodyCount;

    /// <summary>
    /// Gets a value indicating whether an EDSM total body count is known.
    /// </summary>
    /// <value><c>true</c> if an EDSM total body count is known; otherwise, <c>false</c>.</value>
    public bool HasEdsmTotalBodyCount
    {
        get
        {
            if (EdsmTotalBodyCount.HasValue)
            {
                return EdsmTotalBodyCount > 0;
            }
            return false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether no EDSM total body count is known.
    /// </summary>
    /// <value><c>true</c> if no EDSM total body count is known; otherwise, <c>false</c>.</value>
    public bool HasNoEdsmTotalBodyCount => !HasEdsmTotalBodyCount;

    /// <summary>
    /// Gets the number of bodies explored according to EDSM.
    /// </summary>
    /// <value>The EDSM explored body count.</value>
    public int EdsmExploredBodyCount { get; set; }

    /// <summary>
    /// Gets the total number of bodies used for display.
    /// </summary>
    /// <value>The total bodies count.</value>
    public int TotalBodies { get; }

    /// <summary>
    /// Gets a value indicating whether there are total bodies.
    /// </summary>
    /// <value><c>true</c> if there are total bodies; otherwise, <c>false</c>.</value>
    public bool HasTotalBodies => TotalBodies > 0;

    /// <summary>
    /// Gets a value indicating whether there are no total bodies.
    /// </summary>
    /// <value><c>true</c> if there are no total bodies; otherwise, <c>false</c>.</value>
    public bool HasNoTotalBodies => !HasTotalBodies;

    /// <summary>
    /// Gets the number of explored stars.
    /// </summary>
    /// <value>The explored star count.</value>
    public int ExploredStars { get; }

    /// <summary>
    /// Gets a value indicating whether any stars were explored.
    /// </summary>
    /// <value><c>true</c> if stars were explored; otherwise, <c>false</c>.</value>
    public bool HasExploredStars { get; }

    /// <summary>
    /// Gets the number of explored planets.
    /// </summary>
    /// <value>The explored planet count.</value>
    public int ExploredPlanets { get; }

    /// <summary>
    /// Gets a value indicating whether any planets were explored.
    /// </summary>
    /// <value><c>true</c> if planets were explored; otherwise, <c>false</c>.</value>
    public bool HasExploredPlanets { get; }

    /// <summary>
    /// Gets the number of explored bodies.
    /// </summary>
    /// <value>The explored body count.</value>
    public int ExploredBodies { get; }

    /// <summary>
    /// Gets the number of explored non-bodies.
    /// </summary>
    /// <value>The explored non-body count.</value>
    public int ExploredNonBodies { get; }

    /// <summary>
    /// Gets the number of explored landables used for sorting.
    /// </summary>
    /// <value>The sortable landable count.</value>
    public int ExploredLandablesSort { get; }

    /// <summary>
    /// Gets the formatted count of explored landables.
    /// </summary>
    /// <value>The explored landable count string.</value>
    public string ExploredLandablesCount { get; }

    /// <summary>
    /// Gets a value indicating whether any landables were explored.
    /// </summary>
    /// <value><c>true</c> if landables were explored; otherwise, <c>false</c>.</value>
    public bool HasExploredLandables { get; }

    /// <summary>
    /// Gets the number of explored terraformables used for sorting.
    /// </summary>
    /// <value>The sortable terraformable count.</value>
    public int ExploredTerraformablesSort { get; }

    /// <summary>
    /// Gets the formatted count of explored terraformables.
    /// </summary>
    /// <value>The explored terraformable count string.</value>
    public string ExploredTerraformablesCount { get; }

    /// <summary>
    /// Gets a value indicating whether any terraformables were explored.
    /// </summary>
    /// <value><c>true</c> if terraformables were explored; otherwise, <c>false</c>.</value>
    public bool HasExploredTerraformables { get; }

    /// <summary>
    /// Gets the number of valuable bodies used for sorting.
    /// </summary>
    /// <value>The sortable valuable body count.</value>
    public int ValuableBodiesSort { get; }

    /// <summary>
    /// Gets the formatted count of valuable bodies.
    /// </summary>
    /// <value>The valuable body count string.</value>
    public string ValuableBodiesCount { get; }

    /// <summary>
    /// Gets a value indicating whether any valuable bodies are present.
    /// </summary>
    /// <value><c>true</c> if valuable bodies are present; otherwise, <c>false</c>.</value>
    public bool HasValuableBodies { get; }

    /// <summary>
    /// Gets the journal exploration status as a localized string.
    /// </summary>
    /// <value>The journal exploration status string.</value>
    public string JournalExplorationStatus => journalExplorationStatus switch
    {
        StarSystemExplorationStatus.Unknown => Resources.Status_Unknown,
        StarSystemExplorationStatus.Unexplored => Resources.Status_Unexplored,
        StarSystemExplorationStatus.Unscanned => Resources.Status_Unscanned,
        StarSystemExplorationStatus.Incomplete => Resources.Status_Incomplete,
        StarSystemExplorationStatus.Complete => Resources.Status_Complete,
        _ => Resources.Status_Unknown
    };

    /// <summary>
    /// Gets the EDSM exploration status as a localized string.
    /// </summary>
    /// <value>The EDSM exploration status string.</value>
    public string EdsmExplorationStatus => edsmExplorationStatus switch
    {
        StarSystemExplorationStatus.Unknown => Resources.Status_Unknown,
        StarSystemExplorationStatus.Unexplored => Resources.Status_Unexplored,
        StarSystemExplorationStatus.Unscanned => Resources.Status_Unscanned,
        StarSystemExplorationStatus.Incomplete => Resources.Status_Incomplete,
        StarSystemExplorationStatus.Complete => Resources.Status_Complete,
        _ => Resources.Status_Unknown
    };

    /// <summary>
    /// Gets the combined exploration status as a localized string.
    /// </summary>
    /// <value>The exploration status string.</value>
    public string ExplorationStatus => explorationStatus switch
    {
        StarSystemExplorationStatus.Unknown => Resources.Status_Unknown,
        StarSystemExplorationStatus.Unexplored => Resources.Status_Unexplored,
        StarSystemExplorationStatus.Unscanned => Resources.Status_Unscanned,
        StarSystemExplorationStatus.Incomplete => Resources.Status_Incomplete,
        StarSystemExplorationStatus.Complete => Resources.Status_Complete,
        _ => Resources.Status_Unknown
    };

    /// <summary>
    /// Gets the combined exploration status as an enum value.
    /// </summary>
    /// <value>The exploration status enum.</value>
    public StarSystemExplorationStatus ExplorationStatusEnum => explorationStatus;

    /// <summary>
    /// Gets the bodies info text for the exploration status.
    /// </summary>
    /// <value>The bodies info string.</value>
    public string ExplorationStatusBodiesInfo { get; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the exploration status is unknown.
    /// </summary>
    /// <value><c>true</c> if the status is unknown; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusUnknown { get; }

    /// <summary>
    /// Gets a value indicating whether the exploration status is known.
    /// </summary>
    /// <value><c>true</c> if the status is known; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusKnown => !IsExplorationStatusUnknown;

    /// <summary>
    /// Gets a value indicating whether the exploration status is unexplored.
    /// </summary>
    /// <value><c>true</c> if the status is unexplored; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusUnexplored { get; }

    /// <summary>
    /// Gets a value indicating whether the exploration status is complete.
    /// </summary>
    /// <value><c>true</c> if the status is complete; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusComplete { get; }

    /// <summary>
    /// Gets a value indicating whether the exploration status is unscanned.
    /// </summary>
    /// <value><c>true</c> if the status is unscanned; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusUnscanned { get; }

    /// <summary>
    /// Gets a value indicating whether the exploration status is incomplete.
    /// </summary>
    /// <value><c>true</c> if the status is incomplete; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusIncomplete { get; }

    /// <summary>
    /// Gets a value indicating whether the exploration status is ahead of the journal.
    /// </summary>
    /// <value><c>true</c> if the status is ahead of the journal; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusAheadOfJournal { get; }

    /// <summary>
    /// Gets the bodies cartographic maximum value used for sorting.
    /// </summary>
    /// <value>The sortable cartographic maximum value.</value>
    public int BodiesCartographicMaxValueSort { get; }

    /// <summary>
    /// Gets the formatted bodies cartographic maximum value.
    /// </summary>
    /// <value>The cartographic maximum value string.</value>
    public string BodiesCartographicMaxValue { get; }

    /// <summary>
    /// Gets a value indicating whether there is a bodies cartographic maximum value.
    /// </summary>
    /// <value><c>true</c> if a maximum value is present; otherwise, <c>false</c>.</value>
    public bool HasBodiesCartographicMaxValue { get; }

    /// <summary>
    /// Gets the formatted bodies cartographic value.
    /// </summary>
    /// <value>The cartographic value string.</value>
    public string BodiesCartographicValue { get; }

    /// <summary>
    /// Gets a value indicating whether there is a bodies cartographic value.
    /// </summary>
    /// <value><c>true</c> if a cartographic value is present; otherwise, <c>false</c>.</value>
    public bool HasBodiesCartographicValue { get; }

    /// <summary>
    /// Gets the formatted bodies cartographic base value.
    /// </summary>
    /// <value>The base cartographic value string.</value>
    public string BodiesCartographicBaseValue { get; }

    /// <summary>
    /// Gets a value indicating whether there is a bodies cartographic base value.
    /// </summary>
    /// <value><c>true</c> if a base value is present; otherwise, <c>false</c>.</value>
    public bool HasBodiesCartographicBaseValue { get; }

    /// <summary>
    /// Gets the formatted bodies cartographic surface scan value.
    /// </summary>
    /// <value>The surface scan value string.</value>
    public string BodiesCartographicSurfaceScanValue { get; }

    /// <summary>
    /// Gets a value indicating whether there is a surface scan value.
    /// </summary>
    /// <value><c>true</c> if a surface scan value is present; otherwise, <c>false</c>.</value>
    public bool HasBodiesCartographicSurfaceScanValue { get; }

    /// <summary>
    /// Gets a value indicating whether all bodies were surface scanned.
    /// </summary>
    /// <value><c>true</c> if all bodies were surface scanned; otherwise, <c>false</c>.</value>
    public bool WasCompletelySurfaceScanned { get; }

    /// <summary>
    /// Gets the formatted bodies cartographic bonus value.
    /// </summary>
    /// <value>The bonus value string.</value>
    public string BodiesCartographicBonusValue { get; }

    /// <summary>
    /// Gets a value indicating whether there is a bonus value.
    /// </summary>
    /// <value><c>true</c> if a bonus value is present; otherwise, <c>false</c>.</value>
    public bool HasBodiesCartographicBonusValue { get; }

    /// <summary>
    /// Gets the formatted bodies Vista Genomics value.
    /// </summary>
    /// <value>The Vista Genomics value string.</value>
    public string BodiesVistaGenomicsValue { get; }

    /// <summary>
    /// Gets the exploration value used for sorting.
    /// </summary>
    /// <value>The sortable exploration value.</value>
    public int ExplorationValueSort { get; }

    /// <summary>
    /// Gets the formatted exploration value.
    /// </summary>
    /// <value>The exploration value string.</value>
    public string ExplorationValue { get; }

    /// <summary>
    /// Gets a value indicating whether there is an exploration value.
    /// </summary>
    /// <value><c>true</c> if an exploration value is present; otherwise, <c>false</c>.</value>
    public bool HasExplorationValue { get; }

    /// <summary>
    /// Gets the formatted bodies cartographic progress.
    /// </summary>
    /// <value>The cartographic progress string.</value>
    public string BodiesCartographicProgress { get; }

    /// <summary>
    /// Gets a value indicating whether the cartographic maximum value is not reached.
    /// </summary>
    /// <value><c>true</c> if the maximum value is not reached; otherwise, <c>false</c>.</value>
    public bool BodiesCartographicMaxValueNotReached { get; }

    /// <summary>
    /// Gets the formatted bodies Vista Genomics progress.
    /// </summary>
    /// <value>The Vista Genomics progress string.</value>
    public string BodiesVistaGenomicsProgress { get; }

    /// <summary>
    /// Gets the formatted bodies biologicals count.
    /// </summary>
    /// <value>The biologicals count string.</value>
    public string BodiesBiologicalsCount { get; }

    /// <summary>
    /// Gets a value indicating whether there are biologicals on bodies.
    /// </summary>
    /// <value><c>true</c> if biologicals are present; otherwise, <c>false</c>.</value>
    public bool HasBodiesBiologicalsCount { get; }

    /// <summary>
    /// Gets the formatted bodies analysed genuses count.
    /// </summary>
    /// <value>The analysed genuses count string.</value>
    public string BodiesAnalysedGenusesCount { get; }

    /// <summary>
    /// Gets a value indicating whether all genuses on bodies are analysed.
    /// </summary>
    /// <value><c>true</c> if all genuses are analysed; otherwise, <c>false</c>.</value>
    public bool BodiesAllGenusesAnalysed { get; }

    /// <summary>
    /// Gets a value indicating whether there is any overall progress.
    /// </summary>
    /// <value><c>true</c> if overall progress is present; otherwise, <c>false</c>.</value>
    public bool HasOverallProgress { get; }

    /// <summary>
    /// Gets the formatted overall progress.
    /// </summary>
    /// <value>The overall progress string.</value>
    public string OverallProgress { get; }

    /// <summary>
    /// Gets a value indicating whether the overall progress is completed.
    /// </summary>
    /// <value><c>true</c> if overall progress is completed; otherwise, <c>false</c>.</value>
    public bool OverallProgressCompleted { get; }

    /// <summary>
    /// Gets a value indicating whether the system is active.
    /// </summary>
    /// <value><c>true</c> if the system is active; otherwise, <c>false</c>.</value>
    public bool IsActive { get; }

    /// <summary>
    /// Gets a value indicating whether all bodies were surface scanned.
    /// </summary>
    /// <value><c>true</c> if all bodies were surface scanned; otherwise, <c>false</c>.</value>
    public bool AllBodiesSurfaceScanned { get; }

    /// <summary>
    /// Gets a value indicating whether all valuable bodies were surface scanned.
    /// </summary>
    /// <value><c>true</c> if all valuable bodies were surface scanned; otherwise, <c>false</c>.</value>
    public bool AllValuableBodiesSurfaceScanned { get; }

    /// <summary>
    /// Gets the population used for sorting.
    /// </summary>
    /// <value>The sortable population.</value>
    public long PopulationSort { get; }

    /// <summary>
    /// Gets the formatted population.
    /// </summary>
    /// <value>The population string.</value>
    public string Population { get; }

    /// <summary>
    /// Gets a value indicating whether the system has a population.
    /// </summary>
    /// <value><c>true</c> if the system has a population; otherwise, <c>false</c>.</value>
    public bool HasPopulation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StarSystemViewModel"/> class.
    /// </summary>
    /// <param name="starSystem">The star system model to wrap.</param>
    public StarSystemViewModel(StarSystem starSystem)
    {
        _starSystem = starSystem;
        JumpDistanceLy = ((_starSystem.JumpDistanceLy == 0.0 || IsCurrentSystemInRoute || IsPastSystemInRoute) ? string.Empty : (_starSystem.JumpDistanceLy.ToString("n2") + " " + Resources.UnitLightYears));
        if (TotalBodyCount > 0)
        {
            TotalBodies = TotalBodyCount;
        }
        else if (EdsmTotalBodyCount.HasValue)
        {
            TotalBodies = EdsmTotalBodyCount.Value;
        }
        if (string.IsNullOrEmpty(_starSystem.StarClass))
        {
            StarClass = EdsmPrimaryStarType;
        }
        else if (Globals.PlotterStarClasses.ContainsValue(_starSystem.StarClass) && !string.IsNullOrEmpty(EdsmPrimaryStarType))
        {
            StarClass = EdsmPrimaryStarType;
        }
        else
        {
            StarClass = _starSystem.StarClass;
        }
        PopulationSort = _starSystem.Population;
        Population = PopulationSort.ToString("n0");
        HasPopulation = PopulationSort > 0;
        MatchingPlanetClassifications = new ConcurrentDictionary<string, int>();
        int matchingPlanetsCount = 0;
        int cartographicMaxValueSum = 0;
        int cartographicValueSum = 0;
        int cartographicBaseValueSum = 0;
        int surfaceScanValueSum = 0;
        int vistaGenomicsValueSum = 0;
        int biologicalsCount = 0;
        int analysedGenusesCount = 0;
        AllBodiesSurfaceScanned = true;
        AllValuableBodiesSurfaceScanned = true;
        foreach (Body body in _starSystem.Bodies.Values)
        {
            BodyViewModel bodyViewModel = new BodyViewModel(body);
            cartographicMaxValueSum += body.CartographicMaxValue;
            cartographicValueSum += body.CartographicValue;
            cartographicBaseValueSum += body.CartographicBaseValue;
            if (bodyViewModel.IsValuableBody)
            {
                ValuableBodiesSort++;
            }
            if (body.Type == BodyType.Star || body.Type == BodyType.Planet)
            {
                if (body.WasReadFromJournal)
                {
                    ExploredBodyCount++;
                }
                if (body.WasReadFromEdsm)
                {
                    EdsmExploredBodyCount++;
                }
                if (body.Type == BodyType.Star)
                {
                    ExploredStars++;
                    if (body.Name == PrimaryStarName || body.Name == EdsmPrimaryStarName)
                    {
                        EdsmDiscoveryCommander = bodyViewModel.EdsmDiscoveryCommander;
                    }
                }
                else
                {
                    if (body.Type != BodyType.Planet)
                    {
                        continue;
                    }
                    Planet planet = (Planet)body;
                    ExploredPlanets++;
                    if (planet.IsLandable)
                    {
                        ExploredLandablesSort++;
                    }
                    if (planet.IsTerraformable)
                    {
                        ExploredTerraformablesSort++;
                    }
                    surfaceScanValueSum += planet.CartographicSurfaceScanValue;
                    vistaGenomicsValueSum += bodyViewModel.GenusesVistaGenomicsValue;
                    biologicalsCount += bodyViewModel.BiologicalsSort;
                    analysedGenusesCount += bodyViewModel.AnalysedGenusesCount;
                    if (!bodyViewModel.SurfaceScanned)
                    {
                        AllBodiesSurfaceScanned = false;
                        if (bodyViewModel.IsValuableBody)
                        {
                            AllValuableBodiesSurfaceScanned = false;
                        }
                    }
                    if (bodyViewModel.MatchingPlanetClassifications == null || !bodyViewModel.MatchingPlanetClassificationsAvailable)
                    {
                        continue;
                    }
                    matchingPlanetsCount++;
                    foreach (PlanetClassificationViewModel matchingPlanetClassification in bodyViewModel.MatchingPlanetClassifications)
                    {
                        MatchingPlanetClassificationsSort++;
                        if (!MatchingPlanetClassifications.TryAdd(matchingPlanetClassification.Name, 1) && MatchingPlanetClassifications.ContainsKey(matchingPlanetClassification.Name))
                        {
                            MatchingPlanetClassifications.TryUpdate(matchingPlanetClassification.Name, MatchingPlanetClassifications[matchingPlanetClassification.Name] + 1, MatchingPlanetClassifications[matchingPlanetClassification.Name]);
                        }
                    }
                }
            }
            else
            {
                ExploredNonBodies++;
            }
        }
        HasExploredStars = ExploredStars > 0;
        HasExploredPlanets = ExploredPlanets > 0;
        ExploredBodies = ExploredStars + ExploredPlanets;
        ExploredLandablesCount = ExploredLandablesSort.ToString();
        HasExploredLandables = ExploredLandablesSort > 0;
        ExploredTerraformablesCount = ExploredTerraformablesSort.ToString();
        HasExploredTerraformables = ExploredTerraformablesSort > 0;
        ValuableBodiesCount = ValuableBodiesSort.ToString();
        HasValuableBodies = ValuableBodiesSort > 0;
        MatchingPlanetClassificationsCount = MatchingPlanetClassificationsSort.ToString();
        HasMatchingPlanetClassifications = MatchingPlanetClassificationsSort > 0;
        PlanetsWithMatchingPlanetClassificationsCount = matchingPlanetsCount.ToString();
        if (!EdsmTotalBodyCount.HasValue)
        {
            if (EdsmExploredBodyCount == 0)
            {
                if (!_starSystem.WasReadFromEdsm)
                {
                    edsmExplorationStatus = StarSystemExplorationStatus.Unknown;
                }
                else
                {
                    edsmExplorationStatus = StarSystemExplorationStatus.Unexplored;
                }
            }
            else
            {
                edsmExplorationStatus = StarSystemExplorationStatus.Unscanned;
            }
        }
        else if (EdsmTotalBodyCount > EdsmExploredBodyCount)
        {
            edsmExplorationStatus = StarSystemExplorationStatus.Incomplete;
        }
        else if (EdsmTotalBodyCount != 0)
        {
            edsmExplorationStatus = StarSystemExplorationStatus.Complete;
        }
        if (!_starSystem.WasReadFromJournal)
        {
            journalExplorationStatus = StarSystemExplorationStatus.Unknown;
        }
        else if (TotalBodyCount == 0 && ExploredBodyCount == 0)
        {
            journalExplorationStatus = StarSystemExplorationStatus.Unexplored;
        }
        else if (AllBodiesFound || TotalBodyCount == ExploredBodyCount)
        {
            journalExplorationStatus = StarSystemExplorationStatus.Complete;
        }
        else if (TotalBodyCount < ExploredBodyCount)
        {
            journalExplorationStatus = StarSystemExplorationStatus.Unscanned;
        }
        else if (TotalBodyCount > ExploredBodyCount)
        {
            journalExplorationStatus = StarSystemExplorationStatus.Incomplete;
        }
        if (TotalBodies == 0 && ExploredBodies == 0)
        {
            if (journalExplorationStatus == StarSystemExplorationStatus.Unknown && edsmExplorationStatus == StarSystemExplorationStatus.Unknown)
            {
                explorationStatus = StarSystemExplorationStatus.Unknown;
                ExplorationStatusBodiesInfo = Resources.StatusNoData;
                IsExplorationStatusUnknown = true;
            }
            else if (journalExplorationStatus == StarSystemExplorationStatus.Unexplored || edsmExplorationStatus == StarSystemExplorationStatus.Unexplored)
            {
                explorationStatus = StarSystemExplorationStatus.Unexplored;
                ExplorationStatusBodiesInfo = Resources.StatusNoData;
                IsExplorationStatusUnexplored = true;
            }
        }
        else if (ExploredBodies > TotalBodies)
        {
            explorationStatus = StarSystemExplorationStatus.Unscanned;
            ExplorationStatusBodiesInfo = string.Format(Resources.StatusScanMissing, $"{ExploredBodies} {((ExploredBodies == 1) ? Resources.UnitBodySingular : Resources.UnitBodyPlural)}");
            IsExplorationStatusUnscanned = true;
        }
        else if (TotalBodies > ExploredBodies)
        {
            explorationStatus = StarSystemExplorationStatus.Incomplete;
            ExplorationStatusBodiesInfo = string.Format(Resources.StatusBodiesMissing, $"{TotalBodies - ExploredBodies} {((TotalBodies == 1) ? Resources.UnitBodySingular : Resources.UnitBodyPlural)}");
            IsExplorationStatusIncomplete = true;
        }
        else
        {
            explorationStatus = StarSystemExplorationStatus.Complete;
            ExplorationStatusBodiesInfo = ((TotalBodies == 1) ? Resources.StatusSingleBody : $"{TotalBodies} {Resources.UnitBodyPlural}");
            IsExplorationStatusComplete = true;
        }
        IsExplorationStatusAheadOfJournal = explorationStatus > journalExplorationStatus;
        IsActive = _starSystem.WasRequestedFromEdsm || TotalBodyCount > 0 || ExploredBodies > 0;
        int cartographicBonusValueSum = cartographicMaxValueSum - cartographicBaseValueSum - surfaceScanValueSum;
        BodiesCartographicMaxValueSort = cartographicMaxValueSum;
        HasBodiesCartographicMaxValue = cartographicMaxValueSum > 0;
        BodiesCartographicMaxValue = (HasBodiesCartographicMaxValue ? (cartographicMaxValueSum.ToString("n0") + " " + Resources.UnitCredits) : string.Empty);
        HasBodiesCartographicValue = cartographicValueSum > 0;
        BodiesCartographicValue = (HasBodiesCartographicValue ? (cartographicValueSum.ToString("n0") + " " + Resources.UnitCredits) : string.Empty);
        BodiesCartographicMaxValueNotReached = BodiesCartographicValue != BodiesCartographicMaxValue;
        HasBodiesCartographicBaseValue = cartographicBaseValueSum > 0;
        BodiesCartographicBaseValue = (HasBodiesCartographicBaseValue ? (cartographicBaseValueSum.ToString("n0") + " " + Resources.UnitCredits) : string.Empty);
        HasBodiesCartographicSurfaceScanValue = surfaceScanValueSum > 0;
        BodiesCartographicSurfaceScanValue = (HasBodiesCartographicSurfaceScanValue ? (surfaceScanValueSum.ToString("n0") + " " + Resources.UnitCredits) : string.Empty);
        HasBodiesCartographicBonusValue = cartographicBonusValueSum > 0;
        BodiesCartographicBonusValue = (HasBodiesCartographicBonusValue ? (cartographicBonusValueSum.ToString("n0") + " " + Resources.UnitCredits) : string.Empty);
        WasCompletelySurfaceScanned = WasCompletelyReadFromJournal && HasBodiesCartographicSurfaceScanValue && AllBodiesSurfaceScanned;
        HasBodiesBiologicalsCount = biologicalsCount > 0;
        BodiesVistaGenomicsValue = (HasBodiesBiologicalsCount ? (vistaGenomicsValueSum.ToString("n0") + " " + Resources.UnitCredits) : string.Empty);
        ExplorationValueSort = cartographicValueSum + vistaGenomicsValueSum;
        ExplorationValue = ((ExplorationValueSort > 0) ? (ExplorationValueSort.ToString("n0") + " " + Resources.UnitCredits) : string.Empty);
        HasExplorationValue = ExplorationValueSort > 0;
        BodiesBiologicalsCount = biologicalsCount.ToString("n0");
        BodiesAnalysedGenusesCount = analysedGenusesCount.ToString("n0");
        BodiesAllGenusesAnalysed = BodiesBiologicalsCount == BodiesAnalysedGenusesCount;
        double cartographicProgressPercent = (HasBodiesCartographicMaxValue ? ((double)cartographicValueSum / (double)cartographicMaxValueSum * 100.0 * (double)ExploredBodies / (double)TotalBodies) : 0.0);
        double vistaGenomicsProgressPercent = ((biologicalsCount > 0) ? ((double)analysedGenusesCount / (double)biologicalsCount * 100.0) : (-1.0));
        double overallProgressPercent = ((vistaGenomicsProgressPercent == -1.0) ? cartographicProgressPercent : ((cartographicProgressPercent + vistaGenomicsProgressPercent) / 2.0));
        BodiesCartographicProgress = $"{cartographicProgressPercent:n1} " + Resources.UnitPercent + "";
        BodiesVistaGenomicsProgress = ((vistaGenomicsProgressPercent > -1.0) ? $"{vistaGenomicsProgressPercent:n1} " + Resources.UnitPercent + "" : string.Empty);
        HasOverallProgress = overallProgressPercent > 0.0;
        OverallProgress = (HasOverallProgress ? $"{overallProgressPercent:n1} " + Resources.UnitPercent + "" : string.Empty);
        OverallProgressCompleted = cartographicValueSum == cartographicMaxValueSum && analysedGenusesCount == biologicalsCount;
    }
}
