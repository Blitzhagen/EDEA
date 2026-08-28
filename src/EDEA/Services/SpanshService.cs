using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using EDEA.Enums;
using EDEA.Models;

namespace EDEA.Services;

/// <summary>Represents the SpanshService class.</summary>
public class SpanshService
{
    /// <summary>The DefaultMaxTime field.</summary>
    private const int DefaultMaxTime = 120;

    /// <summary>The _webApi field.</summary>
    private readonly WebApiProvider _webApi;

    /// <summary>The _isLoading field.</summary>
    private bool _isLoading;

    /// <summary>Gets the IsLoading.</summary>
    /// <value>A bool value.</value>
    public bool IsLoading => _isLoading;

    /// <summary>Occurs when the IsLoadingChanged event is raised.</summary>
    public event EventHandler? IsLoadingChanged;

    /// <summary>Initializes a new instance of the SpanshService class.</summary>
    /// <param name="webApi">The WebApiProvider value of the webApi parameter.</param>
    public SpanshService(WebApiProvider webApi)
    {
        _webApi = webApi;
    }

    /// <summary>Sets IsLoading.</summary>
    /// <param name="value">The bool value of the value parameter.</param>
    private void SetIsLoading(bool value)
    {
        if (_isLoading != value)
        {
            _isLoading = value;
            IsLoadingChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Retrieves RouteByNameAsync.</summary>
    /// <param name="sourceName">The string value of the sourceName parameter.</param>
    /// <param name="targetName">The string value of the targetName parameter.</param>
    /// <param name="settings">The SpanshSettings value of the settings parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<IReadOnlyList<string>?> representing the asynchronous operation.</returns>
    public async Task<IReadOnlyList<string>?> GetRouteByNameAsync(string sourceName, string targetName, SpanshSettings settings, CancellationToken ct = default)
    {
        SetIsLoading(true);
        try
        {
            var sourceId = await ResolveSystemId64Async(sourceName, ct).ConfigureAwait(false);
            var targetId = await ResolveSystemId64Async(targetName, ct).ConfigureAwait(false);

            if (!sourceId.HasValue || !targetId.HasValue)
                return null;

            var route = await GetPlotterRouteAsync(sourceId.Value, targetId.Value, settings, DefaultMaxTime, ct).ConfigureAwait(false);
            if (route is null)
                return null;

            return ExtractSystemNames(route);
        }
        finally
        {
            SetIsLoading(false);
        }
    }

    /// <summary>Performs the ResolveSystemId64Async operation.</summary>
    /// <param name="systemName">The string value of the systemName parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<long?> representing the asynchronous operation.</returns>
    public async Task<long?> ResolveSystemId64Async(string systemName, CancellationToken ct = default)
    {
        try
        {
            var json = await _webApi.EdsmRequestSystemAsync(systemName, ct).ConfigureAwait(false);
            return json?["id64"]?.GetValue<long?>();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Performs the SearchSystemNamesAsync operation.</summary>
    /// <param name="query">The string value of the query parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<IReadOnlyList<string>?> representing the asynchronous operation.</returns>
    public async Task<IReadOnlyList<string>?> SearchSystemNamesAsync(string query, CancellationToken ct = default)
    {
        SetIsLoading(true);
        try
        {
            var json = await _webApi.SpanshRequestSystemNamesAsync(query, ct).ConfigureAwait(false);
            if (json is JsonArray array)
                return ExtractStrings(array);

            if (json is JsonObject obj)
            {
                if (obj["values"] is JsonArray values)
                    return ExtractStrings(values);
                if (obj["data"] is JsonArray data)
                    return ExtractStrings(data);
                if (obj["result"] is JsonArray result)
                    return ExtractStrings(result);
            }

            return new List<string>();
        }
        catch
        {
            return null;
        }
        finally
        {
            SetIsLoading(false);
        }
    }

    /// <summary>Retrieves PlotterRouteAsync.</summary>
    /// <param name="sourceId64">The long value of the sourceId64 parameter.</param>
    /// <param name="targetId64">The long value of the targetId64 parameter.</param>
    /// <param name="settings">The SpanshSettings value of the settings parameter.</param>
    /// <param name="maxTime">The int value of the maxTime parameter.</param>
    /// <param name="ct">The CancellationToken value of the ct parameter.</param>
    /// <returns>A Task<JsonArray?> representing the asynchronous operation.</returns>
    public async Task<JsonArray?> GetPlotterRouteAsync(long sourceId64, long targetId64, SpanshSettings settings, int maxTime, CancellationToken ct = default)
    {
        SetIsLoading(true);
        try
        {
            var extra = new Dictionary<string, string>
            {
                ["algorithm"] = GetAlgorithmName(settings.RoutingAlgorithm)
            };

            var routeResponse = await _webApi.SpanshRequestGalaxyRouteAsync(
                sourceId64,
                targetId64,
                maxTime,
                extra,
                ct).ConfigureAwait(false);

            if (routeResponse is null)
                return null;

            var directResult = routeResponse["result"];
            if (directResult is JsonArray directArray)
                return directArray;

            var jobId = routeResponse["job"]?.GetValue<string>()
                ?? routeResponse["job_id"]?.GetValue<string>();

            if (string.IsNullOrWhiteSpace(jobId))
                return null;

            for (var i = 0; i < 15; i++)
            {
                var pollResponse = await _webApi.SpanshPollJobResultAsync(jobId, ct).ConfigureAwait(false);

                if (pollResponse is null)
                    return null;

                var result = pollResponse["result"];
                if (result is JsonArray array)
                    return array;

                if (i < 14)
                    await Task.Delay(2000, ct).ConfigureAwait(false);

                if (i < 4)
                    await Task.Delay(TimeSpan.FromSeconds(1), ct).ConfigureAwait(false);
            }

            return null;
        }
        catch
        {
            return null;
        }
        finally
        {
            SetIsLoading(false);
        }
    }

    /// <summary>Performs the ExtractSystemNames operation.</summary>
    /// <param name="jsonArray">The JsonArray value of the jsonArray parameter.</param>
    /// <returns>A IReadOnlyList<string> result.</returns>
    private static IReadOnlyList<string> ExtractSystemNames(JsonArray jsonArray)
    {
        var names = new List<string>(jsonArray.Count);
        foreach (var jsonNode in jsonArray)
        {
            var name = jsonNode?["name"]?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(name))
                names.Add(name);
        }

        return names;
    }

    /// <summary>Performs the ExtractStrings operation.</summary>
    /// <param name="jsonArray">The JsonArray value of the jsonArray parameter.</param>
    /// <returns>A IReadOnlyList<string> result.</returns>
    private static IReadOnlyList<string> ExtractStrings(JsonArray jsonArray)
    {
        var names = new List<string>(jsonArray.Count);
        foreach (var jsonNode in jsonArray)
        {
            var name = jsonNode?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(name))
                names.Add(name);
        }

        return names;
    }

    /// <summary>Retrieves AlgorithmName.</summary>
    /// <param name="algorithm">The SpanshRoutingAlgorithm value of the algorithm parameter.</param>
    /// <returns>A string result.</returns>
    private static string GetAlgorithmName(SpanshRoutingAlgorithm algorithm)
    {
        return algorithm switch
        {
            SpanshRoutingAlgorithm.Fuel => "fuel",
            SpanshRoutingAlgorithm.Fuel_Jumps => "fuel_jumps",
            SpanshRoutingAlgorithm.Guided => "guided",
            SpanshRoutingAlgorithm.Optimistic => "optimistic",
            SpanshRoutingAlgorithm.Pessimistic => "pessimistic",
            _ => "optimistic"
        };
    }
}
