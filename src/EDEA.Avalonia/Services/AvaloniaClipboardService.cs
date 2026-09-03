using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input.Platform;
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
        try
        {
            var clipboard = GetClipboard();
            if (clipboard is null)
            {
                return;
            }

            // Avalonia's clipboard API is async; block on a background thread to keep the synchronous contract.
            var thread = new Thread(() =>
            {
                try
                {
                    clipboard.SetTextAsync(text).GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                    // ignored
                }
            });
            if (OperatingSystem.IsWindows())
            {
                thread.SetApartmentState(ApartmentState.STA);
            }
            thread.Start();
            thread.Join();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    /// <summary>
    /// Gets the current application clipboard, if available.
    /// </summary>
    /// <returns>The clipboard instance, or <see langword="null"/>.</returns>
    private static IClipboard? GetClipboard()
    {
        if (Application.Current?.ApplicationLifetime is global::Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow?.Clipboard;
        }
        return null;
    }
}
