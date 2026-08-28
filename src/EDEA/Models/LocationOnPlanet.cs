namespace EDEA.Models;

/// <summary>
/// Represents a geographic location on a planet.
/// </summary>
public class LocationOnPlanet
{
    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    /// <value>The longitude in degrees.</value>
    public double Longitude { get; set; }

    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    /// <value>The latitude in degrees.</value>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the planet radius.
    /// </summary>
    /// <value>The planet radius.</value>
    public double PlanetRadius { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationOnPlanet"/> class.
    /// </summary>
    public LocationOnPlanet()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationOnPlanet"/> class.
    /// </summary>
    /// <param name="longitude">The longitude in degrees.</param>
    /// <param name="latitude">The latitude in degrees.</param>
    /// <param name="planetRadius">The planet radius.</param>
    public LocationOnPlanet(double longitude, double latitude, double planetRadius)
    {
        Longitude = longitude;
        Latitude = latitude;
        PlanetRadius = planetRadius;
    }

    /// <summary>
    /// Returns a string representation of the location.
    /// </summary>
    /// <returns>A string describing the longitude, latitude and planet radius.</returns>
    public override string ToString()
    {
        return $"{Longitude}, {Latitude} (r: {PlanetRadius})";
    }
}
