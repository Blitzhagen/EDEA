using System;
using System.Collections.Generic;
using System.Linq;
using EDEA.Models;
using EDEA.Properties;

namespace EDEA.ViewModels;

/// <summary>
/// View model that wraps a <see cref="Body"/> for display in the bodies table.
/// </summary>
public class BodyViewModel : ViewModelBase
{
    /// <summary>
    /// The underlying body model.
    /// </summary>
    private readonly Body _body;

    /// <summary>
    /// Gets the body name.
    /// </summary>
    /// <value>The body name.</value>
    public string Name => _body.Name;

    /// <summary>
    /// Gets the short body name.
    /// </summary>
    /// <value>The short body name.</value>
    public string ShortName => _body.ShortName;

    /// <summary>
    /// Gets the name of the star system the body belongs to.
    /// </summary>
    /// <value>The star system name.</value>
    public string StarSystemName => _body.StarSystem?.Name ?? string.Empty;

    /// <summary>
    /// Gets the body identifier.
    /// </summary>
    /// <value>The body identifier.</value>
    public int Id => _body.Id;

    /// <summary>
    /// Gets the formatted distance to the body.
    /// </summary>
    /// <value>The distance string.</value>
    public string Distance { get; }

    /// <summary>
    /// Gets the distance value used for sorting.
    /// </summary>
    /// <value>The sortable distance.</value>
    public int DistanceSort { get; }

    /// <summary>
    /// Gets a value indicating whether the body was already discovered.
    /// </summary>
    /// <value><c>true</c> if the body was discovered; otherwise, <c>false</c>.</value>
    public bool WasDiscovered => _body.WasDiscovered;

    /// <summary>
    /// Gets a value indicating whether the body is a new discovery.
    /// </summary>
    /// <value><c>true</c> if the body is a new discovery; otherwise, <c>false</c>.</value>
    public bool NewDiscovery => !_body.WasDiscovered;

    /// <summary>
    /// Gets a value indicating whether the body was read from the journal.
    /// </summary>
    /// <value><c>true</c> if the body was read from the journal; otherwise, <c>false</c>.</value>
    public bool WasReadFromJournal => _body.WasReadFromJournal;

    /// <summary>
    /// Gets a value indicating whether the body was read from EDSM.
    /// </summary>
    /// <value><c>true</c> if the body was read from EDSM; otherwise, <c>false</c>.</value>
    public bool WasReadFromEdsm => _body.WasReadFromEdsm;

    /// <summary>
    /// Gets a value indicating whether the body was read from EDSM only.
    /// </summary>
    /// <value><c>true</c> if the body was read from EDSM only; otherwise, <c>false</c>.</value>
    public bool WasReadFromEdsmOnly { get; }

    /// <summary>
    /// Gets a value indicating whether the body is considered valuable.
    /// </summary>
    /// <value><c>true</c> if the body is valuable; otherwise, <c>false</c>.</value>
    public bool IsValuableBody { get; }

    /// <summary>
    /// Gets a value indicating whether the body is a planet or star.
    /// </summary>
    /// <value><c>true</c> if the body is a planet or star; otherwise, <c>false</c>.</value>
    public bool IsPlanetOrStar { get; }

    /// <summary>
    /// Gets a value indicating whether the body is a planet.
    /// </summary>
    /// <value><c>true</c> if the body is a planet; otherwise, <c>false</c>.</value>
    public bool IsPlanet { get; }

    /// <summary>
    /// Gets a value indicating whether the body is a star.
    /// </summary>
    /// <value><c>true</c> if the body is a star; otherwise, <c>false</c>.</value>
    public bool IsStar { get; }

    /// <summary>
    /// Gets a value indicating whether the body is a scoopable star.
    /// </summary>
    /// <value><c>true</c> if the star is scoopable; otherwise, <c>false</c>.</value>
    public bool IsScoopableStar { get; }

    /// <summary>
    /// Gets a value indicating whether the body is a non-scoopable star.
    /// </summary>
    /// <value><c>true</c> if the star is not scoopable; otherwise, <c>false</c>.</value>
    public bool IsNoScoopableStar { get; }

    /// <summary>
    /// Gets the body type.
    /// </summary>
    /// <value>The body type.</value>
    public BodyType Type => _body.Type;

    /// <summary>
    /// Gets the human-readable body type.
    /// </summary>
    /// <value>The human-readable body type.</value>
    public string TypeHumanReadable { get; }

    /// <summary>
    /// Gets a value indicating whether the body is terraformable.
    /// </summary>
    /// <value><c>true</c> if the body is terraformable; otherwise, <c>false</c>.</value>
    public bool Terraformable { get; }

    /// <summary>
    /// Gets a value indicating whether the body was mapped.
    /// </summary>
    /// <value><c>true</c> if the body was mapped; otherwise, <c>false</c>.</value>
    public bool WasMapped { get; }

    /// <summary>
    /// Gets a value indicating whether the body was not mapped.
    /// </summary>
    /// <value><c>true</c> if the body was not mapped; otherwise, <c>false</c>.</value>
    public bool WasNotMapped { get; }

    /// <summary>
    /// Gets a value indicating whether the body's surface was scanned.
    /// </summary>
    /// <value><c>true</c> if the surface was scanned; otherwise, <c>false</c>.</value>
    public bool SurfaceScanned { get; }

    /// <summary>
    /// Gets the surface scan status of the body.
    /// </summary>
    /// <value>The surface scan status.</value>
    public SurfaceScanStatus SurfaceScanStatus { get; }

    /// <summary>
    /// Gets a value indicating whether the body was efficiently scanned.
    /// </summary>
    /// <value><c>true</c> if the body was efficiently scanned; otherwise, <c>false</c>.</value>
    public bool EfficientlyScanned { get; }

    /// <summary>
    /// Gets the gravity value used for sorting.
    /// </summary>
    /// <value>The sortable gravity.</value>
    public double GravitySort { get; }

    /// <summary>
    /// Gets the formatted gravity of the body.
    /// </summary>
    /// <value>The gravity string.</value>
    public string Gravity { get; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the body has a gravity value.
    /// </summary>
    /// <value><c>true</c> if gravity is available; otherwise, <c>false</c>.</value>
    public bool HasGravity { get; }

    /// <summary>
    /// Gets the formatted atmosphere of the body.
    /// </summary>
    /// <value>The atmosphere string.</value>
    public string Atmosphere { get; } = string.Empty;

    /// <summary>
    /// Gets the formatted radius of the body.
    /// </summary>
    /// <value>The radius string.</value>
    public string Radius { get; } = string.Empty;

    /// <summary>
    /// Gets the radius value used for sorting.
    /// </summary>
    /// <value>The sortable radius.</value>
    public double RadiusSort { get; }

    /// <summary>
    /// Gets a value indicating whether the body has a radius value.
    /// </summary>
    /// <value><c>true</c> if radius is available; otherwise, <c>false</c>.</value>
    public bool HasRadius { get; }

    /// <summary>
    /// Gets the surface temperature value used for sorting.
    /// </summary>
    /// <value>The sortable surface temperature.</value>
    public double SurfaceTemperatureSort { get; }

    /// <summary>
    /// Gets the formatted surface temperature of the body.
    /// </summary>
    /// <value>The surface temperature string.</value>
    public string SurfaceTemperature { get; } = string.Empty;

    /// <summary>
    /// Gets the formatted volcanism of the body.
    /// </summary>
    /// <value>The volcanism string.</value>
    public string Volcanism { get; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the body is landable.
    /// </summary>
    /// <value><c>true</c> if the body is landable; otherwise, <c>false</c>.</value>
    public bool Landable { get; }

    /// <summary>
    /// Gets a value indicating whether a touchdown was recorded on the body.
    /// </summary>
    /// <value><c>true</c> if a touchdown was recorded; otherwise, <c>false</c>.</value>
    public bool Touchdown { get; }

    /// <summary>
    /// Gets a value indicating whether the body has rings.
    /// </summary>
    /// <value><c>true</c> if the body has rings; otherwise, <c>false</c>.</value>
    public bool HasRings { get; }

    /// <summary>
    /// Gets the formatted count of rings.
    /// </summary>
    /// <value>The rings count string.</value>
    public string RingsCount { get; } = string.Empty;

    /// <summary>
    /// Gets the ring count used for sorting.
    /// </summary>
    /// <value>The sortable ring count.</value>
    public int RingsSort { get; }

    /// <summary>
    /// Gets the ring reserve level.
    /// </summary>
    /// <value>The ring reserve level.</value>
    public RingReserveLevel RingsReserveLevel { get; }

    /// <summary>
    /// Gets the localized display name of the ring reserve level.
    /// </summary>
    /// <value>The localized ring reserve level name.</value>
    public string RingsReserveLevelName { get; }

    /// <summary>
    /// Gets the formatted total width of all rings.
    /// </summary>
    /// <value>The total ring width string.</value>
    public string RingsTotalWidth { get; } = string.Empty;

    /// <summary>
    /// Gets the collection of ring view models.
    /// </summary>
    /// <value>The rings of the body.</value>
    public IEnumerable<RingViewModel> Rings { get; } = Array.Empty<RingViewModel>();

    /// <summary>
    /// Gets a value indicating whether the body has geological signals.
    /// </summary>
    /// <value><c>true</c> if geological signals are present; otherwise, <c>false</c>.</value>
    public bool HasGeologicals { get; }

    /// <summary>
    /// Gets the formatted count of geological signals.
    /// </summary>
    /// <value>The geological signals count string.</value>
    public string GeologicalsCount { get; } = string.Empty;

    /// <summary>
    /// Gets the geological signals count used for sorting.
    /// </summary>
    /// <value>The sortable geological signals count.</value>
    public int GeologicalsSort { get; }

    /// <summary>
    /// Gets a value indicating whether the body has biological signals.
    /// </summary>
    /// <value><c>true</c> if biological signals are present; otherwise, <c>false</c>.</value>
    public bool HasBiologicals { get; }

    /// <summary>
    /// Gets the formatted count of biological signals.
    /// </summary>
    /// <value>The biological signals count string.</value>
    public string BiologicalsCount { get; } = string.Empty;

    /// <summary>
    /// Gets the biological signals count used for sorting.
    /// </summary>
    /// <value>The sortable biological signals count.</value>
    public int BiologicalsSort { get; }

    /// <summary>
    /// Gets the collection of genus view models.
    /// </summary>
    /// <value>The genuses found on the body.</value>
    public IEnumerable<GenusViewModel> Genuses { get; } = Array.Empty<GenusViewModel>();

    /// <summary>
    /// Gets the number of genuses.
    /// </summary>
    /// <value>The genus count.</value>
    public int GenusesCount { get; }

    /// <summary>
    /// Gets the EDSM discovery commander name.
    /// </summary>
    /// <value>The EDSM discovery commander.</value>
    public string EdsmDiscoveryCommander { get; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this body is the current planet in the system.
    /// </summary>
    /// <value><c>true</c> if this is the current planet; otherwise, <c>false</c>.</value>
    public bool IsCurrentPlanetInSystem { get; }

    /// <summary>
    /// Gets a value indicating whether the body is landable but has no touchdown recorded.
    /// </summary>
    /// <value><c>true</c> if landable without touchdown; otherwise, <c>false</c>.</value>
    public bool LandableNoTouchdown { get; }

    /// <summary>
    /// Gets a value indicating whether all genuses have been analysed.
    /// </summary>
    /// <value><c>true</c> if all genuses are analysed; otherwise, <c>false</c>.</value>
    public bool AllGenusesAnalysed { get; }

    /// <summary>
    /// Gets a value indicating whether genuses are partly analysed.
    /// </summary>
    /// <value><c>true</c> if some genuses are analysed; otherwise, <c>false</c>.</value>
    public bool GenusesPartlyAnalysed { get; }

    /// <summary>
    /// Gets the number of analysed genuses.
    /// </summary>
    /// <value>The analysed genus count.</value>
    public int AnalysedGenusesCount { get; }

    /// <summary>
    /// Gets a value indicating whether genuses are available.
    /// </summary>
    /// <value><c>true</c> if genuses are available; otherwise, <c>false</c>.</value>
    public bool GenusesAvailable { get; }

    /// <summary>
    /// Gets a value indicating whether no genuses are available.
    /// </summary>
    /// <value><c>true</c> if no genuses are available; otherwise, <c>false</c>.</value>
    public bool GenusesNotAvailable { get; }

    /// <summary>
    /// Gets the total Vista Genomics value of the genuses.
    /// </summary>
    /// <value>The Vista Genomics value.</value>
    public int GenusesVistaGenomicsValue { get; }

    /// <summary>
    /// Gets a value indicating whether any genus is valuable.
    /// </summary>
    /// <value><c>true</c> if a valuable genus is present; otherwise, <c>false</c>.</value>
    public bool HasValuableGenera { get; }

    /// <summary>
    /// Gets the collection of predicted species view models.
    /// </summary>
    /// <value>The predicted species.</value>
    public IEnumerable<GenusClassificationViewModel> PredictedSpecies { get; } = Array.Empty<GenusClassificationViewModel>();

    /// <summary>
    /// Gets the number of predicted species.
    /// </summary>
    /// <value>The predicted species count.</value>
    public int PredictedSpeciesCount { get; }

    /// <summary>
    /// Gets a value indicating whether predicted species are available.
    /// </summary>
    /// <value><c>true</c> if predicted species are available; otherwise, <c>false</c>.</value>
    public bool PredictedSpeciesAvailable { get; }

    /// <summary>
    /// Gets a value indicating whether no predicted species are available.
    /// </summary>
    /// <value><c>true</c> if no predicted species are available; otherwise, <c>false</c>.</value>
    public bool PredictedSpeciesNotAvailable { get; }

    /// <summary>
    /// Gets a value indicating whether any predicted species is valuable.
    /// </summary>
    /// <value><c>true</c> if a valuable predicted species is present; otherwise, <c>false</c>.</value>
    public bool HasValuablePredictedSpecies { get; }

    /// <summary>
    /// Gets the collection of matching planet classification view models.
    /// </summary>
    /// <value>The matching planet classifications.</value>
    public IEnumerable<PlanetClassificationViewModel> MatchingPlanetClassifications { get; } = Array.Empty<PlanetClassificationViewModel>();

    /// <summary>
    /// Gets the formatted count of matching planet classifications.
    /// </summary>
    /// <value>The matching planet classifications count string.</value>
    public string MatchingPlanetClassificationsCount { get; } = string.Empty;

    /// <summary>
    /// Gets the matching planet classifications count used for sorting.
    /// </summary>
    /// <value>The sortable matching planet classifications count.</value>
    public int MatchingPlanetClassificationsSort { get; }

    /// <summary>
    /// Gets a value indicating whether matching planet classifications are available.
    /// </summary>
    /// <value><c>true</c> if matching planet classifications are available; otherwise, <c>false</c>.</value>
    public bool MatchingPlanetClassificationsAvailable { get; }

    /// <summary>
    /// Gets the identifier of the parent star, if any.
    /// </summary>
    /// <value>The parent star identifier, or <c>null</c>.</value>
    public int? ParentStarId { get; }

    /// <summary>
    /// Gets the parent star, if any.
    /// </summary>
    /// <value>The parent star, or <c>null</c>.</value>
    public Star? ParentStar { get; }

    /// <summary>
    /// Gets the identifier of the parent planet, if any.
    /// </summary>
    /// <value>The parent planet identifier, or <c>null</c>.</value>
    public int? ParentPlanetId { get; }

    /// <summary>
    /// Gets the parent planet, if any.
    /// </summary>
    /// <value>The parent planet, or <c>null</c>.</value>
    public Planet? ParentPlanet { get; }

    /// <summary>
    /// Gets the formatted mass of the body.
    /// </summary>
    /// <value>The mass string.</value>
    public string Mass { get; } = string.Empty;

    /// <summary>
    /// Gets the mass value used for sorting.
    /// </summary>
    /// <value>The sortable mass.</value>
    public double MassSort { get; }

    /// <summary>
    /// Gets a value indicating whether the body has a mass value.
    /// </summary>
    /// <value><c>true</c> if mass is available; otherwise, <c>false</c>.</value>
    public bool HasMass { get; }

    /// <summary>
    /// Gets the formatted orbital inclination of the body.
    /// </summary>
    /// <value>The orbital inclination string.</value>
    public string OrbitalInclination { get; } = string.Empty;

    /// <summary>
    /// Gets the orbital inclination value used for sorting.
    /// </summary>
    /// <value>The sortable orbital inclination.</value>
    public double OrbitalInclinationSort { get; }

    /// <summary>
    /// Gets a value indicating whether the body has an orbital inclination value.
    /// </summary>
    /// <value><c>true</c> if orbital inclination is available; otherwise, <c>false</c>.</value>
    public bool HasOrbitalInclination { get; }

    /// <summary>
    /// Gets the formatted cartographic value of the body.
    /// </summary>
    /// <value>The cartographic value string.</value>
    public string CartographicValue { get; }

    /// <summary>
    /// Gets the cartographic value used for sorting.
    /// </summary>
    /// <value>The sortable cartographic value.</value>
    public double CartographicValueSort { get; }

    /// <summary>
    /// Gets a value indicating whether the body has a cartographic value.
    /// </summary>
    /// <value><c>true</c> if a cartographic value is available; otherwise, <c>false</c>.</value>
    public bool HasCartographicValue { get; }

    /// <summary>
    /// Gets the formatted maximum cartographic value of the body.
    /// </summary>
    /// <value>The maximum cartographic value string.</value>
    public string CartographicMaxValue { get; }

    /// <summary>
    /// Gets the maximum cartographic value used for sorting.
    /// </summary>
    /// <value>The sortable maximum cartographic value.</value>
    public double CartographicMaxValueSort { get; }

    /// <summary>
    /// Gets the formatted base cartographic value of the body.
    /// </summary>
    /// <value>The base cartographic value string.</value>
    public string CartographicBaseValue { get; }

    /// <summary>
    /// Gets the formatted cartographic surface scan value.
    /// </summary>
    /// <value>The cartographic surface scan value string.</value>
    public string CartographicSurfaceScanValue { get; } = string.Empty;

    /// <summary>
    /// Gets the formatted first surface scan bonus value.
    /// </summary>
    /// <value>The first surface scan bonus value string.</value>
    public string CartographicFirstSurfaceScanBonusValue { get; } = string.Empty;

    /// <summary>
    /// Gets the formatted first discovery bonus value.
    /// </summary>
    /// <value>The first discovery bonus value string.</value>
    public string CartographicFirstDiscoveryBonusValue { get; } = string.Empty;

    /// <summary>
    /// Gets the formatted first discovery bonus range.
    /// </summary>
    /// <value>The first discovery bonus range string.</value>
    public string CartographicFirstDiscoveryBonusRange { get; } = string.Empty;

    /// <summary>
    /// Gets the formatted efficiently scanned bonus value.
    /// </summary>
    /// <value>The efficiently scanned bonus value string.</value>
    public string CartographicEfficientlyScannedBonusValue { get; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the maximum cartographic value is reached.
    /// </summary>
    /// <value><c>true</c> if the maximum cartographic value is reached; otherwise, <c>false</c>.</value>
    public bool CartographicMaxValueReached { get; }

    /// <summary>
    /// Gets a value indicating whether the maximum cartographic value is not reached.
    /// </summary>
    /// <value><c>true</c> if the maximum cartographic value is not reached; otherwise, <c>false</c>.</value>
    public bool CartographicMaxValueNotReached { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BodyViewModel"/> class.
    /// </summary>
    /// <param name="body">The body model to wrap.</param>
    public BodyViewModel(Body body)
    {
        _body = body;
        Distance = _body.Distance.ToString("n0") + " " + Resources.UnitLightSeconds;
        DistanceSort = (int)Math.Round(_body.Distance);
        IsPlanetOrStar = _body.Type == BodyType.Planet || _body.Type == BodyType.Star;
        IsPlanet = _body.Type == BodyType.Planet;
        IsStar = _body.Type == BodyType.Star;
        IsScoopableStar = _body.Type == BodyType.Star && Helpsters.CheckStarClassForScoopable(((Star)_body).StarType);
        IsNoScoopableStar = !IsScoopableStar;
        if (_body.Radius > 0.0)
        {
            Radius = Math.Round(_body.Radius / 1000.0).ToString("n0") + " " + Resources.UnitKilometers;
            HasRadius = true;
        }
        RadiusSort = _body.Radius;
        MassSort = _body.Mass;
        if (_body.OrbitalInclination.HasValue)
        {
            OrbitalInclinationSort = Convert.ToDouble(_body.OrbitalInclination);
            OrbitalInclination = $"{OrbitalInclinationSort:n3}°";
            HasOrbitalInclination = true;
        }
        WasReadFromEdsmOnly = _body.WasReadFromEdsmOnly;
        SurfaceScanStatus = SurfaceScanStatus.UnscannedAndWasNotMapped;
        if (_body.EdsmDiscoveryCommander != null)
        {
            EdsmDiscoveryCommander = EDEA.Globals.CommanderNamePrefix + _body.EdsmDiscoveryCommander;
        }
        CartographicValue = _body.CartographicValue.ToString("n0") + " " + Resources.UnitCredits;
        CartographicValueSort = _body.CartographicValue;
        HasCartographicValue = CartographicValueSort > 0.0;
        CartographicMaxValue = _body.CartographicMaxValue.ToString("n0") + " " + Resources.UnitCredits;
        CartographicMaxValueSort = _body.CartographicMaxValue;
        CartographicMaxValueReached = CartographicValue == CartographicMaxValue;
        CartographicMaxValueNotReached = CartographicValue != CartographicMaxValue;
        CartographicBaseValue = _body.CartographicBaseValue.ToString("n0") + " " + Resources.UnitCredits;
        CartographicFirstDiscoveryBonusRange = _body.CartographicFirstDiscoveryBonusValue.ToString("n0") + " " + Resources.UnitCredits;
        IsValuableBody = _body.CartographicMaxValue >= Preferences.Other.ValuableBodyThreshold;
        HasRings = _body.HasRings;
        RingsReserveLevel = RingReserveLevel.Unknown;
        if (HasRings)
        {
            Rings = from item in _body.Rings.Values
                    select new RingViewModel(item, _body.Name) into item
                    orderby item.Name
                    select item;
            RingsCount = _body.Rings.Count.ToString();
            RingsSort = _body.Rings.Count;
            RingsReserveLevel = _body.RingsReserveLevel;
            RingsTotalWidth = (_body.RingsTotalWidth / 1000).ToString("n0") + " " + Resources.UnitKilometers;
        }

        RingsReserveLevelName = Helpsters.GetRingReserveLevelName(RingsReserveLevel);
        if (_body.IsPlanet)
        {
            Planet planet = (Planet)_body;
            TypeHumanReadable = Globals.GetLocalizedPlanetClass(((Planet)_body).PlanetClass);
            Terraformable = planet.IsTerraformable;
            WasMapped = planet.WasMapped;
            WasNotMapped = !planet.WasMapped;
            SurfaceScanned = planet.SurfaceScanned;
            if (!SurfaceScanned && WasMapped)
            {
                SurfaceScanStatus = SurfaceScanStatus.UnscannedAndWasMapped;
            }
            else if (!SurfaceScanned && !WasMapped)
            {
                SurfaceScanStatus = SurfaceScanStatus.UnscannedAndWasNotMapped;
            }
            else if (SurfaceScanned && WasMapped)
            {
                SurfaceScanStatus = SurfaceScanStatus.ScannedAndWasMapped;
            }
            else
            {
                SurfaceScanStatus = SurfaceScanStatus.ScannedAndWasNotMapped;
            }
            EfficientlyScanned = planet.EfficientlyScanned;
            GravitySort = planet.Gravity;
            SurfaceTemperatureSort = planet.SurfaceTemperature;
            Landable = planet.IsLandable;
            IsCurrentPlanetInSystem = planet.IsCurrentPlanetInSystem;
            Volcanism = Globals.GetLocalizedVolcanism(planet.Volcanism);
            ParentStarId = planet.ParentStarId;
            ParentStar = planet.ParentStar;
            ParentPlanetId = planet.ParentPlanetId;
            ParentPlanet = planet.ParentPlanet;
            if (planet.Mass > 0.0)
            {
                Mass = $"{planet.Mass:n4} EM";
            }
            CartographicSurfaceScanValue = planet.CartographicSurfaceScanValue.ToString("n0") + " " + Resources.UnitCredits;
            CartographicFirstSurfaceScanBonusValue = planet.CartographicFirstSurfaceScanBonusValue.ToString("n0") + " " + Resources.UnitCredits;
            CartographicEfficientlyScannedBonusValue = planet.CartographicEfficientlyScannedBonusValue.ToString("n0") + " " + Resources.UnitCredits;
            if (_body.CartographicFirstDiscoveryBonusValue != _body.CartographicFirstDiscoveryBonusWithoutSurfaceScanValue)
            {
                CartographicFirstDiscoveryBonusRange = _body.CartographicFirstDiscoveryBonusWithoutSurfaceScanValue.ToString("n0") + " " + Globals.Hyphen + " " + CartographicFirstDiscoveryBonusRange;
            }
            if (planet.Gravity > 0.0)
            {
                Gravity = $"{planet.Gravity:n2} g";
                HasGravity = true;
            }
            if (!string.Equals(planet.Atmosphere, "no atmosphere", StringComparison.OrdinalIgnoreCase))
            {
                Atmosphere = Globals.GetLocalizedAtmosphere(planet.Atmosphere);
            }
            if (planet.SurfaceTemperature > 0.0)
            {
                SurfaceTemperature = $"{Math.Round(planet.SurfaceTemperature)} K";
            }
            if (planet.IsLandable)
            {
                Touchdown = planet.Touchdown;
                LandableNoTouchdown = Landable && !Touchdown;
                if (!planet.WasReadFromJournal)
                {
                    GeologicalsCount = "?";
                    BiologicalsCount = "?";
                }
                else
                {
                    if (planet.GeologicalCount != 0)
                    {
                        HasGeologicals = true;
                        GeologicalsCount = planet.GeologicalCount.ToString();
                        GeologicalsSort = planet.GeologicalCount;
                    }
                    if (planet.BiologicalCount != 0)
                    {
                        HasBiologicals = true;
                        BiologicalsCount = planet.BiologicalCount.ToString();
                        BiologicalsSort = planet.BiologicalCount;
                        KeyValuePair<string, Genus>[] source = planet.Genuses.ToArray();
                        Genuses = source.Select((KeyValuePair<string, Genus> genusEntry) => new GenusViewModel(genusEntry.Value));
                        GenusesCount = Genuses.Count();
                        AnalysedGenusesCount = source.Count(genusEntry => genusEntry.Value.AnalysisComplete);
                        HasValuableGenera = Genuses.Any(vm => vm.IsValuableGenus);
                        AllGenusesAnalysed = planet.BiologicalCount == AnalysedGenusesCount;
                        GenusesPartlyAnalysed = !AllGenusesAnalysed && AnalysedGenusesCount > 0;
                        GenusesAvailable = GenusesCount > 0;
                        GenusesNotAvailable = !GenusesAvailable;
                        if (AnalysedGenusesCount > 0)
                        {
                            GenusesVistaGenomicsValue = (int)((from genusEntry in source
                                                               where genusEntry.Value.AnalysisComplete
                                                               select genusEntry.Value.VistaGenomicsValue).Sum());
                        }
                        PredictedSpecies = from species in planet.PredictedSpecies
                                           select new GenusClassificationViewModel(species, planet) into speciesVm
                                           orderby speciesVm.VistaGenomicsBaseValueSort descending
                                           select speciesVm;
                        PredictedSpeciesCount = PredictedSpecies.Count();
                        HasValuablePredictedSpecies = PredictedSpecies.Any(vm => vm.IsValuable);
                        PredictedSpeciesAvailable = PredictedSpeciesCount > 0;
                        PredictedSpeciesNotAvailable = !PredictedSpeciesAvailable;
                    }
                }
            }
            if (planet.MatchingPlanetClassifications.Count() != 0)
            {
                MatchingPlanetClassifications = from item in planet.MatchingPlanetClassifications
                                                select new PlanetClassificationViewModel(item) into item
                                                orderby item.Name
                                                select item;
                MatchingPlanetClassificationsSort = MatchingPlanetClassifications.Count();
                MatchingPlanetClassificationsCount = MatchingPlanetClassificationsSort.ToString();
                MatchingPlanetClassificationsAvailable = MatchingPlanetClassificationsSort > 0;
            }
        }
        else if (_body.Type == BodyType.Star)
        {
            Star star = (Star)_body;
            TypeHumanReadable = star.StarType + (WasReadFromEdsmOnly ? string.Empty : " " + Resources.WordStar);
            if (star.Mass > 0.0)
            {
                Mass = $"{star.Mass:n4} SM";
            }
        }
        else
        {
            TypeHumanReadable = string.Empty;
        }
        HasMass = !string.IsNullOrWhiteSpace(Mass);
    }
}
