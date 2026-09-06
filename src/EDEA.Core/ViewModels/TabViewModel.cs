using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.ViewModels;

/// <summary>
/// Base class for view models that are presented as tabs.
/// </summary>
public partial class TabViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the header text of the tab.
    /// </summary>
    /// <value>The tab header text.</value>
    [ObservableProperty]
    private string _tabHeader;

    /// <summary>
    /// Gets or sets the visibility of the tab.
    /// </summary>
    /// <value>The tab visibility string.</value>
    [ObservableProperty]
    private string _tabVisibility;

    /// <summary>
    /// Gets the name of the tab.
    /// </summary>
    /// <value>The tab name.</value>
    public virtual string TabName => TabHeader;

    /// <summary>
    /// Initializes a new instance of the <see cref="TabViewModel"/> class.
    /// </summary>
    /// <param name="tabHeader">The header text of the tab.</param>
    /// <param name="tabVisibility">The initial visibility of the tab.</param>
    public TabViewModel(string tabHeader, string tabVisibility)
    {
        _tabHeader = tabHeader;
        _tabVisibility = tabVisibility;
    }
}
