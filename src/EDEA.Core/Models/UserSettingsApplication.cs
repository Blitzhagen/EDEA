using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Models;

/// <summary>
/// Represents the application-related user settings.
/// </summary>
public partial class UserSettingsApplication : ObservableObject
{
    /// <summary>
    /// Whether the HUD window was open on shutdown.
    /// </summary>
    [ObservableProperty]
    private bool _hudWindowOpenOnShutdown;

    /// <summary>
    /// Whether mouse pass-through is enabled for the HUD window.
    /// </summary>
    [ObservableProperty]
    private bool _hudWindowMousePassThroughEnabled;

    /// <summary>
    /// Whether the about window was open on shutdown.
    /// </summary>
    [ObservableProperty]
    private bool _aboutWindowOpenOnShutdown;

    /// <summary>
    /// Whether the feedback report issue window was open on shutdown.
    /// </summary>
    [ObservableProperty]
    private bool _feedbackReportIssueWindowOpenOnShutdown;

    /// <summary>
    /// Whether the preferences window was open on shutdown.
    /// </summary>
    [ObservableProperty]
    private bool _preferencesWindowOpenOnShutdown;

    /// <summary>
    /// Whether the journal history import window was open on shutdown.
    /// </summary>
    [ObservableProperty]
    private bool _journalHistoryImportWindowOpenOnShutdown;

    /// <summary>
    /// Whether the route plotter window was open on shutdown.
    /// </summary>
    [ObservableProperty]
    private bool _routePlotterWindowOpenOnShutdown;

    /// <summary>
    /// The selected language.
    /// </summary>
    [ObservableProperty]
    private string _language = "Auto";

    /// <summary>
    /// The selected tab index.
    /// </summary>
    [ObservableProperty]
    private int _selectedTabIndex;

    /// <summary>
    /// The body table view sort member path.
    /// </summary>
    [ObservableProperty]
    private string _bodyTableViewSortMemberPath = string.Empty;

    /// <summary>
    /// The body table view sort direction.
    /// </summary>
    [ObservableProperty]
    private ListSortDirection _bodyTableViewSortDirection;

    /// <summary>
    /// The surroundings table view sort member path.
    /// </summary>
    [ObservableProperty]
    private string _surroundingsTableViewSortMemberPath = string.Empty;

    /// <summary>
    /// The surroundings table view sort direction.
    /// </summary>
    [ObservableProperty]
    private ListSortDirection _surroundingsTableViewSortDirection;

    // Non-1:1 convenience fields, used by the current FeedbackReportIssueWindow until it is ported
    /// <summary>
    /// The feedback name.
    /// </summary>
    [ObservableProperty]
    private string _feedbackName = string.Empty;

    /// <summary>
    /// The feedback email.
    /// </summary>
    [ObservableProperty]
    private string _feedbackEmail = string.Empty;

    /// <summary>
    /// The feedback message.
    /// </summary>
    [ObservableProperty]
    private string _feedbackMessage = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsApplication"/> class.
    /// </summary>
    public UserSettingsApplication()
    {
        SetDefaultValues();
    }

    /// <summary>
    /// Returns the default values for the application settings.
    /// </summary>
    /// <returns>A dictionary of property names and default values.</returns>
    protected virtual Dictionary<string, object> GetDefaultValues()
    {
        return new Dictionary<string, object>
        {
            { "HudWindowOpenOnShutdown", false },
            { "HudWindowMousePassThroughEnabled", false },
            { "AboutWindowOpenOnShutdown", false },
            { "FeedbackReportIssueWindowOpenOnShutdown", false },
            { "PreferencesWindowOpenOnShutdown", false },
            { "JournalHistoryImportWindowOpenOnShutdown", false },
            { "RoutePlotterWindowOpenOnShutdown", false },
            { "Language", "Auto" },
            { "SelectedTabIndex", 0 },
            { "BodyTableViewSortMemberPath", "DistanceSort" },
            { "BodyTableViewSortDirection", ListSortDirection.Ascending },
            { "SurroundingsTableViewSortMemberPath", "JumpDistanceLySort" },
            { "SurroundingsTableViewSortDirection", ListSortDirection.Ascending }
        };
    }

    /// <summary>
    /// Sets all or a single setting to its default value.
    /// </summary>
    /// <param name="singlePropertyInfo">The property to reset, or <see langword="null"/> to reset all.</param>
    public virtual void SetDefaultValues(PropertyInfo? singlePropertyInfo = null)
    {
        var defaultValues = GetDefaultValues();
        if (singlePropertyInfo is null)
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (defaultValues.TryGetValue(propertyInfo.Name, out var defaultValue))
                {
                    propertyInfo.SetValue(this, defaultValue);
                }
            }
        }
        else if (defaultValues.TryGetValue(singlePropertyInfo.Name, out var defaultValue))
        {
            singlePropertyInfo.SetValue(this, defaultValue);
        }
    }
}
