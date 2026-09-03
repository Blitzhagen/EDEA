using System;
using System.Windows.Input;
using EDEA.Commands;
using EDEA.Services;
using EDEA.Windows;

namespace EDEA.ViewModels;

/// <summary>
/// Provides data and commands for the feedback and issue report window.
/// </summary>
public class FeedbackReportIssueViewModel : ViewModelBase
{
    /// <summary>
    /// Holds the singleton instance of the feedback report issue window.
    /// </summary>
    private static FeedbackReportIssueWindow? feedbackReportIssueWindow;

    /// <summary>
    /// Gets a value indicating whether the feedback window is currently open.
    /// </summary>
    /// <value><c>true</c> if the feedback window is open; otherwise, <c>false</c>.</value>
    public bool FeedbackReportIssueWindowOpen => feedbackReportIssueWindow != null;

    /// <summary>
    /// Gets the command that opens the logfile folder.
    /// </summary>
    /// <value>The open logfile folder command.</value>
    public ICommand OpenLogfileFolderCommand { get; }

    /// <summary>
    /// Gets the command that starts a mail to the feedback address.
    /// </summary>
    /// <value>The mail to command.</value>
    public ICommand MailToFeedbackReportIssueMailAddressCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedbackReportIssueViewModel"/> class.
    /// </summary>
    public FeedbackReportIssueViewModel()
    {
        OpenLogfileFolderCommand = new OpenLogfileFolderCommand();
        MailToFeedbackReportIssueMailAddressCommand = new MailToFeedbackReportIssueMailAddressCommand();
    }

    /// <summary>
    /// Shows the feedback and issue report window or activates it if already open.
    /// </summary>
    public void ShowFeedbackReportIssueWindow()
    {
        if (feedbackReportIssueWindow == null)
        {
            feedbackReportIssueWindow = new FeedbackReportIssueWindow();
            PlatformServices.WindowState?.Track(feedbackReportIssueWindow, "FeedbackReportIssueWindow");
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

    /// <summary>
    /// Handles the <see cref="FeedbackReportIssueWindow.Closed"/> event and clears the window reference.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void FeedbackReportIssueWindow_Closed(object? sender, EventArgs e)
    {
        feedbackReportIssueWindow = null;
    }
}
