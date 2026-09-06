using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using EDEA.Enums;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

/// <summary>Represents the WebApiProvider class.</summary>
public class WebApiProvider
{
    /// <summary>The log field.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(WebApiProvider));

    /// <summary>The absoluteRequestsInSession field.</summary>
    internal static int absoluteRequestsInSession;
    /// <summary>The activeRequests field.</summary>
    internal static int activeRequests;

    /// <summary>The instance field.</summary>
    private static WebApiProvider? instance;
    /// <summary>The _starSystemProvider field.</summary>
    private readonly StarSystemProvider _starSystemProvider;
    /// <summary>The _registeredRequests field.</summary>
    private readonly ConcurrentDictionary<Guid, WebApiRequest> _registeredRequests = new();
    /// <summary>The spanshCancellationOfGalaxyRouteCalculationRequested field.</summary>
    private bool spanshCancellationOfGalaxyRouteCalculationRequested;
    /// <summary>The _loadingCount field.</summary>
    private long _loadingCount;

    /// <summary>Gets or sets the isLoading.</summary>
    /// <value>A bool value.</value>
    public bool isLoading { get; private set; }

    /// <summary>Gets the AbsoluteRequestsInSession.</summary>
    /// <value>A int value.</value>
    public int AbsoluteRequestsInSession => absoluteRequestsInSession;

    /// <summary>Gets the httpClient.</summary>
    /// <value>A HttpClient value.</value>
    public HttpClient httpClient { get; }

    /// <summary>Gets or sets the xRateRemaining.</summary>
    /// <value>A int value.</value>
    public int xRateRemaining { get; set; } = 700;

    /// <summary>Gets the maxActiveRequests.</summary>
    /// <value>A int value.</value>
    public int maxActiveRequests { get; } = 17;

    /// <summary>Gets the requestWaitDelay.</summary>
    /// <value>A int value.</value>
    public int requestWaitDelay { get; } = 700;

    /// <summary>The edsmSemaphore field. Allows up to 4 concurrent EDSM requests.</summary>
    internal readonly SemaphoreSlim edsmSemaphore = new(4, 4);

    /// <summary>Gets the registeredRequestsCount.</summary>
    /// <value>A int value.</value>
    public int registeredRequestsCount => _registeredRequests.Count;

    /// <summary>Occurs when the WebApiLoadingStatusChanged event is raised.</summary>
    public event WebApiLoadingStatusChangedEventHandler WebApiLoadingStatusChanged = delegate { };

    /// <summary>Initializes a new instance of the WebApiProvider class.</summary>
    /// <param name="httpClient">The HttpClient value of the httpClient parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    private WebApiProvider(HttpClient httpClient, StarSystemProvider starSystemProvider)
    {
        _starSystemProvider = starSystemProvider;
        _starSystemProvider.RegisterProvider(this);
        this.httpClient = httpClient;
        xRateRemaining = 700;
        absoluteRequestsInSession = 0;
        maxActiveRequests = 17;
        requestWaitDelay = 700;
        activeRequests = 0;
    }

    /// <summary>Performs the Instance operation.</summary>
    /// <param name="httpClient">The HttpClient value of the httpClient parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    /// <returns>A WebApiProvider result.</returns>
    public static WebApiProvider Instance(HttpClient httpClient, StarSystemProvider starSystemProvider)
    {
        if (instance == null)
        {
            instance = new WebApiProvider(httpClient, starSystemProvider);
        }

        return instance!;
    }

    /// <summary>Performs the SpanshRequestBasicSystemData operation.</summary>
    /// <param name="starSystemNameQuery">The string value of the starSystemNameQuery parameter.</param>
    /// <param name="starSystems">The ObservableCollection<StarSystem> value of the starSystems parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    public void SpanshRequestBasicSystemData(string starSystemNameQuery, ObservableCollection<StarSystem> starSystems, Action<WebApiParameter> requestCallBack)
    {
        var webApiObject = new WebApiParameterSpanshBasicSystemData(starSystems);
        const string apiUrl = "https://spansh.co.uk/api/systems/field_values/system_names";
        string queryData = "q=" + starSystemNameQuery;
        _ = new WebApiRequest(this, apiUrl, new WepApiQueryData(WepApiQueryType.GetQuery, queryData), onSpanshResponseBasicSystemData, webApiObject, requestCallBack, ignoreSpeechOutput: true);
    }

    /// <summary>Performs the onSpanshResponseBasicSystemData operation.</summary>
    /// <param name="jToken">The JsonNode? value of the jToken parameter.</param>
    /// <param name="webApiParameter">The WebApiParameter value of the webApiParameter parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    /// <param name="ignoreSpeechOutput">The bool value of the ignoreSpeechOutput parameter.</param>
    private void onSpanshResponseBasicSystemData(JsonNode? jToken, WebApiParameter webApiParameter, Action<WebApiParameter> requestCallBack, bool ignoreSpeechOutput)
    {
        try
        {
            if (webApiParameter is not WebApiParameterSpanshBasicSystemData parameterSpanshSystemNames)
            {
                requestCallBack(webApiParameter);
                return;
            }

            var jSystems = Helpsters.ConvertJObjectValue<JsonArray?>(jToken as JsonObject ?? new JsonObject(), "min_max", new JsonArray());
            foreach (var jSystem in jSystems!.OfType<JsonObject>())
            {
                parameterSpanshSystemNames.StarSystems.Add(new StarSystem(Helpsters.ConvertJObjectValue(jSystem, "id64", 0L), Helpsters.ConvertJObjectValue<string>(jSystem, "name")));
            }
            requestCallBack(parameterSpanshSystemNames);
        }
        catch (Exception exception)
        {
            log.Error($"Error while updating Spansh system names information, Spansh Response: {(jToken)}", exception);
        }
    }

    /// <summary>Performs the SpanshRequestGalaxyRouteCalculation operation.</summary>
    /// <param name="sourceSystem">The StarSystem value of the sourceSystem parameter.</param>
    /// <param name="targetSystem">The StarSystem value of the targetSystem parameter.</param>
    /// <param name="calculationTime">The int value of the calculationTime parameter.</param>
    /// <param name="requestDelay">The int value of the requestDelay parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    public void SpanshRequestGalaxyRouteCalculation(StarSystem sourceSystem, StarSystem targetSystem, int calculationTime, int requestDelay, Action<WebApiParameter> requestCallBack)
    {
        spanshCancellationOfGalaxyRouteCalculationRequested = false;
        var webApiObject = new WebApiParameterSpanshGalaxyRoute(calculationTime, requestDelay);
        const string apiUrl = "https://spansh.co.uk/api/generic/route";
        IEnumerable<KeyValuePair<string, string?>> nameValueCollection = new KeyValuePair<string, string?>[]
        {
            new KeyValuePair<string, string?>("source", Convert.ToString(sourceSystem.Id)),
            new KeyValuePair<string, string?>("destination", Convert.ToString(targetSystem.Id)),
            new KeyValuePair<string, string?>("is_supercharged", Convert.ToString(Convert.ToInt32(_starSystemProvider.CurrentShip?.JetConeBoost))),
            new KeyValuePair<string, string?>("use_supercharge", Convert.ToString(Convert.ToInt32(Preferences.Spansh.UseSupercharge))),
            new KeyValuePair<string, string?>("use_injections", Convert.ToString(Convert.ToInt32(Preferences.Spansh.UseInjections))),
            new KeyValuePair<string, string?>("exclude_secondary", Convert.ToString(Convert.ToInt32(Preferences.Spansh.ExcludeSecondary))),
            new KeyValuePair<string, string?>("refuel_every_scoopable", Convert.ToString(Convert.ToInt32(Preferences.Spansh.RefuelEveryScoopable))),
            new KeyValuePair<string, string?>("fuel_power", Convert.ToString(_starSystemProvider.CurrentShip?.FrameShiftDrive?.FuelPower, CultureInfo.InvariantCulture)),
            new KeyValuePair<string, string?>("fuel_multiplier", Convert.ToString(_starSystemProvider.CurrentShip?.FrameShiftDrive?.FuelMultiplier, CultureInfo.InvariantCulture)),
            new KeyValuePair<string, string?>("optimal_mass", Convert.ToString(_starSystemProvider.CurrentShip?.FrameShiftDrive?.OptimalMass, CultureInfo.InvariantCulture)),
            new KeyValuePair<string, string?>("base_mass", Convert.ToString(_starSystemProvider.CurrentShip?.BaseMass, CultureInfo.InvariantCulture)),
            new KeyValuePair<string, string?>("tank_size", Convert.ToString(_starSystemProvider.CurrentShip?.MainFuelCapacity, CultureInfo.InvariantCulture)),
            new KeyValuePair<string, string?>("internal_tank_size", Convert.ToString(_starSystemProvider.CurrentShip?.ReserveFuelCapacity, CultureInfo.InvariantCulture)),
            new KeyValuePair<string, string?>("reserve_size", "0"),
            new KeyValuePair<string, string?>("max_fuel_per_jump", Convert.ToString(_starSystemProvider.CurrentShip?.FrameShiftDrive?.MaxFuelPerJump, CultureInfo.InvariantCulture)),
            new KeyValuePair<string, string?>("range_boost", (_starSystemProvider.CurrentShip?.GuardianFsdBooster == null) ? "0" : Convert.ToString(_starSystemProvider.CurrentShip?.GuardianFsdBooster?.JumpBoost, CultureInfo.InvariantCulture)),
            new KeyValuePair<string, string?>("ship_build", _starSystemProvider.CurrentShip?.SLEF?.ToJsonString() ?? string.Empty),
            new KeyValuePair<string, string?>("max_time", Convert.ToString(calculationTime)),
            new KeyValuePair<string, string?>("cargo", Convert.ToString(_starSystemProvider.CurrentShip?.CargoCount)),
            new KeyValuePair<string, string?>("algorithm", Enum.GetName(typeof(SpanshRoutingAlgorithm), Preferences.Spansh.RoutingAlgorithm)?.ToLower() ?? string.Empty),
            new KeyValuePair<string, string?>("supercharge_multiplier", Convert.ToString(_starSystemProvider.CurrentShip?.FrameShiftDrive?.JumpBoostMultiplier)),
            new KeyValuePair<string, string?>("injection_multiplier", "2")
        };
        _ = new WebApiRequest(this, apiUrl, new WepApiQueryData(WepApiQueryType.PostFormUrlEncodedContent, new FormUrlEncodedContent(nameValueCollection)), onSpanshRequestGalaxyRouteCalculation, webApiObject, requestCallBack, ignoreSpeechOutput: true);
    }

    /// <summary>Performs the SpanshRequestNeutronRouteCalculation operation.</summary>
    /// <param name="sourceSystem">The StarSystem value of the sourceSystem parameter.</param>
    /// <param name="targetSystem">The StarSystem value of the targetSystem parameter.</param>
    /// <param name="range">The double value of the range parameter.</param>
    /// <param name="efficiency">The int value of the efficiency parameter.</param>
    /// <param name="superchargeMultiplier">The double value of the superchargeMultiplier parameter.</param>
    /// <param name="requestDelay">The int value of the requestDelay parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    public void SpanshRequestNeutronRouteCalculation(StarSystem sourceSystem, StarSystem targetSystem, double range, int efficiency, double superchargeMultiplier, int requestDelay, Action<WebApiParameter> requestCallBack)
    {
        spanshCancellationOfGalaxyRouteCalculationRequested = false;
        var webApiObject = new WebApiParameterSpanshGalaxyRoute(0, requestDelay);
        const string apiUrl = "https://spansh.co.uk/api/route";
        string queryData =
            $"from={Uri.EscapeDataString(sourceSystem.Name)}&" +
            $"to={Uri.EscapeDataString(targetSystem.Name)}&" +
            $"range={Uri.EscapeDataString(Convert.ToString(range, CultureInfo.InvariantCulture))}&" +
            $"efficiency={efficiency}&" +
            $"supercharge_multiplier={Uri.EscapeDataString(Convert.ToString(superchargeMultiplier, CultureInfo.InvariantCulture))}";
        log.Info($"Requesting Spansh neutron route: {apiUrl}?{queryData}");
        _ = new WebApiRequest(this, apiUrl, new WepApiQueryData(WepApiQueryType.GetQuery, queryData), onSpanshRequestNeutronRouteCalculation, webApiObject, requestCallBack, ignoreSpeechOutput: true);
    }

    /// <summary>Performs the SpanshRequestCancellationOfGalaxyRouteCalculation operation.</summary>
    public void SpanshRequestCancellationOfGalaxyRouteCalculation()
    {
        spanshCancellationOfGalaxyRouteCalculationRequested = true;
    }

    /// <summary>Performs the onSpanshRequestNeutronRouteCalculation operation.</summary>
    /// <param name="jToken">The JsonNode? value of the jToken parameter.</param>
    /// <param name="webApiParameter">The WebApiParameter value of the webApiParameter parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    /// <param name="ignoreSpeechOutput">The bool value of the ignoreSpeechOutput parameter.</param>
    private void onSpanshRequestNeutronRouteCalculation(JsonNode? jToken, WebApiParameter webApiParameter, Action<WebApiParameter> requestCallBack, bool ignoreSpeechOutput)
    {
        if (jToken is not JsonObject jObject)
        {
            requestCallBack(webApiParameter);
            return;
        }

        string jobId = Helpsters.ConvertJObjectValue<string>(jObject, "job");
        string jobStatus = Helpsters.ConvertJObjectValue<string>(jObject, "status");
        if (string.IsNullOrEmpty(jobId) || string.IsNullOrEmpty(jobStatus))
        {
            log.Error($"Spansh neutron job has unknown parameter: jobId {(jobId)}, status {(jobStatus)}");
            requestCallBack(webApiParameter);
            return;
        }

        if (webApiParameter is not WebApiParameterSpanshGalaxyRoute webApiParameterSpanshGalaxyRoute)
        {
            log.Error($"Spansh neutron job has invalid web API parameter: type is {(webApiParameter.GetType())} instead of {(typeof(WebApiParameterSpanshGalaxyRoute))}");
            requestCallBack(webApiParameter);
            return;
        }

        webApiParameterSpanshGalaxyRoute.RequestCount++;
        if (webApiParameterSpanshGalaxyRoute.RequestCount == 1)
        {
            log.Info($"Spansh neutron job {jobId} started (status: {jobStatus}); result can be compared at https://www.spansh.co.uk/plotter/results/{jobId}");
        }
        switch (webApiParameterSpanshGalaxyRoute.StatusCode)
        {
            case HttpStatusCode.Accepted:
                Task.Run(async () =>
                {
                    log.Debug($"Got job {(jobId)} with status {(jobStatus)} from spansh neutron API, waiting for {(webApiParameterSpanshGalaxyRoute.RequestDelay)} ms before requesting again ...");
                    await Task.Delay(webApiParameterSpanshGalaxyRoute.RequestDelay);
                    string apiUrl = "https://spansh.co.uk";
                    string queryData = "/api/results/" + jobId;
                    if (spanshCancellationOfGalaxyRouteCalculationRequested)
                    {
                        spanshCancellationOfGalaxyRouteCalculationRequested = false;
                    }
                    else
                    {
                        _ = new WebApiRequest(this, apiUrl, new WepApiQueryData(WepApiQueryType.GetPath, queryData), onSpanshRequestNeutronRouteCalculation, webApiParameter, requestCallBack, ignoreSpeechOutput: true);
                    }
                });
                break;
            case HttpStatusCode.OK:
                {
                    var result = Helpsters.ConvertJObjectValue<JsonObject?>(jObject, "result");
                    if (result == null)
                    {
                        log.Error($"Spansh neutron job {(jobId)} with status {(jobStatus)} returned an invalid response: {(result)}");
                    }
                    else
                    {
                        var jumps = BuildNeutronJumps(result);
                        if (jumps != null && jumps.Count > 1)
                        {
                            webApiParameterSpanshGalaxyRoute.Jumps = jumps;
                            log.Info($"Spansh neutron job {jobId} finished with {jumps.Count} jumps:");
                            int jumpIndex = 0;
                            foreach (JsonNode? node in jumps)
                            {
                                if (node is JsonObject jump)
                                {
                                    log.Info($"  Jump {jumpIndex}: {Helpsters.ConvertJObjectValue<string>(jump, "name")}, " +
                                        $"{Helpsters.ConvertJObjectValue(jump, "distance", 0.0):F2} Ly" +
                                        (Helpsters.ConvertJObjectValue(jump, "has_neutron", false) ? ", neutron" : string.Empty) +
                                        (Helpsters.ConvertJObjectValue(jump, "is_refuel", false) ? ", refuel" : string.Empty) +
                                        (Helpsters.ConvertJObjectValue(jump, "is_scoopable", false) ? ", scoopable" : string.Empty));
                                }
                                jumpIndex++;
                            }
                        }
                    }
                    requestCallBack(webApiParameterSpanshGalaxyRoute);
                    break;
                }
            default:
                requestCallBack(webApiParameter);
                break;
        }
    }

    /// <summary>Performs the onSpanshRequestGalaxyRouteCalculation operation.</summary>
    /// <param name="jToken">The JsonNode? value of the jToken parameter.</param>
    /// <param name="webApiParameter">The WebApiParameter value of the webApiParameter parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    /// <param name="ignoreSpeechOutput">The bool value of the ignoreSpeechOutput parameter.</param>
    private void onSpanshRequestGalaxyRouteCalculation(JsonNode? jToken, WebApiParameter webApiParameter, Action<WebApiParameter> requestCallBack, bool ignoreSpeechOutput)
    {
        if (jToken is not JsonObject jObject)
        {
            requestCallBack(webApiParameter);
            return;
        }

        string jobId = Helpsters.ConvertJObjectValue<string>(jObject, "job");
        string jobStatus = Helpsters.ConvertJObjectValue<string>(jObject, "status");
        if (string.IsNullOrEmpty(jobId) || string.IsNullOrEmpty(jobStatus))
        {
            log.Error($"Spansh job has unknown parameter: jobId {(jobId)}, status {(jobStatus)}");
            requestCallBack(webApiParameter);
            return;
        }

        if (webApiParameter is not WebApiParameterSpanshGalaxyRoute webApiParameterSpanshGalaxyRoute)
        {
            log.Error($"Spansh job has invalid web API parameter: type is {(webApiParameter.GetType())} instead of {(typeof(WebApiParameterSpanshGalaxyRoute))}");
            requestCallBack(webApiParameter);
            return;
        }

        webApiParameterSpanshGalaxyRoute.RequestCount++;
        switch (webApiParameterSpanshGalaxyRoute.StatusCode)
        {
            case HttpStatusCode.Accepted:
                Task.Run(async () =>
                {
                    log.Debug($"Got job {(jobId)} with status {(jobStatus)} from spansh API, waiting for {(webApiParameterSpanshGalaxyRoute.RequestDelay)} ms before requesting again ...");
                    await Task.Delay(webApiParameterSpanshGalaxyRoute.RequestDelay);
                    string apiUrl = "https://spansh.co.uk";
                    string queryData = "/api/results/" + jobId;
                    if (spanshCancellationOfGalaxyRouteCalculationRequested)
                    {
                        spanshCancellationOfGalaxyRouteCalculationRequested = false;
                    }
                    else
                    {
                        _ = new WebApiRequest(this, apiUrl, new WepApiQueryData(WepApiQueryType.GetPath, queryData), onSpanshRequestGalaxyRouteCalculation, webApiParameter, requestCallBack, ignoreSpeechOutput: true);
                    }
                });
                break;
            case HttpStatusCode.OK:
                {
                    var result = Helpsters.ConvertJObjectValue<JsonObject?>(jObject, "result");
                    if (result == null)
                    {
                        log.Error($"Spansh job {(jobId)} with status {(jobStatus)} returned an invalid response: {(result)}");
                    }
                    else
                    {
                        var jumps = Helpsters.ConvertJObjectValue<JsonArray?>(result, "jumps");
                        if (jumps != null)
                        {
                            webApiParameterSpanshGalaxyRoute.Jumps = jumps;
                        }
                    }
                    requestCallBack(webApiParameterSpanshGalaxyRoute);
                    break;
                }
            default:
                requestCallBack(webApiParameter);
                break;
        }
    }

    /// <summary>Performs the EnterLoading operation.</summary>
    /// <param name="webApiParameter">The WebApiParameter? value of the webApiParameter parameter.</param>
    private void EnterLoading(WebApiParameter? webApiParameter = null)
    {
        if (Interlocked.Increment(ref _loadingCount) == 1)
        {
            isLoading = true;
            WebApiLoadingStatusChanged?.Invoke(this, webApiParameter);
        }
    }

    /// <summary>Performs the ExitLoading operation.</summary>
    /// <param name="webApiParameter">The WebApiParameter? value of the webApiParameter parameter.</param>
    private void ExitLoading(WebApiParameter? webApiParameter = null)
    {
        if (Interlocked.Decrement(ref _loadingCount) == 0)
        {
            isLoading = false;
            WebApiLoadingStatusChanged?.Invoke(this, webApiParameter);
        }
    }

    /// <summary>Performs the registerRequest operation.</summary>
    /// <param name="webApiRequest">The WebApiRequest value of the webApiRequest parameter.</param>
    /// <param name="force">The bool value of the force parameter.</param>
    internal void registerRequest(WebApiRequest webApiRequest, bool force = false)
    {
        if (_registeredRequests.Count == 0)
        {
            EnterLoading(webApiRequest?.Parameter);
        }

        if (_registeredRequests.TryAdd(webApiRequest!.Id, webApiRequest!))
        {
            log.Debug($"Registered request {(webApiRequest.Id)}. Current request count: | {(absoluteRequestsInSession)} absolute | {(registeredRequestsCount)} registered | {(activeRequests)} active |");
        }
        else
        {
            log.Error($"Can not register request {(webApiRequest.Id)}. It exists already!");
        }
    }

    /// <summary>Performs the unregisterRequest operation.</summary>
    /// <param name="webApiRequest">The WebApiRequest value of the webApiRequest parameter.</param>
    internal void unregisterRequest(WebApiRequest webApiRequest)
    {
        if (_registeredRequests.TryRemove(webApiRequest.Id, out _))
        {
            log.Debug($"Unregistered request {(webApiRequest.Id)}. Current request count: | {(absoluteRequestsInSession)} absolute | {(registeredRequestsCount)} registered | {(activeRequests)} active |");
        }
        else
        {
            log.Error($"Can not unregister request {(webApiRequest.Id)}!");
        }

        webApiRequest = null!;
        if (_registeredRequests.Count == 0)
        {
            ExitLoading(null);
        }
    }

    /// <summary>Performs the SendGetAsync operation.</summary>
    /// <param name="uri">The Uri value of the uri parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<JsonNode?> representing the asynchronous operation.</returns>
    private async Task<JsonNode?> SendGetAsync(Uri uri, CancellationToken ct = default)
    {
        EnterLoading();
        Interlocked.Increment(ref absoluteRequestsInSession);

        try
        {
            while (activeRequests >= maxActiveRequests)
            {
                log.Debug($"Too many active requests, waiting for {(requestWaitDelay)}ms ... Current request count: | {(absoluteRequestsInSession)} absolute | {(registeredRequestsCount)} registered | {(activeRequests)} active |");
                await Task.Delay(requestWaitDelay, ct).ConfigureAwait(false);
            }

            Interlocked.Increment(ref activeRequests);
            try
            {
                using var response = await httpClient.GetAsync(uri, ct).ConfigureAwait(false);

                if (response.Headers.TryGetValues("X-Rate-Limit-Remaining", out IEnumerable<string>? values))
                {
                    xRateRemaining = Convert.ToInt32(values.First(), CultureInfo.InvariantCulture);
                }

                if (xRateRemaining < 100)
                {
                    log.Warn($"Web API X-Rate-Limit-Remaining value is low: {(xRateRemaining)}");
                }

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                {
                    log.Error($"Http status neither OK nor Accepted, code: {(response.StatusCode)}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                return JsonNode.Parse(json);
            }
            finally
            {
                Interlocked.Decrement(ref activeRequests);
            }
        }
        catch (Exception exception)
        {
            log.Error($"Error while sending GET request to {(uri)}", exception);
            return null;
        }
        finally
        {
            ExitLoading();
        }
    }

    /// <summary>Performs the SendPostAsync operation.</summary>
    /// <param name="uri">The Uri value of the uri parameter.</param>
    /// <param name="content">The FormUrlEncodedContent value of the content parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<JsonNode?> representing the asynchronous operation.</returns>
    private async Task<JsonNode?> SendPostAsync(Uri uri, FormUrlEncodedContent content, CancellationToken ct = default)
    {
        EnterLoading();
        Interlocked.Increment(ref absoluteRequestsInSession);

        try
        {
            while (activeRequests >= maxActiveRequests)
            {
                log.Debug($"Too many active requests, waiting for {(requestWaitDelay)}ms ... Current request count: | {(absoluteRequestsInSession)} absolute | {(registeredRequestsCount)} registered | {(activeRequests)} active |");
                await Task.Delay(requestWaitDelay, ct).ConfigureAwait(false);
            }

            Interlocked.Increment(ref activeRequests);
            try
            {
                using var response = await httpClient.PostAsync(uri, content, ct).ConfigureAwait(false);

                if (response.Headers.TryGetValues("X-Rate-Limit-Remaining", out IEnumerable<string>? values))
                {
                    xRateRemaining = Convert.ToInt32(values.First(), CultureInfo.InvariantCulture);
                }

                if (xRateRemaining < 100)
                {
                    log.Warn($"Web API X-Rate-Limit-Remaining value is low: {(xRateRemaining)}");
                }

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                {
                    log.Error($"Http status neither OK nor Accepted, code: {(response.StatusCode)}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                return JsonNode.Parse(json);
            }
            finally
            {
                Interlocked.Decrement(ref activeRequests);
            }
        }
        catch (Exception exception)
        {
            log.Error($"Error while sending POST request to {(uri)}", exception);
            return null;
        }
        finally
        {
            ExitLoading();
        }
    }

    /// <summary>Performs the EdsmRequestSurroundingStarSystemsInformation operation.</summary>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    /// <param name="radius">The SurroundingsRadius value of the radius parameter.</param>
    public void EdsmRequestSurroundingStarSystemsInformation(StarSystem starSystem, Action<WebApiParameter> requestCallBack, SurroundingsRadius radius = SurroundingsRadius.Close)
    {
        var webApiObject = new WebApiParameterEdsmSurroundings(starSystem, radius);
        log.Debug($"Requesting the closest {(25)} surrounding systems within a radius of {((int)radius)} Ly for star system '{(starSystem.Name)}' ({(starSystem.Id)}) from EDSM");
        const string apiUrl = "https://www.edsm.net/api-v1/sphere-systems";
        var queryData = $"systemName={Uri.EscapeDataString(starSystem.Name)}&showId=1&showPrimaryStar=1&showCoordinates=1&showInformation=1&radius={(int)radius}";
        _ = new WebApiRequest(
            this,
            apiUrl,
            new WepApiQueryData(WepApiQueryType.GetQuery, queryData),
            edsmUpdateSurroundingStarSystemsInformation,
            webApiObject,
            requestCallBack,
            ignoreSpeechOutput: true);
    }

    /// <summary>Performs the edsmUpdateSurroundingStarSystemsInformation operation.</summary>
    /// <param name="jToken">The JsonNode? value of the jToken parameter.</param>
    /// <param name="webApiParameter">The WebApiParameter value of the webApiParameter parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    /// <param name="ignoreSpeechOutput">The bool value of the ignoreSpeechOutput parameter.</param>
    private void edsmUpdateSurroundingStarSystemsInformation(JsonNode? jToken, WebApiParameter webApiParameter, Action<WebApiParameter> requestCallBack, bool ignoreSpeechOutput)
    {
        var webApiParameterEdsmSurroundings = webApiParameter as WebApiParameterEdsmSurroundings;
        if (webApiParameterEdsmSurroundings is null)
        {
            requestCallBack(webApiParameter);
            return;
        }

        try
        {
            if (jToken is not JsonArray jArrayInput)
            {
                requestCallBack(webApiParameter);
                return;
            }

            var ordered = jArrayInput
                .OrderBy(jObj => Helpsters.ConvertJObjectValue(jObj, "distance", 0.0))
                .ToArray();

            if (ordered.Length < 26)
            {
                var values = (int[])Enum.GetValues(typeof(SurroundingsRadius));
                var index = Array.IndexOf(values, (int)webApiParameterEdsmSurroundings.Radius);
                if (index < values.Length - 1)
                {
                    log.Debug($"Less than {(26)} found ({(ordered.Length)}) within a radius of {(values[index])} Ly, increasing radius to {(values[index + 1])} Ly");
                    EdsmRequestSurroundingStarSystemsInformation(
                        webApiParameterEdsmSurroundings.StarSystem,
                        requestCallBack,
                        (SurroundingsRadius)values[index + 1]);
                    return;
                }

                log.Debug($"Less than {(26)} found ({(ordered.Length)}) within a maximum radius of {(values[index])} Ly, continuing ...");
            }

            for (var i = 0; i < ordered.Length && i != 26; i++)
            {
                if (ordered[i] is not JsonObject jObject)
                {
                    continue;
                }

                var name = Helpsters.ConvertJObjectValue(jObject, "name", string.Empty);
                var id = Helpsters.ConvertJObjectValue(jObject, "id64", 0L);

                if (id != 0L && !string.IsNullOrEmpty(name) && id != webApiParameterEdsmSurroundings.StarSystem.Id)
                {
                    var starSystem = new StarSystem(id, name);
                    starSystem.WasRequestedFromEdsm = true;
                    edsmUpdateBasicSystemData(jObject, starSystem);
                    webApiParameterEdsmSurroundings.StarSystems.Add(starSystem);
                    log.Debug($"'{(starSystem.Name)}' (body count: {(starSystem.EdsmTotalBodyCount)}, distance: {(starSystem.JumpDistanceLy)}) added to surrounding star systems, count is {(webApiParameterEdsmSurroundings.StarSystems.Count)}");
                }
            }
        }
        catch (Exception exception)
        {
            log.Error($"Error while updating EDSM surrounding star systems information for star system '{(webApiParameterEdsmSurroundings?.StarSystem?.Name)}' ({(webApiParameterEdsmSurroundings?.StarSystem?.Id)}), EDSM Response: {(jToken)}, radius: {(webApiParameterEdsmSurroundings?.Radius)} Ly", exception);
        }

        requestCallBack(webApiParameter);
    }

    /// <summary>Performs the EdsmCheckAndRequestStarSystemInformation operation.</summary>
    /// <param name="webApiParameterEdsmStarystem">The WebApiParameterEdsmStarystem value of the webApiParameterEdsmStarystem parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    /// <param name="forceUpdate">The bool value of the forceUpdate parameter.</param>
    /// <param name="ignoreSpeechOutput">The bool value of the ignoreSpeechOutput parameter.</param>
    public void EdsmCheckAndRequestStarSystemInformation(WebApiParameterEdsmStarystem webApiParameterEdsmStarystem, Action<WebApiParameter> requestCallBack, bool forceUpdate, bool ignoreSpeechOutput)
    {
        if (webApiParameterEdsmStarystem.StarSystem.Id == 0L)
        {
            log.Debug($"Star system '{(webApiParameterEdsmStarystem.StarSystem.Name)}' ({(webApiParameterEdsmStarystem.StarSystem.Id)}) is not a valid system, will not request EDSM data");
            requestCallBack(webApiParameterEdsmStarystem);
        }
        else if (webApiParameterEdsmStarystem.StarSystem.NeedsEdsmSystemUpdate || forceUpdate)
        {
            log.Debug($"Star system '{(webApiParameterEdsmStarystem.StarSystem.Name)}' needs {(forceUpdate ? "forced" : "unforced")} EDSM system update");
            const string apiUrl = "https://www.edsm.net/api-v1/system";
            var queryData = "systemName=" + Uri.EscapeDataString(webApiParameterEdsmStarystem.StarSystem.Name) + "&showPrimaryStar=1&showId=1&showCoordinates=1&showInformation=1";
            _ = new WebApiRequest(
                this,
                apiUrl,
                new WepApiQueryData(WepApiQueryType.GetQuery, queryData),
                edsmUpdateSystemInformation,
                webApiParameterEdsmStarystem,
                requestCallBack,
                ignoreSpeechOutput);
        }
        else if (webApiParameterEdsmStarystem.StarSystem.NeedsEdsmBodiesUpdate)
        {
            log.Debug($"Star system '{(webApiParameterEdsmStarystem.StarSystem.Name)}' ({(webApiParameterEdsmStarystem.StarSystem.Id)}) needs EDSM bodies update");
            edsmRequestCelestialBodiesInformation(webApiParameterEdsmStarystem, requestCallBack, ignoreSpeechOutput);
        }
        else
        {
            log.Debug($"Star system '{(webApiParameterEdsmStarystem.StarSystem.Name)}' ({(webApiParameterEdsmStarystem.StarSystem.Id)}) does not need EDSM system or bodies update");
            requestCallBack(webApiParameterEdsmStarystem);
        }
    }

    /// <summary>Performs the edsmUpdateSystemInformation operation.</summary>
    /// <param name="jToken">The JsonNode? value of the jToken parameter.</param>
    /// <param name="webApiParameter">The WebApiParameter value of the webApiParameter parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    /// <param name="ignoreSpeechOutput">The bool value of the ignoreSpeechOutput parameter.</param>
    /// <exception cref="InvalidOperationException">Thrown when the operation fails.</exception>
    private void edsmUpdateSystemInformation(JsonNode? jToken, WebApiParameter webApiParameter, Action<WebApiParameter> requestCallBack, bool ignoreSpeechOutput)
    {
        var starSystem = (webApiParameter as WebApiParameterEdsmStarystem)?.StarSystem;
        if (starSystem is null)
        {
            requestCallBack(webApiParameter);
            return;
        }

        starSystem.WasRequestedFromEdsm = true;
        try
        {
            if (jToken is JsonArray jsonArray && jsonArray.Count == 0)
            {
                log.Debug($"EDSM returned no data for '{starSystem.Name}' ({starSystem.Id}); marking system as read with 0 bodies");
                starSystem.WasReadFromEdsm = true;
                starSystem.EdsmTotalBodyCount = 0;
                if (string.IsNullOrEmpty(starSystem.StarClass))
                {
                    starSystem.StarClass = "Unknown";
                }
                requestCallBack(webApiParameter);
                return;
            }

            if (jToken is JsonObject jObject && edsmCheckSystemId(jObject, starSystem) && edsmUpdateBasicSystemData(jObject, starSystem))
            {
                bool shouldLoadBodies = true;
                if (_starSystemProvider.StarSystemsOnRoute.ContainsKey(starSystem.Id) && _starSystemProvider.CurrentSystem != null)
                {
                    int jumpsAhead = starSystem.JumpDistance - _starSystemProvider.CurrentSystem.JumpDistance;
                    if (jumpsAhead > 3)
                    {
                        shouldLoadBodies = false;
                        log.Debug($"Skipping EDSM body request for '{starSystem.Name}' ({starSystem.Id}), it is {jumpsAhead} jumps ahead");
                    }
                }
                if (shouldLoadBodies)
                {
                    edsmRequestCelestialBodiesInformation(webApiParameter as WebApiParameterEdsmStarystem ?? throw new InvalidOperationException(), requestCallBack, ignoreSpeechOutput);
                    return;
                }
            }
        }
        catch (Exception exception)
        {
            log.Error($"Error while updating EDSM star system information, EDSM Response: {(jToken)}, WebEdsmRequestParameter: {(webApiParameter)}", exception);
        }

        requestCallBack(webApiParameter);
    }

    /// <summary>Performs the edsmRequestCelestialBodiesInformation operation.</summary>
    /// <param name="webApiParameterEdsmStarystem">The WebApiParameterEdsmStarystem value of the webApiParameterEdsmStarystem parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    /// <param name="ignoreSpeechOutput">The bool value of the ignoreSpeechOutput parameter.</param>
    private void edsmRequestCelestialBodiesInformation(WebApiParameterEdsmStarystem webApiParameterEdsmStarystem, Action<WebApiParameter> requestCallBack, bool ignoreSpeechOutput)
    {
        const string apiUrl = "https://www.edsm.net/api-system-v1/bodies";
        var queryData = "systemName=" + Uri.EscapeDataString(webApiParameterEdsmStarystem.StarSystem.Name);
        log.Debug($"System {(webApiParameterEdsmStarystem.StarSystem.Name)} ({(webApiParameterEdsmStarystem.StarSystem.Id)}) needs update for celestial bodies information from EDSM");
        _ = new WebApiRequest(
            this,
            apiUrl,
            new WepApiQueryData(WepApiQueryType.GetQuery, queryData),
            edsmUpdateCelestialBodiesInformation,
            webApiParameterEdsmStarystem,
            requestCallBack,
            ignoreSpeechOutput,
            followUpRequest: true);
    }

    /// <summary>Performs the edsmUpdateCelestialBodiesInformation operation.</summary>
    /// <param name="jToken">The JsonNode? value of the jToken parameter.</param>
    /// <param name="webEdsmRequestParameter">The WebApiParameter value of the webEdsmRequestParameter parameter.</param>
    /// <param name="requestCallBack">The Action<WebApiParameter> value of the requestCallBack parameter.</param>
    /// <param name="ignoreSpeechOutput">The bool value of the ignoreSpeechOutput parameter.</param>
    private void edsmUpdateCelestialBodiesInformation(JsonNode? jToken, WebApiParameter webEdsmRequestParameter, Action<WebApiParameter> requestCallBack, bool ignoreSpeechOutput)
    {
        var webApiParameterEdsmStarystem = webEdsmRequestParameter as WebApiParameterEdsmStarystem;
        if (webApiParameterEdsmStarystem is null)
        {
            requestCallBack(webEdsmRequestParameter);
            return;
        }

        try
        {
            var starSystem = webApiParameterEdsmStarystem.StarSystem;
            if (jToken is JsonArray jsonArray && jsonArray.Count == 0)
            {
                log.Debug($"EDSM returned no body data for '{starSystem.Name}' ({starSystem.Id}); setting body count to 0");
                starSystem.EdsmTotalBodyCount = 0;
                requestCallBack(webEdsmRequestParameter);
                return;
            }

            if (jToken is not JsonObject jObject || !edsmCheckSystemId(jObject, starSystem))
            {
                requestCallBack(webEdsmRequestParameter);
                return;
            }

            starSystem.EdsmTotalBodyCount = Helpsters.ConvertJObjectValue<int?>(jObject, "bodyCount");

            foreach (var jBody in Helpsters.ConvertJObjectValue(jObject, "bodies", new JsonArray()).OfType<JsonObject>())
            {
                var id = Helpsters.ConvertJObjectValue(jBody, "bodyId", 0);
                var name = Helpsters.ConvertJObjectValue(jBody, "name", string.Empty);
                var starSystemId = Helpsters.ConvertJObjectValue(jObject, "id64", 0L);
                var distance = Helpsters.ConvertJObjectValue(jBody, "distanceToArrival", 0.0);
                var subType = Helpsters.ConvertJObjectValue(jBody, "subType", string.Empty);
                var orbitalInclination = Helpsters.ConvertJObjectValue<double?>(jBody, "orbitalInclination");
                var commander = Helpsters.ConvertJObjectValue(Helpsters.ConvertJObjectValue<JsonObject?>(jBody, "discovery", null!), "commander", string.Empty);
                var type = Helpsters.ConvertJObjectValue(jBody, "type", string.Empty);

                Body body = type switch
                {
                    "Star" => new Star(
                        id,
                        starSystemId,
                        name,
                        distance,
                        subType,
                        Helpsters.ConvertJObjectValue(jBody, "solarRadius", 0.0) * 695500000.0,
                        Helpsters.ConvertJObjectValue(jBody, "solarMasses", 0.0),
                        orbitalInclination),
                    "Planet" => new Planet(
                        id,
                        starSystemId,
                        name,
                        distance,
                        subType,
                        Helpsters.ConvertJObjectValue(jBody, "isLandable", false),
                        Helpsters.ConvertJObjectValue(jBody, "terraformingState", string.Empty),
                        Helpsters.ConvertJObjectValue(jBody, "gravity", 0.0),
                        Helpsters.ConvertJObjectValue(jBody, "surfaceTemperature", 0.0),
                        Helpsters.ConvertJObjectValue(jBody, "volcanismType", string.Empty),
                        Helpsters.ConvertJObjectValue(jBody, "atmosphereType", string.Empty),
                        Helpsters.ConvertJObjectValue(jBody, "radius", 0.0) * 1000.0,
                        Helpsters.DetermineParentIdsOfBody(jBody, DataSource.Edsm).parentStarId,
                        Helpsters.DetermineParentIdsOfBody(jBody, DataSource.Edsm).parentPlanetId,
                        Helpsters.ConvertJObjectValue(jBody, "earthMasses", 0.0),
                        orbitalInclination)
                    {
                        MatchingPlanetClassificationsAnnounced = ignoreSpeechOutput
                    },
                    _ => new Body(id, starSystemId, name, distance, 0.0, 0.0, orbitalInclination)
                };

                body.StarSystem = starSystem;
                if (!string.IsNullOrEmpty(commander))
                {
                    body.EdsmDiscoveryCommander = commander;
                }

                var result = starSystem.TryAddOrUpdateBody(body, ignoreSpeechOutput: true, DataSource.Edsm, out var addedOrUpdatedBody);
                if (result == 0 || addedOrUpdatedBody is null)
                {
                    continue;
                }

                var rings = Helpsters.ConvertJObjectValue(jBody, "rings", new JsonArray());
                if (rings.Count > 0)
                {
                    addedOrUpdatedBody.RingsReserveLevel = Helpsters.GetRingReserveLevel(Helpsters.ConvertJObjectValue<string>(jBody, "reserveLevel"));
                    foreach (var ringObject in rings.OfType<JsonObject>())
                    {
                        var ring = new Ring(
                            Helpsters.ConvertJObjectValue(ringObject, "name", string.Empty),
                            Helpsters.ConvertJObjectValue(jObject, "id64", 0L),
                            Helpsters.ConvertJObjectValue(jBody, "bodyId", 0),
                            Helpsters.GetRingType(Helpsters.ConvertJObjectValue(ringObject, "type", string.Empty)),
                            Helpsters.ConvertJObjectValue(ringObject, "mass", 0L),
                            Helpsters.ConvertJObjectValue(ringObject, "innerRadius", 0L) * 1000,
                            Helpsters.ConvertJObjectValue(ringObject, "outerRadius", 0L) * 1000);
                        addedOrUpdatedBody.TryAddOrUpdateRing(ring, DataSource.Edsm);
                    }
                }

                addedOrUpdatedBody.CalculateCartographicValue(ignoreSpeechOutput || result == 2);
                if (addedOrUpdatedBody.Type == BodyType.Planet) _starSystemProvider.FindMatchingClassificationsForPlanet(addedOrUpdatedBody as Planet);
            }
        }
        catch (Exception exception)
        {
            log.Error($"Error while updating EDSM celestial bodies information, EDSM Response: {(jToken)}, star system: '{(webApiParameterEdsmStarystem?.StarSystem?.Name)}' ({(webApiParameterEdsmStarystem?.StarSystem?.Id)})", exception);
        }

        requestCallBack(webEdsmRequestParameter);
    }

    /// <summary>Performs the edsmCheckSystemId operation.</summary>
    /// <param name="jStarSystem">The JsonObject? value of the jStarSystem parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    /// <returns>A bool result.</returns>
    private bool edsmCheckSystemId(JsonObject? jStarSystem, StarSystem starSystem)
    {
        var id = Helpsters.ConvertJObjectValue(jStarSystem, "id64", 0L);
        var name = Helpsters.ConvertJObjectValue(jStarSystem, "name", string.Empty);
        if (id != starSystem.Id)
        {
            log.Info($"Requested EDSM star system '{(name)}' has a different id ({(id)}) than the star system '{(starSystem.Name)}' to be updated ({(starSystem.Id)})");
            return false;
        }

        return true;
    }

    /// <summary>Performs the edsmUpdateBasicSystemData operation.</summary>
    /// <param name="jStarSystem">The JsonObject? value of the jStarSystem parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    /// <returns>A bool result.</returns>
    private bool edsmUpdateBasicSystemData(JsonObject? jStarSystem, StarSystem starSystem)
    {
        try
        {
            var id = Helpsters.ConvertJObjectValue(jStarSystem, "id64", 0L);
            var name = Helpsters.ConvertJObjectValue(jStarSystem, "name", string.Empty);
            if (id != starSystem.Id)
            {
                log.Info($"Requested EDSM star system '{(name)}' has a different id ({(id)}) than the star system '{(starSystem.Name)}' to be updated ({(starSystem.Id)}) - aborting update!");
                return false;
            }

            if (Helpsters.ConvertJObjectValue(jStarSystem, "distance", 0.0) != 0.0)
            {
                starSystem.JumpDistanceLy = Helpsters.ConvertJObjectValue(jStarSystem, "distance", 0.0);
            }

            starSystem.EdsmName = name;
            starSystem.EdsmTotalBodyCount = Helpsters.ConvertJObjectValue<int?>(jStarSystem, "bodyCount");
            starSystem.JumpDistanceLy = Helpsters.ConvertJObjectValue(jStarSystem, "distance", 0.0);

            var primaryStar = Helpsters.ConvertJObjectValue<JsonObject?>(jStarSystem, "primaryStar", null!);
            if (primaryStar is not null)
            {
                starSystem.EdsmPrimaryStarType = Helpsters.ConvertJObjectValue(primaryStar, "type", string.Empty);
                starSystem.EdsmPrimaryStarName = Helpsters.ConvertJObjectValue(primaryStar, "name", string.Empty);
                starSystem.EdsmPrimaryStarIsScoopable = Helpsters.ConvertJObjectValue(primaryStar, "isScoopable", false);
            }

            if (string.IsNullOrEmpty(starSystem.StarClass) && !string.IsNullOrEmpty(starSystem.EdsmPrimaryStarType))
            {
                starSystem.StarClass = starSystem.EdsmPrimaryStarType;
            }

            var coords = Helpsters.ConvertJObjectValue<JsonObject?>(jStarSystem, "coords", null!);
            var coordsLocked = Helpsters.ConvertJObjectValue(jStarSystem, "coordsLocked", false);
            if (coords is not null && coordsLocked)
            {
                starSystem.StarPositionX = Helpsters.ConvertJObjectValue(coords, "x", 0.0);
                starSystem.StarPositionY = Helpsters.ConvertJObjectValue(coords, "y", 0.0);
                starSystem.StarPositionZ = Helpsters.ConvertJObjectValue(coords, "z", 0.0);
            }

            var information = Helpsters.ConvertJObjectValue<JsonObject?>(jStarSystem, "information", null!);
            if (information is not null)
            {
                starSystem.Population = Helpsters.ConvertJObjectValue(information, "population", 0L);
            }

            starSystem.WasReadFromEdsm = true;
            return true;
        }
        catch (Exception exception)
        {
            log.Error($"Error while updating EDSM basic system data, EDSM Response: {(jStarSystem)}, star system: '{(starSystem.Name)}' ({(starSystem.Id)})", exception);
        }

        return false;
    }

    /// <summary>Performs the EdsmRequestSystemAsync operation.</summary>
    /// <param name="systemName">The string value of the systemName parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<JsonNode?> representing the asynchronous operation.</returns>
    public async Task<JsonNode?> EdsmRequestSystemAsync(string systemName, CancellationToken ct = default)
    {
        var url = $"https://www.edsm.net/api-v1/system?systemName={Uri.EscapeDataString(systemName)}&showPrimaryStar=1&showId=1&showCoordinates=1&showInformation=1";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

    /// <summary>Performs the EdsmRequestBodiesAsync operation.</summary>
    /// <param name="systemName">The string value of the systemName parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<JsonNode?> representing the asynchronous operation.</returns>
    public async Task<JsonNode?> EdsmRequestBodiesAsync(string systemName, CancellationToken ct = default)
    {
        var url = $"https://www.edsm.net/api-system-v1/bodies?systemName={Uri.EscapeDataString(systemName)}";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

    /// <summary>Performs the EdsmRequestSurroundingsAsync operation.</summary>
    /// <param name="systemName">The string value of the systemName parameter.</param>
    /// <param name="radius">The int value of the radius parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<JsonNode?> representing the asynchronous operation.</returns>
    public async Task<JsonNode?> EdsmRequestSurroundingsAsync(string systemName, int radius = 20, CancellationToken ct = default)
    {
        var url = $"https://www.edsm.net/api-v1/sphere-systems?systemName={Uri.EscapeDataString(systemName)}&showId=1&showPrimaryStar=1&showCoordinates=1&showInformation=1&radius={radius}";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

    /// <summary>Performs the SpanshRequestSystemNamesAsync operation.</summary>
    /// <param name="query">The string value of the query parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<JsonNode?> representing the asynchronous operation.</returns>
    public async Task<JsonNode?> SpanshRequestSystemNamesAsync(string query, CancellationToken ct = default)
    {
        var url = $"https://spansh.co.uk/api/systems/field_values/system_names?q={Uri.EscapeDataString(query)}";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

    public async Task<JsonNode?> SpanshRequestGalaxyRouteAsync(
        long sourceId64,
        long targetId64,
        int maxTime,
        Dictionary<string, string>? extra = null,
        CancellationToken ct = default)
    {
        var ship = _starSystemProvider.CurrentShip;
        var parameters = new Dictionary<string, string>
        {
            ["source"] = sourceId64.ToString(),
            ["destination"] = targetId64.ToString(),
            ["is_supercharged"] = Convert.ToString(Convert.ToInt32(ship?.JetConeBoost)),
            ["use_supercharge"] = Convert.ToString(Convert.ToInt32(Preferences.Spansh.UseSupercharge)),
            ["use_injections"] = Convert.ToString(Convert.ToInt32(Preferences.Spansh.UseInjections)),
            ["exclude_secondary"] = Convert.ToString(Convert.ToInt32(Preferences.Spansh.ExcludeSecondary)),
            ["refuel_every_scoopable"] = Convert.ToString(Convert.ToInt32(Preferences.Spansh.RefuelEveryScoopable)),
            ["fuel_power"] = Convert.ToString(ship?.FrameShiftDrive?.FuelPower, CultureInfo.InvariantCulture) ?? string.Empty,
            ["fuel_multiplier"] = Convert.ToString(ship?.FrameShiftDrive?.FuelMultiplier, CultureInfo.InvariantCulture) ?? string.Empty,
            ["optimal_mass"] = Convert.ToString(ship?.FrameShiftDrive?.OptimalMass, CultureInfo.InvariantCulture) ?? string.Empty,
            ["base_mass"] = Convert.ToString(ship?.BaseMass, CultureInfo.InvariantCulture) ?? string.Empty,
            ["tank_size"] = Convert.ToString(ship?.MainFuelCapacity, CultureInfo.InvariantCulture) ?? string.Empty,
            ["internal_tank_size"] = Convert.ToString(ship?.ReserveFuelCapacity, CultureInfo.InvariantCulture) ?? string.Empty,
            ["reserve_size"] = "0",
            ["max_fuel_per_jump"] = Convert.ToString(ship?.FrameShiftDrive?.MaxFuelPerJump, CultureInfo.InvariantCulture) ?? string.Empty,
            ["range_boost"] = (ship?.GuardianFsdBooster == null) ? "0" : Convert.ToString(ship?.GuardianFsdBooster?.JumpBoost, CultureInfo.InvariantCulture) ?? string.Empty,
            ["ship_build"] = ship?.SLEF?.ToJsonString() ?? string.Empty,
            ["max_time"] = maxTime.ToString(),
            ["cargo"] = Convert.ToString(ship?.CargoCount) ?? string.Empty,
            ["supercharge_multiplier"] = Convert.ToString(ship?.FrameShiftDrive?.JumpBoostMultiplier) ?? string.Empty,
            ["injection_multiplier"] = "2"
        };

        if (extra != null)
        {
            foreach (var kvp in extra)
            {
                parameters[kvp.Key] = kvp.Value;
            }
        }

        var content = new FormUrlEncodedContent(parameters);
        return await SendPostAsync(new Uri("https://spansh.co.uk/api/generic/route"), content, ct).ConfigureAwait(false);
    }

    /// <summary>Performs the SpanshPollJobResultAsync operation.</summary>
    /// <param name="jobId">The string value of the jobId parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<JsonNode?> representing the asynchronous operation.</returns>
    public async Task<JsonNode?> SpanshPollJobResultAsync(string jobId, CancellationToken ct = default)
    {
        var url = $"https://spansh.co.uk/api/results/{Uri.EscapeDataString(jobId)}";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

    /// <summary>Determines whether CanonnRequestBioStatsAsync.</summary>
    /// <param name="genus">The string value of the genus parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<JsonNode?> representing the asynchronous operation.</returns>
    public async Task<JsonNode?> CanonnRequestBioStatsAsync(string genus, CancellationToken ct = default)
    {
        var url = $"https://api.canonn.tech/biostats?genus={Uri.EscapeDataString(genus)}";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

    /// <summary>Creates NeutronJumps.</summary>
    /// <param name="result">The JsonObject value of the result parameter.</param>
    /// <returns>A JsonArray? result.</returns>
    private static JsonArray? BuildNeutronJumps(JsonObject result)
    {
        var systemJumps = Helpsters.ConvertJObjectValue<JsonArray?>(result, "system_jumps");
        if (systemJumps == null)
        {
            return null;
        }

        var waypoints = new List<JsonObject>();
        foreach (JsonNode? node in systemJumps)
        {
            if (node is not JsonObject waypoint)
            {
                continue;
            }

            long id64 = Helpsters.ConvertJObjectValue(waypoint, "id64", 0L);
            string? name = Helpsters.ConvertJObjectValue<string?>(waypoint, "system") ?? Helpsters.ConvertJObjectValue<string?>(waypoint, "name");
            if (id64 == 0L || string.IsNullOrEmpty(name))
            {
                continue;
            }

            waypoints.Add(waypoint);
        }

        var refuelStars = Helpsters.ConvertJObjectValue<JsonArray?>(result, "refuel_stars");
        if (refuelStars != null)
        {
            foreach (JsonNode? node in refuelStars)
            {
                if (node is not JsonObject refuel)
                {
                    continue;
                }

                long id64 = Helpsters.ConvertJObjectValue(refuel, "id64", 0L);
                string? name = Helpsters.ConvertJObjectValue<string?>(refuel, "name") ?? Helpsters.ConvertJObjectValue<string?>(refuel, "system");
                if (id64 == 0L || string.IsNullOrEmpty(name))
                {
                    continue;
                }

                refuel["is_refuel"] = true;

                double refuelDistanceLeft = Helpsters.ConvertJObjectValue(refuel, "distance", 0.0);
                bool inserted = false;
                for (int i = 0; i < waypoints.Count; i++)
                {
                    double jumpDistanceLeft = Helpsters.ConvertJObjectValue(waypoints[i], "distance_left", -1.0);
                    if (jumpDistanceLeft >= 0 && jumpDistanceLeft < refuelDistanceLeft)
                    {
                        waypoints.Insert(i, refuel);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                {
                    waypoints.Add(refuel);
                }
            }
        }

        var jumps = new JsonArray();
        foreach (JsonObject waypoint in waypoints)
        {
            long id64 = Helpsters.ConvertJObjectValue(waypoint, "id64", 0L);
            string? name = Helpsters.ConvertJObjectValue<string?>(waypoint, "name") ?? Helpsters.ConvertJObjectValue<string?>(waypoint, "system");
            double distance = Helpsters.ConvertJObjectValue(waypoint, "distance_jumped", Helpsters.ConvertJObjectValue(waypoint, "distance", 0.0));
            if (id64 == 0L || string.IsNullOrEmpty(name))
            {
                continue;
            }

            var normalized = new JsonObject
            {
                ["id64"] = id64,
                ["name"] = name,
                ["distance"] = distance
            };

            if (Helpsters.ConvertJObjectValue(waypoint, "is_refuel", false))
            {
                normalized["is_refuel"] = true;
                normalized["is_scoopable"] = true;
            }
            else
            {
                if (Helpsters.ConvertJObjectValue(waypoint, "is_scoopable", false))
                {
                    normalized["is_scoopable"] = true;
                }
                if (Helpsters.ConvertJObjectValue(waypoint, "has_neutron", false))
                {
                    normalized["has_neutron"] = true;
                }
            }

            jumps.Add(normalized);
        }

        return jumps;
    }
}
