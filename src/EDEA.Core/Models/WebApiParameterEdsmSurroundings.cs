using System.Collections.Generic;
using EDEA.Enums;

namespace EDEA.Models;

/// <summary>
/// Represents the parameters for an EDSM surroundings web API request.
/// </summary>
public class WebApiParameterEdsmSurroundings : WebApiParameterEdsmStarystem
{
    /// <summary>
    /// Gets the search radius.
    /// </summary>
    /// <value>The search radius.</value>
    public SurroundingsRadius Radius { get; }

    /// <summary>
    /// Gets the list of surrounding star systems.
    /// </summary>
    /// <value>The surrounding star systems.</value>
    public List<StarSystem> StarSystems { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiParameterEdsmSurroundings"/> class.
    /// </summary>
    /// <param name="starSystem">The center star system.</param>
    /// <param name="radius">The search radius.</param>
    public WebApiParameterEdsmSurroundings(StarSystem starSystem, SurroundingsRadius radius)
        : base(starSystem)
    {
        Radius = radius;
        StarSystems = new List<StarSystem>();
    }
}
