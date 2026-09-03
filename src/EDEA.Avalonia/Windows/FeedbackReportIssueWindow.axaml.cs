using Avalonia.Controls;
using Avalonia.Interactivity;
using EDEA.Avalonia.ViewModels;
using EDEA.Services;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia window for sending feedback or reporting an issue.
/// </summary>
public partial class FeedbackReportIssueWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeedbackReportIssueWindow"/> class.
    /// </summary>
    public FeedbackReportIssueWindow()
    {
        InitializeComponent();
        DataContext = new FeedbackReportIssueViewModel();
    }

    /// <summary>
    /// Composes a mail message from the input fields and opens the default mail client.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void SendButton_Click(object? sender, RoutedEventArgs e)
    {
        var name = NameTextBox.Text ?? string.Empty;
        var email = EmailTextBox.Text ?? string.Empty;
        var message = MessageTextBox.Text ?? string.Empty;

        var subject = $"EDEA Feedback from {name}";
        var body = $"Name: {name}%0D%0AEmail: {email}%0D%0A%0D%0A{message}";

        PlatformServices.Platform?.OpenMailTo("edea-feedback@example.com", subject, body);
        Close();
    }

    /// <summary>
    /// Closes the feedback window without sending a message.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
