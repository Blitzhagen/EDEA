using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace EDEA.Avalonia.Converters;

/// <summary>
/// Converts a boolean value to its inverse.
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    /// <summary>
    /// The default instance of the converter.
    /// </summary>
    public static readonly InverseBoolConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return !b;
        }

        return value ?? false;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return !b;
        }

        return value ?? false;
    }
}
