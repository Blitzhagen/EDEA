using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using NLayer;
using PortAudioSharp;

namespace EDEA.Avalonia.Services.Audio;

/// <summary>
/// Cross-platform audio playback via PortAudio (bundled native library) — used on Linux
/// and any non-Windows platform. MP3 decoding is done in managed code by NLayer;
/// PortAudio only outputs the decoded float PCM samples.
/// </summary>
internal sealed class PortAudioAudioPlayer : IAudioPlayer
{
    /// <summary>
    /// The logger.
    /// </summary>
    private static readonly ILog Log = LogManager.GetLogger(typeof(PortAudioAudioPlayer));

    /// <summary>
    /// Whether PortAudio has been initialized (0 = not yet, 1 = done).
    /// </summary>
    private static int _portAudioInitialized;

    /// <summary>
    /// The currently active PortAudio output stream.
    /// </summary>
    private PortAudioSharp.Stream? _stream;

    /// <summary>
    /// The decoded PCM samples currently being played.
    /// </summary>
    private float[]? _samples;

    /// <summary>
    /// Number of samples already consumed by the playback callback.
    /// </summary>
    private int _position;

    /// <summary>
    /// Set to true when playback finished (end of data or aborted).
    /// </summary>
    private volatile bool _playbackDone;

    /// <summary>
    /// Set by <see cref="Stop"/> to abort playback.
    /// </summary>
    private volatile bool _stopRequested;

    /// <inheritdoc />
    public async Task PlayAsync(string filePath, float volume)
    {
        try
        {
            EnsurePortAudioInitialized();
            float[] samples;
            int sampleRate;
            int channels;
            using (var mpeg = new MpegFile(filePath))
            {
                sampleRate = mpeg.SampleRate;
                channels = mpeg.Channels;
                samples = DecodeAll(mpeg, volume);
            }

            int deviceIndex = PortAudio.DefaultOutputDevice;
            if (deviceIndex == PortAudio.NoDevice)
            {
                Log.Warn("No default audio output device found, skipping speech playback");
                return;
            }

            DeviceInfo info = PortAudio.GetDeviceInfo(deviceIndex);
            var param = new StreamParameters
            {
                device = deviceIndex,
                channelCount = channels,
                sampleFormat = SampleFormat.Float32,
                suggestedLatency = info.defaultLowOutputLatency,
                hostApiSpecificStreamInfo = IntPtr.Zero
            };

            _stopRequested = false;
            _playbackDone = false;
            _position = 0;
            _samples = samples;

            PortAudioSharp.Stream.Callback playCallback = (IntPtr input, IntPtr output,
                uint frameCount, ref StreamCallbackTimeInfo timeInfo,
                StreamCallbackFlags statusFlags, IntPtr userData) =>
            {
                if (_stopRequested)
                {
                    _playbackDone = true;
                    return StreamCallbackResult.Abort;
                }

                var currentSamples = _samples!;
                int samplesNeeded = (int)frameCount * channels;
                int available = currentSamples.Length - _position;
                int toCopy = Math.Min(samplesNeeded, available);
                if (toCopy > 0)
                {
                    Marshal.Copy(currentSamples, _position, output, toCopy);
                    _position += toCopy;
                }

                if (toCopy < samplesNeeded)
                {
                    int remainingBytes = (samplesNeeded - toCopy) * sizeof(float);
                    Marshal.Copy(new byte[remainingBytes], 0, IntPtr.Add(output, toCopy * sizeof(float)), remainingBytes);
                }

                if (_position >= currentSamples.Length)
                {
                    _playbackDone = true;
                    return StreamCallbackResult.Complete;
                }

                return StreamCallbackResult.Continue;
            };

            _stream = new PortAudioSharp.Stream(
                inParams: null,
                outParams: param,
                sampleRate: sampleRate,
                framesPerBuffer: 0,
                streamFlags: StreamFlags.ClipOff,
                callback: playCallback,
                userData: IntPtr.Zero);
            _stream.Start();

            while (!_playbackDone)
            {
                await Task.Delay(20).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Log.Error("Could not play audio via PortAudio", ex);
        }
        finally
        {
            CloseStream();
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        _stopRequested = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _stopRequested = true;
        CloseStream();
    }

    /// <summary>
    /// Decodes the whole MP3 file to an interleaved float PCM buffer, applying the volume.
    /// </summary>
    private static float[] DecodeAll(MpegFile mpeg, float volume)
    {
        var result = new List<float>();
        var buffer = new float[mpeg.SampleRate * Math.Max(mpeg.Channels, 1)];
        int read;
        while ((read = mpeg.ReadSamples(buffer, 0, buffer.Length)) > 0)
        {
            for (int i = 0; i < read; i++)
            {
                result.Add(buffer[i] * volume);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Initializes PortAudio once per process.
    /// </summary>
    private static void EnsurePortAudioInitialized()
    {
        if (Interlocked.CompareExchange(ref _portAudioInitialized, 1, 0) == 0)
        {
            PortAudio.Initialize();
            Log.Debug("PortAudio initialized");
        }
    }

    /// <summary>
    /// Stops and disposes the active stream, if any.
    /// </summary>
    private void CloseStream()
    {
        var stream = _stream;
        _stream = null;
        if (stream == null)
        {
            return;
        }

        try
        {
            if (stream.IsActive)
            {
                stream.Abort();
            }
        }
        catch
        {
            // Ignore cleanup failures.
        }

        try
        {
            stream.Dispose();
        }
        catch
        {
            // Ignore cleanup failures.
        }
    }
}
