namespace EDEA.Services;

/// <summary>
/// Abstraction for platform-specific operations such as opening URIs, folders or files.
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Opens the specified URI with the default application.
    /// </summary>
    /// <param name="uri">The URI to open.</param>
    void OpenUri(string uri);

    /// <summary>
    /// Opens the specified folder in the platform file manager.
    /// </summary>
    /// <param name="path">The folder path.</param>
    void OpenFolder(string path);

    /// <summary>
    /// Opens the specified file with the default application.
    /// </summary>
    /// <param name="path">The file path.</param>
    void OpenFile(string path);

    /// <summary>
    /// Opens the default mail client with a new message to the specified address.
    /// </summary>
    /// <param name="address">The recipient address.</param>
    /// <param name="subject">The message subject.</param>
    /// <param name="body">The message body.</param>
    void OpenMailTo(string address, string? subject = null, string? body = null);
}
