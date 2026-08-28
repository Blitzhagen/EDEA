using System.Windows;
using System.Windows.Controls;
using EDEA.ViewModels;

namespace EDEA.Views;

/// <summary>
/// HUD view that displays the navigation route.
/// </summary>
public partial class NavRouteHudTableView : UserControl
{
    /// <summary>
    /// View model that backs the navigation route table.
    /// </summary>
    private NavRouteTableViewModel? navRouteTableViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavRouteHudTableView"/> class.
    /// </summary>
    public NavRouteHudTableView()
    {
        InitializeComponent();
        Loaded += navRouteHudTableView_Loaded;
        Unloaded += navRouteHudTableView_Unloaded;
    }

    /// <summary>
    /// Handles the <see cref="Loaded"/> event to register this view with the view model.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void navRouteHudTableView_Loaded(object? sender, RoutedEventArgs e)
    {
        navRouteTableViewModel = (NavRouteTableViewModel)DataContext;
        navRouteTableViewModel.registerView(this);
    }

    /// <summary>
    /// Handles the <see cref="Unloaded"/> event to unregister this view from the view model.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void navRouteHudTableView_Unloaded(object? sender, RoutedEventArgs e)
    {
        navRouteTableViewModel?.unregisterView(this);
    }
}
