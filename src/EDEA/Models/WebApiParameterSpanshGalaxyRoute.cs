using System.Text.Json.Nodes;

namespace EDEA.Models;

/// <summary>
/// Represents the parameters for a Spansh galaxy route web API request.
/// </summary>
public class WebApiParameterSpanshGalaxyRoute : WebApiParameter
{
    /// <summary>
    /// Gets or sets the route jumps.
    /// </summary>
    /// <value>The route jumps.</value>
    public JsonArray Jumps { get; set; }

    /// <summary>
    /// Gets the calculation time.
    /// </summary>
    /// <value>The calculation time.</value>
    public int CalculationTime { get; }

    /// <summary>
    /// Gets the request delay.
    /// </summary>
    /// <value>The request delay.</value>
    public int RequestDelay { get; }

    /// <summary>
    /// Gets or sets the request count.
    /// </summary>
    /// <value>The request count.</value>
    public int RequestCount { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiParameterSpanshGalaxyRoute"/> class.
    /// </summary>
    /// <param name="calculationTime">The calculation time.</param>
    /// <param name="requestDelay">The request delay.</param>
    public WebApiParameterSpanshGalaxyRoute(int calculationTime, int requestDelay)
    {
        Jumps = new JsonArray();
        CalculationTime = calculationTime;
        RequestDelay = requestDelay;
        RequestCount = 0;
    }
}
