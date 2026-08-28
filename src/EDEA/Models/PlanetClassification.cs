using System;
using System.Collections.Generic;
using System.Linq;
using EDEA;

namespace EDEA.Models;

public class PlanetClassification : ICloneable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsActive { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<string> PlanetClasses { get; set; } = new();
    public string PlanetClassesAsString => string.Join(";", PlanetClasses);

    public List<string> Atmospheres { get; set; } = new();
    public string AtmospheresAsString => string.Join(";", Atmospheres);

    public List<string> Volcanisms { get; set; } = new();
    public string VolcanismsAsString => string.Join(";", Volcanisms);

    public List<string> StarClasses { get; set; } = new();
    public string StarClassesAsString => string.Join(";", StarClasses);

    public List<string> RingTypes { get; set; } = new();
    public List<string> RingReserveLevels { get; set; } = new();

    public double? GravityMin { get; set; }
    public double? GravityMax { get; set; }
    public double? TemperatureMin { get; set; }
    public double? TemperatureMax { get; set; }
    public double? DistanceMin { get; set; }
    public double? DistanceMax { get; set; }
    public double? RadiusMin { get; set; }
    public double? RadiusMax { get; set; }
    public bool? Landable { get; set; }

    public long? RingWidthMin { get; set; }
    public long? RingWidthMax { get; set; }
    public long? RingsTotalWidthMin { get; set; }
    public long? RingsTotalWidthMax { get; set; }
    public double? RingDensityMin { get; set; }
    public double? RingDensityMax { get; set; }

    public string ParentPlanetClassificationId { get; set; } = null!;

    public double? OrbitalInclinationMin { get; set; }
    public double? OrbitalInclinationMax { get; set; }

    public string RingTypesAsString => string.Join(";", RingTypes);
    public string RingReserveLevelsAsString => string.Join(";", RingReserveLevels);

    public PlanetClassification(string name, bool isActive = false)
    {
        Name = name;
        IsActive = isActive;
    }

    private PlanetClassification()
    {
    }

    public object Clone() => MemberwiseClone();

    public bool HasMatchingPlanetClass(Body body)
    {
        if (body is not Planet planet)
            return false;
        if (PlanetClasses.Count == 0)
            return true;
        return PlanetClasses.Any(c => c.Equals(planet.PlanetClass, StringComparison.OrdinalIgnoreCase));
    }

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

    public bool HasMatchingStarClass(Planet planet)
    {
        if (planet == null || planet.ParentStar == null || string.IsNullOrEmpty(planet.ParentStar.StarType))
            return false;
        if (StarClasses.Count == 0)
            return true;
        return StarClasses.Any(c => c.Equals(planet.ParentStar.StarType, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasMatchingRingType(Ring ring)
    {
        if (RingTypes.Count == 0)
            return true;
        return RingTypes.Any(c => Helpsters.GetRingType(c) == ring.Type);
    }

    public bool HasMatchingReserveLevel(Body body)
    {
        if (body == null)
            return false;
        if (RingReserveLevels.Count == 0)
            return true;
        return RingReserveLevels.Any(c => c.Equals(body.RingsReserveLevel.ToString(), StringComparison.OrdinalIgnoreCase));
    }

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
