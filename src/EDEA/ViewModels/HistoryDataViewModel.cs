using EDEA.Models;
using EDEA.Properties;

namespace EDEA.ViewModels;

public class HistoryDataViewModel : ViewModelBase
{
    public string SystemCount { get; }

    public string TripSystemCount { get; }

    public string SystemFirstDiscoveryCount { get; }

    public string TripSystemFirstDiscoveryCount { get; }

    public string BodyCartographicBaseValueSum { get; }

    public string TripBodyCartographicBaseValueSum { get; }

    public string SystemMostFrequentStarClass { get; }

    public string TripSystemMostFrequentStarClass { get; }

    public string SystemRarestStarClass { get; }

    public string TripSystemRarestStarClass { get; }

    public string BodySignalSum { get; }

    public string TripBodySignalSum { get; }

    public string BodyCount { get; }

    public string TripBodyCount { get; }

    public string BodyFirstDiscoveryCount { get; }

    public string TripBodyFirstDiscoveryCount { get; }

    public string BodyTerraformableCount { get; }

    public string TripBodyTerraformableCount { get; }

    public string BodyValuableCount { get; }

    public string TripBodyValuableCount { get; }

    public string BodyCartographicSurfaceScanValueSum { get; }

    public string TripBodyCartographicSurfaceScanValueSum { get; }

    public string BodySurfaceScanCount { get; }

    public string TripBodySurfaceScanCount { get; }

    public string BodyTouchdownCount { get; }

    public string TripBodyTouchdownCount { get; }

    public string BodyRingBodyCount { get; }

    public string TripBodyRingBodyCount { get; }

    public string GenusSignalSum { get; }

    public string TripGenusSignalSum { get; }

    public string GenusCount { get; }

    public string TripGenusCount { get; }

    public string GenusAnalysisCompleteCount { get; }

    public string TripGenusAnalysisCompleteCount { get; }

    public string GenusFirstDiscoveryCount { get; }

    public string TripGenusFirstDiscoveryCount { get; }

    public string GenusVistaGenomicsValueSum { get; }

    public string TripGenusVistaGenomicsValueSum { get; }

    public string GenusMostFrequentSpecies { get; }

    public string TripGenusMostFrequentSpecies { get; }

    public string GenusRarestSpecies { get; }

    public string TripGenusRarestSpecies { get; }

    public string TotalExplorationDataValueSum { get; }

    public string TripTotalExplorationDataValueSum { get; }

    public string BodyCartographicValueSum { get; }

    public string TripBodyCartographicValueSum { get; }

    public string BodyCartographicBonusValueSum { get; }

    public string TripBodyCartographicBonusValueSum { get; }

    public string GenusVistaGenomicsBaseValueSum { get; }

    public string TripGenusVistaGenomicsBaseValueSum { get; }

    public string GenusVistaGenomicsFirstDiscoveryBonusValueSum { get; }

    public string TripGenusVistaGenomicsFirstDiscoveryBonusValueSum { get; }

    public string RingCount { get; }

    public string TripRingCount { get; }

    public string RingFirstDiscoveryCount { get; }

    public string TripRingFirstDiscoveryCount { get; }

    public string RingMostFrequentType { get; }

    public string TripRingMostFrequentType { get; }

    public string RingRarestType { get; }

    public string TripRingRarestType { get; }

    public HistoryDataViewModel(HistoryData historyData)
    {
        SystemCount = ((historyData.SystemCount == 0) ? "–" : historyData.SystemCount.ToString("n0"));
        TripSystemCount = ((historyData.TripSystemCount == 0) ? "–" : historyData.TripSystemCount.ToString("n0"));
        SystemFirstDiscoveryCount = ((historyData.SystemFirstDiscoveryCount == 0) ? "–" : historyData.SystemFirstDiscoveryCount.ToString("n0"));
        TripSystemFirstDiscoveryCount = ((historyData.TripSystemFirstDiscoveryCount == 0) ? "–" : historyData.TripSystemFirstDiscoveryCount.ToString("n0"));
        BodyCartographicBaseValueSum = ((historyData.BodyCartographicBaseValueSum == 0L) ? "–" : (historyData.BodyCartographicBaseValueSum.ToString("n0") + " " + Resources.UnitCredits));
        TripBodyCartographicBaseValueSum = ((historyData.TripBodyCartographicBaseValueSum == 0L) ? "–" : (historyData.TripBodyCartographicBaseValueSum.ToString("n0") + " " + Resources.UnitCredits));
        SystemMostFrequentStarClass = ((historyData.SystemStarClassStatistics.MostFrequentTitle == string.Empty) ? "–" : (historyData.SystemStarClassStatistics.MostFrequentTitle + " (" + historyData.SystemStarClassStatistics.MostFrequentCount.ToString("n0") + ")"));
        TripSystemMostFrequentStarClass = ((historyData.TripSystemStarClassStatistics.MostFrequentTitle == string.Empty) ? "–" : (historyData.TripSystemStarClassStatistics.MostFrequentTitle + " (" + historyData.TripSystemStarClassStatistics.MostFrequentCount.ToString("n0") + ")"));
        SystemRarestStarClass = ((historyData.SystemStarClassStatistics.RarestTitle == string.Empty) ? "–" : (historyData.SystemStarClassStatistics.RarestTitle + " (" + historyData.SystemStarClassStatistics.RarestCount.ToString("n0") + ")"));
        TripSystemRarestStarClass = ((historyData.TripSystemStarClassStatistics.RarestTitle == string.Empty) ? "–" : (historyData.TripSystemStarClassStatistics.RarestTitle + " (" + historyData.TripSystemStarClassStatistics.RarestCount.ToString("n0") + ")"));
        BodySignalSum = ((historyData.BodySignalSum == 0) ? "–" : historyData.BodySignalSum.ToString("n0"));
        TripBodySignalSum = ((historyData.TripBodySignalSum == 0) ? "–" : historyData.TripBodySignalSum.ToString("n0"));
        BodyCount = ((historyData.BodyCount == 0) ? "–" : historyData.BodyCount.ToString("n0"));
        TripBodyCount = ((historyData.TripBodyCount == 0) ? "–" : historyData.TripBodyCount.ToString("n0"));
        BodyFirstDiscoveryCount = ((historyData.BodyFirstDiscoveryCount == 0) ? "–" : historyData.BodyFirstDiscoveryCount.ToString("n0"));
        TripBodyFirstDiscoveryCount = ((historyData.TripBodyFirstDiscoveryCount == 0) ? "–" : historyData.TripBodyFirstDiscoveryCount.ToString("n0"));
        BodyTerraformableCount = ((historyData.BodyTerraformableCount == 0) ? "–" : historyData.BodyTerraformableCount.ToString("n0"));
        TripBodyTerraformableCount = ((historyData.TripBodyTerraformableCount == 0) ? "–" : historyData.TripBodyTerraformableCount.ToString("n0"));
        BodyValuableCount = ((historyData.BodyValuableCount == 0) ? "–" : historyData.BodyValuableCount.ToString("n0"));
        TripBodyValuableCount = ((historyData.TripBodyValuableCount == 0) ? "–" : historyData.TripBodyValuableCount.ToString("n0"));
        BodyCartographicSurfaceScanValueSum = ((historyData.BodyCartographicSurfaceScanValueSum == 0L) ? "–" : (historyData.BodyCartographicSurfaceScanValueSum.ToString("n0") + " " + Resources.UnitCredits));
        TripBodyCartographicSurfaceScanValueSum = ((historyData.TripBodyCartographicSurfaceScanValueSum == 0L) ? "–" : (historyData.TripBodyCartographicSurfaceScanValueSum.ToString("n0") + " " + Resources.UnitCredits));
        BodySurfaceScanCount = ((historyData.BodySurfaceScanCount == 0) ? "–" : historyData.BodySurfaceScanCount.ToString("n0"));
        TripBodySurfaceScanCount = ((historyData.TripBodySurfaceScanCount == 0) ? "–" : historyData.TripBodySurfaceScanCount.ToString("n0"));
        BodyTouchdownCount = ((historyData.BodyTouchdownCount == 0) ? "–" : historyData.BodyTouchdownCount.ToString("n0"));
        TripBodyTouchdownCount = ((historyData.TripBodyTouchdownCount == 0) ? "–" : historyData.TripBodyTouchdownCount.ToString("n0"));
        BodyRingBodyCount = ((historyData.BodyRingBodyCount == 0) ? "–" : historyData.BodyRingBodyCount.ToString("n0"));
        TripBodyRingBodyCount = ((historyData.TripBodyRingBodyCount == 0) ? "–" : historyData.TripBodyRingBodyCount.ToString("n0"));
        GenusSignalSum = ((historyData.GenusSignalSum == 0) ? "–" : historyData.GenusSignalSum.ToString("n0"));
        TripGenusSignalSum = ((historyData.TripGenusSignalSum == 0) ? "–" : historyData.TripGenusSignalSum.ToString("n0"));
        GenusCount = ((historyData.GenusCount == 0) ? "–" : historyData.GenusCount.ToString("n0"));
        TripGenusCount = ((historyData.TripGenusCount == 0) ? "–" : historyData.TripGenusCount.ToString("n0"));
        GenusAnalysisCompleteCount = ((historyData.GenusAnalysisCompleteCount == 0) ? "–" : historyData.GenusAnalysisCompleteCount.ToString("n0"));
        TripGenusAnalysisCompleteCount = ((historyData.TripGenusAnalysisCompleteCount == 0) ? "–" : historyData.TripGenusAnalysisCompleteCount.ToString("n0"));
        GenusFirstDiscoveryCount = ((historyData.GenusFirstDiscoveryCount == 0) ? "–" : historyData.GenusFirstDiscoveryCount.ToString("n0"));
        TripGenusFirstDiscoveryCount = ((historyData.TripGenusFirstDiscoveryCount == 0) ? "–" : historyData.TripGenusFirstDiscoveryCount.ToString("n0"));
        GenusVistaGenomicsValueSum = ((historyData.GenusVistaGenomicsValueSum == 0L) ? "–" : (historyData.GenusVistaGenomicsValueSum.ToString("n0") + " " + Resources.UnitCredits));
        TripGenusVistaGenomicsValueSum = ((historyData.TripGenusVistaGenomicsValueSum == 0L) ? "–" : (historyData.TripGenusVistaGenomicsValueSum.ToString("n0") + " " + Resources.UnitCredits));
        GenusMostFrequentSpecies = ((historyData.GenusSpeciesStatistics.MostFrequentTitle == string.Empty) ? "–" : (historyData.GenusSpeciesStatistics.MostFrequentTitle + " (" + historyData.GenusSpeciesStatistics.MostFrequentCount.ToString("n0") + ")"));
        TripGenusMostFrequentSpecies = ((historyData.TripGenusSpeciesStatistics.MostFrequentTitle == string.Empty) ? "–" : (historyData.TripGenusSpeciesStatistics.MostFrequentTitle + " (" + historyData.TripGenusSpeciesStatistics.MostFrequentCount.ToString("n0") + ")"));
        GenusRarestSpecies = ((historyData.GenusSpeciesStatistics.RarestTitle == string.Empty) ? "–" : (historyData.GenusSpeciesStatistics.RarestTitle + " (" + historyData.GenusSpeciesStatistics.RarestCount.ToString("n0") + ")"));
        TripGenusRarestSpecies = ((historyData.TripGenusSpeciesStatistics.RarestTitle == string.Empty) ? "–" : (historyData.TripGenusSpeciesStatistics.RarestTitle + " (" + historyData.TripGenusSpeciesStatistics.RarestCount.ToString("n0") + ")"));
        BodyCartographicValueSum = ((historyData.BodyCartographicValueSum == 0L) ? "–" : (historyData.BodyCartographicValueSum.ToString("n0") + " " + Resources.UnitCredits));
        TripBodyCartographicValueSum = ((historyData.TripBodyCartographicValueSum == 0L) ? "–" : (historyData.TripBodyCartographicValueSum.ToString("n0") + " " + Resources.UnitCredits));
        GenusVistaGenomicsBaseValueSum = ((historyData.GenusVistaGenomicsBaseValueSum == 0L) ? "–" : (historyData.GenusVistaGenomicsBaseValueSum.ToString("n0") + " " + Resources.UnitCredits));
        TripGenusVistaGenomicsBaseValueSum = ((historyData.TripGenusVistaGenomicsBaseValueSum == 0L) ? "–" : (historyData.TripGenusVistaGenomicsBaseValueSum.ToString("n0") + " " + Resources.UnitCredits));
        RingCount = ((historyData.RingCount == 0) ? "–" : historyData.RingCount.ToString("n0"));
        TripRingCount = ((historyData.TripRingCount == 0) ? "–" : historyData.TripRingCount.ToString("n0"));
        RingFirstDiscoveryCount = ((historyData.RingFirstDiscoveryCount == 0) ? "–" : historyData.RingFirstDiscoveryCount.ToString("n0"));
        TripRingFirstDiscoveryCount = ((historyData.TripRingFirstDiscoveryCount == 0) ? "–" : historyData.TripRingFirstDiscoveryCount.ToString("n0"));
        RingMostFrequentType = ((historyData.RingTypeStatistics.MostFrequentTitle == string.Empty) ? "–" : (historyData.RingTypeStatistics.MostFrequentTitle + " (" + historyData.RingTypeStatistics.MostFrequentCount.ToString("n0") + ")"));
        TripRingMostFrequentType = ((historyData.TripRingTypeStatistics.MostFrequentTitle == string.Empty) ? "–" : (historyData.TripRingTypeStatistics.MostFrequentTitle + " (" + historyData.TripRingTypeStatistics.MostFrequentCount.ToString("n0") + ")"));
        RingRarestType = ((historyData.RingTypeStatistics.RarestTitle == string.Empty) ? "–" : (historyData.RingTypeStatistics.RarestTitle + " (" + historyData.RingTypeStatistics.RarestCount.ToString("n0") + ")"));
        TripRingRarestType = ((historyData.TripRingTypeStatistics.RarestTitle == string.Empty) ? "–" : (historyData.TripRingTypeStatistics.RarestTitle + " (" + historyData.TripRingTypeStatistics.RarestCount.ToString("n0") + ")"));
        long genusFirstDiscoveryBonus = historyData.GenusVistaGenomicsValueSum - historyData.GenusVistaGenomicsBaseValueSum;
        GenusVistaGenomicsFirstDiscoveryBonusValueSum = ((genusFirstDiscoveryBonus == 0L) ? "–" : (genusFirstDiscoveryBonus.ToString("n0") + " " + Resources.UnitCredits));
        long tripGenusFirstDiscoveryBonus = historyData.TripGenusVistaGenomicsValueSum - historyData.TripGenusVistaGenomicsBaseValueSum;
        TripGenusVistaGenomicsFirstDiscoveryBonusValueSum = ((tripGenusFirstDiscoveryBonus == 0L) ? "–" : (tripGenusFirstDiscoveryBonus.ToString("n0") + " " + Resources.UnitCredits));
        long bodyCartographicBonus = historyData.BodyCartographicValueSum - historyData.BodyCartographicBaseValueSum - historyData.BodyCartographicSurfaceScanValueSum;
        BodyCartographicBonusValueSum = ((bodyCartographicBonus == 0L) ? "–" : (bodyCartographicBonus.ToString("n0") + " " + Resources.UnitCredits));
        long tripBodyCartographicBonus = historyData.TripBodyCartographicValueSum - historyData.TripBodyCartographicBaseValueSum - historyData.TripBodyCartographicSurfaceScanValueSum;
        TripBodyCartographicBonusValueSum = ((tripBodyCartographicBonus == 0L) ? "–" : (tripBodyCartographicBonus.ToString("n0") + " " + Resources.UnitCredits));
        long totalExplorationValue = historyData.GenusVistaGenomicsValueSum + historyData.BodyCartographicValueSum;
        TotalExplorationDataValueSum = ((totalExplorationValue == 0L) ? "–" : (totalExplorationValue.ToString("n0") + " " + Resources.UnitCredits));
        long tripTotalExplorationValue = historyData.TripGenusVistaGenomicsValueSum + historyData.TripBodyCartographicValueSum;
        TripTotalExplorationDataValueSum = ((tripTotalExplorationValue == 0L) ? "–" : (tripTotalExplorationValue.ToString("n0") + " " + Resources.UnitCredits));
    }
}
