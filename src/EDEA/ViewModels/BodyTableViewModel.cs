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

public class BodyTableViewModel : TabViewModel
{
    public override string TabName => "Bodies";

    private static readonly ILog log = LogManager.GetLogger(typeof(BodyTableViewModel));

    public StarSystemProvider StarSystemProvider { get; }

    public ICommand CopyBodyNameToClipboardCommand { get; }

    public IEnumerable<BodyViewModel> Bodies { get; private set; } = Enumerable.Empty<BodyViewModel>();

    public string NoBodiesInfo => Resources.NoBodiesInfo;

    public bool AreNoBodiesAvailable
    {
        get
        {
            StarSystem currentSystem = StarSystemProvider.CurrentSystem;
            if (currentSystem == null)
            {
                return false;
            }
            return currentSystem.Bodies.Count() == 0;
        }
    }

    public string CommanderName => StarSystemProvider.CommanderName;

    public List<string> TeammateNames => StarSystemProvider.TeammateNames;

    public BodyViewModel? CurrentPlanet => Bodies?.FirstOrDefault(body => body.IsCurrentPlanetInSystem);

    public BodyTableViewModel(string tabHeader, string tabVisibility, StarSystemProvider starSystemProvider)
        : base(tabHeader, tabVisibility)
    {
        StarSystemProvider = starSystemProvider;
        CopyBodyNameToClipboardCommand = new CopyToClipboardCommand();
        StarSystemProvider.GuiDataUpdated += delegate
        {
            Application.Current?.Dispatcher.Invoke(delegate
            {
                RefreshBodiesDataView();
            });
        };
    }

    public void GenerateBodyViews()
    {
        try
        {
            string propertyName = Preferences.Application.BodyTableViewSortMemberPath;
            PropertyInfo sortProperty = typeof(BodyViewModel).GetProperty(propertyName!)!;
            if (Preferences.Application.BodyTableViewSortDirection == ListSortDirection.Descending)
            {
                Bodies = StarSystemProvider.CurrentSystem?.Bodies?.Where((KeyValuePair<int, Body> bodyEntry) => bodyEntry.Value.IsPlanetOrStar).Select((KeyValuePair<int, Body> bodyEntry) => new BodyViewModel(bodyEntry.Value)).OrderByDescending(orderByFunc)
                    .ToList() ?? Enumerable.Empty<BodyViewModel>();
            }
            else
            {
                Bodies = StarSystemProvider.CurrentSystem?.Bodies?.Where((KeyValuePair<int, Body> bodyEntry) => bodyEntry.Value.IsPlanetOrStar).Select((KeyValuePair<int, Body> bodyEntry) => new BodyViewModel(bodyEntry.Value)).OrderBy(orderByFunc)
                    .ToList() ?? Enumerable.Empty<BodyViewModel>();
            }
            dynamic orderByFunc(BodyViewModel bodyViewModel)
            {
                return sortProperty.GetValue(bodyViewModel)!;
            }
        }
        catch (Exception exception)
        {
            log.Error("Could not apply custom sorting to body table view, using distance instead", exception);
            Bodies = StarSystemProvider.CurrentSystem?.Bodies?.Where((KeyValuePair<int, Body> bodyEntry) => bodyEntry.Value.IsPlanetOrStar).Select((KeyValuePair<int, Body> bodyEntry) => new BodyViewModel(bodyEntry.Value)).OrderBy((BodyViewModel bodyViewModel) => bodyViewModel.DistanceSort)
                .ToList() ?? Enumerable.Empty<BodyViewModel>();
        }
    }

    private async void RefreshBodiesDataView()
    {
        try
        {

            log.Debug("EDEA4711: Refresh of BodyTableView");
            await Task.Run(delegate
            {
                GenerateBodyViews();
                OnPropertyChanged("Bodies");
                OnPropertyChanged("AreNoBodiesAvailable");
                OnPropertyChanged("CurrentPlanet");
                OnPropertyChanged("CommanderName");
                OnPropertyChanged("TeammateNames");
            });

        }
        catch (Exception exception)
        {
            log.Error("Error in RefreshBodiesDataView", exception);
        }
    }
}
