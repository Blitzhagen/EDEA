using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace EDEA.Windows;

/// <summary>
/// Window for plotting a route via an external routing service.
/// </summary>
public partial class RoutePlotterWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoutePlotterWindow"/> class.
    /// </summary>
    public RoutePlotterWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Opens a hyperlink in the default browser.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The navigation event data.</param>
    private void OnRequestNavigate(object? sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }
}
