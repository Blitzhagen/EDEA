using EDEA.ViewModels;

namespace EDEA.Commands;

/// <summary>
/// Command that opens the feedback and issue report window.
/// </summary>
public class ShowFeedbackReportIssueWindowCommand : CommandBase
{
    private readonly FeedbackReportIssueViewModel _feedbackReportIssueViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowFeedbackReportIssueWindowCommand"/> class.
    /// </summary>
    /// <param name="feedbackReportIssueViewModel">The view model that controls the feedback window.</param>
    public ShowFeedbackReportIssueWindowCommand(FeedbackReportIssueViewModel feedbackReportIssueViewModel)
    {
        _feedbackReportIssueViewModel = feedbackReportIssueViewModel;
    }

    /// <summary>
    /// Shows the feedback and issue report window.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _feedbackReportIssueViewModel.ShowFeedbackReportIssueWindow();
    }
}
