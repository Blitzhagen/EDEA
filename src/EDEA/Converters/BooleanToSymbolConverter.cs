using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

/// <summary>
/// Converts a boolean value to a symbol string, optionally inverting the result.
/// </summary>
public class BooleanToSymbolConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a symbol string.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">A string separated by a pipe, where the first part is the symbol and the optional second part <c>"Invert"</c> inverts the boolean.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The symbol string when the value is <c>true</c>; otherwise <see cref="string.Empty"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var s = (parameter as string) ?? "✓";
        var parts = s.Split('|');
        var symbol = parts[0];
        var invert = parts.Length > 1 && parts[1].Equals("Invert", StringComparison.OrdinalIgnoreCase);
        var b = value is true;
        if (invert)
            b = !b;
        return b ? symbol : string.Empty;
    }

    /// <summary>
    /// Converts a symbol string back to a boolean value. Not supported.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>Not supported; throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Always thrown because the conversion is not supported.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
