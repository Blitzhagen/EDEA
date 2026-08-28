using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using EDEA.Enums;
using EDEA.Services;
using log4net;

namespace EDEA.Models;

/// <summary>
/// Represents an asynchronous web API request with callback handling.
/// </summary>
public class WebApiRequest : IEquatable<WebApiRequest>
{
    /// <summary>
    /// The logger for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(WebApiRequest));

    /// <summary>
    /// The web API provider that manages this request.
    /// </summary>
    private readonly WebApiProvider _webApiProvider;

    /// <summary>
    /// The API URL.
    /// </summary>
    private readonly string _apiUrl;

    /// <summary>
    /// The query data for the request.
    /// </summary>
    private readonly WepApiQueryData _queryData;

    /// <summary>
    /// The callback that processes the web API response.
    /// </summary>
    private readonly Action<JsonNode?, WebApiParameter, Action<WebApiParameter>, bool> _webApiCallBack;

    /// <summary>
    /// The parameter object passed to the callback.
    /// </summary>
    private readonly WebApiParameter _webApiObject;

    /// <summary>
    /// The callback invoked when the request completes.
    /// </summary>
    private readonly Action<WebApiParameter> _requestCallBack;

    /// <summary>
    /// Whether this request is a follow-up request.
    /// </summary>
    private readonly bool _followUpRequest;

    /// <summary>
    /// Whether to ignore speech output for this request.
    /// </summary>
    private readonly bool _ignoreSpeechOutput;

    /// <summary>
    /// Gets the unique request identifier.
    /// </summary>
    /// <value>The request identifier.</value>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiRequest"/> class.
    /// </summary>
    /// <param name="webApiProvider">The web API provider.</param>
    /// <param name="apiUrl">The API URL.</param>
    /// <param name="wepApiQueryData">The query data.</param>
    /// <param name="webApiCallBack">The callback that processes the response.</param>
    /// <param name="webApiObject">The parameter object.</param>
    /// <param name="requestCallBack">The completion callback.</param>
    /// <param name="ignoreSpeechOutput">Whether to ignore speech output.</param>
    /// <param name="followUpRequest">Whether this is a follow-up request.</param>
    public WebApiRequest(
        WebApiProvider webApiProvider,
        string apiUrl,
        WepApiQueryData wepApiQueryData,
        Action<JsonNode?, WebApiParameter, Action<WebApiParameter>, bool> webApiCallBack,
        WebApiParameter webApiObject,
        Action<WebApiParameter> requestCallBack,
        bool ignoreSpeechOutput,
        bool followUpRequest = false)
    {
        Id = Guid.NewGuid();
        _webApiProvider = webApiProvider;
        Interlocked.Increment(ref WebApiProvider.absoluteRequestsInSession);
        _apiUrl = apiUrl;
        _queryData = wepApiQueryData;
        _webApiCallBack = webApiCallBack;
        _webApiObject = webApiObject;
        _requestCallBack = requestCallBack;
        _followUpRequest = followUpRequest;
        _ignoreSpeechOutput = ignoreSpeechOutput;
        _webApiProvider.registerRequest(this);
        _ = initialize();
    }

    /// <summary>
    /// Gets the request parameter.
    /// </summary>
    /// <value>The request parameter.</value>
    public WebApiParameter Parameter => _webApiObject;

    /// <summary>
    /// Initializes and sends the web API request.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task initialize()
    {
        bool edsmLockHeld = false;
        try
        {
            if (!_followUpRequest && _apiUrl.Contains("edsm.net", StringComparison.OrdinalIgnoreCase))
            {
                await _webApiProvider.edsmSemaphore.WaitAsync().ConfigureAwait(false);
                edsmLockHeld = true;
                var sinceLast = DateTime.UtcNow - _webApiProvider.lastEdsmRequest;
                if (sinceLast < TimeSpan.FromSeconds(1) && _webApiProvider.lastEdsmRequest != DateTime.MinValue)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1) - sinceLast).ConfigureAwait(false);
                }
                _webApiProvider.lastEdsmRequest = DateTime.UtcNow;
            }

            if (!_followUpRequest)
            {
                while (WebApiProvider.activeRequests > _webApiProvider.maxActiveRequests)
                {
                    log.Debug($"Too many active requests, request {(Id)} waiting for {(_webApiProvider.requestWaitDelay)}ms ... Current request count: | {(WebApiProvider.absoluteRequestsInSession)} absolute | {(_webApiProvider.registeredRequestsCount)} registered | {(WebApiProvider.activeRequests)} active |");
                    await Task.Delay(_webApiProvider.requestWaitDelay).ConfigureAwait(false);
                }
            }

            setActive();
            var uriBuilder = new UriBuilder(_apiUrl);
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            if (_queryData.Type == WepApiQueryType.GetQuery)
            {
                uriBuilder.Query = _queryData.QueryString;
                log.Debug($"Sending GET request {(Id)} to web API: {(uriBuilder.Uri)}");
                httpResponseMessage = await _webApiProvider.httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false);
            }
            else if (_queryData.Type == WepApiQueryType.GetPath)
            {
                uriBuilder.Path = _queryData.QueryString;
                log.Debug($"Sending GET request {(Id)} to web API: {(uriBuilder.Uri)}");
                httpResponseMessage = await _webApiProvider.httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false);
            }
            else if (_queryData.Type == WepApiQueryType.PostFormUrlEncodedContent)
            {
                log.Debug($"Sending POST request {(Id)} to web API: {(uriBuilder.Uri)}, content: {(_queryData.QueryFormUrlEncodedContent)}");
                httpResponseMessage = await _webApiProvider.httpClient.PostAsync(uriBuilder.Uri, _queryData.QueryFormUrlEncodedContent!).ConfigureAwait(false);
            }

            _webApiObject.StatusCode = httpResponseMessage.StatusCode;

            if (httpResponseMessage.Headers.TryGetValues("X-Rate-Limit-Remaining", out IEnumerable<string>? values))
            {
                _webApiProvider.xRateRemaining = Convert.ToInt32(values.First(), CultureInfo.InvariantCulture);
            }

            if (_webApiProvider.xRateRemaining < 100)
            {
                log.Warn($"Web API X-Rate-Limit-Remaining value is low: {(_webApiProvider.xRateRemaining)}");
            }

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK && httpResponseMessage.StatusCode != HttpStatusCode.Accepted)
            {
                log.Error($"Http status neither OK nor Accepted, code: {(httpResponseMessage.StatusCode)}, too many requests?");
                _requestCallBack(_webApiObject);
                setInactiveAndUnregister();
                return;
            }

            string json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                JsonNode? jToken = JsonNode.Parse(json);
                if (jToken != null)
                {
                    _webApiCallBack(jToken, _webApiObject, _requestCallBack, _ignoreSpeechOutput);
                }
                else
                {
                    _requestCallBack(_webApiObject);
                }
            }
            catch (Exception exception)
            {
                log.Error($"Error while deserializing JSON object. URL: {(uriBuilder.Uri)}, Function {(_requestCallBack)}", exception);
                _requestCallBack(_webApiObject);
            }

            setInactiveAndUnregister();
        }
        catch (Exception exception)
        {
            log.Error($"Unhandled error in WebApiRequest {(Id)}", exception);
            try
            {
                _requestCallBack(_webApiObject);
            }
            catch
            {
                // ignored
            }
            setInactiveAndUnregister();
        }
        finally
        {
            if (edsmLockHeld)
            {
                _webApiProvider.edsmSemaphore.Release();
            }
        }
    }

    /// <summary>
    /// Marks this request as active.
    /// </summary>
    private void setActive()
    {
        Interlocked.Increment(ref WebApiProvider.activeRequests);
        log.Debug($"Made request {(Id)} active. Current request count: | {(WebApiProvider.absoluteRequestsInSession)} absolute | {(_webApiProvider.registeredRequestsCount)} registered | {(WebApiProvider.activeRequests)} active |");
    }

    /// <summary>
    /// Marks this request as inactive and unregisters it.
    /// </summary>
    private void setInactiveAndUnregister()
    {
        Interlocked.Decrement(ref WebApiProvider.activeRequests);
        log.Debug($"Removed request {(Id)} from active requests. Current request count: | {(WebApiProvider.absoluteRequestsInSession)} absolute | {(_webApiProvider.registeredRequestsCount)} registered | {(WebApiProvider.activeRequests)} active |");
        _webApiProvider.unregisterRequest(this);
    }

    /// <summary>
    /// Determines whether the specified <see cref="WebApiRequest"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The other request.</param>
    /// <returns><see langword="true"/> if the identifiers match; otherwise, <see langword="false"/>.</returns>
    public bool Equals(WebApiRequest? other)
    {
        if (other is null)
        {
            return false;
        }
        return Id.Equals(other.Id);
    }
}
