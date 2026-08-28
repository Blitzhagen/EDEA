using System;
using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

[ValueConversion(typeof(Enum), typeof(bool))]
public class EnumToBoolConverter : IValueConverter
{
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
