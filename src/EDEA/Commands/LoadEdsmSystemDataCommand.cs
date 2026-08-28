using EDEA.Services;
using EDEA.ViewModels;

namespace EDEA.Commands;

public class LoadEdsmSystemDataCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly StarSystemProvider _starSystemProvider;

    public LoadEdsmSystemDataCommand(MainViewModel mainViewModel, StarSystemProvider starSystemProvider)
    {
        _mainViewModel = mainViewModel;
        _starSystemProvider = starSystemProvider;
    }

    public override void Execute(object? parameter)
    {
        _starSystemProvider.HandleLoadEdsmSystemDataCommand(forceUpdate: true);
        _mainViewModel.SelectTabByName("Bodies");
    }
}
