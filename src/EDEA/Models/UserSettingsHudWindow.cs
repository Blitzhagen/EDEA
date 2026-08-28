using System.Collections.Generic;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Models;

public partial class UserSettingsHudWindow : ObservableObject
{
    [ObservableProperty]
    private bool _bodiesColumnTypeVisible = false;

    [ObservableProperty]
    private bool _bodiesColumnTemperatureVisible = false;

    [ObservableProperty]
    private bool _bodiesColumnAtmosphereVisible = false;

    [ObservableProperty]
    private bool _bodiesColumnCartographicValueVisible = false;

    [ObservableProperty]
    private bool _bodiesColumnEdsmDiscovererVisible = false;

    [ObservableProperty]
    private bool _bodiesColumnDistanceVisible = true;

    [ObservableProperty]
    private bool _routeColumnCartographicValueVisible = false;

    [ObservableProperty]
    private bool _routeColumnEdsmDiscovererVisible = false;

    [ObservableProperty]
    private bool _routeColumnStarClassVisible = true;

    [ObservableProperty]
    private bool _routeColumnExplorationStatusIconVisible = true;

    [ObservableProperty]
    private bool _routeColumnValuableIconVisible = true;

    [ObservableProperty]
    private bool _routeColumnLandableIconVisible = true;

    [ObservableProperty]
    private bool _routeColumnTerraformableIconVisible = false;

    [ObservableProperty]
    private bool _bodiesColumnSurfaceScannedIconVisible = true;

    [ObservableProperty]
    private bool _bodiesColumnValuableIconVisible = true;

    [ObservableProperty]
    private bool _bodiesColumnGeologicalsIconVisible = true;

    [ObservableProperty]
    private bool _bodiesColumnBiologicalsIconVisible = true;

    [ObservableProperty]
    private bool _bodiesColumnTerraformableIconVisible = true;

    [ObservableProperty]
    private bool _bodiesColumnLandableIconVisible = true;

    [ObservableProperty]
    private bool _bodiesColumnNewIconVisible = true;

    [ObservableProperty]
    private bool _bodiesColumnRingsIconVisible = true;

    [ObservableProperty]
    private bool _routeColumnSystemNameVisible = true;

    [ObservableProperty]
    private double _opacity = 0.1;

    [ObservableProperty]
    private int _hudWindowTabViewModel = 0;

    [ObservableProperty]
    private bool _routeColumnDistanceVisible = false;

    [ObservableProperty]
    private bool _hideOnNoFocus = false;

    [ObservableProperty]
    private bool _hideOnInternalPanel = false;

    [ObservableProperty]
    private bool _hideOnExternalPanel = false;

    [ObservableProperty]
    private bool _hideOnCommsPanel = false;

    [ObservableProperty]
    private bool _hideOnRolePanel = false;

    [ObservableProperty]
    private bool _hideOnStationServices = false;

    [ObservableProperty]
    private bool _hideOnGalaxyMap = false;

    [ObservableProperty]
    private bool _hideOnSystemMap = false;

    [ObservableProperty]
    private bool _hideOnOrrery = false;

    [ObservableProperty]
    private bool _hideOnFSSmode = false;

    [ObservableProperty]
    private bool _hideOnSAAmode = false;

    [ObservableProperty]
    private bool _hideOnCodex = false;

    [ObservableProperty]
    private bool _hideOnOnFoot = false;

    [ObservableProperty]
    private bool _genusColumnSpeciesVisible = true;

    [ObservableProperty]
    private bool _genusColumnVariantVisible = true;

    [ObservableProperty]
    private bool _genusColumnAnalysisCompleteIconVisible = true;

    [ObservableProperty]
    private bool _genusColumnScansVisible = true;

    [ObservableProperty]
    private bool _genusColumnClonalColonyRangeVisible = true;

    [ObservableProperty]
    private bool _genusColumnVistaGenomicsValueVisible = true;

    [ObservableProperty]
    private bool _bodiesColumnPlanetOfInterestIconVisible = true;

    [ObservableProperty]
    private bool _routeColumnProgressVisible = true;

    [ObservableProperty]
    private bool _genusColumnNewIconVisible = true;

    [ObservableProperty]
    private bool _genusColumnValuableIconVisible = true;

    [ObservableProperty]
    private bool _routeColumnPlanetOfInterestIconVisible = true;

    [ObservableProperty]
    private bool _routeColumnPopulatedIconVisible = true;

    public UserSettingsHudWindow()
    {
        SetDefaultValues();
    }

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
