using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Models;

public partial class UserSettingsApplication : ObservableObject
{
    [ObservableProperty]
    private bool _hudWindowOpenOnShutdown;

    [ObservableProperty]
    private bool _hudWindowMousePassThroughEnabled;

    [ObservableProperty]
    private bool _aboutWindowOpenOnShutdown;

    [ObservableProperty]
    private bool _feedbackReportIssueWindowOpenOnShutdown;

    [ObservableProperty]
    private bool _preferencesWindowOpenOnShutdown;

    [ObservableProperty]
    private bool _journalHistoryImportWindowOpenOnShutdown;

    [ObservableProperty]
    private bool _routePlotterWindowOpenOnShutdown;

    [ObservableProperty]
    private string _language = "Auto";

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private string _bodyTableViewSortMemberPath = string.Empty;

    [ObservableProperty]
    private ListSortDirection _bodyTableViewSortDirection;

    [ObservableProperty]
    private string _surroundingsTableViewSortMemberPath = string.Empty;

    [ObservableProperty]
    private ListSortDirection _surroundingsTableViewSortDirection;

    // Non-1:1 convenience fields, used by the current FeedbackReportIssueWindow until it is ported
    [ObservableProperty]
    private string _feedbackName = string.Empty;

    [ObservableProperty]
    private string _feedbackEmail = string.Empty;

    [ObservableProperty]
    private string _feedbackMessage = string.Empty;

    public UserSettingsApplication()
    {
        SetDefaultValues();
    }

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
