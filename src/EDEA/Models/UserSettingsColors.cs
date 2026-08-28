using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Models;

public partial class UserSettingsColors : ObservableObject
{
    [ObservableProperty]
    private Color _mainColor;

    [ObservableProperty]
    private Color _inactiveMainColor;

    [ObservableProperty]
    private Color _highlightColor;

    [ObservableProperty]
    private Color _backgroundColor;

    [ObservableProperty]
    private Color _cellColor;

    [ObservableProperty]
    private Color _cellHighlightColor;

    [ObservableProperty]
    private Color _titleBarColor;

    [ObservableProperty]
    private Color _iconEdsmColor;

    [ObservableProperty]
    private Color _iconValuableColor;

    [ObservableProperty]
    private Color _iconGeologicalsColor;

    [ObservableProperty]
    private Color _iconBiologicalsColor;

    [ObservableProperty]
    private Color _iconTerraformableColor;

    [ObservableProperty]
    private Color _iconLandableColor;

    [ObservableProperty]
    private Color _iconScoopableColor;

    [ObservableProperty]
    private Color _iconInactiveScoopableColor;

    [ObservableProperty]
    private Color _iconPlanetOfInterestColor;

    [ObservableProperty]
    private Color _iconPopulatedColor;

    [ObservableProperty]
    private Color _iconRingsColor;

    [ObservableProperty]
    private Color _explorationUnexploredColor;

    [ObservableProperty]
    private Color _explorationUnknownColor;

    [ObservableProperty]
    private Color _explorationInactiveUnknownColor;

    [ObservableProperty]
    private Color _explorationUnscannedColor;

    [ObservableProperty]
    private Color _explorationIncompleteColor;

    [ObservableProperty]
    private Color _highlightColor2;

    [ObservableProperty]
    private Color _highlightColor3;

    [ObservableProperty]
    private Color _highlightColor4;

    [ObservableProperty]
    private Color _highlightColor5;

    [ObservableProperty]
    private Color _neutralColor;

    [ObservableProperty]
    private Color _inactiveNeutralColor;

    [ObservableProperty]
    private Color _neutralCellColor;

    [ObservableProperty]
    private Color _neutralCellHighlightColor;

    [ObservableProperty]
    private Color _inClonalColonyRangeColor;

    [ObservableProperty]
    private Color _outOfClonalColonyRangeColor;

    public UserSettingsColors()
    {
        SetDefaultValues();
    }

    private static Color C(string hex) => (Color)ColorConverter.ConvertFromString(hex)!;

    protected virtual Dictionary<string, object> GetDefaultValues()
    {
        return new Dictionary<string, object>
        {
            { "MainColor", C("#FFFF6F00") },
            { "InactiveMainColor", C("#FF7A3500") },
            { "HighlightColor", C("#FFA1DFE0") },
            { "BackgroundColor", C("#FF090000") },
            { "CellColor", C("#FF271406") },
            { "CellHighlightColor", C("#FF411601") },
            { "TitleBarColor", C("#FF271406") },
            { "IconEdsmColor", C("#FF8C8B8B") },
            { "IconValuableColor", C("#FFDABE54") },
            { "IconGeologicalsColor", C("#FFFF7C7C") },
            { "IconBiologicalsColor", C("#FF88CF7F") },
            { "IconTerraformableColor", C("#FF2491DF") },
            { "IconLandableColor", C("#FFA1DFE0") },
            { "IconScoopableColor", C("#FF88CF7F") },
            { "IconInactiveScoopableColor", C("#FF274023") },
            { "IconPlanetOfInterestColor", C("#FFC6478C") },
            { "IconPopulatedColor", C("#FFBCA79D") },
            { "IconRingsColor", C("#FFFFD3B2") },
            { "ExplorationUnexploredColor", C("#FFFF7C7C") },
            { "ExplorationUnknownColor", C("#FF8C8B8B") },
            { "ExplorationInactiveUnknownColor", C("#FF363230") },
            { "ExplorationUnscannedColor", C("#FF2491DF") },
            { "ExplorationIncompleteColor", C("#FFDABE54") },
            { "HighlightColor2", C("#FF88CF7F") },
            { "HighlightColor3", C("#FFFF7C7C") },
            { "HighlightColor4", C("#FF2491DF") },
            { "HighlightColor5", C("#FFDABE54") },
            { "NeutralColor", C("#FF8C8B8B") },
            { "InactiveNeutralColor", C("#FF363230") },
            { "NeutralCellColor", C("#FF161413") },
            { "NeutralCellHighlightColor", C("#FF2B2826") },
            { "InClonalColonyRangeColor", C("#FFFF7C7C") },
            { "OutOfClonalColonyRangeColor", C("#FF88CF7F") }
        };
    }

    public virtual void SetDefaultValues(PropertyInfo? singlePropertyInfo = null)
    {
        var defaultValues = GetDefaultValues();
        if (singlePropertyInfo is null)
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (defaultValues.TryGetValue(propertyInfo.Name, out var defaultValue))
                {
                    propertyInfo.SetValue(this, defaultValue);
                }
            }
        }
        else if (defaultValues.TryGetValue(singlePropertyInfo.Name, out var defaultValue))
        {
            singlePropertyInfo.SetValue(this, defaultValue);
        }
    }
}
