using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using SayIt;
using EDEA;
using EDEA.Models;
using EDEA.Properties;
using log4net;

namespace EDEA.Services;

/// <summary>Provides SpeechProvider functionality.</summary>
public static class SpeechProvider
{
    /// <summary>Gets the SpeechOutputType.</summary>
    /// <value>A enum value.</value>
    /// <summary>Defines the SpeechOutputType values.</summary>
    public enum SpeechOutputType
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

    /// <summary>The _waveOut field.</summary>
    private static WaveOutEvent? _waveOut;
    /// <summary>The _audioReader field.</summary>
    private static AudioFileReader? _audioReader;
    /// <summary>The _speechQueue field.</summary>
    private static readonly ConcurrentQueue<string> _speechQueue = new();
    /// <summary>The _isProcessing field.</summary>
    private static int _isProcessing;

    /// <summary>The log field.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(SpeechProvider));

    /// <summary>The _preferencesParameter field.</summary>
    private static readonly Dictionary<string, (string Label, SpeechOutput[] SpeechOutputs)> _preferencesParameter;

    /// <summary>The _voiceNames field.</summary>
    private static List<string> _voiceNames = new List<string>();
    /// <summary>The _voicesLoading field.</summary>
    private static bool _voicesLoading;

    /// <summary>Gets or sets the IsSpeaking.</summary>
    /// <value>A bool value.</value>
    public static bool IsSpeaking { get; private set; }

    /// <summary>Occurs when the SpeechSynthesizerStateChanged event is raised.</summary>
    public static event EventHandler SpeechSynthesizerStateChanged = delegate { };
    /// <summary>Occurs when the VoicesLoaded event is raised.</summary>
    public static event EventHandler VoicesLoaded = delegate { };

    /// <summary>Initializes the SpeechProvider class.</summary>
    static SpeechProvider()
    {
        SpeechSynthesizerStateChanged = delegate { };
        _preferencesParameter = new Dictionary<string, (string, SpeechOutput[])>();
        generatePreferencesParameter();
        log.Info("Speech provider initialized with SayIt (Edge TTS).");
    }

    /// <summary>Performs the Speak operation.</summary>
    /// <param name="text">The string value of the text parameter.</param>
    /// <param name="synchronous">The bool value of the synchronous parameter.</param>
    public static void Speak(string text, bool synchronous = false)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        log.Info($"Enqueued speech: {text}");
        _speechQueue.Enqueue(text);

        if (Interlocked.Exchange(ref _isProcessing, 1) == 0)
        {
            _ = Task.Run(ProcessQueueAsync);
        }
    }

    /// <summary>Performs the ProcessQueueAsync operation.</summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private static async Task ProcessQueueAsync()
    {
        while (_speechQueue.TryDequeue(out var speechText) && speechText != null)
        {
            try
            {
                var tempFile = Path.GetTempFileName() + ".mp3";
                var config = BuildSayItConfig();
                await global::SayIt.SayIt.SaveAsync(speechText, tempFile, config);
                log.Info($"Saved synthesized speech to {tempFile}");
                await PlayTempFileAsync(tempFile);
            }
            catch (Exception ex)
            {
                log.Error("Could not process queued speech", ex);
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

    /// <summary>Performs the PlayTempFileAsync operation.</summary>
    /// <param name="tempFile">The string value of the tempFile parameter.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private static async Task PlayTempFileAsync(string tempFile)
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
                IsSpeaking = false;
                SpeechSynthesizerStateChanged(null, EventArgs.Empty);
                _audioReader?.Dispose();
                _audioReader = null;
                _waveOut?.Dispose();
                _waveOut = null;
                tcs.TrySetResult();
                log.Debug("NAudio finished playback");
            };
            _waveOut.Init(_audioReader);
            _waveOut.Play();

            IsSpeaking = true;
            SpeechSynthesizerStateChanged(null, EventArgs.Empty);
            log.Info($"Playing synthesized speech from {tempFile} at volume {_audioReader.Volume}");
            await tcs.Task;
        }
        catch (Exception ex)
        {
            log.Error("Could not play synthesized speech", ex);
            tcs.TrySetResult();
        }
    }

    /// <summary>Creates SayItConfig.</summary>
    /// <returns>A SayItConfig result.</returns>
    private static SayItConfig BuildSayItConfig()
    {
        string voice = Preferences.Speech.SpeechSynthesizerVoice;
        if (string.IsNullOrEmpty(voice) || !GetVoiceNames().Contains(voice))
        {
            voice = GetDefaultVoice();
        }

        var config = new SayItConfig().WithVoice(voice).WithVolume("100%");

        int rate = Preferences.Speech.SpeechSynthesizerRate;
        if (rate != 0)
        {
            string rateString = (rate * 25).ToString("+#;-#;+0") + "%";
            config.WithRate(rateString);
        }

        return config;
    }

    /// <summary>Retrieves DefaultVoice.</summary>
    /// <returns>A string result.</returns>
    private static string GetDefaultVoice()
    {
        var voices = GetVoiceNames();
        var defaultVoice = voices.FirstOrDefault(v => v.Contains("-KatjaNeural"))
            ?? voices.FirstOrDefault(v => v.Contains("-ConradNeural"))
            ?? voices.FirstOrDefault(v => v.StartsWith("de-"))
            ?? voices.FirstOrDefault(v => v.StartsWith("en-"));
        if (defaultVoice != null)
            return defaultVoice;

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

    /// <summary>Performs the ShutUp operation.</summary>
    public static void ShutUp()
    {
        _speechQueue.Clear();
        _waveOut?.Stop();
        IsSpeaking = false;
        SpeechSynthesizerStateChanged(null, EventArgs.Empty);
    }

    /// <summary>Retrieves InstalledVoices.</summary>
    /// <returns>A IReadOnlyList<string> result.</returns>
    public static IReadOnlyList<string> GetInstalledVoices()
    {
        return GetVoiceNames();
    }

    /// <summary>Retrieves VoiceNames.</summary>
    /// <returns>A List<string> result.</returns>
    public static List<string> GetVoiceNames()
    {
        if (!_voicesLoading)
        {
            _voicesLoading = true;
            _ = Task.Run(async () =>
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
                    _voiceNames = filtered;
                    log.Info($"Loaded {filtered.Count} SayIt voices for {currentLang}");
                    VoicesLoaded(null, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    log.Error("Unable to get voices from SayIt", ex);
                }
            });
        }
        return _voiceNames;
    }

    /// <summary>Performs the SpeakPreferencesSelection operation.</summary>
    /// <param name="speechOutputType">The SpeechOutputType value of the speechOutputType parameter.</param>
    public static void SpeakPreferencesSelection(SpeechOutputType speechOutputType)
    {
        speak(speechOutputType, GetExampleSpeechOutputs(speechOutputType), synchronous: false, ignoreSettings: true);
    }

    /// <summary>Performs the SpeakWelcome operation.</summary>
    /// <param name="speechOutputCommander">The SpeechOutputCommander value of the speechOutputCommander parameter.</param>
    public static void SpeakWelcome(SpeechOutputCommander speechOutputCommander)
    {
        speak(SpeechOutputType.Welcome, new SpeechOutput[1] { speechOutputCommander });
    }

    /// <summary>Performs the SpeakGoodbye operation.</summary>
    /// <param name="speechOutputCommander">The SpeechOutputCommander value of the speechOutputCommander parameter.</param>
    public static void SpeakGoodbye(SpeechOutputCommander speechOutputCommander)
    {
        speak(SpeechOutputType.Goodbye, new SpeechOutput[1] { speechOutputCommander }, synchronous: true);
    }

    /// <summary>Performs the SpeakGeologicalSignals operation.</summary>
    /// <param name="speechOutputPlanet">The SpeechOutputPlanet value of the speechOutputPlanet parameter.</param>
    public static void SpeakGeologicalSignals(SpeechOutputPlanet speechOutputPlanet)
    {
        speak(SpeechOutputType.GeologicalSignals, new SpeechOutput[1] { speechOutputPlanet });
    }

    /// <summary>Performs the SpeakBiologicalSignals operation.</summary>
    /// <param name="speechOutputPlanet">The SpeechOutputPlanet value of the speechOutputPlanet parameter.</param>
    public static void SpeakBiologicalSignals(SpeechOutputPlanet speechOutputPlanet)
    {
        speak(SpeechOutputType.BiologicalSignals, new SpeechOutput[1] { speechOutputPlanet });
    }

    /// <summary>Performs the SpeakFirstDiscoverySystem operation.</summary>
    /// <param name="speechOutputSystem">The SpeechOutputSystem value of the speechOutputSystem parameter.</param>
    public static void SpeakFirstDiscoverySystem(SpeechOutputSystem speechOutputSystem)
    {
        speak(SpeechOutputType.FirstDiscoverySystem, new SpeechOutput[1] { speechOutputSystem });
    }

    /// <summary>Performs the SpeakFirstDiscoveryBody operation.</summary>
    /// <param name="speechOutputBody">The SpeechOutputBody value of the speechOutputBody parameter.</param>
    public static void SpeakFirstDiscoveryBody(SpeechOutputBody speechOutputBody)
    {
        speak(SpeechOutputType.FirstDiscoveryBody, new SpeechOutput[1] { speechOutputBody });
    }

    /// <summary>Performs the SpeakTerraformable operation.</summary>
    /// <param name="speechOutputPlanet">The SpeechOutputPlanet value of the speechOutputPlanet parameter.</param>
    public static void SpeakTerraformable(SpeechOutputPlanet speechOutputPlanet)
    {
        speak(SpeechOutputType.Terraformable, new SpeechOutput[1] { speechOutputPlanet });
    }

    /// <summary>Performs the SpeakLandable operation.</summary>
    /// <param name="speechOutputPlanet">The SpeechOutputPlanet value of the speechOutputPlanet parameter.</param>
    public static void SpeakLandable(SpeechOutputPlanet speechOutputPlanet)
    {
        speak(SpeechOutputType.Landable, new SpeechOutput[1] { speechOutputPlanet });
    }

    /// <summary>Performs the SpeakValuableBody operation.</summary>
    /// <param name="speechOutputBody">The SpeechOutputBody value of the speechOutputBody parameter.</param>
    /// <param name="speechOutputCartographicValues">The SpeechOutputCartographicValues value of the speechOutputCartographicValues parameter.</param>
    public static void SpeakValuableBody(SpeechOutputBody speechOutputBody, SpeechOutputCartographicValues speechOutputCartographicValues)
    {
        speak(SpeechOutputType.ValuableBody, new SpeechOutput[2] { speechOutputBody, speechOutputCartographicValues });
    }

    /// <summary>Performs the SpeakValuableGenusPredicted operation.</summary>
    /// <param name="speechOutputPlanet">The SpeechOutputPlanet value of the speechOutputPlanet parameter.</param>
    /// <param name="speechOutputSpecies">The SpeechOutputSpecies value of the speechOutputSpecies parameter.</param>
    public static void SpeakValuableGenusPredicted(SpeechOutputPlanet speechOutputPlanet, SpeechOutputSpecies speechOutputSpecies)
    {
        speak(SpeechOutputType.ValuableGenusPredicted, new SpeechOutput[2] { speechOutputPlanet, speechOutputSpecies });
    }

    /// <summary>Performs the SpeakValuableGeneraPredicted operation.</summary>
    /// <param name="speechOutputPlanet">The SpeechOutputPlanet value of the speechOutputPlanet parameter.</param>
    /// <param name="speechOutputValuableSpeciesCount">The SpeechOutputValuableSpeciesCount value of the speechOutputValuableSpeciesCount parameter.</param>
    public static void SpeakValuableGeneraPredicted(SpeechOutputPlanet speechOutputPlanet, SpeechOutputValuableSpeciesCount speechOutputValuableSpeciesCount)
    {
        speak(SpeechOutputType.ValuableGeneraPredicted, new SpeechOutput[2] { speechOutputPlanet, speechOutputValuableSpeciesCount });
    }

    /// <summary>Performs the SpeakLeaveClonalColonyRange operation.</summary>
    /// <param name="speechOutputSpecies">The SpeechOutputSpecies value of the speechOutputSpecies parameter.</param>
    public static void SpeakLeaveClonalColonyRange(SpeechOutputSpecies speechOutputSpecies)
    {
        speak(SpeechOutputType.LeaveClonalColonyRange, new SpeechOutput[1] { speechOutputSpecies });
    }

    /// <summary>Performs the SpeakEnterClonalColonyRange operation.</summary>
    /// <param name="speechOutputSpecies">The SpeechOutputSpecies value of the speechOutputSpecies parameter.</param>
    public static void SpeakEnterClonalColonyRange(SpeechOutputSpecies speechOutputSpecies)
    {
        speak(SpeechOutputType.EnterClonalColonyRange, new SpeechOutput[1] { speechOutputSpecies });
    }

    /// <summary>Performs the SpeakMatchingClassificationFound operation.</summary>
    /// <param name="speechOutputPlanet">The SpeechOutputPlanet value of the speechOutputPlanet parameter.</param>
    /// <param name="speechOutputPlanetClassification">The SpeechOutputPlanetClassification value of the speechOutputPlanetClassification parameter.</param>
    public static void SpeakMatchingClassificationFound(SpeechOutputPlanet speechOutputPlanet, SpeechOutputPlanetClassification speechOutputPlanetClassification)
    {
        speak(SpeechOutputType.MatchingClassificationFound, new SpeechOutput[2] { speechOutputPlanet, speechOutputPlanetClassification });
    }

    /// <summary>Performs the SpeakMatchingClassificationsFound operation.</summary>
    /// <param name="speechOutputPlanet">The SpeechOutputPlanet value of the speechOutputPlanet parameter.</param>
    /// <param name="speechOutputMatchingClassificationsCount">The SpeechOutputMatchingClassificationsCount value of the speechOutputMatchingClassificationsCount parameter.</param>
    public static void SpeakMatchingClassificationsFound(SpeechOutputPlanet speechOutputPlanet, SpeechOutputMatchingClassificationsCount speechOutputMatchingClassificationsCount)
    {
        speak(SpeechOutputType.MatchingClassificationsFound, new SpeechOutput[2] { speechOutputPlanet, speechOutputMatchingClassificationsCount });
    }

    /// <summary>Performs the SpeakRingCount operation.</summary>
    /// <param name="speechOutputBody">The SpeechOutputBody value of the speechOutputBody parameter.</param>
    /// <param name="speechOutputRingsCount">The SpeechOutputRingsCount value of the speechOutputRingsCount parameter.</param>
    public static void SpeakRingCount(SpeechOutputBody speechOutputBody, SpeechOutputRingsCount speechOutputRingsCount)
    {
        speak(SpeechOutputType.RingCount, new SpeechOutput[2] { speechOutputBody, speechOutputRingsCount });
    }

    /// <summary>Performs the SpeakRing operation.</summary>
    /// <param name="speechOutputRing">The SpeechOutputRing value of the speechOutputRing parameter.</param>
    /// <param name="speechOutputBody">The SpeechOutputBody value of the speechOutputBody parameter.</param>
    public static void SpeakRing(SpeechOutputRing speechOutputRing, SpeechOutputBody speechOutputBody)
    {
        speak(SpeechOutputType.Ring, new SpeechOutput[2] { speechOutputRing, speechOutputBody });
    }

    /// <summary>Retrieves ExampleSpeechOutputs.</summary>
    /// <param name="output">The SpeechOutputType value of the output parameter.</param>
    /// <returns>A SpeechOutput[] result.</returns>
    public static SpeechOutput[] GetExampleSpeechOutputs(SpeechOutputType output)
    {
        return _preferencesParameter[Enum.GetName(typeof(SpeechOutputType), output)!].SpeechOutputs;
    }

    /// <summary>Retrieves LabelForSpeechOutput.</summary>
    /// <param name="output">The SpeechOutputType value of the output parameter.</param>
    /// <returns>A string result.</returns>
    public static string GetLabelForSpeechOutput(SpeechOutputType output)
    {
        return _preferencesParameter[Enum.GetName(typeof(SpeechOutputType), output)!].Label;
    }

    /// <summary>Retrieves PlaceholdersFromSpeechOutputs.</summary>
    /// <param name="speechOutputs">The SpeechOutput[] value of the speechOutputs parameter.</param>
    /// <returns>A Dictionary<string, string> result.</returns>
    public static Dictionary<string, string> GetPlaceholdersFromSpeechOutputs(SpeechOutput[] speechOutputs)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        if (speechOutputs == null)
        {
            return dictionary;
        }
        for (int i = 0; i < speechOutputs.Length; i++)
        {
            foreach (KeyValuePair<SpeechOutputPlaceholderKeys, string> placeholder in speechOutputs[i].Placeholders)
            {
                dictionary.Add("{" + Enum.GetName(typeof(SpeechOutputPlaceholderKeys), placeholder.Key) + "}", placeholder.Value);
            }
        }
        return dictionary;
    }

    /// <summary>Sets SpeechSynthesizerParameter.</summary>
    public static void UpdateSpeechSynthesizerParameter()
    {
        try
        {
            if (IsSpeaking)
            {
                _waveOut?.Stop();
            }

            var voices = GetVoiceNames();
            if (!string.IsNullOrEmpty(Preferences.Speech.SpeechSynthesizerVoice) && !voices.Contains(Preferences.Speech.SpeechSynthesizerVoice))
            {
                log.Warn($"Cannot set SayIt voice '{Preferences.Speech.SpeechSynthesizerVoice}', voice not available; using default.");
                Preferences.Speech.SpeechSynthesizerVoice = GetDefaultVoice();
            }
        }
        catch (Exception)
        {
            log.Error("Could not update speech synthesizer parameter");
        }
    }

    /// <summary>Performs the FixGermanSingularOne operation.</summary>
    /// <param name="text">The string value of the text parameter.</param>
    /// <returns>A string result.</returns>
    private static string FixGermanSingularOne(string text)
    {
        text = Regex.Replace(text, @"\b1\s+biologische\s+Signale\b", "Ein biologisches Signal");
        text = Regex.Replace(text, @"\b1\s+geologische\s+Signale\b", "Ein geologisches Signal");
        text = Regex.Replace(text, @"\b1\s+(geologischen|biologischen)\s+Signalen\b", "einem $1 Signal");
        return text;
    }

    /// <summary>Performs the speak operation.</summary>
    /// <param name="speechOutputType">The SpeechOutputType value of the speechOutputType parameter.</param>
    /// <param name="speechOutputs">The SpeechOutput[]? value of the speechOutputs parameter.</param>
    /// <param name="synchronous">The bool value of the synchronous parameter.</param>
    /// <param name="ignoreSettings">The bool value of the ignoreSettings parameter.</param>
    private static void speak(SpeechOutputType speechOutputType, SpeechOutput[]? speechOutputs = null, bool synchronous = false, bool ignoreSettings = false)
    {
        string name = Enum.GetName(typeof(SpeechOutputType), speechOutputType)!;
        if (speechOutputs == null)
        {
            return;
        }
        try
        {
            if (speechOutputs.Length != _preferencesParameter[name].SpeechOutputs.Length)
            {
                log.Error($"Invalid number of placeholders for spoken message! {name} needs {_preferencesParameter[name].SpeechOutputs.Length} placeholder, but only {speechOutputs.Length} were provided");
                return;
            }
            for (int i = 0; i < speechOutputs.Length; i++)
            {
                if (speechOutputs[i].GetType() != _preferencesParameter[name].SpeechOutputs[i].GetType())
                {
                    log.Error($"Invalid type of placeholder for spoken message! {name} needs {_preferencesParameter[name].SpeechOutputs[i].GetType()} as parameter , but {speechOutputs[i].GetType()} was provided");
                    return;
                }
            }
            if (!(bool)Preferences.Speech.GetType().GetProperty(name)!.GetValue(Preferences.Speech)! && !ignoreSettings)
            {
                return;
            }
            string[] tokens = Regex.Split((string)Preferences.Speech.GetType().GetProperty(name + "Speech")!.GetValue(Preferences.Speech)!, "(?={.+})|(?<={.+})");
            Dictionary<string, string> placeholdersFromSpeechOutputs = GetPlaceholdersFromSpeechOutputs(speechOutputs);
            StringBuilder message = new StringBuilder();
            foreach (string token in tokens)
            {
                if (placeholdersFromSpeechOutputs.ContainsKey(token))
                {
                    log.Debug("Speech placeholder found: " + token + ", ");
                    string placeholderValue = placeholdersFromSpeechOutputs[token];
                    if (placeholderValue.Length > 1 && placeholderValue.Any(char.IsLetter) && placeholderValue.Where(char.IsLetter).All(char.IsUpper))
                    {
                        placeholderValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(placeholderValue.ToLowerInvariant());
                    }
                    message.Append(placeholderValue);
                    message.Append(" ");
                }
                else
                {
                    message.Append(token);
                    message.Append(" ");
                }
            }
            string speechText = message.ToString().Trim();
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "de")
            {
                speechText = FixGermanSingularOne(speechText);
            }
            Speak(speechText, synchronous);
        }
        catch (Exception ex)
        {
            if (ex.GetType() == typeof(OperationCanceledException))
            {
                log.Debug("Speaking message for " + name + " was interrupted", ex);
            }
            else if (ex.GetType() == typeof(FormatException))
            {
                log.Error("Format error on speaking message " + name + ", trying to reset message to default value ...");
                Preferences.Speech.SetDefaultValues(Preferences.Speech.GetType().GetProperty(name + "Speech")!);
            }
            else
            {
                log.Error("Error on speaking message " + name, ex);
            }
        }
    }

    /// <summary>Performs the generatePreferencesParameter operation.</summary>
    private static void generatePreferencesParameter()
    {
        StarSystem starSystem = new StarSystem(17000000L, "HIP 85639")
        {
            StarClass = "M"
        };
        Ring ring = new Ring("HIP 85639 4 a A Ring", starSystem.Id, 17, RingType.MetalRich, 5891600000L, 10889000L, 15267000L);
        Planet planet = new Planet(17, 17000000L, "HIP 85639 4 a", 1007.0, "High metal content body", true, string.Empty, 0.22, 401.0, "water geysers volcanism", "thin oxygen atmosphere", 9749629.0, null, null, 0.3, 3.4)
        {
            GeologicalCount = 2,
            BiologicalCount = 5,
            StarSystem = starSystem,
            RingsReserveLevel = RingReserveLevel.Major
        };
        planet.TryAddOrUpdateRing(ring, DataSource.Journal);
        Genus genus = new Genus("Stratum", 17000000L, 17, null, "Stratum Tectonicas", "Stratum Tectonicas - Lime")
        {
            ScanCount = 1
        };
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.Welcome)!, (Resources.SpeechOutput_Welcome, new SpeechOutput[1]
        {
            new SpeechOutputCommander("Panostrede")
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.Goodbye)!, (Resources.SpeechOutput_Goodbye, new SpeechOutput[1]
        {
            new SpeechOutputCommander("Panostrede")
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.GeologicalSignals)!, (Resources.SpeechOutput_GeologicalSignals, new SpeechOutput[1]
        {
            new SpeechOutputPlanet(planet)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.BiologicalSignals)!, (Resources.SpeechOutput_BiologicalSignals, new SpeechOutput[1]
        {
            new SpeechOutputPlanet(planet)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.FirstDiscoverySystem)!, (Resources.SpeechOutput_FirstDiscoverySystem, new SpeechOutput[1]
        {
            new SpeechOutputSystem(starSystem)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.FirstDiscoveryBody)!, (Resources.SpeechOutput_FirstDiscoveryBody, new SpeechOutput[1]
        {
            new SpeechOutputBody(planet)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.Terraformable)!, (Resources.SpeechOutput_Terraformable, new SpeechOutput[1]
        {
            new SpeechOutputPlanet(planet)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.Landable)!, (Resources.SpeechOutput_Landable, new SpeechOutput[1]
        {
            new SpeechOutputPlanet(planet)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.ValuableBody)!, (Resources.SpeechOutput_ValuableBody, new SpeechOutput[2]
        {
            new SpeechOutputBody(planet),
            new SpeechOutputCartographicValues(2791256.0, 352687.0)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.RingCount)!, (Resources.SpeechOutput_RingCount, new SpeechOutput[2]
        {
            new SpeechOutputBody(planet),
            new SpeechOutputRingsCount(planet)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.Ring)!, (Resources.SpeechOutput_Ring, new SpeechOutput[2]
        {
            new SpeechOutputRing(ring, planet),
            new SpeechOutputBody(planet)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.ValuableGenusPredicted)!, (Resources.SpeechOutput_ValuableGenusPredicted, new SpeechOutput[2]
        {
            new SpeechOutputPlanet(planet),
            new SpeechOutputSpecies(genus)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.ValuableGeneraPredicted)!, (Resources.SpeechOutput_ValuableGeneraPredicted, new SpeechOutput[2]
        {
            new SpeechOutputPlanet(planet),
            new SpeechOutputValuableSpeciesCount(2)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.LeaveClonalColonyRange)!, (Resources.SpeechOutput_LeaveClonalColonyRange, new SpeechOutput[1]
        {
            new SpeechOutputSpecies(genus)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.EnterClonalColonyRange)!, (Resources.SpeechOutput_EnterClonalColonyRange, new SpeechOutput[1]
        {
            new SpeechOutputSpecies(genus)
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.MatchingClassificationFound)!, (Resources.SpeechOutput_MatchingClassificationFound, new SpeechOutput[2]
        {
            new SpeechOutputPlanet(planet),
            new SpeechOutputPlanetClassification(new PlanetClassification(Resources.PlanetOfInterest_HighGravityLandable))
        }));
        _preferencesParameter.Add(Enum.GetName(typeof(SpeechOutputType), SpeechOutputType.MatchingClassificationsFound)!, (Resources.SpeechOutput_MatchingClassificationsFound, new SpeechOutput[2]
        {
            new SpeechOutputPlanet(planet),
            new SpeechOutputMatchingClassificationsCount(4)
        }));
    }
}
