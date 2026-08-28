using System.Collections.Generic;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Enums;

namespace EDEA.Models;

// 1:1-Port des Original-UserSettingsSpansh, aber ohne UserSettings-Basisklasse
// (da Spansh-Plotter-Parameter nicht direkt in der zentralen UserSettings-Gruppe landen sollen).
public partial class SpanshSettings : ObservableObject
{
    [ObservableProperty]
    private SpanshRoutingAlgorithm _routingAlgorithm;

    [ObservableProperty]
    private bool _useSupercharge;

    [ObservableProperty]
    private bool _useInjections;

    [ObservableProperty]
    private bool _excludeSecondary;

    [ObservableProperty]
    private bool _refuelEveryScoopable;

    [ObservableProperty]
    private int _efficiency;

    public SpanshSettings()
    {
        SetDefaultValues();
    }

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
