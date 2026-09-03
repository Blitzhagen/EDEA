using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
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
    public IReadOnlyList<string> InstalledVoices => new List<string>();

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

    /// <inheritdoc />
    public void UpdateSpeechSynthesizerParameter() { }

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
