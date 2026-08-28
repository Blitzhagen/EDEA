using System;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Enums;

namespace EDEA.Models;

public class HotkeyItem : ObservableObject
{
    private readonly UserSettingsHotkeys _hotkeys;
    private readonly string _propertyName;
    private readonly KeyConverter _keyConverter = new();

    private Hotkey? Hotkey => _hotkeys.GetType().GetProperty(_propertyName)?.GetValue(_hotkeys) as Hotkey;

    public HotkeyId Id => Hotkey?.Id ?? default;
    public string Description { get; }

    public string FullKey
    {
        get
        {
            var h = Hotkey;
            if (h is null || !h.IsValid)
                return "<Unassigned>";
            return h.Modifier.ToString().Replace(", ", "+") + "+" + _keyConverter.ConvertToString(h.Key);
        }
    }

    public bool IsValid => Hotkey?.IsValid ?? false;

    public HotkeyItem(UserSettingsHotkeys hotkeys, string description, string propertyName)
    {
        _hotkeys = hotkeys;
        Description = description;
        _propertyName = propertyName;
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(FullKey));
        OnPropertyChanged(nameof(IsValid));
    }
}
