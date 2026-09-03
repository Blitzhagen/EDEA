using System;
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
        // TODO: Implement Avalonia message box in Phase 5
        Console.Error.WriteLine($"[ERROR] {title}: {message}");
    }

    /// <summary>
    /// Shows a warning message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    public void ShowWarning(string message, string? title = null)
    {
        // TODO: Implement Avalonia message box in Phase 5
        Console.WriteLine($"[WARNING] {title}: {message}");
    }

    /// <summary>
    /// Shows an information message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The dialog title.</param>
    public void ShowInformation(string message, string? title = null)
    {
        // TODO: Implement Avalonia message box in Phase 5
        Console.WriteLine($"[INFO] {title}: {message}");
    }
}
