namespace EDEA.Models;

/// <summary>
/// Stores the most frequent and rarest titles for a history statistic category.
/// </summary>
public class HistoryStatistics
{
    /// <summary>
    /// Gets or sets the most frequent title.
    /// </summary>
    /// <value>The most frequent title.</value>
    public string MostFrequentTitle { get; set; }

    /// <summary>
    /// Gets or sets the count of the most frequent title.
    /// </summary>
    /// <value>The most frequent count.</value>
    public int MostFrequentCount { get; set; }

    /// <summary>
    /// Gets or sets the rarest title.
    /// </summary>
    /// <value>The rarest title.</value>
    public string RarestTitle { get; set; }

    /// <summary>
    /// Gets or sets the count of the rarest title.
    /// </summary>
    /// <value>The rarest count.</value>
    public int RarestCount { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryStatistics"/> class.
    /// </summary>
    public HistoryStatistics()
    {
        MostFrequentTitle = string.Empty;
        RarestTitle = string.Empty;
    }
}
