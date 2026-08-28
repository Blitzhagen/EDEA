using EDEA.ViewModels;
using EDEA.Windows;

namespace EDEA.Commands;

/// <summary>
/// Command that saves all preferences and closes the preferences window.
/// </summary>
public class SaveAndClosePreferencesCommand : CommandBase
{
    private readonly PreferencesWindow _preferencesWindow;
    private readonly PreferencesViewModel _preferencesViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveAndClosePreferencesCommand"/> class.
    /// </summary>
    /// <param name="preferencesViewModel">The preferences view model to save.</param>
    /// <param name="preferencesWindow">The preferences window to close.</param>
    public SaveAndClosePreferencesCommand(PreferencesViewModel preferencesViewModel, PreferencesWindow preferencesWindow)
    {
        _preferencesWindow = preferencesWindow;
        _preferencesViewModel = preferencesViewModel;
    }

    /// <summary>
    /// Saves all preference changes and closes the associated window.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _preferencesViewModel.SaveAllPreferences();
        _preferencesWindow.Close();
    }
}
