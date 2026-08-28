using EDEA.Services;

namespace EDEA.Commands;

/// <summary>
/// Command that persists the current settings.
/// </summary>
public class SaveSettingsCommand : CommandBase
{
    private readonly SettingsProvider _settingsProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveSettingsCommand"/> class.
    /// </summary>
    /// <param name="settingsProvider">The provider responsible for saving the settings.</param>
    public SaveSettingsCommand(SettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    /// <summary>
    /// Saves the current settings.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _settingsProvider.Save();
    }
}
