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

public class GenusTableViewModel : TabViewModel
{
    public override string TabName => "Biologicals";

    private static readonly ILog log = LogManager.GetLogger(typeof(GenusTableViewModel));

    private readonly StarSystemProvider _starSystemProvider;

    public IEnumerable<GenusViewModel> Genuses => _starSystemProvider.CurrentPlanet?.Genuses.Select((KeyValuePair<string, Genus> genusEntry) => new GenusViewModel(genusEntry.Value)) ?? Enumerable.Empty<GenusViewModel>();

    public IEnumerable<GenusClassificationViewModel> PredictedSpecies => _starSystemProvider.CurrentPlanet?.PredictedSpecies.Select((GenusClassification species) => new GenusClassificationViewModel(species, _starSystemProvider.CurrentPlanet)).OrderByDescending((GenusClassificationViewModel speciesVm) => speciesVm.VistaGenomicsBaseValueSort) ?? Enumerable.Empty<GenusClassificationViewModel>();

    public string UnknownBiologicalsInfo => string.Format(Resources.GenusTable_UnknownBiologicalSignalsInfo, _starSystemProvider.CurrentPlanet?.BiologicalCount);

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

    public bool AreBiologicalsKnown => !AreBiologicalsUnknown;

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
