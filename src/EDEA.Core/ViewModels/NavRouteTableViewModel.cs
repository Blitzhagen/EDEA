using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using log4net;

namespace EDEA.ViewModels;

/// <summary>
/// View model that manages the route tab and its data presentation.
/// </summary>
public class NavRouteTableViewModel : TabViewModel
{
    /// <summary>
    /// Gets the name of the tab.
    /// </summary>
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
    /// Gets the collection of route star system view models.
    /// </summary>
    public IEnumerable<StarSystemViewModel> RouteStarSystems { get; private set; } = Enumerable.Empty<StarSystemViewModel>();

    /// <summary>
    /// Gets the localized info text shown when no route is available.
    /// </summary>
    public string NoRouteInfo => Resources.NoRouteInfo;

    /// <summary>
    /// Gets a value indicating whether there are no star systems on the route.
    /// </summary>
    public bool IsNoRouteAvailable => _starSystemProvider.StarSystemsOnRoute.Count == 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavRouteTableViewModel"/> class.
    /// </summary>
    /// <param name="tabHeader">The header text for the tab.</param>
    /// <param name="tabVisibility">The initial visibility of the tab.</param>
    /// <param name="starSystemProvider">The provider for star system data.</param>
    public NavRouteTableViewModel(string tabHeader, string tabVisibility, StarSystemProvider starSystemProvider)
        : base(tabHeader, tabVisibility)
    {
        _starSystemProvider = starSystemProvider;
        _starSystemProvider.GuiDataUpdated += delegate
        {
            PlatformServices.Dispatcher?.Invoke(delegate
            {
                RefreshRouteDataView();
            });
        };
    }

    /// <summary>
    /// Generates the route star system view models.
    /// </summary>
    public void GenerateRouteViews()
    {
        RouteStarSystems = _starSystemProvider.StarSystemsOnRoute
            .Select((KeyValuePair<long, StarSystem> entry) => new StarSystemViewModel(entry.Value))
            .ToList();
    }

    /// <summary>
    /// Refreshes the route data view on the UI thread.
    /// </summary>
    private async void RefreshRouteDataView()
    {
        try
        {
            await Task.Run(delegate
            {
                GenerateRouteViews();
                OnPropertyChanged("RouteStarSystems");
                OnPropertyChanged("IsNoRouteAvailable");
            });
        }
        catch (Exception exception)
        {
            log.Error("Error in RefreshRouteDataView", exception);
        }
    }
}
