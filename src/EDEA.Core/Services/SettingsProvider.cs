using System.IO;
using System.Text.Json;
using EDEA.Core.Drawing;
using EDEA.Models;
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
            return JsonSerializer.Deserialize<UserSettings>(json, Options) ?? new UserSettings();
        }
        catch (Exception exception)
        {
            log.Warn($"Could not deserialize settings from '{_path}', using defaults", exception);
            return new UserSettings();
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
