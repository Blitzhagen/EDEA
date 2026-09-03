using System.Net;

namespace EDEA.Models;

/// <summary>
/// Represents the base class for web API request parameters.
/// </summary>
public abstract class WebApiParameter
{
    /// <summary>
    /// Gets or sets the HTTP status code of the web API response.
    /// </summary>
    /// <value>The HTTP status code.</value>
    public HttpStatusCode StatusCode { get; set; }
}
