using System.Collections.Generic;
using EDEA.Enums;

namespace EDEA.Models;

public class WebApiParameterEdsmSurroundings : WebApiParameterEdsmStarystem
{
    public SurroundingsRadius Radius { get; }

    public List<StarSystem> StarSystems { get; }

    public WebApiParameterEdsmSurroundings(StarSystem starSystem, SurroundingsRadius radius)
        : base(starSystem)
    {
        Radius = radius;
        StarSystems = new List<StarSystem>();
    }
}
