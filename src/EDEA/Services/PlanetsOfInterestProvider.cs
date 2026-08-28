using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

/// <summary>Represents the PlanetsOfInterestProvider class.</summary>
public class PlanetsOfInterestProvider
{
    /// <summary>The instance field.</summary>
    private static PlanetsOfInterestProvider? instance;

    /// <summary>The log field.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(PlanetsOfInterestProvider));

    /// <summary>The _starSystemProvider field.</summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>Gets the planetClassifications.</summary>
    /// <value>A List<PlanetClassification> value.</value>
    private List<PlanetClassification> planetClassifications => Preferences.PlanetsOfInterest.PlanetClassifications;

    /// <summary>Initializes a new instance of the PlanetsOfInterestProvider class.</summary>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    private PlanetsOfInterestProvider(StarSystemProvider starSystemProvider)
    {
        _starSystemProvider = starSystemProvider;
        _starSystemProvider.RegisterProvider(this);
    }

    /// <summary>Performs the Instance operation.</summary>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    /// <returns>A PlanetsOfInterestProvider result.</returns>
    public static PlanetsOfInterestProvider Instance(StarSystemProvider starSystemProvider)
    {
        if (instance == null)
        {
            instance = new PlanetsOfInterestProvider(starSystemProvider);
        }
        return instance;
    }

    /// <summary>Sets PlanetClassifications.</summary>
    /// <param name="planetClassifications">The List<PlanetClassification> value of the planetClassifications parameter.</param>
    public void SetPlanetClassifications(List<PlanetClassification> planetClassifications)
    {
        if (planetClassifications != null)
        {
            Preferences.PlanetsOfInterest.PlanetClassifications = planetClassifications;
        }
        _starSystemProvider.FindMatchingClassificationsForSystem(_starSystemProvider.CurrentSystem, ignoreSpeechOutput: true);
        _starSystemProvider.FindMatchingClassificationsForCurrentRouteAndSurroundings();
    }

    /// <summary>Retrieves ClonedPlanetClassifications.</summary>
    /// <returns>A List<PlanetClassification> result.</returns>
    public List<PlanetClassification> GetClonedPlanetClassifications()
    {
        List<PlanetClassification> clones = new List<PlanetClassification>();
        foreach (PlanetClassification planetClassification in planetClassifications)
        {
            clones.Add((PlanetClassification)planetClassification.Clone());
        }
        return clones;
    }

    /// <summary>Determines whether IsBodyOfInterest.</summary>
    /// <param name="body">The Body value of the body parameter.</param>
    /// <returns>A bool result.</returns>
    public bool IsBodyOfInterest(Body body)
    {
        if (body is not Planet planet)
        {
            return false;
        }
        foreach (PlanetClassification planetClassification in planetClassifications)
        {
            if (planetClassification.IsActive && checkForMatchingPlanetClassification(planet, planetClassification))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>Performs the MatchingClassificationNames operation.</summary>
    /// <param name="body">The Body value of the body parameter.</param>
    /// <returns>A List<string> result.</returns>
    public List<string> MatchingClassificationNames(Body body)
    {
        List<string> names = new List<string>();
        if (body is not Planet planet)
        {
            return names;
        }
        foreach (PlanetClassification planetClassification in planetClassifications)
        {
            if (planetClassification.IsActive && checkForMatchingPlanetClassification(planet, planetClassification))
            {
                names.Add(planetClassification.Name);
            }
        }
        return names;
    }

    /// <summary>Performs the FindAndSetMatchingPlanetClassifications operation.</summary>
    /// <param name="planet">The Planet value of the planet parameter.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task FindAndSetMatchingPlanetClassifications(Planet planet)
    {
        await Task.Run(delegate
        {
            planet.MatchingPlanetClassifications.Clear();
            foreach (PlanetClassification planetClassification in planetClassifications)
            {
                if (planetClassification.IsActive)
                {
                    if (planetClassification.ParentPlanetClassificationId != null)
                    {
                        if (planet.ParentPlanet == null)
                        {
                            continue;
                        }
                        PlanetClassification? parentClassification = planetClassifications.FirstOrDefault(pc => pc.Id == planetClassification.ParentPlanetClassificationId);
                        if (parentClassification == null || !checkForMatchingPlanetClassification(planet.ParentPlanet, parentClassification))
                        {
                            continue;
                        }
                    }
                    if (checkForMatchingPlanetClassification(planet, planetClassification))
                    {
                        planet.MatchingPlanetClassifications.Add(planetClassification);
                        log.Debug($"######################## Matching classification '{planetClassification.Name}' for planet '{planet.Name}' ({planet.Id}):");
                        if (planetClassification.Landable.HasValue && planetClassification.Landable == planet.IsLandable)
                        {
                            log.Debug($"########################   Planet landable: {planet.IsLandable} | Classification requirement: {planetClassification.Landable}");
                        }
                        if (planetClassification.GravityMin.HasValue || planetClassification.GravityMax.HasValue)
                        {
                            log.Debug($"########################   Planet gravity: {planet.Gravity.ToString("n0")} | Classification range: {planetClassification.GravityMin?.ToString("n7")} - {planetClassification.GravityMax?.ToString("n7")} g");
                        }
                        if (planetClassification.TemperatureMin.HasValue || planetClassification.TemperatureMax.HasValue)
                        {
                            log.Debug($"########################   Planet temperature: {planet.SurfaceTemperature.ToString("n0")} | Classification range: {planetClassification.TemperatureMin?.ToString("n0")} - {planetClassification.TemperatureMax?.ToString("n0")} K");
                        }
                        if (planetClassification.DistanceMin.HasValue || planetClassification.DistanceMax.HasValue)
                        {
                            log.Debug($"########################   Planet distance: {planet.Distance.ToString("n0")} | Classification range: {planetClassification.DistanceMin?.ToString("n0")} - {planetClassification.DistanceMax?.ToString("n0")} Ls");
                        }
                        if (planetClassification.RadiusMin.HasValue || planetClassification.RadiusMax.HasValue)
                        {
                            log.Debug($"########################   Planet radius: {planet.Radius.ToString("n0")} | Classification range: {planetClassification.RadiusMin?.ToString("n0")} - {planetClassification.RadiusMax?.ToString("n0")} m");
                        }
                        if (planetClassification.PlanetClasses.Count > 0)
                        {
                            log.Debug("########################   Planet class: " + planet.PlanetClass + " | Classification planet classes: " + string.Join(", ", planetClassification.PlanetClasses));
                        }
                        if (planetClassification.Atmospheres.Count > 0)
                        {
                            log.Debug("########################   Planet atmosphere: " + planet.Atmosphere + " | Classification atmospheres: " + string.Join(", ", planetClassification.Atmospheres));
                        }
                        if (planetClassification.Volcanisms.Count > 0)
                        {
                            log.Debug("########################   Planet volcanism: " + planet.Volcanism + " | Classification volcanisms: " + string.Join(", ", planetClassification.Volcanisms));
                        }
                        if (planetClassification.StarClasses.Count > 0)
                        {
                            log.Debug("########################   Planet star class: " + planet.ParentStar?.StarType + " | Classification star classes: " + string.Join(", ", planetClassification.StarClasses));
                        }
                        if (planetClassification.RingTypes.Count > 0)
                        {
                            log.Debug("########################   Planet ring type(s): " + string.Join(", ", planet.Rings.Values.Select((Ring ring) => ring.Type).ToList()) + " | Classification ring types: " + string.Join(", ", planetClassification.RingTypes));
                        }
                        if (planetClassification.RingReserveLevels.Count > 0)
                        {
                            log.Debug($"########################   Planet ring reserve level: {planet.RingsReserveLevel} | Classification ring reserve levels: {string.Join(", ", planetClassification.RingReserveLevels)}");
                        }
                        if (planetClassification.RingsTotalWidthMin.HasValue || planetClassification.RingsTotalWidthMax.HasValue)
                        {
                            log.Debug($"########################   Rings total width: {planet.RingsTotalWidth.ToString("n0")} | Classification range: {planetClassification.RingsTotalWidthMin?.ToString("n0")} - {planetClassification.RingsTotalWidthMax?.ToString("n0")} m");
                        }
                        if (planetClassification.RingWidthMin.HasValue || planetClassification.RingWidthMax.HasValue)
                        {
                            log.Debug($"########################   Ring width (min/max): {planet.Rings.Min((KeyValuePair<string, Ring> r) => r.Value.Width):n0}/{planet.Rings.Max((KeyValuePair<string, Ring> r) => r.Value.Width):n0} | Classification range: {planetClassification.RingWidthMin?.ToString("n0")} - {planetClassification.RingWidthMax?.ToString("n0")} m");
                        }
                        if (planetClassification.RingDensityMin.HasValue || planetClassification.RingDensityMax.HasValue)
                        {
                            log.Debug($"########################   Ring density (min/max): {planet.Rings.Min((KeyValuePair<string, Ring> r) => r.Value.Density):n0}/{planet.Rings.Max((KeyValuePair<string, Ring> r) => r.Value.Density):n0} | Classification range: {planetClassification.RingDensityMin?.ToString("n0")} - {planetClassification.RingDensityMax?.ToString("n0")} m");
                        }
                    }
                }
            }
        });
    }

    /// <summary>Performs the FindMatchingPlanetClassifications operation.</summary>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task FindMatchingPlanetClassifications(StarSystem starSystem)
    {
        if (starSystem == null || starSystem.Bodies.Count < 2)
        {
            return;
        }
        foreach (Body body in starSystem.Bodies.Values)
        {
            if (body.Type == BodyType.Planet)
            {
                await FindAndSetMatchingPlanetClassifications((Planet)body);
            }
        }
    }

    /// <summary>Performs the checkForMatchingPlanetClassification operation.</summary>
    /// <param name="planet">The Planet value of the planet parameter.</param>
    /// <param name="planetClassification">The PlanetClassification value of the planetClassification parameter.</param>
    /// <returns>A bool result.</returns>
    private bool checkForMatchingPlanetClassification(Planet planet, PlanetClassification planetClassification)
    {
        if (planetClassification.Landable.HasValue && planetClassification.Landable != planet.IsLandable)
        {
            return false;
        }
        if (planetClassification.GravityMin.HasValue && planetClassification.GravityMin > planet.Gravity)
        {
            return false;
        }
        if (planetClassification.GravityMax.HasValue && planetClassification.GravityMax < planet.Gravity)
        {
            return false;
        }
        if (planetClassification.TemperatureMin.HasValue && planetClassification.TemperatureMin > planet.SurfaceTemperature)
        {
            return false;
        }
        if (planetClassification.TemperatureMax.HasValue && planetClassification.TemperatureMax < planet.SurfaceTemperature)
        {
            return false;
        }
        if (planetClassification.DistanceMin.HasValue && planetClassification.DistanceMin > planet.Distance)
        {
            return false;
        }
        if (planetClassification.DistanceMax.HasValue && planetClassification.DistanceMax < planet.Distance)
        {
            return false;
        }
        if (planetClassification.RadiusMin.HasValue && planetClassification.RadiusMin > planet.Radius)
        {
            return false;
        }
        if (planetClassification.RadiusMax.HasValue && planetClassification.RadiusMax < planet.Radius)
        {
            return false;
        }
        if (planetClassification.PlanetClasses.Count > 0 && !planetClassification.HasMatchingPlanetClass(planet))
        {
            return false;
        }
        if (planetClassification.Atmospheres.Count > 0 && !planetClassification.HasMatchingAtmosphere(planet))
        {
            return false;
        }
        if (planetClassification.Volcanisms.Count > 0 && !planetClassification.HasMatchingVolcanism(planet))
        {
            return false;
        }
        if (planetClassification.StarClasses.Count > 0 && !planetClassification.HasMatchingStarClass(planet))
        {
            return false;
        }
        if (planetClassification.RingsTotalWidthMin.HasValue && (!planet.HasRings || planetClassification.RingsTotalWidthMin > planet.RingsTotalWidth))
        {
            return false;
        }
        if (planetClassification.RingsTotalWidthMax.HasValue && (!planet.HasRings || planetClassification.RingsTotalWidthMax < planet.RingsTotalWidth))
        {
            return false;
        }
        if (planetClassification.RingReserveLevels.Count > 0 && !planetClassification.HasMatchingReserveLevel(planet))
        {
            return false;
        }
        bool hasMatchingRing = planet.Rings.Values.Any(ring =>
        {
            if (planetClassification.RingWidthMin.HasValue && planetClassification.RingWidthMin > ring.Width)
            {
                return false;
            }
            if (planetClassification.RingWidthMax.HasValue && planetClassification.RingWidthMax < ring.Width)
            {
                return false;
            }
            if (planetClassification.RingDensityMin.HasValue && planetClassification.RingDensityMin > ring.Density)
            {
                return false;
            }
            if (planetClassification.RingDensityMax.HasValue && planetClassification.RingDensityMax < ring.Density)
            {
                return false;
            }
            return planetClassification.RingTypes.Count <= 0 || planetClassification.HasMatchingRingType(ring);
        });
        if ((planetClassification.RingWidthMin.HasValue || planetClassification.RingWidthMax.HasValue || planetClassification.RingDensityMin.HasValue || planetClassification.RingDensityMax.HasValue || planetClassification.RingTypes.Count > 0) && !hasMatchingRing)
        {
            return false;
        }
        if (planetClassification.OrbitalInclinationMax.HasValue && (!planet.OrbitalInclination.HasValue || planetClassification.OrbitalInclinationMax < planet.OrbitalInclination))
        {
            return false;
        }
        if (planetClassification.OrbitalInclinationMin.HasValue && (!planet.OrbitalInclination.HasValue || planetClassification.OrbitalInclinationMin > planet.OrbitalInclination))
        {
            return false;
        }
        return true;
    }
}
