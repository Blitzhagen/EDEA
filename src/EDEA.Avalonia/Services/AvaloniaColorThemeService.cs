using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Media;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IColorThemeService"/>.
/// </summary>
public sealed class AvaloniaColorThemeService : IColorThemeService
{
    /// <summary>
    /// Mapping between <see cref="EDEA.Models.UserSettingsColors"/> property names and application resource keys.
    /// </summary>
    private static readonly Dictionary<string, string[]> ColorKeyMapping = new()
    {
        { "MainColor", new[] { "MainColor", "JournalIconColor", "TouchdownIconColor", "NewIconColor", "CurrentEntityIconColor", "CompleteIconColor", "SurfaceScannedIconColor", "BiologicalsCompleteIconColor", "CloseIconColor", "ResizeIconColor" } },
        { "InactiveMainColor", new[] { "InactiveMainColor" } },
        { "HighlightColor", new[] { "HighlightColor", "HighlightGreen" } },
        { "HighlightColor2", new[] { "HighlightGreen" } },
        { "HighlightColor3", new[] { "HighlightRed" } },
        { "HighlightColor4", new[] { "HighlightBlue" } },
        { "HighlightColor5", new[] { "HighlightYellow" } },
        { "NeutralColor", new[] { "HighlightGrey" } },
        { "InactiveNeutralColor", new[] { "InactiveHighlightGrey" } },
        { "BackgroundColor", new[] { "MainBackgroundColor", "MainLineColor", "HeaderIconColor" } },
        { "CellColor", new[] { "MainCellColor" } },
        { "CellHighlightColor", new[] { "CellHighlightColor" } },
        { "TitleBarColor", new[] { "TitleBarColor" } },
        { "NeutralCellColor", new[] { "InverseCellColor" } },
        { "NeutralCellHighlightColor", new[] { "InverseCellHighlightColor" } },
        { "IconEdsmColor", new[] { "EdsmIconColor" } },
        { "IconValuableColor", new[] { "ValuableIconColor" } },
        { "IconGeologicalsColor", new[] { "GeologicalsIconColor" } },
        { "IconBiologicalsColor", new[] { "BiologicalsIconColor" } },
        { "IconTerraformableColor", new[] { "TerraformableIconColor" } },
        { "IconLandableColor", new[] { "LandableIconColor" } },
        { "IconScoopableColor", new[] { "ScoopableIconColor" } },
        { "IconInactiveScoopableColor", new[] { "InactiveScoopableIconColor" } },
        { "IconPlanetOfInterestColor", new[] { "PlanetOfInterestIconColor" } },
        { "IconPopulatedColor", new[] { "PopulatedIconColor" } },
        { "IconRingsColor", new[] { "RingsIconColor" } },
        { "ExplorationUnexploredColor", new[] { "UnexploredIconColor" } },
        { "ExplorationUnknownColor", new[] { "UnknownIconColor" } },
        { "ExplorationInactiveUnknownColor", new[] { "InactiveUnknownIconColor" } },
        { "ExplorationUnscannedColor", new[] { "UnscannedIconColor" } },
        { "ExplorationIncompleteColor", new[] { "IncompleteIconColor" } },
        { "InClonalColonyRangeColor", new[] { "InClonalColonyRangeColor" } },
        { "OutOfClonalColonyRangeColor", new[] { "OutOfClonalColonyRangeColor" } },
    };

    /// <summary>
    /// Applies all currently configured colors to the application resources.
    /// </summary>
    public void ApplyCurrentColors()
    {
        var colors = EDEA.Preferences.Colors;
        foreach (var property in colors.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.PropertyType != typeof(EDEA.Core.Drawing.Color))
            {
                continue;
            }

            var value = (EDEA.Core.Drawing.Color)property.GetValue(colors)!;
            ApplyColor(property.Name, value);
        }
    }

    /// <summary>
    /// Applies the specified color to all matching application resource keys.
    /// </summary>
    /// <param name="propertyName">The name of the color property to apply.</param>
    /// <param name="color">The color to apply.</param>
    public void ApplyColor(string propertyName, EDEA.Core.Drawing.Color color)
    {
        if (!ColorKeyMapping.TryGetValue(propertyName, out var keys))
        {
            return;
        }

        var application = Application.Current;
        if (application == null)
        {
            return;
        }

        var mediaColor = global::Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);

        foreach (var key in keys)
        {
            if (application.Resources.TryGetResource(key, null, out var resource) && resource is SolidColorBrush brush)
            {
                brush.Color = mediaColor;
            }
            else if (application.Resources.ContainsKey(key))
            {
                application.Resources[key] = new SolidColorBrush(mediaColor);
            }
        }
    }

    /// <summary>
    /// Applies the current display size to the application resources.
    /// </summary>
    public void ApplyCurrentDisplaySize()
    {
        var application = Application.Current;
        if (application == null)
        {
            return;
        }

        int displaySize = EDEA.Preferences.Other.DisplaySize;
        int displaySizeOffset = displaySize - 2;

        application.Resources["MainFontSize"] = 12.0 + displaySizeOffset;
        application.Resources["HeaderFontSize"] = 14.0 + displaySizeOffset;
        application.Resources["SmallFontSize"] = 10.0 + displaySizeOffset;
        application.Resources["HistoryFontSize"] = 20.0 + displaySizeOffset;
        application.Resources["HistoryTotalValueFontSize"] = 30.0 + displaySizeOffset;
        application.Resources["MainIconSize"] = 15.0 + displaySizeOffset;
        application.Resources["MediumIconSize"] = 12.0 + displaySizeOffset;
        application.Resources["SmallIconSize"] = 7.0 + displaySizeOffset;

        var headerFontSize = 14.0 + displaySizeOffset;
        application.Resources["TabStripWidth"] = headerFontSize + 16.0;

        var borderTop = 21.0 + displaySizeOffset;
        if (application.Resources.TryGetResource("TabViewBorderSizes", null, out var border1) && border1 is Thickness borderSizes)
        {
            application.Resources["TabViewBorderSizes"] = new Thickness(borderSizes.Left, borderTop, borderSizes.Right, borderSizes.Bottom);
        }
        else
        {
            application.Resources["TabViewBorderSizes"] = new Thickness(6, borderTop, 0, 0);
        }

        if (application.Resources.TryGetResource("TabViewInverseBorderSizes", null, out var border2) && border2 is Thickness inverseBorderSizes)
        {
            application.Resources["TabViewInverseBorderSizes"] = new Thickness(inverseBorderSizes.Left, borderTop, inverseBorderSizes.Right, inverseBorderSizes.Bottom);
        }
        else
        {
            application.Resources["TabViewInverseBorderSizes"] = new Thickness(0, borderTop, 0, 0);
        }
    }
}
