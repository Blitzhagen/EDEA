using System;
using EDEA.Models;
using EDEA.Services;

namespace EDEA.ViewModels;

/// <summary>
/// View model that wraps a <see cref="Genus"/> for display in the biologicals table.
/// </summary>
public class GenusViewModel : ViewModelBase
{
    /// <summary>
    /// The underlying genus model.
    /// </summary>
    private readonly Genus _genus;

    /// <summary>
    /// Gets the genus name.
    /// </summary>
    /// <value>The genus name.</value>
    public string Name => _genus.Name;

    /// <summary>
    /// Gets the species name.
    /// </summary>
    /// <value>The species name.</value>
    public string Species => _genus.Species;

    /// <summary>
    /// Gets the short species name.
    /// </summary>
    /// <value>The short species name.</value>
    public string SpeciesShort => _genus.SpeciesShort;

    /// <summary>
    /// Gets the variant name.
    /// </summary>
    /// <value>The variant name.</value>
    public string Variant => _genus.Variant;

    /// <summary>
    /// Gets the short variant name.
    /// </summary>
    /// <value>The short variant name.</value>
    public string VariantShort => _genus.VariantShort;

    /// <summary>
    /// Gets the scan count formatted for display.
    /// </summary>
    /// <value>The scan count string.</value>
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

    /// <summary>
    /// Gets a value indicating whether the analysis is complete.
    /// </summary>
    /// <value><c>true</c> if the analysis is complete; otherwise, <c>false</c>.</value>
    public bool AnalysisComplete => _genus.AnalysisComplete;

    /// <summary>
    /// Gets a value indicating whether the analysis is not complete.
    /// </summary>
    /// <value><c>true</c> if the analysis is not complete; otherwise, <c>false</c>.</value>
    public bool AnalysisNotComplete => !_genus.AnalysisComplete;

    /// <summary>
    /// Gets the scan count as a string, always showing the numeric value.
    /// </summary>
    /// <value>The scan count string, including zero.</value>
    public string ScanCountDisplay => _genus.ScanCount.ToString();

    /// <summary>
    /// Gets the formatted Vista Genomics value, always showing the numeric value.
    /// </summary>
    /// <value>The formatted value string, including zero.</value>
    public string VistaGenomicsValueDisplay => $"{_genus.VistaGenomicsValue:n0} {"Cr"}";

    /// <summary>
    /// Gets the Vista Genomics base value used for sorting.
    /// </summary>
    /// <value>The sortable base value.</value>
    public int VistaGenomicsBaseValueSort => _genus.VistaGenomicsBaseValue;

    /// <summary>
    /// Gets the formatted Vista Genomics base value.
    /// </summary>
    /// <value>The formatted base value.</value>
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

    /// <summary>
    /// Gets the Vista Genomics first discovery bonus value used for sorting.
    /// </summary>
    /// <value>The sortable first discovery bonus value.</value>
    public int VistaGenomicsFirstDiscoveryBonusValueSort => _genus.VistaGenomicsFirstDiscoveryBonusValue;

    /// <summary>
    /// Gets the formatted Vista Genomics first discovery bonus value.
    /// </summary>
    /// <value>The formatted first discovery bonus value.</value>
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

    /// <summary>
    /// Gets the Vista Genomics maximum value used for sorting.
    /// </summary>
    /// <value>The sortable maximum value.</value>
    public int VistaGenomicsMaxValueSort => _genus.VistaGenomicsMaxValue;

    /// <summary>
    /// Gets the formatted Vista Genomics maximum value.
    /// </summary>
    /// <value>The formatted maximum value.</value>
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

    /// <summary>
    /// Gets the Vista Genomics value used for sorting.
    /// </summary>
    /// <value>The sortable value.</value>
    public int VistaGenomicsValueSort => (int)_genus.VistaGenomicsValue;

    /// <summary>
    /// Gets the formatted Vista Genomics value.
    /// </summary>
    /// <value>The formatted value.</value>
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

    /// <summary>
    /// Gets a value indicating whether this genus is valuable.
    /// </summary>
    /// <value><c>true</c> if this genus is valuable; otherwise, <c>false</c>.</value>
    public bool IsValuableGenus => _genus.VistaGenomicsMaxValue >= Preferences.Other.ValuableGenusThreshold;

    /// <summary>
    /// Gets a value indicating whether this genus is a first discovery.
    /// </summary>
    /// <value><c>true</c> if this genus is a first discovery; otherwise, <c>false</c>.</value>
    public bool IsFirstDiscovery => _genus.IsFirstDiscovery;

    /// <summary>
    /// Gets a value indicating whether this genus is a first discovery and the analysis is complete.
    /// </summary>
    /// <value><c>true</c> if this is a first discovery and analysis is complete; otherwise, <c>false</c>.</value>
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

    /// <summary>
    /// Gets a value indicating whether this genus is a first discovery and the species is set.
    /// </summary>
    /// <value><c>true</c> if this is a first discovery and species is set; otherwise, <c>false</c>.</value>
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

    /// <summary>
    /// Gets the longitude of the first scan.
    /// </summary>
    /// <value>The first scan longitude, or <c>null</c>.</value>
    public double? LongitudeAt1stScan => _genus.LongitudeAt1stScan;

    /// <summary>
    /// Gets the latitude of the first scan.
    /// </summary>
    /// <value>The first scan latitude, or <c>null</c>.</value>
    public double? LatitudeAt1stScan => _genus.LatitudeAt1stScan;

    /// <summary>
    /// Gets the formatted location of the first scan.
    /// </summary>
    /// <value>The first scan location string.</value>
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

    /// <summary>
    /// Gets the longitude of the second scan.
    /// </summary>
    /// <value>The second scan longitude, or <c>null</c>.</value>
    public double? LongitudeAt2ndScan => _genus.LongitudeAt2ndScan;

    /// <summary>
    /// Gets the latitude of the second scan.
    /// </summary>
    /// <value>The second scan latitude, or <c>null</c>.</value>
    public double? LatitudeAt2ndScan => _genus.LatitudeAt2ndScan;

    /// <summary>
    /// Gets the formatted location of the second scan.
    /// </summary>
    /// <value>The second scan location string.</value>
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

    /// <summary>
    /// Gets a value indicating whether this genus is in analysis.
    /// </summary>
    /// <value><c>true</c> if the genus is in analysis; otherwise, <c>false</c>.</value>
    public bool IsInAnalysis => _genus.IsInAnalysis;

    /// <summary>
    /// Gets a value indicating whether the species is set.
    /// </summary>
    /// <value><c>true</c> if the species is set; otherwise, <c>false</c>.</value>
    public bool SpeciesSet => _genus.SpeciesSet;

    /// <summary>
    /// Gets a value indicating whether the species is not set.
    /// </summary>
    /// <value><c>true</c> if the species is not set; otherwise, <c>false</c>.</value>
    public bool SpeciesNotSet => !_genus.SpeciesSet;

    /// <summary>
    /// Gets the clonal colony range used for sorting.
    /// </summary>
    /// <value>The sortable clonal colony range.</value>
    public int ClonalColonyRangeSort => _genus.ClonalColonyRange;

    /// <summary>
    /// Gets the formatted clonal colony range.
    /// </summary>
    /// <value>The formatted clonal colony range.</value>
    public string ClonalColonyRange
    {
        get
        {
            int range = _genus.ClonalColonyRange != 0 ? _genus.ClonalColonyRange : GeneraIndexProvider.GetClonalColonyRangeForGenus(_genus.Name);
            if (range != 0)
            {
                return $"{range:n0} m";
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the current distance to the first scan is available.
    /// </summary>
    /// <value><c>true</c> if the distance is available; otherwise, <c>false</c>.</value>
    public bool CurrentDistanceToLocationAt1stScanAvailable => _genus.CurrentDistanceToLocationAt1stScanAvailable;

    /// <summary>
    /// Gets the current distance to the first scan location used for sorting.
    /// </summary>
    /// <value>The sortable distance, or <c>null</c>.</value>
    public int? CurrentDistanceToLocationAt1stScanSort => _genus.CurrentDistanceToLocationAt1stScan;

    /// <summary>
    /// Gets the formatted current distance to the first scan location.
    /// </summary>
    /// <value>The formatted distance string.</value>
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

    /// <summary>
    /// Gets a value indicating whether the current distance to the second scan is available.
    /// </summary>
    /// <value><c>true</c> if the distance is available; otherwise, <c>false</c>.</value>
    public bool CurrentDistanceToLocationAt2ndScanAvailable => _genus.CurrentDistanceToLocationAt2ndScanAvailable;

    /// <summary>
    /// Gets the current distance to the second scan location used for sorting.
    /// </summary>
    /// <value>The sortable distance, or <c>null</c>.</value>
    public int? CurrentDistanceToLocationAt2ndScanSort => _genus.CurrentDistanceToLocationAt2ndScan;

    /// <summary>
    /// Gets the formatted current distance to the second scan location.
    /// </summary>
    /// <value>The formatted distance string.</value>
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

    /// <summary>
    /// Gets a value indicating whether the first scan is out of clonal colony range.
    /// </summary>
    /// <value><c>true</c> if out of range, <c>false</c> if in range, or <c>null</c> if unknown.</value>
    public bool? Is1stScanOutOfClonalColonyRange => _genus.Is1stScanOutOfClonalColonyRange;

    /// <summary>
    /// Gets a value indicating whether the second scan is out of clonal colony range.
    /// </summary>
    /// <value><c>true</c> if out of range, <c>false</c> if in range, or <c>null</c> if unknown.</value>
    public bool? Is2ndScanOutOfClonalColonyRange => _genus.Is2ndScanOutOfClonalColonyRange;

    /// <summary>
    /// Gets a value indicating whether the current scan is out of clonal colony range.
    /// </summary>
    /// <value><c>true</c> if out of range, <c>false</c> if in range, or <c>null</c> if unknown.</value>
    public bool? IsOutOfClonalColonyRange => _genus.IsOutOfClonalColonyRange;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenusViewModel"/> class.
    /// </summary>
    /// <param name="genus">The genus model to wrap.</param>
    public GenusViewModel(Genus genus)
    {
        _genus = genus;
    }
}
