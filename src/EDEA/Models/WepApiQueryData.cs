using System.Net.Http;
using EDEA.Enums;

namespace EDEA.Models;

public class WepApiQueryData
{
    public WepApiQueryType Type { get; }

    public string QueryString { get; }

    public FormUrlEncodedContent? QueryFormUrlEncodedContent { get; }

    public WepApiQueryData(WepApiQueryType type, string queryString)
    {
        Type = type;
        QueryString = queryString;
    }

    public WepApiQueryData(WepApiQueryType type, FormUrlEncodedContent content)
    {
        Type = type;
        QueryString = string.Empty;
        QueryFormUrlEncodedContent = content;
    }
}
