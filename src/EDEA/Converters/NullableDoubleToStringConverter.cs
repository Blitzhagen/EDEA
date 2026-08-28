using System;
using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

public class NullableDoubleToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!((double?)value).HasValue)
        {
            return string.Empty;
        }

        return ((double)value).ToString(CultureInfo.CurrentCulture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!string.IsNullOrEmpty((string)value) && double.TryParse((string)value, out double result))
        {
            return result;
        }

        return null!;
    }
}
