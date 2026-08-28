using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using EDEA.Enums;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Converters;

/// <summary>
/// Converts a system or route exploration status into a brush color.
/// </summary>
public class SystemExplorationStatusToBrushColorConverter : IValueConverter
{
    private static readonly ILog log = LogManager.GetLogger(typeof(SystemExplorationStatusToBrushColorConverter));

    /// <summary>
    /// Converts a star system view model or route view to a brush color based on its exploration status.
    /// </summary>
    /// <param name="value">The <see cref="StarSystemViewModel"/> or <see cref="RouteView"/> to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A brush resource representing the exploration status, or the unknown icon color as fallback.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (value is StarSystemViewModel starSystemViewModel)
            {
                if (!starSystemViewModel.IsActive)
                {
                    return GetResource("InactiveUnknownIconColor");
                }

                if (starSystemViewModel.IsExplorationStatusAheadOfJournal)
                {
                    return GetResource("UnknownIconColor");
                }

                return starSystemViewModel.ExplorationStatusEnum switch
                {
                    StarSystemExplorationStatus.Unexplored => GetResource("UnexploredIconColor"),
                    StarSystemExplorationStatus.Complete => GetResource("MainColor"),
                    StarSystemExplorationStatus.Incomplete => GetResource("IncompleteIconColor"),
                    StarSystemExplorationStatus.Unscanned => GetResource("UnscannedIconColor"),
                    _ => GetResource("UnknownIconColor")
                };
            }

            if (value is RouteView routeView)
            {
                if (!routeView.IsActive)
                {
                    return GetResource("InactiveUnknownIconColor");
                }

                return routeView.ExplorationStatus switch
                {
                    "Unexplored" => GetResource("UnexploredIconColor"),
                    "Complete" => GetResource("MainColor"),
                    "Incomplete" => GetResource("IncompleteIconColor"),
                    "Unscanned" => GetResource("UnscannedIconColor"),
                    _ => GetResource("UnknownIconColor")
                };
            }
        }
        catch (Exception exception)
        {
            log.Warn($"Could not set exploration status color for system {(value)}", exception);
        }

        return GetResource("UnknownIconColor");
    }

    /// <summary>
    /// Converts a brush color back to a system or route view. Not supported.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>Not supported; throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Always thrown because the conversion is not supported.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
        if (Application.Current == null)
            return Brushes.Gray;

        try
        {
            var resource = Application.Current.FindResource(key);
            if (resource is Brush brush)
                return brush;
        }
        catch (Exception exception)
        {
            log.Warn($"Could not find resource {key}", exception);
        }

        return Brushes.Gray;
    }
}
