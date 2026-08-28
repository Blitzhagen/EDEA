using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Enums;

namespace EDEA.Models;

public partial class UserSettingsHotkeys : ObservableObject
{
    [ObservableProperty]
    private Hotkey _toggleHudWindow = null!;

    [ObservableProperty]
    private Hotkey _toggleHudMousePassThrough = null!;

    [ObservableProperty]
    private Hotkey _openRouteTab = null!;

    [ObservableProperty]
    private Hotkey _openBodiesTab = null!;

    [ObservableProperty]
    private Hotkey _openBiologicalsTab = null!;

    [ObservableProperty]
    private Hotkey _openSurroundingsTab = null!;

    [ObservableProperty]
    private Hotkey _openHistoryTab = null!;

    [ObservableProperty]
    private Hotkey _tryCopyNextSystemToClipboard = null!;

    [ObservableProperty]
    private Hotkey _quitSpeechOutput = null!;

    public UserSettingsHotkeys()
    {
        SetDefaultValues();
    }

    protected virtual Dictionary<string, object> GetDefaultValues()
    {
        return new Dictionary<string, object>
        {
            { "ToggleHudWindow", new Hotkey(HotkeyId.ToggleHudWindow, ModifierKeys.Alt | ModifierKeys.Control, Key.H) },
            { "ToggleHudMousePassThrough", new Hotkey(HotkeyId.ToggleHudMousePassThrough, ModifierKeys.Alt | ModifierKeys.Control, Key.M) },
            { "OpenRouteTab", new Hotkey(HotkeyId.OpenRouteTab, ModifierKeys.None, Key.None) },
            { "OpenBodiesTab", new Hotkey(HotkeyId.OpenBodiesTab, ModifierKeys.None, Key.None) },
            { "OpenBiologicalsTab", new Hotkey(HotkeyId.OpenBiologicalsTab, ModifierKeys.None, Key.None) },
            { "OpenSurroundingsTab", new Hotkey(HotkeyId.OpenSurroundingsTab, ModifierKeys.None, Key.None) },
            { "OpenHistoryTab", new Hotkey(HotkeyId.OpenHistoryTab, ModifierKeys.None, Key.None) },
            { "TryCopyNextSystemToClipboard", new Hotkey(HotkeyId.TryCopyNextSystemToClipboard, ModifierKeys.None, Key.None) },
            { "QuitSpeechOutput", new Hotkey(HotkeyId.QuitSpeechOutput, ModifierKeys.None, Key.None) }
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
