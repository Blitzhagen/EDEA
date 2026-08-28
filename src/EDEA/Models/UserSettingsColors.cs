using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Models;

/// <summary>
/// Represents the color-related user settings.
/// </summary>
public partial class UserSettingsColors : ObservableObject
{
    /// <summary>
    /// The main color.
    /// </summary>
    [ObservableProperty]
    private Color _mainColor;

    /// <summary>
    /// The inactive main color.
    /// </summary>
    [ObservableProperty]
    private Color _inactiveMainColor;

    /// <summary>
    /// The highlight color.
    /// </summary>
    [ObservableProperty]
    private Color _highlightColor;

    /// <summary>
    /// The background color.
    /// </summary>
    [ObservableProperty]
    private Color _backgroundColor;

    /// <summary>
    /// The cell color.
    /// </summary>
    [ObservableProperty]
    private Color _cellColor;

    /// <summary>
    /// The cell highlight color.
    /// </summary>
    [ObservableProperty]
    private Color _cellHighlightColor;

    /// <summary>
    /// The title bar color.
    /// </summary>
    [ObservableProperty]
    private Color _titleBarColor;

    /// <summary>
    /// The EDSM icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconEdsmColor;

    /// <summary>
    /// The valuable icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconValuableColor;

    /// <summary>
    /// The geologicals icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconGeologicalsColor;

    /// <summary>
    /// The biologicals icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconBiologicalsColor;

    /// <summary>
    /// The terraformable icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconTerraformableColor;

    /// <summary>
    /// The landable icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconLandableColor;

    /// <summary>
    /// The scoopable icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconScoopableColor;

    /// <summary>
    /// The inactive scoopable icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconInactiveScoopableColor;

    /// <summary>
    /// The planet of interest icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconPlanetOfInterestColor;

    /// <summary>
    /// The populated icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconPopulatedColor;

    /// <summary>
    /// The rings icon color.
    /// </summary>
    [ObservableProperty]
    private Color _iconRingsColor;

    /// <summary>
    /// The unexplored color.
    /// </summary>
    [ObservableProperty]
    private Color _explorationUnexploredColor;

    /// <summary>
    /// The unknown color.
    /// </summary>
    [ObservableProperty]
    private Color _explorationUnknownColor;

    /// <summary>
    /// The inactive unknown color.
    /// </summary>
    [ObservableProperty]
    private Color _explorationInactiveUnknownColor;

    /// <summary>
    /// The unscanned color.
    /// </summary>
    [ObservableProperty]
    private Color _explorationUnscannedColor;

    /// <summary>
    /// The incomplete color.
    /// </summary>
    [ObservableProperty]
    private Color _explorationIncompleteColor;

    /// <summary>
    /// The second highlight color.
    /// </summary>
    [ObservableProperty]
    private Color _highlightColor2;

    /// <summary>
    /// The third highlight color.
    /// </summary>
    [ObservableProperty]
    private Color _highlightColor3;

    /// <summary>
    /// The fourth highlight color.
    /// </summary>
    [ObservableProperty]
    private Color _highlightColor4;

    /// <summary>
    /// The fifth highlight color.
    /// </summary>
    [ObservableProperty]
    private Color _highlightColor5;

    /// <summary>
    /// The neutral color.
    /// </summary>
    [ObservableProperty]
    private Color _neutralColor;

    /// <summary>
    /// The inactive neutral color.
    /// </summary>
    [ObservableProperty]
    private Color _inactiveNeutralColor;

    /// <summary>
    /// The neutral cell color.
    /// </summary>
    [ObservableProperty]
    private Color _neutralCellColor;

    /// <summary>
    /// The neutral cell highlight color.
    /// </summary>
    [ObservableProperty]
    private Color _neutralCellHighlightColor;

    /// <summary>
    /// The in clonal colony range color.
    /// </summary>
    [ObservableProperty]
    private Color _inClonalColonyRangeColor;

    /// <summary>
    /// The out of clonal colony range color.
    /// </summary>
    [ObservableProperty]
    private Color _outOfClonalColonyRangeColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsColors"/> class.
    /// </summary>
    public UserSettingsColors()
    {
        SetDefaultValues();
    }

    /// <summary>
    /// Converts a hex string to a <see cref="Color"/>.
    /// </summary>
    /// <param name="hex">The hexadecimal color string.</param>
    /// <returns>The converted color.</returns>
    private static Color C(string hex) => (Color)ColorConverter.ConvertFromString(hex)!;

    /// <summary>
    /// Returns the default values for the color settings.
    /// </summary>
    /// <returns>A dictionary of property names and default values.</returns>
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

    /// <summary>
    /// Sets all or a single setting to its default value.
    /// </summary>
    /// <param name="singlePropertyInfo">The property to reset, or <see langword="null"/> to reset all.</param>
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
