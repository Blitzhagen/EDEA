namespace EDEA.Models;

/// <summary>
/// Represents the parameters for an EDSM star system web API request.
/// </summary>
public class WebApiParameterEdsmStarystem : WebApiParameter
{
    /// <summary>
    /// Gets the star system.
    /// </summary>
    /// <value>The star system.</value>
    public StarSystem StarSystem { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiParameterEdsmStarystem"/> class.
    /// </summary>
    /// <param name="starSystem">The star system.</param>
    public WebApiParameterEdsmStarystem(StarSystem starSystem)
    {
        StarSystem = starSystem;
    }
}
