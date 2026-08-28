using EDEA.Services;

namespace EDEA.Commands;

public class SaveSettingsCommand : CommandBase
{
    private readonly SettingsProvider _settingsProvider;

    public SaveSettingsCommand(SettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    public override void Execute(object? parameter)
    {
        _settingsProvider.Save();
    }
}
