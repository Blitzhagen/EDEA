using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Threading;
using EDEA.ViewModels;

namespace EDEA.Avalonia.Views;

/// <summary>
/// Avalonia view for the navigation route HUD table.
/// </summary>
public partial class NavRouteHudTableView : UserControl
{
    /// <summary>
    /// The last system the view was scrolled to.
    /// </summary>
    private (string? name, int jumpDistance) _lastScrolledSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavRouteHudTableView"/> class.
    /// </summary>
    public NavRouteHudTableView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += (_, _) => ScrollToCurrentSystem();
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is NavRouteTableViewModel oldViewModel)
        {
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (DataContext is NavRouteTableViewModel newViewModel)
        {
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NavRouteTableViewModel.CurrentSystem))
        {
            ScrollToCurrentSystem();
        }
    }

    private void ScrollToCurrentSystem()
    {
        if (DataContext is not NavRouteTableViewModel viewModel)
        {
            return;
        }

        if (viewModel.CurrentSystem == null)
        {
            return;
        }

        var current = viewModel.CurrentSystem;
        if (current.Name == _lastScrolledSystem.name && current.JumpDistance == _lastScrolledSystem.jumpDistance)
        {
            return;
        }

        _lastScrolledSystem = (current.Name, current.JumpDistance);

        Dispatcher.UIThread.Post(() =>
        {
            if (RouteDataGrid == null)
            {
                return;
            }

            // Select the current row and scroll it into view after the DataGrid
            // has laid out its items.
            RouteDataGrid.SelectedItem = current;
            RouteDataGrid.ScrollIntoView(current, null);
        }, DispatcherPriority.Background);
    }
}
