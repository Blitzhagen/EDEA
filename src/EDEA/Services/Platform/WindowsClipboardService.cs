using System;
using System.Threading;
using System.Windows;
using EDEA.Services;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF implementation of <see cref="IClipboardService"/>.
/// </summary>
public sealed class WindowsClipboardService : IClipboardService
{
    /// <summary>
    /// Sets the specified text on the system clipboard.
    /// </summary>
    /// <param name="text">The text to set.</param>
    public void SetText(string text)
    {
        var thread = new Thread(() =>
        {
            try
            {
                Clipboard.SetText(text, TextDataFormat.UnicodeText);
            }
            catch (Exception exception)
            {
                // TODO: Use a logger once the service has access to one.
                _ = exception;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
    }
}
