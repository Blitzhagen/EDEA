using System.IO;
using System.Text.Json;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

public class SettingsProvider
{
    private static readonly ILog log = LogManager.GetLogger(typeof(SettingsProvider));

    private readonly string _path;

    public UserSettings Settings { get; private set; }

    public SettingsProvider(string? path = null)
    {
        _path = path ?? Path.Combine(
            Globals.AppDataFolder,
            "settings.json");

        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        Settings = Load();
        Preferences.User = Settings;
    }

    private UserSettings Load()
    {
        if (!File.Exists(_path))
            return new UserSettings();

        var json = File.ReadAllText(_path);
        try
        {
            return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
        }
        catch (Exception exception)
        {
            log.Warn($"Could not deserialize settings from '{_path}', using defaults", exception);
            return new UserSettings();
        }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_path, json);
    }

    public void Reload()
    {
        Settings = Load();
        Preferences.User = Settings;
    }
}
