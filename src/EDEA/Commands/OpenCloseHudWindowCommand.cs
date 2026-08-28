using EDEA.ViewModels;

namespace EDEA.Commands;

public class OpenCloseHudWindowCommand : CommandBase
{
    private readonly HudViewModel _hudViewModel;

    public OpenCloseHudWindowCommand(HudViewModel hudViewModel)
    {
        _hudViewModel = hudViewModel;
    }

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
