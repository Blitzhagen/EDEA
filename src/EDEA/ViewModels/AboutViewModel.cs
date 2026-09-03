using System;
using EDEA.Services;
using EDEA.Windows;

namespace EDEA.ViewModels;

/// <summary>
/// Provides data and commands for the about window.
/// </summary>
public class AboutViewModel : ViewModelBase
{
    /// <summary>
    /// Holds the singleton instance of the about window.
    /// </summary>
    private static AboutWindow? aboutWindow;

    /// <summary>
    /// Gets a value indicating whether the about window is currently open.
    /// </summary>
    /// <value><c>true</c> if the about window is open; otherwise, <c>false</c>.</value>
    public bool AboutWindowOpen => aboutWindow != null;

    /// <summary>
    /// Gets the application title.
    /// </summary>
    /// <value>The application title.</value>
    public string Title { get; }

    /// <summary>
    /// Gets the application version string.
    /// </summary>
    /// <value>The application version string.</value>
    public string Version { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
    /// </summary>
    /// <param name="mainViewModel">The main view model of the application.</param>
    public AboutViewModel(MainViewModel mainViewModel)
    {
        Title = GetType().Assembly.GetName().Name!;
        Version = Globals.AppVersionString;
    }

    /// <summary>
    /// Shows the about window or activates it if already open.
    /// </summary>
    public void ShowAboutWindow()
    {
        if (aboutWindow == null)
        {
            aboutWindow = new AboutWindow();
            PlatformServices.WindowState?.Track(aboutWindow, "AboutWindow");
            aboutWindow.DataContext = this;
            aboutWindow.Closed += AboutWindow_Closed;
            aboutWindow.Show();
            return;
        }

        if (!aboutWindow.IsActive)
        {
            aboutWindow.Activate();
        }

        if (!aboutWindow.IsFocused)
        {
            aboutWindow.Focus();
        }
    }

    /// <summary>
    /// Handles the <see cref="AboutWindow.Closed"/> event and clears the window reference.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void AboutWindow_Closed(object? sender, EventArgs e)
    {
        aboutWindow = null;
    }
}
