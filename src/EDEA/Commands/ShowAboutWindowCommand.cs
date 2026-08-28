using EDEA.ViewModels;

namespace EDEA.Commands;

public class ShowAboutWindowCommand : CommandBase
{
    private readonly AboutViewModel _aboutViewModel;

    public ShowAboutWindowCommand(AboutViewModel aboutViewModel)
    {
        _aboutViewModel = aboutViewModel;
    }

    public override void Execute(object? parameter)
    {
        _aboutViewModel.ShowAboutWindow();
    }
}
