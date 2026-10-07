using System;
using System.Threading.Tasks;

namespace EDEA.Avalonia.Services.Audio;

/// <summary>
/// Plays an audio file to the default output device.
/// </summary>
internal interface IAudioPlayer : IDisposable
{
    /// <summary>
    /// Plays the specified audio file and completes when playback ends or <see cref="Stop"/> is called.
    /// </summary>
    /// <param name="filePath">The audio file to play.</param>
    /// <param name="volume">The playback volume in the range 0.0 to 1.0.</param>
    Task PlayAsync(string filePath, float volume);

    /// <summary>
    /// Stops the current playback, causing a running <see cref="PlayAsync"/> to complete.
    /// </summary>
    void Stop();
}
