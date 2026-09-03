using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia window for importing a Spansh route.
/// </summary>
public partial class RoutePlotterWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoutePlotterWindow"/> class.
    /// </summary>
    public RoutePlotterWindow()
    {
        InitializeComponent();
        RouteUrlTextBox.Text = string.Empty;
    }

    /// <summary>
    /// Saves the route URL and closes the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void GenerateButton_Click(object? sender, RoutedEventArgs e)
    {
        Preferences.SaveUserSettings();
        Close();
    }

    /// <summary>
    /// Closes the window without saving.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
