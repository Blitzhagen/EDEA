using System;
using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

public class NullableLongToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!((long?)value).HasValue)
        {
            return string.Empty;
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!string.IsNullOrEmpty((string)value) && long.TryParse((string)value, out long result))
        {
            return result;
        }

        return null!;
    }
}
