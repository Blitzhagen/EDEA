using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

public class IntEqualsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() == parameter?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is true && parameter is not null && int.TryParse(parameter.ToString(), out var i))
            return i;
        return 0;
    }
}
