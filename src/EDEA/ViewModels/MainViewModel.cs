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

/// <summary>
/// Coordinates the main view, commands, tab switching and provider integration for the EDEA application.
/// </summary>
public class MainViewModel : ObservableObject
{
    /// <summary>The log4net logger used by this view model.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));

    /// <summary>Index of the currently selected tab.</summary>
    private int selectedTabIndex;
    /// <summary>View model for the HUD window.</summary>
    private HudViewModel hudViewModel;
    /// <summary>View model for the about window.</summary>
    private AboutViewModel aboutViewModel;
    /// <summary>View model for the feedback and report issue window.</summary>
    private FeedbackReportIssueViewModel feedbackReportIssueViewModel;
    /// <summary>View model for the preferences window.</summary>
    private PreferencesViewModel preferencesViewModel;
    /// <summary>View model for the journal history import.</summary>
    private JournalHistoryImportViewModel importJournalHistoryViewModel;
    /// <summary>The last processed activity, used to avoid redundant tab switches.</summary>
    private Activity lastActivity = Activity.Other;

    /// <summary>Provides star system data.</summary>
    private readonly StarSystemProvider _starSystemProvider;
    /// <summary>Provides route data.</summary>
    private readonly RouteProvider _routeProvider;
    /// <summary>Provides web API access.</summary>
    private readonly WebApiProvider _webApiProvider;
    /// <summary>Provides status information.</summary>
    private readonly StatusProvider _statusProvider;
    /// <summary>Provides hotkey definitions.</summary>
    private readonly HotkeyProvider _hotkeyProvider;

    /// <summary>View model for the route plotter window.</summary>
    private readonly RoutePlotterViewModel _routePlotterViewModel;

    /// <summary>Reloads EDSM data for the current star system.</summary>
    /// <value>The command that triggers an EDSM data reload.</value>
    public ICommand ReloadEdsmDataCommand { get; }
    /// <summary>Displays the about window.</summary>
    /// <value>The command that opens the about window.</value>
    public ICommand ShowAboutWindowCommand { get; }
    /// <summary>Opens or closes the HUD window.</summary>
    /// <value>The command that toggles the HUD window.</value>
    public ICommand OpenCloseHudWindowCommand { get; }
    /// <summary>Opens the feedback and report issue window.</summary>
    /// <value>The command that opens the feedback window.</value>
    public ICommand ShowFeedbackReportIssueWindowCommand { get; }
    /// <summary>Clears the commander history.</summary>
    /// <value>The command that clears the history.</value>
    public ICommand ClearHistoryCommand { get; }
    /// <summary>Generates or clears the galaxy plotter route.</summary>
    /// <value>The command that toggles the galaxy plotter route.</value>
    public ICommand GenerateClearPlotterRouteCommand { get; }
    /// <summary>Copies the current system name to the clipboard.</summary>
    /// <value>The command that copies the system name.</value>
    public ICommand CopySystemNameToClipboardCommand { get; }
    /// <summary>Opens the preferences window.</summary>
    /// <value>The command that opens the preferences window.</value>
    public ICommand ShowPreferencesWindowCommand { get; }
    /// <summary>Imports journal history data.</summary>
    /// <value>The command that starts a journal history import.</value>
    public ICommand ImportJournalHistoryCommand { get; }
    /// <summary>Enables or disables mouse pass-through for the HUD window.</summary>
    /// <value>The command that toggles the HUD mouse pass-through.</value>
    public ICommand EnableDisableHudWindowMousePassThroughCommand { get; }
    /// <summary>Locks or unlocks the current route.</summary>
    /// <value>The command that toggles the route lock.</value>
    public ICommand LockUnlockRouteCommand { get; }
    /// <summary>Imports a route from Spansh.</summary>
    /// <value>The command that starts a Spansh route import.</value>
    public ICommand ImportSpanshRouteCommand { get; }

    /// <summary>Collection of tab view models displayed in the main view.</summary>
    /// <value>The collection of tab view models.</value>
    public ObservableCollection<TabViewModel> TabViewModels { get; set; }

    /// <summary>Gets or sets the index of the currently selected tab.</summary>
    /// <value>The zero-based index of the selected tab.</value>
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

    /// <summary>Gets or sets the view model for the current star system.</summary>
    /// <value>The view model of the current star system.</value>
    public StarSystemViewModel CurrentSystemViewModel { get; private set; }
    /// <summary>Gets a value indicating whether a current system is available.</summary>
    /// <value>true if a current system is available; otherwise, false.</value>
    public bool CurrentSystemAvailable => CurrentSystemViewModel.Name != string.Empty;
    /// <summary>Gets or sets the current application status text.</summary>
    /// <value>The current application status text.</value>
    public string CurrentStatus { get; set; } = string.Empty;
    /// <summary>Gets the name of the current star system.</summary>
    /// <value>The name of the current star system.</value>
    public string SystemName => CurrentSystemViewModel.Name;
    /// <summary>Gets the exploration status of the current system.</summary>
    /// <value>The exploration status text.</value>
    public string ExplorationStatus => CurrentSystemViewModel.ExplorationStatus;
    /// <summary>Gets the formatted body exploration status.</summary>
    /// <value>The formatted body exploration status text.</value>
    public string BodyExplorationStatus => string.Format(Resources.StatusBodiesExploredOfTotal, CurrentSystemViewModel.ExploredBodies, CurrentSystemViewModel.TotalBodies);
    /// <summary>Gets the formatted non-body exploration status.</summary>
    /// <value>The formatted non-body exploration status text.</value>
    public string NonBodyExplorationStatus => string.Format(Resources.StatusNonBodyBelts, CurrentSystemViewModel.TotalNonBodyCount, CurrentSystemViewModel.ExploredNonBodies);
    /// <summary>Gets the localized title of the main window.</summary>
    /// <value>The localized title of the main window.</value>
    public string Title => Resources.MainWindow_Title;
    /// <summary>Gets a value indicating whether data is currently loading.</summary>
    /// <value>true if data is loading; otherwise, false.</value>
    public bool DataIsLoading
    {
        get
        {
            if (!_webApiProvider.isLoading)
                return _starSystemProvider.RouteIsLoading;
            return true;
        }
    }

    /// <summary>Gets the localized menu text for the generate or clear plotter route command.</summary>
    /// <value>The localized menu text for the generate or clear plotter route command.</value>
    public string GenerateClearPlotterRouteCommandMenuItemString =>
        _routeProvider.IsCustomRoute ? Resources.MenuItem_ClearGalaxyPlotterRoute : Resources.MenuItem_GenerateGalaxyPlotterRoute;

    /// <summary>Gets the localized menu text for the lock or unlock route command.</summary>
    /// <value>The localized menu text for the lock or unlock route command.</value>
    public string LockUnlockRouteCommandMenuItemString =>
        _routeProvider.IsLocked ? Resources.MenuItem_UnlockRoute : Resources.MenuItem_LockRoute;

    /// <summary>Gets a value indicating whether a custom route is active.</summary>
    /// <value>true if a custom route is active; otherwise, false.</value>
    public bool IsRouteAvailable => _routeProvider.IsCustomRoute;

    /// <summary>Gets the localized menu text for opening or closing the HUD window.</summary>
    /// <value>The localized menu text for opening or closing the HUD window.</value>
    public string OpenCloseHudWindowCommandMenuItemString =>
        !hudViewModel.HudWindowOpen ? Resources.MenuItem_OpenHudWindow : Resources.MenuItem_CloseHudWindow;

    /// <summary>Gets the localized menu text for enabling or disabling HUD mouse pass-through.</summary>
    /// <value>The localized menu text for enabling or disabling HUD mouse pass-through.</value>
    public string EnableDisableHudWindowMousePassThroughCommandMenuItemString =>
        !HudWindowMousePassThroughEnabled
            ? Resources.MenuItem_EnableHudMousePassThrough
            : Resources.MenuItem_DisableHudMousePassThrough;

    /// <summary>Gets a value indicating whether the HUD window ignores mouse input.</summary>
    /// <value>true if the HUD window ignores mouse input; otherwise, false.</value>
    public bool HudWindowMousePassThroughEnabled => hudViewModel.HudWindowMousePassThroughEnabled;

    /// <summary>Gets the view model for the currently selected planet, if any.</summary>
    /// <value>The view model of the currently selected planet, or null if none is selected.</value>
    public BodyViewModel? CurrentPlanet => _starSystemProvider.CurrentPlanet != null ? new BodyViewModel(_starSystemProvider.CurrentPlanet) : null;
    /// <summary>Gets a value indicating whether a current planet is selected.</summary>
    /// <value>true if a current planet is selected; otherwise, false.</value>
    public bool CurrentPlanetAvailable => _starSystemProvider.CurrentPlanet != null;

    /// <summary>Gets the configured hotkeys.</summary>
    /// <value>A dictionary of hotkey view models grouped by name.</value>
    public Dictionary<string, HotkeyViewModel> Hotkeys => _hotkeyProvider.Hotkeys;

    /// <summary>Gets a value indicating whether the surroundings tab is selected.</summary>
    /// <value>true if the surroundings tab is selected; otherwise, false.</value>
    public bool IsSurroundingsTabSelected
    {
        get
        {
            if (selectedTabIndex < 0 || selectedTabIndex >= TabViewModels.Count)
                return false;
            return TabViewModels[selectedTabIndex].GetType() == typeof(SurroundingsTableViewModel);
        }
    }

    /// <summary>Occurs when the selected tab index has changed.</summary>
    public event EventHandler SelectedTabIndexChanged = delegate { };
    /// <summary>Occurs when the GUI/HUD data has been updated.</summary>
    public event EventHandler GuiHudDataUpdated = delegate { };

    /// <summary>
    /// Initializes a new instance of the MainViewModel class.
    /// </summary>
    /// <param name="hotkeyProvider">The provider of hotkey definitions.</param>
    /// <param name="webApiProvider">The provider of web API services.</param>
    /// <param name="starSystemProvider">The provider of star system data.</param>
    /// <param name="historyProvider">The provider of commander history data.</param>
    /// <param name="routeProvider">The provider of route data.</param>
    /// <param name="statusProvider">The provider of status information.</param>
    /// <param name="planetsOfInterestProvider">The provider of planets of interest data.</param>
    /// <param name="journalHistoryImporter">The importer for journal history.</param>
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

        var navRouteTableViewModel = new NavRouteTableViewModel(Resources.TabHeader_Route, "Visible", _starSystemProvider, _routeProvider);
        var bodyTableViewModel = new BodyTableViewModel(Resources.TabHeader_Bodies, "Visible", _starSystemProvider);
        var genusTableViewModel = new GenusTableViewModel(Resources.TabHeader_Biologicals, "Collapsed", _starSystemProvider);
        var surroundingsTableViewModel = new SurroundingsTableViewModel(Resources.TabHeader_Surroundings, "Visible", _starSystemProvider);
        var historyViewModel = new HistoryViewModel(Resources.TabHeader_History, "Visible", historyProvider);

        bodyTableViewModel.CopyBodyNameToClipboardCommand = new CopyToClipboardCommand();
        surroundingsTableViewModel.CopySystemNameToClipboardCommand = new CopyToClipboardCommand();
        historyViewModel.CopyHistoryDataToClipboardCommand = new CopyToClipboardCommand();

        TabViewModels = new ObservableCollection<TabViewModel>
        {
            navRouteTableViewModel,
            bodyTableViewModel,
            genusTableViewModel,
            surroundingsTableViewModel,
            historyViewModel
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
        SelectedTabIndexChanged += (sender, e) => _starSystemProvider.OnSurroundingsTabSelected(IsSurroundingsTabSelected);

        _starSystemProvider.GuiDataUpdated += delegate
        {
            PlatformServices.Dispatcher?.Invoke(UpdateDataView);
        };

        _starSystemProvider.RouteLoadingStatusChanged += delegate
        {
            PlatformServices.Dispatcher?.Invoke(() => OnPropertyChanged(nameof(DataIsLoading)));
        };

        _webApiProvider.WebApiLoadingStatusChanged += (s, p) =>
        {
            PlatformServices.Dispatcher?.Invoke(() => OnPropertyChanged(nameof(DataIsLoading)));
        };

        if (!Preferences.Other.AutomaticTabSwitching)
        {
            SelectedTabIndex = Preferences.Application.SelectedTabIndex;
            OnPropertyChanged(nameof(SelectedTabIndex));
        }

        UpdateDataView();
    }

    /// <summary>Restores previously open windows and the selected tab on startup.</summary>
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

    /// <summary>Saves window states and shuts down the application.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
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

    /// <summary>Handles property changes on the HUD view model.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The property changed event data.</param>
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

    /// <summary>Handles property changes on the hotkey provider.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The property changed event data.</param>
    private void _hotkeyProvider_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HotkeyProvider.Hotkeys))
        {
            OnPropertyChanged(nameof(Hotkeys));
        }
    }

    /// <summary>Refreshes the menu item texts related to route and plotter commands.</summary>
    public void RefreshMenuItems()
    {
        OnPropertyChanged(nameof(GenerateClearPlotterRouteCommandMenuItemString));
        OnPropertyChanged(nameof(LockUnlockRouteCommandMenuItemString));
        OnPropertyChanged(nameof(IsRouteAvailable));
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>Opens the tab that matches the specified view model type.</summary>
    /// <param name="type">The view model type to search for.</param>
    /// <param name="forceOpen">true to force opening the tab regardless of the last activity; otherwise, false.</param>
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

    /// <summary>Updates the current view and tab selection based on the active game state.</summary>
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

    /// <summary>Selects the tab that matches the specified name.</summary>
    /// <param name="name">The name, header or type name of the tab to select.</param>
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

    /// <summary>Registers all hotkeys with the specified provider.</summary>
    /// <param name="provider">The hotkey provider to assign hotkeys to.</param>
    public void RegisterHotkeys(HotkeyProvider provider)
    {
        provider.AssignAllHotkeys();
    }
}
