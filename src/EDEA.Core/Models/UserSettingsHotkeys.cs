using System.Collections.Generic;
using System.Reflection;
using EDEA.Core.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Enums;

namespace EDEA.Models;

/// <summary>
/// Represents the hotkey user settings.
/// </summary>
public partial class UserSettingsHotkeys : ObservableObject
{
    /// <summary>
    /// The hotkey to toggle the HUD window.
    /// </summary>
    [ObservableProperty]
    private Hotkey _toggleHudWindow = null!;

    /// <summary>
    /// The hotkey to toggle HUD mouse pass-through.
    /// </summary>
    [ObservableProperty]
    private Hotkey _toggleHudMousePassThrough = null!;

    /// <summary>
    /// The hotkey to open the route tab.
    /// </summary>
    [ObservableProperty]
    private Hotkey _openRouteTab = null!;

    /// <summary>
    /// The hotkey to open the bodies tab.
    /// </summary>
    [ObservableProperty]
    private Hotkey _openBodiesTab = null!;

    /// <summary>
    /// The hotkey to open the biologicals tab.
    /// </summary>
    [ObservableProperty]
    private Hotkey _openBiologicalsTab = null!;

    /// <summary>
    /// The hotkey to open the surroundings tab.
    /// </summary>
    [ObservableProperty]
    private Hotkey _openSurroundingsTab = null!;

    /// <summary>
    /// The hotkey to open the history tab.
    /// </summary>
    [ObservableProperty]
    private Hotkey _openHistoryTab = null!;

    /// <summary>
    /// The hotkey to copy the next system to the clipboard.
    /// </summary>
    [ObservableProperty]
    private Hotkey _tryCopyNextSystemToClipboard = null!;

    /// <summary>
    /// The hotkey to quit speech output.
    /// </summary>
    [ObservableProperty]
    private Hotkey _quitSpeechOutput = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsHotkeys"/> class.
    /// </summary>
    public UserSettingsHotkeys()
    {
        SetDefaultValues();
    }

    /// <summary>
    /// Returns the default values for the hotkey settings.
    /// </summary>
    /// <returns>A dictionary of property names and default values.</returns>
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
