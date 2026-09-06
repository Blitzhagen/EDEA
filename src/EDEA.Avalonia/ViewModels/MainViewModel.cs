using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EDEA;
using EDEA.Avalonia.Windows;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using EDEA.ViewModels;

namespace EDEA.Avalonia.ViewModels;

/// <summary>
/// Main view model for the Avalonia application.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// The selected tab index.
    /// </summary>
    [ObservableProperty]
    private int _selectedTabIndex;

    /// <summary>
    /// The currently selected tab view model.
    /// </summary>
    [ObservableProperty]
    private TabViewModel? _selectedTab;

    partial void OnSelectedTabChanged(TabViewModel? value)
    {
        _starSystemProvider?.OnSurroundingsTabSelected(value is SurroundingsTableViewModel);
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        if (value >= 0 && value < TabViewModels.Count)
        {
            var tab = TabViewModels[value];
            if (tab.TabVisibility.Equals("Visible", StringComparison.OrdinalIgnoreCase))
            {
                SelectedTab = tab;
            }
            else
            {
                EnsureSelectedTabVisible();
            }
        }
    }

    /// <summary>
    /// Ensures the selected tab is visible, otherwise falls back to the first visible tab.
    /// </summary>
    private void EnsureSelectedTabVisible()
    {
        var visibleTab = TabViewModels.FirstOrDefault(t => t.TabVisibility.Equals("Visible", StringComparison.OrdinalIgnoreCase));
        if (visibleTab != null)
        {
            SelectedTabIndex = TabViewModels.IndexOf(visibleTab);
            SelectedTab = visibleTab;
        }
    }

    /// <summary>
    /// The collection of tab view models.
    /// </summary>
    public ObservableCollection<TabViewModel> TabViewModels { get; }

    /// <summary>
    /// The star system provider.
    /// </summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>
    /// The status provider.
    /// </summary>
    private readonly StatusProvider _statusProvider;

    /// <summary>
    /// The last activity used to avoid redundant automatic tab switches.
    /// </summary>
    private Activity _lastActivity = Activity.Other;

    /// <summary>
    /// The currently open HUD window, if any.
    /// </summary>
    private HudWindow? _hudWindow;

    /// <summary>
    /// Whether the HUD window is currently in click-through mode.
    /// </summary>
    private bool _hudClickThrough;

    /// <summary>
    /// The current star system view model.
    /// </summary>
    [ObservableProperty]
    private StarSystemViewModel _currentSystemViewModel = new StarSystemViewModel(new StarSystem(0L, string.Empty));

    /// <summary>
    /// The formatted body exploration status.
    /// </summary>
    [ObservableProperty]
    private string _bodyExplorationStatus = string.Empty;

    /// <summary>
    /// The formatted non-body exploration status.
    /// </summary>
    [ObservableProperty]
    private string _nonBodyExplorationStatus = string.Empty;

    /// <summary>
    /// The current application status text.
    /// </summary>
    [ObservableProperty]
    private string _currentStatus = Resources.CurrentStatus_WaitingForGame;

    /// <summary>
    /// A value indicating whether data is currently loading.
    /// </summary>
    [ObservableProperty]
    private bool _dataIsLoading;

    /// <summary>
    /// A value indicating whether the HUD window is currently in mouse pass-through mode.
    /// </summary>
    [ObservableProperty]
    private bool _hudWindowMousePassThroughEnabled;

    /// <summary>
    /// The menu header for the HUD mouse pass-through toggle command.
    /// </summary>
    [ObservableProperty]
    private string _toggleHudMousePassThroughMenuItemHeader = Resources.MenuItem_HudPassThroughEnable;

    /// <summary>
    /// Gets the main window title.
    /// </summary>
    public string Title => Resources.MainWindow_Title;

    /// <summary>
    /// Gets the command that opens the about window.
    /// </summary>
    public ICommand ShowAboutWindowCommand { get; }

    /// <summary>
    /// Gets the command that opens the preferences window.
    /// </summary>
    public ICommand ShowPreferencesWindowCommand { get; }

    /// <summary>
    /// Gets the command that opens the feedback window.
    /// </summary>
    public ICommand ShowFeedbackReportIssueWindowCommand { get; }

    /// <summary>
    /// Gets the command that opens the HUD window.
    /// </summary>
    public ICommand OpenCloseHudWindowCommand { get; }

    /// <summary>
    /// Gets the command that toggles mouse pass-through for the currently open HUD window.
    /// </summary>
    public ICommand ToggleHudMousePassThroughCommand { get; }

    /// <summary>
    /// Gets the command that opens the journal history import window.
    /// </summary>
    public ICommand ImportJournalHistoryCommand { get; }

    /// <summary>
    /// Gets the command that reloads EDSM data for the current star system.
    /// </summary>
    public ICommand ReloadEdsmDataCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class with the specified providers.
    /// </summary>
    public MainViewModel(StarSystemProvider starSystemProvider, HistoryProvider historyProvider, RouteProvider routeProvider, StatusProvider statusProvider)
    {
        _starSystemProvider = starSystemProvider;
        _statusProvider = statusProvider;

        _starSystemProvider.GuiDataUpdated += (_, _) => Dispatcher.UIThread.Post(UpdateDataView);
        _starSystemProvider.RouteLoadingStatusChanged += (_, _) => Dispatcher.UIThread.Post(() => DataIsLoading = _starSystemProvider.RouteIsLoading);

        var navRouteTableViewModel = new NavRouteTableViewModel(Resources.TabHeader_Route, "Visible", starSystemProvider, routeProvider);
        var bodyTableViewModel = new BodyTableViewModel(Resources.TabHeader_Bodies, "Visible", starSystemProvider);
        var genusTableViewModel = new GenusTableViewModel(Resources.TabHeader_Biologicals, "Collapsed", starSystemProvider);
        var surroundingsTableViewModel = new SurroundingsTableViewModel(Resources.TabHeader_Surroundings, "Visible", starSystemProvider);
        var historyViewModel = new HistoryViewModel(Resources.TabHeader_History, "Visible", historyProvider);

        TabViewModels = new ObservableCollection<TabViewModel>
        {
            navRouteTableViewModel,
            bodyTableViewModel,
            genusTableViewModel,
            surroundingsTableViewModel,
            historyViewModel,
        };

        SelectedTab = TabViewModels[0];

        foreach (var tab in TabViewModels)
        {
            tab.PropertyChanged += TabViewModel_PropertyChanged;
        }

        ShowAboutWindowCommand = new RelayCommand(ShowAboutWindow);
        ShowPreferencesWindowCommand = new RelayCommand(ShowPreferencesWindow);
        ShowFeedbackReportIssueWindowCommand = new RelayCommand(ShowFeedbackReportIssueWindow);
        OpenCloseHudWindowCommand = new RelayCommand(OpenCloseHudWindow);
        ToggleHudMousePassThroughCommand = new RelayCommand(ToggleHudMousePassThrough, () => _hudWindow != null);
        ImportJournalHistoryCommand = new RelayCommand(ImportJournalHistory);
        ReloadEdsmDataCommand = new RelayCommand(ReloadEdsmData);
    }

    private void ReloadEdsmData()
    {
        _starSystemProvider.HandleLoadEdsmSystemDataCommand(true);
    }

    private void ShowAboutWindow()
    {
        new AboutWindow().Show();
    }

    private void ShowPreferencesWindow()
    {
        new PreferencesWindow().Show();
    }

    private void ShowFeedbackReportIssueWindow()
    {
        new FeedbackReportIssueWindow().Show();
    }

    private void OpenCloseHudWindow()
    {
        if (_hudWindow != null)
        {
            _hudWindow.Close();
            ResetHudWindowState();
        }
        else
        {
            _hudClickThrough = Preferences.Application.HudWindowMousePassThroughEnabled;
            HudWindowMousePassThroughEnabled = _hudClickThrough;
            ToggleHudMousePassThroughMenuItemHeader = _hudClickThrough
                ? Resources.MenuItem_HudPassThroughDisable
                : Resources.MenuItem_HudPassThroughEnable;

            _hudWindow = new HudWindow(_starSystemProvider, this);
            _hudWindow.Opened += (_, _) =>
            {
                PlatformServices.HudWindow?.SetClickThrough(_hudWindow, _hudClickThrough);
            };
            _hudWindow.Closed += (_, _) =>
            {
                Dispatcher.UIThread.Post(ResetHudWindowState);
            };
            PlatformServices.WindowState?.Track(_hudWindow, "HudWindow");
            _hudWindow.Show();
        }

        ((IRelayCommand)ToggleHudMousePassThroughCommand).NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Resets the cached HUD window and mouse pass-through state.
    /// </summary>
    private void ResetHudWindowState()
    {
        if (_hudWindow != null)
        {
            PlatformServices.WindowState?.StopTracking(_hudWindow, false);
        }

        _hudWindow = null;
        _hudClickThrough = false;
        HudWindowMousePassThroughEnabled = false;
        ToggleHudMousePassThroughMenuItemHeader = Resources.MenuItem_HudPassThroughEnable;
        ((IRelayCommand)ToggleHudMousePassThroughCommand).NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Copies the name of the next route system to the clipboard if available.
    /// </summary>
    public void CopyNextSystemToClipboard()
    {
        _starSystemProvider.CopyNextSystemNametoClipboard();
    }

    /// <summary>
    /// Toggles mouse pass-through for the currently open HUD window.
    /// </summary>
    public void ToggleHudMousePassThrough()
    {
        if (_hudWindow == null)
        {
            return;
        }

        _hudClickThrough = !_hudClickThrough;
        PlatformServices.HudWindow?.SetClickThrough(_hudWindow, _hudClickThrough);
        HudWindowMousePassThroughEnabled = _hudClickThrough;
        ToggleHudMousePassThroughMenuItemHeader = _hudClickThrough
            ? Resources.MenuItem_HudPassThroughDisable
            : Resources.MenuItem_HudPassThroughEnable;
        Preferences.Application.HudWindowMousePassThroughEnabled = _hudClickThrough;
    }

    /// <summary>
    /// Restores the previously selected tab and any windows that were open on shutdown.
    /// </summary>
    public void RestoreWindows()
    {
        SelectedTabIndex = Math.Min(Math.Max(Preferences.Application.SelectedTabIndex, 0), TabViewModels.Count - 1);
        OnPropertyChanged(nameof(SelectedTabIndex));

        if (Preferences.Application.HudWindowOpenOnShutdown)
        {
            OpenCloseHudWindow();
        }
    }

    /// <summary>
    /// Saves window state information before the main window closes.
    /// </summary>
    public void OnMainWindowClosing()
    {
        Preferences.Application.HudWindowOpenOnShutdown = _hudWindow != null;
        Preferences.Application.SelectedTabIndex = SelectedTabIndex;
        Preferences.SaveUserSettings();
    }

    private void ImportJournalHistory()
    {
        new JournalHistoryImportWindow().Show();
    }

    private void TabViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TabViewModel.TabVisibility) && sender is TabViewModel tab)
        {
            if (SelectedTab == tab && !tab.TabVisibility.Equals("Visible", StringComparison.OrdinalIgnoreCase))
            {
                EnsureSelectedTabVisible();
            }
        }
    }

    private void UpdateDataView()
    {
        switch (_starSystemProvider.CurrentActivity)
        {
            case Activity.None:
                CurrentStatus = Resources.CurrentStatus_WaitingForGame;
                OpenTabOfType(typeof(HistoryViewModel), false);
                break;
            case Activity.ExploreSystem:
                CurrentStatus = Resources.CurrentStatus_ExploringSystem;
                OpenTabOfType(typeof(BodyTableViewModel), false);
                break;
            case Activity.GalaxyMap:
                CurrentStatus = Resources.CurrentStatus_PlanningRoute;
                OpenTabOfType(typeof(NavRouteTableViewModel), false);
                break;
            case Activity.Jump:
                CurrentStatus = Resources.CurrentStatus_Jumping;
                OpenTabOfType(typeof(NavRouteTableViewModel), false);
                break;
            case Activity.ExplorePlanet:
                {
                    CurrentStatus = Resources.CurrentStatus_ExploringPlanet + _starSystemProvider.CurrentPlanet?.ShortName;
                    Planet? currentPlanet = _starSystemProvider.CurrentPlanet;
                    if (currentPlanet == null || currentPlanet.Genuses.Count <= 0)
                    {
                        if (currentPlanet == null || currentPlanet.BiologicalCount <= 0)
                        {
                            OpenTabOfType(typeof(BodyTableViewModel), false);
                            break;
                        }
                    }

                    OpenTabOfType(typeof(GenusTableViewModel), false);
                    break;
                }
            case Activity.Other:
                CurrentStatus = Resources.CurrentStatus_Loitering;
                OpenTabOfType(typeof(HistoryViewModel), false);
                break;
        }

        _lastActivity = _starSystemProvider.CurrentActivity;
        CurrentSystemViewModel = new StarSystemViewModel(_starSystemProvider.CurrentSystem);
        BodyExplorationStatus = string.Format(Resources.StatusBodiesExploredOfTotal, CurrentSystemViewModel.ExploredBodies, CurrentSystemViewModel.TotalBodies);
        NonBodyExplorationStatus = string.Format(Resources.StatusNonBodyBelts, CurrentSystemViewModel.TotalNonBodyCount, CurrentSystemViewModel.ExploredNonBodies);
    }

    private void OpenTabOfType(Type type, bool forceOpen)
    {
        if (forceOpen || (_lastActivity != _starSystemProvider.CurrentActivity && Preferences.Other.AutomaticTabSwitching))
        {
            var match = TabViewModels.FirstOrDefault(x => x.GetType() == type);
            int tabIndex = match is null ? -1 : TabViewModels.IndexOf(match);
            if (tabIndex >= 0 && tabIndex != SelectedTabIndex)
            {
                SelectedTabIndex = tabIndex;
            }
        }
    }

    private static string MapActivityToString(Activity activity)
    {
        return activity switch
        {
            Activity.None => Resources.CurrentStatus_WaitingForGame,
            Activity.ExploreSystem => Resources.CurrentStatus_ExploringSystem,
            Activity.GalaxyMap => Resources.CurrentStatus_PlanningRoute,
            Activity.Jump => Resources.CurrentStatus_Jumping,
            Activity.ExplorePlanet => Resources.CurrentStatus_ExploringPlanet,
            _ => Resources.CurrentStatus_Loitering
        };
    }
}
