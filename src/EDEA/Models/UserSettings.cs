using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Properties;

namespace EDEA.Models;

public partial class UserSettings : ObservableObject
{
    [ObservableProperty]
    private UserSettingsSpeech _speech = new();

    [ObservableProperty]
    private UserSettingsOther _other = new();

    [ObservableProperty]
    private UserSettingsApplication _application = new();

    [ObservableProperty]
    private UserSettingsColors _colors = new();

    [ObservableProperty]
    private UserSettingsHotkeys _hotkeys = new();

    [ObservableProperty]
    private UserSettingsHudWindow _hudWindow = new();

    [ObservableProperty]
    private UserSettingsPlanetsOfInterest _planetsOfInterest = new();

    [ObservableProperty]
    private SpanshSettings _spansh = new();

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

public partial class UserSettingsSpeech : ObservableObject
{
    [ObservableProperty]
    private bool _welcome = true;

    [ObservableProperty]
    private string _welcomeSpeech = "o7 Commander {CommanderName}, EDEA at your service!";

    [ObservableProperty]
    private bool _goodbye = false;

    [ObservableProperty]
    private string _goodbyeSpeech = "See you in the black!";

    [ObservableProperty]
    private bool _geologicalSignals = false;

    [ObservableProperty]
    private string _geologicalSignalsSpeech = "{PlanetGeoCount} geological signals found on {PlanetType} {PlanetName}.";

    [ObservableProperty]
    private bool _biologicalSignals = true;

    [ObservableProperty]
    private string _biologicalSignalsSpeech = "{PlanetBioCount} biological signals found on {PlanetType} {PlanetName}.";

    [ObservableProperty]
    private string _speechSynthesizerVoice = string.Empty;

    [ObservableProperty]
    private int _speechSynthesizerRate = 0;

    [ObservableProperty]
    private int _speechSynthesizerVolume = 100;

    [ObservableProperty]
    private bool _firstDiscoverySystem = true;

    [ObservableProperty]
    private string _firstDiscoverySystemSpeech = "First discoverer of star system {SystemName} with primary star of type {SystemStarType}.";

    [ObservableProperty]
    private bool _firstDiscoveryBody = false;

    [ObservableProperty]
    private string _firstDiscoveryBodySpeech = "First discoverer of {BodyType} {BodyName}.";

    [ObservableProperty]
    private bool _terraformable = true;

    [ObservableProperty]
    private string _terraformableSpeech = "{PlanetType} {PlanetName}, is terraformable.";

    [ObservableProperty]
    private bool _landable = false;

    [ObservableProperty]
    private string _landableSpeech = "{PlanetType} {PlanetName}, is landable at a distance of {PlanetDistance} Lightseconds with {PlanetAtmosphere} atmosphere, a temperature of {PlanetTemperature} Kelvin, a gravity of {PlanetGravity} g, {PlanetVolcanism} volcanism, a radius of {PlanetRadius} kilometer, {PlanetGeoCount} geological and {PlanetBioCount} biological signals.";

    [ObservableProperty]
    private bool _valuableBody = true;

    [ObservableProperty]
    private string _valuableBodySpeech = "{BodyType} {BodyName} has a potential exploration value of {BodyAchievableValue} Credits.";

    [ObservableProperty]
    private bool _valuableGenusPredicted = false;

    [ObservableProperty]
    private string _valuableGenusPredictedSpeech = "Possible occurrence of valuable species {SpeciesName} predicted on {PlanetType} {PlanetName}.";

    [ObservableProperty]
    private bool _valuableGeneraPredicted = true;

    [ObservableProperty]
    private string _valuableGeneraPredictedSpeech = "Possible occurrences of {ValuableSpeciesCount} potentially valuable species predicted on {PlanetType} {PlanetName}.";

    [ObservableProperty]
    private bool _leaveClonalColonyRange = true;

    [ObservableProperty]
    private string _leaveClonalColonyRangeSpeech = "Leaving {SpeciesClonColRng} meter clonal colony range of {SpeciesName}, scan count is {SpeciesScanCount}.";

    [ObservableProperty]
    private bool _enterClonalColonyRange = true;

    [ObservableProperty]
    private string _enterClonalColonyRangeSpeech = "Entering clonal colony range of {SpeciesName}.";

    [ObservableProperty]
    private bool _matchingClassificationsFound = false;

    [ObservableProperty]
    private string _matchingClassificationsFoundSpeech = "{PlanetType} {PlanetName}, is a planet of interest with {PoiCriteriaSetsCount} matching criteria sets.";

    [ObservableProperty]
    private bool _matchingClassificationFound = true;

    [ObservableProperty]
    private string _matchingClassificationFoundSpeech = "{PlanetType} {PlanetName}, is a {PoiCriteriaSetName} planet of interest.";

    [ObservableProperty]
    private bool _ring = false;

    [ObservableProperty]
    private string _ringSpeech = "{RingType} ring with a density of {RingDensity} mega tonnes per square kilometer and {RingsReserveLevel} reserve levels at {BodyType} {BodyName}.";

    [ObservableProperty]
    private bool _ringCount = false;

    [ObservableProperty]
    private string _ringCountSpeech = "{BodyType} {BodyName}, is a ring body with {RingsReserveLevel} ring reserve levels, ring count is {RingsCount}.";

    public UserSettingsSpeech()
    {
        SetDefaultValues();
    }

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
            { "LandableSpeech", Resources.Speech_Landable },
            { "ValuableBody", true },
            { "ValuableBodySpeech", Resources.Speech_ValuableBody },
            { "ValuableGenusPredicted", false },
            { "ValuableGenusPredictedSpeech", Resources.Speech_ValuableGenusPredicted },
            { "ValuableGeneraPredicted", true },
            { "ValuableGeneraPredictedSpeech", Resources.Speech_ValuableGeneraPredicted },
            { "LeaveClonalColonyRange", true },
            { "LeaveClonalColonyRangeSpeech", Resources.Speech_LeaveClonalColonyRange },
            { "EnterClonalColonyRange", true },
            { "EnterClonalColonyRangeSpeech", Resources.Speech_EnterClonalColonyRange },
            { "MatchingClassificationsFound", false },
            { "MatchingClassificationsFoundSpeech", Resources.Speech_MatchingClassificationsFound },
            { "MatchingClassificationFound", true },
            { "MatchingClassificationFoundSpeech", Resources.Speech_MatchingClassificationFound },
            { "Ring", false },
            { "RingSpeech", Resources.Speech_Ring },
            { "RingCount", false },
            { "RingCountSpeech", Resources.Speech_RingCount }
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

public partial class UserSettingsOther : ObservableObject
{
    [ObservableProperty]
    private string _edSavedGamePath = string.Empty;

    [ObservableProperty]
    private bool _automaticTabSwitching;

    [ObservableProperty]
    private int _displaySize;

    [ObservableProperty]
    private int _valuableGenusThreshold;

    [ObservableProperty]
    private int _valuableBodyThreshold;

    [ObservableProperty]
    private int _biologicalsViewAltitudeThreshold;

    public UserSettingsOther()
    {
        SetDefaultValues();
    }

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

    private static string GetDefaultEdSavedGamePath()
    {
        var path = Path.Combine(
            Environment.GetEnvironmentVariable("USERPROFILE") ?? string.Empty,
            "Saved Games",
            "Frontier Developments",
            "Elite Dangerous");

        if (Directory.Exists(path))
        {
            return path;
        }

        return string.Empty;
    }
}
