using System;

namespace EDEA.Models;

public class Star : Body
{
    public string StarType { get; set; }

    public Star()
    {
        StarType = string.Empty;
    }

    public Star(int id, long starSystemId, string name, double distance, string starType, double radius, double mass, double? orbitalInclination)
        : base(id, starSystemId, name, distance, radius, mass, orbitalInclination)
    {
        StarType = starType ?? string.Empty;
        base.Type = BodyType.Star;
    }

    public Star(long id, long starSystemId, string name, long type, double distance, long wasDiscovered, long wasMapped, long? wasFootfalled, long wasReadFromJournal, long wasReadFromEdsm, string edsmDiscoveryCommander, string planetClass, long isLandable, string terraformingState, long surfaceScanned, double gravity, long geologicalCount, long biologicalCount, string starType, double surfaceTemperature, long touchdown, string volcanism, string atmosphere, double radius, long? parentStarId, long? parentPlanetId, double mass, double? orbitalInclination, long efficientlyScanned, long cartographicValue, long cartographicMaxValue, long cartographicBaseValue, long cartographicFirstDiscoveryBonusValue, long cartographicSurfaceScanValue, long cartographicFirstSurfaceScanBonusValue, long cartographicEfficientlyScannedBonusValue, long cartographicFirstDiscoveryBonusWithoutEfficiencyValue, long cartographicFirstDiscoveryBonusWithoutSurfaceScanValue, long ringsReserveLevel)
        : this(Convert.ToInt32(id), starSystemId, name, distance, starType, radius, mass, orbitalInclination)
    {
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

    public void UpdateStar(Star star, DataSource dataSource)
    {
        UpdateBody(star, dataSource);
        if (dataSource != DataSource.Edsm || !base.WasReadFromJournal)
        {
            StarType = star.StarType;
        }
    }
}
