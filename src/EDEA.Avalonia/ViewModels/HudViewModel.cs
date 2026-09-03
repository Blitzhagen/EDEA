using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Avalonia.ViewModels;

/// <summary>
/// Proof-of-concept view model for the Avalonia HUD window.
/// </summary>
public partial class HudViewModel : ObservableObject
{
    /// <summary>
    /// Gets the system name.
    /// </summary>
    [ObservableProperty]
    private string _systemName = "Sol";

    /// <summary>
    /// Gets the body exploration status.
    /// </summary>
    [ObservableProperty]
    private string _bodyExplorationStatus = "Bodies: 5 / 8";

    /// <summary>
    /// Gets the exploration status.
    /// </summary>
    [ObservableProperty]
    private string _explorationStatus = "Exploring";

    /// <summary>
    /// Gets the non-body exploration status.
    /// </summary>
    [ObservableProperty]
    private string _nonBodyExplorationStatus = "Rings: 1 / 1";

    /// <summary>
    /// Gets the table headline.
    /// </summary>
    [ObservableProperty]
    private string _tableHeadline = "Bodies";
}
