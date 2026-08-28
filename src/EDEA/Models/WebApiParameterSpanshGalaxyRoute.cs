using System.Text.Json.Nodes;

namespace EDEA.Models;

public class WebApiParameterSpanshGalaxyRoute : WebApiParameter
{
    public JsonArray Jumps { get; set; }

    public int CalculationTime { get; }

    public int RequestDelay { get; }

    public int RequestCount { get; set; }

    public WebApiParameterSpanshGalaxyRoute(int calculationTime, int requestDelay)
    {
        Jumps = new JsonArray();
        CalculationTime = calculationTime;
        RequestDelay = requestDelay;
        RequestCount = 0;
    }
}
