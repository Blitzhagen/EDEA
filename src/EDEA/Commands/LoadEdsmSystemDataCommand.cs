using EDEA.Services;
using EDEA.ViewModels;

namespace EDEA.Commands;

/// <summary>
/// Command that loads EDSM system data and selects the bodies tab.
/// </summary>
public class LoadEdsmSystemDataCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadEdsmSystemDataCommand"/> class.
    /// </summary>
    /// <param name="mainViewModel">The main view model used to select the bodies tab.</param>
    /// <param name="starSystemProvider">The provider that loads EDSM system data.</param>
    public LoadEdsmSystemDataCommand(MainViewModel mainViewModel, StarSystemProvider starSystemProvider)
    {
        _mainViewModel = mainViewModel;
        _starSystemProvider = starSystemProvider;
    }

    /// <summary>
    /// Loads EDSM system data and switches to the bodies tab.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _starSystemProvider.HandleLoadEdsmSystemDataCommand(forceUpdate: true);
        _mainViewModel.SelectTabByName("Bodies");
    }
}
