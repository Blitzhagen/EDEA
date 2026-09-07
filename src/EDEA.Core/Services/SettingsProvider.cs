using System.Globalization;
using System.IO;
using System.Text.Json;
using EDEA.Core.Drawing;
using EDEA.Models;
using EDEA.Properties;
using log4net;

namespace EDEA.Services;

/// <summary>Represents the SettingsProvider class.</summary>
public class SettingsProvider
{
    /// <summary>The log field.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(SettingsProvider));

    /// <summary>The _path field.</summary>
    private readonly string _path;

    /// <summary>Gets or sets the Settings.</summary>
    /// <value>A UserSettings value.</value>
    public UserSettings Settings { get; private set; }

    /// <summary>Initializes a new instance of the SettingsProvider class.</summary>
    /// <param name="path">The string? value of the path parameter.</param>
    public SettingsProvider(string? path = null)
    {
        _path = path ?? Path.Combine(
            Globals.AppDataFolder,
            "settings.json");

        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        Settings = Load();
        Preferences.User = Settings;
        Resources.CultureChanged += () => MigrateSpeechDefaults(Preferences.User);
    }

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new ColorJsonConverter() },
    };

    /// <summary>Loads .</summary>
    /// <returns>A UserSettings result.</returns>
    private UserSettings Load()
    {
        if (!File.Exists(_path))
            return new UserSettings();

        var json = File.ReadAllText(_path);
        try
        {
            var settings = JsonSerializer.Deserialize<UserSettings>(json, Options) ?? new UserSettings();
            MigrateSpeechDefaults(settings);
            return settings;
        }
        catch (Exception exception)
        {
            log.Warn($"Could not deserialize settings from '{_path}', using defaults", exception);
            return new UserSettings();
        }
    }

    /// <summary>
    /// Replaces speech texts that still hold a known default (any supported language or a legacy
    /// value) with the default of the current UI culture. User-customized texts are kept.
    /// </summary>
    /// <param name="settings">The loaded user settings.</param>
    private static void MigrateSpeechDefaults(UserSettings settings)
    {
        var knownCultures = new[] { new CultureInfo("en"), new CultureInfo("de"), new CultureInfo("ru") };
        foreach (var property in typeof(UserSettingsSpeech).GetProperties())
        {
            if (!property.Name.EndsWith("Speech", StringComparison.Ordinal)
                || property.PropertyType != typeof(string)
                || !property.CanWrite)
            {
                continue;
            }

            var key = "Speech_" + property.Name[..^"Speech".Length];
            var current = (string?)property.GetValue(settings.Speech);
            if (string.IsNullOrEmpty(current))
            {
                continue;
            }

            var defaults = knownCultures.Select(culture => Resources.LookupFor(key, culture)).ToHashSet();
            if (property.Name == nameof(UserSettingsSpeech.WelcomeSpeech))
            {
                defaults.Add("o7 Commander {CommanderName}, Erkundungsassistent ist bereit.");
                defaults.Add("o7 Commander {CommanderName}, EDEA at your service!");
            }

            if (defaults.Contains(current))
            {
                property.SetValue(settings.Speech, Resources.Lookup(key));
            }
        }
    }

    /// <summary>Saves .</summary>
    public void Save()
    {
        var json = JsonSerializer.Serialize(Settings, Options);
        File.WriteAllText(_path, json);
    }

    /// <summary>Performs the Reload operation.</summary>
    public void Reload()
    {
        Settings = Load();
        Preferences.User = Settings;
    }
}
