using System.Windows;
using EDEA.ViewModels;
using EDEA.Windows;

namespace EDEA.Commands;

public class RestoreDefaultPreferencesCommand : CommandBase
{
    private readonly PreferencesWindow _preferencesWindow;
    private readonly PreferencesViewModel _preferencesViewModel;

    public RestoreDefaultPreferencesCommand(PreferencesViewModel preferencesViewModel, PreferencesWindow preferencesWindow)
    {
        _preferencesWindow = preferencesWindow;
        _preferencesViewModel = preferencesViewModel;
    }

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
