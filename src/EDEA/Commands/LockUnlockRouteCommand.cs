using System;
using System.Windows;
using EDEA.Services;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Commands;

/// <summary>
/// Command that toggles the lock state of the current route.
/// </summary>
public class LockUnlockRouteCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(LockUnlockRouteCommand));

    private readonly MainViewModel _mainViewModel;
    private readonly RouteProvider _routeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LockUnlockRouteCommand"/> class.
    /// </summary>
    /// <param name="mainViewModel">The main view model used to open and refresh the navigation route tab.</param>
    /// <param name="routeProvider">The provider that manages the route lock state.</param>
    public LockUnlockRouteCommand(MainViewModel mainViewModel, RouteProvider routeProvider)
    {
        _mainViewModel = mainViewModel;
        _routeProvider = routeProvider;
    }

    /// <summary>
    /// Unlocks the route when locked, otherwise locks it.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _mainViewModel.OpenTabOfType(typeof(NavRouteTableViewModel), forceOpen: true);
        if (_routeProvider.IsLocked)
        {
            _routeProvider.UnlockRoute();
        }
        else
        {
            _routeProvider.LockRoute();
        }
        _mainViewModel.RefreshMenuItems();
    }
}
