using System;
using System.Diagnostics;
using log4net;

namespace EDEA.Commands;

public class MailToFeedbackReportIssueMailAddressCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(MailToFeedbackReportIssueMailAddressCommand));

    public override void Execute(object? parameter)
    {
        try
        {
            Process.Start(new ProcessStartInfo($"mailto:{Globals.FeedbackReportIssueMailAddress}?subject={Globals.FeedbackReportIssueMailSubject}&body={Globals.FeedbackReportIssueMailBody}")
            {
                UseShellExecute = true
            });
        }
        catch (Exception exception)
        {
            log.Error("Error on opening mail application.", exception);
        }
    }
}
