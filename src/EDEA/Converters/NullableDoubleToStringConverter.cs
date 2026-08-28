using System;
using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

/// <summary>
/// Converts a nullable double value to and from a string.
/// </summary>
public class NullableDoubleToStringConverter : IValueConverter
{
    /// <summary>
    /// Converts a nullable double value to a string using the current culture.
    /// </summary>
    /// <param name="value">The nullable double value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The string representation of the value, or <see cref="string.Empty"/> when the value is <c>null</c>.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!((double?)value).HasValue)
        {
            return string.Empty;
        }

        return ((double)value).ToString(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Converts a string back to a nullable double value.
    /// </summary>
    /// <param name="value">The string to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The parsed double value, or <c>null</c> when the string is empty or invalid.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!string.IsNullOrEmpty((string)value) && double.TryParse((string)value, out double result))
        {
            return result;
        }

        return null!;
    }
}
