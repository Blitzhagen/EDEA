using System.Windows;
using System.Windows.Controls;
using EDEA.ViewModels;

namespace EDEA.Views;

public partial class NavRouteHudTableView : UserControl
{
    private NavRouteTableViewModel? navRouteTableViewModel;

    public NavRouteHudTableView()
    {
        InitializeComponent();
        Loaded += navRouteHudTableView_Loaded;
        Unloaded += navRouteHudTableView_Unloaded;
    }

    private void navRouteHudTableView_Loaded(object? sender, RoutedEventArgs e)
    {
        navRouteTableViewModel = (NavRouteTableViewModel)DataContext;
        navRouteTableViewModel.registerView(this);
    }

    private void navRouteHudTableView_Unloaded(object? sender, RoutedEventArgs e)
    {
        navRouteTableViewModel?.unregisterView(this);
    }
}
