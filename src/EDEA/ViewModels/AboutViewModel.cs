using System;
using EDEA.Services;
using EDEA.Windows;

namespace EDEA.ViewModels;

public class AboutViewModel : ViewModelBase
{
    private static AboutWindow? aboutWindow;

    public bool AboutWindowOpen => aboutWindow != null;

    public string Title { get; }

    public string Version { get; }

    public AboutViewModel(MainViewModel mainViewModel)
    {
        Title = GetType().Assembly.GetName().Name!;
        Version = Globals.AppVersionString;
    }

    public void ShowAboutWindow()
    {
        if (aboutWindow == null)
        {
            aboutWindow = new AboutWindow();
            JotSettingsProvider.Tracker.Track(aboutWindow);
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

    private void AboutWindow_Closed(object? sender, EventArgs e)
    {
        aboutWindow = null;
    }
}
