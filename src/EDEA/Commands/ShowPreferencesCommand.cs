using EDEA.ViewModels;

namespace EDEA.Commands;

public class ShowPreferencesWindowCommand : CommandBase
{
    private readonly PreferencesViewModel _preferencesViewModel;

    public ShowPreferencesWindowCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    public override void Execute(object? parameter)
    {
        _preferencesViewModel.ShowPreferencesWindow();
    }
}
