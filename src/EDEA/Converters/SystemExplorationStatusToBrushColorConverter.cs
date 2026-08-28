using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using EDEA.Enums;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Converters;

public class SystemExplorationStatusToBrushColorConverter : IValueConverter
{
    private static readonly ILog log = LogManager.GetLogger(typeof(SystemExplorationStatusToBrushColorConverter));

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

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

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
