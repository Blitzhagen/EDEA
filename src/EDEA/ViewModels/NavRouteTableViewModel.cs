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

public class NavRouteTableViewModel : TabViewModel
{
    public override string TabName => "Route";

    private static readonly ILog log = LogManager.GetLogger(typeof(NavRouteTableViewModel));

    private readonly StarSystemProvider _starSystemProvider;

    private readonly RouteProvider _routeProvider;

    private RouteTableView navRouteTableView = null!;

    private RouteTableView navRouteHudTableView = null!;

    private bool refreshTaskRunning;

    public ICommand CopySystemNameToClipboardCommand { get; }

    public IEnumerable<StarSystemViewModel> Route { get; private set; } = Array.Empty<StarSystemViewModel>();

    public string NoRouteInfo => Resources.NoRouteInfo;

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

    public StarSystemViewModel? CurrentSystem => Route?.FirstOrDefault(starSystem => starSystem.IsCurrentSystemInRoute);

    public bool HasNoCurrentSystem => CurrentSystem == null;

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

    public string RouteIsLoadingInfoText { get; private set; }

    public string CommanderName => _starSystemProvider.CommanderName;

    public List<string> TeammateNames => _starSystemProvider.TeammateNames;

    private ConcurrentDictionary<long, StarSystem> _route => _starSystemProvider.StarSystemsOnRoute;

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
