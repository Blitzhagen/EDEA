using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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
    /// A version counter used to discard stale route refresh results.
    /// </summary>
    private long _routeRefreshVersion;

    /// <summary>
    /// Snapshot of the previously displayed route row values.
    /// This lets SyncRouteCollection detect property changes on reused StarSystem objects.
    /// </summary>
    private readonly record struct RouteRowState(
        int JumpDistance,
        string Name,
        string JumpDistanceLy,
        string ExplorationStatus,
        string StarClass,
        string EdsmName,
        string OverallProgress,
        string BodiesCartographicMaxValue,
        bool HasMatchingPlanetClassifications,
        bool HasValuableBodies,
        bool IsCurrentSystemInRoute,
        bool IsJumpDestinationSystemInRoute,
        bool IsPastSystemInRoute);

    /// <summary>
    /// The current loading info text for the route.
    /// </summary>
    private string _routeIsLoadingInfoText = string.Empty;

    /// <summary>
    /// The collection of star systems in the route.
    /// </summary>
    public ObservableCollection<StarSystemViewModel> Route { get; } = new ObservableCollection<StarSystemViewModel>();

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
        long version = Interlocked.Increment(ref _routeRefreshVersion);
        try
        {
            _refreshTaskRunning = true;
            OnPropertyChanged("RouteIsLoading");
            OnPropertyChanged("ShowNoRouteInfo");

            // Capture the currently displayed values before recomputing flags.
            // This lets us detect stale rows even when the same StarSystem object
            // is reused and its properties change in place.
            var previousState = Route.Select(vm => new RouteRowState(
                vm.JumpDistance,
                vm.Name,
                vm.JumpDistanceLy,
                vm.ExplorationStatus,
                vm.StarClass,
                vm.EdsmName,
                vm.OverallProgress,
                vm.BodiesCartographicMaxValue,
                vm.HasMatchingPlanetClassifications,
                vm.HasValuableBodies,
                vm.IsCurrentSystemInRoute,
                vm.IsJumpDestinationSystemInRoute,
                vm.IsPastSystemInRoute)).ToList();

            // Recompute route flags from authoritative state before building the view.
            // This acts as a safety net for events that may not have updated them.
            _starSystemProvider.UpdateRouteSystemStateFlags();

            var route = await Task.Run(() => _starSystemProvider.StarSystemsOnRoute
                .Select((KeyValuePair<long, StarSystem> entry) => new StarSystemViewModel(entry.Value))
                .OrderBy(item => item.JumpDistance)
                .ToList());

            // A newer refresh was requested while we were building; discard this result.
            if (version != _routeRefreshVersion)
            {
                log.Debug($"RefreshRouteDataView discarding stale result (version {version}, current {_routeRefreshVersion})");
                return;
            }

            var currentNames = route.Where(r => r.IsCurrentSystemInRoute).Select(r => $"{r.Name}({r.JumpDistance})").ToList();
            var jumpNames = route.Where(r => r.IsJumpDestinationSystemInRoute).Select(r => $"{r.Name}({r.JumpDistance})").ToList();
            log.Info($"RefreshRouteDataView applying version {version}: rows={route.Count}, current=[{string.Join(", ", currentNames)}], jump=[{string.Join(", ", jumpNames)}]");

            string tabHeaderKey = _routeProvider.IsCustomRoute ? "TabHeader_PlotterRoute" : "TabHeader_Route";

            SyncRouteCollection(route, previousState);
            TabHeaderKey = tabHeaderKey;

            OnPropertyChanged("RouteIsLoading");
            OnPropertyChanged("ShowNoRouteInfo");
            OnPropertyChanged("CurrentSystem");
            OnPropertyChanged("HasNoCurrentSystem");
            OnPropertyChanged("CommanderName");
            OnPropertyChanged("TeammateNames");
            OnPropertyChanged("TabHeader");
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

    /// <summary>
    /// Updates the existing route collection in place to avoid resetting the
    /// DataGrid scroll position on every refresh.
    /// </summary>
    /// <param name="newRoute">The newly built route list.</param>
    /// <param name="previousState">The values displayed before this refresh started.</param>
    private void SyncRouteCollection(List<StarSystemViewModel> newRoute, List<RouteRowState> previousState)
    {
        bool sameIdentity = Route.Count == newRoute.Count;
        if (sameIdentity)
        {
            for (int i = 0; i < newRoute.Count; i++)
            {
                if (Route[i].JumpDistance != newRoute[i].JumpDistance ||
                    Route[i].Name != newRoute[i].Name)
                {
                    sameIdentity = false;
                    break;
                }
            }
        }

        if (sameIdentity)
        {
            // Only replace rows whose displayed data has changed.
            for (int i = 0; i < newRoute.Count; i++)
            {
                StarSystemViewModel newRow = newRoute[i];
                RouteRowState? previous = i < previousState.Count ? previousState[i] : null;

                if (previous?.JumpDistanceLy != newRow.JumpDistanceLy ||
                    previous?.ExplorationStatus != newRow.ExplorationStatus ||
                    previous?.StarClass != newRow.StarClass ||
                    previous?.EdsmName != newRow.EdsmName ||
                    previous?.OverallProgress != newRow.OverallProgress ||
                    previous?.BodiesCartographicMaxValue != newRow.BodiesCartographicMaxValue ||
                    previous?.HasMatchingPlanetClassifications != newRow.HasMatchingPlanetClassifications ||
                    previous?.HasValuableBodies != newRow.HasValuableBodies ||
                    previous?.IsCurrentSystemInRoute != newRow.IsCurrentSystemInRoute ||
                    previous?.IsJumpDestinationSystemInRoute != newRow.IsJumpDestinationSystemInRoute ||
                    previous?.IsPastSystemInRoute != newRow.IsPastSystemInRoute)
                {
                    Route[i] = newRow;
                }
            }
        }
        else
        {
            Route.Clear();
            foreach (var item in newRoute)
            {
                Route.Add(item);
            }
        }
    }
}
