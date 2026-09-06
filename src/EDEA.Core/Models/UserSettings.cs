using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Properties;

namespace EDEA.Models;

/// <summary>
/// Represents the complete user settings for the application.
/// </summary>
public partial class UserSettings : ObservableObject
{
    /// <summary>
    /// The speech settings.
    /// </summary>
    [ObservableProperty]
    private UserSettingsSpeech _speech = new();

    /// <summary>
    /// The other settings.
    /// </summary>
    [ObservableProperty]
    private UserSettingsOther _other = new();

    /// <summary>
    /// The application settings.
    /// </summary>
    [ObservableProperty]
    private UserSettingsApplication _application = new();

    /// <summary>
    /// The color settings.
    /// </summary>
    [ObservableProperty]
    private UserSettingsColors _colors = new();

    /// <summary>
    /// The hotkey settings.
    /// </summary>
    [ObservableProperty]
    private UserSettingsHotkeys _hotkeys = new();

    /// <summary>
    /// The HUD window settings.
    /// </summary>
    [ObservableProperty]
    private UserSettingsHudWindow _hudWindow = new();

    /// <summary>
    /// The planets of interest settings.
    /// </summary>
    [ObservableProperty]
    private UserSettingsPlanetsOfInterest _planetsOfInterest = new();

    /// <summary>
    /// The Spansh settings.
    /// </summary>
    [ObservableProperty]
    private SpanshSettings _spansh = new();

    /// <summary>
    /// Sets all or a single settings property to its default value.
    /// </summary>
    /// <param name="singlePropertyInfo">The property to reset, or <see langword="null"/> to reset all.</param>
    public void SetDefaultValues(PropertyInfo? singlePropertyInfo = null)
    {
        Speech.SetDefaultValues(singlePropertyInfo);
        Other.SetDefaultValues(singlePropertyInfo);
        Application.SetDefaultValues(singlePropertyInfo);
        Colors.SetDefaultValues(singlePropertyInfo);
        Hotkeys.SetDefaultValues(singlePropertyInfo);
        HudWindow.SetDefaultValues(singlePropertyInfo);
        PlanetsOfInterest.SetDefaultValues(singlePropertyInfo);
        Spansh.SetDefaultValues(singlePropertyInfo);
    }
}

/// <summary>
/// Represents the speech-related user settings.
/// </summary>
public partial class UserSettingsSpeech : ObservableObject
{
    /// <summary>
    /// Whether the welcome speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _welcome = true;

    /// <summary>
    /// The welcome speech text.
    /// </summary>
    [ObservableProperty]
    private string _welcomeSpeech = "o7 Commander {CommanderName}, Exploration Assistant at your service!";

    /// <summary>
    /// Whether the goodbye speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _goodbye = false;

    /// <summary>
    /// The goodbye speech text.
    /// </summary>
    [ObservableProperty]
    private string _goodbyeSpeech = "See you in the black!";

    /// <summary>
    /// Whether geological signal speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _geologicalSignals = false;

    /// <summary>
    /// The geological signal speech text.
    /// </summary>
    [ObservableProperty]
    private string _geologicalSignalsSpeech = "{PlanetGeoCount} {PlanetGeoSignalNoun} found on {PlanetType} {PlanetName}.";

    /// <summary>
    /// Whether biological signal speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _biologicalSignals = true;

    /// <summary>
    /// The biological signal speech text.
    /// </summary>
    [ObservableProperty]
    private string _biologicalSignalsSpeech = "{PlanetBioCount} {PlanetBioSignalNoun} found on {PlanetType} {PlanetName}.";

    /// <summary>
    /// The speech synthesizer voice.
    /// </summary>
    [ObservableProperty]
    private string _speechSynthesizerVoice = string.Empty;

    /// <summary>
    /// The speech synthesizer rate.
    /// </summary>
    [ObservableProperty]
    private double _speechSynthesizerRate = 0;

    /// <summary>
    /// The speech synthesizer volume.
    /// </summary>
    [ObservableProperty]
    private int _speechSynthesizerVolume = 100;

    /// <summary>
    /// Whether first discovery of a system speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _firstDiscoverySystem = true;

    /// <summary>
    /// The first discovery of a system speech text.
    /// </summary>
    [ObservableProperty]
    private string _firstDiscoverySystemSpeech = "First discoverer of star system {SystemName} with primary star of type {SystemStarType}.";

    /// <summary>
    /// Whether first discovery of a body speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _firstDiscoveryBody = false;

    /// <summary>
    /// The first discovery of a body speech text.
    /// </summary>
    [ObservableProperty]
    private string _firstDiscoveryBodySpeech = "First discoverer of {BodyType} {BodyName}.";

    /// <summary>
    /// Whether terraformable planet speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _terraformable = true;

    /// <summary>
    /// The terraformable planet speech text.
    /// </summary>
    [ObservableProperty]
    private string _terraformableSpeech = "{PlanetType} {PlanetName}, is terraformable.";

    /// <summary>
    /// Whether landable planet speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _landable = false;

    /// <summary>
    /// The landable planet speech text.
    /// </summary>
    [ObservableProperty]
    private string _landableSpeech = string.Empty;

    /// <summary>
    /// Whether valuable body speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _valuableBody = true;

    /// <summary>
    /// The valuable body speech text.
    /// </summary>
    [ObservableProperty]
    private string _valuableBodySpeech = string.Empty;

    /// <summary>
    /// Whether valuable genus predicted speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _valuableGenusPredicted = false;

    /// <summary>
    /// The valuable genus predicted speech text.
    /// </summary>
    [ObservableProperty]
    private string _valuableGenusPredictedSpeech = string.Empty;

    /// <summary>
    /// Whether valuable genera predicted speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _valuableGeneraPredicted = true;

    /// <summary>
    /// The valuable genera predicted speech text.
    /// </summary>
    [ObservableProperty]
    private string _valuableGeneraPredictedSpeech = string.Empty;

    /// <summary>
    /// Whether leaving clonal colony range speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _leaveClonalColonyRange = true;

    /// <summary>
    /// The leaving clonal colony range speech text.
    /// </summary>
    [ObservableProperty]
    private string _leaveClonalColonyRangeSpeech = string.Empty;

    /// <summary>
    /// Whether entering clonal colony range speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _enterClonalColonyRange = true;

    /// <summary>
    /// The entering clonal colony range speech text.
    /// </summary>
    [ObservableProperty]
    private string _enterClonalColonyRangeSpeech = string.Empty;

    /// <summary>
    /// Whether matching classifications found speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _matchingClassificationsFound = false;

    /// <summary>
    /// The matching classifications found speech text.
    /// </summary>
    [ObservableProperty]
    private string _matchingClassificationsFoundSpeech = string.Empty;

    /// <summary>
    /// Whether matching classification found speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _matchingClassificationFound = true;

    /// <summary>
    /// The matching classification found speech text.
    /// </summary>
    [ObservableProperty]
    private string _matchingClassificationFoundSpeech = string.Empty;

    /// <summary>
    /// Whether ring speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _ring = false;

    /// <summary>
    /// The ring speech text.
    /// </summary>
    [ObservableProperty]
    private string _ringSpeech = string.Empty;

    /// <summary>
    /// Whether ring count speech is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _ringCount = false;

    /// <summary>
    /// The ring count speech text.
    /// </summary>
    [ObservableProperty]
    private string _ringCountSpeech = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsSpeech"/> class.
    /// </summary>
    public UserSettingsSpeech()
    {
        SetDefaultValues();
    }

    /// <summary>
    /// Returns the default values for the speech settings.
    /// </summary>
    /// <returns>A dictionary of property names and default values.</returns>
    protected virtual Dictionary<string, object> GetDefaultValues()
    {
        return new Dictionary<string, object>
        {
            { "Welcome", true },
            { "WelcomeSpeech", Resources.Speech_Welcome },
            { "GeologicalSignals", false },
            { "GeologicalSignalsSpeech", Resources.Speech_GeologicalSignals },
            { "BiologicalSignals", true },
            { "BiologicalSignalsSpeech", Resources.Speech_BiologicalSignals },
            { "SpeechSynthesizerVoice", string.Empty },
            { "SpeechSynthesizerRate", 0 },
            { "SpeechSynthesizerVolume", 100 },
            { "FirstDiscoverySystem", true },
            { "FirstDiscoverySystemSpeech", Resources.Speech_FirstDiscoverySystem },
            { "FirstDiscoveryBody", false },
            { "FirstDiscoveryBodySpeech", Resources.Speech_FirstDiscoveryBody },
            { "Goodbye", false },
            { "GoodbyeSpeech", Resources.Speech_Goodbye },
            { "Terraformable", true },
            { "TerraformableSpeech", Resources.Speech_Terraformable },
            { "Landable", false },
            { "LandableSpeech", Properties.Resources.Speech_Landable },
            { "ValuableBody", true },
            { "ValuableBodySpeech", Properties.Resources.Speech_ValuableBody },
            { "ValuableGenusPredicted", false },
            { "ValuableGenusPredictedSpeech", Properties.Resources.Speech_ValuableGenusPredicted },
            { "ValuableGeneraPredicted", true },
            { "ValuableGeneraPredictedSpeech", Properties.Resources.Speech_ValuableGeneraPredicted },
            { "LeaveClonalColonyRange", true },
            { "LeaveClonalColonyRangeSpeech", Properties.Resources.Speech_LeaveClonalColonyRange },
            { "EnterClonalColonyRange", true },
            { "EnterClonalColonyRangeSpeech", Properties.Resources.Speech_EnterClonalColonyRange },
            { "MatchingClassificationsFound", false },
            { "MatchingClassificationsFoundSpeech", Properties.Resources.Speech_MatchingClassificationsFound },
            { "MatchingClassificationFound", true },
            { "MatchingClassificationFoundSpeech", Properties.Resources.Speech_MatchingClassificationFound },
            { "Ring", false },
            { "RingSpeech", Properties.Resources.Speech_Ring },
            { "RingCount", false },
            { "RingCountSpeech", Properties.Resources.Speech_RingCount }
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

/// <summary>
/// Represents the miscellaneous user settings.
/// </summary>
public partial class UserSettingsOther : ObservableObject
{
    /// <summary>
    /// The Elite Dangerous saved game path.
    /// </summary>
    [ObservableProperty]
    private string _edSavedGamePath = string.Empty;

    /// <summary>
    /// Whether automatic tab switching is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _automaticTabSwitching;

    /// <summary>
    /// The display size.
    /// </summary>
    [ObservableProperty]
    private int _displaySize;

    /// <summary>
    /// The threshold above which a genus is considered valuable.
    /// </summary>
    [ObservableProperty]
    private int _valuableGenusThreshold;

    /// <summary>
    /// The threshold above which a body is considered valuable.
    /// </summary>
    [ObservableProperty]
    private int _valuableBodyThreshold;

    /// <summary>
    /// The altitude threshold for displaying biological signals.
    /// </summary>
    [ObservableProperty]
    private int _biologicalsViewAltitudeThreshold;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsOther"/> class.
    /// </summary>
    public UserSettingsOther()
    {
        SetDefaultValues();
    }

    /// <summary>
    /// Returns the default values for the other settings.
    /// </summary>
    /// <returns>A dictionary of property names and default values.</returns>
    protected virtual Dictionary<string, object> GetDefaultValues()
    {
        return new Dictionary<string, object>
        {
            { "DisplaySize", 2 },
            { "AutomaticTabSwitching", true },
            { "ValuableGenusThreshold", 8_000_000 },
            { "EdSavedGamePath", GetDefaultEdSavedGamePath() },
            { "ValuableBodyThreshold", 2_000_000 },
            { "BiologicalsViewAltitudeThreshold", 150_000 }
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

    /// <summary>
    /// Gets the default Elite Dangerous saved game path.
    /// </summary>
    /// <returns>The default saved game path, or an empty string if the directory does not exist.</returns>
    private static string GetDefaultEdSavedGamePath()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrEmpty(userProfile))
        {
            return string.Empty;
        }

        var path = Path.Combine(userProfile, "Saved Games", "Frontier Developments", "Elite Dangerous");
        if (Directory.Exists(path))
        {
            return path;
        }

        return string.Empty;
    }
}
