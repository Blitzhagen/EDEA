using System;
using System.Collections.Generic;
using System.Linq;
using EDEA;

namespace EDEA.Models;

/// <summary>
/// Represents a filterable classification for planets.
/// </summary>
public class PlanetClassification : ICloneable
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    /// <value>The unique identifier.</value>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets a value indicating whether this classification is active.
    /// </summary>
    /// <value><see langword="true"/> if active; otherwise, <see langword="false"/>.</value>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The classification name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of matching planet classes.
    /// </summary>
    /// <value>The list of planet classes.</value>
    public List<string> PlanetClasses { get; set; } = new();

    /// <summary>
    /// Gets the planet classes as a semicolon-separated string.
    /// </summary>
    /// <value>The joined planet classes.</value>
    public string PlanetClassesAsString => string.Join(";", PlanetClasses);

    /// <summary>
    /// Gets or sets the list of matching atmospheres.
    /// </summary>
    /// <value>The list of atmospheres.</value>
    public List<string> Atmospheres { get; set; } = new();

    /// <summary>
    /// Gets the atmospheres as a semicolon-separated string.
    /// </summary>
    /// <value>The joined atmospheres.</value>
    public string AtmospheresAsString => string.Join(";", Atmospheres);

    /// <summary>
    /// Gets or sets the list of matching volcanisms.
    /// </summary>
    /// <value>The list of volcanisms.</value>
    public List<string> Volcanisms { get; set; } = new();

    /// <summary>
    /// Gets the volcanisms as a semicolon-separated string.
    /// </summary>
    /// <value>The joined volcanisms.</value>
    public string VolcanismsAsString => string.Join(";", Volcanisms);

    /// <summary>
    /// Gets or sets the list of matching star classes.
    /// </summary>
    /// <value>The list of star classes.</value>
    public List<string> StarClasses { get; set; } = new();

    /// <summary>
    /// Gets the star classes as a semicolon-separated string.
    /// </summary>
    /// <value>The joined star classes.</value>
    public string StarClassesAsString => string.Join(";", StarClasses);

    /// <summary>
    /// Gets or sets the list of matching ring types.
    /// </summary>
    /// <value>The list of ring types.</value>
    public List<string> RingTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of matching ring reserve levels.
    /// </summary>
    /// <value>The list of ring reserve levels.</value>
    public List<string> RingReserveLevels { get; set; } = new();

    /// <summary>
    /// Gets or sets the minimum gravity.
    /// </summary>
    /// <value>The minimum gravity, or <see langword="null"/> if not specified.</value>
    public double? GravityMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum gravity.
    /// </summary>
    /// <value>The maximum gravity, or <see langword="null"/> if not specified.</value>
    public double? GravityMax { get; set; }

    /// <summary>
    /// Gets or sets the minimum temperature.
    /// </summary>
    /// <value>The minimum temperature, or <see langword="null"/> if not specified.</value>
    public double? TemperatureMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum temperature.
    /// </summary>
    /// <value>The maximum temperature, or <see langword="null"/> if not specified.</value>
    public double? TemperatureMax { get; set; }

    /// <summary>
    /// Gets or sets the minimum distance.
    /// </summary>
    /// <value>The minimum distance, or <see langword="null"/> if not specified.</value>
    public double? DistanceMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum distance.
    /// </summary>
    /// <value>The maximum distance, or <see langword="null"/> if not specified.</value>
    public double? DistanceMax { get; set; }

    /// <summary>
    /// Gets or sets the minimum radius.
    /// </summary>
    /// <value>The minimum radius, or <see langword="null"/> if not specified.</value>
    public double? RadiusMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum radius.
    /// </summary>
    /// <value>The maximum radius, or <see langword="null"/> if not specified.</value>
    public double? RadiusMax { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the planet must be landable.
    /// </summary>
    /// <value><see langword="true"/> if landable required; <see langword="false"/> if not; <see langword="null"/> if unspecified.</value>
    public bool? Landable { get; set; }

    /// <summary>
    /// Gets or sets the minimum ring width.
    /// </summary>
    /// <value>The minimum ring width, or <see langword="null"/> if not specified.</value>
    public long? RingWidthMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum ring width.
    /// </summary>
    /// <value>The maximum ring width, or <see langword="null"/> if not specified.</value>
    public long? RingWidthMax { get; set; }

    /// <summary>
    /// Gets or sets the minimum total ring width.
    /// </summary>
    /// <value>The minimum total ring width, or <see langword="null"/> if not specified.</value>
    public long? RingsTotalWidthMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum total ring width.
    /// </summary>
    /// <value>The maximum total ring width, or <see langword="null"/> if not specified.</value>
    public long? RingsTotalWidthMax { get; set; }

    /// <summary>
    /// Gets or sets the minimum ring density.
    /// </summary>
    /// <value>The minimum ring density, or <see langword="null"/> if not specified.</value>
    public double? RingDensityMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum ring density.
    /// </summary>
    /// <value>The maximum ring density, or <see langword="null"/> if not specified.</value>
    public double? RingDensityMax { get; set; }

    /// <summary>
    /// Gets or sets the parent planet classification identifier.
    /// </summary>
    /// <value>The parent classification identifier.</value>
    public string ParentPlanetClassificationId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the minimum orbital inclination.
    /// </summary>
    /// <value>The minimum orbital inclination, or <see langword="null"/> if not specified.</value>
    public double? OrbitalInclinationMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum orbital inclination.
    /// </summary>
    /// <value>The maximum orbital inclination, or <see langword="null"/> if not specified.</value>
    public double? OrbitalInclinationMax { get; set; }

    /// <summary>
    /// Gets the ring types as a semicolon-separated string.
    /// </summary>
    /// <value>The joined ring types.</value>
    public string RingTypesAsString => string.Join(";", RingTypes);

    /// <summary>
    /// Gets the ring reserve levels as a semicolon-separated string.
    /// </summary>
    /// <value>The joined ring reserve levels.</value>
    public string RingReserveLevelsAsString => string.Join(";", RingReserveLevels);

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanetClassification"/> class.
    /// </summary>
    /// <param name="name">The classification name.</param>
    /// <param name="isActive">Whether the classification is active.</param>
    public PlanetClassification(string name, bool isActive = false)
    {
        Name = name;
        IsActive = isActive;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanetClassification"/> class for serialization.
    /// </summary>
    private PlanetClassification()
    {
    }

    /// <summary>
    /// Creates a shallow copy of this classification.
    /// </summary>
    /// <returns>The cloned classification.</returns>
    public object Clone() => MemberwiseClone();

    /// <summary>
    /// Determines whether the specified body has a matching planet class.
    /// </summary>
    /// <param name="body">The body to check.</param>
    /// <returns><see langword="true"/> if the planet class matches or no filter is set; otherwise, <see langword="false"/>.</returns>
    public bool HasMatchingPlanetClass(Body body)
    {
        if (body is not Planet planet)
            return false;
        if (PlanetClasses.Count == 0)
            return true;
        return PlanetClasses.Any(c => c.Equals(planet.PlanetClass, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines whether the specified body has a matching atmosphere.
    /// </summary>
    /// <param name="body">The body to check.</param>
    /// <returns><see langword="true"/> if the atmosphere matches or no filter is set; otherwise, <see langword="false"/>.</returns>
    public bool HasMatchingAtmosphere(Body body)
    {
        if (body is not Planet planet)
            return false;
        if (Atmospheres.Count == 0)
            return true;
        if (string.IsNullOrEmpty(planet.Atmosphere))
            return false;
        return Atmospheres.Any(c => c.Equals(planet.Atmosphere, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines whether the specified body has a matching volcanism.
    /// </summary>
    /// <param name="body">The body to check.</param>
    /// <returns><see langword="true"/> if the volcanism matches or no filter is set; otherwise, <see langword="false"/>.</returns>
    public bool HasMatchingVolcanism(Body body)
    {
        if (body is not Planet planet)
            return false;
        if (Volcanisms.Count == 0)
            return true;
        if (string.IsNullOrEmpty(planet.Volcanism))
            return false;
        return Volcanisms.Any(c => c.Equals(planet.Volcanism, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines whether the specified planet's parent star has a matching star class.
    /// </summary>
    /// <param name="planet">The planet to check.</param>
    /// <returns><see langword="true"/> if the star class matches or no filter is set; otherwise, <see langword="false"/>.</returns>
    public bool HasMatchingStarClass(Planet planet)
    {
        if (planet == null || planet.ParentStar == null || string.IsNullOrEmpty(planet.ParentStar.StarType))
            return false;
        if (StarClasses.Count == 0)
            return true;
        return StarClasses.Any(c => c.Equals(planet.ParentStar.StarType, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines whether the specified ring has a matching ring type.
    /// </summary>
    /// <param name="ring">The ring to check.</param>
    /// <returns><see langword="true"/> if the ring type matches or no filter is set; otherwise, <see langword="false"/>.</returns>
    public bool HasMatchingRingType(Ring ring)
    {
        if (RingTypes.Count == 0)
            return true;
        return RingTypes.Any(c => Helpsters.GetRingType(c) == ring.Type);
    }

    /// <summary>
    /// Determines whether the specified body has a matching ring reserve level.
    /// </summary>
    /// <param name="body">The body to check.</param>
    /// <returns><see langword="true"/> if the reserve level matches or no filter is set; otherwise, <see langword="false"/>.</returns>
    public bool HasMatchingReserveLevel(Body body)
    {
        if (body == null)
            return false;
        if (RingReserveLevels.Count == 0)
            return true;
        return RingReserveLevels.Any(c => c.Equals(body.RingsReserveLevel.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines whether the specified planet matches all active criteria.
    /// </summary>
    /// <param name="body">The body to check.</param>
    /// <returns><see langword="true"/> if the planet matches; otherwise, <see langword="false"/>.</returns>
    public bool IsMatch(Body body)
    {
        if (!IsActive)
            return false;
        if (body is not Planet planet)
            return false;

        if (!HasMatchingPlanetClass(body))
            return false;
        if (!HasMatchingAtmosphere(body))
            return false;
        if (!HasMatchingVolcanism(body))
            return false;

        if (Landable.HasValue && planet.IsLandable != Landable.Value)
            return false;
        if (GravityMin.HasValue && planet.Gravity < GravityMin.Value)
            return false;
        if (GravityMax.HasValue && planet.Gravity > GravityMax.Value)
            return false;
        if (TemperatureMin.HasValue && planet.SurfaceTemperature < TemperatureMin.Value)
            return false;
        if (TemperatureMax.HasValue && planet.SurfaceTemperature > TemperatureMax.Value)
            return false;
        if (DistanceMin.HasValue && planet.Distance < DistanceMin.Value)
            return false;
        if (DistanceMax.HasValue && planet.Distance > DistanceMax.Value)
            return false;
        if (RadiusMin.HasValue && planet.Radius < RadiusMin.Value)
            return false;
        if (RadiusMax.HasValue && planet.Radius > RadiusMax.Value)
            return false;

        return true;
    }
}
