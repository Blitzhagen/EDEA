using System;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Services;
using EDEA.ViewModels;

namespace EDEA.Avalonia.ViewModels;

/// <summary>
/// Avalonia view model for the HUD window.
/// </summary>
public partial class HudViewModel : ObservableObject
{
    /// <summary>
    /// The star system provider.
    /// </summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>
    /// Gets the current star system view model.
    /// </summary>
    [ObservableProperty]
    private StarSystemViewModel _currentSystem = new StarSystemViewModel(new EDEA.Models.StarSystem(0L, string.Empty));

    /// <summary>
    /// Gets the table headline.
    /// </summary>
    [ObservableProperty]
    private string _tableHeadline = "Bodies";

    /// <summary>
    /// Initializes a new instance of the <see cref="HudViewModel"/> class.
    /// </summary>
    /// <param name="starSystemProvider">The star system provider.</param>
    public HudViewModel(StarSystemProvider starSystemProvider)
    {
        _starSystemProvider = starSystemProvider;
        _starSystemProvider.GuiDataUpdated += OnGuiDataUpdated;
        _currentSystem = new StarSystemViewModel(_starSystemProvider.CurrentSystem);
    }

    private void OnGuiDataUpdated(object? sender, EventArgs e)
    {
        CurrentSystem = new StarSystemViewModel(_starSystemProvider.CurrentSystem);
    }
}
