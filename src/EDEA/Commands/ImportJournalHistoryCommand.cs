using EDEA.ViewModels;
using log4net;

namespace EDEA.Commands;

public class ImportJournalHistoryCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ImportJournalHistoryCommand));

    private readonly JournalHistoryImportViewModel _importJournalHistoryViewModel;

    public ImportJournalHistoryCommand(JournalHistoryImportViewModel importJournalHistoryViewModel)
    {
        _importJournalHistoryViewModel = importJournalHistoryViewModel;
    }

    public override void Execute(object? parameter)
    {
        _importJournalHistoryViewModel.ShowImportJournalHistoryWindow();
    }
}
