using System;

namespace EDEA.Models;

/// <summary>
/// Represents a star in a star system.
/// </summary>
public class Star : Body
{
    /// <summary>
    /// Gets or sets the star type.
    /// </summary>
    /// <value>The star type.</value>
    public string StarType { get; set; }

    /// <summary>
    /// Gets or sets the star luminosity class (e.g. "V", "Va", "IV").
    /// </summary>
    /// <value>The journal luminosity value.</value>
    public string Luminosity { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Star"/> class.
    /// </summary>
    public Star()
    {
        StarType = string.Empty;
        Luminosity = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Star"/> class.
    /// </summary>
    /// <param name="id">The body identifier.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="name">The star name.</param>
    /// <param name="distance">The distance from the arrival point.</param>
    /// <param name="starType">The star type.</param>
    /// <param name="radius">The star radius.</param>
    /// <param name="mass">The star mass.</param>
    /// <param name="orbitalInclination">The orbital inclination, or <see langword="null"/> if not specified.</param>
    public Star(int id, long starSystemId, string name, double distance, string starType, double radius, double mass, double? orbitalInclination)
        : base(id, starSystemId, name, distance, radius, mass, orbitalInclination)
    {
        StarType = starType ?? string.Empty;
        Luminosity = string.Empty;
        base.Type = BodyType.Star;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Star"/> class from persisted data.
    /// </summary>
    /// <param name="id">The body identifier.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="name">The star name.</param>
    /// <param name="type">The body type.</param>
    /// <param name="distance">The distance from the arrival point.</param>
    /// <param name="wasDiscovered">Whether the star was already discovered.</param>
    /// <param name="wasMapped">Whether the star was mapped.</param>
    /// <param name="wasFootfalled">Whether the star was footfalled, or <see langword="null"/> if unspecified.</param>
    /// <param name="wasReadFromJournal">Whether the star was read from a journal.</param>
    /// <param name="wasReadFromEdsm">Whether the star was read from EDSM.</param>
    /// <param name="edsmDiscoveryCommander">The EDSM discovery commander.</param>
    /// <param name="planetClass">The planet class.</param>
    /// <param name="isLandable">Whether the star is landable.</param>
    /// <param name="terraformingState">The terraforming state.</param>
    /// <param name="surfaceScanned">Whether the star was surface scanned.</param>
    /// <param name="gravity">The gravity.</param>
    /// <param name="geologicalCount">The number of geological signals.</param>
    /// <param name="biologicalCount">The number of biological signals.</param>
    /// <param name="starType">The star type.</param>
    /// <param name="surfaceTemperature">The surface temperature.</param>
    /// <param name="touchdown">Whether touchdown occurred.</param>
    /// <param name="volcanism">The volcanism description.</param>
    /// <param name="atmosphere">The atmosphere description.</param>
    /// <param name="radius">The radius.</param>
    /// <param name="parentStarId">The parent star identifier, or <see langword="null"/> if not specified.</param>
    /// <param name="parentPlanetId">The parent planet identifier, or <see langword="null"/> if not specified.</param>
    /// <param name="mass">The mass.</param>
    /// <param name="orbitalInclination">The orbital inclination, or <see langword="null"/> if not specified.</param>
    /// <param name="efficientlyScanned">Whether the star was efficiently scanned.</param>
    /// <param name="cartographicValue">The cartographic value.</param>
    /// <param name="cartographicMaxValue">The maximum cartographic value.</param>
    /// <param name="cartographicBaseValue">The base cartographic value.</param>
    /// <param name="cartographicFirstDiscoveryBonusValue">The first discovery bonus value.</param>
    /// <param name="cartographicSurfaceScanValue">The surface scan value.</param>
    /// <param name="cartographicFirstSurfaceScanBonusValue">The first surface scan bonus value.</param>
    /// <param name="cartographicEfficientlyScannedBonusValue">The efficiently scanned bonus value.</param>
    /// <param name="cartographicFirstDiscoveryBonusWithoutEfficiencyValue">The first discovery bonus without efficiency value.</param>
    /// <param name="cartographicFirstDiscoveryBonusWithoutSurfaceScanValue">The first discovery bonus without surface scan value.</param>
    /// <param name="ringsReserveLevel">The rings reserve level.</param>
    /// <param name="luminosity">The star luminosity class.</param>
    public Star(long id, long starSystemId, string name, long type, double distance, long wasDiscovered, long wasMapped, long? wasFootfalled, long wasReadFromJournal, long wasReadFromEdsm, string edsmDiscoveryCommander, string planetClass, long isLandable, string terraformingState, long surfaceScanned, double gravity, long geologicalCount, long biologicalCount, string starType, double surfaceTemperature, long touchdown, string volcanism, string atmosphere, double radius, long? parentStarId, long? parentPlanetId, double mass, double? orbitalInclination, long efficientlyScanned, long cartographicValue, long cartographicMaxValue, long cartographicBaseValue, long cartographicFirstDiscoveryBonusValue, long cartographicSurfaceScanValue, long cartographicFirstSurfaceScanBonusValue, long cartographicEfficientlyScannedBonusValue, long cartographicFirstDiscoveryBonusWithoutEfficiencyValue, long cartographicFirstDiscoveryBonusWithoutSurfaceScanValue, long ringsReserveLevel, string? luminosity)
        : this(Convert.ToInt32(id), starSystemId, name, distance, starType, radius, mass, orbitalInclination)
    {
        Luminosity = luminosity ?? string.Empty;
        base.WasDiscovered = Convert.ToBoolean(wasDiscovered);
        base.Type = (BodyType)type;
        base.WasReadFromJournal = Convert.ToBoolean(wasReadFromJournal);
        base.WasReadFromEdsm = Convert.ToBoolean(wasReadFromEdsm);
        base.EdsmDiscoveryCommander = edsmDiscoveryCommander;
        base.CartographicValue = Convert.ToInt32(cartographicValue);
        base.CartographicMaxValue = Convert.ToInt32(cartographicMaxValue);
        base.CartographicBaseValue = Convert.ToInt32(cartographicBaseValue);
        base.CartographicFirstDiscoveryBonusValue = Convert.ToInt32(cartographicFirstDiscoveryBonusValue);
        base.CartographicFirstDiscoveryBonusWithoutEfficiencyValue = Convert.ToInt32(cartographicFirstDiscoveryBonusWithoutEfficiencyValue);
        base.CartographicFirstDiscoveryBonusWithoutSurfaceScanValue = Convert.ToInt32(cartographicFirstDiscoveryBonusWithoutSurfaceScanValue);
        base.RingsReserveLevel = (RingReserveLevel)ringsReserveLevel;
    }

    /// <summary>
    /// Updates this star with data from the specified star and source.
    /// </summary>
    /// <param name="star">The star to copy data from.</param>
    /// <param name="dataSource">The data source that provided the new data.</param>
    public void UpdateStar(Star star, DataSource dataSource)
    {
        UpdateBody(star, dataSource);
        if (dataSource != DataSource.Edsm || !base.WasReadFromJournal)
        {
            StarType = star.StarType;
            if (!string.IsNullOrEmpty(star.Luminosity))
            {
                Luminosity = star.Luminosity;
            }
        }
    }
}
