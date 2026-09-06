using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EDEA.Services;
using log4net;

namespace EDEA.Models;

/// <summary>
/// Represents a planet in a star system.
/// </summary>
public class Planet : Body
{
    /// <summary>
    /// The logger for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(Planet));

    /// <summary>
    /// The genera detected on this planet, keyed by name.
    /// </summary>
    private readonly ConcurrentDictionary<string, Genus> _genuses;

    /// <summary>
    /// The cached parent star.
    /// </summary>
    private Star? parentStar;

    /// <summary>
    /// The cached parent planet.
    /// </summary>
    private Planet? parentPlanet;

    /// <summary>
    /// Initializes a new instance of the <see cref="Planet"/> class.
    /// </summary>
    public Planet()
    {
        _genuses = new ConcurrentDictionary<string, Genus>();
        PredictedSpecies = new List<GenusClassification>();
        MatchingPlanetClassifications = new List<PlanetClassification>();
        PlanetClass = string.Empty;
        TerraformingState = string.Empty;
        Volcanism = string.Empty;
        Atmosphere = string.Empty;
    }

    /// <summary>
    /// Gets or sets the planet class.
    /// </summary>
    /// <value>The planet class.</value>
    public string PlanetClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the planet is landable.
    /// </summary>
    /// <value><see langword="true"/> if landable; otherwise, <see langword="false"/>.</value>
    public bool IsLandable { get; set; }

    /// <summary>
    /// Gets or sets the terraforming state.
    /// </summary>
    /// <value>The terraforming state.</value>
    public string TerraformingState { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the planet was mapped.
    /// </summary>
    /// <value><see langword="true"/> if mapped; otherwise, <see langword="false"/>.</value>
    public bool WasMapped { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the planet was footfalled.
    /// </summary>
    /// <value><see langword="true"/> if footfalled; <see langword="false"/> if not; <see langword="null"/> if unspecified.</value>
    public bool? WasFootfalled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the planet was surface scanned.
    /// </summary>
    /// <value><see langword="true"/> if surface scanned; otherwise, <see langword="false"/>.</value>
    public bool SurfaceScanned { get; set; }

    /// <summary>
    /// Gets or sets the surface gravity.
    /// </summary>
    /// <value>The gravity.</value>
    public double Gravity { get; set; }

    /// <summary>
    /// Gets or sets the surface temperature.
    /// </summary>
    /// <value>The surface temperature.</value>
    public double SurfaceTemperature { get; set; }

    /// <summary>
    /// Gets or sets the number of geological signals.
    /// </summary>
    /// <value>The geological signal count.</value>
    public int GeologicalCount { get; set; }

    /// <summary>
    /// Gets or sets the number of biological signals.
    /// </summary>
    /// <value>The biological signal count.</value>
    public int BiologicalCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the current planet in the system.
    /// </summary>
    /// <value><see langword="true"/> if current; otherwise, <see langword="false"/>.</value>
    public bool IsCurrentPlanetInSystem { get; set; }

    /// <summary>
    /// Gets the genera detected on this planet.
    /// </summary>
    /// <value>A read-only dictionary of genera keyed by name.</value>
    public IReadOnlyDictionary<string, Genus> Genuses => _genuses;

    /// <summary>
    /// Gets a value indicating whether the planet has any genera.
    /// </summary>
    /// <value><see langword="true"/> if genera exist; otherwise, <see langword="false"/>.</value>
    public bool HasGenera => _genuses.Count > 0;

    /// <summary>
    /// Gets or sets a value indicating whether a touchdown occurred.
    /// </summary>
    /// <value><see langword="true"/> if touchdown occurred; otherwise, <see langword="false"/>.</value>
    public bool Touchdown { get; set; }

    /// <summary>
    /// Gets or sets the volcanism description.
    /// </summary>
    /// <value>The volcanism description.</value>
    public string Volcanism { get; set; }

    /// <summary>
    /// Gets or sets the atmosphere description.
    /// </summary>
    /// <value>The atmosphere description.</value>
    public string Atmosphere { get; set; }

    /// <summary>
    /// Gets or sets the parent star identifier.
    /// </summary>
    /// <value>The parent star identifier, or <see langword="null"/> if not specified.</value>
    public int? ParentStarId { get; set; }

    /// <summary>
    /// Gets or sets the parent planet identifier.
    /// </summary>
    /// <value>The parent planet identifier, or <see langword="null"/> if not specified.</value>
    public int? ParentPlanetId { get; set; }

    /// <summary>
    /// Gets the list of predicted species.
    /// </summary>
    /// <value>The predicted species.</value>
    public List<GenusClassification> PredictedSpecies { get; }

    /// <summary>
    /// Gets or sets a value indicating whether an initial species prediction was made.
    /// </summary>
    /// <value><see langword="true"/> if initial prediction was made; otherwise, <see langword="false"/>.</value>
    public bool InitialPredictionOfSpecies { get; set; }

    /// <summary>
    /// Gets the list of matching planet classifications.
    /// </summary>
    /// <value>The matching planet classifications.</value>
    public List<PlanetClassification> MatchingPlanetClassifications { get; }

    /// <summary>
    /// Gets or sets a value indicating whether matching classifications were announced.
    /// </summary>
    /// <value><see langword="true"/> if announced; otherwise, <see langword="false"/>.</value>
    public bool MatchingPlanetClassificationsAnnounced { get; set; }

    /// <summary>
    /// Gets a value indicating whether the planet is terraformable.
    /// </summary>
    /// <value><see langword="true"/> if terraformable; otherwise, <see langword="false"/>.</value>
    public bool IsTerraformable
    {
        get
        {
            if (TerraformingState == "Terraformable")
            {
                return true;
            }
            if (TerraformingState == "Candidate for terraforming")
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Gets the parent star of this planet.
    /// </summary>
    /// <value>The parent star, or <see langword="null"/> if not found.</value>
    public Star? ParentStar
    {
        get
        {
            if (parentStar != null)
            {
                return parentStar;
            }
            if (!ParentStarId.HasValue)
            {
                return null;
            }
            parentStar = (Star?)base.StarSystem?.Bodies?.FirstOrDefault(x => x.Value.Id == ParentStarId.Value && x.Value.Type == BodyType.Star).Value;
            return parentStar;
        }
    }

    /// <summary>
    /// Gets the parent planet of this planet.
    /// </summary>
    /// <value>The parent planet, or <see langword="null"/> if not found.</value>
    public Planet? ParentPlanet
    {
        get
        {
            if (parentPlanet != null)
            {
                return parentPlanet;
            }
            if (!ParentPlanetId.HasValue)
            {
                return null;
            }
            parentPlanet = (Planet?)base.StarSystem?.Bodies?.FirstOrDefault(x => x.Value.Id == ParentPlanetId.Value && x.Value.Type == BodyType.Planet).Value;
            return parentPlanet;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the planet was efficiently scanned.
    /// </summary>
    /// <value><see langword="true"/> if efficiently scanned; otherwise, <see langword="false"/>.</value>
    public bool EfficientlyScanned { get; set; }

    /// <summary>
    /// Gets or sets the surface scan cartographic value.
    /// </summary>
    /// <value>The surface scan value.</value>
    public int CartographicSurfaceScanValue { get; set; }

    /// <summary>
    /// Gets or sets the first surface scan bonus value.
    /// </summary>
    /// <value>The first surface scan bonus value.</value>
    public int CartographicFirstSurfaceScanBonusValue { get; set; }

    /// <summary>
    /// Gets or sets the efficiently scanned bonus value.
    /// </summary>
    /// <value>The efficiently scanned bonus value.</value>
    public int CartographicEfficientlyScannedBonusValue { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Planet"/> class.
    /// </summary>
    /// <param name="id">The body identifier.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="name">The planet name.</param>
    /// <param name="distance">The distance from the arrival point.</param>
    /// <param name="planetClass">The planet class.</param>
    /// <param name="isLandable">Whether the planet is landable.</param>
    /// <param name="terraformingState">The terraforming state.</param>
    /// <param name="gravity">The surface gravity.</param>
    /// <param name="surfaceTemperature">The surface temperature.</param>
    /// <param name="volcanism">The volcanism description.</param>
    /// <param name="atmosphere">The atmosphere description.</param>
    /// <param name="radius">The radius.</param>
    /// <param name="parentStarId">The parent star identifier, or <see langword="null"/> if not specified.</param>
    /// <param name="parentPlanetId">The parent planet identifier, or <see langword="null"/> if not specified.</param>
    /// <param name="mass">The mass.</param>
    /// <param name="orbitalInclination">The orbital inclination, or <see langword="null"/> if not specified.</param>
    public Planet(int id, long starSystemId, string name, double distance, string planetClass, bool isLandable, string terraformingState, double gravity, double surfaceTemperature, string volcanism, string atmosphere, double radius, int? parentStarId, int? parentPlanetId, double mass, double? orbitalInclination)
        : base(id, starSystemId, name, distance, radius, mass, orbitalInclination)
    {
        _genuses = new ConcurrentDictionary<string, Genus>();
        PlanetClass = planetClass ?? string.Empty;
        IsLandable = isLandable;
        TerraformingState = terraformingState ?? string.Empty;
        WasMapped = true;
        WasFootfalled = null;
        Gravity = gravity;
        Atmosphere = atmosphere ?? string.Empty;
        base.Radius = radius;
        base.Type = BodyType.Planet;
        SurfaceTemperature = surfaceTemperature;
        Volcanism = volcanism ?? string.Empty;
        ParentStarId = parentStarId;
        ParentPlanetId = parentPlanetId;
        PredictedSpecies = new List<GenusClassification>();
        InitialPredictionOfSpecies = false;
        MatchingPlanetClassificationsAnnounced = false;
        MatchingPlanetClassifications = new List<PlanetClassification>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Planet"/> class from persisted data.
    /// </summary>
    /// <param name="id">The body identifier.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="name">The planet name.</param>
    /// <param name="type">The body type.</param>
    /// <param name="distance">The distance from the arrival point.</param>
    /// <param name="wasDiscovered">Whether the planet was already discovered.</param>
    /// <param name="wasMapped">Whether the planet was mapped.</param>
    /// <param name="wasFootfalled">Whether the planet was footfalled, or <see langword="null"/> if unspecified.</param>
    /// <param name="wasReadFromJournal">Whether the planet was read from a journal.</param>
    /// <param name="wasReadFromEdsm">Whether the planet was read from EDSM.</param>
    /// <param name="edsmDiscoveryCommander">The EDSM discovery commander.</param>
    /// <param name="planetClass">The planet class.</param>
    /// <param name="isLandable">Whether the planet is landable.</param>
    /// <param name="terraformingState">The terraforming state.</param>
    /// <param name="surfaceScanned">Whether the planet was surface scanned.</param>
    /// <param name="gravity">The surface gravity.</param>
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
    /// <param name="efficientlyScanned">Whether the planet was efficiently scanned.</param>
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
    public Planet(long id, long starSystemId, string name, long type, double distance, long wasDiscovered, long wasMapped, long? wasFootfalled, long wasReadFromJournal, long wasReadFromEdsm, string edsmDiscoveryCommander, string planetClass, long isLandable, string terraformingState, long surfaceScanned, double gravity, long geologicalCount, long biologicalCount, string starType, double surfaceTemperature, long touchdown, string volcanism, string atmosphere, double radius, long? parentStarId, long? parentPlanetId, double mass, double? orbitalInclination, long efficientlyScanned, long cartographicValue, long cartographicMaxValue, long cartographicBaseValue, long cartographicFirstDiscoveryBonusValue, long cartographicSurfaceScanValue, long cartographicFirstSurfaceScanBonusValue, long cartographicEfficientlyScannedBonusValue, long cartographicFirstDiscoveryBonusWithoutEfficiencyValue, long cartographicFirstDiscoveryBonusWithoutSurfaceScanValue, long ringsReserveLevel)
        : this(Convert.ToInt32(id), starSystemId, name, distance, planetClass, Convert.ToBoolean(isLandable), terraformingState, gravity, surfaceTemperature, volcanism ?? string.Empty, atmosphere ?? string.Empty, radius, (int?)parentStarId, (int?)parentPlanetId, mass, orbitalInclination)
    {
        base.WasDiscovered = Convert.ToBoolean(wasDiscovered);
        base.Type = (BodyType)type;
        base.WasReadFromJournal = Convert.ToBoolean(wasReadFromJournal);
        base.WasReadFromEdsm = Convert.ToBoolean(wasReadFromEdsm);
        base.EdsmDiscoveryCommander = edsmDiscoveryCommander;
        WasMapped = Convert.ToBoolean(wasMapped);
        WasFootfalled = ((!wasFootfalled.HasValue) ? ((bool?)null) : new bool?(Convert.ToBoolean(wasFootfalled)));
        SurfaceScanned = Convert.ToBoolean(surfaceScanned);
        GeologicalCount = Convert.ToInt32(geologicalCount);
        BiologicalCount = Convert.ToInt32(biologicalCount);
        Touchdown = Convert.ToBoolean(touchdown);
        EfficientlyScanned = Convert.ToBoolean(efficientlyScanned);
        base.CartographicValue = Convert.ToInt32(cartographicValue);
        base.CartographicMaxValue = Convert.ToInt32(cartographicMaxValue);
        base.CartographicBaseValue = Convert.ToInt32(cartographicBaseValue);
        base.CartographicFirstDiscoveryBonusValue = Convert.ToInt32(cartographicFirstDiscoveryBonusValue);
        CartographicSurfaceScanValue = Convert.ToInt32(cartographicSurfaceScanValue);
        CartographicFirstSurfaceScanBonusValue = Convert.ToInt32(cartographicFirstSurfaceScanBonusValue);
        CartographicEfficientlyScannedBonusValue = Convert.ToInt32(cartographicEfficientlyScannedBonusValue);
        base.CartographicFirstDiscoveryBonusWithoutEfficiencyValue = Convert.ToInt32(cartographicFirstDiscoveryBonusWithoutEfficiencyValue);
        base.CartographicFirstDiscoveryBonusWithoutSurfaceScanValue = Convert.ToInt32(cartographicFirstDiscoveryBonusWithoutSurfaceScanValue);
        base.RingsReserveLevel = (RingReserveLevel)ringsReserveLevel;
        PredictedSpecies = new List<GenusClassification>();
        InitialPredictionOfSpecies = false;
        MatchingPlanetClassificationsAnnounced = true;
        MatchingPlanetClassifications = new List<PlanetClassification>();
    }

    /// <summary>
    /// Updates this planet with data from the specified planet and source.
    /// </summary>
    /// <param name="planet">The planet to copy data from.</param>
    /// <param name="dataSource">The data source that provided the new data.</param>
    public void UpdatePlanet(Planet planet, DataSource dataSource)
    {
        UpdateBody(planet, dataSource);
        if (dataSource != DataSource.Edsm || !base.WasReadFromJournal)
        {
            PlanetClass = planet.PlanetClass;
            IsLandable = planet.IsLandable;
            TerraformingState = planet.TerraformingState;
            WasMapped = planet.WasMapped;
            WasFootfalled = planet.WasFootfalled;
            if (!SurfaceScanned)
            {
                SurfaceScanned = planet.SurfaceScanned;
            }
            if (!EfficientlyScanned)
            {
                EfficientlyScanned = planet.EfficientlyScanned;
            }
            Gravity = planet.Gravity;
            SurfaceTemperature = planet.SurfaceTemperature;
            if (planet.GeologicalCount > GeologicalCount)
            {
                GeologicalCount = planet.GeologicalCount;
            }
            if (planet.BiologicalCount > BiologicalCount)
            {
                BiologicalCount = planet.BiologicalCount;
            }
            Touchdown = planet.Touchdown;
            Volcanism = planet.Volcanism;
            Atmosphere = planet.Atmosphere;
            if (planet.ParentStarId.HasValue)
            {
                ParentStarId = planet.ParentStarId;
            }
            if (planet.ParentPlanetId.HasValue)
            {
                ParentPlanetId = planet.ParentPlanetId;
            }
            if (!MatchingPlanetClassificationsAnnounced)
            {
                MatchingPlanetClassificationsAnnounced = planet.MatchingPlanetClassificationsAnnounced;
            }
            CartographicSurfaceScanValue = planet.CartographicSurfaceScanValue;
            CartographicFirstSurfaceScanBonusValue = planet.CartographicFirstSurfaceScanBonusValue;
            CartographicEfficientlyScannedBonusValue = planet.CartographicEfficientlyScannedBonusValue;
        }
    }

    /// <summary>
    /// Determines the first discovery status for the specified genus.
    /// </summary>
    /// <param name="genus">The genus to evaluate.</param>
    public void DetermineFirstDiscoveryStatusForGenus(Genus genus)
    {
        if (genus.BodyId != base.Id || genus.StarSystemId != base.StarSystemId)
        {
            log.Warn($"First discovery status for genus '{genus.Name}' on planet '{base.Name}' ({base.Id}) was not determined because it belongs to planet id {genus.BodyId} / star system id {genus.StarSystemId}");
        }
        else if (!base.WasDiscovered && !WasMapped)
        {
            genus.IsFirstDiscovery = true;
            log.Info($"Genus '{genus.Name}' on planet '{base.Name}' ({base.Id}) was marked as the first discovery (with certainty) because the planet was undiscovered before");
        }
        else if (!WasMapped)
        {
            genus.IsFirstDiscovery = true;
            log.Info($"Genus '{genus.Name}' on planet '{base.Name}' ({base.Id}) was marked as the first discovery (with high probability) because the planet was unmapped before");
        }
        else
        {
            log.Info($"Genus '{genus.Name}' on planet '{base.Name}' ({base.Id}) was not marked as the first discovery (with no guarantee) because the planet was mapped by another commander");
        }
    }

    /// <summary>
    /// Attempts to add or update the specified genus for this planet.
    /// </summary>
    /// <param name="genus">The genus to add or update.</param>
    /// <param name="locationOnPlanet">The optional location of the scan.</param>
    /// <param name="scanType">The type of scan, or <see langword="null"/> if unspecified.</param>
    public void TryAddOrUpdateGenus(Genus genus, LocationOnPlanet? locationOnPlanet = null, string? scanType = null)
    {
        if (genus.BodyId != base.Id || genus.StarSystemId != base.StarSystemId)
        {
            log.Warn($"Genus '{genus.Name}' was not added or updated on planet '{base.Name}' ({base.Id}) because it belongs to planet id {genus.BodyId} / star system id {genus.StarSystemId}");
            return;
        }
        if (Genuses.ContainsKey(genus.Name))
        {
            if (genus.WasLogged.HasValue && !Genuses[genus.Name].WasLogged.HasValue)
            {
                Genuses[genus.Name].WasLogged = genus.WasLogged;
                log.Info($"Added WasLogged status '{Genuses[genus.Name].WasLogged}' to genus {Genuses[genus.Name].SpeciesShort} on planet '{base.Name}'");
            }
            if (Genuses[genus.Name].AnalysisComplete)
            {
                return;
            }
            if (!string.IsNullOrEmpty(genus.Species) && string.IsNullOrEmpty(Genuses[genus.Name].Species))
            {
                Genuses[genus.Name].Species = genus.Species;
                log.Info($"Species {Genuses[genus.Name].SpeciesShort} added to genus {genus.Name} on planet '{base.Name}'");
            }
            if (!string.IsNullOrEmpty(genus.Variant) && string.IsNullOrEmpty(Genuses[genus.Name].Variant))
            {
                Genuses[genus.Name].Variant = genus.Variant;
                log.Info($"Variant {Genuses[genus.Name].VariantShort} added to genus {genus.Name} and species {genus.SpeciesShort} on planet '{base.Name}'");
            }
        }
        else
        {
            _genuses.TryAdd(genus.Name, genus);
            log.Info($"Genus '{genus.Name}' with {(string.IsNullOrEmpty(genus.Species) ? "unknown " : string.Empty)}species {(string.IsNullOrEmpty(genus.Species) ? string.Empty : ("'" + genus.SpeciesShort + "' "))}and {(string.IsNullOrEmpty(genus.Variant) ? "unknown " : string.Empty)}variant {(string.IsNullOrEmpty(genus.Variant) ? string.Empty : ("'" + genus.VariantShort + "' "))}added to planet '{base.Name}', scan count is {genus.ScanCount}, analysis is{(genus.AnalysisComplete ? "" : " not")} completed");
        }
        if (Genuses[genus.Name].ClonalColonyRange == 0)
        {
            Genuses[genus.Name].ClonalColonyRange = GeneraIndexProvider.GetClonalColonyRangeForGenus(genus.Name);
        }
        DetermineFirstDiscoveryStatusForGenus(Genuses[genus.Name]);
        switch (scanType)
        {
            case "Log":
                Genuses[genus.Name].ScanCount = 1;
                if (locationOnPlanet != null)
                {
                    Genuses[genus.Name].LongitudeAt1stScan = locationOnPlanet.Longitude;
                    Genuses[genus.Name].LatitudeAt1stScan = locationOnPlanet.Latitude;
                }
                break;
            case "Sample":
                Genuses[genus.Name].ScanCount = 2;
                if (locationOnPlanet != null)
                {
                    Genuses[genus.Name].LongitudeAt2ndScan = locationOnPlanet.Longitude;
                    Genuses[genus.Name].LatitudeAt2ndScan = locationOnPlanet.Latitude;
                }
                break;
            case "Analyse":
                Genuses[genus.Name].ScanCount = 3;
                Genuses[genus.Name].AnalysisComplete = true;
                Genuses[genus.Name].CurrentDistanceToLocationAt1stScan = null;
                Genuses[genus.Name].CurrentDistanceToLocationAt2ndScan = null;
                break;
        }
    }
}
