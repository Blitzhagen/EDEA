using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using log4net;

namespace EDEA.ViewModels;

/// <summary>
/// View model that manages the history tab and its data presentation.
/// </summary>
public class HistoryViewModel : TabViewModel
{
    /// <summary>
    /// Gets the name of the tab.
    /// </summary>
    /// <value>The tab name displayed in the UI.</value>
    public override string TabName => "History";

    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(HistoryViewModel));

    /// <summary>
    /// The provider for history data.
    /// </summary>
    private readonly HistoryProvider _historyProvider;

    /// <summary>
    /// Gets or sets the command that copies the history data to the clipboard.
    /// </summary>
    /// <value>The copy command.</value>
    public ICommand? CopyHistoryDataToClipboardCommand { get; set; }

    /// <summary>
    /// Gets or sets the formatted history data.
    /// </summary>
    /// <value>The history data view model.</value>
    public HistoryDataViewModel HistoryData { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryViewModel"/> class.
    /// </summary>
    /// <param name="tabHeader">The header text for the tab.</param>
    /// <param name="tabVisibility">The initial visibility of the tab.</param>
    /// <param name="historyProvider">The provider for history data.</param>
    public HistoryViewModel(string tabHeader, string tabVisibility, HistoryProvider historyProvider)
        : base(tabHeader, tabVisibility)
    {
        _historyProvider = historyProvider;
        HistoryData = new HistoryDataViewModel(new HistoryData());
        getHistoryData().ContinueWith(delegate
        {
            _historyProvider.HistoryUpdated += _historyProvider_HistoryUpdated;
            Resources.CultureChanged += () => _ = getHistoryData();
        });
    }

    /// <summary>
    /// Handles the <see cref="HistoryProvider.HistoryUpdated"/> event by refreshing the history data.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void _historyProvider_HistoryUpdated(object? sender, EventArgs e)
    {
        Task _ = getHistoryData();
    }

    /// <summary>
    /// Loads the history data asynchronously and updates the view.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task getHistoryData()
    {
        HistoryData data = await _historyProvider.GetHistoryData();
        PlatformServices.Dispatcher?.Invoke(delegate
        {
            HistoryData = new HistoryDataViewModel(data);
            updateView();
        });
    }

    /// <summary>
    /// Notifies the view that the history data has changed.
    /// </summary>
    private void updateView()
    {
        OnPropertyChanged("HistoryData");
    }
}
