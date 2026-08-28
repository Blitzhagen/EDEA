using System;
using System.Collections.Concurrent;
using System.Reflection;
using EDEA.Models;
using EDEA.Properties;
using log4net;

using EDEA.Enums;

namespace EDEA.ViewModels;

public class StarSystemViewModel : ViewModelBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(StarSystemViewModel));

    private readonly StarSystem _starSystem;

    private StarSystemExplorationStatus journalExplorationStatus { get; }

    private StarSystemExplorationStatus edsmExplorationStatus { get; }

    private StarSystemExplorationStatus explorationStatus { get; }

    public string Name => _starSystem.Name;

    public string StarClass { get; }

    public string PrimaryStarName => _starSystem.PrimaryStarName ?? string.Empty;

    public bool PrimaryStarIsScoopable => _starSystem.PrimaryStarIsScoopable;

    public int JumpDistance => _starSystem.JumpDistance;

    public double JumpDistanceLySort => _starSystem.JumpDistanceLy;

    public string JumpDistanceLy { get; } = string.Empty;

    public bool IsPastSystemInRoute => _starSystem.IsPastSystemInRoute;

    public bool IsJumpDestinationSystemInRoute => _starSystem.IsJumpDestinationSystemInRoute;

    public bool IsCurrentSystemInRoute => _starSystem.IsCurrentSystemInRoute;

    public bool IsSystemInRouteAhead => _starSystem.IsSystemInRouteAhead;

    public string EdsmName => _starSystem.EdsmName ?? string.Empty;

    public string EdsmPrimaryStarName => _starSystem.EdsmPrimaryStarName ?? string.Empty;

    public string EdsmPrimaryStarType => _starSystem.EdsmPrimaryStarType ?? string.Empty;

    public bool EdsmPrimaryStarIsScoopable => _starSystem.EdsmPrimaryStarIsScoopable;

    public bool WasReadFromEdsm => _starSystem.WasReadFromEdsm;

    public bool WasReadFromEdsmOnly => _starSystem.WasReadFromEdsmOnly;

    public bool WasCompletelyReadFromJournal => journalExplorationStatus == StarSystemExplorationStatus.Complete;

    public ConcurrentDictionary<string, int> MatchingPlanetClassifications { get; }

    public int MatchingPlanetClassificationsSort { get; }

    public string MatchingPlanetClassificationsCount { get; }

    public bool HasMatchingPlanetClassifications { get; }

    public string PlanetsWithMatchingPlanetClassificationsCount { get; }

    public string EdsmDiscoveryCommander { get; } = string.Empty;

    public int TotalBodyCount => _starSystem.TotalBodyCount;

    public bool HasTotalBodyCount => TotalBodyCount > 0;

    public bool HasNoTotalBodyCount => !HasTotalBodyCount;

    public int TotalNonBodyCount => _starSystem.TotalNonBodyCount;

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

    public int ExploredBodyCount { get; }

    public int? EdsmTotalBodyCount => _starSystem.EdsmTotalBodyCount;

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

    public bool HasNoEdsmTotalBodyCount => !HasEdsmTotalBodyCount;

    public int EdsmExploredBodyCount { get; set; }

    public int TotalBodies { get; }

    public bool HasTotalBodies => TotalBodies > 0;

    public bool HasNoTotalBodies => !HasTotalBodies;

    public int ExploredStars { get; }

    public bool HasExploredStars { get; }

    public int ExploredPlanets { get; }

    public bool HasExploredPlanets { get; }

    public int ExploredBodies { get; }

    public int ExploredNonBodies { get; }

    public int ExploredLandablesSort { get; }

    public string ExploredLandablesCount { get; }

    public bool HasExploredLandables { get; }

    public int ExploredTerraformablesSort { get; }

    public string ExploredTerraformablesCount { get; }

    public bool HasExploredTerraformables { get; }

    public int ValuableBodiesSort { get; }

    public string ValuableBodiesCount { get; }

    public bool HasValuableBodies { get; }

    public string JournalExplorationStatus => journalExplorationStatus switch
    {
        StarSystemExplorationStatus.Unknown => Resources.Status_Unknown,
        StarSystemExplorationStatus.Unexplored => Resources.Status_Unexplored,
        StarSystemExplorationStatus.Unscanned => Resources.Status_Unscanned,
        StarSystemExplorationStatus.Incomplete => Resources.Status_Incomplete,
        StarSystemExplorationStatus.Complete => Resources.Status_Complete,
        _ => Resources.Status_Unknown
    };

    public string EdsmExplorationStatus => edsmExplorationStatus switch
    {
        StarSystemExplorationStatus.Unknown => Resources.Status_Unknown,
        StarSystemExplorationStatus.Unexplored => Resources.Status_Unexplored,
        StarSystemExplorationStatus.Unscanned => Resources.Status_Unscanned,
        StarSystemExplorationStatus.Incomplete => Resources.Status_Incomplete,
        StarSystemExplorationStatus.Complete => Resources.Status_Complete,
        _ => Resources.Status_Unknown
    };

    public string ExplorationStatus => explorationStatus switch
    {
        StarSystemExplorationStatus.Unknown => Resources.Status_Unknown,
        StarSystemExplorationStatus.Unexplored => Resources.Status_Unexplored,
        StarSystemExplorationStatus.Unscanned => Resources.Status_Unscanned,
        StarSystemExplorationStatus.Incomplete => Resources.Status_Incomplete,
        StarSystemExplorationStatus.Complete => Resources.Status_Complete,
        _ => Resources.Status_Unknown
    };

    public StarSystemExplorationStatus ExplorationStatusEnum => explorationStatus;

    public string ExplorationStatusBodiesInfo { get; } = string.Empty;

    public bool IsExplorationStatusUnknown { get; }

    public bool IsExplorationStatusKnown => !IsExplorationStatusUnknown;

    public bool IsExplorationStatusUnexplored { get; }

    public bool IsExplorationStatusComplete { get; }

    public bool IsExplorationStatusUnscanned { get; }

    public bool IsExplorationStatusIncomplete { get; }

    public bool IsExplorationStatusAheadOfJournal { get; }

    public int BodiesCartographicMaxValueSort { get; }

    public string BodiesCartographicMaxValue { get; }

    public bool HasBodiesCartographicMaxValue { get; }

    public string BodiesCartographicValue { get; }

    public bool HasBodiesCartographicValue { get; }

    public string BodiesCartographicBaseValue { get; }

    public bool HasBodiesCartographicBaseValue { get; }

    public string BodiesCartographicSurfaceScanValue { get; }

    public bool HasBodiesCartographicSurfaceScanValue { get; }

    public bool WasCompletelySurfaceScanned { get; }

    public string BodiesCartographicBonusValue { get; }

    public bool HasBodiesCartographicBonusValue { get; }

    public string BodiesVistaGenomicsValue { get; }

    public int ExplorationValueSort { get; }

    public string ExplorationValue { get; }

    public bool HasExplorationValue { get; }

    public string BodiesCartographicProgress { get; }

    public bool BodiesCartographicMaxValueNotReached { get; }

    public string BodiesVistaGenomicsProgress { get; }

    public string BodiesBiologicalsCount { get; }

    public bool HasBodiesBiologicalsCount { get; }

    public string BodiesAnalysedGenusesCount { get; }

    public bool BodiesAllGenusesAnalysed { get; }

    public bool HasOverallProgress { get; }

    public string OverallProgress { get; }

    public bool OverallProgressCompleted { get; }

    public bool IsActive { get; }

    public bool AllBodiesSurfaceScanned { get; }

    public bool AllValuableBodiesSurfaceScanned { get; }

    public long PopulationSort { get; }

    public string Population { get; }

    public bool HasPopulation { get; }

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
