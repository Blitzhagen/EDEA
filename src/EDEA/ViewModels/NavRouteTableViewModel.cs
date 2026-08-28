using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EDEA.Commands;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using EDEA.Views;
using log4net;

namespace EDEA.ViewModels;

/// <summary>
/// View model that manages the navigation route tab and its data presentation.
/// </summary>
public class NavRouteTableViewModel : TabViewModel
{
    /// <summary>
    /// Gets the name of the tab.
    /// </summary>
    /// <value>The tab name displayed in the UI.</value>
    public override string TabName => "Route";

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
    /// The main route table view.
    /// </summary>
    private RouteTableView navRouteTableView = null!;

    /// <summary>
    /// The HUD route table view.
    /// </summary>
    private RouteTableView navRouteHudTableView = null!;

    /// <summary>
    /// A value indicating whether a refresh task is currently running.
    /// </summary>
    private bool refreshTaskRunning;

    /// <summary>
    /// Gets the command that copies the selected system name to the clipboard.
    /// </summary>
    /// <value>The copy command.</value>
    public ICommand CopySystemNameToClipboardCommand { get; }

    /// <summary>
    /// Gets the collection of star systems in the route.
    /// </summary>
    /// <value>The route star system view models.</value>
    public IEnumerable<StarSystemViewModel> Route { get; private set; } = Array.Empty<StarSystemViewModel>();

    /// <summary>
    /// Gets the localized info text shown when no route is available.
    /// </summary>
    /// <value>The no route info text.</value>
    public string NoRouteInfo => Resources.NoRouteInfo;

    /// <summary>
    /// Gets a value indicating whether the no route info should be shown.
    /// </summary>
    /// <value><c>true</c> if the no route info is visible; otherwise, <c>false</c>.</value>
    public bool ShowNoRouteInfo
    {
        get
        {
            IEnumerable<StarSystemViewModel> route = Route;
            if (route != null && route.Count() < 1 && !RouteIsLoading)
            {
                return !refreshTaskRunning;
            }
            return false;
        }
    }

    /// <summary>
    /// Gets the current star system in the route.
    /// </summary>
    /// <value>The current system, or <c>null</c> if none is found.</value>
    public StarSystemViewModel? CurrentSystem => Route?.FirstOrDefault(starSystem => starSystem.IsCurrentSystemInRoute);

    /// <summary>
    /// Gets a value indicating whether no current system is present in the route.
    /// </summary>
    /// <value><c>true</c> if no current system is present; otherwise, <c>false</c>.</value>
    public bool HasNoCurrentSystem => CurrentSystem == null;

    /// <summary>
    /// Gets a value indicating whether the route is currently loading.
    /// </summary>
    /// <value><c>true</c> if the route is loading; otherwise, <c>false</c>.</value>
    public bool RouteIsLoading
    {
        get
        {
            if (!_starSystemProvider.RouteIsLoading)
            {
                return refreshTaskRunning;
            }
            return true;
        }
    }

    /// <summary>
    /// Gets the loading info text for the route.
    /// </summary>
    /// <value>The loading info text.</value>
    public string RouteIsLoadingInfoText { get; private set; }

    /// <summary>
    /// Gets the current commander name.
    /// </summary>
    /// <value>The commander name.</value>
    public string CommanderName => _starSystemProvider.CommanderName;

    /// <summary>
    /// Gets the list of teammate names.
    /// </summary>
    /// <value>The teammate names.</value>
    public List<string> TeammateNames => _starSystemProvider.TeammateNames;

    /// <summary>
    /// Gets the dictionary of star systems on the route.
    /// </summary>
    /// <value>The route systems.</value>
    private ConcurrentDictionary<long, StarSystem> _route => _starSystemProvider.StarSystemsOnRoute;

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
        CopySystemNameToClipboardCommand = new CopyToClipboardCommand();
        RouteIsLoadingInfoText = string.Empty;
        _starSystemProvider.GuiDataUpdated += delegate
        {
            Application.Current?.Dispatcher.Invoke(delegate
            {
                Task _ = refreshView();
            });
        };
        _starSystemProvider.CurrentSystemInRouteChanged += delegate
        {
            Application.Current?.Dispatcher.Invoke((Func<Task>)async delegate
            {
                if (CurrentSystem == null)
                {
                    await refreshView();
                }
                if (navRouteTableView != null)
                {
                    scrollToCurrentSystem(navRouteTableView);
                }
                if (navRouteHudTableView != null)
                {
                    scrollToCurrentSystem(navRouteHudTableView);
                }
            });
        };
        _starSystemProvider.RouteLoadingStatusChanged += delegate (object? sender, string text)
        {
            Application.Current?.Dispatcher.Invoke(delegate
            {
                if (!string.IsNullOrEmpty(text))
                {
                    RouteIsLoadingInfoText = text;
                }
                else
                {
                    RouteIsLoadingInfoText = Resources.RouteIsLoading_PleaseWait;
                }
                OnPropertyChanged("RouteIsLoadingInfoText");
                OnPropertyChanged("RouteIsLoading");
                OnPropertyChanged("ShowNoRouteInfo");
            });
        };
    }

    /// <summary>
    /// Registers a view for scrolling to the current system.
    /// </summary>
    /// <param name="view">The view to register.</param>
    public void registerView(UserControl view)
    {
        if (view.GetType() == typeof(RouteTableView))
        {
            navRouteTableView = (RouteTableView)view;
            scrollToCurrentSystem(navRouteTableView);
        }
        if (view.GetType() == typeof(RouteTableView))
        {
            navRouteHudTableView = (RouteTableView)view;
            scrollToCurrentSystem(navRouteHudTableView);
        }
    }

    /// <summary>
    /// Unregisters a view from scrolling.
    /// </summary>
    /// <param name="view">The view to unregister.</param>
    public void unregisterView(UserControl view)
    {
        if (view.GetType() == typeof(RouteTableView))
        {
            navRouteTableView = null!;
        }
        if (view.GetType() == typeof(RouteTableView))
        {
            navRouteHudTableView = null!;
        }
    }

    /// <summary>
    /// Scrolls the specified view to the current system.
    /// </summary>
    /// <param name="view">The view to scroll.</param>
    private void scrollToCurrentSystem(dynamic view)
    {
        try
        {
            if (view == null)
            {
                return;
            }
            bool isInvalid = CurrentSystem == null
                || view.RouteDataGrid == null
                || view.RouteDataGrid.ItemsSource == null
                || view.RouteDataGrid.Items == null;

            if (isInvalid || view.RouteDataGrid.Items.Count < 1)
            {
                return;
            }

            int index = view.RouteDataGrid.Items.IndexOf(CurrentSystem);
            if (index <= -1)
            {
                return;
            }

            view.RouteDataGrid.ScrollIntoView(view.RouteDataGrid.Items[view.RouteDataGrid.Items.Count - 1]);
            view.RouteDataGrid.UpdateLayout();
            int previousIndex = (index > 0) ? (index - 1) : 0;
            view.RouteDataGrid.ScrollIntoView(view.RouteDataGrid.Items[previousIndex]);
            view.RouteDataGrid.UpdateLayout();
        }
        catch (Exception exception)
        {
            log.Error("Error on scrolling to current system", exception);
        }
    }

    /// <summary>
    /// Refreshes the route data view on the UI thread.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task refreshView()
    {
        log.Debug("EDEA4711: Refresh of NavRouteTable");
        refreshTaskRunning = true;
        OnPropertyChanged("refreshTaskRunning");
        OnPropertyChanged("RouteIsLoading");
        OnPropertyChanged("ShowNoRouteInfo");
        await Task.Run(delegate
        {
            ImmutableDictionary<long, StarSystem> source = _route.ToImmutableDictionary();
            Route = (from item in source
                     select new StarSystemViewModel(item.Value) into item
                     orderby item.JumpDistance
                     select item).ToList();
            base.TabHeader = _routeProvider.IsCustomRoute ? Resources.TabHeader_PlotterRoute : Resources.TabHeader_Route;
            OnPropertyChanged("Route");
            OnPropertyChanged("RouteIsLoading");
            OnPropertyChanged("ShowNoRouteInfo");
            OnPropertyChanged("CurrentSystem");
            OnPropertyChanged("HasNoCurrentSystem");
            OnPropertyChanged("CommanderName");
            OnPropertyChanged("TeammateNames");
            OnPropertyChanged("TabHeader");
        });
        refreshTaskRunning = false;
        OnPropertyChanged("refreshTaskRunning");
        OnPropertyChanged("RouteIsLoading");
        OnPropertyChanged("ShowNoRouteInfo");
    }
}
