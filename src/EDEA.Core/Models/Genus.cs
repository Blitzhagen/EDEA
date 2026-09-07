using System;
using EDEA.Services;

namespace EDEA.Models;

/// <summary>
/// Represents a biological genus with scanning and analysis data.
/// </summary>
public class Genus
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The genus identifier.</value>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The genus name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the codex genus identifier (e.g. "$Codex_Ent_Bacterial_Genus_Name;").
    /// </summary>
    /// <value>The language-independent codex key from the journal "Genus" field.</value>
    public string CodexKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the star system identifier.
    /// </summary>
    /// <value>The star system identifier.</value>
    public long StarSystemId { get; set; }

    /// <summary>
    /// Gets or sets the star system identifier alias.
    /// </summary>
    /// <value>The star system identifier alias.</value>
    public long SystemId64
    {
        get => StarSystemId;
        set => StarSystemId = value;
    }

    /// <summary>
    /// Gets or sets the body identifier.
    /// </summary>
    /// <value>The body identifier.</value>
    public int BodyId { get; set; }

    /// <summary>
    /// Gets or sets the species.
    /// </summary>
    /// <value>The species name.</value>
    public string Species { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variant.
    /// </summary>
    /// <value>The variant name.</value>
    public string Variant { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the codex species identifier (e.g. "$Codex_Ent_Bacterial_01_Name;").
    /// </summary>
    /// <value>The language-independent codex key from the journal "Species" field.</value>
    public string SpeciesKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the codex variant identifier.
    /// </summary>
    /// <value>The language-independent codex key from the journal "Variant" field.</value>
    public string VariantKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets the short species name.
    /// </summary>
    /// <value>The short species name.</value>
    public string SpeciesShort => Species.Replace(Name, "").Trim();

    /// <summary>
    /// Gets the short variant name.
    /// </summary>
    /// <value>The short variant name.</value>
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

    /// <summary>
    /// Gets or sets the current scan count.
    /// </summary>
    /// <value>The scan count.</value>
    public int ScanCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether analysis is complete.
    /// </summary>
    /// <value><see langword="true"/> if analysis is complete; otherwise, <see langword="false"/>.</value>
    public bool AnalysisComplete { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the genus has been analysed.
    /// </summary>
    /// <value><see langword="true"/> if analysed; otherwise, <see langword="false"/>.</value>
    public bool IsAnalysed
    {
        get => AnalysisComplete;
        set => AnalysisComplete = value;
    }

    /// <summary>
    /// Gets the base Vista Genomics value for this species.
    /// </summary>
    /// <value>The base Vista Genomics value.</value>
    public int VistaGenomicsBaseValue => BiologyCatalogProvider.GetVistaGenomicsValueForSpecies(!string.IsNullOrEmpty(SpeciesKey) ? SpeciesKey : Species);

    /// <summary>
    /// Gets the first discovery bonus for Vista Genomics.
    /// </summary>
    /// <value>The first discovery bonus value.</value>
    public int VistaGenomicsFirstDiscoveryBonusValue => 4 * VistaGenomicsBaseValue;

    /// <summary>
    /// Gets the maximum Vista Genomics value.
    /// </summary>
    /// <value>The maximum value.</value>
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

    /// <summary>
    /// Gets or sets the Vista Genomics value.
    /// </summary>
    /// <value>The Vista Genomics value.</value>
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

    /// <summary>
    /// The stored Vista Genomics value.
    /// </summary>
    private decimal _vistaGenomicsValue;

    /// <summary>
    /// Gets or sets a value indicating whether this is a first discovery.
    /// </summary>
    /// <value><see langword="true"/> if first discovery; otherwise, <see langword="false"/>.</value>
    public bool IsFirstDiscovery { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a first discovered genus.
    /// </summary>
    /// <value><see langword="true"/> if first discovered; otherwise, <see langword="false"/>.</value>
    public bool IsFirstDiscovered
    {
        get => IsFirstDiscovery;
        set => IsFirstDiscovery = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the genus was logged.
    /// </summary>
    /// <value><see langword="true"/> if logged; <see langword="false"/> if not logged; <see langword="null"/> if unspecified.</value>
    public bool? WasLogged { get; set; }

    /// <summary>
    /// Gets or sets the longitude of the first scan.
    /// </summary>
    /// <value>The longitude, or <see langword="null"/> if not set.</value>
    public double? LongitudeAt1stScan { get; set; }

    /// <summary>
    /// Gets or sets the latitude of the first scan.
    /// </summary>
    /// <value>The latitude, or <see langword="null"/> if not set.</value>
    public double? LatitudeAt1stScan { get; set; }

    /// <summary>
    /// Gets or sets the longitude of the second scan.
    /// </summary>
    /// <value>The longitude, or <see langword="null"/> if not set.</value>
    public double? LongitudeAt2ndScan { get; set; }

    /// <summary>
    /// Gets or sets the latitude of the second scan.
    /// </summary>
    /// <value>The latitude, or <see langword="null"/> if not set.</value>
    public double? LatitudeAt2ndScan { get; set; }

    /// <summary>
    /// Gets a value indicating whether analysis is in progress.
    /// </summary>
    /// <value><see langword="true"/> if analysis is in progress; otherwise, <see langword="false"/>.</value>
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

    /// <summary>
    /// Gets a value indicating whether the current distance to the first scan is available.
    /// </summary>
    /// <value><see langword="true"/> if the distance is available; otherwise, <see langword="false"/>.</value>
    public bool CurrentDistanceToLocationAt1stScanAvailable => CurrentDistanceToLocationAt1stScan.HasValue;

    /// <summary>
    /// Gets or sets the current distance to the first scan location.
    /// </summary>
    /// <value>The distance in meters, or <see langword="null"/> if not available.</value>
    public int? CurrentDistanceToLocationAt1stScan { get; set; }

    /// <summary>
    /// Gets a value indicating whether the current distance to the second scan is available.
    /// </summary>
    /// <value><see langword="true"/> if the distance is available; otherwise, <see langword="false"/>.</value>
    public bool CurrentDistanceToLocationAt2ndScanAvailable => CurrentDistanceToLocationAt2ndScan.HasValue;

    /// <summary>
    /// Gets or sets the current distance to the second scan location.
    /// </summary>
    /// <value>The distance in meters, or <see langword="null"/> if not available.</value>
    public int? CurrentDistanceToLocationAt2ndScan { get; set; }

    /// <summary>
    /// Gets a value indicating whether the first scan is out of clonal colony range.
    /// </summary>
    /// <value><see langword="true"/> if out of range; <see langword="false"/> if in range; <see langword="null"/> if unavailable.</value>
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

    /// <summary>
    /// Gets a value indicating whether the second scan is out of clonal colony range.
    /// </summary>
    /// <value><see langword="true"/> if out of range; <see langword="false"/> if in range; <see langword="null"/> if unavailable.</value>
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

    /// <summary>
    /// Gets a value indicating whether the genus is out of clonal colony range.
    /// </summary>
    /// <value><see langword="true"/> if out of range; <see langword="false"/> if in range; <see langword="null"/> if unavailable.</value>
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

    /// <summary>
    /// Gets a value indicating whether the species is set.
    /// </summary>
    /// <value><see langword="true"/> if the species is set; otherwise, <see langword="false"/>.</value>
    public bool SpeciesSet => !string.IsNullOrWhiteSpace(Species);

    /// <summary>
    /// Gets or sets the clonal colony range in meters.
    /// </summary>
    /// <value>The clonal colony range.</value>
    public int ClonalColonyRange { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Genus"/> class.
    /// </summary>
    public Genus()
    {
        ScanCount = 0;
        AnalysisComplete = false;
        IsFirstDiscovery = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Genus"/> class.
    /// </summary>
    /// <param name="name">The genus name.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="bodyId">The body identifier.</param>
    /// <param name="wasLogged">Whether the genus was logged, or <see langword="null"/> if unspecified.</param>
    /// <param name="species">The species name.</param>
    /// <param name="variant">The variant name.</param>
    /// <param name="codexKey">The codex genus identifier.</param>
    /// <param name="speciesKey">The codex species identifier.</param>
    /// <param name="variantKey">The codex variant identifier.</param>
    public Genus(string name, long starSystemId, int bodyId, bool? wasLogged, string species = "", string variant = "", string codexKey = "", string speciesKey = "", string variantKey = "")
    {
        Name = name;
        CodexKey = codexKey;
        BodyId = bodyId;
        StarSystemId = starSystemId;
        Species = species;
        SpeciesKey = speciesKey;
        Variant = variant;
        VariantKey = variantKey;
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
        ClonalColonyRange = BiologyCatalogProvider.GetClonalColonyRangeForGenus(!string.IsNullOrEmpty(CodexKey) ? CodexKey : Name);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Genus"/> class from persisted data.
    /// </summary>
    /// <param name="name">The genus name.</param>
    /// <param name="bodyId">The body identifier.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="species">The species name.</param>
    /// <param name="variant">The variant name.</param>
    /// <param name="scanCount">The scan count.</param>
    /// <param name="analysisComplete">Whether analysis is complete.</param>
    /// <param name="vistaGenomicsValue">The stored Vista Genomics value.</param>
    /// <param name="longitudeAt1stScan">The longitude of the first scan.</param>
    /// <param name="latitudeAt1stScan">The latitude of the first scan.</param>
    /// <param name="longitudeAt2ndScan">The longitude of the second scan.</param>
    /// <param name="latitudeAt2ndScan">The latitude of the second scan.</param>
    /// <param name="vistaGenomicsMaxValue">The maximum Vista Genomics value.</param>
    /// <param name="vistaGenomicsBaseValue">The base Vista Genomics value.</param>
    /// <param name="vistaGenomicsFirstDiscoveryBonusValue">The first discovery bonus value.</param>
    /// <param name="isFirstDiscovery">Whether this is a first discovery.</param>
    /// <param name="wasLogged">Whether the genus was logged, or <see langword="null"/> if unspecified.</param>
    /// <param name="codexKey">The codex genus identifier.</param>
    /// <param name="speciesKey">The codex species identifier.</param>
    /// <param name="variantKey">The codex variant identifier.</param>
    public Genus(string name, long bodyId, long starSystemId, string species, string variant, long scanCount, long analysisComplete, long vistaGenomicsValue, double? longitudeAt1stScan, double? latitudeAt1stScan, double? longitudeAt2ndScan, double? latitudeAt2ndScan, long vistaGenomicsMaxValue, long vistaGenomicsBaseValue, long vistaGenomicsFirstDiscoveryBonusValue, long isFirstDiscovery, long? wasLogged, string? codexKey, string? speciesKey, string? variantKey)
    {
        Name = name;
        CodexKey = codexKey ?? string.Empty;
        BodyId = Convert.ToInt32(bodyId);
        StarSystemId = starSystemId;
        Species = species;
        SpeciesKey = speciesKey ?? string.Empty;
        Variant = variant;
        VariantKey = variantKey ?? string.Empty;
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
        ClonalColonyRange = BiologyCatalogProvider.GetClonalColonyRangeForGenus(!string.IsNullOrEmpty(CodexKey) ? CodexKey : Name);
    }

    /// <summary>
    /// Resets all analysis and scan data if analysis is not yet complete.
    /// </summary>
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
