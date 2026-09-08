using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Avalonia.ViewModels;

/// <summary>
/// Avalonia view model for the HUD window.
/// </summary>
public partial class HudViewModel : ObservableObject
{
    /// <summary>
    /// The logger for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(HudViewModel));
    /// <summary>
    /// The star system provider.
    /// </summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>
    /// The main view model to track the selected tab.
    /// </summary>
    private readonly MainViewModel _mainViewModel;

    /// <summary>
    /// Gets the main view model.
    /// </summary>
    public MainViewModel MainViewModel => _mainViewModel;

    /// <summary>
    /// Gets the current star system view model.
    /// </summary>
    [ObservableProperty]
    private StarSystemViewModel _currentSystem = new(new StarSystem(0L, string.Empty));

    /// <summary>
    /// Gets the view model currently displayed in the HUD.
    /// </summary>
    [ObservableProperty]
    private TabViewModel? _currentViewModel;

    /// <summary>
    /// Gets the table headline.
    /// </summary>
    [ObservableProperty]
    private string _tableHeadline = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="HudViewModel"/> class.
    /// </summary>
    /// <param name="starSystemProvider">The star system provider.</param>
    /// <param name="mainViewModel">The main view model.</param>
    public HudViewModel(StarSystemProvider starSystemProvider, MainViewModel mainViewModel)
    {
        _starSystemProvider = starSystemProvider;
        _mainViewModel = mainViewModel;

        _starSystemProvider.GuiDataUpdated += (_, _) =>
        {
            CurrentSystem = new StarSystemViewModel(_starSystemProvider.CurrentSystem);
        };

        _mainViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
        Preferences.HudWindow.PropertyChanged += OnHudWindowPropertyChanged;

        CurrentSystem = new StarSystemViewModel(_starSystemProvider.CurrentSystem);
        UpdateView();

        Resources.CultureChanged += () =>
        {
            CurrentSystem = new StarSystemViewModel(_starSystemProvider.CurrentSystem);
            UpdateView();
        };
    }

    /// <summary>
    /// Updates the view when a relevant main view model property changes.
    /// </summary>
    private void OnMainViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.SelectedTab))
        {
            UpdateView();
        }
    }

    /// <summary>
    /// Updates the view when a relevant HUD window preference changes.
    /// </summary>
    private void OnHudWindowPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UserSettingsHudWindow.HudWindowTabViewModel))
        {
            UpdateView();
        }
    }

    /// <summary>
    /// Sets the current HUD view model and table headline based on the selected tab or HUD window preference.
    /// </summary>
    private void UpdateView()
    {
        CurrentViewModel = ResolveHudViewModel();
        TableHeadline = CurrentViewModel?.TabHeader ?? string.Empty;
        log.Debug($"HUD view updated: viewModel={(CurrentViewModel?.GetType().Name ?? "null")}, hudSetting={Preferences.HudWindow.HudWindowTabViewModel}, selectedTab={(_mainViewModel.SelectedTab?.GetType().Name ?? "null")}");
    }

    /// <summary>
    /// Resolves the view model to display in the HUD based on the user preference or selected tab.
    /// Only body, route and genus view models are supported by the HUD.
    /// </summary>
    /// <returns>The view model to display.</returns>
    private TabViewModel? ResolveHudViewModel()
    {
        var setting = Preferences.HudWindow.HudWindowTabViewModel;
        var target = setting switch
        {
            1 => _mainViewModel.TabViewModels.OfType<BodyTableViewModel>().FirstOrDefault(),
            2 => _mainViewModel.TabViewModels.OfType<NavRouteTableViewModel>().FirstOrDefault(),
            3 => _mainViewModel.TabViewModels.OfType<GenusTableViewModel>().FirstOrDefault(),
            _ => GetSupportedTab(_mainViewModel.SelectedTab),
        };

        return target ?? GetSupportedTab(_mainViewModel.SelectedTab);
    }

    /// <summary>
    /// Returns the given tab if it is supported by the HUD, otherwise falls back to the first supported tab.
    /// </summary>
    /// <param name="tab">The tab to check.</param>
    /// <returns>A supported tab view model, or <c>null</c> if none is available.</returns>
    private TabViewModel? GetSupportedTab(TabViewModel? tab)
    {
        if (tab is BodyTableViewModel or NavRouteTableViewModel or GenusTableViewModel)
        {
            return tab;
        }

        return _mainViewModel.TabViewModels.FirstOrDefault(t => t is BodyTableViewModel or NavRouteTableViewModel or GenusTableViewModel);
    }
}
