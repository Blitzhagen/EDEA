using System.Collections.Generic;

namespace EDEA.ViewModels;

public class RouteView
{
    // Existing UI-facing properties
    public int Jump { get; set; }
    public string SystemName { get; set; } = string.Empty;
    public string StarClass { get; set; } = string.Empty;
    public string DiscoveryStatus { get; set; } = string.Empty;
    public string CartographicValue { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string EdsmDiscoverer { get; set; } = string.Empty;
    public string Distance { get; set; } = string.Empty;

    // StarPos coordinates for distance calculation
    public double? X { get; set; }
    public double? Y { get; set; }
    public double? Z { get; set; }

    // Ported properties matching the original NavRouteTableView templates
    public string Name => SystemName;
    public string JumpDistance => Jump > 0 ? Jump.ToString() : string.Empty;
    public string JumpDistanceLy => Distance;
    public string JumpDistanceLySort => Distance;

    public string BodiesCartographicMaxValue => CartographicValue;
    public string BodiesCartographicMaxValueSort => CartographicValue;
    public bool BodiesCartographicMaxValueNotReached => false;
    public bool HasBodiesCartographicValue => !string.IsNullOrEmpty(CartographicValue);

    public string OverallProgress => Progress;
    public string ExplorationValueSort => Progress;
    public bool OverallProgressCompleted => false;
    public bool HasOverallProgress => !string.IsNullOrEmpty(Progress);

    public string EdsmDiscoveryCommander => EdsmDiscoverer;

    public string ExplorationStatus => DiscoveryStatus;
    public string ExplorationStatusBodiesInfo { get; set; } = string.Empty;
    public bool IsExplorationStatusKnown => false;
    public bool IsExplorationStatusComplete => false;
    public bool IsExplorationStatusUnexplored => false;
    public bool IsExplorationStatusUnscanned => false;
    public bool IsExplorationStatusIncomplete => false;
    public bool IsExplorationStatusUnknown => false;
    public int ExploredBodies { get; set; }
    public int TotalBodies { get; set; }
    public bool HasNoTotalBodies => TotalBodies == 0;
    public bool HasTotalBodies => TotalBodies > 0;

    public bool IsActive { get; set; } = true;
    public bool WasReadFromEdsmOnly { get; set; }
    public bool IsCurrentSystemInRoute { get; set; }
    public bool IsJumpDestinationSystemInRoute { get; set; }
    public bool PrimaryStarIsScoopable { get; set; }

    public bool HasMatchingPlanetClassifications { get; set; }
    public int MatchingPlanetClassificationsCount { get; set; }
    public int PlanetsWithMatchingPlanetClassificationsCount { get; set; }
    public Dictionary<string, string> MatchingPlanetClassifications { get; set; } = new();
    public string MatchingPlanetClassificationsSort { get; set; } = string.Empty;

    public bool HasValuableBodies { get; set; }
    public bool AllValuableBodiesSurfaceScanned { get; set; }
    public int ValuableBodiesCount { get; set; }
    public string ValuableBodiesSort { get; set; } = string.Empty;

    public bool HasExploredTerraformables { get; set; }
    public int ExploredTerraformablesCount { get; set; }
    public string ExploredTerraformablesSort { get; set; } = string.Empty;

    public bool HasExploredLandables { get; set; }
    public int ExploredLandablesCount { get; set; }
    public string ExploredLandablesSort { get; set; } = string.Empty;

    public bool HasPopulation { get; set; }
    public string Population { get; set; } = string.Empty;
    public string PopulationSort { get; set; } = string.Empty;
}
