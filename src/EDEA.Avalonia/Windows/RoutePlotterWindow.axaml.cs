using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia route plotter window placeholder.
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
    /// Closes the route plotter window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
