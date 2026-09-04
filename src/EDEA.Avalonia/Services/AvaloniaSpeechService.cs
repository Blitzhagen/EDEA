using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using EDEA;
using EDEA.Models;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia/cross-platform implementation of <see cref="ISpeechService"/>.
/// </summary>
/// <remarks>
/// On Windows this uses <c>System.Speech.Synthesis</c>. On Linux it falls back to
/// <c>espeak</c> via a process call if available. Other platforms are no-ops.
/// </remarks>
public sealed class AvaloniaSpeechService : ISpeechService
{
    /// <summary>
    /// The Windows speech synthesizer, if available.
    /// </summary>
    private readonly object? _synthesizer;

    /// <summary>
    /// Whether the current platform is Windows.
    /// </summary>
    private readonly bool _isWindows;

    /// <summary>
    /// Whether the current platform is Linux.
    /// </summary>
    private readonly bool _isLinux;

    /// <summary>
    /// The cached list of installed synthesizer voices.
    /// </summary>
    private readonly List<string> _installedVoices = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaSpeechService"/> class.
    /// </summary>
    public AvaloniaSpeechService()
    {
        _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        _isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        if (_isWindows)
        {
            try
            {
                var type = Type.GetType("System.Speech.Synthesis.SpeechSynthesizer, System.Speech");
                _synthesizer = type != null ? Activator.CreateInstance(type) : null;
                CacheInstalledVoices();
            }
            catch
            {
                _synthesizer = null;
            }
        }
    }

    /// <inheritdoc />
    public bool IsSpeaking => false;

    /// <inheritdoc />
    public IReadOnlyList<string> InstalledVoices => _installedVoices;

    /// <inheritdoc />
#pragma warning disable CS0067
    public event EventHandler? StateChanged;

    /// <inheritdoc />
    public event EventHandler? VoicesLoaded;
#pragma warning restore CS0067

    /// <inheritdoc />
    public void SpeakWelcome(SpeechOutputCommander commander) => Speak("Welcome commander");

    /// <inheritdoc />
    public void SpeakGoodbye(SpeechOutputCommander commander) => Speak("See you in the black");

    /// <inheritdoc />
    public void SpeakFirstDiscoverySystem(SpeechOutputSystem system) => Speak($"First discovery of system {system}");

    /// <inheritdoc />
    public void SpeakFirstDiscoveryBody(SpeechOutputBody body) => Speak($"First discovery of {body}");

    /// <inheritdoc />
    public void SpeakTerraformable(SpeechOutputPlanet planet) => Speak($"{planet} is terraformable");

    /// <inheritdoc />
    public void SpeakLandable(SpeechOutputPlanet planet) => Speak($"{planet} is landable");

    /// <inheritdoc />
    public void SpeakGeologicalSignals(SpeechOutputPlanet planet) => Speak($"Geological signals on {planet}");

    /// <inheritdoc />
    public void SpeakBiologicalSignals(SpeechOutputPlanet planet) => Speak($"Biological signals on {planet}");

    /// <inheritdoc />
    public void SpeakValuableBody(SpeechOutputBody body, SpeechOutputCartographicValues values) => Speak($"{body} is valuable");

    /// <inheritdoc />
    public void SpeakValuableGenusPredicted(SpeechOutputPlanet planet, SpeechOutputSpecies species) => Speak($"Valuable genus {species} predicted on {planet}");

    /// <inheritdoc />
    public void SpeakValuableGeneraPredicted(SpeechOutputPlanet planet, SpeechOutputValuableSpeciesCount count) => Speak($"{count} valuable genera predicted on {planet}");

    /// <inheritdoc />
    public void SpeakLeaveClonalColonyRange(SpeechOutputSpecies species) => Speak($"Leaving range of {species}");

    /// <inheritdoc />
    public void SpeakEnterClonalColonyRange(SpeechOutputSpecies species) => Speak($"Entering range of {species}");

    /// <inheritdoc />
    public void SpeakMatchingClassificationFound(SpeechOutputPlanet planet, SpeechOutputPlanetClassification classification) => Speak($"{planet} is a planet of interest");

    /// <inheritdoc />
    public void SpeakMatchingClassificationsFound(SpeechOutputPlanet planet, SpeechOutputMatchingClassificationsCount count) => Speak($"{planet} has planets of interest");

    /// <inheritdoc />
    public void SpeakRingCount(SpeechOutputBody body, SpeechOutputRingsCount count) => Speak($"{body} has {count} rings");

    /// <inheritdoc />
    public void SpeakRing(SpeechOutputRing ring, SpeechOutputBody body) => Speak($"Ring at {body}");

    /// <inheritdoc />
    public void SpeakPreferencesSelection(string label, SpeechOutput[] outputs) => Speak(label);

    /// <inheritdoc />
    public void ShutUp()
    {
        if (_isWindows && _synthesizer != null)
        {
            try
            {
                _synthesizer.GetType().GetMethod("SpeakAsyncCancelAll")?.Invoke(_synthesizer, null);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Caches the names of all installed synthesizer voices.
    /// </summary>
    private void CacheInstalledVoices()
    {
        if (_synthesizer == null)
        {
            return;
        }

        try
        {
            var getInstalledVoices = _synthesizer.GetType().GetMethod("GetInstalledVoices");
            if (getInstalledVoices == null)
            {
                return;
            }

            var voices = getInstalledVoices.Invoke(_synthesizer, null) as System.Collections.IEnumerable;
            if (voices == null)
            {
                return;
            }

            foreach (var voice in voices)
            {
                var enabled = voice.GetType().GetProperty("Enabled")?.GetValue(voice) as bool? ?? true;
                if (!enabled)
                {
                    continue;
                }

                var voiceInfo = voice.GetType().GetProperty("VoiceInfo")?.GetValue(voice);
                var name = voiceInfo?.GetType().GetProperty("Name")?.GetValue(voiceInfo)?.ToString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    _installedVoices.Add(name);
                }
            }
        }
        catch
        {
            // Ignore voice enumeration failures.
        }
    }

    /// <inheritdoc />
    public void UpdateSpeechSynthesizerParameter()
    {
        if (_synthesizer == null || !_isWindows)
        {
            return;
        }

        try
        {
            var type = _synthesizer.GetType();
            var speech = Preferences.Speech;

            type.GetProperty("Rate")?.SetValue(_synthesizer, speech.SpeechSynthesizerRate);
            type.GetProperty("Volume")?.SetValue(_synthesizer, speech.SpeechSynthesizerVolume);

            var voice = speech.SpeechSynthesizerVoice;
            if (!string.IsNullOrWhiteSpace(voice))
            {
                type.GetMethod("SelectVoice", new[] { typeof(string) })?.Invoke(_synthesizer, new object[] { voice });
            }
        }
        catch
        {
            // Ignore synthesizer configuration failures.
        }
    }

    /// <inheritdoc />
    public string GetLabelForSpeechOutput(string outputName)
    {
        try
        {
            var resourceName = $"SpeechOutput_{outputName}";
            var property = typeof(EDEA.Properties.Resources).GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);
            return property?.GetValue(null)?.ToString() ?? outputName;
        }
        catch
        {
            return outputName;
        }
    }

    /// <inheritdoc />
    public Dictionary<string, string> GetPlaceholdersFromSpeechOutputs(SpeechOutput[] outputs) => new();

    /// <inheritdoc />
    public SpeechOutput[] GetExampleSpeechOutputs(string outputName) => Array.Empty<SpeechOutput>();

    /// <summary>
    /// Synthesizes the specified text on the current platform.
    /// </summary>
    /// <param name="text">The text to speak.</param>
    private void Speak(string text)
    {
        UpdateSpeechSynthesizerParameter();

        if (_isWindows && _synthesizer != null)
        {
            try
            {
                _synthesizer.GetType().GetMethod("Speak")?.Invoke(_synthesizer, new object[] { text });
            }
            catch
            {
                // Fall through to no-op on failure.
            }
        }
        else if (_isLinux)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "espeak",
                    Arguments = $"\"{text.Replace("\"", "\\\"")}\"",
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                });
            }
            catch
            {
                // espeak not available.
            }
        }
    }
}
