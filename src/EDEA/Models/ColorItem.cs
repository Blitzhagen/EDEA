using System.Reflection;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Models;

public partial class ColorItem : ObservableObject
{
    private readonly PropertyInfo _property;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private Color _color;

    public ColorItem(string name, PropertyInfo property, Color color)
    {
        _name = name;
        _property = property;
        _color = color;
    }

    partial void OnColorChanged(Color value)
    {
        _property.SetValue(global::EDEA.Preferences.Colors, value);
    }
}
