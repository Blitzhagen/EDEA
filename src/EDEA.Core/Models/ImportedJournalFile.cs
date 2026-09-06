namespace EDEA.Models;

/// <summary>
/// Tracks a journal file that has already been imported into the history database.
/// </summary>
public class ImportedJournalFile
{
    /// <summary>
    /// Gets or sets the full path of the imported journal file.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file's last write time in UTC ticks.
    /// </summary>
    public long LastWriteTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets the file length in bytes.
    /// </summary>
    public long Length { get; set; }
}
