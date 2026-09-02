using System;
using System.Text.RegularExpressions;

using EDEA;

namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for a planet.
/// </summary>
public class SpeechOutputPlanet : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputPlanet"/> class.
    /// </summary>
    /// <param name="planet">The planet to describe.</param>
    public SpeechOutputPlanet(Planet planet)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetType, Globals.GetLocalizedPlanetClass(planet.PlanetClass));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetName, planet.ShortName);
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetDistance, Helpsters.DoubleToHumanRounded(planet.Distance));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetAtmosphere, Globals.GetLocalizedAtmosphereForSpeech(planet.Atmosphere ?? string.Empty));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetTemperature, Helpsters.DoubleToHumanRounded(planet.SurfaceTemperature));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetGravity, Helpsters.DoubleToHumanRounded(planet.Gravity));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetVolcanism, Globals.GetLocalizedVolcanismForSpeech(planet.Volcanism ?? string.Empty));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetRadius, Helpsters.DoubleToHumanRounded(planet.Radius / 1000.0));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetGeoCount, Convert.ToString(planet.GeologicalCount));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetGeoSignalNoun, planet.GeologicalCount == 1 ? Properties.Resources.Speech_GeologicalSignalSingular : Properties.Resources.Speech_GeologicalSignalPlural);
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetBioCount, Convert.ToString(planet.BiologicalCount));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetBioSignalNoun, planet.BiologicalCount == 1 ? Properties.Resources.Speech_BiologicalSignalSingular : Properties.Resources.Speech_BiologicalSignalPlural);
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetOrbitalInclination,
            planet.OrbitalInclination.HasValue
                ? Helpsters.DoubleToHumanRounded(planet.OrbitalInclination.Value)
                : Properties.Resources.Status_Unknown);
    }
}
