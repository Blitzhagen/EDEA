using System.Windows;
using EDEA.ViewModels;

namespace EDEA.Commands;

/// <summary>
/// Command that cancels pending preference changes and closes the preferences window.
/// </summary>
public class CancelPreferencesCommand : CommandBase
{
    private readonly PreferencesViewModel _viewModel;
    private readonly Window _window;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelPreferencesCommand"/> class.
    /// </summary>
    /// <param name="viewModel">The preferences view model to cancel changes on.</param>
    /// <param name="window">The window to close.</param>
    public CancelPreferencesCommand(PreferencesViewModel viewModel, Window window)
    {
        _viewModel = viewModel;
        _window = window;
    }

    /// <summary>
    /// Cancels the preference changes and closes the associated window.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _viewModel.Cancel();
        _window.Close();
    }
}
