using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using EDEA.Commands;
using EDEA.Models;
using EDEA.Services;
using log4net;

namespace EDEA.ViewModels;

public class HistoryViewModel : TabViewModel
{
    public override string TabName => "History";

    private static readonly ILog log = LogManager.GetLogger(typeof(HistoryViewModel));

    private readonly HistoryProvider _historyProvider;

    public ICommand CopyHistoryDataToClipboardCommand { get; }

    public HistoryDataViewModel HistoryData { get; set; }

    public HistoryViewModel(string tabHeader, string tabVisibility, HistoryProvider historyProvider)
        : base(tabHeader, tabVisibility)
    {
        _historyProvider = historyProvider;
        HistoryData = new HistoryDataViewModel(new HistoryData());
        CopyHistoryDataToClipboardCommand = new CopyToClipboardCommand();
        getHistoryData().ContinueWith(delegate
        {
            _historyProvider.HistoryUpdated += _historyProvider_HistoryUpdated;
        });
    }

    private void _historyProvider_HistoryUpdated(object? sender, EventArgs e)
    {
        Task _ = getHistoryData();
    }

    private async Task getHistoryData()
    {
        HistoryData = new HistoryDataViewModel(await _historyProvider.GetHistoryData());
        updateView();
    }

    private void updateView()
    {
        OnPropertyChanged("HistoryData");
    }
}
