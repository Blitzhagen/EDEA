using EDEA.ViewModels;

namespace EDEA.Commands;

/// <summary>
/// Command that toggles the mouse pass-through state of the HUD window.
/// </summary>
public class EnableDisableHudWindowMousePassThroughCommand : CommandBase
{
    private readonly HudViewModel _hudViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnableDisableHudWindowMousePassThroughCommand"/> class.
    /// </summary>
    /// <param name="hudViewModel">The HUD view model whose mouse pass-through state is toggled.</param>
    public EnableDisableHudWindowMousePassThroughCommand(HudViewModel hudViewModel)
    {
        _hudViewModel = hudViewModel;
    }

    /// <summary>
    /// Toggles the HUD window mouse pass-through state.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _hudViewModel.SetMousePassThrough(!_hudViewModel.HudWindowMousePassThroughEnabled);
    }
}
