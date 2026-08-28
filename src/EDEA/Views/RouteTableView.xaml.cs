using System.Windows;
using System.Windows.Controls;
using EDEA.ViewModels;

namespace EDEA.Views;

public partial class RouteTableView : UserControl
{
    public RouteTableView()
    {
        InitializeComponent();
    }

    private void RouteTableView_Loaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is NavRouteTableViewModel vm)
        {
            vm.registerView(this);
        }
    }

    private void RouteTableView_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is NavRouteTableViewModel vm)
        {
            vm.unregisterView(this);
        }
    }
}

