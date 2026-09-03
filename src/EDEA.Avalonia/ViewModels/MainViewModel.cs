using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EDEA.Avalonia.Windows;
using EDEA.Models;
using EDEA.Properties;
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
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel()
    {
        TabViewModels = new ObservableCollection<TabViewModel>
        {
            new TabViewModel(Resources.TabHeader_Route, "Visible"),
            new TabViewModel(Resources.TabHeader_Bodies, "Visible"),
            new TabViewModel(Resources.TabHeader_Biologicals, "Visible"),
            new TabViewModel(Resources.TabHeader_Surroundings, "Visible"),
            new TabViewModel(Resources.TabHeader_History, "Visible"),
        };

        ShowAboutWindowCommand = new RelayCommand(ShowAboutWindow);
        ShowPreferencesWindowCommand = new RelayCommand(ShowPreferencesWindow);
        ShowFeedbackReportIssueWindowCommand = new RelayCommand(ShowFeedbackReportIssueWindow);
        OpenCloseHudWindowCommand = new RelayCommand(OpenCloseHudWindow);
        ImportJournalHistoryCommand = new RelayCommand(ImportJournalHistory);
    }

    /// <summary>
    /// Gets the owner window for dialogs.
    /// </summary>
    /// <returns>The current main window, or <see langword="null"/>.</returns>
    private static global::Avalonia.Controls.Window? GetOwnerWindow()
    {
        if (global::Avalonia.Application.Current?.ApplicationLifetime is global::Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }

    private void ShowAboutWindow()
    {
        var window = new AboutWindow();
        window.Show();
    }

    private void ShowPreferencesWindow()
    {
        var window = new PreferencesWindow();
        window.Show();
    }

    private void ShowFeedbackReportIssueWindow()
    {
        var window = new FeedbackReportIssueWindow();
        window.Show();
    }

    private void OpenCloseHudWindow()
    {
        var window = new HudWindow();
        window.Show();
    }

    private void ImportJournalHistory()
    {
        var window = new JournalHistoryImportWindow();
        window.Show();
    }
}
