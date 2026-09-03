using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace EDEA.Avalonia.Converters;

/// <summary>
/// Converts a visibility string ("Visible", "Collapsed") to a boolean value.
/// </summary>
public class TabVisibilityConverter : IValueConverter
{
    /// <summary>
    /// The default instance of the converter.
    /// </summary>
    public static readonly TabVisibilityConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            return s.Equals("Visible", StringComparison.OrdinalIgnoreCase);
        }

        return value is true;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? "Visible" : "Collapsed";
        }

        return "Collapsed";
    }
}
