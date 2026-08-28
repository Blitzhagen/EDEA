using System.Collections.Generic;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Properties;

namespace EDEA.Models;

/// <summary>
/// Represents the planets of interest user settings.
/// </summary>
public partial class UserSettingsPlanetsOfInterest : ObservableObject
{
    /// <summary>
    /// The list of planet classifications.
    /// </summary>
    [ObservableProperty]
    private List<PlanetClassification> _planetClassifications = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsPlanetsOfInterest"/> class.
    /// </summary>
    public UserSettingsPlanetsOfInterest()
    {
        SetDefaultValues();
    }

    /// <summary>
    /// Returns the default values for the planets of interest settings.
    /// </summary>
    /// <returns>A dictionary of property names and default values.</returns>
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
