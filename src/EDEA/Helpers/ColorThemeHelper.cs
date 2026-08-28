using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;

namespace EDEA.Helpers;

public static class ColorThemeHelper
{
    public static void ApplyCurrentColors()
    {
        foreach (var pair in GetColorKeyMapping())
        {
            PropertyInfo? property = Preferences.Colors.GetType().GetProperty(pair.Value);
            if (property == null)
                continue;

            Color? color = property.GetValue(Preferences.Colors) as Color?;
            if (!color.HasValue)
                continue;

            ApplyBrushColor(pair.Key, color.Value);
        }
    }

    public static void ApplyColor(string propertyName, Color color)
    {
        foreach (var pair in GetColorKeyMapping())
        {
            if (pair.Value == propertyName)
            {
                ApplyBrushColor(pair.Key, color);
            }
        }
    }

    private static void ApplyBrushColor(object key, Color color)
    {
        try
        {
            Application.Current.Resources[key] = new SolidColorBrush(color);
        }
        catch (Exception)
        {
            // ignore failed resource update
        }
    }

    private static Dictionary<object, string> _colorKeyMapping = null!;

    private static Dictionary<object, string> GetColorKeyMapping()
    {
        if (_colorKeyMapping != null)
            return _colorKeyMapping;

        _colorKeyMapping = new Dictionary<object, string>();

        foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
        {
            foreach (object? key in dictionary.Keys)
            {
                if (key == null || dictionary[key] is not SolidColorBrush brush)
                    continue;

                Binding? binding = BindingOperations.GetBinding(brush, SolidColorBrush.ColorProperty);
                if (!string.IsNullOrEmpty(binding?.Path?.Path))
                {
                    _colorKeyMapping[key] = binding!.Path!.Path!;
                }
            }
        }

        return _colorKeyMapping;
    }
}
