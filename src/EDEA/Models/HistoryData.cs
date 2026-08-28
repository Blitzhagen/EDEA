namespace EDEA.Models;

public class HistoryData
{
    public int SystemCount { get; set; }

    public int TripSystemCount { get; set; }

    public int SystemFirstDiscoveryCount { get; set; }

    public int TripSystemFirstDiscoveryCount { get; set; }

    public long BodyCartographicBaseValueSum { get; set; }

    public long TripBodyCartographicBaseValueSum { get; set; }

    public HistoryStatistics SystemStarClassStatistics { get; set; }

    public HistoryStatistics TripSystemStarClassStatistics { get; set; }

    public int BodySignalSum { get; set; }

    public int TripBodySignalSum { get; set; }

    public int BodyCount { get; set; }

    public int TripBodyCount { get; set; }

    public int BodyFirstDiscoveryCount { get; set; }

    public int TripBodyFirstDiscoveryCount { get; set; }

    public int BodyTerraformableCount { get; set; }

    public int TripBodyTerraformableCount { get; set; }

    public int BodyValuableCount { get; set; }

    public int TripBodyValuableCount { get; set; }

    public int BodySurfaceScanCount { get; set; }

    public int TripBodySurfaceScanCount { get; set; }

    public long BodyCartographicSurfaceScanValueSum { get; set; }

    public long TripBodyCartographicSurfaceScanValueSum { get; set; }

    public int BodyTouchdownCount { get; set; }

    public int TripBodyTouchdownCount { get; set; }

    public int BodyRingBodyCount { get; set; }

    public int TripBodyRingBodyCount { get; set; }

    public int GenusSignalSum { get; set; }

    public int TripGenusSignalSum { get; set; }

    public int GenusCount { get; set; }

    public int TripGenusCount { get; set; }

    public int GenusAnalysisCompleteCount { get; set; }

    public int TripGenusAnalysisCompleteCount { get; set; }

    public long GenusVistaGenomicsValueSum { get; set; }

    public long TripGenusVistaGenomicsValueSum { get; set; }

    public HistoryStatistics GenusSpeciesStatistics { get; set; }

    public HistoryStatistics TripGenusSpeciesStatistics { get; set; }

    public long BodyCartographicValueSum { get; set; }

    public long TripBodyCartographicValueSum { get; set; }

    public long GenusVistaGenomicsBaseValueSum { get; set; }

    public long TripGenusVistaGenomicsBaseValueSum { get; set; }

    public int GenusFirstDiscoveryCount { get; set; }

    public int TripGenusFirstDiscoveryCount { get; set; }

    public int RingCount { get; set; }

    public int TripRingCount { get; set; }

    public int RingFirstDiscoveryCount { get; set; }

    public int TripRingFirstDiscoveryCount { get; set; }

    public HistoryStatistics RingTypeStatistics { get; set; }

    public HistoryStatistics TripRingTypeStatistics { get; set; }

    public HistoryData()
    {
        SystemStarClassStatistics = new HistoryStatistics();
        TripSystemStarClassStatistics = new HistoryStatistics();
        GenusSpeciesStatistics = new HistoryStatistics();
        TripGenusSpeciesStatistics = new HistoryStatistics();
        RingTypeStatistics = new HistoryStatistics();
        TripRingTypeStatistics = new HistoryStatistics();
    }
}
