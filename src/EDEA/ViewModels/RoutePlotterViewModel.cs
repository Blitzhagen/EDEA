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

public class RoutePlotterViewModel : ViewModelBase
{
    private StarSystem? selectedSourceSystem;
    private StarSystem? selectedTargetSystem;

    private static readonly ILog log = LogManager.GetLogger(typeof(RoutePlotterViewModel));

    private static RoutePlotterWindow? routePlotterWindow;

    private readonly MainViewModel _mainViewModel;
    private readonly StarSystemProvider _starSystemProvider;
    private readonly RouteProvider _routeProvider;
    private readonly WebApiProvider _webApiProvider;

    private Button? generateRouteButton;
    private TextBlock? sourceSystemTextBlock;
    private Popup? sourceSystemSearchPopUp;
    private TextBox? sourceSystemSearchTextBox;
    private ListBox? sourceSystemSearchResultListBox;
    private TextBlock? targetSystemTextBlock;
    private Popup? targetSystemSearchPopUp;
    private TextBox? targetSystemSearchTextBox;
    private ListBox? targetSystemSearchResultListBox;
    private Timer? searchTextIsChangingTimer;
    private Grid? loadingMessageGrid;
    private Grid? errorMessageGrid;
    private Button? tryAgainErrorButton;
    private ProgressBar? loadingProgressBar;

    private const int searchTextIsChangingTimerInterval = 300;

    public bool RoutePlotterWindowOpen => routePlotterWindow != null;

    public ObservableCollection<StarSystem> SelectableSourceSystems { get; }

    public ObservableCollection<StarSystem> SelectableTargetSystems { get; }

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

    public Ship? CurrentShip => _starSystemProvider.CurrentShip;

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

    public bool HasNoShipData => CurrentShip == null;

    public string WindowTitle => Resources.RoutePlotterWindow_Title;

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

    public ICommand? CancelButtonCommand { get; private set; }

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

    private void onResponseCurrentSystemData(WebApiParameter webApiParameter)
    {
        SelectedSourceSystem = SelectableSourceSystems.FirstOrDefault(s => s.Id == _starSystemProvider.CurrentSystem.Id);
    }

    private void routePlotterWindow_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        hideTargetSearchBox();
        hideSourceSearchBox();
    }

    private void sourceSystemTextBlock_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock)
        {
            e.Handled = true;
            hideTargetSearchBox();
            showSourceSearchBox();
        }
    }

    private void targetSystemTextBlock_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock)
        {
            e.Handled = true;
            hideSourceSearchBox();
            showTargetSearchBox();
        }
    }

    private void sourceSystemSearchResultListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is StarSystem starSystem)
        {
            SelectedSourceSystem = starSystem;
            hideSourceSearchBox();
        }
    }

    private void targetSystemSearchResultListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is StarSystem starSystem)
        {
            SelectedTargetSystem = starSystem;
            hideTargetSearchBox();
        }
    }

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

    private void resetTimer()
    {
        killTimer();
        searchTextIsChangingTimer = new Timer();
        searchTextIsChangingTimer.Interval = searchTextIsChangingTimerInterval;
        searchTextIsChangingTimer.Enabled = true;
    }

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

    private void routePlotterWindow_LostFocus(object? sender, EventArgs e)
    {
        hideSourceSearchBox();
        hideTargetSearchBox();
    }

    private void routePlotterWindow_Closed(object? sender, EventArgs e)
    {
        killTimer();
        _webApiProvider.SpanshRequestCancellationOfGalaxyRouteCalculation();
        routePlotterWindow = null;
        _webApiProvider.WebApiLoadingStatusChanged -= _webApiProvider_WebApiLoadingStatusChanged;
        Preferences.ReloadUserSettings();
    }

    private void hideSourceSearchBox()
    {
        if (sourceSystemSearchTextBox != null)
            sourceSystemSearchTextBox!.Tag = sourceSystemSearchTextBox!.Text;
        if (sourceSystemSearchPopUp != null)
            sourceSystemSearchPopUp.IsOpen = false;
    }

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

    private void hideTargetSearchBox()
    {
        if (targetSystemSearchTextBox != null)
            targetSystemSearchTextBox!.Tag = targetSystemSearchTextBox!.Text;
        if (targetSystemSearchPopUp != null)
            targetSystemSearchPopUp.IsOpen = false;
    }

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

    private void _starSystemProvider_GuiShipFuelDataUpdated(object? sender, EventArgs e)
    {
        routePlotterWindow?.Dispatcher.Invoke(() =>
        {
            OnPropertyChanged("CurrentShip");
        });
    }

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
