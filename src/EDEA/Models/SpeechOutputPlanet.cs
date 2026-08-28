using System;
using System.Text.RegularExpressions;

using EDEA;

namespace EDEA.Models;

public class SpeechOutputPlanet : SpeechOutput
{
    public SpeechOutputPlanet(Planet planet)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetType, planet.PlanetClass);
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetName, planet.ShortName);
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetDistance, Helpsters.DoubleToHumanRounded(planet.Distance));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetAtmosphere, string.IsNullOrEmpty(planet.Atmosphere)
            ? "No"
            : Regex.Replace(planet.Atmosphere.Replace("atmosphere", "").Trim(), "^[a-z]", c => c.Value.ToUpper()));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetTemperature, Helpsters.DoubleToHumanRounded(planet.SurfaceTemperature));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetGravity, Helpsters.DoubleToHumanRounded(planet.Gravity));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetVolcanism, string.IsNullOrEmpty(planet.Volcanism)
            ? "No"
            : Regex.Replace(planet.Volcanism.Replace("volcanism", "").Trim(), "^[a-z]", c => c.Value.ToUpper()));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetRadius, Helpsters.DoubleToHumanRounded(planet.Radius / 1000.0));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetGeoCount, Convert.ToString(planet.GeologicalCount));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetBioCount, Convert.ToString(planet.BiologicalCount));
        Placeholders.Add(SpeechOutputPlaceholderKeys.PlanetOrbitalInclination,
            planet.OrbitalInclination.HasValue
                ? Helpsters.DoubleToHumanRounded(planet.OrbitalInclination.Value)
                : "Unknown");
    }
}
