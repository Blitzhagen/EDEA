using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using log4net;

namespace EDEA.ViewModels;

/// <summary>
/// View model that manages the bodies tab and its data presentation.
/// </summary>
public class BodyTableViewModel : TabViewModel
{
    /// <summary>
    /// Gets the name of the tab.
    /// </summary>
    /// <value>The tab name displayed in the UI.</value>
    public override string TabName => "Bodies";

    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(BodyTableViewModel));

    /// <summary>
    /// Provides star system and related data.
    /// </summary>
    public StarSystemProvider StarSystemProvider { get; }

    /// <summary>
    /// Gets or sets the command that copies the selected body name to the clipboard.
    /// </summary>
    /// <value>The copy command.</value>
    public ICommand? CopyBodyNameToClipboardCommand { get; set; }

    /// <summary>
    /// Gets the collection of view models representing the current system's bodies.
    /// </summary>
    /// <value>The body view models.</value>
    public IEnumerable<BodyViewModel> Bodies { get; private set; } = Enumerable.Empty<BodyViewModel>();

    /// <summary>
    /// Gets the localized info text shown when no bodies are available.
    /// </summary>
    /// <value>The no bodies info text.</value>
    public string NoBodiesInfo => Resources.NoBodiesInfo;

    /// <summary>
    /// Gets a value indicating whether there are no bodies available in the current system.
    /// </summary>
    /// <value><c>true</c> if no bodies are available; otherwise, <c>false</c>.</value>
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

    /// <summary>
    /// Gets the current commander's name.
    /// </summary>
    /// <value>The commander name.</value>
    public string CommanderName => StarSystemProvider.CommanderName;

    /// <summary>
    /// Gets the list of teammate names.
    /// </summary>
    /// <value>The teammate names.</value>
    public List<string> TeammateNames => StarSystemProvider.TeammateNames;

    /// <summary>
    /// Gets the body view model representing the current planet.
    /// </summary>
    /// <value>The current planet, or <c>null</c> if none is found.</value>
    public BodyViewModel? CurrentPlanet => Bodies?.FirstOrDefault(body => body.IsCurrentPlanetInSystem);

    /// <summary>
    /// Initializes a new instance of the <see cref="BodyTableViewModel"/> class.
    /// </summary>
    /// <param name="tabHeader">The header text for the tab.</param>
    /// <param name="tabVisibility">The initial visibility of the tab.</param>
    /// <param name="starSystemProvider">The provider for star system data.</param>
    public BodyTableViewModel(string tabHeader, string tabVisibility, StarSystemProvider starSystemProvider)
        : base(tabHeader, tabVisibility)
    {
        StarSystemProvider = starSystemProvider;
        StarSystemProvider.GuiDataUpdated += delegate
        {
            PlatformServices.Dispatcher?.Invoke(delegate
            {
                RefreshBodiesDataView();
            });
        };
        Resources.CultureChanged += delegate
        {
            PlatformServices.Dispatcher?.Invoke(RefreshBodiesDataView);
        };
    }

    /// <summary>
    /// Generates the body view models for the current system, applying the configured sorting.
    /// </summary>
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

    /// <summary>
    /// Refreshes the body data view on the UI thread.
    /// </summary>
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
