using System.Windows;
using EDEA.ViewModels;

namespace EDEA.Commands;

public class CancelPreferencesCommand : CommandBase
{
    private readonly PreferencesViewModel _viewModel;
    private readonly Window _window;

    public CancelPreferencesCommand(PreferencesViewModel viewModel, Window window)
    {
        _viewModel = viewModel;
        _window = window;
    }

    public override void Execute(object? parameter)
    {
        _viewModel.Cancel();
        _window.Close();
    }
}
