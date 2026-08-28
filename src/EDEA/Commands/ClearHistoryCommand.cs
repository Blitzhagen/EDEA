using System.Windows;
using EDEA.Services;
using EDEA.ViewModels;

namespace EDEA.Commands;

/// <summary>
/// Command that clears exploration trip or full exploration history after user confirmation.
/// </summary>
public class ClearHistoryCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly HistoryProvider _historyProvider;
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClearHistoryCommand"/> class.
    /// </summary>
    /// <param name="mainViewModel">The main view model used to select the history tab.</param>
    /// <param name="historyProvider">The provider that manages exploration history data.</param>
    /// <param name="starSystemProvider">The provider that exposes the current star system.</param>
    public ClearHistoryCommand(MainViewModel mainViewModel, HistoryProvider historyProvider, StarSystemProvider starSystemProvider)
    {
        _mainViewModel = mainViewModel;
        _historyProvider = historyProvider;
        _starSystemProvider = starSystemProvider;
    }

    /// <summary>
    /// Prompts the user for confirmation and clears either the current trip data or the entire exploration history.
    /// </summary>
    /// <param name="parameter">A string value; if <c>"Trip"</c>, only trip data is reset. Otherwise the full history is cleared.</param>
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
