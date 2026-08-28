using System;
using System.Windows;
using System.Windows.Controls;
using EDEA.Properties;
using EDEA.Services;
using EDEA.Windows;
using log4net;

namespace EDEA.ViewModels;

public class JournalHistoryImportViewModel : ViewModelBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(JournalHistoryImportViewModel));

    private static JournalHistoryImportWindow? journalHistoryImportWindow;

    private readonly JournalHistoryImporter _journalHistoryImporter;

    private Button? startImportButton;

    private Button? stopImportCloseButton;

    public int ProgressBarValue => _journalHistoryImporter.StatusPercentage;

    public string StatusPercentage => $"{_journalHistoryImporter.StatusPercentage} {Resources.UnitPercent}";

    public string JournalFilesProcessed => $"{_journalHistoryImporter.StatusData.JournalFilesProcessed} {((_journalHistoryImporter.StatusData.JournalFilesProcessed == 1) ? Resources.UnitFileSingular : Resources.UnitFilePlural)}";

    public string SystemsScanProgress => $"{_journalHistoryImporter.StatusData.SystemsScanProgress} {Resources.UnitPercent}";

    public string BodiesScanProgress => $"{_journalHistoryImporter.StatusData.BodiesScanProgress} {Resources.UnitPercent}";

    public string AdditionalDataScanProgress => $"{_journalHistoryImporter.StatusData.AdditionalDataScanProgress} {Resources.UnitPercent}";

    public string AddedToHistoryCount => $"{_journalHistoryImporter.StatusData.AddedToHistoryCount} {((_journalHistoryImporter.StatusData.AddedToHistoryCount == 1) ? Resources.UnitSystemSingular : Resources.UnitSystemPlural)}";

    public string UpdatedInHistoryCount => $"{_journalHistoryImporter.StatusData.UpdatedInHistoryCount} {((_journalHistoryImporter.StatusData.UpdatedInHistoryCount == 1) ? Resources.UnitSystemSingular : Resources.UnitSystemPlural)}";

    public string IgnoredSystemsCount => $"{_journalHistoryImporter.StatusData.IgnoredSystemsCount} {((_journalHistoryImporter.StatusData.IgnoredSystemsCount == 1) ? Resources.UnitSystemSingular : Resources.UnitSystemPlural)}";

    public string IntroText { get; set; } = string.Empty;

    public bool JournalHistoryImportWindowOpen => journalHistoryImportWindow != null;

    public JournalHistoryImportViewModel(JournalHistoryImporter journalHistoryImporter)
    {
        _journalHistoryImporter = journalHistoryImporter;
        _journalHistoryImporter.JournalHistoryImportProgressChanged += _journalHistoryImporter_JournalHistoryImportProgressChanged;
    }

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

    private void stopImportOkButton_Click(object? sender, RoutedEventArgs e)
    {
        _journalHistoryImporter.CancelJournalImport();
        journalHistoryImportWindow?.Close();
    }

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

    private void journalHistoryImportWindow_Closed(object? sender, EventArgs e)
    {
        startImportButton = null;
        stopImportCloseButton = null;
        journalHistoryImportWindow = null;
        _journalHistoryImporter.CancelJournalImport();
    }

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
