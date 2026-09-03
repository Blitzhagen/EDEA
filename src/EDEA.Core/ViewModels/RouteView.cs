using System.Collections.Generic;

namespace EDEA.ViewModels;

/// <summary>
/// Simple data transfer object for route jump information used in the UI.
/// </summary>
public class RouteView
{
    /// <summary>
    /// Gets or sets the jump number.
    /// </summary>
    /// <value>The jump number.</value>
    public int Jump { get; set; }

    /// <summary>
    /// Gets or sets the name of the star system.
    /// </summary>
    /// <value>The system name.</value>
    public string SystemName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the star class of the primary star.
    /// </summary>
    /// <value>The star class.</value>
    public string StarClass { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the discovery status.
    /// </summary>
    /// <value>The discovery status.</value>
    public string DiscoveryStatus { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cartographic value.
    /// </summary>
    /// <value>The cartographic value.</value>
    public string CartographicValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the exploration progress.
    /// </summary>
    /// <value>The progress.</value>
    public string Progress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the EDSM discoverer.
    /// </summary>
    /// <value>The EDSM discoverer.</value>
    public string EdsmDiscoverer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the distance to the next system.
    /// </summary>
    /// <value>The distance.</value>
    public string Distance { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the X coordinate of the system.
    /// </summary>
    /// <value>The X coordinate.</value>
    public double? X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of the system.
    /// </summary>
    /// <value>The Y coordinate.</value>
    public double? Y { get; set; }

    /// <summary>
    /// Gets or sets the Z coordinate of the system.
    /// </summary>
    /// <value>The Z coordinate.</value>
    public double? Z { get; set; }

    /// <summary>
    /// Gets the system name.
    /// </summary>
    /// <value>The system name.</value>
    public string Name => SystemName;

    /// <summary>
    /// Gets the jump number for display.
    /// </summary>
    /// <value>The jump number string.</value>
    public string JumpDistance => Jump > 0 ? Jump.ToString() : string.Empty;

    /// <summary>
    /// Gets the jump distance in light years.
    /// </summary>
    /// <value>The jump distance string.</value>
    public string JumpDistanceLy => Distance;

    /// <summary>
    /// Gets the jump distance in light years for sorting.
    /// </summary>
    /// <value>The jump distance string.</value>
    public string JumpDistanceLySort => Distance;

    /// <summary>
    /// Gets the bodies cartographic maximum value.
    /// </summary>
    /// <value>The cartographic value.</value>
    public string BodiesCartographicMaxValue => CartographicValue;

    /// <summary>
    /// Gets the bodies cartographic maximum value for sorting.
    /// </summary>
    /// <value>The cartographic value for sorting.</value>
    public string BodiesCartographicMaxValueSort => CartographicValue;

    /// <summary>
    /// Gets a value indicating whether the cartographic maximum value is not reached.
    /// </summary>
    /// <value>Always <c>false</c>.</value>
    public bool BodiesCartographicMaxValueNotReached => false;

    /// <summary>
    /// Gets a value indicating whether the bodies have a cartographic value.
    /// </summary>
    /// <value><c>true</c> if a cartographic value is present; otherwise, <c>false</c>.</value>
    public bool HasBodiesCartographicValue => !string.IsNullOrEmpty(CartographicValue);

    /// <summary>
    /// Gets the overall exploration progress.
    /// </summary>
    /// <value>The progress string.</value>
    public string OverallProgress => Progress;

    /// <summary>
    /// Gets the exploration value for sorting.
    /// </summary>
    /// <value>The progress string.</value>
    public string ExplorationValueSort => Progress;

    /// <summary>
    /// Gets a value indicating whether the overall progress is completed.
    /// </summary>
    /// <value>Always <c>false</c>.</value>
    public bool OverallProgressCompleted => false;

    /// <summary>
    /// Gets a value indicating whether there is any overall progress.
    /// </summary>
    /// <value><c>true</c> if progress is present; otherwise, <c>false</c>.</value>
    public bool HasOverallProgress => !string.IsNullOrEmpty(Progress);

    /// <summary>
    /// Gets the EDSM discovery commander.
    /// </summary>
    /// <value>The EDSM discoverer.</value>
    public string EdsmDiscoveryCommander => EdsmDiscoverer;

    /// <summary>
    /// Gets the exploration status.
    /// </summary>
    /// <value>The exploration status string.</value>
    public string ExplorationStatus => DiscoveryStatus;

    /// <summary>
    /// Gets or sets the exploration status bodies info.
    /// </summary>
    /// <value>The bodies info string.</value>
    public string ExplorationStatusBodiesInfo { get; set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the exploration status is known.
    /// </summary>
    /// <value>Always <c>false</c>.</value>
    public bool IsExplorationStatusKnown => false;

    /// <summary>
    /// Gets a value indicating whether the exploration status is complete.
    /// </summary>
    /// <value>Always <c>false</c>.</value>
    public bool IsExplorationStatusComplete => false;

    /// <summary>
    /// Gets a value indicating whether the exploration status is unexplored.
    /// </summary>
    /// <value>Always <c>false</c>.</value>
    public bool IsExplorationStatusUnexplored => false;

    /// <summary>
    /// Gets a value indicating whether the exploration status is incomplete.
    /// </summary>
    /// <value>Always <c>false</c>.</value>
    public bool IsExplorationStatusIncomplete => false;

    /// <summary>
    /// Gets a value indicating whether the exploration status is unscanned.
    /// </summary>
    /// <value>Always <c>false</c>.</value>
    public bool IsExplorationStatusUnscanned => false;

    /// <summary>
    /// Gets a value indicating whether the exploration status is unknown.
    /// </summary>
    /// <value>Always <c>false</c>.</value>
    public bool IsExplorationStatusUnknown => false;

    /// <summary>
    /// Gets or sets the number of explored bodies.
    /// </summary>
    /// <value>The explored body count.</value>
    public int ExploredBodies { get; set; }

    /// <summary>
    /// Gets or sets the total number of bodies.
    /// </summary>
    /// <value>The total body count.</value>
    public int TotalBodies { get; set; }

    /// <summary>
    /// Gets a value indicating whether there are no total bodies.
    /// </summary>
    /// <value><c>true</c> if there are no total bodies; otherwise, <c>false</c>.</value>
    public bool HasNoTotalBodies => TotalBodies == 0;

    /// <summary>
    /// Gets a value indicating whether there are total bodies.
    /// </summary>
    /// <value><c>true</c> if there are total bodies; otherwise, <c>false</c>.</value>
    public bool HasTotalBodies => TotalBodies > 0;

    /// <summary>
    /// Gets or sets a value indicating whether this jump is active.
    /// </summary>
    /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the entry was read from EDSM only.
    /// </summary>
    /// <value><c>true</c> if read from EDSM only; otherwise, <c>false</c>.</value>
    public bool WasReadFromEdsmOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the current system in the route.
    /// </summary>
    /// <value><c>true</c> if this is the current system; otherwise, <c>false</c>.</value>
    public bool IsCurrentSystemInRoute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the jump destination system in the route.
    /// </summary>
    /// <value><c>true</c> if this is the destination; otherwise, <c>false</c>.</value>
    public bool IsJumpDestinationSystemInRoute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the primary star is scoopable.
    /// </summary>
    /// <value><c>true</c> if the primary star is scoopable; otherwise, <c>false</c>.</value>
    public bool PrimaryStarIsScoopable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether there are matching planet classifications.
    /// </summary>
    /// <value><c>true</c> if matching planet classifications exist; otherwise, <c>false</c>.</value>
    public bool HasMatchingPlanetClassifications { get; set; }

    /// <summary>
    /// Gets or sets the count of matching planet classifications.
    /// </summary>
    /// <value>The matching planet classifications count.</value>
    public int MatchingPlanetClassificationsCount { get; set; }

    /// <summary>
    /// Gets or sets the count of planets with matching planet classifications.
    /// </summary>
    /// <value>The planets with matching classifications count.</value>
    public int PlanetsWithMatchingPlanetClassificationsCount { get; set; }

    /// <summary>
    /// Gets or sets the matching planet classifications dictionary.
    /// </summary>
    /// <value>The matching planet classifications.</value>
    public Dictionary<string, string> MatchingPlanetClassifications { get; set; } = new();

    /// <summary>
    /// Gets or sets the matching planet classifications for sorting.
    /// </summary>
    /// <value>The matching planet classifications sort string.</value>
    public string MatchingPlanetClassificationsSort { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether there are valuable bodies.
    /// </summary>
    /// <value><c>true</c> if valuable bodies exist; otherwise, <c>false</c>.</value>
    public bool HasValuableBodies { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether all valuable bodies are surface scanned.
    /// </summary>
    /// <value><c>true</c> if all valuable bodies are surface scanned; otherwise, <c>false</c>.</value>
    public bool AllValuableBodiesSurfaceScanned { get; set; }

    /// <summary>
    /// Gets or sets the count of valuable bodies.
    /// </summary>
    /// <value>The valuable body count.</value>
    public int ValuableBodiesCount { get; set; }

    /// <summary>
    /// Gets or sets the valuable bodies for sorting.
    /// </summary>
    /// <value>The valuable bodies sort string.</value>
    public string ValuableBodiesSort { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether there are explored terraformables.
    /// </summary>
    /// <value><c>true</c> if explored terraformables exist; otherwise, <c>false</c>.</value>
    public bool HasExploredTerraformables { get; set; }

    /// <summary>
    /// Gets or sets the count of explored terraformables.
    /// </summary>
    /// <value>The explored terraformable count.</value>
    public int ExploredTerraformablesCount { get; set; }

    /// <summary>
    /// Gets or sets the explored terraformables for sorting.
    /// </summary>
    /// <value>The explored terraformables sort string.</value>
    public string ExploredTerraformablesSort { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether there are explored landables.
    /// </summary>
    /// <value><c>true</c> if explored landables exist; otherwise, <c>false</c>.</value>
    public bool HasExploredLandables { get; set; }

    /// <summary>
    /// Gets or sets the count of explored landables.
    /// </summary>
    /// <value>The explored landable count.</value>
    public int ExploredLandablesCount { get; set; }

    /// <summary>
    /// Gets or sets the explored landables for sorting.
    /// </summary>
    /// <value>The explored landables sort string.</value>
    public string ExploredLandablesSort { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the system has a population.
    /// </summary>
    /// <value><c>true</c> if the system has a population; otherwise, <c>false</c>.</value>
    public bool HasPopulation { get; set; }

    /// <summary>
    /// Gets or sets the formatted population.
    /// </summary>
    /// <value>The population string.</value>
    public string Population { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the population for sorting.
    /// </summary>
    /// <value>The population sort string.</value>
    public string PopulationSort { get; set; } = string.Empty;
}
