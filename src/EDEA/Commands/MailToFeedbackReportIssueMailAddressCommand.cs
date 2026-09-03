using System;
using EDEA.Services;
using log4net;

namespace EDEA.Commands;

/// <summary>
/// Command that opens the default mail application with a preconfigured feedback or issue report.
/// </summary>
public class MailToFeedbackReportIssueMailAddressCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(MailToFeedbackReportIssueMailAddressCommand));

    /// <summary>
    /// Opens the default mail application using the preconfigured address, subject, and body.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        try
        {
            PlatformServices.Platform?.OpenMailTo(
                Globals.FeedbackReportIssueMailAddress,
                Globals.FeedbackReportIssueMailSubject,
                Globals.FeedbackReportIssueMailBody);
        }
        catch (Exception exception)
        {
            log.Error("Error on opening mail application.", exception);
        }
    }
}
