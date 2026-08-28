using EDEA.Models;
using EDEA.Properties;

namespace EDEA.ViewModels;

/// <summary>
/// View model that wraps a <see cref="GenusClassification"/> for display.
/// </summary>
public class GenusClassificationViewModel : ViewModelBase
{
    /// <summary>
    /// The underlying genus classification model.
    /// </summary>
    private readonly GenusClassification _genusClassification;

    /// <summary>
    /// The planet associated with the classification.
    /// </summary>
    private readonly Planet _planet;

    /// <summary>
    /// Gets the genus name.
    /// </summary>
    /// <value>The genus name.</value>
    public string Name => _genusClassification.Name;

    /// <summary>
    /// Gets the species name.
    /// </summary>
    /// <value>The species name.</value>
    public string Species => _genusClassification.Species;

    /// <summary>
    /// Gets the short species name.
    /// </summary>
    /// <value>The short species name.</value>
    public string SpeciesShort => _genusClassification.SpeciesShort;

    /// <summary>
    /// Gets the variant name.
    /// </summary>
    /// <value>The variant name.</value>
    public string Variant => _genusClassification.Variant;

    /// <summary>
    /// Gets the short variant name.
    /// </summary>
    /// <value>The short variant name.</value>
    public string VariantShort => _genusClassification.VariantShort;

    /// <summary>
    /// Gets the Vista Genomics base value used for sorting.
    /// </summary>
    /// <value>The sortable Vista Genomics base value.</value>
    public int VistaGenomicsBaseValueSort => _genusClassification.VistaGenomicsBaseValue;

    /// <summary>
    /// Gets the formatted Vista Genomics base value.
    /// </summary>
    /// <value>The formatted Vista Genomics base value.</value>
    public string VistaGenomicsBaseValue
    {
        get
        {
            if (_genusClassification.VistaGenomicsBaseValue != 0)
            {
                return _genusClassification.VistaGenomicsBaseValue.ToString("n0") + " " + Resources.UnitCredits;
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// Gets the Vista Genomics first discovery bonus value used for sorting.
    /// </summary>
    /// <value>The sortable first discovery bonus value.</value>
    public int VistaGenomicsFirstDiscoveryBonusValueSort => 4 * _genusClassification.VistaGenomicsBaseValue;

    /// <summary>
    /// Gets the formatted Vista Genomics first discovery bonus value.
    /// </summary>
    /// <value>The formatted first discovery bonus value.</value>
    public string VistaGenomicsFirstDiscoveryBonusValue
    {
        get
        {
            if (VistaGenomicsFirstDiscoveryBonusValueSort != 0)
            {
                return VistaGenomicsFirstDiscoveryBonusValueSort.ToString("n0") + " " + Resources.UnitCredits;
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// Gets the maximum Vista Genomics value used for sorting.
    /// </summary>
    /// <value>The sortable maximum value.</value>
    public int VistaGenomicsMaxValueSort
    {
        get
        {
            if (!IsFirstDiscovery)
            {
                return VistaGenomicsBaseValueSort;
            }
            return VistaGenomicsFirstDiscoveryBonusValueSort + VistaGenomicsBaseValueSort;
        }
    }

    /// <summary>
    /// Gets the formatted maximum Vista Genomics value.
    /// </summary>
    /// <value>The formatted maximum value.</value>
    public string VistaGenomicsMaxValue
    {
        get
        {
            if (VistaGenomicsMaxValueSort != 0)
            {
                return VistaGenomicsMaxValueSort.ToString("n0") + " " + Resources.UnitCredits;
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// Gets the clonal colony range used for sorting.
    /// </summary>
    /// <value>The sortable clonal colony range.</value>
    public int ClonalColonyRangeSort => _genusClassification.ClonalColonyRange;

    /// <summary>
    /// Gets the formatted clonal colony range.
    /// </summary>
    /// <value>The formatted clonal colony range.</value>
    public string ClonalColonyRange
    {
        get
        {
            if (_genusClassification.ClonalColonyRange != 0)
            {
                return _genusClassification.ClonalColonyRange.ToString("n0") + " " + Resources.UnitMeters;
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this classification is a first discovery.
    /// </summary>
    /// <value><c>true</c> if this is a first discovery; otherwise, <c>false</c>.</value>
    public bool IsFirstDiscovery
    {
        get
        {
            if (_planet == null)
            {
                return false;
            }
            return !_planet.WasMapped;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this classification is valuable.
    /// </summary>
    /// <value><c>true</c> if this classification is valuable; otherwise, <c>false</c>.</value>
    public bool IsValuable => VistaGenomicsMaxValueSort >= Preferences.Other.ValuableGenusThreshold;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenusClassificationViewModel"/> class.
    /// </summary>
    /// <param name="genusClassification">The genus classification model.</param>
    /// <param name="planet">The planet associated with the classification.</param>
    public GenusClassificationViewModel(GenusClassification genusClassification, Planet planet)
    {
        _genusClassification = genusClassification;
        _planet = planet;
    }
}
