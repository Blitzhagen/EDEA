using System.Windows;
using EDEA.Properties;
using EDEA.Services;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF implementation of <see cref="IDialogService"/>.
/// </summary>
public sealed class WindowsDialogService : IDialogService
{
    /// <summary>
    /// Shows an error message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    public void ShowError(string message, string? title = null)
        => MessageBox.Show(message, title ?? Resources.MessageBoxTitle_Error, MessageBoxButton.OK, MessageBoxImage.Error);

    /// <summary>
    /// Shows a warning message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    public void ShowWarning(string message, string? title = null)
        => MessageBox.Show(message, title ?? Resources.MessageBoxTitle_Error, MessageBoxButton.OK, MessageBoxImage.Warning);

    /// <summary>
    /// Shows an information message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    public void ShowInformation(string message, string? title = null)
        => MessageBox.Show(message, title ?? Resources.MessageBoxTitle_Error, MessageBoxButton.OK, MessageBoxImage.Information);
}
