using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using log4net;

namespace EDEA.Converters;

/// <summary>
/// Multi-value converter that selects a brush color based on commander name matching.
/// </summary>
public class CommanderNameToBrushColorConverter : IMultiValueConverter
{
    private static readonly ILog log = LogManager.GetLogger(typeof(CommanderNameToBrushColorConverter));

    /// <summary>
    /// Converts multiple values into a brush color for commander name display.
    /// </summary>
    /// <param name="values">The values to convert. The first value is the commander name, the second is the selected commander, and the third is a list of teammate names.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A brush resource for the commander name, or <see cref="Brushes.Gray"/> as fallback.</returns>
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

    /// <summary>
    /// Converts a brush color back to commander name values. Not supported.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetTypes">The target types of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>Not supported; throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Always thrown because the conversion is not supported.</exception>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Retrieves a brush from the application resources or returns a gray fallback.
    /// </summary>
    /// <param name="key">The resource key to look up.</param>
    /// <returns>The brush associated with the key, or <see cref="Brushes.Gray"/>.</returns>
    private static object GetResource(string key)
    {
        if (Application.Current != null && Application.Current.TryFindResource(key) is Brush brush)
            return brush;

        return Brushes.Gray;
    }
}
