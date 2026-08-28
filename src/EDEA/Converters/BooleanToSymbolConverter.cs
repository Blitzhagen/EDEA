using System.Globalization;
using System.Windows.Data;

namespace EDEA.Converters;

public class BooleanToSymbolConverter : IValueConverter
{
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

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
