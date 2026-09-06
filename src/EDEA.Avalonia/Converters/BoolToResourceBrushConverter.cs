using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace EDEA.Avalonia.Converters;

/// <summary>
/// Converts a nullable boolean value to a brush looked up from the application resources.
/// The <see cref="IValueConverter.Convert"/> parameter must be one or two resource keys separated by a semicolon.
/// If the value is null and a third key is provided, that brush is returned.
/// </summary>
public class BoolToResourceBrushConverter : IValueConverter
{
    /// <summary>
    /// The default instance of the converter.
    /// </summary>
    public static readonly BoolToResourceBrushConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var keys = parameter as string;
        var parts = keys?.Split(';') ?? Array.Empty<string>();

        var key = value switch
        {
            null => parts.Length > 2 ? parts[2] : string.Empty,
            true => parts.Length > 0 ? parts[0] : string.Empty,
            false => parts.Length > 1 ? parts[1] : string.Empty,
            _ => string.Empty,
        };

        if (string.IsNullOrEmpty(key))
        {
            return Brushes.Transparent;
        }

        if (string.Equals(key, "Transparent", StringComparison.OrdinalIgnoreCase))
        {
            return Brushes.Transparent;
        }

        if (Application.Current?.Resources.TryGetResource(key, Application.Current.ActualThemeVariant, out var resource) == true && resource is IBrush brush)
        {
            return brush;
        }

        return Brushes.Transparent;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
