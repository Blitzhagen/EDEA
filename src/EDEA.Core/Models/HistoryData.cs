namespace EDEA.Models;

/// <summary>
/// Aggregates exploration and biological statistics for a history session.
/// </summary>
public class HistoryData
{
    /// <summary>
    /// Gets or sets the total number of visited star systems.
    /// </summary>
    /// <value>The total system count.</value>
    public int SystemCount { get; set; }

    /// <summary>
    /// Gets or sets the number of visited star systems for the current trip.
    /// </summary>
    /// <value>The trip system count.</value>
    public int TripSystemCount { get; set; }

    /// <summary>
    /// Gets or sets the number of first discovered star systems.
    /// </summary>
    /// <value>The first discovery system count.</value>
    public int SystemFirstDiscoveryCount { get; set; }

    /// <summary>
    /// Gets or sets the number of first discovered star systems for the current trip.
    /// </summary>
    /// <value>The trip first discovery system count.</value>
    public int TripSystemFirstDiscoveryCount { get; set; }

    /// <summary>
    /// Gets or sets the sum of base cartographic values for all bodies.
    /// </summary>
    /// <value>The base cartographic value sum.</value>
    public long BodyCartographicBaseValueSum { get; set; }

    /// <summary>
    /// Gets or sets the sum of base cartographic values for the current trip.
    /// </summary>
    /// <value>The trip base cartographic value sum.</value>
    public long TripBodyCartographicBaseValueSum { get; set; }

    /// <summary>
    /// Gets or sets the star class statistics for all systems.
    /// </summary>
    /// <value>The star class statistics.</value>
    public HistoryStatistics SystemStarClassStatistics { get; set; }

    /// <summary>
    /// Gets or sets the star class statistics for the current trip.
    /// </summary>
    /// <value>The trip star class statistics.</value>
    public HistoryStatistics TripSystemStarClassStatistics { get; set; }

    /// <summary>
    /// Gets or sets the total number of body signals.
    /// </summary>
    /// <value>The body signal sum.</value>
    public int BodySignalSum { get; set; }

    /// <summary>
    /// Gets or sets the number of body signals for the current trip.
    /// </summary>
    /// <value>The trip body signal sum.</value>
    public int TripBodySignalSum { get; set; }

    /// <summary>
    /// Gets or sets the total body count.
    /// </summary>
    /// <value>The body count.</value>
    public int BodyCount { get; set; }

    /// <summary>
    /// Gets or sets the body count for the current trip.
    /// </summary>
    /// <value>The trip body count.</value>
    public int TripBodyCount { get; set; }

    /// <summary>
    /// Gets or sets the number of first discovered bodies.
    /// </summary>
    /// <value>The first discovery body count.</value>
    public int BodyFirstDiscoveryCount { get; set; }

    /// <summary>
    /// Gets or sets the number of first discovered bodies for the current trip.
    /// </summary>
    /// <value>The trip first discovery body count.</value>
    public int TripBodyFirstDiscoveryCount { get; set; }

    /// <summary>
    /// Gets or sets the number of terraformable bodies.
    /// </summary>
    /// <value>The terraformable body count.</value>
    public int BodyTerraformableCount { get; set; }

    /// <summary>
    /// Gets or sets the number of terraformable bodies for the current trip.
    /// </summary>
    /// <value>The trip terraformable body count.</value>
    public int TripBodyTerraformableCount { get; set; }

    /// <summary>
    /// Gets or sets the number of valuable bodies.
    /// </summary>
    /// <value>The valuable body count.</value>
    public int BodyValuableCount { get; set; }

    /// <summary>
    /// Gets or sets the number of valuable bodies for the current trip.
    /// </summary>
    /// <value>The trip valuable body count.</value>
    public int TripBodyValuableCount { get; set; }

    /// <summary>
    /// Gets or sets the number of surface scanned bodies.
    /// </summary>
    /// <value>The surface scan body count.</value>
    public int BodySurfaceScanCount { get; set; }

    /// <summary>
    /// Gets or sets the number of surface scanned bodies for the current trip.
    /// </summary>
    /// <value>The trip surface scan body count.</value>
    public int TripBodySurfaceScanCount { get; set; }

    /// <summary>
    /// Gets or sets the sum of surface scan cartographic values for all bodies.
    /// </summary>
    /// <value>The surface scan value sum.</value>
    public long BodyCartographicSurfaceScanValueSum { get; set; }

    /// <summary>
    /// Gets or sets the sum of surface scan cartographic values for the current trip.
    /// </summary>
    /// <value>The trip surface scan value sum.</value>
    public long TripBodyCartographicSurfaceScanValueSum { get; set; }

    /// <summary>
    /// Gets or sets the total number of touchdowns.
    /// </summary>
    /// <value>The touchdown count.</value>
    public int BodyTouchdownCount { get; set; }

    /// <summary>
    /// Gets or sets the number of touchdowns for the current trip.
    /// </summary>
    /// <value>The trip touchdown count.</value>
    public int TripBodyTouchdownCount { get; set; }

    /// <summary>
    /// Gets or sets the number of ringed bodies.
    /// </summary>
    /// <value>The ringed body count.</value>
    public int BodyRingBodyCount { get; set; }

    /// <summary>
    /// Gets or sets the number of ringed bodies for the current trip.
    /// </summary>
    /// <value>The trip ringed body count.</value>
    public int TripBodyRingBodyCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of genus signals.
    /// </summary>
    /// <value>The genus signal sum.</value>
    public int GenusSignalSum { get; set; }

    /// <summary>
    /// Gets or sets the number of genus signals for the current trip.
    /// </summary>
    /// <value>The trip genus signal sum.</value>
    public int TripGenusSignalSum { get; set; }

    /// <summary>
    /// Gets or sets the total genus count.
    /// </summary>
    /// <value>The genus count.</value>
    public int GenusCount { get; set; }

    /// <summary>
    /// Gets or sets the genus count for the current trip.
    /// </summary>
    /// <value>The trip genus count.</value>
    public int TripGenusCount { get; set; }

    /// <summary>
    /// Gets or sets the number of completely analysed genus entries.
    /// </summary>
    /// <value>The analysis complete genus count.</value>
    public int GenusAnalysisCompleteCount { get; set; }

    /// <summary>
    /// Gets or sets the number of completely analysed genus entries for the current trip.
    /// </summary>
    /// <value>The trip analysis complete genus count.</value>
    public int TripGenusAnalysisCompleteCount { get; set; }

    /// <summary>
    /// Gets or sets the sum of Vista Genomics values for all genus entries.
    /// </summary>
    /// <value>The Vista Genomics value sum.</value>
    public long GenusVistaGenomicsValueSum { get; set; }

    /// <summary>
    /// Gets or sets the sum of Vista Genomics values for the current trip.
    /// </summary>
    /// <value>The trip Vista Genomics value sum.</value>
    public long TripGenusVistaGenomicsValueSum { get; set; }

    /// <summary>
    /// Gets or sets the species statistics for all genus entries.
    /// </summary>
    /// <value>The species statistics.</value>
    public HistoryStatistics GenusSpeciesStatistics { get; set; }

    /// <summary>
    /// Gets or sets the species statistics for the current trip.
    /// </summary>
    /// <value>The trip species statistics.</value>
    public HistoryStatistics TripGenusSpeciesStatistics { get; set; }

    /// <summary>
    /// Gets or sets the sum of cartographic values for all bodies.
    /// </summary>
    /// <value>The cartographic value sum.</value>
    public long BodyCartographicValueSum { get; set; }

    /// <summary>
    /// Gets or sets the sum of cartographic values for the current trip.
    /// </summary>
    /// <value>The trip cartographic value sum.</value>
    public long TripBodyCartographicValueSum { get; set; }

    /// <summary>
    /// Gets or sets the sum of base Vista Genomics values for all genus entries.
    /// </summary>
    /// <value>The base Vista Genomics value sum.</value>
    public long GenusVistaGenomicsBaseValueSum { get; set; }

    /// <summary>
    /// Gets or sets the sum of base Vista Genomics values for the current trip.
    /// </summary>
    /// <value>The trip base Vista Genomics value sum.</value>
    public long TripGenusVistaGenomicsBaseValueSum { get; set; }

    /// <summary>
    /// Gets or sets the number of first discovered genus entries.
    /// </summary>
    /// <value>The first discovery genus count.</value>
    public int GenusFirstDiscoveryCount { get; set; }

    /// <summary>
    /// Gets or sets the number of first discovered genus entries for the current trip.
    /// </summary>
    /// <value>The trip first discovery genus count.</value>
    public int TripGenusFirstDiscoveryCount { get; set; }

    /// <summary>
    /// Gets or sets the total ring count.
    /// </summary>
    /// <value>The ring count.</value>
    public int RingCount { get; set; }

    /// <summary>
    /// Gets or sets the ring count for the current trip.
    /// </summary>
    /// <value>The trip ring count.</value>
    public int TripRingCount { get; set; }

    /// <summary>
    /// Gets or sets the number of first discovered rings.
    /// </summary>
    /// <value>The first discovery ring count.</value>
    public int RingFirstDiscoveryCount { get; set; }

    /// <summary>
    /// Gets or sets the number of first discovered rings for the current trip.
    /// </summary>
    /// <value>The trip first discovery ring count.</value>
    public int TripRingFirstDiscoveryCount { get; set; }

    /// <summary>
    /// Gets or sets the ring type statistics for all rings.
    /// </summary>
    /// <value>The ring type statistics.</value>
    public HistoryStatistics RingTypeStatistics { get; set; }

    /// <summary>
    /// Gets or sets the ring type statistics for the current trip.
    /// </summary>
    /// <value>The trip ring type statistics.</value>
    public HistoryStatistics TripRingTypeStatistics { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryData"/> class.
    /// </summary>
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
