using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using log4net;

namespace EDEA.Converters;

public class CommanderNameToBrushColorConverter : IMultiValueConverter
{
    private static readonly ILog log = LogManager.GetLogger(typeof(CommanderNameToBrushColorConverter));

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (values[0]?.GetType() == typeof(string))
            {
                string commanderName = (string)values[0];

                if (values[1]?.GetType() == typeof(string) && ((string)values[1]).Equals(commanderName))
                {
                    return GetResource("MainColor");
                }

                if (values[2]?.GetType() == typeof(List<string>) && ((List<string>)values[2]).Contains(commanderName))
                {
                    return GetResource("HighlightGreen");
                }
            }
        }
        catch (Exception exception)
        {
            log.Warn($"Could not set color for commander name {(values[1])} or teammate names {(string.Join(", ", values[2]))}", exception);
        }

        return GetResource("HighlightGrey");
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static object GetResource(string key)
    {
        if (Application.Current != null && Application.Current.TryFindResource(key) is Brush brush)
            return brush;

        return Brushes.Gray;
    }
}
