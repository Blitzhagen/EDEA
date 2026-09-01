using System.Diagnostics;
using System.Windows;

namespace EDEA.Windows;

/// <summary>
/// Window that displays information about the application.
/// </summary>
public partial class AboutWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutWindow"/> class.
    /// </summary>
    public AboutWindow()
    {
        InitializeComponent();
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

    /// <summary>
    /// Opens the hyperlink in the default browser.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e"> The routed event data.</param>
    private void OnRequestNavigate(object? sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Documents.Hyperlink hyperlink && !string.IsNullOrEmpty(hyperlink.NavigateUri?.ToString()))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = hyperlink.NavigateUri.ToString(),
                UseShellExecute = true
            });
            e.Handled = true;
        }
    }
}
