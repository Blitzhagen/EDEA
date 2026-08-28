using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using log4net;

namespace EDEA.ViewModels;

/// <summary>
/// View model that manages the biologicals tab and its data presentation.
/// </summary>
public class GenusTableViewModel : TabViewModel
{
    /// <summary>
    /// Gets the name of the tab.
    /// </summary>
    /// <value>The tab name displayed in the UI.</value>
    public override string TabName => "Biologicals";

    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(GenusTableViewModel));

    /// <summary>
    /// The provider for star system and current planet data.
    /// </summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>
    /// Gets the genuses present on the current planet.
    /// </summary>
    /// <value>The genus view models.</value>
    public IEnumerable<GenusViewModel> Genuses => _starSystemProvider.CurrentPlanet?.Genuses.Select((KeyValuePair<string, Genus> genusEntry) => new GenusViewModel(genusEntry.Value)) ?? Enumerable.Empty<GenusViewModel>();

    /// <summary>
    /// Gets the predicted species for the current planet.
    /// </summary>
    /// <value>The predicted species view models.</value>
    public IEnumerable<GenusClassificationViewModel> PredictedSpecies => _starSystemProvider.CurrentPlanet?.PredictedSpecies.Select((GenusClassification species) => new GenusClassificationViewModel(species, _starSystemProvider.CurrentPlanet)).OrderByDescending((GenusClassificationViewModel speciesVm) => speciesVm.VistaGenomicsBaseValueSort) ?? Enumerable.Empty<GenusClassificationViewModel>();

    /// <summary>
    /// Gets the info text for unknown biological signals.
    /// </summary>
    /// <value>The unknown biologicals info text.</value>
    public string UnknownBiologicalsInfo => string.Format(Resources.GenusTable_UnknownBiologicalSignalsInfo, _starSystemProvider.CurrentPlanet?.BiologicalCount);

    /// <summary>
    /// Gets a value indicating whether biological signals on the current planet are unknown.
    /// </summary>
    /// <value><c>true</c> if biologicals are unknown; otherwise, <c>false</c>.</value>
    public bool AreBiologicalsUnknown
    {
        get
        {
            Planet currentPlanet = _starSystemProvider.CurrentPlanet;
            return currentPlanet != null
                && currentPlanet.Genuses.Count == 0
                && currentPlanet.BiologicalCount > 0;
        }
    }

    /// <summary>
    /// Gets a value indicating whether biological signals on the current planet are known.
    /// </summary>
    /// <value><c>true</c> if biologicals are known; otherwise, <c>false</c>.</value>
    public bool AreBiologicalsKnown => !AreBiologicalsUnknown;

    /// <summary>
    /// Gets a value indicating whether predicted species are available.
    /// </summary>
    /// <value><c>true</c> if predicted species are available; otherwise, <c>false</c>.</value>
    public bool PredictedSpeciesAvailable
    {
        get
        {
            if (PredictedSpecies != null)
            {
                return PredictedSpecies.Any();
            }
            return false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether any genus is currently in analysis.
    /// </summary>
    /// <value><c>true</c> if a genus is in analysis; otherwise, <c>false</c>.</value>
    public bool GeneraInAnalysisAvailabe
    {
        get
        {
            if (_starSystemProvider.CurrentPlanet != null)
            {
                return _starSystemProvider.CurrentPlanet.Genuses.Any(g => g.Value.IsInAnalysis);
            }
            return false;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenusTableViewModel"/> class.
    /// </summary>
    /// <param name="tabHeader">The header text for the tab.</param>
    /// <param name="tabVisibility">The initial visibility of the tab.</param>
    /// <param name="starSystemProvider">The provider for star system data.</param>
    public GenusTableViewModel(string tabHeader, string tabVisibility, StarSystemProvider starSystemProvider)
        : base(tabHeader, tabVisibility)
    {
        _starSystemProvider = starSystemProvider;
        _starSystemProvider.GuiDataUpdated += delegate
        {
            Application.Current?.Dispatcher.Invoke(delegate
            {
                refreshView();
            });
        };
        _starSystemProvider.GuiLocationDataUpdated += delegate
        {
            Application.Current?.Dispatcher.Invoke(delegate
            {
                updateCurrentLocation();
            });
        };
    }

    /// <summary>
    /// Refreshes the view based on the current planet and activity.
    /// </summary>
    private void refreshView()
    {
        log.Debug("EDEA4711: Refresh of GenusTableView");
        bool shouldShow = false;
        if (_starSystemProvider.CurrentActivity == Activity.ExplorePlanet)
        {
            Planet currentPlanet = _starSystemProvider.CurrentPlanet;
            shouldShow = currentPlanet != null
                && (currentPlanet.Genuses.Count > 0 || currentPlanet.BiologicalCount > 0);
        }

        base.TabVisibility = shouldShow ? "Visible" : "Collapsed";
        if (shouldShow)
        {
            OnPropertyChanged("Genuses");
            OnPropertyChanged("PredictedSpecies");
            OnPropertyChanged("UnknownBiologicalsInfo");
            OnPropertyChanged("AreBiologicalsUnknown");
            OnPropertyChanged("AreBiologicalsKnown");
            OnPropertyChanged("PredictedSpeciesAvailable");
            OnPropertyChanged("GeneraInAnalysisAvailabe");
            updateCurrentLocation();
        }
        OnPropertyChanged("TabVisibility");
    }

    /// <summary>
    /// Updates the current location for genuses in analysis.
    /// </summary>
    private void updateCurrentLocation()
    {
        if (base.TabVisibility != "Visible" || _starSystemProvider.LocationOnCurrentPlanet == null || _starSystemProvider.CurrentPlanet == null)
        {
            return;
        }
        IEnumerable<Genus> enumerable = from item in _starSystemProvider.CurrentPlanet.Genuses
                                        where item.Value.IsInAnalysis
                                        select item.Value;
        if (!enumerable.Any())
        {
            return;
        }
        foreach (Genus genus in enumerable)
        {
            bool? isOutOfClonalColonyRange = genus.IsOutOfClonalColonyRange;
            if (genus.LongitudeAt1stScan.HasValue && genus.LatitudeAt1stScan.HasValue)
            {
                genus.CurrentDistanceToLocationAt1stScan = calculateDistance(genus.LongitudeAt1stScan.Value, genus.LatitudeAt1stScan.Value, _starSystemProvider.LocationOnCurrentPlanet.Longitude, _starSystemProvider.LocationOnCurrentPlanet.Latitude, _starSystemProvider.LocationOnCurrentPlanet.PlanetRadius);
            }
            if (genus.LongitudeAt2ndScan.HasValue && genus.LatitudeAt2ndScan.HasValue)
            {
                genus.CurrentDistanceToLocationAt2ndScan = calculateDistance(genus.LongitudeAt2ndScan.Value, genus.LatitudeAt2ndScan.Value, _starSystemProvider.LocationOnCurrentPlanet.Longitude, _starSystemProvider.LocationOnCurrentPlanet.Latitude, _starSystemProvider.LocationOnCurrentPlanet.PlanetRadius);
            }
            if (isOutOfClonalColonyRange.HasValue && genus.IsOutOfClonalColonyRange.HasValue && isOutOfClonalColonyRange != genus.IsOutOfClonalColonyRange)
            {
                SpeechOutputSpecies speechOutputSpecies = new SpeechOutputSpecies(genus);
                if (genus.IsOutOfClonalColonyRange.Value)
                {
                    SpeechProvider.SpeakLeaveClonalColonyRange(speechOutputSpecies);
                }
                else if (genus.IsInAnalysis && ((!genus.CurrentDistanceToLocationAt2ndScanAvailable && genus.CurrentDistanceToLocationAt1stScan > 30) || (genus.CurrentDistanceToLocationAt2ndScanAvailable && genus.CurrentDistanceToLocationAt2ndScan > 30)))
                {
                    SpeechProvider.SpeakEnterClonalColonyRange(speechOutputSpecies);
                }
            }
        }
        OnPropertyChanged("Genuses");
    }

    /// <summary>
    /// Calculates the great-circle distance between two coordinates on a sphere.
    /// </summary>
    /// <param name="lon1">The longitude of the first coordinate.</param>
    /// <param name="lat1">The latitude of the first coordinate.</param>
    /// <param name="lon2">The longitude of the second coordinate.</param>
    /// <param name="lat2">The latitude of the second coordinate.</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <returns>The distance in meters.</returns>
    private int calculateDistance(double lon1, double lat1, double lon2, double lat2, double radius)
    {
        lon1 = Math.PI * lon1 / 180.0;
        lat1 = Math.PI * lat1 / 180.0;
        lon2 = Math.PI * lon2 / 180.0;
        lat2 = Math.PI * lat2 / 180.0;
        double latSin = Math.Sin((lat2 - lat1) / 2.0);
        double lonSin = Math.Sin((lon2 - lon1) / 2.0);
        double d = latSin * latSin + Math.Cos(lat1) * Math.Cos(lat2) * lonSin * lonSin;
        return (int)Math.Round(2.0 * radius * Math.Asin(Math.Sqrt(d)));
    }
}
