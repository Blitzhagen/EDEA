using System.Diagnostics;
using System.Windows;

namespace EDEA.Windows;

/// <summary>
/// Window for sending feedback or reporting an issue.
/// </summary>
public partial class FeedbackReportIssueWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeedbackReportIssueWindow"/> class.
    /// </summary>
    public FeedbackReportIssueWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Composes a mail message from the input fields and opens the default mail client.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void SendButton_Click(object? sender, RoutedEventArgs e)
    {
        var name = NameTextBox.Text;
        var email = EmailTextBox.Text;
        var message = MessageTextBox.Text;

        var subject = $"EDEA Feedback from {name}";
        var body = $"Name: {name}%0D%0AEmail: {email}%0D%0A%0D%0A{message}";
        var url = $"mailto:edea-feedback@example.com?subject={subject}&body={body}";

        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
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
