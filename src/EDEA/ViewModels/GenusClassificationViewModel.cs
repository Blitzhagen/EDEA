using EDEA.Models;
using EDEA.Properties;

namespace EDEA.ViewModels;

public class GenusClassificationViewModel : ViewModelBase
{
    private readonly GenusClassification _genusClassification;

    private readonly Planet _planet;

    public string Name => _genusClassification.Name;

    public string Species => _genusClassification.Species;

    public string SpeciesShort => _genusClassification.SpeciesShort;

    public string Variant => _genusClassification.Variant;

    public string VariantShort => _genusClassification.VariantShort;

    public int VistaGenomicsBaseValueSort => _genusClassification.VistaGenomicsBaseValue;

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

    public int VistaGenomicsFirstDiscoveryBonusValueSort => 4 * _genusClassification.VistaGenomicsBaseValue;

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

    public int ClonalColonyRangeSort => _genusClassification.ClonalColonyRange;

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

    public bool IsValuable => VistaGenomicsMaxValueSort >= Preferences.Other.ValuableGenusThreshold;

    public GenusClassificationViewModel(GenusClassification genusClassification, Planet planet)
    {
        _genusClassification = genusClassification;
        _planet = planet;
    }
}
