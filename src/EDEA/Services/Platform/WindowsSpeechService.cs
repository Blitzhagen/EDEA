using System;
using System.Collections.Generic;
using EDEA.Models;
using EDEA.Services;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF implementation of <see cref="ISpeechService"/> that delegates to <see cref="SpeechProvider"/>.
/// </summary>
public sealed class WindowsSpeechService : ISpeechService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsSpeechService"/> class.
    /// </summary>
    public WindowsSpeechService()
    {
        SpeechProvider.SpeechSynthesizerStateChanged += (sender, e) => StateChanged?.Invoke(sender, e);
        SpeechProvider.VoicesLoaded += (sender, e) => VoicesLoaded?.Invoke(sender, e);
    }

    /// <inheritdoc />
    public bool IsSpeaking => SpeechProvider.IsSpeaking;

    /// <inheritdoc />
    public IReadOnlyList<string> InstalledVoices => SpeechProvider.GetInstalledVoices();

    /// <inheritdoc />
    public event EventHandler? StateChanged;

    /// <inheritdoc />
    public event EventHandler? VoicesLoaded;

    /// <inheritdoc />
    public void SpeakWelcome(SpeechOutputCommander commander)
        => SpeechProvider.SpeakWelcome(commander);

    /// <inheritdoc />
    public void SpeakGoodbye(SpeechOutputCommander commander)
        => SpeechProvider.SpeakGoodbye(commander);

    /// <inheritdoc />
    public void SpeakFirstDiscoverySystem(SpeechOutputSystem system)
        => SpeechProvider.SpeakFirstDiscoverySystem(system);

    /// <inheritdoc />
    public void SpeakFirstDiscoveryBody(SpeechOutputBody body)
        => SpeechProvider.SpeakFirstDiscoveryBody(body);

    /// <inheritdoc />
    public void SpeakTerraformable(SpeechOutputPlanet planet)
        => SpeechProvider.SpeakTerraformable(planet);

    /// <inheritdoc />
    public void SpeakLandable(SpeechOutputPlanet planet)
        => SpeechProvider.SpeakLandable(planet);

    /// <inheritdoc />
    public void SpeakGeologicalSignals(SpeechOutputPlanet planet)
        => SpeechProvider.SpeakGeologicalSignals(planet);

    /// <inheritdoc />
    public void SpeakBiologicalSignals(SpeechOutputPlanet planet)
        => SpeechProvider.SpeakBiologicalSignals(planet);

    /// <inheritdoc />
    public void SpeakValuableBody(SpeechOutputBody body, SpeechOutputCartographicValues values)
        => SpeechProvider.SpeakValuableBody(body, values);

    /// <inheritdoc />
    public void SpeakValuableGenusPredicted(SpeechOutputPlanet planet, SpeechOutputSpecies species)
        => SpeechProvider.SpeakValuableGenusPredicted(planet, species);

    /// <inheritdoc />
    public void SpeakValuableGeneraPredicted(SpeechOutputPlanet planet, SpeechOutputValuableSpeciesCount count)
        => SpeechProvider.SpeakValuableGeneraPredicted(planet, count);

    /// <inheritdoc />
    public void SpeakLeaveClonalColonyRange(SpeechOutputSpecies species)
        => SpeechProvider.SpeakLeaveClonalColonyRange(species);

    /// <inheritdoc />
    public void SpeakEnterClonalColonyRange(SpeechOutputSpecies species)
        => SpeechProvider.SpeakEnterClonalColonyRange(species);

    /// <inheritdoc />
    public void SpeakMatchingClassificationFound(SpeechOutputPlanet planet, SpeechOutputPlanetClassification classification)
        => SpeechProvider.SpeakMatchingClassificationFound(planet, classification);

    /// <inheritdoc />
    public void SpeakMatchingClassificationsFound(SpeechOutputPlanet planet, SpeechOutputMatchingClassificationsCount count)
        => SpeechProvider.SpeakMatchingClassificationsFound(planet, count);

    /// <inheritdoc />
    public void SpeakRingCount(SpeechOutputBody body, SpeechOutputRingsCount count)
        => SpeechProvider.SpeakRingCount(body, count);

    /// <inheritdoc />
    public void SpeakRing(SpeechOutputRing ring, SpeechOutputBody body)
        => SpeechProvider.SpeakRing(ring, body);

    /// <inheritdoc />
    public void SpeakPreferencesSelection(string label, SpeechOutput[] outputs)
    {
        if (Enum.TryParse<SpeechProvider.SpeechOutputType>(label, out var type))
        {
            SpeechProvider.SpeakPreferencesSelection(type);
        }
    }

    /// <inheritdoc />
    public void ShutUp()
        => SpeechProvider.ShutUp();

    /// <inheritdoc />
    public void UpdateSpeechSynthesizerParameter()
        => SpeechProvider.UpdateSpeechSynthesizerParameter();

    /// <inheritdoc />
    public string GetLabelForSpeechOutput(string outputName)
    {
        if (Enum.TryParse<SpeechProvider.SpeechOutputType>(outputName, out var type))
        {
            return SpeechProvider.GetLabelForSpeechOutput(type);
        }
        return outputName;
    }

    /// <inheritdoc />
    public Dictionary<string, string> GetPlaceholdersFromSpeechOutputs(SpeechOutput[] outputs)
        => SpeechProvider.GetPlaceholdersFromSpeechOutputs(outputs);

    /// <inheritdoc />
    public SpeechOutput[] GetExampleSpeechOutputs(string outputName)
    {
        if (Enum.TryParse<SpeechProvider.SpeechOutputType>(outputName, out var type))
        {
            return SpeechProvider.GetExampleSpeechOutputs(type);
        }
        return Array.Empty<SpeechOutput>();
    }
}
