using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using log4net;

namespace EDEA.Avalonia.ViewModels;

/// <summary>
/// View model for the neutron route plotter window.
/// </summary>
public partial class RoutePlotterViewModel : ObservableObject
{
    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(RoutePlotterViewModel));

    /// <summary>
    /// The delay in milliseconds between Spansh result polling requests.
    /// </summary>
    private const int RequestDelayMilliseconds = 2000;

    /// <summary>
    /// The timeout in milliseconds for system search requests.
    /// </summary>
    private const int SearchTimeoutMilliseconds = 8000;

    /// <summary>
    /// The main view model.
    /// </summary>
    private readonly MainViewModel _mainViewModel;

    /// <summary>
    /// The provider for star system data.
    /// </summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>
    /// The provider for route data.
    /// </summary>
    private readonly RouteProvider _routeProvider;

    /// <summary>
    /// The provider for web API data.
    /// </summary>
    private readonly WebApiProvider _webApiProvider;

    /// <summary>
    /// The selected source star system.
    /// </summary>
    [ObservableProperty]
    private StarSystem? _selectedSourceSystem;

    /// <summary>
    /// The selected target star system.
    /// </summary>
    [ObservableProperty]
    private StarSystem? _selectedTargetSystem;

    /// <summary>
    /// Whether a route calculation is currently running.
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Whether the route calculation error message is shown.
    /// </summary>
    [ObservableProperty]
    private bool _hasError;

    /// <summary>
    /// The current loading progress of the route calculation.
    /// </summary>
    [ObservableProperty]
    private double _loadingProgress;

    /// <summary>
    /// Raised when the window should be closed (route imported or cancelled).
    /// </summary>
    public event EventHandler? CloseRequested;

    /// <summary>
    /// The collection used to receive the current system search results.
    /// </summary>
    private readonly ObservableCollection<StarSystem> _currentSystemSearchResults = new();

    /// <summary>
    /// Gets the maximum value of the loading progress bar.
    /// </summary>
    public double LoadingMaximum => 60.0;

    /// <summary>
    /// Gets the Spansh plotter settings.
    /// </summary>
    public SpanshSettings Spansh => Preferences.Spansh;

    /// <summary>
    /// Gets the current ship.
    /// </summary>
    public Ship? CurrentShip => _starSystemProvider.CurrentShip;

    /// <summary>
    /// Gets a value indicating whether no ship data is available.
    /// </summary>
    public bool HasNoShipData => CurrentShip == null;

    /// <summary>
    /// Gets a value indicating whether both source and target systems are selected.
    /// </summary>
    public bool IsSelectionValid => SelectedSourceSystem != null && SelectedTargetSystem != null;

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string WindowTitle => Resources.RoutePlotterWindow_Title;

    /// <summary>
    /// Gets the supercharged info text.
    /// </summary>
    public string InfoSupercharged
    {
        get
        {
            if (CurrentShip == null || !CurrentShip.JetConeBoost)
            {
                return Resources.RoutePlotter_No;
            }
            return $"{Resources.RoutePlotter_Yes} ({CurrentShip.JetConeBoostValue}x)";
        }
    }

    /// <summary>
    /// Gets the FSD booster info text.
    /// </summary>
    public string InfoBooster
    {
        get
        {
            if (CurrentShip == null || CurrentShip.GuardianFsdBooster == null)
            {
                return Resources.RoutePlotter_No;
            }
            return $"{CurrentShip.GuardianFsdBooster.JumpBoost} Ly";
        }
    }

    /// <summary>
    /// Gets the info text shown while a route calculation is running.
    /// </summary>
    public string LoadingText =>
        $"{Resources.RoutePlotterWindow_Loading_Part1}{Globals.PlotterUrl}\n" +
        $"{Resources.RoutePlotterWindow_Loading_Line2}\n\n" +
        $"{Resources.RoutePlotterWindow_Loading_Line4}\n" +
        $"{Resources.RoutePlotterWindow_Loading_Line5}";

    /// <summary>
    /// Gets the populator used by the source system auto-complete box.
    /// </summary>
    public Func<string, CancellationToken, Task<IEnumerable<object>>> SourceSystemPopulator { get; }

    /// <summary>
    /// Gets the populator used by the target system auto-complete box.
    /// </summary>
    public Func<string, CancellationToken, Task<IEnumerable<object>>> TargetSystemPopulator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutePlotterViewModel"/> class.
    /// </summary>
    /// <param name="mainViewModel">The main view model.</param>
    /// <param name="starSystemProvider">The provider for star system data.</param>
    /// <param name="routeProvider">The provider for route data.</param>
    /// <param name="webApiProvider">The provider for web API data.</param>
    public RoutePlotterViewModel(MainViewModel mainViewModel, StarSystemProvider starSystemProvider, RouteProvider routeProvider, WebApiProvider webApiProvider)
    {
        _mainViewModel = mainViewModel;
        _starSystemProvider = starSystemProvider;
        _routeProvider = routeProvider;
        _webApiProvider = webApiProvider;

        SourceSystemPopulator = PopulateSystemsAsync;
        TargetSystemPopulator = PopulateSystemsAsync;

        _starSystemProvider.GuiDataUpdated += OnGuiDataUpdated;
        _starSystemProvider.GuiShipFuelDataUpdated += OnGuiShipFuelDataUpdated;
        _webApiProvider.WebApiLoadingStatusChanged += OnWebApiLoadingStatusChanged;
    }

    /// <summary>
    /// Called when the window is opened. Pre-selects the current system as source if possible.
    /// </summary>
    public void OnWindowOpened()
    {
        RequestCurrentSystemData();
    }

    /// <summary>
    /// Called when the window is closed. Cancels a running calculation and unsubscribes from events.
    /// </summary>
    public void OnWindowClosed()
    {
        _webApiProvider.SpanshRequestCancellationOfGalaxyRouteCalculation();
        _starSystemProvider.GuiDataUpdated -= OnGuiDataUpdated;
        _starSystemProvider.GuiShipFuelDataUpdated -= OnGuiShipFuelDataUpdated;
        _webApiProvider.WebApiLoadingStatusChanged -= OnWebApiLoadingStatusChanged;
        Preferences.ReloadUserSettings();
    }

    /// <summary>
    /// Requests a neutron route calculation from Spansh.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsSelectionValid))]
    private void GenerateRoute()
    {
        var currentShip = _starSystemProvider.CurrentShip;
        if (currentShip == null)
        {
            log.Warn("Cannot calculate neutron route: no current ship");
            HasError = true;
            return;
        }

        double range = currentShip.CurrentJumpRange > 0.5 ? currentShip.CurrentJumpRange - 0.5 : currentShip.MaxJumpRange - 0.5;
        int efficiency = Preferences.Spansh.Efficiency;
        double superchargeMultiplier = currentShip.FrameShiftDrive?.JumpBoostMultiplier ?? 0.0;
        if (superchargeMultiplier <= 1.0)
        {
            superchargeMultiplier = 4.0;
        }

        Preferences.SaveUserSettings();
        LoadingProgress = 0.0;
        IsLoading = true;

        log.Info($"Route plotter request: from '{SelectedSourceSystem!.Name}' to '{SelectedTargetSystem!.Name}', " +
            $"range {range:F2} Ly, efficiency {efficiency}%, supercharge multiplier {superchargeMultiplier}x");

        _webApiProvider.SpanshRequestNeutronRouteCalculation(SelectedSourceSystem!, SelectedTargetSystem!, range, efficiency, superchargeMultiplier, RequestDelayMilliseconds, OnRouteCalculationResponse);
    }

    /// <summary>
    /// Hides the error message so the user can try again.
    /// </summary>
    [RelayCommand]
    private void DismissError()
    {
        HasError = false;
    }

    /// <summary>
    /// Requests the window to be closed.
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the route calculation response.
    /// </summary>
    /// <param name="webApiParameter">The web API response parameter.</param>
    private void OnRouteCalculationResponse(WebApiParameter webApiParameter)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var jumps = (webApiParameter as WebApiParameterSpanshGalaxyRoute)?.Jumps;
            if (jumps != null && jumps.Count > 1 && _routeProvider.ImportPlotterRoute(jumps))
            {
                _mainViewModel.OpenRouteTab();
                CloseRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            log.Warn($"An error occurred while processing galaxy plotter response {jumps}");
            IsLoading = false;
            HasError = true;
        });
    }

    /// <summary>
    /// Queries Spansh for systems matching the search text.
    /// </summary>
    /// <param name="searchText">The search text.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching systems.</returns>
    private Task<IEnumerable<object>> PopulateSystemsAsync(string searchText, CancellationToken cancellationToken)
    {
        var completion = new TaskCompletionSource<IEnumerable<object>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var results = new ObservableCollection<StarSystem>();

        cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken));
        _ = Task.Delay(SearchTimeoutMilliseconds, CancellationToken.None)
            .ContinueWith(_ => completion.TrySetResult(Enumerable.Empty<object>()));

        _webApiProvider.SpanshRequestBasicSystemData(searchText, results, _ => completion.TrySetResult(results.Cast<object>()));

        return completion.Task;
    }

    /// <summary>
    /// Requests basic data for the current system to pre-select it as the source.
    /// </summary>
    private void RequestCurrentSystemData()
    {
        var current = _starSystemProvider.CurrentSystem;
        if (current.Id == 0L || string.IsNullOrEmpty(current.Name))
        {
            return;
        }

        if (SelectedSourceSystem == null || current.Name != SelectedSourceSystem.Name)
        {
            _webApiProvider.SpanshRequestBasicSystemData(current.Name, _currentSystemSearchResults, OnCurrentSystemDataResponse);
        }
    }

    /// <summary>
    /// Handles the response with current system data.
    /// </summary>
    /// <param name="webApiParameter">The web API response parameter.</param>
    private void OnCurrentSystemDataResponse(WebApiParameter webApiParameter)
    {
        Dispatcher.UIThread.Post(() =>
        {
            SelectedSourceSystem = _currentSystemSearchResults.FirstOrDefault(s => s.Id == _starSystemProvider.CurrentSystem.Id);
        });
    }

    /// <summary>
    /// Handles general GUI data updates.
    /// </summary>
    private void OnGuiDataUpdated(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            RequestCurrentSystemData();
            OnPropertyChanged(nameof(CurrentShip));
            OnPropertyChanged(nameof(InfoBooster));
            OnPropertyChanged(nameof(InfoSupercharged));
            OnPropertyChanged(nameof(HasNoShipData));
        });
    }

    /// <summary>
    /// Handles ship fuel data updates.
    /// </summary>
    private void OnGuiShipFuelDataUpdated(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() => OnPropertyChanged(nameof(CurrentShip)));
    }

    /// <summary>
    /// Handles web API loading status updates for the route calculation.
    /// </summary>
    private void OnWebApiLoadingStatusChanged(object? sender, WebApiParameter? webApiParameter)
    {
        if (webApiParameter is WebApiParameterSpanshGalaxyRoute galaxyRoute)
        {
            Dispatcher.UIThread.Post(() =>
            {
                LoadingProgress = galaxyRoute.RequestDelay / 1000 * galaxyRoute.RequestCount;
            });
        }
    }

    partial void OnSelectedSourceSystemChanged(StarSystem? value)
    {
        OnPropertyChanged(nameof(IsSelectionValid));
        GenerateRouteCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedTargetSystemChanged(StarSystem? value)
    {
        OnPropertyChanged(nameof(IsSelectionValid));
        GenerateRouteCommand.NotifyCanExecuteChanged();
    }
}
