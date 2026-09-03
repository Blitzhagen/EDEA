using System;
using System.Diagnostics;
using EDEA.Services;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF implementation of <see cref="IPlatformService"/>.
/// </summary>
public sealed class WindowsPlatformService : IPlatformService
{
    /// <summary>
    /// Opens the specified URI with the default application.
    /// </summary>
    /// <param name="uri">The URI to open.</param>
    public void OpenUri(string uri)
    {
        try
        {
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
        }
        catch (Exception exception)
        {
            _ = exception;
        }
    }

    /// <summary>
    /// Opens the specified folder in the Windows Explorer.
    /// </summary>
    /// <param name="path">The folder path.</param>
    public void OpenFolder(string path)
    {
        try
        {
            Process.Start("explorer.exe", path);
        }
        catch (Exception exception)
        {
            _ = exception;
        }
    }

    /// <summary>
    /// Opens the specified file with the default application.
    /// </summary>
    /// <param name="path">The file path.</param>
    public void OpenFile(string path)
    {
        try
        {
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception exception)
        {
            _ = exception;
        }
    }

    /// <summary>
    /// Opens the default mail client with a new message to the specified address.
    /// </summary>
    /// <param name="address">The recipient address.</param>
    /// <param name="subject">The message subject.</param>
    /// <param name="body">The message body.</param>
    public void OpenMailTo(string address, string? subject = null, string? body = null)
    {
        try
        {
            var uri = $"mailto:{address}?subject={Uri.EscapeDataString(subject ?? string.Empty)}&body={Uri.EscapeDataString(body ?? string.Empty)}";
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
        }
        catch (Exception exception)
        {
            _ = exception;
        }
    }
}
