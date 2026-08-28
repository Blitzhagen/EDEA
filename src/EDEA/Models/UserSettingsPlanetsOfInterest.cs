using System.Collections.Generic;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Properties;

namespace EDEA.Models;

public partial class UserSettingsPlanetsOfInterest : ObservableObject
{
    [ObservableProperty]
    private List<PlanetClassification> _planetClassifications = new();

    public UserSettingsPlanetsOfInterest()
    {
        SetDefaultValues();
    }

    protected virtual Dictionary<string, object> GetDefaultValues()
    {
        return new Dictionary<string, object>
        {
            {
                "PlanetClassifications",
                new List<PlanetClassification>
                {
                    new PlanetClassification(Resources.PlanetOfInterest_NearbyHMC)
                    {
                        DistanceMax = 300.0,
                        PlanetClasses = new List<string> { "High metal content world" }
                    },
                    new PlanetClassification(Resources.PlanetOfInterest_HighGravityLandable, true)
                    {
                        IsActive = true,
                        Landable = true,
                        GravityMin = 10.0
                    },
                    new PlanetClassification(Resources.PlanetOfInterest_GuardianRuins)
                    {
                        Landable = true,
                        TemperatureMin = 200.0,
                        TemperatureMax = 306.0,
                        PlanetClasses = new List<string> { "High metal content world", "Rocky body" },
                        Atmospheres = new List<string> { "No atmosphere" },
                        RadiusMin = 1000000.0,
                        GravityMax = 0.42
                    }
                }
            }
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
