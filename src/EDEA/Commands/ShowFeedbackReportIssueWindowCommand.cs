using EDEA.ViewModels;

namespace EDEA.Commands;

public class ShowFeedbackReportIssueWindowCommand : CommandBase
{
    private readonly FeedbackReportIssueViewModel _feedbackReportIssueViewModel;

    public ShowFeedbackReportIssueWindowCommand(FeedbackReportIssueViewModel feedbackReportIssueViewModel)
    {
        _feedbackReportIssueViewModel = feedbackReportIssueViewModel;
    }

    public override void Execute(object? parameter)
    {
        _feedbackReportIssueViewModel.ShowFeedbackReportIssueWindow();
    }
}
