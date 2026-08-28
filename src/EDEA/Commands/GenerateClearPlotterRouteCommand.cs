using System;
using EDEA.Services;
using EDEA.ViewModels;

namespace EDEA.Commands;

/// <summary>
/// Command that clears an existing, unlocked custom plotter route or opens the route plotter window.
/// Deletion is disabled while the route is locked.
/// </summary>
public class GenerateClearPlotterRouteCommand : CommandBase
{
    private readonly RoutePlotterViewModel _routePlotterViewModel;
    private readonly RouteProvider _routeProvider;
    private readonly Action? _onCleared;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateClearPlotterRouteCommand"/> class.
    /// </summary>
    /// <param name="routePlotterViewModel">The view model for the route plotter window.</param>
    /// <param name="routeProvider">The provider that manages route data.</param>
    /// <param name="onCleared">An optional action invoked after a route has been cleared.</param>
    public GenerateClearPlotterRouteCommand(RoutePlotterViewModel routePlotterViewModel, RouteProvider routeProvider, Action? onCleared = null)
    {
        _routePlotterViewModel = routePlotterViewModel;
        _routeProvider = routeProvider;
        _onCleared = onCleared;
    }

    /// <summary>
    /// Determines whether the command can execute.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    /// <returns><c>true</c> if the route is not a custom route or is not locked; otherwise, <c>false</c>.</returns>
    public override bool CanExecute(object? parameter)
    {
        return !_routeProvider.IsCustomRoute || !_routeProvider.IsLocked;
    }

    /// <summary>
    /// Deletes the custom plotter route when one exists; otherwise opens the route plotter window.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
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
