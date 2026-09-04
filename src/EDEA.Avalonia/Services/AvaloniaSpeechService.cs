using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EDEA;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using log4net;
using NAudio.Wave;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia/cross-platform implementation of <see cref="ISpeechService"/> using SayIt
/// (Microsoft Edge TTS) and NAudio for playback, matching the original WPF behavior.
/// </summary>
public sealed class AvaloniaSpeechService : ISpeechService
{
    /// <summary>
    /// The logger.
    /// </summary>
    private static readonly ILog Log = LogManager.GetLogger(typeof(AvaloniaSpeechService));

    /// <summary>
    /// The queue of speech texts waiting to be synthesized and played.
    /// </summary>
    private readonly ConcurrentQueue<string> _speechQueue = new();

    /// <summary>
    /// The collection of installed TTS voices.
    /// </summary>
    private readonly ObservableCollection<string> _installedVoices = new();

    /// <summary>
    /// The metadata describing every configurable speech output.
    /// </summary>
    private readonly Dictionary<string, (string Label, SpeechOutput[] SpeechOutputs)> _preferencesParameter;

    /// <summary>
    /// The current NAudio output device.
    /// </summary>
    private WaveOutEvent? _waveOut;

    /// <summary>
    /// The current NAudio reader.
    /// </summary>
    private AudioFileReader? _audioReader;

    /// <summary>
    /// Whether a voice list request is currently in flight.
    /// </summary>
    private int _voicesLoading;

    /// <summary>
    /// Whether the speech queue is currently being processed.
    /// </summary>
    private int _isProcessing;

    /// <summary>
    /// Whether speech is currently playing.
    /// </summary>
    private bool _isSpeaking;

    /// <summary>
    /// Whether the voice list has already been loaded successfully.
    /// </summary>
    private bool _voicesLoaded;

    /// <summary>
    /// Occurs when the speech synthesizer state changes.
    /// </summary>
    public event EventHandler? StateChanged;

    /// <summary>
    /// Occurs when the list of available voices has been loaded.
    /// </summary>
    public event EventHandler? VoicesLoaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaSpeechService"/> class.
    /// </summary>
    public AvaloniaSpeechService()
    {
        _preferencesParameter = new Dictionary<string, (string, SpeechOutput[])>();
        GeneratePreferencesParameter();
        Log.Info("Speech provider initialized with SayIt (Edge TTS).");
        Interlocked.Exchange(ref _voicesLoading, 1);
        _ = LoadVoicesAsync();
    }

    /// <inheritdoc />
    public bool IsSpeaking => _isSpeaking;

    /// <inheritdoc />
    public IReadOnlyList<string> InstalledVoices
    {
        get
        {
            if (!_voicesLoaded && Interlocked.CompareExchange(ref _voicesLoading, 1, 0) == 0)
            {
                _ = LoadVoicesAsync();
            }

            return _installedVoices;
        }
    }

    /// <inheritdoc />
    public void SpeakWelcome(SpeechOutputCommander commander) => SpeakOutput(nameof(SpeechOutputType.Welcome), new SpeechOutput[] { commander });

    /// <inheritdoc />
    public void SpeakGoodbye(SpeechOutputCommander commander) => SpeakOutput(nameof(SpeechOutputType.Goodbye), new SpeechOutput[] { commander });

    /// <inheritdoc />
    public void SpeakFirstDiscoverySystem(SpeechOutputSystem system) => SpeakOutput(nameof(SpeechOutputType.FirstDiscoverySystem), new SpeechOutput[] { system });

    /// <inheritdoc />
    public void SpeakFirstDiscoveryBody(SpeechOutputBody body) => SpeakOutput(nameof(SpeechOutputType.FirstDiscoveryBody), new SpeechOutput[] { body });

    /// <inheritdoc />
    public void SpeakTerraformable(SpeechOutputPlanet planet) => SpeakOutput(nameof(SpeechOutputType.Terraformable), new SpeechOutput[] { planet });

    /// <inheritdoc />
    public void SpeakLandable(SpeechOutputPlanet planet) => SpeakOutput(nameof(SpeechOutputType.Landable), new SpeechOutput[] { planet });

    /// <inheritdoc />
    public void SpeakGeologicalSignals(SpeechOutputPlanet planet) => SpeakOutput(nameof(SpeechOutputType.GeologicalSignals), new SpeechOutput[] { planet });

    /// <inheritdoc />
    public void SpeakBiologicalSignals(SpeechOutputPlanet planet) => SpeakOutput(nameof(SpeechOutputType.BiologicalSignals), new SpeechOutput[] { planet });

    /// <inheritdoc />
    public void SpeakValuableBody(SpeechOutputBody body, SpeechOutputCartographicValues values) => SpeakOutput(nameof(SpeechOutputType.ValuableBody), new SpeechOutput[] { body, values });

    /// <inheritdoc />
    public void SpeakValuableGenusPredicted(SpeechOutputPlanet planet, SpeechOutputSpecies species) => SpeakOutput(nameof(SpeechOutputType.ValuableGenusPredicted), new SpeechOutput[] { planet, species });

    /// <inheritdoc />
    public void SpeakValuableGeneraPredicted(SpeechOutputPlanet planet, SpeechOutputValuableSpeciesCount count) => SpeakOutput(nameof(SpeechOutputType.ValuableGeneraPredicted), new SpeechOutput[] { planet, count });

    /// <inheritdoc />
    public void SpeakLeaveClonalColonyRange(SpeechOutputSpecies species) => SpeakOutput(nameof(SpeechOutputType.LeaveClonalColonyRange), new SpeechOutput[] { species });

    /// <inheritdoc />
    public void SpeakEnterClonalColonyRange(SpeechOutputSpecies species) => SpeakOutput(nameof(SpeechOutputType.EnterClonalColonyRange), new SpeechOutput[] { species });

    /// <inheritdoc />
    public void SpeakMatchingClassificationFound(SpeechOutputPlanet planet, SpeechOutputPlanetClassification classification) => SpeakOutput(nameof(SpeechOutputType.MatchingClassificationFound), new SpeechOutput[] { planet, classification });

    /// <inheritdoc />
    public void SpeakMatchingClassificationsFound(SpeechOutputPlanet planet, SpeechOutputMatchingClassificationsCount count) => SpeakOutput(nameof(SpeechOutputType.MatchingClassificationsFound), new SpeechOutput[] { planet, count });

    /// <inheritdoc />
    public void SpeakRingCount(SpeechOutputBody body, SpeechOutputRingsCount count) => SpeakOutput(nameof(SpeechOutputType.RingCount), new SpeechOutput[] { body, count });

    /// <inheritdoc />
    public void SpeakRing(SpeechOutputRing ring, SpeechOutputBody body) => SpeakOutput(nameof(SpeechOutputType.Ring), new SpeechOutput[] { ring, body });

    /// <inheritdoc />
    public void SpeakPreferencesSelection(string label, SpeechOutput[] outputs)
    {
        if (string.IsNullOrWhiteSpace(label) || outputs == null)
        {
            return;
        }

        var placeholders = GetPlaceholdersFromSpeechOutputs(outputs);
        var text = BuildSpeechText(label, placeholders);
        Speak(text);
    }

    /// <inheritdoc />
    public void ShutUp()
    {
        _speechQueue.Clear();
        _waveOut?.Stop();
        SetIsSpeaking(false);
    }

    /// <inheritdoc />
    public void UpdateSpeechSynthesizerParameter()
    {
        try
        {
            if (_isSpeaking)
            {
                _waveOut?.Stop();
            }

            var voice = Preferences.Speech.SpeechSynthesizerVoice;
            if (!string.IsNullOrEmpty(voice) && !_installedVoices.Contains(voice))
            {
                Log.Warn($"Cannot set SayIt voice '{voice}', voice not available; using default.");
                Preferences.Speech.SpeechSynthesizerVoice = GetDefaultVoice();
            }
        }
        catch
        {
            Log.Error("Could not update speech synthesizer parameter");
        }
    }

    /// <inheritdoc />
    public string GetLabelForSpeechOutput(string outputName)
    {
        try
        {
            var resourceName = $"SpeechOutput_{outputName}";
            var property = typeof(Resources).GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);
            return property?.GetValue(null)?.ToString() ?? outputName;
        }
        catch
        {
            return outputName;
        }
    }

    /// <inheritdoc />
    public Dictionary<string, string> GetPlaceholdersFromSpeechOutputs(SpeechOutput[] outputs)
    {
        var result = new Dictionary<string, string>();
        if (outputs == null)
        {
            return result;
        }

        foreach (var output in outputs)
        {
            foreach (var placeholder in output.Placeholders)
            {
                var key = "{" + Enum.GetName(typeof(SpeechOutputPlaceholderKeys), placeholder.Key) + "}";
                result[key] = placeholder.Value;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public SpeechOutput[] GetExampleSpeechOutputs(string outputName)
    {
        if (_preferencesParameter.TryGetValue(outputName, out var parameter))
        {
            return parameter.SpeechOutputs;
        }

        return Array.Empty<SpeechOutput>();
    }

    /// <summary>
    /// Loads the list of available SayIt voices asynchronously.
    /// </summary>
    private async Task LoadVoicesAsync()
    {
        try
        {
            var allVoices = await global::SayIt.SayIt.ListVoicesAsync();
            var currentLang = Resources.Culture.TwoLetterISOLanguageName;
            var filtered = allVoices
                .Where(v => v.Locale.StartsWith(currentLang, StringComparison.OrdinalIgnoreCase))
                .Select(v => v.ShortName)
                .ToList();
            if (filtered.Count == 0)
            {
                filtered = allVoices
                    .Where(v => v.Locale.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                    .Select(v => v.ShortName)
                    .ToList();
            }

            _installedVoices.Clear();
            foreach (var voice in filtered)
            {
                _installedVoices.Add(voice);
            }

            _voicesLoaded = true;
            Log.Info($"Loaded {_installedVoices.Count} SayIt voices for {currentLang}");
            VoicesLoaded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Log.Error("Unable to get voices from SayIt", ex);
        }
        finally
        {
            Interlocked.Exchange(ref _voicesLoading, 0);
        }
    }

    /// <summary>
    /// Builds the SayIt configuration from the current preferences.
    /// </summary>
    private global::SayIt.SayItConfig BuildSayItConfig()
    {
        string voice = Preferences.Speech.SpeechSynthesizerVoice;
        if (string.IsNullOrEmpty(voice))
        {
            voice = GetDefaultVoice();
        }

        var config = new global::SayIt.SayItConfig()
            .WithVoice(voice)
            .WithVolume("100%");

        int rate = (int)Math.Round(Preferences.Speech.SpeechSynthesizerRate);
        if (rate != 0)
        {
            string rateString = (rate * 25).ToString("+#;-#;+0") + "%";
            config = config.WithRate(rateString);
        }

        return config;
    }

    /// <summary>
    /// Gets a default voice from the installed voices, or a hard-coded fallback.
    /// </summary>
    private string GetDefaultVoice()
    {
        var defaultVoice = _installedVoices.FirstOrDefault(v => v.Contains("-KatjaNeural"))
            ?? _installedVoices.FirstOrDefault(v => v.Contains("-ConradNeural"))
            ?? _installedVoices.FirstOrDefault(v => v.StartsWith("de-", StringComparison.OrdinalIgnoreCase))
            ?? _installedVoices.FirstOrDefault(v => v.StartsWith("en-", StringComparison.OrdinalIgnoreCase));

        if (defaultVoice != null)
        {
            return defaultVoice;
        }

        return Resources.Culture.TwoLetterISOLanguageName.ToLowerInvariant() switch
        {
            "de" => "de-DE-KatjaNeural",
            "en" => "en-US-JennyNeural",
            "fr" => "fr-FR-DeniseNeural",
            "es" => "es-ES-ElviraNeural",
            "pt" => "pt-BR-FranciscaNeural",
            "ru" => "ru-RU-SvetlanaNeural",
            _ => "en-US-JennyNeural"
        };
    }

    /// <summary>
    /// Enqueues the specified text for speech synthesis and playback.
    /// </summary>
    private void Speak(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        Log.Info($"Enqueued speech: {text}");
        _speechQueue.Enqueue(text);

        if (Interlocked.Exchange(ref _isProcessing, 1) == 0)
        {
            _ = Task.Run(ProcessQueueAsync);
        }
    }

    /// <summary>
    /// Processes the speech queue by synthesizing and playing each entry.
    /// </summary>
    private async Task ProcessQueueAsync()
    {
        while (_speechQueue.TryDequeue(out var speechText) && speechText != null)
        {
            var tempFile = Path.GetTempFileName() + ".mp3";
            try
            {
                var config = BuildSayItConfig();
                await global::SayIt.SayIt.SaveAsync(speechText, tempFile, config);
                Log.Info($"Saved synthesized speech to {tempFile}");
                await PlayTempFileAsync(tempFile);
            }
            catch (Exception ex)
            {
                Log.Error("Could not process queued speech", ex);
            }
            finally
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch
                {
                    // Ignore cleanup failures.
                }
            }
        }

        Interlocked.Exchange(ref _isProcessing, 0);

        if (!_speechQueue.IsEmpty)
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 0)
            {
                _ = Task.Run(ProcessQueueAsync);
            }
        }
    }

    /// <summary>
    /// Plays the specified MP3 file through NAudio and waits for completion.
    /// </summary>
    private async Task PlayTempFileAsync(string tempFile)
    {
        var tcs = new TaskCompletionSource();
        try
        {
            _audioReader?.Dispose();
            _waveOut?.Dispose();

            _audioReader = new AudioFileReader(tempFile);
            _audioReader.Volume = Preferences.Speech.SpeechSynthesizerVolume / 100.0f;

            _waveOut = new WaveOutEvent();
            _waveOut.PlaybackStopped += (s, e) =>
            {
                SetIsSpeaking(false);
                _audioReader?.Dispose();
                _audioReader = null;
                _waveOut?.Dispose();
                _waveOut = null;
                tcs.TrySetResult();
                Log.Debug("NAudio finished playback");
            };
            _waveOut.Init(_audioReader);
            _waveOut.Play();

            SetIsSpeaking(true);
            Log.Info($"Playing synthesized speech from {tempFile} at volume {_audioReader.Volume}");
            await tcs.Task;
        }
        catch (Exception ex)
        {
            Log.Error("Could not play synthesized speech", ex);
            tcs.TrySetResult();
        }
    }

    /// <summary>
    /// Builds and speaks a configured speech output.
    /// </summary>
    private void SpeakOutput(string outputName, SpeechOutput[] outputs, bool ignoreSettings = false)
    {
        if (outputs == null)
        {
            return;
        }

        try
        {
            if (!_preferencesParameter.TryGetValue(outputName, out var parameter))
            {
                return;
            }

            if (parameter.SpeechOutputs.Length != outputs.Length)
            {
                Log.Error($"Invalid number of placeholders for spoken message! {outputName} needs {parameter.SpeechOutputs.Length} placeholder, but only {outputs.Length} were provided");
                return;
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i].GetType() != parameter.SpeechOutputs[i].GetType())
                {
                    Log.Error($"Invalid type of placeholder for spoken message! {outputName} needs {parameter.SpeechOutputs[i].GetType()} as parameter, but {outputs[i].GetType()} was provided");
                    return;
                }
            }

            var speechType = Preferences.Speech.GetType();
            var enabledProperty = speechType.GetProperty(outputName);
            if (enabledProperty == null || enabledProperty.PropertyType != typeof(bool))
            {
                return;
            }

            if (!(bool)enabledProperty.GetValue(Preferences.Speech)! && !ignoreSettings)
            {
                return;
            }

            var phraseProperty = speechType.GetProperty(outputName + "Speech");
            if (phraseProperty == null || phraseProperty.PropertyType != typeof(string))
            {
                return;
            }

            var phrase = (string)phraseProperty.GetValue(Preferences.Speech)!;
            var placeholders = GetPlaceholdersFromSpeechOutputs(outputs);
            var text = BuildSpeechText(phrase, placeholders);
            Speak(text);
        }
        catch (Exception ex)
        {
            if (ex is OperationCanceledException)
            {
                Log.Debug($"Speaking message for {outputName} was interrupted", ex);
            }
            else if (ex is FormatException)
            {
                Log.Error($"Format error on speaking message {outputName}, trying to reset message to default value ...");
                Preferences.Speech.SetDefaultValues(Preferences.Speech.GetType().GetProperty(outputName + "Speech")!);
            }
            else
            {
                Log.Error($"Error on speaking message {outputName}", ex);
            }
        }
    }

    /// <summary>
    /// Replaces placeholders in the phrase with the supplied values.
    /// </summary>
    private string BuildSpeechText(string phrase, Dictionary<string, string> placeholders)
    {
        var tokens = Regex.Split(phrase, "(?={.+})|(?<={.+})");
        var message = new StringBuilder();
        foreach (var token in tokens)
        {
            if (placeholders.TryGetValue(token, out var value))
            {
                if (value.Length > 1 && value.Any(char.IsLetter) && value.Where(char.IsLetter).All(char.IsUpper))
                {
                    value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());
                }

                message.Append(value);
                message.Append(' ');
            }
            else
            {
                message.Append(token);
                message.Append(' ');
            }
        }

        var text = message.ToString().Trim();
        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "de")
        {
            text = FixGermanSingularOne(text);
        }

        return text;
    }

    /// <summary>
    /// Fixes common German singular/plural mismatches.
    /// </summary>
    private static string FixGermanSingularOne(string text)
    {
        text = Regex.Replace(text, @"\b1\s+biologische\s+Signale\b", "Ein biologisches Signal");
        text = Regex.Replace(text, @"\b1\s+geologische\s+Signale\b", "Ein geologisches Signal");
        text = Regex.Replace(text, @"\b1\s+(geologischen|biologischen)\s+Signalen\b", "einem $1 Signal");
        return text;
    }

    /// <summary>
    /// Generates the example data used for the speech output preferences UI.
    /// </summary>
    private void GeneratePreferencesParameter()
    {
        var starSystem = new StarSystem(17000000L, "HIP 85639")
        {
            StarClass = "M"
        };
        var ring = new Ring("HIP 85639 4 a A Ring", starSystem.Id, 17, RingType.MetalRich, 5891600000L, 10889000L, 15267000L);
        var planet = new Planet(17, 17000000L, "HIP 85639 4 a", 1007.0, "High metal content body", true, string.Empty, 0.22, 401.0, "water geysers volcanism", "thin oxygen atmosphere", 9749629.0, null, null, 0.3, 3.4)
        {
            GeologicalCount = 2,
            BiologicalCount = 5,
            StarSystem = starSystem,
            RingsReserveLevel = RingReserveLevel.Major
        };
        planet.TryAddOrUpdateRing(ring, DataSource.Journal);
        var genus = new Genus("Stratum", 17000000L, 17, null, "Stratum Tectonicas", "Stratum Tectonicas - Lime")
        {
            ScanCount = 1
        };

        _preferencesParameter.Add(nameof(SpeechOutputType.Welcome), (Resources.SpeechOutput_Welcome, new SpeechOutput[]
        {
            new SpeechOutputCommander("Panostrede")
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.Goodbye), (Resources.SpeechOutput_Goodbye, new SpeechOutput[]
        {
            new SpeechOutputCommander("Panostrede")
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.GeologicalSignals), (Resources.SpeechOutput_GeologicalSignals, new SpeechOutput[]
        {
            new SpeechOutputPlanet(planet)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.BiologicalSignals), (Resources.SpeechOutput_BiologicalSignals, new SpeechOutput[]
        {
            new SpeechOutputPlanet(planet)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.FirstDiscoverySystem), (Resources.SpeechOutput_FirstDiscoverySystem, new SpeechOutput[]
        {
            new SpeechOutputSystem(starSystem)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.FirstDiscoveryBody), (Resources.SpeechOutput_FirstDiscoveryBody, new SpeechOutput[]
        {
            new SpeechOutputBody(planet)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.Terraformable), (Resources.SpeechOutput_Terraformable, new SpeechOutput[]
        {
            new SpeechOutputPlanet(planet)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.Landable), (Resources.SpeechOutput_Landable, new SpeechOutput[]
        {
            new SpeechOutputPlanet(planet)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.ValuableBody), (Resources.SpeechOutput_ValuableBody, new SpeechOutput[]
        {
            new SpeechOutputBody(planet),
            new SpeechOutputCartographicValues(2791256.0, 352687.0)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.RingCount), (Resources.SpeechOutput_RingCount, new SpeechOutput[]
        {
            new SpeechOutputBody(planet),
            new SpeechOutputRingsCount(planet)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.Ring), (Resources.SpeechOutput_Ring, new SpeechOutput[]
        {
            new SpeechOutputRing(ring, planet),
            new SpeechOutputBody(planet)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.ValuableGenusPredicted), (Resources.SpeechOutput_ValuableGenusPredicted, new SpeechOutput[]
        {
            new SpeechOutputPlanet(planet),
            new SpeechOutputSpecies(genus)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.ValuableGeneraPredicted), (Resources.SpeechOutput_ValuableGeneraPredicted, new SpeechOutput[]
        {
            new SpeechOutputPlanet(planet),
            new SpeechOutputValuableSpeciesCount(2)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.LeaveClonalColonyRange), (Resources.SpeechOutput_LeaveClonalColonyRange, new SpeechOutput[]
        {
            new SpeechOutputSpecies(genus)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.EnterClonalColonyRange), (Resources.SpeechOutput_EnterClonalColonyRange, new SpeechOutput[]
        {
            new SpeechOutputSpecies(genus)
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.MatchingClassificationFound), (Resources.SpeechOutput_MatchingClassificationFound, new SpeechOutput[]
        {
            new SpeechOutputPlanet(planet),
            new SpeechOutputPlanetClassification(new PlanetClassification(Resources.PlanetOfInterest_HighGravityLandable))
        }));
        _preferencesParameter.Add(nameof(SpeechOutputType.MatchingClassificationsFound), (Resources.SpeechOutput_MatchingClassificationsFound, new SpeechOutput[]
        {
            new SpeechOutputPlanet(planet),
            new SpeechOutputMatchingClassificationsCount(4)
        }));
    }

    /// <summary>
    /// Updates the speaking state and raises the <see cref="StateChanged"/> event.
    /// </summary>
    private void SetIsSpeaking(bool value)
    {
        _isSpeaking = value;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Defines the speech output names used for preference metadata.
    /// </summary>
    private enum SpeechOutputType
    {
        Welcome,
        Goodbye,
        GeologicalSignals,
        BiologicalSignals,
        FirstDiscoverySystem,
        FirstDiscoveryBody,
        Terraformable,
        Landable,
        ValuableBody,
        RingCount,
        Ring,
        ValuableGenusPredicted,
        ValuableGeneraPredicted,
        LeaveClonalColonyRange,
        EnterClonalColonyRange,
        MatchingClassificationFound,
        MatchingClassificationsFound
    }
}
