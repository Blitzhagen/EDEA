using System;
using EDEA.Models;

namespace EDEA.ViewModels;

public class GenusViewModel : ViewModelBase
{
    private readonly Genus _genus;

    public string Name => _genus.Name;

    public string Species => _genus.Species;

    public string SpeciesShort => _genus.SpeciesShort;

    public string Variant => _genus.Variant;

    public string VariantShort => _genus.VariantShort;

    public string ScanCount
    {
        get
        {
            if (_genus.ScanCount != 0)
            {
                return Convert.ToString(_genus.ScanCount);
            }
            return string.Empty;
        }
    }

    public bool AnalysisComplete => _genus.AnalysisComplete;

    public bool AnalysisNotComplete => !_genus.AnalysisComplete;

    public int VistaGenomicsBaseValueSort => _genus.VistaGenomicsBaseValue;

    public string VistaGenomicsBaseValue
    {
        get
        {
            if (_genus.VistaGenomicsBaseValue != 0)
            {
                return $"{_genus.VistaGenomicsBaseValue:n0} {"Cr"}";
            }
            return string.Empty;
        }
    }

    public int VistaGenomicsFirstDiscoveryBonusValueSort => _genus.VistaGenomicsFirstDiscoveryBonusValue;

    public string VistaGenomicsFirstDiscoveryBonusValue
    {
        get
        {
            if (_genus.VistaGenomicsFirstDiscoveryBonusValue != 0)
            {
                return $"{_genus.VistaGenomicsFirstDiscoveryBonusValue:n0} {"Cr"}";
            }
            return string.Empty;
        }
    }

    public int VistaGenomicsMaxValueSort => _genus.VistaGenomicsMaxValue;

    public string VistaGenomicsMaxValue
    {
        get
        {
            if (_genus.VistaGenomicsMaxValue != 0)
            {
                return $"{_genus.VistaGenomicsMaxValue:n0} {"Cr"}";
            }
            return string.Empty;
        }
    }

    public int VistaGenomicsValueSort => (int)_genus.VistaGenomicsValue;

    public string VistaGenomicsValue
    {
        get
        {
            if (_genus.VistaGenomicsValue != 0)
            {
                return $"{_genus.VistaGenomicsValue:n0} {"Cr"}";
            }
            return string.Empty;
        }
    }

    public bool IsValuableGenus => _genus.VistaGenomicsMaxValue >= Preferences.Other.ValuableGenusThreshold;

    public bool IsFirstDiscovery => _genus.IsFirstDiscovery;

    public bool IsFirstDiscoveryAndAnalysisComplete
    {
        get
        {
            if (AnalysisComplete)
            {
                return IsFirstDiscovery;
            }
            return false;
        }
    }

    public bool IsFirstDiscoveryAndSpeciesSet
    {
        get
        {
            if (SpeciesSet)
            {
                return IsFirstDiscovery;
            }
            return false;
        }
    }

    public double? LongitudeAt1stScan => _genus.LongitudeAt1stScan;

    public double? LatitudeAt1stScan => _genus.LatitudeAt1stScan;

    public string LocationAt1stScan
    {
        get
        {
            if (!_genus.LongitudeAt1stScan.HasValue || !_genus.LatitudeAt1stScan.HasValue)
            {
                return string.Empty;
            }
            return $"{_genus.LongitudeAt1stScan}, {_genus.LatitudeAt1stScan}";
        }
    }

    public double? LongitudeAt2ndScan => _genus.LongitudeAt2ndScan;

    public double? LatitudeAt2ndScan => _genus.LatitudeAt2ndScan;

    public string LocationAt2ndScan
    {
        get
        {
            if (!_genus.LongitudeAt2ndScan.HasValue || !_genus.LatitudeAt2ndScan.HasValue)
            {
                return string.Empty;
            }
            return $"{_genus.LongitudeAt2ndScan}, {_genus.LatitudeAt2ndScan}";
        }
    }

    public bool IsInAnalysis => _genus.IsInAnalysis;

    public bool SpeciesSet => _genus.SpeciesSet;

    public bool SpeciesNotSet => !_genus.SpeciesSet;

    public int ClonalColonyRangeSort => _genus.ClonalColonyRange;

    public string ClonalColonyRange
    {
        get
        {
            if (_genus.ClonalColonyRange != 0)
            {
                return $"{_genus.ClonalColonyRange:n0} m";
            }
            return string.Empty;
        }
    }

    public bool CurrentDistanceToLocationAt1stScanAvailable => _genus.CurrentDistanceToLocationAt1stScanAvailable;

    public int? CurrentDistanceToLocationAt1stScanSort => _genus.CurrentDistanceToLocationAt1stScan;

    public string CurrentDistanceToLocationAt1stScan
    {
        get
        {
            if (!CurrentDistanceToLocationAt1stScanAvailable)
            {
                return string.Empty;
            }
            if (CurrentDistanceToLocationAt1stScanSort!.Value < 10000)
            {
                return $"{CurrentDistanceToLocationAt1stScanSort!.Value:n0} m";
            }
            return $"{CurrentDistanceToLocationAt1stScanSort!.Value / 1000:n0} km";
        }
    }

    public bool CurrentDistanceToLocationAt2ndScanAvailable => _genus.CurrentDistanceToLocationAt2ndScanAvailable;

    public int? CurrentDistanceToLocationAt2ndScanSort => _genus.CurrentDistanceToLocationAt2ndScan;

    public string CurrentDistanceToLocationAt2ndScan
    {
        get
        {
            if (!CurrentDistanceToLocationAt2ndScanAvailable)
            {
                return string.Empty;
            }
            if (CurrentDistanceToLocationAt2ndScanSort!.Value < 10000)
            {
                return $"{CurrentDistanceToLocationAt2ndScanSort!.Value:n0} m";
            }
            return $"{CurrentDistanceToLocationAt2ndScanSort!.Value / 1000:n0} km";
        }
    }

    public bool? Is1stScanOutOfClonalColonyRange => _genus.Is1stScanOutOfClonalColonyRange;

    public bool? Is2ndScanOutOfClonalColonyRange => _genus.Is2ndScanOutOfClonalColonyRange;

    public bool? IsOutOfClonalColonyRange => _genus.IsOutOfClonalColonyRange;

    public GenusViewModel(Genus genus)
    {
        _genus = genus;
    }
}
