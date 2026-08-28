using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;

namespace EDEA.Helpers;

/// <summary>
/// Helper class for applying color theme settings to the application resources.
/// </summary>
public static class ColorThemeHelper
{
    /// <summary>
    /// Applies all currently configured colors to the application resources.
    /// </summary>
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

    /// <summary>
    /// Applies the specified color to all matching application resource keys.
    /// </summary>
    /// <param name="propertyName">The name of the color property to apply.</param>
    /// <param name="color">The color to apply.</param>
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

    /// <summary>
    /// Updates a single application resource brush with the specified color.
    /// </summary>
    /// <param name="key">The resource key of the brush to update.</param>
    /// <param name="color">The color to set on the brush.</param>
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

    /// <summary>
    /// Stores the mapping between application resource keys and color property names.
    /// </summary>
    private static Dictionary<object, string> _colorKeyMapping = null!;

    /// <summary>
    /// Builds and returns the mapping between resource keys and color property names.
    /// </summary>
    /// <returns>A dictionary with resource keys and color property names.</returns>
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
