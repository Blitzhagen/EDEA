using System.Collections.Generic;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Enums;

namespace EDEA.Models;

// 1:1-Port des Original-UserSettingsSpansh, aber ohne UserSettings-Basisklasse
// (da Spansh-Plotter-Parameter nicht direkt in der zentralen UserSettings-Gruppe landen sollen).

/// <summary>
/// Represents the user-configurable settings for the Spansh route plotter.
/// </summary>
public partial class SpanshSettings : ObservableObject
{
    /// <summary>
    /// The routing algorithm.
    /// </summary>
    [ObservableProperty]
    private SpanshRoutingAlgorithm _routingAlgorithm;

    /// <summary>
    /// Whether to use neutron star supercharging.
    /// </summary>
    [ObservableProperty]
    private bool _useSupercharge;

    /// <summary>
    /// Whether to use injections.
    /// </summary>
    [ObservableProperty]
    private bool _useInjections;

    /// <summary>
    /// Whether to exclude secondary stars.
    /// </summary>
    [ObservableProperty]
    private bool _excludeSecondary;

    /// <summary>
    /// Whether to refuel at every scoopable star.
    /// </summary>
    [ObservableProperty]
    private bool _refuelEveryScoopable;

    /// <summary>
    /// The route efficiency percentage.
    /// </summary>
    [ObservableProperty]
    private int _efficiency;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpanshSettings"/> class.
    /// </summary>
    public SpanshSettings()
    {
        SetDefaultValues();
    }

    /// <summary>
    /// Returns the default values for all settings.
    /// </summary>
    /// <returns>A dictionary of property names and default values.</returns>
    protected virtual Dictionary<string, object> GetDefaultValues()
    {
        return new Dictionary<string, object>
        {
            { "RoutingAlgorithm", SpanshRoutingAlgorithm.Optimistic },
            { "UseSupercharge", true },
            { "UseInjections", false },
            { "ExcludeSecondary", false },
            { "RefuelEveryScoopable", true },
            { "Efficiency", 60 }
        };
    }

    /// <summary>
    /// Sets the properties to their default values.
    /// </summary>
    /// <param name="singlePropertyInfo">The property to reset, or <see langword="null"/> to reset all properties.</param>
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
