using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

/// <summary>
/// Inverts a boolean value during conversion.
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    /// <summary>
    /// Inverts a boolean value.
    /// </summary>
    /// <param name="value">The boolean value to invert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The inverted boolean value, or the original value when it is not a boolean.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : value;
    }

    /// <summary>
    /// Inverts a boolean value back.
    /// </summary>
    /// <param name="value">The boolean value to invert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The inverted boolean value, or the original value when it is not a boolean.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : value;
    }
}
