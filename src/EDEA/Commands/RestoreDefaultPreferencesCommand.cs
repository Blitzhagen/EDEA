using System.Windows;
using EDEA.ViewModels;
using EDEA.Windows;

namespace EDEA.Commands;

/// <summary>
/// Command that restores default preferences and closes the preferences window after confirmation.
/// </summary>
public class RestoreDefaultPreferencesCommand : CommandBase
{
    private readonly PreferencesWindow _preferencesWindow;
    private readonly PreferencesViewModel _preferencesViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="RestoreDefaultPreferencesCommand"/> class.
    /// </summary>
    /// <param name="preferencesViewModel">The preferences view model to reset.</param>
    /// <param name="preferencesWindow">The preferences window to close.</param>
    public RestoreDefaultPreferencesCommand(PreferencesViewModel preferencesViewModel, PreferencesWindow preferencesWindow)
    {
        _preferencesWindow = preferencesWindow;
        _preferencesViewModel = preferencesViewModel;
    }

    /// <summary>
    /// Prompts the user for confirmation and resets all preferences to their default values.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        string caption = "Warning: Resetting All Preferences!";
        MessageBoxButton button = MessageBoxButton.YesNoCancel;
        MessageBoxImage icon = MessageBoxImage.Exclamation;
        if (MessageBox.Show("Are you really sure to reset ALL your preferences to default values including your color and speech settings and POI criteria sets?", caption, button, icon, MessageBoxResult.No) == MessageBoxResult.Yes)
        {
            _preferencesViewModel.ResetAllPreferences();
            _preferencesWindow.Close();
        }
    }
}
