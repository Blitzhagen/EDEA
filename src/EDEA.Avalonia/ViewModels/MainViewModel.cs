using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    /// <summary>
    /// Called when the selected tab index changes.
    /// </summary>
    partial void OnSelectedTabIndexChanged(int value)
    {
        if (value >= 0 && value < TabViewModels.Count)
        {
            SelectedTab = TabViewModels[value];
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
    /// Gets the command that opens the journal history import window.
    /// </summary>
    public ICommand ImportJournalHistoryCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class with the specified providers.
    /// </summary>
    public MainViewModel(StarSystemProvider starSystemProvider, HistoryProvider historyProvider, RouteProvider routeProvider, StatusProvider statusProvider)
    {
        _starSystemProvider = starSystemProvider;
        _statusProvider = statusProvider;

        _starSystemProvider.GuiDataUpdated += (_, _) => Dispatcher.UIThread.Post(UpdateDataView);
        _starSystemProvider.RouteLoadingStatusChanged += (_, _) => Dispatcher.UIThread.Post(() => DataIsLoading = _starSystemProvider.RouteIsLoading);
        _statusProvider.StatusUpdated += (_, activity, _, _, _) => Dispatcher.UIThread.Post(() => CurrentStatus = MapActivityToString(activity));

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

        ShowAboutWindowCommand = new RelayCommand(ShowAboutWindow);
        ShowPreferencesWindowCommand = new RelayCommand(ShowPreferencesWindow);
        ShowFeedbackReportIssueWindowCommand = new RelayCommand(ShowFeedbackReportIssueWindow);
        OpenCloseHudWindowCommand = new RelayCommand(OpenCloseHudWindow);
        ImportJournalHistoryCommand = new RelayCommand(ImportJournalHistory);
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
            _hudWindow = null;
            _hudClickThrough = false;
        }
        else
        {
            _hudWindow = new HudWindow(_starSystemProvider);
            _hudWindow.Closed += (_, _) =>
            {
                _hudWindow = null;
                _hudClickThrough = false;
            };
            _hudWindow.Show();
        }
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

        var handle = PlatformServices.HudWindow?.GetWindowHandle(_hudWindow) ?? 0;
        if (handle == 0)
        {
            return;
        }

        _hudClickThrough = !_hudClickThrough;
        PlatformServices.HudWindow?.SetClickThrough(handle, _hudClickThrough);
    }

    private void ImportJournalHistory()
    {
        new JournalHistoryImportWindow().Show();
    }

    private void UpdateDataView()
    {
        CurrentSystemViewModel = new StarSystemViewModel(_starSystemProvider.CurrentSystem);
        BodyExplorationStatus = string.Format(Resources.StatusBodiesExploredOfTotal, CurrentSystemViewModel.ExploredBodies, CurrentSystemViewModel.TotalBodies);
        NonBodyExplorationStatus = string.Format(Resources.StatusNonBodyBelts, CurrentSystemViewModel.TotalNonBodyCount, CurrentSystemViewModel.ExploredNonBodies);
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
