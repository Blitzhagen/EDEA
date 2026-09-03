namespace EDEA.Services;

/// <summary>
/// Abstraction for showing platform-native dialogs.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an error message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    void ShowError(string message, string? title = null);

    /// <summary>
    /// Shows a warning message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    void ShowWarning(string message, string? title = null);

    /// <summary>
    /// Shows an information message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    void ShowInformation(string message, string? title = null);
}
