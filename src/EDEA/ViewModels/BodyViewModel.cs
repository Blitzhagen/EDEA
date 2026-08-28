using System;
using System.Collections.Generic;
using System.Linq;
using EDEA.Models;
using EDEA.Properties;

namespace EDEA.ViewModels;

public class BodyViewModel : ViewModelBase
{
    private readonly Body _body;

    public string Name => _body.Name;

    public string ShortName => _body.ShortName;

    public string StarSystemName => _body.StarSystem?.Name ?? string.Empty;

    public int Id => _body.Id;

    public string Distance { get; }

    public int DistanceSort { get; }

    public bool WasDiscovered => _body.WasDiscovered;

    public bool NewDiscovery => !_body.WasDiscovered;

    public bool WasReadFromJournal => _body.WasReadFromJournal;

    public bool WasReadFromEdsm => _body.WasReadFromEdsm;

    public bool WasReadFromEdsmOnly { get; }

    public bool IsValuableBody { get; }

    public bool IsPlanetOrStar { get; }

    public bool IsPlanet { get; }

    public bool IsStar { get; }

    public bool IsScoopableStar { get; }

    public bool IsNoScoopableStar { get; }

    public BodyType Type => _body.Type;

    public string TypeHumanReadable { get; }

    public bool Terraformable { get; }

    public bool WasMapped { get; }

    public bool WasNotMapped { get; }

    public bool SurfaceScanned { get; }

    public SurfaceScanStatus SurfaceScanStatus { get; }

    public bool EfficientlyScanned { get; }

    public double GravitySort { get; }

    public string Gravity { get; } = string.Empty;

    public bool HasGravity { get; }

    public string Atmosphere { get; } = string.Empty;

    public string Radius { get; } = string.Empty;

    public double RadiusSort { get; }

    public bool HasRadius { get; }

    public double SurfaceTemperatureSort { get; }

    public string SurfaceTemperature { get; } = string.Empty;

    public string Volcanism { get; } = string.Empty;

    public bool Landable { get; }

    public bool Touchdown { get; }

    public bool HasRings { get; }

    public string RingsCount { get; } = string.Empty;

    public int RingsSort { get; }

    public RingReserveLevel RingsReserveLevel { get; }

    public string RingsTotalWidth { get; } = string.Empty;

    public IEnumerable<RingViewModel> Rings { get; } = Array.Empty<RingViewModel>();

    public bool HasGeologicals { get; }

    public string GeologicalsCount { get; } = string.Empty;

    public int GeologicalsSort { get; }

    public bool HasBiologicals { get; }

    public string BiologicalsCount { get; } = string.Empty;

    public int BiologicalsSort { get; }

    public IEnumerable<GenusViewModel> Genuses { get; } = Array.Empty<GenusViewModel>();

    public int GenusesCount { get; }

    public string EdsmDiscoveryCommander { get; } = string.Empty;

    public bool IsCurrentPlanetInSystem { get; }

    public bool LandableNoTouchdown { get; }

    public bool AllGenusesAnalysed { get; }

    public bool GenusesPartlyAnalysed { get; }

    public int AnalysedGenusesCount { get; }

    public bool GenusesAvailable { get; }

    public bool GenusesNotAvailable { get; }

    public int GenusesVistaGenomicsValue { get; }

    public bool HasValuableGenera { get; }

    public IEnumerable<GenusClassificationViewModel> PredictedSpecies { get; } = Array.Empty<GenusClassificationViewModel>();

    public int PredictedSpeciesCount { get; }

    public bool PredictedSpeciesAvailable { get; }

    public bool PredictedSpeciesNotAvailable { get; }

    public bool HasValuablePredictedSpecies { get; }

    public IEnumerable<PlanetClassificationViewModel> MatchingPlanetClassifications { get; } = Array.Empty<PlanetClassificationViewModel>();

    public string MatchingPlanetClassificationsCount { get; } = string.Empty;

    public int MatchingPlanetClassificationsSort { get; }

    public bool MatchingPlanetClassificationsAvailable { get; }

    public int? ParentStarId { get; }

    public Star? ParentStar { get; }

    public int? ParentPlanetId { get; }

    public Planet? ParentPlanet { get; }

    public string Mass { get; } = string.Empty;

    public double MassSort { get; }

    public bool HasMass { get; }

    public string OrbitalInclination { get; } = string.Empty;

    public double OrbitalInclinationSort { get; }

    public bool HasOrbitalInclination { get; }

    public string CartographicValue { get; }

    public double CartographicValueSort { get; }

    public bool HasCartographicValue { get; }

    public string CartographicMaxValue { get; }

    public double CartographicMaxValueSort { get; }

    public string CartographicBaseValue { get; }

    public string CartographicSurfaceScanValue { get; } = string.Empty;

    public string CartographicFirstSurfaceScanBonusValue { get; } = string.Empty;

    public string CartographicFirstDiscoveryBonusValue { get; } = string.Empty;

    public string CartographicFirstDiscoveryBonusRange { get; } = string.Empty;

    public string CartographicEfficientlyScannedBonusValue { get; } = string.Empty;

    public bool CartographicMaxValueReached { get; }

    public bool CartographicMaxValueNotReached { get; }

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
