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

public class WebApiRequest : IEquatable<WebApiRequest>
{
    private static readonly ILog log = LogManager.GetLogger(typeof(WebApiRequest));

    private readonly WebApiProvider _webApiProvider;
    private readonly string _apiUrl;
    private readonly WepApiQueryData _queryData;
    private readonly Action<JsonNode?, WebApiParameter, Action<WebApiParameter>, bool> _webApiCallBack;
    private readonly WebApiParameter _webApiObject;
    private readonly Action<WebApiParameter> _requestCallBack;
    private readonly bool _followUpRequest;
    private readonly bool _ignoreSpeechOutput;

    public Guid Id { get; }

    public WebApiParameter Parameter => _webApiObject;

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

    private void setActive()
    {
        Interlocked.Increment(ref WebApiProvider.activeRequests);
        log.Debug($"Made request {(Id)} active. Current request count: | {(WebApiProvider.absoluteRequestsInSession)} absolute | {(_webApiProvider.registeredRequestsCount)} registered | {(WebApiProvider.activeRequests)} active |");
    }

    private void setInactiveAndUnregister()
    {
        Interlocked.Decrement(ref WebApiProvider.activeRequests);
        log.Debug($"Removed request {(Id)} from active requests. Current request count: | {(WebApiProvider.absoluteRequestsInSession)} absolute | {(_webApiProvider.registeredRequestsCount)} registered | {(WebApiProvider.activeRequests)} active |");
        _webApiProvider.unregisterRequest(this);
    }

    public bool Equals(WebApiRequest? other)
    {
        if (other is null)
        {
            return false;
        }
        return Id.Equals(other.Id);
    }
}
