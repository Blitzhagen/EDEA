using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace EDEA.Avalonia.Converters;

/// <summary>
/// Converts a tab header string to a height value for a vertically rotated tab label.
/// </summary>
public class TabHeaderToHeightConverter : IValueConverter
{
    /// <summary>
    /// The approximate width per character for the tab font.
    /// </summary>
    private const double CharacterWidth = 11.0;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var header = value?.ToString() ?? string.Empty;
        return Math.Max(45.0, header.Length * CharacterWidth);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
