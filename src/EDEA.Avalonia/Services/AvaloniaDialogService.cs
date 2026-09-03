using Avalonia.Threading;
using EDEA.Avalonia.Windows;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IDialogService"/>.
/// </summary>
public sealed class AvaloniaDialogService : IDialogService
{
    /// <summary>
    /// Shows an error message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    public void ShowError(string message, string? title = null)
    {
        Show(message, title ?? "Error");
    }

    /// <summary>
    /// Shows a warning message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    public void ShowWarning(string message, string? title = null)
    {
        Show(message, title ?? "Warning");
    }

    /// <summary>
    /// Shows an information message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    public void ShowInformation(string message, string? title = null)
    {
        Show(message, title ?? "Information");
    }

    /// <summary>
    /// Shows the message box on the UI thread.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    private static void Show(string message, string title)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var window = new MessageBoxWindow(title, message);
            window.Show();
        });
    }
}
