using System;
using System.Collections.ObjectModel;
using System.Linq;
using Timer = System.Timers.Timer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using EDEA.Commands;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using EDEA.Windows;
using log4net;

namespace EDEA.ViewModels;

/// <summary>
/// View model that controls the neutron route plotter window.
/// </summary>
public class RoutePlotterViewModel : ViewModelBase
{
    /// <summary>
    /// The selected source star system.
    /// </summary>
    private StarSystem? selectedSourceSystem;

    /// <summary>
    /// The selected target star system.
    /// </summary>
    private StarSystem? selectedTargetSystem;

    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(RoutePlotterViewModel));

    /// <summary>
    /// Holds the singleton instance of the route plotter window.
    /// </summary>
    private static RoutePlotterWindow? routePlotterWindow;

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
    /// The generate route button.
    /// </summary>
    private Button? generateRouteButton;

    /// <summary>
    /// The source system text block.
    /// </summary>
    private TextBlock? sourceSystemTextBlock;

    /// <summary>
    /// The source system search popup.
    /// </summary>
    private Popup? sourceSystemSearchPopUp;

    /// <summary>
    /// The source system search text box.
    /// </summary>
    private TextBox? sourceSystemSearchTextBox;

    /// <summary>
    /// The source system search result list box.
    /// </summary>
    private ListBox? sourceSystemSearchResultListBox;

    /// <summary>
    /// The target system text block.
    /// </summary>
    private TextBlock? targetSystemTextBlock;

    /// <summary>
    /// The target system search popup.
    /// </summary>
    private Popup? targetSystemSearchPopUp;

    /// <summary>
    /// The target system search text box.
    /// </summary>
    private TextBox? targetSystemSearchTextBox;

    /// <summary>
    /// The target system search result list box.
    /// </summary>
    private ListBox? targetSystemSearchResultListBox;

    /// <summary>
    /// The timer used to delay system search requests while the user is typing.
    /// </summary>
    private Timer? searchTextIsChangingTimer;

    /// <summary>
    /// The loading message grid.
    /// </summary>
    private Grid? loadingMessageGrid;

    /// <summary>
    /// The error message grid.
    /// </summary>
    private Grid? errorMessageGrid;

    /// <summary>
    /// The try again error button.
    /// </summary>
    private Button? tryAgainErrorButton;

    /// <summary>
    /// The loading progress bar.
    /// </summary>
    private ProgressBar? loadingProgressBar;

    /// <summary>
    /// The interval in milliseconds for the search text change timer.
    /// </summary>
    private const int searchTextIsChangingTimerInterval = 300;

    /// <summary>
    /// Gets a value indicating whether the route plotter window is open.
    /// </summary>
    /// <value><c>true</c> if the plotter window is open; otherwise, <c>false</c>.</value>
    public bool RoutePlotterWindowOpen => routePlotterWindow != null;

    /// <summary>
    /// Gets the collection of selectable source systems.
    /// </summary>
    /// <value>The source systems.</value>
    public ObservableCollection<StarSystem> SelectableSourceSystems { get; }

    /// <summary>
    /// Gets the collection of selectable target systems.
    /// </summary>
    /// <value>The target systems.</value>
    public ObservableCollection<StarSystem> SelectableTargetSystems { get; }

    /// <summary>
    /// Gets or sets the selected source star system.
    /// </summary>
    /// <value>The source system, or <c>null</c>.</value>
    public StarSystem? SelectedSourceSystem
    {
        get => selectedSourceSystem;
        set
        {
            if (selectedSourceSystem != value)
            {
                selectedSourceSystem = value;
                OnPropertyChanged("SelectedSourceSystem");
                OnPropertyChanged("IsSelectionValid");
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected target star system.
    /// </summary>
    /// <value>The target system, or <c>null</c>.</value>
    public StarSystem? SelectedTargetSystem
       {
        get => selectedTargetSystem;
        set
        {
            if (selectedTargetSystem != value)
            {
                selectedTargetSystem = value;
                OnPropertyChanged("SelectedTargetSystem");
                OnPropertyChanged("IsSelectionValid");
            }
        }
    }

    /// <summary>
    /// Gets the current ship.
    /// </summary>
    /// <value>The ship, or <c>null</c>.</value>
    public Ship? CurrentShip => _starSystemProvider.CurrentShip;

    /// <summary>
    /// Gets a value indicating whether both source and target systems are selected.
    /// </summary>
    /// <value><c>true</c> if the selection is valid; otherwise, <c>false</c>.</value>
    public bool IsSelectionValid
    {
        get
        {
            if (SelectedSourceSystem != null)
            {
                return SelectedTargetSystem != null;
            }
            return false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether no ship data is available.
    /// </summary>
    /// <value><c>true</c> if no ship data is available; otherwise, <c>false</c>.</value>
    public bool HasNoShipData => CurrentShip == null;

    /// <summary>
    /// Gets the window title.
    /// </summary>
    /// <value>The window title string.</value>
    public string WindowTitle => Resources.RoutePlotterWindow_Title;

    /// <summary>
    /// Gets the supercharged info text.
    /// </summary>
    /// <value>The supercharged info string.</value>
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
    /// <value>The booster info string.</value>
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
    /// Gets the command that cancels the plotter.
    /// </summary>
    /// <value>The cancel command, or <c>null</c>.</value>
    public ICommand? CancelButtonCommand { get; private set; }

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
        SelectableSourceSystems = new ObservableCollection<StarSystem>();
        SelectableTargetSystems = new ObservableCollection<StarSystem>();
        _starSystemProvider.GuiDataUpdated += _starSystemProvider_GuiDataUpdated;
        _starSystemProvider.GuiShipFuelDataUpdated += _starSystemProvider_GuiShipFuelDataUpdated;
    }

    /// <summary>
    /// Shows the route plotter window or activates it if already open.
    /// </summary>
    public void ShowRoutePlotterWindow()
    {
        if (routePlotterWindow == null)
        {
            routePlotterWindow = new RoutePlotterWindow();
            JotSettingsProvider.Tracker.Track(routePlotterWindow);
            routePlotterWindow.DataContext = this;
            routePlotterWindow.Closed += routePlotterWindow_Closed;
            routePlotterWindow.MouseDown += routePlotterWindow_MouseDown;
            routePlotterWindow.LostFocus += routePlotterWindow_LostFocus;
            routePlotterWindow.Deactivated += routePlotterWindow_LostFocus;
            CancelButtonCommand = new CloseWindowCommand(routePlotterWindow);

            loadingMessageGrid = (Grid)routePlotterWindow.FindName("LoadingMessageGrid");
            loadingProgressBar = (ProgressBar)routePlotterWindow.FindName("LoadingProgressBar");
            errorMessageGrid = (Grid)routePlotterWindow.FindName("ErrorMessageGrid");
            generateRouteButton = (Button)routePlotterWindow.FindName("GenerateRouteButton");
            if (generateRouteButton != null)
            {
                generateRouteButton.Click += requestRouteCalculation;
            }
            tryAgainErrorButton = (Button)routePlotterWindow.FindName("TryAgainErrorButton");
            if (tryAgainErrorButton != null)
            {
                tryAgainErrorButton.Click += delegate
                {
                    if (errorMessageGrid != null)
                        errorMessageGrid.Visibility = Visibility.Collapsed;
                };
            }

            sourceSystemTextBlock = (TextBlock)routePlotterWindow.FindName("SourceSystemTextBlock");
            if (sourceSystemTextBlock != null)
            {
                sourceSystemTextBlock.MouseDown += sourceSystemTextBlock_MouseDown;
            }
            sourceSystemSearchPopUp = (Popup)routePlotterWindow.FindName("SourceSystemSearchPopUp");
            sourceSystemSearchTextBox = (TextBox)routePlotterWindow.FindName("SourceSystemSearchTextBox");
            if (sourceSystemSearchTextBox != null)
            {
                sourceSystemSearchTextBox!.TextChanged += sourceSystemSearchTextBox_TextChanged;
            }
            sourceSystemSearchResultListBox = (ListBox)routePlotterWindow.FindName("SourceSystemSearchResultListBox");
            if (sourceSystemSearchResultListBox != null)
            {
                sourceSystemSearchResultListBox.SelectionChanged += sourceSystemSearchResultListBox_SelectionChanged;
            }
            hideSourceSearchBox();

            targetSystemTextBlock = (TextBlock)routePlotterWindow.FindName("TargetSystemTextBlock");
            if (targetSystemTextBlock != null)
            {
                targetSystemTextBlock.MouseDown += targetSystemTextBlock_MouseDown;
            }
            targetSystemSearchPopUp = (Popup)routePlotterWindow.FindName("TargetSystemSearchPopUp");
            targetSystemSearchTextBox = (TextBox)routePlotterWindow.FindName("TargetSystemSearchTextBox");
            if (targetSystemSearchTextBox != null)
            {
                targetSystemSearchTextBox!.TextChanged += targetSystemSearchTextBox_TextChanged;
            }
            targetSystemSearchResultListBox = (ListBox)routePlotterWindow.FindName("TargetSystemSearchResultListBox");
            if (targetSystemSearchResultListBox != null)
            {
                targetSystemSearchResultListBox.SelectionChanged += targetSystemSearchResultListBox_SelectionChanged;
            }
            hideTargetSearchBox();

            requestCurrentSystemData();
            _webApiProvider.WebApiLoadingStatusChanged += _webApiProvider_WebApiLoadingStatusChanged;
            routePlotterWindow.Show();
            log.Debug(_starSystemProvider.CurrentShip?.SLEF?.ToString() ?? string.Empty);
        }
        else
        {
            if (!routePlotterWindow.IsActive)
            {
                routePlotterWindow.Activate();
            }
            if (!routePlotterWindow.IsFocused)
            {
                routePlotterWindow.Focus();
            }
        }
    }

    /// <summary>
    /// Requests a neutron route calculation from the web API.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void requestRouteCalculation(object? sender, RoutedEventArgs e)
    {
        const int requestDelay = 2000;

        Ship? currentShip = _starSystemProvider.CurrentShip;
        if (currentShip == null)
        {
            log.Warn("Cannot calculate neutron route: no current ship");
            if (errorMessageGrid != null)
                errorMessageGrid.Visibility = Visibility.Visible;
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
        if (loadingProgressBar != null)
        {
            loadingProgressBar.Value = 0.0;
            loadingProgressBar.Maximum = 60;
        }
        if (loadingMessageGrid != null)
            loadingMessageGrid.Visibility = Visibility.Visible;

        _webApiProvider.SpanshRequestNeutronRouteCalculation(selectedSourceSystem!, selectedTargetSystem!, range, efficiency, superchargeMultiplier, requestDelay, onResponseRouteCalculation);
    }

    /// <summary>
    /// Handles the route calculation response.
    /// </summary>
    /// <param name="webApiParameter">The web API response parameter.</param>
    private void onResponseRouteCalculation(WebApiParameter webApiParameter)
    {
        if (routePlotterWindow == null)
            return;

        var jumps = (webApiParameter as WebApiParameterSpanshGalaxyRoute)?.Jumps;

        if (jumps != null && jumps.Count > 1 && _routeProvider.ImportPlotterRoute(jumps))
        {
            _mainViewModel.OpenTabOfType(typeof(NavRouteTableViewModel), true);
            routePlotterWindow.Close();
            return;
        }

        log.Warn($"An error occurred while processing galaxy plotter response {jumps}");
        if (errorMessageGrid != null)
            errorMessageGrid.Visibility = Visibility.Visible;
        if (loadingMessageGrid != null)
            loadingMessageGrid.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Requests basic data for the current system.
    /// </summary>
    private void requestCurrentSystemData()
    {
        if (routePlotterWindow == null || _starSystemProvider.CurrentSystem.Id == 0L)
            return;

        var current = _starSystemProvider.CurrentSystem;
        if (!string.IsNullOrEmpty(current.Name) && (SelectedSourceSystem == null || current.Name != SelectedSourceSystem.Name))
        {
            _webApiProvider.SpanshRequestBasicSystemData(current.Name, SelectableSourceSystems, onResponseCurrentSystemData);
        }
    }

    /// <summary>
    /// Handles the response with current system data.
    /// </summary>
    /// <param name="webApiParameter">The web API response parameter.</param>
    private void onResponseCurrentSystemData(WebApiParameter webApiParameter)
    {
        SelectedSourceSystem = SelectableSourceSystems.FirstOrDefault(s => s.Id == _starSystemProvider.CurrentSystem.Id);
    }

    /// <summary>
    /// Handles the plotter window mouse down event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void routePlotterWindow_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        hideTargetSearchBox();
        hideSourceSearchBox();
    }

    /// <summary>
    /// Handles the source system text block mouse down event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void sourceSystemTextBlock_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock)
        {
            e.Handled = true;
            hideTargetSearchBox();
            showSourceSearchBox();
        }
    }

    /// <summary>
    /// Handles the target system text block mouse down event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void targetSystemTextBlock_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock)
        {
            e.Handled = true;
            hideSourceSearchBox();
            showTargetSearchBox();
        }
    }

    /// <summary>
    /// Handles the source system search result selection change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void sourceSystemSearchResultListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is StarSystem starSystem)
        {
            SelectedSourceSystem = starSystem;
            hideSourceSearchBox();
        }
    }

    /// <summary>
    /// Handles the target system search result selection change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void targetSystemSearchResultListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is StarSystem starSystem)
        {
            SelectedTargetSystem = starSystem;
            hideTargetSearchBox();
        }
    }

    /// <summary>
    /// Stops and disposes the search text timer.
    /// </summary>
    private void killTimer()
    {
        if (searchTextIsChangingTimer != null)
        {
            searchTextIsChangingTimer.Stop();
            searchTextIsChangingTimer.Enabled = false;
            searchTextIsChangingTimer.Dispose();
            searchTextIsChangingTimer = null;
        }
    }

    /// <summary>
    /// Resets the search text timer.
    /// </summary>
    private void resetTimer()
    {
        killTimer();
        searchTextIsChangingTimer = new Timer();
        searchTextIsChangingTimer.Interval = searchTextIsChangingTimerInterval;
        searchTextIsChangingTimer.Enabled = true;
    }

    /// <summary>
    /// Handles the source system search text box text change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void sourceSystemSearchTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sourceSystemSearchTextBox == null)
            return;

        if (sender is TextBox &&
            sourceSystemSearchTextBox!.Text != selectedSourceSystem?.Name &&
            sourceSystemSearchTextBox!.Text != (string?)sourceSystemSearchTextBox!.Tag)
        {
            resetTimer();
            searchTextIsChangingTimer!.Elapsed += delegate
            {
                requestSystems(sourceSystemSearchTextBox!, SelectableSourceSystems);
            };
            searchTextIsChangingTimer!.Start();
        }
    }

    /// <summary>
    /// Handles the target system search text box text change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void targetSystemSearchTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (targetSystemSearchTextBox == null)
            return;

        if (sender is TextBox &&
            targetSystemSearchTextBox!.Text != selectedTargetSystem?.Name &&
            targetSystemSearchTextBox!.Text != (string?)targetSystemSearchTextBox!.Tag)
        {
            resetTimer();
            searchTextIsChangingTimer!.Elapsed += delegate
            {
                requestSystems(targetSystemSearchTextBox!, SelectableTargetSystems);
            };
            searchTextIsChangingTimer!.Start();
        }
    }

    /// <summary>
    /// Requests systems matching the search text from the web API.
    /// </summary>
    /// <param name="searchTextBox">The text box with the search text.</param>
    /// <param name="starSystems">The collection to populate with results.</param>
    private void requestSystems(TextBox searchTextBox, ObservableCollection<StarSystem> starSystems)
    {
        killTimer();

        if (routePlotterWindow == null)
            return;

        routePlotterWindow.Dispatcher.Invoke(delegate
        {
            if (searchTextBox.Text.Length > 0)
            {
                starSystems.Clear();
                _webApiProvider.SpanshRequestBasicSystemData(searchTextBox.Text, starSystems, delegate { });
            }
        });
    }

    /// <summary>
    /// Handles the plotter window lost focus event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void routePlotterWindow_LostFocus(object? sender, EventArgs e)
    {
        hideSourceSearchBox();
        hideTargetSearchBox();
    }

    /// <summary>
    /// Handles the <see cref="RoutePlotterWindow.Closed"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void routePlotterWindow_Closed(object? sender, EventArgs e)
    {
        killTimer();
        _webApiProvider.SpanshRequestCancellationOfGalaxyRouteCalculation();
        routePlotterWindow = null;
        _webApiProvider.WebApiLoadingStatusChanged -= _webApiProvider_WebApiLoadingStatusChanged;
        Preferences.ReloadUserSettings();
    }

    /// <summary>
    /// Hides the source system search box.
    /// </summary>
    private void hideSourceSearchBox()
    {
        if (sourceSystemSearchTextBox != null)
            sourceSystemSearchTextBox!.Tag = sourceSystemSearchTextBox!.Text;
        if (sourceSystemSearchPopUp != null)
            sourceSystemSearchPopUp.IsOpen = false;
    }

    /// <summary>
    /// Shows the source system search box.
    /// </summary>
    private void showSourceSearchBox()
    {
        if (sourceSystemSearchTextBox == null)
            return;

        if (selectedSourceSystem != null)
        {
            sourceSystemSearchTextBox!.Text = selectedSourceSystem.Name;
        }
        else
        {
            sourceSystemSearchTextBox!.Text = (string?)sourceSystemSearchTextBox!.Tag;
        }
        if (sourceSystemSearchPopUp != null)
            sourceSystemSearchPopUp.IsOpen = true;
        sourceSystemSearchTextBox!.Focus();
        sourceSystemSearchTextBox!.CaretIndex = 0;
        sourceSystemSearchTextBox!.SelectionLength = sourceSystemSearchTextBox!.Text!.Length;
    }

    /// <summary>
    /// Hides the target system search box.
    /// </summary>
    private void hideTargetSearchBox()
    {
        if (targetSystemSearchTextBox != null)
            targetSystemSearchTextBox!.Tag = targetSystemSearchTextBox!.Text;
        if (targetSystemSearchPopUp != null)
            targetSystemSearchPopUp.IsOpen = false;
    }

    /// <summary>
    /// Shows the target system search box.
    /// </summary>
    private void showTargetSearchBox()
    {
        if (targetSystemSearchTextBox == null)
            return;

        if (selectedTargetSystem != null)
        {
            targetSystemSearchTextBox!.Text = selectedTargetSystem.Name;
        }
        else
        {
            targetSystemSearchTextBox!.Text = (string?)targetSystemSearchTextBox!.Tag;
        }
        if (targetSystemSearchPopUp != null)
            targetSystemSearchPopUp.IsOpen = true;
        targetSystemSearchTextBox!.Focus();
        targetSystemSearchTextBox!.CaretIndex = 0;
        targetSystemSearchTextBox!.SelectionLength = targetSystemSearchTextBox!.Text!.Length;
    }

    /// <summary>
    /// Handles general GUI data updates.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void _starSystemProvider_GuiDataUpdated(object? sender, EventArgs e)
    {
        routePlotterWindow?.Dispatcher.Invoke(() =>
        {
            requestCurrentSystemData();
            OnPropertyChanged("CurrentShip");
            OnPropertyChanged("InfoBooster");
            OnPropertyChanged("InfoSupercharged");
            OnPropertyChanged("HasNoShipData");
        });
        log.Debug("EDEA4711: Refresh of parts of RoutePlotterWindow");
    }

    /// <summary>
    /// Handles ship fuel data updates.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void _starSystemProvider_GuiShipFuelDataUpdated(object? sender, EventArgs e)
    {
        routePlotterWindow?.Dispatcher.Invoke(() =>
        {
            OnPropertyChanged("CurrentShip");
        });
    }

    /// <summary>
    /// Handles web API loading status updates for the route calculation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="webApiParameter">The web API parameter.</param>
    private void _webApiProvider_WebApiLoadingStatusChanged(object? sender, WebApiParameter? webApiParameter)
    {
        if (webApiParameter != null && webApiParameter.GetType() == typeof(WebApiParameterSpanshGalaxyRoute))
        {
            var webApiParameterSpanshGalaxyRoute = (WebApiParameterSpanshGalaxyRoute)webApiParameter;
            routePlotterWindow?.Dispatcher.Invoke(delegate
            {
                if (loadingProgressBar != null)
                    loadingProgressBar.Value = webApiParameterSpanshGalaxyRoute.RequestDelay / 1000 * webApiParameterSpanshGalaxyRoute.RequestCount;
            });
            log.Debug($"loading status changed: {webApiParameterSpanshGalaxyRoute.CalculationTime}, {webApiParameterSpanshGalaxyRoute.RequestCount}, {webApiParameterSpanshGalaxyRoute.RequestDelay}");
        }
    }
}
