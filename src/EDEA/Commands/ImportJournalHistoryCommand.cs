using EDEA.ViewModels;
using log4net;

namespace EDEA.Commands;

/// <summary>
/// Command that opens the journal history import window.
/// </summary>
public class ImportJournalHistoryCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ImportJournalHistoryCommand));

    private readonly JournalHistoryImportViewModel _importJournalHistoryViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportJournalHistoryCommand"/> class.
    /// </summary>
    /// <param name="importJournalHistoryViewModel">The view model for importing journal history.</param>
    public ImportJournalHistoryCommand(JournalHistoryImportViewModel importJournalHistoryViewModel)
    {
        _importJournalHistoryViewModel = importJournalHistoryViewModel;
    }

    /// <summary>
    /// Opens the window to import journal history.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _importJournalHistoryViewModel.ShowImportJournalHistoryWindow();
    }
}
