using System;
using System.Windows.Input;
using EDEA.Commands;
using EDEA.Services;
using EDEA.Windows;

namespace EDEA.ViewModels;

public class FeedbackReportIssueViewModel : ViewModelBase
{
    private static FeedbackReportIssueWindow? feedbackReportIssueWindow;

    public bool FeedbackReportIssueWindowOpen => feedbackReportIssueWindow != null;

    public ICommand OpenLogfileFolderCommand { get; }

    public ICommand MailToFeedbackReportIssueMailAddressCommand { get; }

    public FeedbackReportIssueViewModel()
    {
        OpenLogfileFolderCommand = new OpenLogfileFolderCommand();
        MailToFeedbackReportIssueMailAddressCommand = new MailToFeedbackReportIssueMailAddressCommand();
    }

    public void ShowFeedbackReportIssueWindow()
    {
        if (feedbackReportIssueWindow == null)
        {
            feedbackReportIssueWindow = new FeedbackReportIssueWindow();
            JotSettingsProvider.Tracker.Track(feedbackReportIssueWindow);
            feedbackReportIssueWindow.DataContext = this;
            feedbackReportIssueWindow.Closed += FeedbackReportIssueWindow_Closed;
            feedbackReportIssueWindow.Show();
            return;
        }

        if (!feedbackReportIssueWindow.IsActive)
        {
            feedbackReportIssueWindow.Activate();
        }

        if (!feedbackReportIssueWindow.IsFocused)
        {
            feedbackReportIssueWindow.Focus();
        }
    }

    private void FeedbackReportIssueWindow_Closed(object? sender, EventArgs e)
    {
        feedbackReportIssueWindow = null;
    }
}
