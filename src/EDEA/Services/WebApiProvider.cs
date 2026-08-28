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
using System.Windows;
using EDEA.Enums;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

public class WebApiProvider
{
    private static readonly ILog log = LogManager.GetLogger(typeof(WebApiProvider));

    internal static int absoluteRequestsInSession;
    internal static int activeRequests;

    private static WebApiProvider? instance;
    private readonly StarSystemProvider _starSystemProvider;
    private readonly ConcurrentDictionary<Guid, WebApiRequest> _registeredRequests = new();
    private bool spanshCancellationOfGalaxyRouteCalculationRequested;
    private long _loadingCount;

    public bool isLoading { get; private set; }

    public int AbsoluteRequestsInSession => absoluteRequestsInSession;

    public HttpClient httpClient { get; }

    public int xRateRemaining { get; set; } = 700;

    public int maxActiveRequests { get; } = 17;

    public int requestWaitDelay { get; } = 700;

    internal readonly SemaphoreSlim edsmSemaphore = new(1, 1);

    internal DateTime lastEdsmRequest = DateTime.MinValue;

    public int registeredRequestsCount => _registeredRequests.Count;

    public event WebApiLoadingStatusChangedEventHandler WebApiLoadingStatusChanged = delegate { };

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

    public static WebApiProvider Instance(HttpClient httpClient, StarSystemProvider starSystemProvider)
    {
        if (instance == null)
        {
            instance = new WebApiProvider(httpClient, starSystemProvider);
        }

        return instance!;
    }

    public void SpanshRequestBasicSystemData(string starSystemNameQuery, ObservableCollection<StarSystem> starSystems, Action<WebApiParameter> requestCallBack)
    {
        var webApiObject = new WebApiParameterSpanshBasicSystemData(starSystems);
        const string apiUrl = "https://spansh.co.uk/api/systems/field_values/system_names";
        string queryData = "q=" + starSystemNameQuery;
        _ = new WebApiRequest(this, apiUrl, new WepApiQueryData(WepApiQueryType.GetQuery, queryData), onSpanshResponseBasicSystemData, webApiObject, requestCallBack, ignoreSpeechOutput: true);
    }

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
            Application.Current?.Dispatcher.Invoke(() =>
            {
                foreach (var jSystem in jSystems!.OfType<JsonObject>())
                {
                    parameterSpanshSystemNames.StarSystems.Add(new StarSystem(Helpsters.ConvertJObjectValue(jSystem, "id64", 0L), Helpsters.ConvertJObjectValue<string>(jSystem, "name")));
                }
            });
            requestCallBack(parameterSpanshSystemNames);
        }
        catch (Exception exception)
        {
            log.Error($"Error while updating Spansh system names information, Spansh Response: {(jToken)}", exception);
        }
    }

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
        _ = new WebApiRequest(this, apiUrl, new WepApiQueryData(WepApiQueryType.GetQuery, queryData), onSpanshRequestNeutronRouteCalculation, webApiObject, requestCallBack, ignoreSpeechOutput: true);
    }

    public void SpanshRequestCancellationOfGalaxyRouteCalculation()
    {
        spanshCancellationOfGalaxyRouteCalculationRequested = true;
    }

    private void onSpanshRequestNeutronRouteCalculation(JsonNode? jToken, WebApiParameter webApiParameter, Action<WebApiParameter> requestCallBack, bool ignoreSpeechOutput)
    {
        if (jToken is not JsonObject jObject)
        {
            Application.Current.Dispatcher.Invoke(() => requestCallBack(webApiParameter));
            return;
        }

        string jobId = Helpsters.ConvertJObjectValue<string>(jObject, "job");
        string jobStatus = Helpsters.ConvertJObjectValue<string>(jObject, "status");
        if (string.IsNullOrEmpty(jobId) || string.IsNullOrEmpty(jobStatus))
        {
            log.Error($"Spansh neutron job has unknown parameter: jobId {(jobId)}, status {(jobStatus)}");
            Application.Current.Dispatcher.Invoke(() => requestCallBack(webApiParameter));
            return;
        }

        if (webApiParameter is not WebApiParameterSpanshGalaxyRoute webApiParameterSpanshGalaxyRoute)
        {
            log.Error($"Spansh neutron job has invalid web API parameter: type is {(webApiParameter.GetType())} instead of {(typeof(WebApiParameterSpanshGalaxyRoute))}");
            Application.Current.Dispatcher.Invoke(() => requestCallBack(webApiParameter));
            return;
        }

        webApiParameterSpanshGalaxyRoute.RequestCount++;
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
                        }
                    }
                    Application.Current.Dispatcher.Invoke(() => requestCallBack(webApiParameterSpanshGalaxyRoute));
                    break;
                }
            default:
                Application.Current.Dispatcher.Invoke(() => requestCallBack(webApiParameter));
                break;
        }
    }

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
                    Application.Current.Dispatcher.Invoke(() => requestCallBack(webApiParameterSpanshGalaxyRoute));
                    break;
                }
            default:
                requestCallBack(webApiParameter);
                break;
        }
    }

    private void EnterLoading(WebApiParameter? webApiParameter = null)
    {
        if (Interlocked.Increment(ref _loadingCount) == 1)
        {
            isLoading = true;
            WebApiLoadingStatusChanged?.Invoke(this, webApiParameter);
        }
    }

    private void ExitLoading(WebApiParameter? webApiParameter = null)
    {
        if (Interlocked.Decrement(ref _loadingCount) == 0)
        {
            isLoading = false;
            WebApiLoadingStatusChanged?.Invoke(this, webApiParameter);
        }
    }

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
            if (jToken is JsonObject jObject && edsmCheckSystemId(jObject, starSystem) && edsmUpdateBasicSystemData(jObject, starSystem))
            {
                edsmRequestCelestialBodiesInformation(webApiParameter as WebApiParameterEdsmStarystem ?? throw new InvalidOperationException(), requestCallBack, ignoreSpeechOutput);
                return;
            }
        }
        catch (Exception exception)
        {
            log.Error($"Error while updating EDSM star system information, EDSM Response: {(jToken)}, WebEdsmRequestParameter: {(webApiParameter)}", exception);
        }

        requestCallBack(webApiParameter);
    }

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

    public async Task<JsonNode?> EdsmRequestSystemAsync(string systemName, CancellationToken ct = default)
    {
        var url = $"https://www.edsm.net/api-v1/system?systemName={Uri.EscapeDataString(systemName)}&showPrimaryStar=1&showId=1&showCoordinates=1&showInformation=1";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

    public async Task<JsonNode?> EdsmRequestBodiesAsync(string systemName, CancellationToken ct = default)
    {
        var url = $"https://www.edsm.net/api-system-v1/bodies?systemName={Uri.EscapeDataString(systemName)}";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

    public async Task<JsonNode?> EdsmRequestSurroundingsAsync(string systemName, int radius = 20, CancellationToken ct = default)
    {
        var url = $"https://www.edsm.net/api-v1/sphere-systems?systemName={Uri.EscapeDataString(systemName)}&showId=1&showPrimaryStar=1&showCoordinates=1&showInformation=1&radius={radius}";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

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

    public async Task<JsonNode?> SpanshPollJobResultAsync(string jobId, CancellationToken ct = default)
    {
        var url = $"https://spansh.co.uk/api/results/{Uri.EscapeDataString(jobId)}";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

    public async Task<JsonNode?> CanonnRequestBioStatsAsync(string genus, CancellationToken ct = default)
    {
        var url = $"https://api.canonn.tech/biostats?genus={Uri.EscapeDataString(genus)}";
        return await SendGetAsync(new Uri(url), ct).ConfigureAwait(false);
    }

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
