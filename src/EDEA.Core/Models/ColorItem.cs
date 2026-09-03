using System.Reflection;
using EDEA.Core.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Models;

/// <summary>
/// Represents a color setting item bound to an application preference property.
/// </summary>
public partial class ColorItem : ObservableObject
{
    /// <summary>
    /// The preference property this color is bound to.
    /// </summary>
    private readonly PropertyInfo _property;

    /// <summary>
    /// The color name.
    /// </summary>
    [ObservableProperty]
    private string _name;

    /// <summary>
    /// The color value.
    /// </summary>
    [ObservableProperty]
    private Color _color;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorItem"/> class.
    /// </summary>
    /// <param name="name">The name of the color.</param>
    /// <param name="property">The preference property to bind to.</param>
    /// <param name="color">The color value.</param>
    public ColorItem(string name, PropertyInfo property, Color color)
    {
        _name = name;
        _property = property;
        _color = color;
    }

    /// <summary>
    /// Called when the color changes, updating the bound preference property.
    /// </summary>
    /// <param name="value">The new color value.</param>
    partial void OnColorChanged(Color value)
    {
        _property.SetValue(global::EDEA.Preferences.Colors, value);
    }
}
