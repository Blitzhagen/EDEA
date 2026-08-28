using System;
using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

/// <summary>
/// Converts a null or non-null value to a boolean.
/// </summary>
public class NullToBoolConverter : IValueConverter
{
    /// <summary>
    /// Converts a value to a boolean indicating whether it is not <c>null</c>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is not <c>null</c>; otherwise, <c>false</c>.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null;
    }

    /// <summary>
    /// Converts a boolean back to a value. Not supported.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>Not supported; returns <c>null</c>.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null!;
    }
}
