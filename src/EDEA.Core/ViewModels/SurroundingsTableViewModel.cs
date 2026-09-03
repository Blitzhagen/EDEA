using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using log4net;

namespace EDEA.ViewModels;

/// <summary>
/// View model that manages the surroundings tab and its data presentation.
/// </summary>
public class SurroundingsTableViewModel : TabViewModel
{
    /// <summary>
    /// Gets the name of the tab.
    /// </summary>
    /// <value>The tab name displayed in the UI.</value>
    public override string TabName => "Surroundings";

    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(SurroundingsTableViewModel));

    /// <summary>
    /// The provider for star system data.
    /// </summary>
    private StarSystemProvider _starSystemProvider;

    /// <summary>
    /// Gets or sets the command that copies the selected system name to the clipboard.
    /// </summary>
    /// <value>The copy command.</value>
    public ICommand? CopySystemNameToClipboardCommand { get; set; }

    /// <summary>
    /// Gets the collection of surrounding star system view models.
    /// </summary>
    /// <value>The surrounding star systems.</value>
    public IEnumerable<StarSystemViewModel> SurroundingStarSystems { get; private set; } = Enumerable.Empty<StarSystemViewModel>();

    /// <summary>
    /// Gets the current commander name.
    /// </summary>
    /// <value>The commander name.</value>
    public string CommanderName => _starSystemProvider.CommanderName;

    /// <summary>
    /// Gets the list of teammate names.
    /// </summary>
    /// <value>The teammate names.</value>
    public List<string> TeammateNames => _starSystemProvider.TeammateNames;

    /// <summary>
    /// Gets the loading info text for the surroundings.
    /// </summary>
    /// <value>The loading info text.</value>
    public string SurroundingsLoadingInfo { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the surroundings loading info should be shown.
    /// </summary>
    /// <value><c>true</c> if the loading info is visible; otherwise, <c>false</c>.</value>
    public bool ShowSurroundingsLoadingInfo => _starSystemProvider.SurroundingsAreLoading;

    /// <summary>
    /// Gets the localized info text shown when no surroundings are available.
    /// </summary>
    /// <value>The no surroundings info text.</value>
    public string NoSurroundingsInfo => Resources.NoSurroundingsInfo;

    /// <summary>
    /// Gets a value indicating whether the no surroundings info should be shown.
    /// </summary>
    /// <value><c>true</c> if the no surroundings info is visible; otherwise, <c>false</c>.</value>
    public bool ShowNoSurroundingsInfo { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SurroundingsTableViewModel"/> class.
    /// </summary>
    /// <param name="tabHeader">The header text for the tab.</param>
    /// <param name="tabVisibility">The initial visibility of the tab.</param>
    /// <param name="starSystemProvider">The provider for star system data.</param>
    public SurroundingsTableViewModel(string tabHeader, string tabVisibility, StarSystemProvider starSystemProvider)
        : base(tabHeader, tabVisibility)
    {
        _starSystemProvider = starSystemProvider;
        SurroundingsLoadingInfo = string.Empty;
        ShowNoSurroundingsInfo = false;
        _starSystemProvider.GuiDataUpdated += delegate
        {
            PlatformServices.Dispatcher?.Invoke(delegate
            {
                refreshView();
            });
        };
        _starSystemProvider.SurroundingsLoadingStatusChanged += _starSystemProvider_SurroundingsLoadingStatusChanged;
    }

    /// <summary>
    /// Handles changes of the surroundings loading status.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="text">The loading status text.</param>
    private void _starSystemProvider_SurroundingsLoadingStatusChanged(object? sender, string text)
    {
        SurroundingsLoadingInfo = text;
        ShowNoSurroundingsInfo = !ShowSurroundingsLoadingInfo && _starSystemProvider.SurroundingStarSystems.Count() < 1;
        OnPropertyChanged("ShowNoSurroundingsInfo");
        OnPropertyChanged("SurroundingsLoadingInfo");
        OnPropertyChanged("ShowSurroundingsLoadingInfo");
    }

    /// <summary>
    /// Generates the surrounding system view models, applying the configured sorting.
    /// </summary>
    public void GenerateSurroundingsViews()
    {
        try
        {
            string propertyName = Preferences.Application.SurroundingsTableViewSortMemberPath;
            PropertyInfo sortProperty = typeof(StarSystemViewModel).GetProperty(propertyName!)!;
            if (Preferences.Application.SurroundingsTableViewSortDirection == ListSortDirection.Descending)
            {
                SurroundingStarSystems = _starSystemProvider.SurroundingStarSystems.Select((KeyValuePair<long, StarSystem> entry) => new StarSystemViewModel(entry.Value)).OrderByDescending(orderByFunc);
            }
            else
            {
                SurroundingStarSystems = _starSystemProvider.SurroundingStarSystems.Select((KeyValuePair<long, StarSystem> entry) => new StarSystemViewModel(entry.Value)).OrderBy(orderByFunc);
            }
            dynamic orderByFunc(StarSystemViewModel viewModel)
            {
                return sortProperty.GetValue(viewModel)!;
            }
        }
        catch (Exception exception)
        {
            log.Error("Could not apply custom sorting to surroundings table view, using jump distance (Ly) instead", exception);
            SurroundingStarSystems = from x in _starSystemProvider.SurroundingStarSystems
                                     select new StarSystemViewModel(x.Value) into x
                                     orderby x.JumpDistanceLySort
                                     select x;
        }
    }

    /// <summary>
    /// Refreshes the surroundings data view on the UI thread.
    /// </summary>
    private async void refreshView()
    {
        try
        {

            log.Debug("EDEA4711: Refresh of SurroundingsTable");
            await Task.Run(delegate
            {
                GenerateSurroundingsViews();
                OnPropertyChanged("SurroundingStarSystems");
                OnPropertyChanged("ShowNoSurroundingsInfo");
                OnPropertyChanged("CommanderName");
                OnPropertyChanged("TeammateNames");
            });

        }
        catch (Exception exception)
        {
            log.Error("Error in refreshView", exception);
        }
    }
}
