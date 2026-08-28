using EDEA.Models;
using EDEA.Properties;

namespace EDEA.ViewModels;

/// <summary>
/// View model that holds formatted exploration history data.
/// </summary>
public class HistoryDataViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the total number of visited systems.
    /// </summary>
    /// <value>The system count string.</value>
    public string SystemCount { get; }

    /// <summary>
    /// Gets the number of visited systems for the current trip.
    /// </summary>
    /// <value>The trip system count string.</value>
    public string TripSystemCount { get; }

    /// <summary>
    /// Gets the number of first-discovered systems.
    /// </summary>
    /// <value>The first discovery system count string.</value>
    public string SystemFirstDiscoveryCount { get; }

    /// <summary>
    /// Gets the number of first-discovered systems for the current trip.
    /// </summary>
    /// <value>The trip first discovery system count string.</value>
    public string TripSystemFirstDiscoveryCount { get; }

    /// <summary>
    /// Gets the sum of base cartographic values for bodies.
    /// </summary>
    /// <value>The base cartographic value sum string.</value>
    public string BodyCartographicBaseValueSum { get; }

    /// <summary>
    /// Gets the sum of base cartographic values for bodies in the current trip.
    /// </summary>
    /// <value>The trip base cartographic value sum string.</value>
    public string TripBodyCartographicBaseValueSum { get; }

    /// <summary>
    /// Gets the most frequent star class in visited systems.
    /// </summary>
    /// <value>The most frequent star class string.</value>
    public string SystemMostFrequentStarClass { get; }

    /// <summary>
    /// Gets the most frequent star class in the current trip.
    /// </summary>
    /// <value>The trip most frequent star class string.</value>
    public string TripSystemMostFrequentStarClass { get; }

    /// <summary>
    /// Gets the rarest star class in visited systems.
    /// </summary>
    /// <value>The rarest star class string.</value>
    public string SystemRarestStarClass { get; }

    /// <summary>
    /// Gets the rarest star class in the current trip.
    /// </summary>
    /// <value>The trip rarest star class string.</value>
    public string TripSystemRarestStarClass { get; }

    /// <summary>
    /// Gets the sum of body signals.
    /// </summary>
    /// <value>The body signal sum string.</value>
    public string BodySignalSum { get; }

    /// <summary>
    /// Gets the sum of body signals in the current trip.
    /// </summary>
    /// <value>The trip body signal sum string.</value>
    public string TripBodySignalSum { get; }

    /// <summary>
    /// Gets the total number of bodies.
    /// </summary>
    /// <value>The body count string.</value>
    public string BodyCount { get; }

    /// <summary>
    /// Gets the number of bodies in the current trip.
    /// </summary>
    /// <value>The trip body count string.</value>
    public string TripBodyCount { get; }

    /// <summary>
    /// Gets the number of first-discovered bodies.
    /// </summary>
    /// <value>The first discovery body count string.</value>
    public string BodyFirstDiscoveryCount { get; }

    /// <summary>
    /// Gets the number of first-discovered bodies in the current trip.
    /// </summary>
    /// <value>The trip first discovery body count string.</value>
    public string TripBodyFirstDiscoveryCount { get; }

    /// <summary>
    /// Gets the number of terraformable bodies.
    /// </summary>
    /// <value>The terraformable body count string.</value>
    public string BodyTerraformableCount { get; }

    /// <summary>
    /// Gets the number of terraformable bodies in the current trip.
    /// </summary>
    /// <value>The trip terraformable body count string.</value>
    public string TripBodyTerraformableCount { get; }

    /// <summary>
    /// Gets the number of valuable bodies.
    /// </summary>
    /// <value>The valuable body count string.</value>
    public string BodyValuableCount { get; }

    /// <summary>
    /// Gets the number of valuable bodies in the current trip.
    /// </summary>
    /// <value>The trip valuable body count string.</value>
    public string TripBodyValuableCount { get; }

    /// <summary>
    /// Gets the sum of surface scan cartographic values.
    /// </summary>
    /// <value>The surface scan value sum string.</value>
    public string BodyCartographicSurfaceScanValueSum { get; }

    /// <summary>
    /// Gets the sum of surface scan cartographic values in the current trip.
    /// </summary>
    /// <value>The trip surface scan value sum string.</value>
    public string TripBodyCartographicSurfaceScanValueSum { get; }

    /// <summary>
    /// Gets the number of surface-scanned bodies.
    /// </summary>
    /// <value>The surface scan count string.</value>
    public string BodySurfaceScanCount { get; }

    /// <summary>
    /// Gets the number of surface-scanned bodies in the current trip.
    /// </summary>
    /// <value>The trip surface scan count string.</value>
    public string TripBodySurfaceScanCount { get; }

    /// <summary>
    /// Gets the number of touchdowns on bodies.
    /// </summary>
    /// <value>The touchdown count string.</value>
    public string BodyTouchdownCount { get; }

    /// <summary>
    /// Gets the number of touchdowns on bodies in the current trip.
    /// </summary>
    /// <value>The trip touchdown count string.</value>
    public string TripBodyTouchdownCount { get; }

    /// <summary>
    /// Gets the number of ring bodies.
    /// </summary>
    /// <value>The ring body count string.</value>
    public string BodyRingBodyCount { get; }

    /// <summary>
    /// Gets the number of ring bodies in the current trip.
    /// </summary>
    /// <value>The trip ring body count string.</value>
    public string TripBodyRingBodyCount { get; }

    /// <summary>
    /// Gets the sum of genus signals.
    /// </summary>
    /// <value>The genus signal sum string.</value>
    public string GenusSignalSum { get; }

    /// <summary>
    /// Gets the sum of genus signals in the current trip.
    /// </summary>
    /// <value>The trip genus signal sum string.</value>
    public string TripGenusSignalSum { get; }

    /// <summary>
    /// Gets the total number of genuses.
    /// </summary>
    /// <value>The genus count string.</value>
    public string GenusCount { get; }

    /// <summary>
    /// Gets the number of genuses in the current trip.
    /// </summary>
    /// <value>The trip genus count string.</value>
    public string TripGenusCount { get; }

    /// <summary>
    /// Gets the number of genuses with complete analysis.
    /// </summary>
    /// <value>The analysis complete genus count string.</value>
    public string GenusAnalysisCompleteCount { get; }

    /// <summary>
    /// Gets the number of genuses with complete analysis in the current trip.
    /// </summary>
    /// <value>The trip analysis complete genus count string.</value>
    public string TripGenusAnalysisCompleteCount { get; }

    /// <summary>
    /// Gets the number of first-discovered genuses.
    /// </summary>
    /// <value>The first discovery genus count string.</value>
    public string GenusFirstDiscoveryCount { get; }

    /// <summary>
    /// Gets the number of first-discovered genuses in the current trip.
    /// </summary>
    /// <value>The trip first discovery genus count string.</value>
    public string TripGenusFirstDiscoveryCount { get; }

    /// <summary>
    /// Gets the sum of Vista Genomics values for genuses.
    /// </summary>
    /// <value>The Vista Genomics value sum string.</value>
    public string GenusVistaGenomicsValueSum { get; }

    /// <summary>
    /// Gets the sum of Vista Genomics values for genuses in the current trip.
    /// </summary>
    /// <value>The trip Vista Genomics value sum string.</value>
    public string TripGenusVistaGenomicsValueSum { get; }

    /// <summary>
    /// Gets the most frequent genus species.
    /// </summary>
    /// <value>The most frequent species string.</value>
    public string GenusMostFrequentSpecies { get; }

    /// <summary>
    /// Gets the most frequent genus species in the current trip.
    /// </summary>
    /// <value>The trip most frequent species string.</value>
    public string TripGenusMostFrequentSpecies { get; }

    /// <summary>
    /// Gets the rarest genus species.
    /// </summary>
    /// <value>The rarest species string.</value>
    public string GenusRarestSpecies { get; }

    /// <summary>
    /// Gets the rarest genus species in the current trip.
    /// </summary>
    /// <value>The trip rarest species string.</value>
    public string TripGenusRarestSpecies { get; }

    /// <summary>
    /// Gets the sum of total exploration data values.
    /// </summary>
    /// <value>The total exploration data value sum string.</value>
    public string TotalExplorationDataValueSum { get; }

    /// <summary>
    /// Gets the sum of total exploration data values in the current trip.
    /// </summary>
    /// <value>The trip total exploration data value sum string.</value>
    public string TripTotalExplorationDataValueSum { get; }

    /// <summary>
    /// Gets the sum of cartographic values for bodies.
    /// </summary>
    /// <value>The cartographic value sum string.</value>
    public string BodyCartographicValueSum { get; }

    /// <summary>
    /// Gets the sum of cartographic values for bodies in the current trip.
    /// </summary>
    /// <value>The trip cartographic value sum string.</value>
    public string TripBodyCartographicValueSum { get; }

    /// <summary>
    /// Gets the sum of cartographic bonus values for bodies.
    /// </summary>
    /// <value>The bonus value sum string.</value>
    public string BodyCartographicBonusValueSum { get; }

    /// <summary>
    /// Gets the sum of cartographic bonus values for bodies in the current trip.
    /// </summary>
    /// <value>The trip bonus value sum string.</value>
    public string TripBodyCartographicBonusValueSum { get; }

    /// <summary>
    /// Gets the sum of Vista Genomics base values for genuses.
    /// </summary>
    /// <value>The base value sum string.</value>
    public string GenusVistaGenomicsBaseValueSum { get; }

    /// <summary>
    /// Gets the sum of Vista Genomics base values for genuses in the current trip.
    /// </summary>
    /// <value>The trip base value sum string.</value>
    public string TripGenusVistaGenomicsBaseValueSum { get; }

    /// <summary>
    /// Gets the sum of Vista Genomics first discovery bonus values for genuses.
    /// </summary>
    /// <value>The first discovery bonus sum string.</value>
    public string GenusVistaGenomicsFirstDiscoveryBonusValueSum { get; }

    /// <summary>
    /// Gets the sum of Vista Genomics first discovery bonus values for genuses in the current trip.
    /// </summary>
    /// <value>The trip first discovery bonus sum string.</value>
    public string TripGenusVistaGenomicsFirstDiscoveryBonusValueSum { get; }

    /// <summary>
    /// Gets the total number of rings.
    /// </summary>
    /// <value>The ring count string.</value>
    public string RingCount { get; }

    /// <summary>
    /// Gets the number of rings in the current trip.
    /// </summary>
    /// <value>The trip ring count string.</value>
    public string TripRingCount { get; }

    /// <summary>
    /// Gets the number of first-discovered rings.
    /// </summary>
    /// <value>The first discovery ring count string.</value>
    public string RingFirstDiscoveryCount { get; }

    /// <summary>
    /// Gets the number of first-discovered rings in the current trip.
    /// </summary>
    /// <value>The trip first discovery ring count string.</value>
    public string TripRingFirstDiscoveryCount { get; }

    /// <summary>
    /// Gets the most frequent ring type.
    /// </summary>
    /// <value>The most frequent ring type string.</value>
    public string RingMostFrequentType { get; }

    /// <summary>
    /// Gets the most frequent ring type in the current trip.
    /// </summary>
    /// <value>The trip most frequent ring type string.</value>
    public string TripRingMostFrequentType { get; }

    /// <summary>
    /// Gets the rarest ring type.
    /// </summary>
    /// <value>The rarest ring type string.</value>
    public string RingRarestType { get; }

    /// <summary>
    /// Gets the rarest ring type in the current trip.
    /// </summary>
    /// <value>The trip rarest ring type string.</value>
    public string TripRingRarestType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryDataViewModel"/> class.
    /// </summary>
    /// <param name="historyData">The history data model.</param>
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
