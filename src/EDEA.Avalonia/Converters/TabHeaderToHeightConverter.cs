using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace EDEA.Avalonia.Converters;

/// <summary>
/// Converts a tab header string to a height value for a vertically rotated tab label.
/// </summary>
public class TabHeaderToHeightConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var header = value?.ToString() ?? string.Empty;
        double headerFontSize = 14.0;
        if (Application.Current?.Resources.TryGetResource("HeaderFontSize", null, out var fontResource) == true && fontResource is double fs)
        {
            headerFontSize = fs;
        }

        double textWidth = header.Length * headerFontSize;
        return parameter?.ToString() == "Width"
            ? Math.Max(45.0, textWidth)
            : Math.Max(45.0, textWidth + 38.0);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
