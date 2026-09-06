using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA;
using EDEA.Properties;
using EDEA.Services;

namespace EDEA.Avalonia.ViewModels;

/// <summary>
/// View model for the Avalonia journal history import window.
/// </summary>
public partial class JournalHistoryImportViewModel : ObservableObject
{
    /// <summary>
    /// The importer that performs the journal history import.
    /// </summary>
    private readonly JournalHistoryImporter? _journalHistoryImporter;

    /// <summary>
    /// Gets the current progress bar value.
    /// </summary>
    /// <value>The progress percentage.</value>
    public int ProgressBarValue => _journalHistoryImporter?.StatusPercentage ?? 0;

    /// <summary>
    /// Gets the formatted import progress percentage.
    /// </summary>
    /// <value>The progress percentage string.</value>
    public string StatusPercentage => $"{_journalHistoryImporter?.StatusPercentage ?? 0} {Resources.UnitPercent}";

    /// <summary>
    /// Gets the number of processed journal files.
    /// </summary>
    /// <value>The processed journal files string.</value>
    public string JournalFilesProcessed => $"{_journalHistoryImporter?.StatusData.JournalFilesProcessed ?? 0} {((_journalHistoryImporter?.StatusData.JournalFilesProcessed ?? 0) == 1 ? Resources.UnitFileSingular : Resources.UnitFilePlural)}";

    /// <summary>
    /// Gets the systems scan progress.
    /// </summary>
    /// <value>The systems scan progress string.</value>
    public string SystemsScanProgress => $"{_journalHistoryImporter?.StatusData.SystemsScanProgress ?? 0} {Resources.UnitPercent}";

    /// <summary>
    /// Gets the bodies scan progress.
    /// </summary>
    /// <value>The bodies scan progress string.</value>
    public string BodiesScanProgress => $"{_journalHistoryImporter?.StatusData.BodiesScanProgress ?? 0} {Resources.UnitPercent}";

    /// <summary>
    /// Gets the additional data scan progress.
    /// </summary>
    /// <value>The additional data scan progress string.</value>
    public string AdditionalDataScanProgress => $"{_journalHistoryImporter?.StatusData.AdditionalDataScanProgress ?? 0} {Resources.UnitPercent}";

    /// <summary>
    /// Gets the number of systems added to history.
    /// </summary>
    /// <value>The added systems count string.</value>
    public string AddedToHistoryCount => $"{_journalHistoryImporter?.StatusData.AddedToHistoryCount ?? 0} {((_journalHistoryImporter?.StatusData.AddedToHistoryCount ?? 0) == 1 ? Resources.UnitSystemSingular : Resources.UnitSystemPlural)}";

    /// <summary>
    /// Gets the number of systems updated in history.
    /// </summary>
    /// <value>The updated systems count string.</value>
    public string UpdatedInHistoryCount => $"{_journalHistoryImporter?.StatusData.UpdatedInHistoryCount ?? 0} {((_journalHistoryImporter?.StatusData.UpdatedInHistoryCount ?? 0) == 1 ? Resources.UnitSystemSingular : Resources.UnitSystemPlural)}";

    /// <summary>
    /// Gets the number of systems ignored during the import.
    /// </summary>
    /// <value>The ignored systems count string.</value>
    public string IgnoredSystemsCount => $"{_journalHistoryImporter?.StatusData.IgnoredSystemsCount ?? 0} {((_journalHistoryImporter?.StatusData.IgnoredSystemsCount ?? 0) == 1 ? Resources.UnitSystemSingular : Resources.UnitSystemPlural)}";

    /// <summary>
    /// Gets or sets the introduction text shown in the import window.
    /// </summary>
    /// <value>The introduction text.</value>
    [ObservableProperty]
    private string _introText = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the start button is visible.
    /// </summary>
    [ObservableProperty]
    private bool _isStartVisible;

    /// <summary>
    /// Gets or sets a value indicating whether the stop/close button is visible.
    /// </summary>
    [ObservableProperty]
    private bool _isStopCloseVisible;

    /// <summary>
    /// Gets or sets a value indicating whether the import has finished.
    /// </summary>
    [ObservableProperty]
    private bool _isImportCompleted;

    /// <summary>
    /// Gets or sets the content of the stop/close button.
    /// </summary>
    [ObservableProperty]
    private string _stopCloseButtonContent = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="JournalHistoryImportViewModel"/> class.
    /// </summary>
    /// <param name="journalHistoryImporter">The importer for journal history data.</param>
    public JournalHistoryImportViewModel(JournalHistoryImporter? journalHistoryImporter)
    {
        _journalHistoryImporter = journalHistoryImporter;
        if (_journalHistoryImporter != null)
        {
            _journalHistoryImporter.JournalHistoryImportProgressChanged += OnJournalHistoryImportProgressChanged;
            _journalHistoryImporter.JournalHistoryImportFinished += OnJournalHistoryImportFinished;
        }
    }

    /// <summary>
    /// Reads the journal files and prepares the import window.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_journalHistoryImporter == null)
        {
            IntroText = Resources.JournalHistoryImport_IntroText_None;
            IsStartVisible = false;
            IsStopCloseVisible = true;
            StopCloseButtonContent = Resources.Button_Close;
            return;
        }

        int journalFileCount = _journalHistoryImporter.ReadJournalFiles();
        if (journalFileCount > 0)
        {
            IntroText = string.Format(Resources.JournalHistoryImport_IntroText_Found, journalFileCount);
            IsStartVisible = true;
            IsStopCloseVisible = false;
            StopCloseButtonContent = Resources.JournalHistoryImportWindow_StopImport;
        }
        else
        {
            IntroText = Resources.JournalHistoryImport_IntroText_None;
            IsStartVisible = false;
            IsStopCloseVisible = true;
            StopCloseButtonContent = Resources.Button_Close;
        }
        OnPropertyChanged(string.Empty);
    }

    /// <summary>
    /// Starts the journal history import.
    /// </summary>
    public void StartImport()
    {
        IsStartVisible = false;
        IsStopCloseVisible = true;
        IsImportCompleted = false;
        StopCloseButtonContent = Resources.JournalHistoryImportWindow_StopImport;
        _journalHistoryImporter?.StartJournalImport();
    }

    /// <summary>
    /// Cancels the journal history import.
    /// </summary>
    public void CancelImport()
    {
        _journalHistoryImporter?.CancelJournalImport();
    }

    /// <summary>
    /// Refreshes all properties when the import progress changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnJournalHistoryImportProgressChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(string.Empty);
    }

    /// <summary>
    /// Marks the import as completed and changes the stop button to a close button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnJournalHistoryImportFinished(object? sender, EventArgs e)
    {
        PlatformServices.Dispatcher?.Invoke(delegate
        {
            IsImportCompleted = true;
            StopCloseButtonContent = Resources.Button_Close;
        });
    }
}