using EDEA.ViewModels;

namespace EDEA.Commands;

/// <summary>
/// Command that opens the preferences window.
/// </summary>
public class ShowPreferencesWindowCommand : CommandBase
{
    private readonly PreferencesViewModel _preferencesViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowPreferencesWindowCommand"/> class.
    /// </summary>
    /// <param name="preferencesViewModel">The view model that controls the preferences window.</param>
    public ShowPreferencesWindowCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    /// <summary>
    /// Shows the preferences window.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _preferencesViewModel.ShowPreferencesWindow();
    }
}
