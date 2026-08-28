using EDEA.ViewModels;
using EDEA.Windows;

namespace EDEA.Commands;

public class SaveAndClosePreferencesCommand : CommandBase
{
    private readonly PreferencesWindow _preferencesWindow;
    private readonly PreferencesViewModel _preferencesViewModel;

    public SaveAndClosePreferencesCommand(PreferencesViewModel preferencesViewModel, PreferencesWindow preferencesWindow)
    {
        _preferencesWindow = preferencesWindow;
        _preferencesViewModel = preferencesViewModel;
    }

    public override void Execute(object? parameter)
    {
        _preferencesViewModel.SaveAllPreferences();
        _preferencesWindow.Close();
    }
}
