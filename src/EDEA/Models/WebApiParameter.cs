using System.Net;

namespace EDEA.Models;

public abstract class WebApiParameter
{
    public HttpStatusCode StatusCode { get; set; }
}
