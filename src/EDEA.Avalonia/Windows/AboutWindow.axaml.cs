using Avalonia.Controls;
using Avalonia.Interactivity;
using EDEA.Avalonia.ViewModels;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia window that displays information about the application.
/// </summary>
public partial class AboutWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutWindow"/> class.
    /// </summary>
    public AboutWindow()
    {
        InitializeComponent();
        DataContext = new AboutViewModel();
    }

    /// <summary>
    /// Closes the about window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
