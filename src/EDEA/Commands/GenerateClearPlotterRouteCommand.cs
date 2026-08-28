using System;
using EDEA.Services;
using EDEA.ViewModels;

namespace EDEA.Commands;

public class GenerateClearPlotterRouteCommand : CommandBase
{
    private readonly RoutePlotterViewModel _routePlotterViewModel;
    private readonly RouteProvider _routeProvider;
    private readonly Action? _onCleared;

    public GenerateClearPlotterRouteCommand(RoutePlotterViewModel routePlotterViewModel, RouteProvider routeProvider, Action? onCleared = null)
    {
        _routePlotterViewModel = routePlotterViewModel;
        _routeProvider = routeProvider;
        _onCleared = onCleared;
    }

    public override void Execute(object? parameter)
    {
        if (_routeProvider.IsCustomRoute)
        {
            _routeProvider.DeletePlotterRoute();
            _onCleared?.Invoke();
        }
        else
        {
            _routePlotterViewModel.ShowRoutePlotterWindow();
        }
    }
}
