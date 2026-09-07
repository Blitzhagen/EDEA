using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Properties;

namespace EDEA.ViewModels;

/// <summary>
/// Base class for view models that are presented as tabs.
/// </summary>
public partial class TabViewModel : ViewModelBase
{
    /// <summary>The tab header text or <see langword="null"/> when <see cref="TabHeaderKey"/> is used.</summary>
    private string _tabHeader;

    /// <summary>The resource key used to resolve <see cref="TabHeader"/>.</summary>
    private string? _tabHeaderKey;

    /// <summary>
    /// Gets or sets the resource key resolving <see cref="TabHeader"/>; reacts to language changes.
    /// </summary>
    /// <value>The resource key of the tab header text.</value>
    public string? TabHeaderKey
    {
        get => _tabHeaderKey;
        set
        {
            if (SetProperty(ref _tabHeaderKey, value))
            {
                OnPropertyChanged(nameof(TabHeader));
            }
        }
    }

    /// <summary>
    /// Gets or sets the header text of the tab.
    /// </summary>
    /// <value>The tab header text.</value>
    public string TabHeader
    {
        get => _tabHeaderKey is not null ? Resources.Lookup(_tabHeaderKey) : _tabHeader;
        set
        {
            _tabHeaderKey = null;
            SetProperty(ref _tabHeader, value);
        }
    }

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
    /// <param name="tabHeaderKey">An optional resource key used to resolve the header text.</param>
    public TabViewModel(string tabHeader, string tabVisibility, string? tabHeaderKey = null)
    {
        _tabHeader = tabHeader;
        _tabHeaderKey = tabHeaderKey;
        _tabVisibility = tabVisibility;
        Resources.CultureChanged += OnCultureChanged;
    }

    /// <summary>
    /// Re-raises all properties when the UI language changes.
    /// </summary>
    private void OnCultureChanged()
    {
        OnPropertyChanged(string.Empty);
    }
}
