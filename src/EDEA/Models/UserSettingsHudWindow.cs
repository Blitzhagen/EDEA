using System.Collections.Generic;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Models;

/// <summary>
/// Represents the HUD window user settings.
/// </summary>
public partial class UserSettingsHudWindow : ObservableObject
{
    /// <summary>
    /// Whether the bodies type column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnTypeVisible = false;

    /// <summary>
    /// Whether the bodies temperature column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnTemperatureVisible = false;

    /// <summary>
    /// Whether the bodies atmosphere column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnAtmosphereVisible = false;

    /// <summary>
    /// Whether the bodies cartographic value column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnCartographicValueVisible = false;

    /// <summary>
    /// Whether the bodies EDSM discoverer column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnEdsmDiscovererVisible = false;

    /// <summary>
    /// Whether the bodies distance column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnDistanceVisible = true;

    /// <summary>
    /// Whether the route cartographic value column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnCartographicValueVisible = false;

    /// <summary>
    /// Whether the route EDSM discoverer column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnEdsmDiscovererVisible = false;

    /// <summary>
    /// Whether the route star class column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnStarClassVisible = true;

    /// <summary>
    /// Whether the route exploration status icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnExplorationStatusIconVisible = true;

    /// <summary>
    /// Whether the route valuable icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnValuableIconVisible = true;

    /// <summary>
    /// Whether the route landable icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnLandableIconVisible = true;

    /// <summary>
    /// Whether the route terraformable icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnTerraformableIconVisible = false;

    /// <summary>
    /// Whether the bodies surface scanned icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnSurfaceScannedIconVisible = true;

    /// <summary>
    /// Whether the bodies valuable icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnValuableIconVisible = true;

    /// <summary>
    /// Whether the bodies geologicals icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnGeologicalsIconVisible = true;

    /// <summary>
    /// Whether the bodies biologicals icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnBiologicalsIconVisible = true;

    /// <summary>
    /// Whether the bodies terraformable icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnTerraformableIconVisible = true;

    /// <summary>
    /// Whether the bodies landable icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnLandableIconVisible = true;

    /// <summary>
    /// Whether the bodies new icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnNewIconVisible = true;

    /// <summary>
    /// Whether the bodies rings icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnRingsIconVisible = true;

    /// <summary>
    /// Whether the route system name column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnSystemNameVisible = true;

    /// <summary>
    /// The HUD window opacity.
    /// </summary>
    [ObservableProperty]
    private double _opacity = 0.1;

    /// <summary>
    /// The selected HUD window tab view model.
    /// </summary>
    [ObservableProperty]
    private int _hudWindowTabViewModel = 0;

    /// <summary>
    /// Whether the route distance column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnDistanceVisible = false;

    /// <summary>
    /// Whether to hide the HUD when no focus is active.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnNoFocus = false;

    /// <summary>
    /// Whether to hide the HUD on the internal panel.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnInternalPanel = false;

    /// <summary>
    /// Whether to hide the HUD on the external panel.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnExternalPanel = false;

    /// <summary>
    /// Whether to hide the HUD on the comms panel.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnCommsPanel = false;

    /// <summary>
    /// Whether to hide the HUD on the role panel.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnRolePanel = false;

    /// <summary>
    /// Whether to hide the HUD on station services.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnStationServices = false;

    /// <summary>
    /// Whether to hide the HUD on the galaxy map.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnGalaxyMap = false;

    /// <summary>
    /// Whether to hide the HUD on the system map.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnSystemMap = false;

    /// <summary>
    /// Whether to hide the HUD on the orrery.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnOrrery = false;

    /// <summary>
    /// Whether to hide the HUD in FSS mode.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnFSSmode = false;

    /// <summary>
    /// Whether to hide the HUD in SAA mode.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnSAAmode = false;

    /// <summary>
    /// Whether to hide the HUD on the codex.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnCodex = false;

    /// <summary>
    /// Whether to hide the HUD when on foot.
    /// </summary>
    [ObservableProperty]
    private bool _hideOnOnFoot = false;

    /// <summary>
    /// Whether the genus species column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _genusColumnSpeciesVisible = true;

    /// <summary>
    /// Whether the genus variant column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _genusColumnVariantVisible = true;

    /// <summary>
    /// Whether the genus analysis complete icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _genusColumnAnalysisCompleteIconVisible = true;

    /// <summary>
    /// Whether the genus scans column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _genusColumnScansVisible = true;

    /// <summary>
    /// Whether the genus clonal colony range column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _genusColumnClonalColonyRangeVisible = true;

    /// <summary>
    /// Whether the genus Vista Genomics value column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _genusColumnVistaGenomicsValueVisible = true;

    /// <summary>
    /// Whether the bodies planet of interest icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _bodiesColumnPlanetOfInterestIconVisible = true;

    /// <summary>
    /// Whether the route progress column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnProgressVisible = true;

    /// <summary>
    /// Whether the genus new icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _genusColumnNewIconVisible = true;

    /// <summary>
    /// Whether the genus valuable icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _genusColumnValuableIconVisible = true;

    /// <summary>
    /// Whether the route planet of interest icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnPlanetOfInterestIconVisible = true;

    /// <summary>
    /// Whether the route populated icon column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _routeColumnPopulatedIconVisible = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsHudWindow"/> class.
    /// </summary>
    public UserSettingsHudWindow()
    {
        SetDefaultValues();
    }

    /// <summary>
    /// Returns the default values for the HUD window settings.
    /// </summary>
    /// <returns>A dictionary of property names and default values.</returns>
    protected virtual Dictionary<string, object> GetDefaultValues()
    {
        return new Dictionary<string, object>
        {
            { "BodiesColumnTypeVisible", false },
            { "BodiesColumnTemperatureVisible", false },
            { "BodiesColumnAtmosphereVisible", false },
            { "BodiesColumnCartographicValueVisible", false },
            { "BodiesColumnEdsmDiscovererVisible", false },
            { "BodiesColumnDistanceVisible", true },
            { "RouteColumnCartographicValueVisible", false },
            { "RouteColumnEdsmDiscovererVisible", false },
            { "RouteColumnStarClassVisible", true },
            { "RouteColumnExplorationStatusIconVisible", true },
            { "RouteColumnValuableIconVisible", true },
            { "RouteColumnLandableIconVisible", true },
            { "RouteColumnTerraformableIconVisible", false },
            { "BodiesColumnSurfaceScannedIconVisible", true },
            { "BodiesColumnValuableIconVisible", true },
            { "BodiesColumnGeologicalsIconVisible", true },
            { "BodiesColumnBiologicalsIconVisible", true },
            { "BodiesColumnTerraformableIconVisible", true },
            { "BodiesColumnLandableIconVisible", true },
            { "BodiesColumnNewIconVisible", true },
            { "BodiesColumnRingsIconVisible", true },
            { "RouteColumnSystemNameVisible", true },
            { "Opacity", 0.1 },
            { "HudWindowTabViewModel", 0 },
            { "RouteColumnDistanceVisible", false },
            { "HideOnNoFocus", false },
            { "HideOnInternalPanel", false },
            { "HideOnExternalPanel", false },
            { "HideOnCommsPanel", false },
            { "HideOnRolePanel", false },
            { "HideOnStationServices", false },
            { "HideOnGalaxyMap", false },
            { "HideOnSystemMap", false },
            { "HideOnOrrery", false },
            { "HideOnFSSmode", false },
            { "HideOnSAAmode", false },
            { "HideOnCodex", false },
            { "HideOnOnFoot", false },
            { "GenusColumnSpeciesVisible", true },
            { "GenusColumnVariantVisible", true },
            { "GenusColumnAnalysisCompleteIconVisible", true },
            { "GenusColumnScansVisible", true },
            { "GenusColumnClonalColonyRangeVisible", true },
            { "GenusColumnVistaGenomicsValueVisible", true },
            { "BodiesColumnPlanetOfInterestIconVisible", true },
            { "RouteColumnProgressVisible", true },
            { "GenusColumnNewIconVisible", true },
            { "GenusColumnValuableIconVisible", true },
            { "RouteColumnPlanetOfInterestIconVisible", true },
            { "RouteColumnPopulatedIconVisible", true }
        };
    }

    /// <summary>
    /// Sets all or a single setting to its default value.
    /// </summary>
    /// <param name="singlePropertyInfo">The property to reset, or <see langword="null"/> to reset all.</param>
    public virtual void SetDefaultValues(PropertyInfo? singlePropertyInfo = null)
    {
        var defaultValues = GetDefaultValues();
        if (singlePropertyInfo is null)
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (defaultValues.TryGetValue(propertyInfo.Name, out var defaultValue))
                {
                    propertyInfo.SetValue(this, defaultValue);
                }
            }
        }
        else if (defaultValues.TryGetValue(singlePropertyInfo.Name, out var defaultValue))
        {
            singlePropertyInfo.SetValue(this, defaultValue);
        }
    }
}
