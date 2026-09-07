using System;
using EDEA.Stores;
using log4net;

namespace EDEA.Services;

/// <summary>
/// Tracks codex biological discoveries and answers codex lookups for the biology prediction engine.
/// Port of the ExploData codex handling (set_codex / check_codex).
/// </summary>
public static class CodexTracker
{
    /// <summary>The logger for this class.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(CodexTracker));

    /// <summary>
    /// Stores a codex biological discovery for the given region.
    /// </summary>
    /// <param name="region">The galactic region identifier.</param>
    /// <param name="biological">The codex entry name (e.g. "$Codex_Ent_Bacterial_01_Genus_Name;").</param>
    /// <param name="starSystemId">The star system identifier the entry was found in.</param>
    public static void MarkFound(int region, string biological, long starSystemId)
    {
        if (string.IsNullOrEmpty(biological))
        {
            return;
        }
        try
        {
            SQLiteStore.Instance().UpsertCodexScan(region, biological, starSystemId);
        }
        catch (Exception exception)
        {
            log.Error($"Could not store codex entry '{biological}' for region {region}", exception);
        }
    }

    /// <summary>
    /// Checks whether a codex biological identifier is already known in the given region,
    /// or galaxy-wide when <paramref name="region"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="region">The region identifier, or <see langword="null"/> for a galaxy-wide check.</param>
    /// <param name="biological">The codex entry name.</param>
    /// <returns><see langword="true"/> when the codex entry exists.</returns>
    public static bool CheckCodex(int? region, string biological)
    {
        try
        {
            return SQLiteStore.Instance().CodexScanExists(region, biological);
        }
        catch (Exception exception)
        {
            log.Debug($"Codex lookup failed for '{biological}' in region {region}: {exception.Message}");
            return false;
        }
    }
}
