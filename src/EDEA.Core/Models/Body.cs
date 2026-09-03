using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EDEA;
using EDEA.Models;
using EDEA.Services;
using log4net;

namespace EDEA.Models;

/// <summary>
/// Represents a celestial body within a star system.
/// </summary>
public class Body
{
    /// <summary>
    /// The logger for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(Body));

    /// <summary>
    /// The rings of this body, keyed by name.
    /// </summary>
    private readonly ConcurrentDictionary<string, Ring> _rings;

    // EDEA-only: retained for SQLiteStore body primary key persistence
    /// <summary>
    /// Gets or sets the 64-bit identifier.
    /// </summary>
    /// <value>The 64-bit identifier used for primary key persistence.</value>
    public long Id64 { get; set; }

    /// <summary>
    /// Gets or sets the body identifier.
    /// </summary>
    /// <value>The body identifier.</value>
    public int Id { get; protected set; }

    // EDEA-only: alias retained for SQLiteStore column mapping
    /// <summary>
    /// Gets or sets the body identifier alias.
    /// </summary>
    /// <value>The body identifier alias.</value>
    public int BodyId { get => Id; set => Id = value; }

    /// <summary>
    /// Gets or sets the star system identifier.
    /// </summary>
    /// <value>The star system identifier.</value>
    public long StarSystemId { get; set; }

    // EDEA-only: alias retained for SQLiteStore column mapping
    /// <summary>
    /// Gets or sets the star system 64-bit identifier alias.
    /// </summary>
    /// <value>The star system 64-bit identifier alias.</value>
    public long SystemId64 { get => StarSystemId; set => StarSystemId = value; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name of the body.</value>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets the short name relative to the star system.
    /// </summary>
    /// <value>The short name, or "⁕" if it cannot be determined.</value>
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

    /// <summary>
    /// Gets or sets the type of the body.
    /// </summary>
    /// <value>The type of the body.</value>
    public BodyType Type { get; set; }

    /// <summary>
    /// Gets a value indicating whether this body is a planet.
    /// </summary>
    /// <value><see langword="true"/> if this body is a planet; otherwise, <see langword="false"/>.</value>
    public bool IsPlanet => Type == BodyType.Planet;

    /// <summary>
    /// Gets a value indicating whether this body is a star.
    /// </summary>
    /// <value><see langword="true"/> if this body is a star; otherwise, <see langword="false"/>.</value>
    public bool IsStar => Type == BodyType.Star;

    /// <summary>
    /// Gets a value indicating whether this body is a planet or a star.
    /// </summary>
    /// <value><see langword="true"/> if this body is a planet or a star; otherwise, <see langword="false"/>.</value>
    public bool IsPlanetOrStar => IsPlanet || IsStar;

    /// <summary>
    /// Gets or sets the distance from the arrival point.
    /// </summary>
    /// <value>The distance from the arrival point.</value>
    public double Distance { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this body was already discovered.
    /// </summary>
    /// <value><see langword="true"/> if this body was already discovered; otherwise, <see langword="false"/>.</value>
    public bool WasDiscovered { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this body was read from a journal.
    /// </summary>
    /// <value><see langword="true"/> if this body was read from a journal; otherwise, <see langword="false"/>.</value>
    public bool WasReadFromJournal { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this body was read from EDSM.
    /// </summary>
    /// <value><see langword="true"/> if this body was read from EDSM; otherwise, <see langword="false"/>.</value>
    public bool WasReadFromEdsm { get; set; }

    /// <summary>
    /// Gets a value indicating whether this body was read from EDSM only.
    /// </summary>
    /// <value><see langword="true"/> if this body was read from EDSM and not from a journal; otherwise, <see langword="false"/>.</value>
    public bool WasReadFromEdsmOnly => WasReadFromEdsm && !WasReadFromJournal;

    /// <summary>
    /// Gets or sets the name of the commander who discovered the body on EDSM.
    /// </summary>
    /// <value>The commander name, or <see langword="null"/> if not known.</value>
    public string? EdsmDiscoveryCommander { get; set; }

    // EDEA-only: alias retained for SQLiteStore column mapping
    /// <summary>
    /// Gets or sets the EDSM discoverer alias.
    /// </summary>
    /// <value>The EDSM discoverer alias.</value>
    public string? EdsmDiscoverer
    {
        get => EdsmDiscoveryCommander;
        set => EdsmDiscoveryCommander = value;
    }

    /// <summary>
    /// Gets or sets the parent star system.
    /// </summary>
    /// <value>The parent star system, or <see langword="null"/> if not set.</value>
    public StarSystem? StarSystem { get; set; }

    /// <summary>
    /// Gets or sets the radius.
    /// </summary>
    /// <value>The radius of the body.</value>
    public double Radius { get; set; }

    /// <summary>
    /// Gets or sets the mass.
    /// </summary>
    /// <value>The mass of the body.</value>
    public double Mass { get; set; }

    /// <summary>
    /// Gets or sets the cartographic value.
    /// </summary>
    /// <value>The current cartographic value.</value>
    public int CartographicValue { get; set; }

    /// <summary>
    /// Gets or sets the maximum cartographic value.
    /// </summary>
    /// <value>The maximum cartographic value.</value>
    public int CartographicMaxValue { get; set; }

    /// <summary>
    /// Gets or sets the base cartographic value.
    /// </summary>
    /// <value>The base cartographic value.</value>
    public int CartographicBaseValue { get; set; }

    /// <summary>
    /// Gets or sets the first discovery bonus value.
    /// </summary>
    /// <value>The first discovery bonus value.</value>
    public int CartographicFirstDiscoveryBonusValue { get; set; }

    /// <summary>
    /// Gets or sets the first discovery bonus value without efficiency.
    /// </summary>
    /// <value>The first discovery bonus value without efficiency.</value>
    public int CartographicFirstDiscoveryBonusWithoutEfficiencyValue { get; set; }

    /// <summary>
    /// Gets or sets the first discovery bonus value without surface scan.
    /// </summary>
    /// <value>The first discovery bonus value without surface scan.</value>
    public int CartographicFirstDiscoveryBonusWithoutSurfaceScanValue { get; set; }

    /// <summary>
    /// Gets the rings of this body.
    /// </summary>
    /// <value>A read-only dictionary of rings keyed by name.</value>
    public IReadOnlyDictionary<string, Ring> Rings => _rings;

    /// <summary>
    /// Gets a value indicating whether this body has rings.
    /// </summary>
    /// <value><see langword="true"/> if this body has rings; otherwise, <see langword="false"/>.</value>
    public bool HasRings => _rings.Count > 0;

    /// <summary>
    /// Gets or sets the reserve level of this body's rings.
    /// </summary>
    /// <value>The reserve level of the rings.</value>
    public RingReserveLevel RingsReserveLevel { get; set; }

    /// <summary>
    /// Gets the total width of all rings.
    /// </summary>
    /// <value>The total width of all rings.</value>
    public long RingsTotalWidth => _rings.Sum((KeyValuePair<string, Ring> ring) => ring.Value.Width);

    /// <summary>
    /// Gets or sets the orbital inclination.
    /// </summary>
    /// <value>The orbital inclination, or <see langword="null"/> if not specified.</value>
    public double? OrbitalInclination { get; set; }

    // EDEA-only: persisted by SQLiteStore for history statistics
    /// <summary>
    /// Gets or sets a value indicating whether this body has biological signals.
    /// </summary>
    /// <value><see langword="true"/> if this body has biological signals; otherwise, <see langword="false"/>.</value>
    public bool HasBiological { get; set; }

    // EDEA-only: persisted by SQLiteStore for history statistics
    /// <summary>
    /// Gets or sets a value indicating whether this body has geological signals.
    /// </summary>
    /// <value><see langword="true"/> if this body has geological signals; otherwise, <see langword="false"/>.</value>
    public bool HasGeological { get; set; }

    // EDEA-only: persisted by SQLiteStore for history statistics
    /// <summary>
    /// Gets or sets a value indicating whether this body is considered valuable.
    /// </summary>
    /// <value><see langword="true"/> if this body is valuable; otherwise, <see langword="false"/>.</value>
    public bool IsValuable { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Body"/> class.
    /// </summary>
    public Body()
    {
        _rings = new ConcurrentDictionary<string, Ring>();
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Body"/> class.
    /// </summary>
    /// <param name="id">The body identifier.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="name">The name of the body.</param>
    /// <param name="distance">The distance from the arrival point.</param>
    /// <param name="radius">The radius of the body.</param>
    /// <param name="mass">The mass of the body.</param>
    /// <param name="orbitalInclination">The orbital inclination, or <see langword="null"/> if not specified.</param>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Body"/> class.
    /// </summary>
    /// <param name="id">The body identifier.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="name">The name of the body.</param>
    /// <param name="type">The type of the body.</param>
    /// <param name="distance">The distance from the arrival point.</param>
    /// <param name="wasDiscovered">Whether this body was already discovered.</param>
    /// <param name="wasMapped">Whether this body was mapped.</param>
    /// <param name="wasFootfalled">Whether this body was footfalled, or <see langword="null"/> if not specified.</param>
    /// <param name="wasReadFromJournal">Whether this body was read from a journal.</param>
    /// <param name="wasReadFromEdsm">Whether this body was read from EDSM.</param>
    /// <param name="edsmDiscoveryCommander">The EDSM discovery commander.</param>
    /// <param name="planetClass">The planet class.</param>
    /// <param name="isLandable">Whether this body is landable.</param>
    /// <param name="terraformingState">The terraforming state.</param>
    /// <param name="surfaceScanned">Whether this body was surface scanned.</param>
    /// <param name="gravity">The gravity of the body.</param>
    /// <param name="geologicalCount">The number of geological signals.</param>
    /// <param name="biologicalCount">The number of biological signals.</param>
    /// <param name="starType">The star type.</param>
    /// <param name="surfaceTemperature">The surface temperature.</param>
    /// <param name="touchdown">Whether touchdown occurred.</param>
    /// <param name="volcanism">The volcanism description.</param>
    /// <param name="atmosphere">The atmosphere description.</param>
    /// <param name="radius">The radius of the body.</param>
    /// <param name="parentStarId">The parent star identifier, or <see langword="null"/> if not specified.</param>
    /// <param name="parentPlanetId">The parent planet identifier, or <see langword="null"/> if not specified.</param>
    /// <param name="mass">The mass of the body.</param>
    /// <param name="orbitalInclination">The orbital inclination, or <see langword="null"/> if not specified.</param>
    /// <param name="efficientlyScanned">Whether this body was efficiently scanned.</param>
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

    /// <summary>
    /// Updates this body with the data from the specified body and source.
    /// </summary>
    /// <param name="body">The body to copy data from.</param>
    /// <param name="dataSource">The data source that provided the new data.</param>
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

    /// <summary>
    /// Attempts to add or update the specified ring for this body.
    /// </summary>
    /// <param name="ring">The ring to add or update.</param>
    /// <param name="dataSource">The data source that provided the ring.</param>
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

    /// <summary>
    /// Calculates the cartographic value for this body.
    /// </summary>
    /// <param name="skipSpeechOutput">Whether to skip speech output.</param>
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
            PlatformServices.Speech?.SpeakValuableBody(
                new SpeechOutputBody(this),
                new SpeechOutputCartographicValues(CartographicMaxValue, CartographicValue));
        }
    }

    /// <summary>
    /// Calculates the achievable surface scan and bonus values.
    /// </summary>
    /// <param name="baseValue">The base value, passed by reference.</param>
    /// <param name="surfaceScanValue">The surface scan value, passed by reference.</param>
    /// <param name="firstSurfaceScanBonusValue">The first surface scan bonus value, passed by reference.</param>
    /// <param name="efficientlyScannedBonusValue">The efficiently scanned bonus value, passed by reference.</param>
    /// <param name="surfaceScanPlanet">The planet used for surface scanning, if applicable.</param>
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

    /// <summary>
    /// Calculates the base cartographic value.
    /// </summary>
    /// <param name="baseValue">The base value, passed by reference.</param>
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
