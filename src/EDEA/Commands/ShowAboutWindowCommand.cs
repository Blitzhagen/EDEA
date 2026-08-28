using EDEA.ViewModels;

namespace EDEA.Commands;

/// <summary>
/// Command that opens the about window.
/// </summary>
public class ShowAboutWindowCommand : CommandBase
{
    private readonly AboutViewModel _aboutViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAboutWindowCommand"/> class.
    /// </summary>
    /// <param name="aboutViewModel">The view model that controls the about window.</param>
    public ShowAboutWindowCommand(AboutViewModel aboutViewModel)
    {
        _aboutViewModel = aboutViewModel;
    }

    /// <summary>
    /// Shows the about window.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _aboutViewModel.ShowAboutWindow();
    }
}
