using EDEA.ViewModels;

namespace EDEA.Commands;

public class EnableDisableHudWindowMousePassThroughCommand : CommandBase
{
    private readonly HudViewModel _hudViewModel;

    public EnableDisableHudWindowMousePassThroughCommand(HudViewModel hudViewModel)
    {
        _hudViewModel = hudViewModel;
    }

    public override void Execute(object? parameter)
    {
        _hudViewModel.SetMousePassThrough(!_hudViewModel.HudWindowMousePassThroughEnabled);
    }
}
