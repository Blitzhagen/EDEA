using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EDEA;
using EDEA.Models;
using EDEA.Services;
using log4net;

namespace EDEA.Models;

public class Body
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Body));

    private readonly ConcurrentDictionary<string, Ring> _rings;

    // EDEA-only: retained for SQLiteStore body primary key persistence
    public long Id64 { get; set; }

    public int Id { get; protected set; }

    // EDEA-only: alias retained for SQLiteStore column mapping
    public int BodyId { get => Id; set => Id = value; }

    public long StarSystemId { get; set; }

    // EDEA-only: alias retained for SQLiteStore column mapping
    public long SystemId64 { get => StarSystemId; set => StarSystemId = value; }

    public string Name { get; protected set; }

    public string ShortName
    {
        get
        {
            if (StarSystem != null)
            {
                string shortName = Name.Replace(StarSystem.Name, "").Trim();
                if (!string.IsNullOrEmpty(shortName))
                {
                    return shortName;
                }
            }
            return "⁕";
        }
    }

    public BodyType Type { get; set; }

    public bool IsPlanet => Type == BodyType.Planet;

    public bool IsStar => Type == BodyType.Star;

    public bool IsPlanetOrStar => IsPlanet || IsStar;

    public double Distance { get; set; }

    public bool WasDiscovered { get; set; }

    public bool WasReadFromJournal { get; set; }

    public bool WasReadFromEdsm { get; set; }

    public bool WasReadFromEdsmOnly => WasReadFromEdsm && !WasReadFromJournal;

    public string? EdsmDiscoveryCommander { get; set; }

    // EDEA-only: alias retained for SQLiteStore column mapping
    public string? EdsmDiscoverer
    {
        get => EdsmDiscoveryCommander;
        set => EdsmDiscoveryCommander = value;
    }

    public StarSystem? StarSystem { get; set; }

    public double Radius { get; set; }

    public double Mass { get; set; }

    public int CartographicValue { get; set; }

    public int CartographicMaxValue { get; set; }

    public int CartographicBaseValue { get; set; }

    public int CartographicFirstDiscoveryBonusValue { get; set; }

    public int CartographicFirstDiscoveryBonusWithoutEfficiencyValue { get; set; }

    public int CartographicFirstDiscoveryBonusWithoutSurfaceScanValue { get; set; }

    public IReadOnlyDictionary<string, Ring> Rings => _rings;

    public bool HasRings => _rings.Count > 0;

    public RingReserveLevel RingsReserveLevel { get; set; }

    public long RingsTotalWidth => _rings.Sum((KeyValuePair<string, Ring> ring) => ring.Value.Width);

    public double? OrbitalInclination { get; set; }

    // EDEA-only: persisted by SQLiteStore for history statistics
    public bool HasBiological { get; set; }

    // EDEA-only: persisted by SQLiteStore for history statistics
    public bool HasGeological { get; set; }

    // EDEA-only: persisted by SQLiteStore for history statistics
    public bool IsValuable { get; set; }

    public Body()
    {
        _rings = new ConcurrentDictionary<string, Ring>();
        Name = string.Empty;
    }

    public Body(int id, long starSystemId, string name, double distance, double radius, double mass, double? orbitalInclination)
    {
        _rings = new ConcurrentDictionary<string, Ring>();
        Id = id;
        StarSystemId = starSystemId;
        Id64 = starSystemId * 1000 + id;
        Name = name;
        Distance = distance;
        WasDiscovered = true;
        Type = BodyType.Unknown;
        Radius = radius;
        Mass = mass;
        OrbitalInclination = orbitalInclination;
        RingsReserveLevel = RingReserveLevel.Unknown;
    }

    public Body(long id, long starSystemId, string name, long type, double distance, long wasDiscovered, long wasMapped, long? wasFootfalled, long wasReadFromJournal, long wasReadFromEdsm, string edsmDiscoveryCommander, string planetClass, long isLandable, string terraformingState, long surfaceScanned, double gravity, long geologicalCount, long biologicalCount, string starType, double surfaceTemperature, long touchdown, string volcanism, string atmosphere, double radius, long? parentStarId, long? parentPlanetId, double mass, double? orbitalInclination, long efficientlyScanned, long cartographicValue, long cartographicMaxValue, long cartographicBaseValue, long cartographicFirstDiscoveryBonusValue, long cartographicSurfaceScanValue, long cartographicFirstSurfaceScanBonusValue, long cartographicEfficientlyScannedBonusValue, long cartographicFirstDiscoveryBonusWithoutEfficiencyValue, long cartographicFirstDiscoveryBonusWithoutSurfaceScanValue, long ringsReserveLevel)
        : this(Convert.ToInt32(id), starSystemId, name, distance, radius, mass, orbitalInclination)
    {
        WasDiscovered = Convert.ToBoolean(wasDiscovered);
        Type = (BodyType)type;
        WasReadFromJournal = Convert.ToBoolean(wasReadFromJournal);
        WasReadFromEdsm = Convert.ToBoolean(wasReadFromEdsm);
        EdsmDiscoveryCommander = edsmDiscoveryCommander;
        CartographicValue = Convert.ToInt32(cartographicValue);
        CartographicMaxValue = Convert.ToInt32(cartographicMaxValue);
        CartographicBaseValue = Convert.ToInt32(cartographicBaseValue);
        CartographicFirstDiscoveryBonusValue = Convert.ToInt32(cartographicFirstDiscoveryBonusValue);
        CartographicFirstDiscoveryBonusWithoutEfficiencyValue = Convert.ToInt32(cartographicFirstDiscoveryBonusWithoutEfficiencyValue);
        CartographicFirstDiscoveryBonusWithoutSurfaceScanValue = Convert.ToInt32(cartographicFirstDiscoveryBonusWithoutSurfaceScanValue);
        RingsReserveLevel = (RingReserveLevel)ringsReserveLevel;
    }

    public void UpdateBody(Body body, DataSource dataSource)
    {
        switch (dataSource)
        {
            case DataSource.Journal:
                WasReadFromJournal = true;
                break;
            case DataSource.Edsm:
                EdsmDiscoveryCommander = body.EdsmDiscoveryCommander;
                WasReadFromEdsm = true;
                break;
        }
        if (dataSource != DataSource.Edsm || !WasReadFromJournal)
        {
            Name = body.Name;
            Distance = body.Distance;
            WasDiscovered = body.WasDiscovered;
            StarSystem = body.StarSystem;
            Radius = body.Radius;
            Mass = body.Mass;
            OrbitalInclination = body.OrbitalInclination;
            CartographicValue = body.CartographicValue;
            CartographicMaxValue = body.CartographicMaxValue;
            CartographicBaseValue = body.CartographicBaseValue;
            CartographicFirstDiscoveryBonusValue = body.CartographicFirstDiscoveryBonusValue;
            CartographicFirstDiscoveryBonusWithoutEfficiencyValue = body.CartographicFirstDiscoveryBonusWithoutEfficiencyValue;
            CartographicFirstDiscoveryBonusWithoutSurfaceScanValue = body.CartographicFirstDiscoveryBonusWithoutSurfaceScanValue;
            RingsReserveLevel = body.RingsReserveLevel;
        }
    }

    public void TryAddOrUpdateRing(Ring ring, DataSource dataSource)
    {
        if (ring.BodyId != Id || ring.StarSystemId != StarSystemId)
        {
            log.Warn($"Ring '{ring.Name}' was not added or updated on planet '{Name}' ({Id}) because it belongs to planet id {ring.BodyId} / star system id {ring.StarSystemId}");
        }
        else if (_rings.TryAdd(ring.Name, ring))
        {
            log.Info($"New ring {_rings[ring.Name].Name} added to planet '{Name}' ({Id})");
        }
        else
        {
            log.Debug($"Ring {ring.Name} already exits at planet '{Name}' ({Id}) - was not added");
        }
    }

    public void CalculateCartographicValue(bool skipSpeechOutput)
    {
        if (!IsPlanetOrStar)
        {
            return;
        }
        log.Debug("Calculating cartographic value for " + Name + " ...");
        bool wasUnvalued = CartographicMaxValue == 0;
        double baseValue = 0.0;
        double firstDiscoveryBonus = 0.0;
        double firstDiscoveryBonusWithoutEfficiency = 0.0;
        double firstDiscoveryBonusWithoutSurfaceScan = 0.0;
        double surfaceScanValue = 0.0;
        double firstSurfaceScanBonusValue = 0.0;
        double efficientlyScannedBonusValue = 0.0;
        calculateBaseValue(ref baseValue);
        calculateAchievableSurfaceScanAndBonusValues(ref baseValue, ref surfaceScanValue, ref firstSurfaceScanBonusValue, ref efficientlyScannedBonusValue, out var surfaceScanPlanet);
        if (!WasDiscovered)
        {
            firstDiscoveryBonus = Math.Max(baseValue + surfaceScanValue + firstSurfaceScanBonusValue + efficientlyScannedBonusValue, 500.0) * 1.6;
            firstDiscoveryBonusWithoutEfficiency = Math.Max(baseValue + surfaceScanValue + firstSurfaceScanBonusValue, 500.0) * 1.6;
            firstDiscoveryBonusWithoutSurfaceScan = Math.Max(baseValue, 500.0) * 1.6;
        }
        CartographicBaseValue = Convert.ToInt32(baseValue);
        CartographicFirstDiscoveryBonusValue = Convert.ToInt32(firstDiscoveryBonus);
        CartographicFirstDiscoveryBonusWithoutEfficiencyValue = Convert.ToInt32(firstDiscoveryBonusWithoutEfficiency);
        CartographicFirstDiscoveryBonusWithoutSurfaceScanValue = Convert.ToInt32(firstDiscoveryBonusWithoutSurfaceScan);
        CartographicMaxValue = CartographicBaseValue + CartographicFirstDiscoveryBonusValue;
        if (surfaceScanPlanet != null)
        {
            surfaceScanPlanet.CartographicSurfaceScanValue = Convert.ToInt32(surfaceScanValue);
            surfaceScanPlanet.CartographicFirstSurfaceScanBonusValue = Convert.ToInt32(firstSurfaceScanBonusValue);
            surfaceScanPlanet.CartographicEfficientlyScannedBonusValue = Convert.ToInt32(efficientlyScannedBonusValue);
            CartographicMaxValue += surfaceScanPlanet.CartographicSurfaceScanValue + surfaceScanPlanet.CartographicFirstSurfaceScanBonusValue + surfaceScanPlanet.CartographicEfficientlyScannedBonusValue;
        }
        CartographicValue = 0;
        if (!WasReadFromEdsmOnly)
        {
            CartographicValue = CartographicBaseValue;
            if (surfaceScanPlanet != null && surfaceScanPlanet.SurfaceScanned)
            {
                CartographicValue += surfaceScanPlanet.CartographicSurfaceScanValue + surfaceScanPlanet.CartographicFirstSurfaceScanBonusValue;
                if (surfaceScanPlanet.EfficientlyScanned)
                {
                    CartographicValue += surfaceScanPlanet.CartographicEfficientlyScannedBonusValue;
                }
            }
            if (CartographicValue < 500)
            {
                CartographicValue = 500;
            }
            if (surfaceScanPlanet != null && surfaceScanPlanet.SurfaceScanned)
            {
                if (surfaceScanPlanet.EfficientlyScanned)
                {
                    CartographicValue += CartographicFirstDiscoveryBonusValue;
                }
                else
                {
                    CartographicValue += CartographicFirstDiscoveryBonusWithoutEfficiencyValue;
                }
            }
            else
            {
                CartographicValue += CartographicFirstDiscoveryBonusWithoutSurfaceScanValue;
            }
        }
        if (wasUnvalued && !skipSpeechOutput && CartographicMaxValue >= Preferences.Other.ValuableBodyThreshold)
        {
            SpeechProvider.SpeakValuableBody(new SpeechOutputBody(this), new SpeechOutputCartographicValues(CartographicMaxValue, CartographicValue));
        }
    }

    private void calculateAchievableSurfaceScanAndBonusValues(ref double baseValue, ref double surfaceScanValue, ref double firstSurfaceScanBonusValue, ref double efficientlyScannedBonusValue, out Planet? surfaceScanPlanet)
    {
        if (!IsPlanet)
        {
            surfaceScanValue = 0.0;
            firstSurfaceScanBonusValue = 0.0;
            surfaceScanPlanet = null;
            return;
        }
        surfaceScanPlanet = (Planet)this;
        Planet planet = surfaceScanPlanet;
        bool firstDiscoveredAndMapped = !planet.WasDiscovered && planet.WasMapped;
        const double BonusMinimum = 555.0;
        double surfaceScanBase = baseValue * 2.3333333333;
        double unmappedDiscoveredBonus = baseValue * 7.095599999999999;
        double unmappedUndiscoveredBonus = baseValue * 2.699622554;
        double firstDiscoveryBonus = 0.0;
        if (!planet.WasMapped && !planet.WasDiscovered)
        {
            firstDiscoveryBonus = unmappedUndiscoveredBonus - surfaceScanBase;
        }
        else if (!planet.WasMapped)
        {
            firstDiscoveryBonus = unmappedDiscoveredBonus - surfaceScanBase;
        }
        double fullSurfaceScanBase = surfaceScanBase + firstDiscoveryBonus;
        double additionalBonus = 0.0;
        if (!firstDiscoveredAndMapped)
        {
            additionalBonus = (baseValue + fullSurfaceScanBase) * 0.3;
            if (additionalBonus < BonusMinimum)
            {
                additionalBonus = BonusMinimum;
            }
        }
        surfaceScanValue = surfaceScanBase + additionalBonus / fullSurfaceScanBase * surfaceScanBase;
        firstSurfaceScanBonusValue = firstDiscoveryBonus + additionalBonus / fullSurfaceScanBase * firstDiscoveryBonus;
        efficientlyScannedBonusValue = (baseValue + surfaceScanValue + firstSurfaceScanBonusValue) * 0.25;
    }

    private void calculateBaseValue(ref double baseValue)
    {
        int baseMultiplier = 0;
        double mass = 0.0;
        if (Type == BodyType.Star)
        {
            baseMultiplier = 1200;
            Star star = (Star)this;
            mass = star.Mass;
            if (new List<string> { "Neutron Star", "N", "Black Hole", "H" }.Contains(star.StarType))
            {
                baseMultiplier = 22628;
            }
            else if (new List<string>
            {
                "White Dwarf (D) Star", "D", "White Dwarf (DA) Star", "DA", "White Dwarf (DAB) Star", "DAB", "White Dwarf (DAO) Star", "DAO", "White Dwarf (DAV) Star", "DAV",
                "White Dwarf (DAZ) Star", "DAZ", "White Dwarf (DB) Star", "DB", "White Dwarf (DBV) Star", "DBV", "White Dwarf (DBZ) Star", "DBZ", "White Dwarf (DC) Star", "DC",
                "White Dwarf (DCV) Star", "DCV", "White Dwarf (DQ) Star", "DQ", "White Dwarf (DX) Star", "DX"
            }.Contains(star.StarType))
            {
                baseMultiplier = 14057;
            }
        }
        else if (Type == BodyType.Planet)
        {
            Planet planet = (Planet)this;
            mass = planet.Mass;
            if (new List<string> { "Metal-rich body", "Metal rich body" }.Contains(planet.PlanetClass))
            {
                baseMultiplier = 21790;
            }
            else if (new List<string> { "Ammonia world", "Ammonia world" }.Contains(planet.PlanetClass))
            {
                baseMultiplier = 96932;
            }
            else if (new List<string> { "Class I gas giant", "Sudarsky class I gas giant" }.Contains(planet.PlanetClass))
            {
                baseMultiplier = 1656;
            }
            else if (new List<string> { "Class II gas giant", "Sudarsky class II gas giant", "High metal content world", "High metal content body" }.Contains(planet.PlanetClass))
            {
                baseMultiplier = 9654;
                if (planet.IsTerraformable)
                {
                    baseMultiplier += 100677;
                }
            }
            else if (new List<string> { "Water world", "Water world" }.Contains(planet.PlanetClass))
            {
                baseMultiplier = 64831;
                if (planet.IsTerraformable)
                {
                    baseMultiplier += 116295;
                }
            }
            else if (new List<string> { "Earth-like world", "Earthlike body" }.Contains(planet.PlanetClass))
            {
                baseMultiplier = 181126;
            }
            else
            {
                baseMultiplier = 300;
                if (planet.IsTerraformable)
                {
                    baseMultiplier += 93328;
                }
            }
        }
        baseValue = (double)baseMultiplier + (double)baseMultiplier * 0.56591828 * Math.Pow(mass, 0.2);
    }
}
