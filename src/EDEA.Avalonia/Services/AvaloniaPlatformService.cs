using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia/cross-platform implementation of <see cref="IPlatformService"/>.
/// </summary>
public sealed class AvaloniaPlatformService : IPlatformService
{
    /// <summary>
    /// Opens the specified URI with the default application.
    /// </summary>
    /// <param name="uri">The URI to open.</param>
    public void OpenUri(string uri)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", uri);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", uri);
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    /// <summary>
    /// Opens the specified folder in the platform file manager.
    /// </summary>
    /// <param name="path">The folder path.</param>
    public void OpenFolder(string path)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start("explorer.exe", path);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", path);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", path);
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    /// <summary>
    /// Opens the specified file with the default application.
    /// </summary>
    /// <param name="path">The file path.</param>
    public void OpenFile(string path)
    {
        OpenUri(path);
    }

    /// <summary>
    /// Opens the default mail client with a new message to the specified address.
    /// </summary>
    /// <param name="address">The recipient address.</param>
    /// <param name="subject">The message subject.</param>
    /// <param name="body">The message body.</param>
    public void OpenMailTo(string address, string? subject = null, string? body = null)
    {
        var uri = $"mailto:{address}?subject={Uri.EscapeDataString(subject ?? string.Empty)}&body={Uri.EscapeDataString(body ?? string.Empty)}";
        OpenUri(uri);
    }
}
