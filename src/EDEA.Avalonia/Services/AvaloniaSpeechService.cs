using System;
using System.Collections.Generic;
using EDEA.Models;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

#pragma warning disable CS0067

/// <summary>
/// Avalonia/cross-platform stub implementation of <see cref="ISpeechService"/>.
/// </summary>
public sealed class AvaloniaSpeechService : ISpeechService
{
    /// <inheritdoc />
    public bool IsSpeaking => false;

    /// <inheritdoc />
    public IReadOnlyList<string> InstalledVoices => new List<string>();

    /// <inheritdoc />
    public event EventHandler? StateChanged;

    /// <inheritdoc />
    public event EventHandler? VoicesLoaded;

    /// <inheritdoc />
    public void SpeakWelcome(SpeechOutputCommander commander) { }

    /// <inheritdoc />
    public void SpeakGoodbye(SpeechOutputCommander commander) { }

    /// <inheritdoc />
    public void SpeakFirstDiscoverySystem(SpeechOutputSystem system) { }

    /// <inheritdoc />
    public void SpeakFirstDiscoveryBody(SpeechOutputBody body) { }

    /// <inheritdoc />
    public void SpeakTerraformable(SpeechOutputPlanet planet) { }

    /// <inheritdoc />
    public void SpeakLandable(SpeechOutputPlanet planet) { }

    /// <inheritdoc />
    public void SpeakGeologicalSignals(SpeechOutputPlanet planet) { }

    /// <inheritdoc />
    public void SpeakBiologicalSignals(SpeechOutputPlanet planet) { }

    /// <inheritdoc />
    public void SpeakValuableBody(SpeechOutputBody body, SpeechOutputCartographicValues values) { }

    /// <inheritdoc />
    public void SpeakValuableGenusPredicted(SpeechOutputPlanet planet, SpeechOutputSpecies species) { }

    /// <inheritdoc />
    public void SpeakValuableGeneraPredicted(SpeechOutputPlanet planet, SpeechOutputValuableSpeciesCount count) { }

    /// <inheritdoc />
    public void SpeakLeaveClonalColonyRange(SpeechOutputSpecies species) { }

    /// <inheritdoc />
    public void SpeakEnterClonalColonyRange(SpeechOutputSpecies species) { }

    /// <inheritdoc />
    public void SpeakMatchingClassificationFound(SpeechOutputPlanet planet, SpeechOutputPlanetClassification classification) { }

    /// <inheritdoc />
    public void SpeakMatchingClassificationsFound(SpeechOutputPlanet planet, SpeechOutputMatchingClassificationsCount count) { }

    /// <inheritdoc />
    public void SpeakRingCount(SpeechOutputBody body, SpeechOutputRingsCount count) { }

    /// <inheritdoc />
    public void SpeakRing(SpeechOutputRing ring, SpeechOutputBody body) { }

    /// <inheritdoc />
    public void SpeakPreferencesSelection(string label, SpeechOutput[] outputs) { }

    /// <inheritdoc />
    public void ShutUp() { }

    /// <inheritdoc />
    public void UpdateSpeechSynthesizerParameter() { }

    /// <inheritdoc />
    public string GetLabelForSpeechOutput(string outputName) => outputName;

    /// <inheritdoc />
    public Dictionary<string, string> GetPlaceholdersFromSpeechOutputs(SpeechOutput[] outputs) => new();

    /// <inheritdoc />
    public SpeechOutput[] GetExampleSpeechOutputs(string outputName) => Array.Empty<SpeechOutput>();
}
