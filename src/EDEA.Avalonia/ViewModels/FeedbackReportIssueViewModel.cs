using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Models;

namespace EDEA.Avalonia.ViewModels;

/// <summary>
/// View model for the Avalonia feedback and issue report window.
/// </summary>
public partial class FeedbackReportIssueViewModel : ObservableObject
{
    /// <summary>
    /// The application settings.
    /// </summary>
    private readonly UserSettingsApplication _settings = Preferences.Application;

    /// <summary>
    /// Gets or sets the feedback name.
    /// </summary>
    public string FeedbackName
    {
        get => _settings.FeedbackName;
        set
        {
            if (_settings.FeedbackName != value)
            {
                _settings.FeedbackName = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the feedback email.
    /// </summary>
    public string FeedbackEmail
    {
        get => _settings.FeedbackEmail;
        set
        {
            if (_settings.FeedbackEmail != value)
            {
                _settings.FeedbackEmail = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the feedback message.
    /// </summary>
    public string FeedbackMessage
    {
        get => _settings.FeedbackMessage;
        set
        {
            if (_settings.FeedbackMessage != value)
            {
                _settings.FeedbackMessage = value;
                OnPropertyChanged();
            }
        }
    }
}
