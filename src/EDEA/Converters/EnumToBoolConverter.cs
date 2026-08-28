using System;
using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

/// <summary>
/// Converts an enumeration value to and from a boolean based on a parameter string.
/// </summary>
[ValueConversion(typeof(Enum), typeof(bool))]
public class EnumToBoolConverter : IValueConverter
{
    /// <summary>
    /// Converts an enumeration value to a boolean indicating whether it matches the parameter value.
    /// </summary>
    /// <param name="value">The enumeration value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The string representation of the expected enumeration value.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns><c>true</c> if the value matches the parameter; otherwise, <c>false</c>.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return false;
        }

        string? valueText = value.ToString();
        string parameterText = parameter.ToString()!;
        return valueText!.Equals(parameterText, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Converts a boolean back to the corresponding enumeration value.
    /// </summary>
    /// <param name="value">The boolean value to convert back.</param>
    /// <param name="targetType">The target enumeration type.</param>
    /// <param name="parameter">The string representation of the enumeration value.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The parsed enumeration value when selected; otherwise <see cref="Binding.DoNothing"/>.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return null!;
        }

        bool isSelected = (bool)value;
        string parameterText = parameter.ToString()!;
        if (isSelected)
        {
            return Enum.Parse(targetType, parameterText);
        }

        return Binding.DoNothing;
    }
}
