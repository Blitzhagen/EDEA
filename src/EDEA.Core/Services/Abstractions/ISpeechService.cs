using System;
using System.Collections.Generic;
using EDEA.Models;

namespace EDEA.Services;

/// <summary>
/// Abstraction for text-to-speech output used by platform-independent code.
/// </summary>
public interface ISpeechService
{
    /// <summary>
    /// Gets a value indicating whether speech output is currently active.
    /// </summary>
    bool IsSpeaking { get; }

    /// <summary>
    /// Gets the list of installed voices.
    /// </summary>
    IReadOnlyList<string> InstalledVoices { get; }

    /// <summary>
    /// Occurs when the speech synthesizer state changes.
    /// </summary>
    event EventHandler? StateChanged;

    /// <summary>
    /// Occurs when the list of available voices has been loaded.
    /// </summary>
    event EventHandler? VoicesLoaded;

    /// <summary>
    /// Speaks the welcome message.
    /// </summary>
    /// <param name="commander">The commander data.</param>
    void SpeakWelcome(SpeechOutputCommander commander);

    /// <summary>
    /// Speaks the goodbye message.
    /// </summary>
    /// <param name="commander">The commander data.</param>
    void SpeakGoodbye(SpeechOutputCommander commander);

    /// <summary>
    /// Speaks a first-discovery system notification.
    /// </summary>
    /// <param name="system">The system data.</param>
    void SpeakFirstDiscoverySystem(SpeechOutputSystem system);

    /// <summary>
    /// Speaks a first-discovery body notification.
    /// </summary>
    /// <param name="body">The body data.</param>
    void SpeakFirstDiscoveryBody(SpeechOutputBody body);

    /// <summary>
    /// Speaks a terraformable notification.
    /// </summary>
    /// <param name="planet">The planet data.</param>
    void SpeakTerraformable(SpeechOutputPlanet planet);

    /// <summary>
    /// Speaks a landable notification.
    /// </summary>
    /// <param name="planet">The planet data.</param>
    void SpeakLandable(SpeechOutputPlanet planet);

    /// <summary>
    /// Speaks a geological signals notification.
    /// </summary>
    /// <param name="planet">The planet data.</param>
    void SpeakGeologicalSignals(SpeechOutputPlanet planet);

    /// <summary>
    /// Speaks a biological signals notification.
    /// </summary>
    /// <param name="planet">The planet data.</param>
    void SpeakBiologicalSignals(SpeechOutputPlanet planet);

    /// <summary>
    /// Speaks a valuable body notification.
    /// </summary>
    /// <param name="body">The body data.</param>
    /// <param name="values">The cartographic values.</param>
    void SpeakValuableBody(SpeechOutputBody body, SpeechOutputCartographicValues values);

    /// <summary>
    /// Speaks a valuable genus predicted notification.
    /// </summary>
    /// <param name="planet">The planet data.</param>
    /// <param name="species">The species data.</param>
    void SpeakValuableGenusPredicted(SpeechOutputPlanet planet, SpeechOutputSpecies species);

    /// <summary>
    /// Speaks a valuable genera predicted notification.
    /// </summary>
    /// <param name="planet">The planet data.</param>
    /// <param name="count">The species count data.</param>
    void SpeakValuableGeneraPredicted(SpeechOutputPlanet planet, SpeechOutputValuableSpeciesCount count);

    /// <summary>
    /// Speaks a leave clonal colony range notification.
    /// </summary>
    /// <param name="species">The species data.</param>
    void SpeakLeaveClonalColonyRange(SpeechOutputSpecies species);

    /// <summary>
    /// Speaks an enter clonal colony range notification.
    /// </summary>
    /// <param name="species">The species data.</param>
    void SpeakEnterClonalColonyRange(SpeechOutputSpecies species);

    /// <summary>
    /// Speaks a matching classification found notification.
    /// </summary>
    /// <param name="planet">The planet data.</param>
    /// <param name="classification">The classification data.</param>
    void SpeakMatchingClassificationFound(SpeechOutputPlanet planet, SpeechOutputPlanetClassification classification);

    /// <summary>
    /// Speaks a matching classifications found notification.
    /// </summary>
    /// <param name="planet">The planet data.</param>
    /// <param name="count">The count data.</param>
    void SpeakMatchingClassificationsFound(SpeechOutputPlanet planet, SpeechOutputMatchingClassificationsCount count);

    /// <summary>
    /// Speaks a ring count notification.
    /// </summary>
    /// <param name="body">The body data.</param>
    /// <param name="count">The ring count data.</param>
    void SpeakRingCount(SpeechOutputBody body, SpeechOutputRingsCount count);

    /// <summary>
    /// Speaks a ring notification.
    /// </summary>
    /// <param name="ring">The ring data.</param>
    /// <param name="body">The body data.</param>
    void SpeakRing(SpeechOutputRing ring, SpeechOutputBody body);

    /// <summary>
    /// Speaks the preference preview for the selected output.
    /// </summary>
    /// <param name="label">The output label.</param>
    /// <param name="outputs">The speech output data.</param>
    void SpeakPreferencesSelection(string label, SpeechOutput[] outputs);

    /// <summary>
    /// Stops all queued and active speech output.
    /// </summary>
    void ShutUp();

    /// <summary>
    /// Applies the current speech synthesizer settings.
    /// </summary>
    void UpdateSpeechSynthesizerParameter();

    /// <summary>
    /// Gets the localized label for the specified speech output.
    /// </summary>
    /// <param name="outputName">The output name.</param>
    /// <returns>The localized label.</returns>
    string GetLabelForSpeechOutput(string outputName);

    /// <summary>
    /// Gets the placeholders for the specified speech outputs.
    /// </summary>
    /// <param name="outputs">The speech outputs.</param>
    /// <returns>A dictionary of placeholder names and values.</returns>
    Dictionary<string, string> GetPlaceholdersFromSpeechOutputs(SpeechOutput[] outputs);

    /// <summary>
    /// Gets the example speech outputs for the specified output.
    /// </summary>
    /// <param name="outputName">The output name.</param>
    /// <returns>The example speech outputs.</returns>
    SpeechOutput[] GetExampleSpeechOutputs(string outputName);
}
