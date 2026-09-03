using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Avalonia.ViewModels;

/// <summary>
/// View model for the Avalonia about window.
/// </summary>
public partial class AboutViewModel : ObservableObject
{
    /// <summary>
    /// Gets the application version.
    /// </summary>
    public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
}
