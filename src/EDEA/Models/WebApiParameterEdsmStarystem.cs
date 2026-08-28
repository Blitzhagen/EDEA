namespace EDEA.Models;

public class WebApiParameterEdsmStarystem : WebApiParameter
{
    public StarSystem StarSystem { get; }

    public WebApiParameterEdsmStarystem(StarSystem starSystem)
    {
        StarSystem = starSystem;
    }
}
