using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IClipboardService"/>.
/// </summary>
public sealed class AvaloniaClipboardService : IClipboardService
{
    /// <summary>
    /// Sets the specified text on the system clipboard.
    /// </summary>
    /// <param name="text">The text to set.</param>
    public void SetText(string text)
    {
        Dispatcher.UIThread.Post(async () =>
        {
            try
            {
                var clipboard = GetClipboard();
                if (clipboard is null)
                {
                    return;
                }

                await clipboard.SetTextAsync(text);
            }
            catch (Exception)
            {
                // Clipboard operations can fail on some platforms; ignore.
            }
        });
    }

    /// <summary>
    /// Gets the current application clipboard, if available.
    /// </summary>
    /// <returns>The clipboard instance, or <see langword="null"/>.</returns>
    private static IClipboard? GetClipboard()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow?.Clipboard;
        }

        return null;
    }
}
