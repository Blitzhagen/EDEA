using System.Net.Http;
using EDEA.Enums;

namespace EDEA.Models;

/// <summary>
/// Represents the data for a web API query.
/// </summary>
public class WepApiQueryData
{
    /// <summary>
    /// Gets the query type.
    /// </summary>
    /// <value>The query type.</value>
    public WepApiQueryType Type { get; }

    /// <summary>
    /// Gets the query string.
    /// </summary>
    /// <value>The query string.</value>
    public string QueryString { get; }

    /// <summary>
    /// Gets the form URL encoded content.
    /// </summary>
    /// <value>The form content, or <see langword="null"/> for GET requests.</value>
    public FormUrlEncodedContent? QueryFormUrlEncodedContent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WepApiQueryData"/> class for a GET request.
    /// </summary>
    /// <param name="type">The query type.</param>
    /// <param name="queryString">The query string or path.</param>
    public WepApiQueryData(WepApiQueryType type, string queryString)
    {
        Type = type;
        QueryString = queryString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WepApiQueryData"/> class for a POST request.
    /// </summary>
    /// <param name="type">The query type.</param>
    /// <param name="content">The form URL encoded content.</param>
    public WepApiQueryData(WepApiQueryType type, FormUrlEncodedContent content)
    {
        Type = type;
        QueryString = string.Empty;
        QueryFormUrlEncodedContent = content;
    }
}
