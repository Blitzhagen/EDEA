using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

public delegate void StatusUpdatedEventHandler(object? sender, Activity activity, bool isInTeam, string? planetNameExploring, long? destinationSystemId);
public delegate void LocationUpdatedEventHandler(object? sender, double? longitude, double? latitude, double? radius);
public delegate void GuiFocusUpdatedEventHandler(object? sender, EdGuiFocus edGuiFocus, bool isOnFoot);
public delegate void ShipFuelUpdatedEventHandler(object? sender, double main, double reservoir);

public class StatusProvider
{
    private static StatusProvider? instance;

    private static readonly ILog log = LogManager.GetLogger(typeof(StatusProvider));

    private readonly EDFileWatcher _fileWatcher;

    private Activity activity;

    private bool isInTeam;

    private string? planetNameExploring;

    private long? destinationSystemId;

    private double? longitude;

    private double? latitude;

    private double? planetRadius;

    private bool readingStatusFile;

    private EdGuiFocus guiFocus;

    private bool isOnFoot;

    private double shipFuelMain;

    private double shipFuelReservoir;

    public Status? Current { get; private set; }

    public event Action<Status?>? StatusChanged;

    public event StatusUpdatedEventHandler StatusUpdated = delegate { };

    public event LocationUpdatedEventHandler LocationUpdated = delegate { };

    public event GuiFocusUpdatedEventHandler GuiFocusUpdated = delegate { };

    public event ShipFuelUpdatedEventHandler ShipFuelUpdated = delegate { };

    private StatusProvider(EDFileWatcher eDFileWatcher, StarSystemProvider starSystemProvider)
    {
        _fileWatcher = eDFileWatcher;
        starSystemProvider.RegisterProvider(this);
        readingStatusFile = false;
        guiFocus = EdGuiFocus.NoFocus;
        isOnFoot = false;
        _fileWatcher.StatusFileChanged += _fileWatcher_StatusFileChanged;
    }

    public static StatusProvider Instance(EDFileWatcher eDFileWatcher, StarSystemProvider starSystemProvider)
    {
        if (instance == null)
        {
            instance = new StatusProvider(eDFileWatcher, starSystemProvider);
        }
        return instance;
    }

    private void _fileWatcher_StatusFileChanged(object? sender, EdFileEvent e)
    {
        _ = readStatusFile();
    }

    private async Task readStatusFile()
    {
        if (readingStatusFile)
        {
            log.Debug("Status file " + _fileWatcher.StatusFilePath + " is currently being read, aborting parallel read request");
            return;
        }
        readingStatusFile = true;
        for (int i = 0; i < 10; i++)
        {
            try
            {
                string fileContent;
                using (var stream = File.Open(_fileWatcher.StatusFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    fileContent = await reader.ReadToEndAsync();
                }
                if (string.IsNullOrEmpty(fileContent))
                {
                    break;
                }
                JsonNode? statusJson = JsonNode.Parse(fileContent);
                if (statusJson is not JsonObject obj)
                {
                    break;
                }
                if (!obj.ContainsKey("Flags"))
                {
                    break;
                }
                EdStatusFlags edStatusFlags = (EdStatusFlags)(Helpsters.ConvertJObjectValue<long?>(obj, "Flags") ?? 0);
                EdGuiFocus edGuiFocus = EdGuiFocus.NoFocus;
                bool onFoot = false;
                double fuelMain = 0.0;
                double fuelReservoir = 0.0;
                if (obj.ContainsKey("GuiFocus"))
                {
                    edGuiFocus = (EdGuiFocus)(Helpsters.ConvertJObjectValue<int?>(obj, "GuiFocus") ?? 0);
                }
                if (obj.ContainsKey("Flags2"))
                {
                    EdStatusFlags2 edStatusFlags2 = (EdStatusFlags2)(Helpsters.ConvertJObjectValue<long?>(obj, "Flags2") ?? 0);
                    onFoot = edStatusFlags2.HasFlag(EdStatusFlags2.OnFoot);
                }
                string? bodyName = obj.ContainsKey("BodyName") ? Helpsters.ConvertJObjectValue<string?>(obj, "BodyName") : null;
                double? lon;
                double? lat;
                double? radius;
                if (obj.ContainsKey("Longitude") && obj.ContainsKey("Latitude") && obj.ContainsKey("PlanetRadius"))
                {
                    lon = Helpsters.ConvertJObjectValue<double?>(obj, "Longitude");
                    lat = Helpsters.ConvertJObjectValue<double?>(obj, "Latitude");
                    radius = Helpsters.ConvertJObjectValue<double?>(obj, "PlanetRadius");
                }
                else
                {
                    lon = null;
                    lat = null;
                    radius = null;
                }
                long? destSystemId = null;
                if (obj.ContainsKey("Destination"))
                {
                    JsonObject? dest = Helpsters.ConvertJObjectValue<JsonObject?>(obj, "Destination");
                    if (dest != null && dest.ContainsKey("System"))
                    {
                        destSystemId = Helpsters.ConvertJObjectValue<long?>(dest, "System");
                    }
                }
                bool inWing = edStatusFlags.HasFlag(EdStatusFlags.InWing);
                Activity activity;
                if (edGuiFocus == EdGuiFocus.GalaxyMap)
                {
                    activity = Activity.GalaxyMap;
                }
                else if (onFoot)
                {
                    EdStatusFlags2 flags2 = (EdStatusFlags2)(Helpsters.ConvertJObjectValue<long?>(obj, "Flags2") ?? 0);
                    activity = flags2.HasFlag(EdStatusFlags2.OnFootExterior) ? Activity.ExplorePlanet : Activity.Other;
                }
                else if (bodyName != null && obj.ContainsKey("Altitude"))
                {
                    int? altitude = Helpsters.ConvertJObjectValue<int?>(obj, "Altitude");
                    if (altitude.HasValue && altitude.Value < Preferences.Other.BiologicalsViewAltitudeThreshold)
                    {
                        activity = Activity.ExplorePlanet;
                    }
                    else
                    {
                        activity = Activity.ExploreSystem;
                    }
                }
                else if (edStatusFlags.HasFlag(EdStatusFlags.FsdCharging))
                {
                    EdStatusFlags2 flags2 = (EdStatusFlags2)(Helpsters.ConvertJObjectValue<long?>(obj, "Flags2") ?? 0);
                    activity = flags2.HasFlag(EdStatusFlags2.FsdHyperdriveCharging) ? Activity.Jump : Activity.ExploreSystem;
                }
                else if (!edStatusFlags.HasFlag(EdStatusFlags.InMainShip))
                {
                    activity = (Helpsters.ConvertJObjectValue<long?>(obj, "Flags") ?? 0) == 0 ? Activity.None : Activity.Other;
                }
                else
                {
                    activity = Activity.ExploreSystem;
                    JsonObject? jObject = Helpsters.ConvertJObjectValue<JsonObject?>(obj, "Fuel");
                    if (jObject != null)
                    {
                        fuelMain = Helpsters.ConvertJObjectValue(jObject, "FuelMain", 0.0);
                        fuelReservoir = Helpsters.ConvertJObjectValue(jObject, "FuelReservoir", 0.0);
                    }
                }
                if (this.activity != activity || isInTeam != inWing || planetNameExploring != bodyName || destinationSystemId != destSystemId)
                {
                    this.activity = activity;
                    isInTeam = inWing;
                    planetNameExploring = bodyName;
                    destinationSystemId = destSystemId;
                    StatusUpdated?.Invoke(this, this.activity, isInTeam, planetNameExploring, destinationSystemId);
                }
                if (longitude != lon || latitude != lat || planetRadius != radius)
                {
                    longitude = lon;
                    latitude = lat;
                    planetRadius = radius;
                    LocationUpdated?.Invoke(this, longitude, latitude, planetRadius);
                }
                if (guiFocus != edGuiFocus || isOnFoot != onFoot)
                {
                    guiFocus = edGuiFocus;
                    isOnFoot = onFoot;
                    GuiFocusUpdated?.Invoke(this, guiFocus, isOnFoot);
                }
                if (shipFuelMain != fuelMain || shipFuelReservoir != fuelReservoir)
                {
                    shipFuelMain = fuelMain;
                    shipFuelReservoir = fuelReservoir;
                    ShipFuelUpdated?.Invoke(this, shipFuelMain, shipFuelReservoir);
                }
                Current = new Status
                {
                    GuiFocus = obj.ContainsKey("GuiFocus") ? (int?)edGuiFocus : null,
                    Flags = (ulong)edStatusFlags,
                    Flags2 = (ulong)(Helpsters.ConvertJObjectValue<long?>(obj, "Flags2") ?? 0),
                    Ship = Helpsters.ConvertJObjectValue<string?>(obj, "ShipType"),
                    ShipIdent = Helpsters.ConvertJObjectValue<string?>(obj, "ShipIdent"),
                    FuelMain = fuelMain,
                    FuelReservoir = fuelReservoir,
                    LegalState = Helpsters.ConvertJObjectValue<string?>(obj, "LegalState"),
                    Latitude = lat ?? 0.0,
                    Longitude = lon ?? 0.0,
                    Altitude = Helpsters.ConvertJObjectValue<double?>(obj, "Altitude") ?? 0.0,
                    BodyName = bodyName,
                    SystemName = Helpsters.ConvertJObjectValue<string?>(obj, "StarSystem")
                };
                StatusChanged?.Invoke(Current);
            }
            catch (Exception exception)
            {
                if (10 - i > 1)
                {
                    log.Warn($"Status file {_fileWatcher.StatusFilePath} missing or locked, waiting for {500}ms, retry {i + 1} of {10}");
                }
                else
                {
                    log.Error($"Status file {_fileWatcher.StatusFilePath} missing or locked, giving up after {i + 1} retries", exception);
                }
                await Task.Delay(500);
                continue;
            }
            break;
        }
        readingStatusFile = false;
    }
}
