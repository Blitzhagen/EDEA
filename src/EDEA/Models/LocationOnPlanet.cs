namespace EDEA.Models;

public class LocationOnPlanet
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public double PlanetRadius { get; set; }

    public LocationOnPlanet()
    {
    }

    public LocationOnPlanet(double longitude, double latitude, double planetRadius)
    {
        Longitude = longitude;
        Latitude = latitude;
        PlanetRadius = planetRadius;
    }

    public override string ToString()
    {
        return $"{Longitude}, {Latitude} (r: {PlanetRadius})";
    }
}
