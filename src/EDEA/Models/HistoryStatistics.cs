namespace EDEA.Models;

public class HistoryStatistics
{
    public string MostFrequentTitle { get; set; }

    public int MostFrequentCount { get; set; }

    public string RarestTitle { get; set; }

    public int RarestCount { get; set; }

    public HistoryStatistics()
    {
        MostFrequentTitle = string.Empty;
        RarestTitle = string.Empty;
    }
}
