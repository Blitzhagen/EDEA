using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

/// <summary>
/// Converts an integer value to and from a boolean equality comparison.
/// </summary>
public class IntEqualsConverter : IValueConverter
{
    /// <summary>
    /// Converts a value to a boolean by comparing its string representation to the parameter.
    /// </summary>
    /// <param name="value">The integer value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The value to compare against.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns><c>true</c> if the string representations are equal; otherwise, <c>false</c>.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() == parameter?.ToString();
    }

    /// <summary>
    /// Converts a boolean back to an integer when the value is <c>true</c>.
    /// </summary>
    /// <param name="value">The boolean value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The integer string to parse.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The parsed integer value when selected; otherwise <c>0</c>.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is true && parameter is not null && int.TryParse(parameter.ToString(), out var i))
            return i;
        return 0;
    }
}
