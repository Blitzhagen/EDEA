namespace EDEA.Models;

public class JournalImportReportData
{
    public int SystemsScanProgress { get; set; }
    public int BodiesScanProgress { get; set; }
    public int AdditionalDataScanProgress { get; set; }
    public int AddedToHistoryCount { get; set; }
    public int UpdatedInHistoryCount { get; set; }
    public int IgnoredSystemsCount { get; set; }
    public int JournalFilesProcessed { get; set; }

    public void ResetData()
    {
        SystemsScanProgress = 0;
        BodiesScanProgress = 0;
        AdditionalDataScanProgress = 0;
        AddedToHistoryCount = 0;
        UpdatedInHistoryCount = 0;
        IgnoredSystemsCount = 0;
        JournalFilesProcessed = 0;
    }
}
