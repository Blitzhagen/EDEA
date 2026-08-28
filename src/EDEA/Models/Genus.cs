using System;
using EDEA.Services;

namespace EDEA.Models;

public class Genus
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public long StarSystemId { get; set; }

    public long SystemId64
    {
        get => StarSystemId;
        set => StarSystemId = value;
    }

    public int BodyId { get; set; }

    public string Species { get; set; } = string.Empty;

    public string Variant { get; set; } = string.Empty;

    public string SpeciesShort => Species.Replace(Name, "").Trim();

    public string VariantShort
    {
        get
        {
            if (!string.IsNullOrEmpty(Species))
            {
                return Variant.Replace(Species, "").Replace("-", "").Trim();
            }
            return string.Empty;
        }
    }

    public int ScanCount { get; set; }

    public bool AnalysisComplete { get; set; }

    public bool IsAnalysed
    {
        get => AnalysisComplete;
        set => AnalysisComplete = value;
    }

    public int VistaGenomicsBaseValue => GeneraIndexProvider.GetVistaGenomicsValueForSpecies(Species);

    public int VistaGenomicsFirstDiscoveryBonusValue => 4 * VistaGenomicsBaseValue;

    public int VistaGenomicsMaxValue
    {
        get
        {
            if (!IsFirstDiscovery)
            {
                return VistaGenomicsBaseValue;
            }
            return VistaGenomicsBaseValue + VistaGenomicsFirstDiscoveryBonusValue;
        }
    }

    public decimal VistaGenomicsValue
    {
        get
        {
            if (!AnalysisComplete)
            {
                return 0;
            }
            if (_vistaGenomicsValue != 0)
            {
                return _vistaGenomicsValue;
            }
            return VistaGenomicsMaxValue;
        }
        set => _vistaGenomicsValue = value;
    }

    private decimal _vistaGenomicsValue;

    public bool IsFirstDiscovery { get; set; }

    public bool IsFirstDiscovered
    {
        get => IsFirstDiscovery;
        set => IsFirstDiscovery = value;
    }

    public bool? WasLogged { get; set; }

    public double? LongitudeAt1stScan { get; set; }

    public double? LatitudeAt1stScan { get; set; }

    public double? LongitudeAt2ndScan { get; set; }

    public double? LatitudeAt2ndScan { get; set; }

    public bool IsInAnalysis
    {
        get
        {
            if (ScanCount > 0)
            {
                return !AnalysisComplete;
            }
            return false;
        }
    }

    public bool CurrentDistanceToLocationAt1stScanAvailable => CurrentDistanceToLocationAt1stScan.HasValue;

    public int? CurrentDistanceToLocationAt1stScan { get; set; }

    public bool CurrentDistanceToLocationAt2ndScanAvailable => CurrentDistanceToLocationAt2ndScan.HasValue;

    public int? CurrentDistanceToLocationAt2ndScan { get; set; }

    public bool? Is1stScanOutOfClonalColonyRange
    {
        get
        {
            if (!CurrentDistanceToLocationAt1stScanAvailable)
            {
                return null;
            }
            return CurrentDistanceToLocationAt1stScan > ClonalColonyRange;
        }
    }

    public bool? Is2ndScanOutOfClonalColonyRange
    {
        get
        {
            if (!CurrentDistanceToLocationAt2ndScanAvailable)
            {
                return null;
            }
            return CurrentDistanceToLocationAt2ndScan > ClonalColonyRange;
        }
    }

    public bool? IsOutOfClonalColonyRange
    {
        get
        {
            if (!CurrentDistanceToLocationAt1stScanAvailable)
            {
                return null;
            }
            return !CurrentDistanceToLocationAt2ndScanAvailable
                ? CurrentDistanceToLocationAt1stScan > ClonalColonyRange
                : CurrentDistanceToLocationAt1stScan > ClonalColonyRange && CurrentDistanceToLocationAt2ndScan > ClonalColonyRange;
        }
    }

    public bool SpeciesSet => !string.IsNullOrWhiteSpace(Species);

    public int ClonalColonyRange { get; set; }

    public Genus()
    {
        ScanCount = 0;
        AnalysisComplete = false;
        IsFirstDiscovery = false;
    }

    public Genus(string name, long starSystemId, int bodyId, bool? wasLogged, string species = "", string variant = "")
    {
        Name = name;
        BodyId = bodyId;
        StarSystemId = starSystemId;
        Species = species;
        Variant = variant;
        ScanCount = 0;
        AnalysisComplete = false;
        IsFirstDiscovery = false;
        WasLogged = wasLogged;
        LongitudeAt1stScan = null;
        LatitudeAt1stScan = null;
        LongitudeAt2ndScan = null;
        LatitudeAt2ndScan = null;
        CurrentDistanceToLocationAt1stScan = null;
        CurrentDistanceToLocationAt2ndScan = null;
        ClonalColonyRange = GeneraIndexProvider.GetClonalColonyRangeForGenus(Name);
    }

    public Genus(string name, long bodyId, long starSystemId, string species, string variant, long scanCount, long analysisComplete, long vistaGenomicsValue, double? longitudeAt1stScan, double? latitudeAt1stScan, double? longitudeAt2ndScan, double? latitudeAt2ndScan, long vistaGenomicsMaxValue, long vistaGenomicsBaseValue, long vistaGenomicsFirstDiscoveryBonusValue, long isFirstDiscovery, long? wasLogged)
    {
        Name = name;
        BodyId = Convert.ToInt32(bodyId);
        StarSystemId = starSystemId;
        Species = species;
        Variant = variant;
        ScanCount = Convert.ToInt32(scanCount);
        AnalysisComplete = Convert.ToBoolean(analysisComplete);
        IsFirstDiscovery = Convert.ToBoolean(isFirstDiscovery);
        WasLogged = wasLogged.HasValue ? Convert.ToBoolean(wasLogged) : null;
        LongitudeAt1stScan = longitudeAt1stScan;
        LatitudeAt1stScan = latitudeAt1stScan;
        LongitudeAt2ndScan = longitudeAt2ndScan;
        LatitudeAt2ndScan = latitudeAt2ndScan;
        CurrentDistanceToLocationAt1stScan = null;
        CurrentDistanceToLocationAt2ndScan = null;
        _vistaGenomicsValue = vistaGenomicsValue;
        ClonalColonyRange = GeneraIndexProvider.GetClonalColonyRangeForGenus(Name);
    }

    public void ResetAnalysisData()
    {
        if (!AnalysisComplete && ScanCount > 0)
        {
            ScanCount = 0;
            LongitudeAt1stScan = null;
            LatitudeAt1stScan = null;
            LongitudeAt2ndScan = null;
            LatitudeAt2ndScan = null;
            CurrentDistanceToLocationAt1stScan = null;
            CurrentDistanceToLocationAt2ndScan = null;
            IsFirstDiscovery = false;
        }
    }
}
