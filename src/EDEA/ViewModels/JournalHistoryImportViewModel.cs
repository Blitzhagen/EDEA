using System;
using System.Windows;
using System.Windows.Controls;
using EDEA.Properties;
using EDEA.Services;
using EDEA.Windows;
using log4net;

namespace EDEA.ViewModels;

/// <summary>
/// View model that controls the journal history import window and progress.
/// </summary>
public class JournalHistoryImportViewModel : ViewModelBase
{
    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(JournalHistoryImportViewModel));

    /// <summary>
    /// Holds the singleton instance of the journal history import window.
    /// </summary>
    private static JournalHistoryImportWindow? journalHistoryImportWindow;

    /// <summary>
    /// The importer that performs the journal history import.
    /// </summary>
    private readonly JournalHistoryImporter _journalHistoryImporter;

    /// <summary>
    /// The start import button.
    /// </summary>
    private Button? startImportButton;

    /// <summary>
    /// The stop/close import button.
    /// </summary>
    private Button? stopImportCloseButton;

    /// <summary>
    /// Gets the current progress bar value.
    /// </summary>
    /// <value>The progress percentage.</value>
    public int ProgressBarValue => _journalHistoryImporter.StatusPercentage;

    /// <summary>
    /// Gets the formatted import progress percentage.
    /// </summary>
    /// <value>The progress percentage string.</value>
    public string StatusPercentage => $"{_journalHistoryImporter.StatusPercentage} {Resources.UnitPercent}";

    /// <summary>
    /// Gets the number of processed journal files.
    /// </summary>
    /// <value>The processed journal files string.</value>
    public string JournalFilesProcessed => $"{_journalHistoryImporter.StatusData.JournalFilesProcessed} {((_journalHistoryImporter.StatusData.JournalFilesProcessed == 1) ? Resources.UnitFileSingular : Resources.UnitFilePlural)}";

    /// <summary>
    /// Gets the systems scan progress.
    /// </summary>
    /// <value>The systems scan progress string.</value>
    public string SystemsScanProgress => $"{_journalHistoryImporter.StatusData.SystemsScanProgress} {Resources.UnitPercent}";

    /// <summary>
    /// Gets the bodies scan progress.
    /// </summary>
    /// <value>The bodies scan progress string.</value>
    public string BodiesScanProgress => $"{_journalHistoryImporter.StatusData.BodiesScanProgress} {Resources.UnitPercent}";

    /// <summary>
    /// Gets the additional data scan progress.
    /// </summary>
    /// <value>The additional data scan progress string.</value>
    public string AdditionalDataScanProgress => $"{_journalHistoryImporter.StatusData.AdditionalDataScanProgress} {Resources.UnitPercent}";

    /// <summary>
    /// Gets the number of systems added to history.
    /// </summary>
    /// <value>The added systems count string.</value>
    public string AddedToHistoryCount => $"{_journalHistoryImporter.StatusData.AddedToHistoryCount} {((_journalHistoryImporter.StatusData.AddedToHistoryCount == 1) ? Resources.UnitSystemSingular : Resources.UnitSystemPlural)}";

    /// <summary>
    /// Gets the number of systems updated in history.
    /// </summary>
    /// <value>The updated systems count string.</value>
    public string UpdatedInHistoryCount => $"{_journalHistoryImporter.StatusData.UpdatedInHistoryCount} {((_journalHistoryImporter.StatusData.UpdatedInHistoryCount == 1) ? Resources.UnitSystemSingular : Resources.UnitSystemPlural)}";

    /// <summary>
    /// Gets the number of systems ignored during the import.
    /// </summary>
    /// <value>The ignored systems count string.</value>
    public string IgnoredSystemsCount => $"{_journalHistoryImporter.StatusData.IgnoredSystemsCount} {((_journalHistoryImporter.StatusData.IgnoredSystemsCount == 1) ? Resources.UnitSystemSingular : Resources.UnitSystemPlural)}";

    /// <summary>
    /// Gets or sets the introduction text shown in the import window.
    /// </summary>
    /// <value>The introduction text.</value>
    public string IntroText { get; set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the journal history import window is open.
    /// </summary>
    /// <value><c>true</c> if the import window is open; otherwise, <c>false</c>.</value>
    public bool JournalHistoryImportWindowOpen => journalHistoryImportWindow != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="JournalHistoryImportViewModel"/> class.
    /// </summary>
    /// <param name="journalHistoryImporter">The importer for journal history data.</param>
    public JournalHistoryImportViewModel(JournalHistoryImporter journalHistoryImporter)
    {
        _journalHistoryImporter = journalHistoryImporter;
        _journalHistoryImporter.JournalHistoryImportProgressChanged += _journalHistoryImporter_JournalHistoryImportProgressChanged;
    }

    /// <summary>
    /// Shows the journal history import window or activates it if already open.
    /// </summary>
    public void ShowImportJournalHistoryWindow()
    {
        if (journalHistoryImportWindow == null)
        {
            journalHistoryImportWindow = new JournalHistoryImportWindow();
            JotSettingsProvider.Tracker.Track(journalHistoryImportWindow);
            journalHistoryImportWindow.DataContext = this;
            journalHistoryImportWindow.Closed += journalHistoryImportWindow_Closed;
            journalHistoryImportWindow.ContentRendered += journalHistoryImportWindow_ContentRendered;
            startImportButton = (Button)journalHistoryImportWindow.FindName("StartImportButton");
            if (startImportButton != null)
            {
                startImportButton.Click += startImportButton_Click;
                startImportButton.Visibility = Visibility.Collapsed;
            }
            stopImportCloseButton = (Button)journalHistoryImportWindow.FindName("StopImportCloseButton");
            if (stopImportCloseButton != null)
            {
                stopImportCloseButton.Click += stopImportOkButton_Click;
            }
            journalHistoryImportWindow.Show();
        }
        else
        {
            if (!journalHistoryImportWindow.IsActive)
            {
                journalHistoryImportWindow.Activate();
            }
            if (!journalHistoryImportWindow.IsFocused)
            {
                journalHistoryImportWindow.Focus();
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="JournalHistoryImportWindow.ContentRendered"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void journalHistoryImportWindow_ContentRendered(object? sender, EventArgs e)
    {
        int journalFileCount = _journalHistoryImporter.ReadJournalFiles();
        if (journalFileCount > 0)
        {
            IntroText = string.Format(Resources.JournalHistoryImport_IntroText_Found, journalFileCount);
            if (startImportButton != null)
            {
                startImportButton.Visibility = Visibility.Visible;
            }
            if (stopImportCloseButton != null)
            {
                stopImportCloseButton.Visibility = Visibility.Collapsed;
            }
        }
        else
        {
            IntroText = Resources.JournalHistoryImport_IntroText_None;
            if (stopImportCloseButton != null)
            {
                stopImportCloseButton.Content = Resources.Button_Close;
            }
        }
        refreshGUI();
    }

    /// <summary>
    /// Handles the stop/close button click by cancelling the import and closing the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void stopImportOkButton_Click(object? sender, RoutedEventArgs e)
    {
        _journalHistoryImporter.CancelJournalImport();
        journalHistoryImportWindow?.Close();
    }

    /// <summary>
    /// Handles the start import button click by starting the import.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void startImportButton_Click(object? sender, RoutedEventArgs e)
    {
        if (startImportButton != null)
        {
            startImportButton.Visibility = Visibility.Collapsed;
        }
        if (stopImportCloseButton != null)
        {
            stopImportCloseButton.Visibility = Visibility.Visible;
        }
        _journalHistoryImporter.StartJournalImport();
    }

    /// <summary>
    /// Handles import progress updates.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void _journalHistoryImporter_JournalHistoryImportProgressChanged(object? sender, EventArgs e)
    {
        if (_journalHistoryImporter.StatusPercentage == 100)
        {
            if (stopImportCloseButton != null)
            {
                stopImportCloseButton.Content = Resources.Button_Close;
            }
        }
        refreshGUI();
    }

    /// <summary>
    /// Handles the <see cref="JournalHistoryImportWindow.Closed"/> event and cleans up the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void journalHistoryImportWindow_Closed(object? sender, EventArgs e)
    {
        startImportButton = null;
        stopImportCloseButton = null;
        journalHistoryImportWindow = null;
        _journalHistoryImporter.CancelJournalImport();
    }

    /// <summary>
    /// Refreshes all displayed properties of the import window.
    /// </summary>
    private void refreshGUI()
    {
        OnPropertyChanged("ProgressBarValue");
        OnPropertyChanged("StatusPercentage");
        OnPropertyChanged("JournalFilesProcessed");
        OnPropertyChanged("SystemsScanProgress");
        OnPropertyChanged("BodiesScanProgress");
        OnPropertyChanged("AdditionalDataScanProgress");
        OnPropertyChanged("AddedToHistoryCount");
        OnPropertyChanged("UpdatedInHistoryCount");
        OnPropertyChanged("IgnoredSystemsCount");
        OnPropertyChanged("IntroText");
    }
}
