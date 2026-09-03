using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
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
    /// The collection of tab view models.
    /// </summary>
    public ObservableCollection<TabViewModel> TabViewModels { get; }

    /// <summary>
    /// The star system provider.
    /// </summary>
    private readonly StarSystemProvider _starSystemProvider;

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
    public MainViewModel(StarSystemProvider starSystemProvider, HistoryProvider historyProvider, RouteProvider routeProvider)
    {
        _starSystemProvider = starSystemProvider;

        var navRouteTableViewModel = new NavRouteTableViewModel(Resources.TabHeader_Route, "Visible", starSystemProvider);
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
        new HudWindow(_starSystemProvider).Show();
    }

    private void ImportJournalHistory()
    {
        new JournalHistoryImportWindow().Show();
    }
}
