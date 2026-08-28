using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EDEA.Commands;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using log4net;

namespace EDEA.ViewModels;

public class SurroundingsTableViewModel : TabViewModel
{
    public override string TabName => "Surroundings";

    private static readonly ILog log = LogManager.GetLogger(typeof(SurroundingsTableViewModel));

    private StarSystemProvider _starSystemProvider;

    public ICommand CopySystemNameToClipboardCommand { get; }

    public IEnumerable<StarSystemViewModel> SurroundingStarSystems { get; private set; } = Enumerable.Empty<StarSystemViewModel>();

    public string CommanderName => _starSystemProvider.CommanderName;

    public List<string> TeammateNames => _starSystemProvider.TeammateNames;

    public string SurroundingsLoadingInfo { get; private set; }

    public bool ShowSurroundingsLoadingInfo => _starSystemProvider.SurroundingsAreLoading;

    public string NoSurroundingsInfo => Resources.NoSurroundingsInfo;

    public bool ShowNoSurroundingsInfo { get; private set; }

    public SurroundingsTableViewModel(string tabHeader, string tabVisibility, StarSystemProvider starSystemProvider)
        : base(tabHeader, tabVisibility)
    {
        _starSystemProvider = starSystemProvider;
        CopySystemNameToClipboardCommand = new CopyToClipboardCommand();
        SurroundingsLoadingInfo = string.Empty;
        ShowNoSurroundingsInfo = false;
        _starSystemProvider.GuiDataUpdated += delegate
        {
            Application.Current?.Dispatcher.Invoke(delegate
            {
                refreshView();
            });
        };
        _starSystemProvider.SurroundingsLoadingStatusChanged += _starSystemProvider_SurroundingsLoadingStatusChanged;
    }

    private void _starSystemProvider_SurroundingsLoadingStatusChanged(object? sender, string text)
    {
        SurroundingsLoadingInfo = text;
        ShowNoSurroundingsInfo = !ShowSurroundingsLoadingInfo && _starSystemProvider.SurroundingStarSystems.Count() < 1;
        OnPropertyChanged("ShowNoSurroundingsInfo");
        OnPropertyChanged("SurroundingsLoadingInfo");
        OnPropertyChanged("ShowSurroundingsLoadingInfo");
    }

    public void GenerateSurroundingsViews()
    {
        try
        {
            string propertyName = Preferences.Application.SurroundingsTableViewSortMemberPath;
            PropertyInfo sortProperty = typeof(StarSystemViewModel).GetProperty(propertyName!)!;
            if (Preferences.Application.SurroundingsTableViewSortDirection == ListSortDirection.Descending)
            {
                SurroundingStarSystems = _starSystemProvider.SurroundingStarSystems.Select((KeyValuePair<long, StarSystem> entry) => new StarSystemViewModel(entry.Value)).OrderByDescending(orderByFunc);
            }
            else
            {
                SurroundingStarSystems = _starSystemProvider.SurroundingStarSystems.Select((KeyValuePair<long, StarSystem> entry) => new StarSystemViewModel(entry.Value)).OrderBy(orderByFunc);
            }
            dynamic orderByFunc(StarSystemViewModel viewModel)
            {
                return sortProperty.GetValue(viewModel)!;
            }
        }
        catch (Exception exception)
        {
            log.Error("Could not apply custom sorting to surroundings table view, using jump distance (Ly) instead", exception);
            SurroundingStarSystems = from x in _starSystemProvider.SurroundingStarSystems
                                     select new StarSystemViewModel(x.Value) into x
                                     orderby x.JumpDistanceLySort
                                     select x;
        }
    }

    private async void refreshView()
    {
        try
        {

            log.Debug("EDEA4711: Refresh of SurroundingsTable");
            await Task.Run(delegate
            {
                GenerateSurroundingsViews();
                OnPropertyChanged("SurroundingStarSystems");
                OnPropertyChanged("ShowNoSurroundingsInfo");
                OnPropertyChanged("CommanderName");
                OnPropertyChanged("TeammateNames");
            });

        }
        catch (Exception exception)
        {
            log.Error("Error in refreshView", exception);
        }
    }
}
