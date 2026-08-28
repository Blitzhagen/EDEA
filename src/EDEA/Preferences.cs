using EDEA.Models;
using EDEA.Services;

namespace EDEA;

public static class Preferences
{
    public static UserSettings User { get; set; } = new();

    public static UserSettingsColors Colors => User.Colors;
    public static UserSettingsOther Other => User.Other;
    public static UserSettingsApplication Application => User.Application;
    public static UserSettingsHotkeys Hotkeys => User.Hotkeys;
    public static UserSettingsHudWindow HudWindow => User.HudWindow;
    public static UserSettingsPlanetsOfInterest PlanetsOfInterest => User.PlanetsOfInterest;
    public static SpanshSettings Spansh => User.Spansh;
    public static UserSettingsSpeech Speech => User.Speech;

    private static SettingsProvider? _settingsProvider;

    public static void SetSettingsProvider(SettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    public static void SaveUserSettings()
    {
        _settingsProvider?.Save();
    }

    public static void ResetUserSettings()
    {
        User.SetDefaultValues();
        _settingsProvider?.Save();
    }

    public static void ReloadUserSettings()
    {
        _settingsProvider?.Reload();
    }
}
