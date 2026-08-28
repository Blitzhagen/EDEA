using System.Windows;
using EDEA.Services;
using EDEA.ViewModels;

namespace EDEA.Commands;

public class ClearHistoryCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly HistoryProvider _historyProvider;
    private readonly StarSystemProvider _starSystemProvider;

    public ClearHistoryCommand(MainViewModel mainViewModel, HistoryProvider historyProvider, StarSystemProvider starSystemProvider)
    {
        _mainViewModel = mainViewModel;
        _historyProvider = historyProvider;
        _starSystemProvider = starSystemProvider;
    }

    public override void Execute(object? parameter)
    {
        _mainViewModel.SelectTabByName("History");
        if (parameter != null && parameter.Equals("Trip"))
        {
            string caption = "Resetting Exploration Trip Data";
            MessageBoxButton button = MessageBoxButton.OKCancel;
            MessageBoxImage icon = MessageBoxImage.Asterisk;
            if (MessageBox.Show("The data of your current exploration trip will be reset. A new exploration trip then begins immediately.", caption, button, icon, MessageBoxResult.Cancel) == MessageBoxResult.OK)
            {
                _ = _historyProvider.ResetTripData();
            }
            return;
        }

        string warningCaption = "Warning: Deleting Entire Exploration History!";
        MessageBoxButton warningButton = MessageBoxButton.YesNoCancel;
        MessageBoxImage warningIcon = MessageBoxImage.Exclamation;
        if (MessageBox.Show("Your entire exploration history in EDEA will be deleted. As a result, all data collected on systems, bodies and biologicals will be erased and all speech output for systems already discovered will be repeated.\n\nAre you sure you want to continue?", warningCaption, warningButton, warningIcon, MessageBoxResult.Cancel) != MessageBoxResult.Yes)
        {
            return;
        }

        string confirmCaption = "Please Confirm: Deleting Entire Exploration History!";
        MessageBoxButton confirmButton = MessageBoxButton.YesNoCancel;
        MessageBoxImage confirmIcon = MessageBoxImage.Exclamation;
        if (MessageBox.Show("All data retrieved by EDSM is lost and cannot be restored by a subsequent journal import!\n\nAre you really sure you want to delete your entire exploration history?", confirmCaption, confirmButton, confirmIcon, MessageBoxResult.Cancel) == MessageBoxResult.Yes)
        {
            _ = _historyProvider.ClearAll();
            if (_starSystemProvider.CurrentSystem != null)
            {
                _starSystemProvider.CurrentSystem.IsTripHistory = true;
            }
        }
    }
}
