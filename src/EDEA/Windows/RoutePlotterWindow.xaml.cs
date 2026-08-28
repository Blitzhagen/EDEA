using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace EDEA.Windows;

public partial class RoutePlotterWindow : Window
{
    public RoutePlotterWindow()
    {
        InitializeComponent();
    }

    private void OnRequestNavigate(object? sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }
}
