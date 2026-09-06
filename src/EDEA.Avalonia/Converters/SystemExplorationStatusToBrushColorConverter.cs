using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using EDEA.Enums;
using EDEA.ViewModels;

namespace EDEA.Avalonia.Converters;

/// <summary>
/// Converts a system or route exploration status into a brush color.
/// </summary>
public sealed class SystemExplorationStatusToBrushColorConverter : IValueConverter
{
    /// <summary>
    /// The default instance of the converter.
    /// </summary>
    public static readonly SystemExplorationStatusToBrushColorConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is StarSystemViewModel starSystemViewModel)
        {
            if (!starSystemViewModel.IsActive)
            {
                return GetBrush("InactiveUnknownIconColor");
            }

            if (starSystemViewModel.IsExplorationStatusAheadOfJournal)
            {
                return GetBrush("UnknownIconColor");
            }

            return starSystemViewModel.ExplorationStatusEnum switch
            {
                StarSystemExplorationStatus.Unexplored => GetBrush("UnexploredIconColor"),
                StarSystemExplorationStatus.Complete => GetBrush("MainColor"),
                StarSystemExplorationStatus.Incomplete => GetBrush("IncompleteIconColor"),
                StarSystemExplorationStatus.Unscanned => GetBrush("UnscannedIconColor"),
                _ => GetBrush("UnknownIconColor")
            };
        }

        if (value is StarSystemExplorationStatus status)
        {
            return status switch
            {
                StarSystemExplorationStatus.Unexplored => GetBrush("UnexploredIconColor"),
                StarSystemExplorationStatus.Complete => GetBrush("MainColor"),
                StarSystemExplorationStatus.Incomplete => GetBrush("IncompleteIconColor"),
                StarSystemExplorationStatus.Unscanned => GetBrush("UnscannedIconColor"),
                _ => GetBrush("UnknownIconColor")
            };
        }

        return Brushes.Transparent;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static IBrush GetBrush(string key)
    {
        if (Application.Current?.Resources.TryGetResource(key, Application.Current.ActualThemeVariant, out var resource) == true && resource is IBrush brush)
        {
            return brush;
        }

        return Brushes.Gray;
    }
}
