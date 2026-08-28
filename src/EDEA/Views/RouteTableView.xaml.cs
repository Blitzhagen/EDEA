using System.Windows;
using System.Windows.Controls;
using EDEA.ViewModels;

namespace EDEA.Views;

/// <summary>
/// View that displays the navigation route.
/// </summary>
public partial class RouteTableView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RouteTableView"/> class.
    /// </summary>
    public RouteTableView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the <see cref="Loaded"/> event to register this view with the view model.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void RouteTableView_Loaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is NavRouteTableViewModel vm)
        {
            vm.registerView(this);
        }
    }

    /// <summary>
    /// Handles the <see cref="Unloaded"/> event to unregister this view from the view model.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void RouteTableView_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is NavRouteTableViewModel vm)
        {
            vm.unregisterView(this);
        }
    }
}
