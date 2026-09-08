using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using log4net;

namespace EDEA.ViewModels;

/// <summary>
/// View model that manages the navigation route tab and its data presentation.
/// </summary>
public class NavRouteTableViewModel : TabViewModel
{
    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(NavRouteTableViewModel));

    /// <summary>
    /// The provider for star system data.
    /// </summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>
    /// The provider for route data.
    /// </summary>
    private readonly RouteProvider _routeProvider;

    /// <summary>
    /// A value indicating whether a refresh task is currently running.
    /// </summary>
    private bool _refreshTaskRunning;

    /// <summary>
    /// The current loading info text for the route.
    /// </summary>
    private string _routeIsLoadingInfoText = string.Empty;

    /// <summary>
    /// The collection of star systems in the route.
    /// </summary>
    public IEnumerable<StarSystemViewModel> Route { get; private set; } = Array.Empty<StarSystemViewModel>();

    /// <summary>
    /// Gets the localized info text shown when no route is available.
    /// </summary>
    public string NoRouteInfo => Resources.NoRouteInfo;

    /// <summary>
    /// Gets a value indicating whether the no route info banner should be shown.
    /// </summary>
    public bool ShowNoRouteInfo
    {
        get
        {
            if (RouteIsLoading || _refreshTaskRunning)
            {
                return false;
            }

            return !Route.Any();
        }
    }

    /// <summary>
    /// Gets the current star system in the route.
    /// </summary>
    public StarSystemViewModel? CurrentSystem => Route.FirstOrDefault(starSystem => starSystem.IsCurrentSystemInRoute);

    /// <summary>
    /// Gets a value indicating whether no current system is present in the route.
    /// </summary>
    public bool HasNoCurrentSystem => CurrentSystem == null;

    /// <summary>
    /// Gets a value indicating whether the route is currently loading.
    /// </summary>
    public bool RouteIsLoading => _starSystemProvider.RouteIsLoading || _refreshTaskRunning;

    /// <summary>
    /// Gets the loading info text for the route.
    /// </summary>
    public string RouteIsLoadingInfoText
    {
        get => _routeIsLoadingInfoText;
        private set
        {
            _routeIsLoadingInfoText = value;
            OnPropertyChanged("RouteIsLoadingInfoText");
        }
    }

    /// <summary>
    /// Gets the current commander name.
    /// </summary>
    public string CommanderName => _starSystemProvider.CommanderName;

    /// <summary>
    /// Gets the list of teammate names.
    /// </summary>
    public List<string> TeammateNames => _starSystemProvider.TeammateNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavRouteTableViewModel"/> class.
    /// </summary>
    /// <param name="tabHeader">The header text for the tab.</param>
    /// <param name="tabVisibility">The initial visibility of the tab.</param>
    /// <param name="starSystemProvider">The provider for star system data.</param>
    /// <param name="routeProvider">The provider for route data.</param>
    public NavRouteTableViewModel(string tabHeader, string tabVisibility, StarSystemProvider starSystemProvider, RouteProvider routeProvider)
        : base(tabHeader, tabVisibility)
    {
        _starSystemProvider = starSystemProvider;
        _routeProvider = routeProvider;

        _starSystemProvider.GuiDataUpdated += delegate
        {
            PlatformServices.Dispatcher?.Invoke(delegate
            {
                RefreshRouteDataView();
            });
        };

        _starSystemProvider.CurrentSystemInRouteChanged += delegate
        {
            PlatformServices.Dispatcher?.Invoke(delegate
            {
                RefreshRouteDataView();
                OnPropertyChanged("CurrentSystem");
                OnPropertyChanged("HasNoCurrentSystem");
            });
        };

        _starSystemProvider.RouteLoadingStatusChanged += delegate (object? sender, string text)
        {
            PlatformServices.Dispatcher?.Invoke(delegate
            {
                if (!string.IsNullOrEmpty(text))
                {
                    RouteIsLoadingInfoText = text;
                }
                else
                {
                    RouteIsLoadingInfoText = Resources.RouteIsLoading_PleaseWait;
                }

                OnPropertyChanged("RouteIsLoading");
                OnPropertyChanged("ShowNoRouteInfo");
            });
        };

        Resources.CultureChanged += delegate
        {
            PlatformServices.Dispatcher?.Invoke(RefreshRouteDataView);
        };
    }

    /// <summary>
    /// Refreshes the route data view on the UI thread.
    /// </summary>
    public async void RefreshRouteDataView()
    {
        try
        {
            _refreshTaskRunning = true;
            OnPropertyChanged("RouteIsLoading");
            OnPropertyChanged("ShowNoRouteInfo");

            await Task.Run(delegate
            {
                Route = _starSystemProvider.StarSystemsOnRoute
                    .Select((KeyValuePair<long, StarSystem> entry) => new StarSystemViewModel(entry.Value))
                    .OrderBy(item => item.JumpDistance)
                    .ToList();

                TabHeaderKey = _routeProvider.IsCustomRoute ? "TabHeader_PlotterRoute" : "TabHeader_Route";

                OnPropertyChanged("Route");
                OnPropertyChanged("RouteIsLoading");
                OnPropertyChanged("ShowNoRouteInfo");
                OnPropertyChanged("CurrentSystem");
                OnPropertyChanged("HasNoCurrentSystem");
                OnPropertyChanged("CommanderName");
                OnPropertyChanged("TeammateNames");
                OnPropertyChanged("TabHeader");
            });
        }
        catch (Exception exception)
        {
            log.Error("Error in RefreshRouteDataView", exception);
        }
        finally
        {
            _refreshTaskRunning = false;
            OnPropertyChanged("RouteIsLoading");
            OnPropertyChanged("ShowNoRouteInfo");
        }
    }
}
