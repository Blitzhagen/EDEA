using System.Diagnostics;
using System.Windows;

namespace EDEA.Windows;

public partial class FeedbackReportIssueWindow : Window
{
    public FeedbackReportIssueWindow()
    {
        InitializeComponent();
    }

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

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
