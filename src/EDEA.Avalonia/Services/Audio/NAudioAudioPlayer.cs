using System;
using System.Threading.Tasks;
using NAudio.Wave;

namespace EDEA.Avalonia.Services.Audio;

/// <summary>
/// Audio playback via NAudio (WaveOut) — used on Windows.
/// </summary>
internal sealed class NAudioAudioPlayer : IAudioPlayer
{
    /// <summary>
    /// The current NAudio output device.
    /// </summary>
    private WaveOutEvent? _waveOut;

    /// <summary>
    /// The current NAudio reader.
    /// </summary>
    private AudioFileReader? _audioReader;

    /// <inheritdoc />
    public async Task PlayAsync(string filePath, float volume)
    {
        var tcs = new TaskCompletionSource();
        _audioReader?.Dispose();
        _waveOut?.Dispose();

        _audioReader = new AudioFileReader(filePath);
        _audioReader.Volume = volume;

        _waveOut = new WaveOutEvent();
        _waveOut.PlaybackStopped += (s, e) =>
        {
            _audioReader?.Dispose();
            _audioReader = null;
            _waveOut?.Dispose();
            _waveOut = null;
            tcs.TrySetResult();
        };
        _waveOut.Init(_audioReader);
        _waveOut.Play();

        await tcs.Task;
    }

    /// <inheritdoc />
    public void Stop() => _waveOut?.Stop();

    /// <inheritdoc />
    public void Dispose()
    {
        _audioReader?.Dispose();
        _waveOut?.Dispose();
    }
}
