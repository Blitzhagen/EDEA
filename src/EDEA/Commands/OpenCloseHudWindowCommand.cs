using EDEA.ViewModels;

namespace EDEA.Commands;

/// <summary>
/// Command that opens or closes the HUD window depending on its current state.
/// </summary>
public class OpenCloseHudWindowCommand : CommandBase
{
    private readonly HudViewModel _hudViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCloseHudWindowCommand"/> class.
    /// </summary>
    /// <param name="hudViewModel">The HUD view model that controls the HUD window.</param>
    public OpenCloseHudWindowCommand(HudViewModel hudViewModel)
    {
        _hudViewModel = hudViewModel;
    }

    /// <summary>
    /// Closes the HUD window when open; otherwise opens it.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        if (_hudViewModel.HudWindowOpen)
        {
            _hudViewModel.CloseHudWindowCommand!.Execute(null);
        }
        else
        {
            _hudViewModel.ShowHudWindow();
        }
    }
}
