using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using EDEA.Avalonia.Windows;

namespace EDEA.Avalonia.Views;

/// <summary>
/// Avalonia main window.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Starts dragging the main window from the custom title bar.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event data.</param>
    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    /// <summary>
    /// Minimizes the main window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Toggles between maximized and normal window state.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void MaximizeRestoreButton_Click(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    /// <summary>
    /// Closes the main window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Toggles the HUD window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void OpenCloseHudWindow_Click(object? sender, RoutedEventArgs e)
    {
        var hudWindow = new HudWindow();
        hudWindow.Show();
    }

    /// <summary>
    /// Toggles mouse pass-through for the HUD window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void ToggleMousePassThrough_Click(object? sender, RoutedEventArgs e)
    {
        // Placeholder: actual implementation requires a live HUD window handle.
    }

    /// <summary>
    /// Shows the preferences window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void ShowPreferences_Click(object? sender, RoutedEventArgs e)
    {
        var preferencesWindow = new PreferencesWindow();
        preferencesWindow.ShowDialog(this);
    }

    /// <summary>
    /// Shows the feedback and report issue window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void ShowFeedback_Click(object? sender, RoutedEventArgs e)
    {
        var feedbackWindow = new FeedbackReportIssueWindow();
        feedbackWindow.ShowDialog(this);
    }

    /// <summary>
    /// Shows the about window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void ShowAbout_Click(object? sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.ShowDialog(this);
    }
}
