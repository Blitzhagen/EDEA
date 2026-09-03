namespace EDEA.Models;

/// <summary>
/// Tracks the progress and results of a journal import operation.
/// </summary>
public class JournalImportReportData
{
    /// <summary>
    /// Gets or sets the star system scan progress.
    /// </summary>
    /// <value>The systems scan progress.</value>
    public int SystemsScanProgress { get; set; }

    /// <summary>
    /// Gets or sets the body scan progress.
    /// </summary>
    /// <value>The bodies scan progress.</value>
    public int BodiesScanProgress { get; set; }

    /// <summary>
    /// Gets or sets the additional data scan progress.
    /// </summary>
    /// <value>The additional data scan progress.</value>
    public int AdditionalDataScanProgress { get; set; }

    /// <summary>
    /// Gets or sets the number of entries added to history.
    /// </summary>
    /// <value>The added to history count.</value>
    public int AddedToHistoryCount { get; set; }

    /// <summary>
    /// Gets or sets the number of entries updated in history.
    /// </summary>
    /// <value>The updated in history count.</value>
    public int UpdatedInHistoryCount { get; set; }

    /// <summary>
    /// Gets or sets the number of ignored systems.
    /// </summary>
    /// <value>The ignored systems count.</value>
    public int IgnoredSystemsCount { get; set; }

    /// <summary>
    /// Gets or sets the number of processed journal files.
    /// </summary>
    /// <value>The processed journal file count.</value>
    public int JournalFilesProcessed { get; set; }

    /// <summary>
    /// Resets all counters to zero.
    /// </summary>
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
