using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA;
using EDEA.Properties;
using EDEA.Commands;
using EDEA.Models;
using EDEA.Services;
using EDEA.Stores;
using EDEA.Windows;
using log4net;

namespace EDEA.ViewModels;

public class MainViewModel : ObservableObject
{
    private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));

    private int selectedTabIndex;
    private HudViewModel hudViewModel;
    private AboutViewModel aboutViewModel;
    private FeedbackReportIssueViewModel feedbackReportIssueViewModel;
    private PreferencesViewModel preferencesViewModel;
    private JournalHistoryImportViewModel importJournalHistoryViewModel;
    private Activity lastActivity = Activity.Other;

    private readonly StarSystemProvider _starSystemProvider;
    private readonly RouteProvider _routeProvider;
    private readonly WebApiProvider _webApiProvider;
    private readonly StatusProvider _statusProvider;
    private readonly HotkeyProvider _hotkeyProvider;

    private readonly RoutePlotterViewModel _routePlotterViewModel;

    public ICommand ReloadEdsmDataCommand { get; }
    public ICommand ShowAboutWindowCommand { get; }
    public ICommand OpenCloseHudWindowCommand { get; }
    public ICommand ShowFeedbackReportIssueWindowCommand { get; }
    public ICommand ClearHistoryCommand { get; }
    public ICommand GenerateClearPlotterRouteCommand { get; }
    public ICommand CopySystemNameToClipboardCommand { get; }
    public ICommand ShowPreferencesWindowCommand { get; }
    public ICommand ImportJournalHistoryCommand { get; }
    public ICommand EnableDisableHudWindowMousePassThroughCommand { get; }
    public ICommand LockUnlockRouteCommand { get; }
    public ICommand ImportSpanshRouteCommand { get; }

    public ObservableCollection<TabViewModel> TabViewModels { get; set; }

    public int SelectedTabIndex
    {
        get => selectedTabIndex;
        set
        {
            int valid = Math.Max(0, Math.Min(value, TabViewModels.Count - 1));
            if (valid != selectedTabIndex)
            {
                selectedTabIndex = valid;
                SelectedTabIndexChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }
    }

    public StarSystemViewModel CurrentSystemViewModel { get; private set; }
    public bool CurrentSystemAvailable => CurrentSystemViewModel.Name != string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public string SystemName => CurrentSystemViewModel.Name;
    public string ExplorationStatus => CurrentSystemViewModel.ExplorationStatus;
    public string BodyExplorationStatus => string.Format(Resources.StatusBodiesExploredOfTotal, CurrentSystemViewModel.ExploredBodies, CurrentSystemViewModel.TotalBodies);
    public string NonBodyExplorationStatus => string.Format(Resources.StatusNonBodyBelts, CurrentSystemViewModel.TotalNonBodyCount, CurrentSystemViewModel.ExploredNonBodies);
    public string Title => Resources.MainWindow_Title;
    public bool DataIsLoading
    {
        get
        {
            if (!_webApiProvider.isLoading)
                return _starSystemProvider.RouteIsLoading;
            return true;
        }
    }

    public string GenerateClearPlotterRouteCommandMenuItemString =>
        _routeProvider.IsCustomRoute ? Resources.MenuItem_ClearGalaxyPlotterRoute : Resources.MenuItem_GenerateGalaxyPlotterRoute;

    public string LockUnlockRouteCommandMenuItemString =>
        _routeProvider.IsLocked ? Resources.MenuItem_UnlockRoute : Resources.MenuItem_LockRoute;

    public bool IsRouteAvailable => _routeProvider.IsCustomRoute;

    public string OpenCloseHudWindowCommandMenuItemString =>
        !hudViewModel.HudWindowOpen ? Resources.MenuItem_OpenHudWindow : Resources.MenuItem_CloseHudWindow;

    public string EnableDisableHudWindowMousePassThroughCommandMenuItemString =>
        !HudWindowMousePassThroughEnabled
            ? Resources.MenuItem_EnableHudMousePassThrough
            : Resources.MenuItem_DisableHudMousePassThrough;

    public bool HudWindowMousePassThroughEnabled => hudViewModel.HudWindowMousePassThroughEnabled;

    public BodyViewModel? CurrentPlanet => _starSystemProvider.CurrentPlanet != null ? new BodyViewModel(_starSystemProvider.CurrentPlanet) : null;
    public bool CurrentPlanetAvailable => _starSystemProvider.CurrentPlanet != null;

    public Dictionary<string, HotkeyViewModel> Hotkeys => _hotkeyProvider.Hotkeys;

    public bool IsSurroundingsTabSelected
    {
        get
        {
            if (selectedTabIndex < 0 || selectedTabIndex >= TabViewModels.Count)
                return false;
            return TabViewModels[selectedTabIndex].GetType() == typeof(SurroundingsTableViewModel);
        }
    }

    public event EventHandler SelectedTabIndexChanged = delegate { };
    public event EventHandler GuiHudDataUpdated = delegate { };

    public MainViewModel(
        HotkeyProvider hotkeyProvider,
        WebApiProvider webApiProvider,
        StarSystemProvider starSystemProvider,
        HistoryProvider historyProvider,
        RouteProvider routeProvider,
        StatusProvider statusProvider,
        PlanetsOfInterestProvider planetsOfInterestProvider,
        JournalHistoryImporter journalHistoryImporter)
    {
        _starSystemProvider = starSystemProvider;
        _routeProvider = routeProvider;
        _webApiProvider = webApiProvider;
        _statusProvider = statusProvider;
        _hotkeyProvider = hotkeyProvider;

        _routePlotterViewModel = new RoutePlotterViewModel(this, _starSystemProvider, _routeProvider, _webApiProvider);

        CurrentSystemViewModel = new StarSystemViewModel(_starSystemProvider.CurrentSystem);

        TabViewModels = new ObservableCollection<TabViewModel>
        {
            new NavRouteTableViewModel(Resources.TabHeader_Route, "Visible", _starSystemProvider, _routeProvider),
            new BodyTableViewModel(Resources.TabHeader_Bodies, "Visible", _starSystemProvider),
            new GenusTableViewModel(Resources.TabHeader_Biologicals, "Collapsed", _starSystemProvider),
            new SurroundingsTableViewModel(Resources.TabHeader_Surroundings, "Visible", _starSystemProvider),
            new HistoryViewModel(Resources.TabHeader_History, "Visible", historyProvider)
        };

        hudViewModel = new HudViewModel(this, _statusProvider);
        preferencesViewModel = new PreferencesViewModel(hudViewModel, planetsOfInterestProvider, _starSystemProvider, hotkeyProvider);
        aboutViewModel = new AboutViewModel(this);
        feedbackReportIssueViewModel = new FeedbackReportIssueViewModel();

        ReloadEdsmDataCommand = new LoadEdsmSystemDataCommand(this, _starSystemProvider);
        ShowAboutWindowCommand = new ShowAboutWindowCommand(aboutViewModel);
        OpenCloseHudWindowCommand = new OpenCloseHudWindowCommand(hudViewModel);
        ClearHistoryCommand = new ClearHistoryCommand(this, historyProvider, _starSystemProvider);
        GenerateClearPlotterRouteCommand = new GenerateClearPlotterRouteCommand(_routePlotterViewModel, _routeProvider, RefreshMenuItems);
        CopySystemNameToClipboardCommand = new CopyToClipboardCommand();
        ShowFeedbackReportIssueWindowCommand = new ShowFeedbackReportIssueWindowCommand(feedbackReportIssueViewModel);
        ShowPreferencesWindowCommand = new ShowPreferencesWindowCommand(preferencesViewModel);
        importJournalHistoryViewModel = new JournalHistoryImportViewModel(journalHistoryImporter);
        ImportJournalHistoryCommand = new ImportJournalHistoryCommand(importJournalHistoryViewModel);
        EnableDisableHudWindowMousePassThroughCommand = new EnableDisableHudWindowMousePassThroughCommand(hudViewModel);
        LockUnlockRouteCommand = new LockUnlockRouteCommand(this, _routeProvider);
        ImportSpanshRouteCommand = new ImportSpanshRouteCommand(this, _routeProvider);

        hudViewModel.PropertyChanged += hudViewModel_PropertyChanged;
        _hotkeyProvider.PropertyChanged += _hotkeyProvider_PropertyChanged;
        SelectedTabIndexChanged += _starSystemProvider.MainViewModel_SelectedTabIndexChanged;

        _starSystemProvider.GuiDataUpdated += delegate
        {
            Application.Current?.Dispatcher.Invoke(UpdateDataView);
        };

        _starSystemProvider.RouteLoadingStatusChanged += delegate
        {
            Application.Current?.Dispatcher.Invoke(() => OnPropertyChanged(nameof(DataIsLoading)));
        };

        _webApiProvider.WebApiLoadingStatusChanged += (s, p) =>
        {
            Application.Current?.Dispatcher.Invoke(() => OnPropertyChanged(nameof(DataIsLoading)));
        };

        if (!Preferences.Other.AutomaticTabSwitching)
        {
            SelectedTabIndex = Preferences.Application.SelectedTabIndex;
            OnPropertyChanged(nameof(SelectedTabIndex));
        }

        UpdateDataView();
    }

    public void RestoreWindows()
    {
        try
        {
            SelectedTabIndex = Math.Min(Math.Max(Preferences.Application.SelectedTabIndex, 0), TabViewModels.Count - 1);
            OnPropertyChanged(nameof(SelectedTabIndex));

            if (Preferences.Application.HudWindowOpenOnShutdown)
                OpenCloseHudWindowCommand.Execute(null);
            if (Preferences.Application.AboutWindowOpenOnShutdown)
                ShowAboutWindowCommand.Execute(null);
            if (Preferences.Application.FeedbackReportIssueWindowOpenOnShutdown)
                ShowFeedbackReportIssueWindowCommand.Execute(null);
            if (Preferences.Application.PreferencesWindowOpenOnShutdown)
                ShowPreferencesWindowCommand.Execute(null);
            if (Preferences.Application.JournalHistoryImportWindowOpenOnShutdown)
                ImportJournalHistoryCommand.Execute(null);
            if (Preferences.Application.RoutePlotterWindowOpenOnShutdown)
                _routePlotterViewModel.ShowRoutePlotterWindow();
        }
        catch (Exception exception)
        {
            log.Error("Could not read user settings", exception);
        }
    }

    public void OnMainWindowClosed(object? sender, EventArgs e)
    {
        Preferences.Application.HudWindowOpenOnShutdown = hudViewModel.HudWindowOpen;
        Preferences.Application.AboutWindowOpenOnShutdown = aboutViewModel.AboutWindowOpen;
        Preferences.Application.FeedbackReportIssueWindowOpenOnShutdown = feedbackReportIssueViewModel.FeedbackReportIssueWindowOpen;
        Preferences.Application.PreferencesWindowOpenOnShutdown = preferencesViewModel.PreferencesWindowOpen;
        Preferences.Application.JournalHistoryImportWindowOpenOnShutdown = importJournalHistoryViewModel.JournalHistoryImportWindowOpen;
        Preferences.Application.RoutePlotterWindowOpenOnShutdown = _routePlotterViewModel.RoutePlotterWindowOpen;
        Preferences.Application.SelectedTabIndex = SelectedTabIndex;
        Preferences.Application.HudWindowMousePassThroughEnabled = hudViewModel.HudWindowMousePassThroughEnabled;
        Preferences.SaveUserSettings();
        Application.Current?.Shutdown();
    }

    private void hudViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HudViewModel.HudWindowMousePassThroughEnabled))
        {
            OnPropertyChanged(nameof(EnableDisableHudWindowMousePassThroughCommandMenuItemString));
            OnPropertyChanged(nameof(HudWindowMousePassThroughEnabled));
        }
        else if (e.PropertyName == nameof(HudViewModel.HudWindowOpen))
        {
            OnPropertyChanged(nameof(OpenCloseHudWindowCommandMenuItemString));
        }
    }

    private void _hotkeyProvider_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HotkeyProvider.Hotkeys))
        {
            OnPropertyChanged(nameof(Hotkeys));
        }
    }

    public void RefreshMenuItems()
    {
        OnPropertyChanged(nameof(GenerateClearPlotterRouteCommandMenuItemString));
        OnPropertyChanged(nameof(LockUnlockRouteCommandMenuItemString));
        OnPropertyChanged(nameof(IsRouteAvailable));
    }

    public void OpenTabOfType(Type type, bool forceOpen)
    {
        if (forceOpen || (lastActivity != _starSystemProvider.CurrentActivity && Preferences.Other.AutomaticTabSwitching))
        {
            var match = TabViewModels.FirstOrDefault(x => x.GetType() == type);
            int tabIndex = match is null ? -1 : TabViewModels.IndexOf(match);
            if (tabIndex >= 0 && tabIndex != selectedTabIndex)
            {
                selectedTabIndex = tabIndex;
                SelectedTabIndexChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }
    }

    public void UpdateDataView()
    {
        log.Debug("EDEA4711: Refresh of MainView");
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
                CurrentStatus = _routeProvider.IsCustomRoute
                    ? Resources.CurrentStatus_UsingPlotterRoute
                    : Resources.CurrentStatus_PlanningRoute;
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

        lastActivity = _starSystemProvider.CurrentActivity;
        CurrentSystemViewModel = new StarSystemViewModel(_starSystemProvider.CurrentSystem);

        OnPropertyChanged(nameof(CurrentSystemViewModel));
        OnPropertyChanged(nameof(CurrentStatus));
        OnPropertyChanged(nameof(SystemName));
        OnPropertyChanged(nameof(ExplorationStatus));
        OnPropertyChanged(nameof(BodyExplorationStatus));
        OnPropertyChanged(nameof(NonBodyExplorationStatus));
        OnPropertyChanged(nameof(CurrentSystemAvailable));
        OnPropertyChanged(nameof(GenerateClearPlotterRouteCommandMenuItemString));
        OnPropertyChanged(nameof(LockUnlockRouteCommandMenuItemString));
        OnPropertyChanged(nameof(IsRouteAvailable));
        OnPropertyChanged(nameof(CurrentPlanetAvailable));
        OnPropertyChanged(nameof(CurrentPlanet));
        GuiHudDataUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void SelectTabByName(string name)
    {
        TabViewModel? match = name switch
        {
            "Route" => TabViewModels.FirstOrDefault(t => t is NavRouteTableViewModel),
            "Bodies" => TabViewModels.FirstOrDefault(t => t is BodyTableViewModel),
            "History" => TabViewModels.FirstOrDefault(t => t is HistoryViewModel),
            _ => TabViewModels.FirstOrDefault(t => t.TabName == name || t.TabHeader == name)
        };
        int index = match is null ? -1 : TabViewModels.IndexOf(match);
        SelectedTabIndex = index;
    }

    public void RegisterHotkeys(HotkeyProvider provider)
    {
        provider.AssignAllHotkeys();
    }
}
