using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace EDEA.Models;

public class Planet : Body
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Planet));

    private readonly ConcurrentDictionary<string, Genus> _genuses;

    private Star? parentStar;

    private Planet? parentPlanet;

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

    public string PlanetClass { get; set; }

    public bool IsLandable { get; set; }

    public string TerraformingState { get; set; }

    public bool WasMapped { get; set; }

    public bool? WasFootfalled { get; set; }

    public bool SurfaceScanned { get; set; }

    public double Gravity { get; set; }

    public double SurfaceTemperature { get; set; }

    public int GeologicalCount { get; set; }

    public int BiologicalCount { get; set; }

    public bool IsCurrentPlanetInSystem { get; set; }

    public IReadOnlyDictionary<string, Genus> Genuses => _genuses;

    public bool HasGenera => _genuses.Count > 0;

    public bool Touchdown { get; set; }

    public string Volcanism { get; set; }

    public string Atmosphere { get; set; }

    public int? ParentStarId { get; set; }

    public int? ParentPlanetId { get; set; }

    public List<GenusClassification> PredictedSpecies { get; }

    public bool InitialPredictionOfSpecies { get; set; }

    public List<PlanetClassification> MatchingPlanetClassifications { get; }

    public bool MatchingPlanetClassificationsAnnounced { get; set; }

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

    public bool EfficientlyScanned { get; set; }

    public int CartographicSurfaceScanValue { get; set; }

    public int CartographicFirstSurfaceScanBonusValue { get; set; }

    public int CartographicEfficientlyScannedBonusValue { get; set; }

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
