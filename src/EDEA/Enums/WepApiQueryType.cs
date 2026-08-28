namespace EDEA.Enums;

/// <summary>
/// Defines the types of web API queries that can be executed.
/// </summary>
public enum WepApiQueryType
{
    /// <summary>
    /// A GET query request.
    /// </summary>
    GetQuery,

    /// <summary>
    /// A GET path request.
    /// </summary>
    GetPath,

    /// <summary>
    /// A POST request with form URL-encoded content.
    /// </summary>
    PostFormUrlEncodedContent
}
