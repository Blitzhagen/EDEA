using System;
using System.Windows;
using EDEA.Services;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Commands;

public class LockUnlockRouteCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(LockUnlockRouteCommand));

    private readonly MainViewModel _mainViewModel;
    private readonly RouteProvider _routeProvider;

    public LockUnlockRouteCommand(MainViewModel mainViewModel, RouteProvider routeProvider)
    {
        _mainViewModel = mainViewModel;
        _routeProvider = routeProvider;
    }

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
