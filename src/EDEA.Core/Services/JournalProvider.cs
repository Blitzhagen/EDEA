using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using EDEA;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

/// <summary>Represents a method that handles the ParsedJournalDataUpdated event.</summary>
/// <param name="sender">The source of the event.</param>
/// <param name="shutdown">The bool value of the shutdown parameter.</param>
public delegate void ParsedJournalDataUpdatedEventHandler(object? sender, bool shutdown);

/// <summary>Represents the JournalProvider class.</summary>
public class JournalProvider
{
    /// <summary>The instance field.</summary>
    private static JournalProvider? instance;

    /// <summary>The log field.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(JournalProvider));

    /// <summary>The _journalStore field.</summary>
    private readonly JournalStore _journalStore;

    /// <summary>The _starSystemProvider field.</summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>The commanderNameUnread field.</summary>
    private bool commanderNameUnread = true;

    /// <summary>The _journalPlanetMemory field.</summary>
    private readonly JournalPlanetMemory _journalPlanetMemory;

    /// <summary>The _journalSystemMemory field.</summary>
    private readonly JournalSystemMemory _journalSystemMemory;

    /// <summary>The journalShutdown field.</summary>
    private bool journalShutdown;

    /// <summary>The journalParseRunning field.</summary>
    private bool journalParseRunning;

    /// <summary>The parseJournalTask field.</summary>
    private Task? parseJournalTask;

    /// <summary>The _firstParseStarted field.</summary>
    private int _firstParseStarted;

    /// <summary>Gets or sets the JournalFirstParse.</summary>
    /// <value>A bool value.</value>
    public bool JournalFirstParse { get; private set; }

    /// <summary>Gets the providerSystem.</summary>
    /// <value>A StarSystem value.</value>
    private StarSystem providerSystem => _starSystemProvider.CurrentSystem;

    /// <summary>Occurs when the ParsedJournalDataUpdated event is raised.</summary>
    public event ParsedJournalDataUpdatedEventHandler ParsedJournalDataUpdated = delegate
    {
    };

    /// <summary>Initializes a new instance of the JournalProvider class.</summary>
    /// <param name="journalStore">The JournalStore value of the journalStore parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    private JournalProvider(JournalStore journalStore, StarSystemProvider starSystemProvider)
    {
        _journalStore = journalStore;
        _starSystemProvider = starSystemProvider;
        _starSystemProvider.RegisterProvider(this);
        _journalPlanetMemory = new JournalPlanetMemory();
        _journalSystemMemory = new JournalSystemMemory();
        JournalFirstParse = true;
    }

    /// <summary>Performs the Instance operation.</summary>
    /// <param name="journalStore">The JournalStore value of the journalStore parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    /// <returns>A JournalProvider result.</returns>
    public static JournalProvider Instance(JournalStore journalStore, StarSystemProvider starSystemProvider)
    {
        if (instance == null)
        {
            instance = new JournalProvider(journalStore, starSystemProvider);
        }
        return instance;
    }

    /// <summary>Performs the _journalStore_JournalUpdated operation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="sequelRead">The bool value of the sequelRead parameter.</param>
    /// <param name="lastJournnalAddition">The IEnumerable<string> value of the lastJournnalAddition parameter.</param>
    private void _journalStore_JournalUpdated(object? sender, bool sequelRead, IEnumerable<string> lastJournnalAddition)
    {
        parseJournalTask = parseJournal(sequelRead, lastJournnalAddition);
    }

    /// <summary>Performs the Initialize operation.</summary>
    public void Initialize()
    {
        if (JournalFirstParse && Interlocked.CompareExchange(ref _firstParseStarted, 1, 0) == 0)
        {
            parseJournalTask = firstParse();
        }
    }

    /// <summary>Performs the processJournalScanEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="journalPlanetMemory">The JournalPlanetMemory value of the journalPlanetMemory parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider? value of the starSystemProvider parameter.</param>
    /// <param name="journalFirstParse">The bool value of the journalFirstParse parameter.</param>
    /// <param name="ignoreSpeechOutput">The bool value of the ignoreSpeechOutput parameter.</param>
    public void processJournalScanEvent(JsonObject jObject, JournalPlanetMemory journalPlanetMemory, StarSystem starSystem, StarSystemProvider? starSystemProvider = null, bool journalFirstParse = false, bool ignoreSpeechOutput = true)
    {
        string bodyName = Helpsters.ConvertJObjectValue<string>(jObject, "BodyName");
        var processableEvents = new List<string> { "Scan" };
        if (!isProcessable(jObject, starSystem, processableEvents, out var jSystemAddress, out var _) || string.IsNullOrEmpty(bodyName))
        {
            return;
        }
        double radius = Helpsters.ConvertJObjectValue(jObject, "Radius", 0.0);
        double? orbitalInclination = Helpsters.ConvertJObjectValue<double?>(jObject, "OrbitalInclination");
        double distance = Helpsters.ConvertJObjectValue(jObject, "DistanceFromArrivalLS", 0.0);
        int bodyId = Helpsters.ConvertJObjectValue(jObject, "BodyID", 0);
        Body? body;
        if (!jObject.ContainsKey("PlanetClass"))
        {
            body = (!jObject.ContainsKey("StarType")
                ? new Body(bodyId, jSystemAddress, bodyName, distance, radius, 0.0, orbitalInclination)
                : new Star(bodyId, jSystemAddress, bodyName, distance, Helpsters.ConvertJObjectValue(jObject, "StarType", string.Empty), radius, Helpsters.ConvertJObjectValue(jObject, "StellarMass", 0.0), orbitalInclination)
                {
                    Luminosity = Helpsters.ConvertJObjectValue(jObject, "Luminosity", string.Empty)
                });
        }
        else
        {
            body = new Planet(bodyId, jSystemAddress, bodyName, distance, Helpsters.ConvertJObjectValue(jObject, "PlanetClass", string.Empty), Helpsters.ConvertJObjectValue(jObject, "Landable", false), Helpsters.ConvertJObjectValue(jObject, "TerraformState", string.Empty), Helpsters.ConvertJObjectValue(jObject, "SurfaceGravity", 0.0) / 9.797759, Helpsters.ConvertJObjectValue(jObject, "SurfaceTemperature", 0.0), Helpsters.ConvertJObjectValue(jObject, "Volcanism", string.Empty), Helpsters.ConvertJObjectValue(jObject, "Atmosphere", string.Empty), radius, Helpsters.DetermineParentIdsOfBody(jObject, DataSource.Journal).parentStarId, Helpsters.DetermineParentIdsOfBody(jObject, DataSource.Journal).parentPlanetId, Helpsters.ConvertJObjectValue(jObject, "MassEM", 0.0), orbitalInclination)
            {
                WasMapped = Helpsters.ConvertJObjectValue(jObject, "WasMapped", true),
                WasFootfalled = Helpsters.ConvertJObjectValue<bool?>(jObject, "WasMapped"),
                AtmosphereType = Helpsters.ConvertJObjectValue(jObject, "AtmosphereType", string.Empty),
                AtmosphereComposition = ReadAtmosphereComposition(jObject),
                SurfacePressure = Helpsters.ConvertJObjectValue(jObject, "SurfacePressure", 0.0) / 101231.656250,
                Materials = ReadMaterials(jObject),
                OrbitalPeriod = Helpsters.ConvertJObjectValue<double?>(jObject, "OrbitalPeriod")
            };
            if (_journalPlanetMemory.Contains(body.StarSystemId, body.Id))
            {
                JournalPlanetMemoryItem? journalPlanetMemoryItem = _journalPlanetMemory.Get(body.StarSystemId, body.Id);
                if (journalPlanetMemoryItem != null)
                {
                    ((Planet)body).SurfaceScanned = journalPlanetMemoryItem.SurfaceScanned;
                    ((Planet)body).BiologicalCount = journalPlanetMemoryItem.BiologicalCount;
                    ((Planet)body).GeologicalCount = journalPlanetMemoryItem.GeologicalCount;
                    ((Planet)body).EfficientlyScanned = journalPlanetMemoryItem.EfficientlyScanned;
                }
            }
        }
        int addOrUpdateResult;
        Body addedOrUpdatedBody;
        if (body != null)
        {
            body.StarSystem = starSystem;
            body.WasDiscovered = Helpsters.ConvertJObjectValue(jObject, "WasDiscovered", false);

            // Set the primary star name and class before TryAddOrUpdateBody so the
            // first-discovery speech output can identify the system correctly.
            if (body.Distance == 0.0 && body.Type == BodyType.Star)
            {
                starSystem.PrimaryStarName = body.Name;
                starSystem.StarClass = (body as Star)!.StarType;
            }

            addOrUpdateResult = starSystem.TryAddOrUpdateBody(body, ignoreSpeechOutput, DataSource.Journal, out addedOrUpdatedBody);
            if (addOrUpdateResult == 0)
            {
                return;
            }
            if (_journalPlanetMemory.Contains(addedOrUpdatedBody.StarSystemId, addedOrUpdatedBody.Id))
            {
                JournalPlanetMemoryItem? existingMemoryItem = _journalPlanetMemory.Get(addedOrUpdatedBody.StarSystemId, addedOrUpdatedBody.Id);
                if (existingMemoryItem != null && existingMemoryItem.Genera.Count > 0)
                {
                    foreach (Genus genus in _journalPlanetMemory.Get(addedOrUpdatedBody.StarSystemId, addedOrUpdatedBody.Id)?.Genera!)
                    {
                        (addedOrUpdatedBody as Planet)!.TryAddOrUpdateGenus(genus, starSystemProvider?.LocationOnCurrentPlanet);
                    }
                    if (!journalFirstParse)
                    {
                        starSystemProvider?.PredictOccurrenceOfSpeciesForPlanet((addedOrUpdatedBody as Planet)!);
                    }
                }
                else if (!journalFirstParse && addedOrUpdatedBody.Type == BodyType.Planet && (addedOrUpdatedBody as Planet)!.BiologicalCount > 0)
                {
                    starSystemProvider?.PredictOccurrenceOfSpeciesForPlanet((addedOrUpdatedBody as Planet)!);
                }
            }
            else if (!journalFirstParse && addedOrUpdatedBody.Type == BodyType.Planet && (addedOrUpdatedBody as Planet)!.BiologicalCount > 0)
            {
                starSystemProvider?.PredictOccurrenceOfSpeciesForPlanet((addedOrUpdatedBody as Planet)!);
            }
        }
        else
        {
            log.Warn($"Could not create body from journal entry: {jObject}");
            return;
        }

        JsonArray rings = Helpsters.ConvertJObjectValue(jObject, "Rings", new JsonArray());
        if (rings != null && rings.Count > 0)
        {
            addedOrUpdatedBody.RingsReserveLevel = Helpsters.GetRingReserveLevel(Helpsters.ConvertJObjectValue<string>(jObject, "ReserveLevel"));
            foreach (JsonObject? ringObject in rings.OfType<JsonObject>())
            {
                if (ringObject == null)
                    continue;
                string ringName = Helpsters.ConvertJObjectValue<string>(ringObject, "Name");
                if (!string.IsNullOrEmpty(ringName) && ringName.Contains("Ring"))
                {
                    Ring ring = new Ring(ringName, jSystemAddress, bodyId, Helpsters.GetRingType(Helpsters.ConvertJObjectValue<string>(ringObject, "RingClass")), Helpsters.ConvertJObjectValue(ringObject, "MassMT", 0L), Helpsters.ConvertJObjectValue(ringObject, "InnerRad", 0L), Helpsters.ConvertJObjectValue(ringObject, "OuterRad", 0L));
                    addedOrUpdatedBody.TryAddOrUpdateRing(ring, DataSource.Journal);
                }
            }
        }
        addedOrUpdatedBody.CalculateCartographicValue(ignoreSpeechOutput || addOrUpdateResult == 2);
        if (!journalFirstParse && addedOrUpdatedBody.Type == BodyType.Planet)
        {
            starSystemProvider?.FindMatchingClassificationsForPlanet(addedOrUpdatedBody as Planet);
        }
    }

    /// <summary>
    /// Reads the atmosphere composition array of a journal Scan event.
    /// </summary>
    /// <param name="jObject">The journal event object.</param>
    /// <returns>A dictionary of gas name to percentage.</returns>
    private static Dictionary<string, double> ReadAtmosphereComposition(JsonObject jObject)
    {
        var composition = new Dictionary<string, double>();
        foreach (JsonObject? component in Helpsters.ConvertJObjectValue(jObject, "AtmosphereComposition", new JsonArray()).OfType<JsonObject>())
        {
            if (component == null)
            {
                continue;
            }
            string name = Helpsters.ConvertJObjectValue<string>(component, "Name");
            if (!string.IsNullOrEmpty(name))
            {
                composition[name] = Helpsters.ConvertJObjectValue(component, "Percent", 0.0);
            }
        }
        return composition;
    }

    /// <summary>
    /// Reads the materials array of a journal Scan event.
    /// </summary>
    /// <param name="jObject">The journal event object.</param>
    /// <returns>A set of material names.</returns>
    private static HashSet<string> ReadMaterials(JsonObject jObject)
    {
        var materials = new HashSet<string>();
        foreach (JsonObject? material in Helpsters.ConvertJObjectValue(jObject, "Materials", new JsonArray()).OfType<JsonObject>())
        {
            string name = Helpsters.ConvertJObjectValue<string>(material, "Name");
            if (!string.IsNullOrEmpty(name))
            {
                materials.Add(name);
            }
        }
        return materials;
    }

    /// <summary>Performs the processJournalStartJumpEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="journalSystemMemory">The JournalSystemMemory value of the journalSystemMemory parameter.</param>
    public void processJournalStartJumpEvent(JsonObject jObject, JournalSystemMemory journalSystemMemory)
    {
        string starClass = Helpsters.ConvertJObjectValue<string>(jObject, "StarClass");
        var processableEvents = new List<string> { "StartJump" };
        if (isProcessable(jObject, null, processableEvents, out var jSystemAddress, out var _) && !string.IsNullOrEmpty(starClass))
        {
            journalSystemMemory.AddOrUpdate(new JournalSystemMemoryItem(jSystemAddress)
            {
                StarClass = starClass
            });
        }
    }

    /// <summary>Performs the processJournalFSDJumpEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="journalSystemMemory">The JournalSystemMemory value of the journalSystemMemory parameter.</param>
    /// <returns>A StarSystem? result.</returns>
    public StarSystem? processJournalFSDJumpEvent(JsonObject jObject, JournalSystemMemory journalSystemMemory)
    {
        string starSystemName = Helpsters.ConvertJObjectValue<string>(jObject, "StarSystem");
        JsonArray starPositions = Helpsters.ConvertJObjectValue(jObject, "StarPos", new JsonArray());
        var processableEvents = new List<string> { "FSDJump", "Location", "CarrierJump" };
        if (!isProcessable(jObject, null, processableEvents, out var jSystemAddress, out var jEvent) || string.IsNullOrEmpty(starSystemName) || starPositions.Count < 3)
        {
            return null;
        }
        StarSystem starSystem = new StarSystem(jSystemAddress, starSystemName)
        {
            StarPositionX = starPositions[0]!.GetValue<double>(),
            StarPositionY = starPositions[1]!.GetValue<double>(),
            StarPositionZ = starPositions[2]!.GetValue<double>(),
            Population = Helpsters.ConvertJObjectValue(jObject, "Population", 0L),
            WasReadFromJournal = true
        };
        if (jEvent != "Location")
        {
            starSystem.PrimaryStarName = Helpsters.ConvertJObjectValue<string>(jObject, "Body");
        }
        if (journalSystemMemory.Contains(starSystem.Id))
        {
            JournalSystemMemoryItem? journalSystemMemoryItem = journalSystemMemory.Get(starSystem.Id);
            if (journalSystemMemoryItem != null)
            {
                starSystem.StarClass = journalSystemMemoryItem.StarClass;
            }
        }
        starSystem.UpdateRegion();
        return starSystem;
    }

    /// <summary>Performs the processJournalFSSDiscoveryScanEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    public void processJournalFSSDiscoveryScanEvent(JsonObject jObject, StarSystem starSystem)
    {
        var processableEvents = new List<string> { "FSSDiscoveryScan" };
        if (isProcessable(jObject, starSystem, processableEvents, out var _, out var _))
        {
            if (starSystem.TotalBodyCount == 0)
            {
                starSystem.TotalBodyCount = Helpsters.ConvertJObjectValue(jObject, "BodyCount", starSystem.TotalBodyCount);
            }
            if (starSystem.TotalNonBodyCount == 0)
            {
                starSystem.TotalNonBodyCount = Helpsters.ConvertJObjectValue(jObject, "NonBodyCount", starSystem.TotalNonBodyCount);
            }
            starSystem.AllBodiesFound = Helpsters.ConvertJObjectValue(jObject, "Progress", 0.0) == 1.0;
        }
    }

    /// <summary>Performs the processJournalNavBeaconScanEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    public void processJournalNavBeaconScanEvent(JsonObject jObject, StarSystem starSystem)
    {
        var processableEvents = new List<string> { "NavBeaconScan" };
        if (isProcessable(jObject, starSystem, processableEvents, out var _, out var _))
        {
            starSystem.NavBeaconScanBodyCount = Helpsters.ConvertJObjectValue(jObject, "NumBodies", 0);
            starSystem.AllBodiesFound = true;
        }
    }

    /// <summary>Performs the processJournalFSSAllBodiesFoundEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    public void processJournalFSSAllBodiesFoundEvent(JsonObject jObject, StarSystem starSystem)
    {
        int bodyCount = Helpsters.ConvertJObjectValue(jObject, "Count", 0);
        var processableEvents = new List<string> { "FSSAllBodiesFound" };
        if (isProcessable(jObject, starSystem, processableEvents, out var _, out var _) && bodyCount > 0)
        {
            starSystem.TotalBodyCount = bodyCount;
            starSystem.AllBodiesFound = true;
        }
    }

    /// <summary>Performs the processJournalSAAScanCompleteEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    /// <param name="journalPlanetMemory">The JournalPlanetMemory value of the journalPlanetMemory parameter.</param>
    /// <param name="ignoreSpeechOutput">The bool value of the ignoreSpeechOutput parameter.</param>
    public void processJournalSAAScanCompleteEvent(JsonObject jObject, StarSystem starSystem, JournalPlanetMemory journalPlanetMemory, bool ignoreSpeechOutput = true)
    {
        int targetProbeCount = Helpsters.ConvertJObjectValue(jObject, "EfficiencyTarget", 0);
        int probesUsed = Helpsters.ConvertJObjectValue(jObject, "ProbesUsed", 0);
        var processableEvents = new List<string> { "SAAScanComplete" };
        if (isProcessable(jObject, starSystem, processableEvents, out var jSystemAddress, out var _) && probesUsed != 0)
        {
            int bodyId = Helpsters.ConvertJObjectValue(jObject, "BodyID", 0);
            bool efficientlyScanned = targetProbeCount == 0 || targetProbeCount > probesUsed;
            if (starSystem.Bodies.TryGetValue(bodyId, out var existingBody) && existingBody.IsPlanet)
            {
                ((Planet)starSystem.Bodies[bodyId]).SurfaceScanned = true;
                ((Planet)starSystem.Bodies[bodyId]).EfficientlyScanned = efficientlyScanned;
                starSystem.Bodies[bodyId].CalculateCartographicValue(ignoreSpeechOutput);
            }
            else
            {
                journalPlanetMemory.AddOrUpdate(new JournalPlanetMemoryItem(jSystemAddress, bodyId)
                {
                    SurfaceScanned = true,
                    EfficientlyScanned = efficientlyScanned
                });
            }
        }
    }

    /// <summary>Performs the processJournalFSSBodySignalsEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="journalPlanetMemory">The JournalPlanetMemory value of the journalPlanetMemory parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider? value of the starSystemProvider parameter.</param>
    /// <param name="journalFirstParse">The bool value of the journalFirstParse parameter.</param>
    public void processJournalFSSBodySignalsEvent(JsonObject jObject, JournalPlanetMemory journalPlanetMemory, StarSystem starSystem, StarSystemProvider? starSystemProvider = null, bool journalFirstParse = false)
    {
        var processableEvents = new List<string> { "FSSBodySignals", "SAASignalsFound" };
        if (!isProcessable(jObject, starSystem, processableEvents, out var jSystemAddress, out var _))
        {
            return;
        }
        int bodyId = Helpsters.ConvertJObjectValue(jObject, "BodyID", 0);
        JsonArray signals = Helpsters.ConvertJObjectValue(jObject, "Signals", new JsonArray());
        JsonArray genuses = Helpsters.ConvertJObjectValue(jObject, "Genuses", new JsonArray());
        if (starSystem.Bodies.TryGetValue(bodyId, out var existingBody) && existingBody.IsPlanet)
        {
            Planet planet = (Planet)existingBody;
            foreach (JsonObject? signal in signals.OfType<JsonObject>())
            {
                if (signal == null)
                    continue;
                string signalType = Helpsters.ConvertJObjectValue<string>(signal, "Type");
                if (signalType == "$SAA_SignalType_Biological;")
                {
                    planet.BiologicalCount = Helpsters.ConvertJObjectValue(signal, "Count", 0);
                    if (!journalFirstParse)
                    {
                        starSystemProvider?.PredictOccurrenceOfSpeciesForPlanet(planet);
                    }
                }
                else if (signalType == "$SAA_SignalType_Geological;")
                {
                    planet.GeologicalCount = Helpsters.ConvertJObjectValue(signal, "Count", 0);
                }
            }
            if (genuses.Count <= 0)
            {
                return;
            }
            foreach (JsonObject? genusNode in genuses.OfType<JsonObject>())
            {
                if (genusNode == null)
                    continue;
                planet.TryAddOrUpdateGenus(new Genus(Helpsters.ConvertJObjectValue<string>(genusNode, "Genus_Localised"), planet.StarSystemId, planet.Id, null, codexKey: Helpsters.ConvertJObjectValue<string>(genusNode, "Genus")), starSystemProvider?.LocationOnCurrentPlanet);
            }
            if (!journalFirstParse)
            {
                starSystemProvider?.PredictOccurrenceOfSpeciesForPlanet(planet);
            }
            return;
        }
        JournalPlanetMemoryItem journalPlanetMemoryItem = new JournalPlanetMemoryItem(jSystemAddress, bodyId);
        foreach (JsonObject? signal in signals.OfType<JsonObject>())
        {
            if (signal == null)
                continue;
            string signalType = Helpsters.ConvertJObjectValue<string>(signal, "Type");
            if (signalType == "$SAA_SignalType_Biological;")
            {
                journalPlanetMemoryItem.BiologicalCount = Helpsters.ConvertJObjectValue(signal, "Count", 0);
            }
            else if (signalType == "$SAA_SignalType_Geological;")
            {
                journalPlanetMemoryItem.GeologicalCount = Helpsters.ConvertJObjectValue(signal, "Count", 0);
            }
        }
        if (genuses.Count > 0)
        {
            foreach (JsonObject? genus in genuses.OfType<JsonObject>())
            {
                if (genus == null)
                    continue;
                journalPlanetMemoryItem.Genera.Add(new Genus(Helpsters.ConvertJObjectValue<string>(genus, "Genus_Localised"), jSystemAddress, bodyId, null, codexKey: Helpsters.ConvertJObjectValue<string>(genus, "Genus")));
            }
        }
        journalPlanetMemory.AddOrUpdate(journalPlanetMemoryItem);
    }

    /// <summary>Performs the processJournalTouchdownEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    public void processJournalTouchdownEvent(JsonObject jObject, StarSystem starSystem)
    {
        var processableEvents = new List<string> { "Touchdown" };
        if (isProcessable(jObject, starSystem, processableEvents, out var _, out var _) && Helpsters.ConvertJObjectValue(jObject, "OnPlanet", false) && jObject.ContainsKey("BodyID") && starSystem.Bodies.TryGetValue(Helpsters.ConvertJObjectValue(jObject, "BodyID", 0), out var existingBody) && existingBody.IsPlanet)
        {
            ((Planet)existingBody).Touchdown = true;
        }
    }

    /// <summary>Performs the processJournalScanOrganicEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    /// <param name="starSystemProvider">The StarSystemProvider? value of the starSystemProvider parameter.</param>
    /// <param name="journalFirstParse">The bool value of the journalFirstParse parameter.</param>
    public void processJournalScanOrganicEvent(JsonObject jObject, StarSystem starSystem, StarSystemProvider? starSystemProvider = null, bool journalFirstParse = false)
    {
        var processableEvents = new List<string> { "ScanOrganic" };
        if (!isProcessable(jObject, starSystem, processableEvents, out var _, out var _))
        {
            return;
        }
        if (starSystem.Bodies.TryGetValue(Helpsters.ConvertJObjectValue(jObject, "Body", 0), out var existingBody) && existingBody is Planet)
        {
            Genus genus = new Genus(Helpsters.ConvertJObjectValue<string>(jObject, "Genus_Localised"), existingBody.StarSystemId, existingBody.Id, Helpsters.ConvertJObjectValue<bool?>(jObject, "WasLogged"), Helpsters.ConvertJObjectValue<string>(jObject, "Species_Localised"), Helpsters.ConvertJObjectValue<string>(jObject, "Variant_Localised"), Helpsters.ConvertJObjectValue<string>(jObject, "Genus"), Helpsters.ConvertJObjectValue<string>(jObject, "Species"), Helpsters.ConvertJObjectValue<string>(jObject, "Variant"));
            string? scanType = Helpsters.ConvertJObjectValue<string>(jObject, "ScanType");
            if (scanType == "Log")
            {
                starSystemProvider?.ResetIncompleteGenera();
            }
            ((Planet)existingBody).TryAddOrUpdateGenus(genus, starSystemProvider?.LocationOnCurrentPlanet, scanType);
            if (!journalFirstParse)
            {
                starSystemProvider?.PredictOccurrenceOfSpeciesForPlanet((Planet)existingBody);
            }
        }
        else
        {
            log.Warn($"Could not create genus from journal entry - no planet found: {jObject}");
        }
    }

    /// <summary>Performs the processJournalCodexEntryEvent operation.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    /// <param name="starSystem">The StarSystem value of the starSystem parameter.</param>
    public void processJournalCodexEntryEvent(JsonObject jObject, StarSystem starSystem)
    {
        var processableEvents = new List<string> { "CodexEntry" };
        if (!isProcessable(jObject, starSystem, processableEvents, out var jSystemAddress, out var _))
        {
            return;
        }
        if (Helpsters.ConvertJObjectValue<string>(jObject, "Category") != "$Codex_Category_Biology;")
        {
            return;
        }
        string codexName = Helpsters.ConvertJObjectValue<string>(jObject, "Name");
        if (string.IsNullOrEmpty(codexName))
        {
            return;
        }
        // The region is taken from the codex entry itself ($Codex_RegionName_XX;),
        // falling back to the current system region.
        int? region = null;
        string regionKey = Helpsters.ConvertJObjectValue<string>(jObject, "Region");
        var regionMatch = System.Text.RegularExpressions.Regex.Match(regionKey ?? string.Empty, @"\$Codex_RegionName_(\d+);");
        if (regionMatch.Success)
        {
            region = int.Parse(regionMatch.Groups[1].Value);
        }
        region ??= starSystem.Region;
        if (!region.HasValue)
        {
            region = GalacticRegionProvider.FindRegionForBoxel(jSystemAddress).id;
        }
        if (!region.HasValue)
        {
            log.Debug($"Codex entry '{codexName}' without resolvable region was not stored");
            return;
        }
        (string genus, string species, string color) = BiologyCatalogProvider.ParseVariant(codexName);
        CodexTracker.MarkFound(region.Value, codexName, jSystemAddress);
        log.Info($"Codex entry '{codexName}' (genus '{genus}', species '{species}', color '{color}') recorded for region {region}");
        // Invalidate the prediction of the affected body so codex markers are refreshed.
        if (jObject.ContainsKey("BodyID")
            && starSystem.Bodies.TryGetValue(Helpsters.ConvertJObjectValue(jObject, "BodyID", 0), out var codexBody)
            && codexBody is Planet codexPlanet)
        {
            BiologyCatalogProvider.InvalidatePrediction(codexPlanet);
            if (!JournalFirstParse)
            {
                _starSystemProvider.PredictOccurrenceOfSpeciesForPlanet(codexPlanet, announce: false);
            }
        }
    }

    /// <summary>Performs the firstParse operation.</summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task firstParse()
    {
        await parseJournal();
        _starSystemProvider.PredictOccurrenceOfSpeciesForCurrentSystem();
        _starSystemProvider.FindMatchingClassificationsForSystem(_starSystemProvider.CurrentSystem, ignoreSpeechOutput: true);
        JournalFirstParse = false;
        _starSystemProvider.RequestSurroundingStarSystemsForCurrentSystem();
        await _starSystemProvider.SetPastStarSystemsOnRouteAndRequestEDSMDataForUpcomingStarSystemsOnRoute();
        _starSystemProvider.CopyNextSystemNametoClipboard();
        _starSystemProvider.InitializeCurrentSystem();
        _starSystemProvider.CopyNextSystemNametoClipboard();
        _journalStore.JournalUpdated += _journalStore_JournalUpdated;
    }

    /// <summary>Performs the parseJournal operation.</summary>
    /// <param name="sequelRead">The bool value of the sequelRead parameter.</param>
    /// <param name="lastJournalAddition">The IEnumerable<string>? value of the lastJournalAddition parameter.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task parseJournal(bool sequelRead = false, IEnumerable<string>? lastJournalAddition = null)
    {
        if (journalParseRunning)
        {
            log.Warn("parsing journal not started: already running");
            return;
        }
        journalParseRunning = true;
        journalShutdown = false;
        Stopwatch journalWatch = Stopwatch.StartNew();
        IEnumerable<string> enumerable = (!sequelRead) ? (await _journalStore.GetJournal()) : (lastJournalAddition ?? new List<string>());
        log.Debug("parsing journal started: " + (sequelRead ? "journal addition" : "full journal"));
        foreach (string journalLine in enumerable)
        {
            JsonNode? jToken = JsonNode.Parse(journalLine);
            if (jToken == null || jToken is not JsonObject jObject)
            {
                continue;
            }
            string? eventName = Helpsters.ConvertJObjectValue<string>(jObject, "event");
            try
            {
                if (commanderNameUnread && eventName == "Commander")
                {
                    string? commanderName = Helpsters.ConvertJObjectValue<string>(jObject, "Name");
                    _starSystemProvider.CommanderName = "CMDR " + commanderName;
                    commanderNameUnread = false;
                    PlatformServices.Speech?.SpeakWelcome(new SpeechOutputCommander(commanderName ?? string.Empty));
                    continue;
                }
                if (eventName == "WingAdd" && jObject.ContainsKey("Name"))
                {
                    _starSystemProvider.TeammateNames.Add("CMDR " + Helpsters.ConvertJObjectValue<string>(jObject, "Name"));
                    continue;
                }
                switch (eventName)
                {
                    case "WingLeave":
                        _starSystemProvider.TeammateNames.Clear();
                        break;
                    case "Loadout":
                        jObject.Remove("timestamp");
                        _starSystemProvider.CurrentShip = new Ship(jObject);
                        break;
                    case "Cargo":
                        if (Helpsters.ConvertJObjectValue<string>(jObject, "Vessel") == "Ship" && _starSystemProvider.CurrentShip != null)
                        {
                            _starSystemProvider.CurrentShip.CargoCount = Helpsters.ConvertJObjectValue(jObject, "Count", 0);
                            _starSystemProvider.CurrentShip.UpdateJumpRange();
                        }
                        break;
                    case "JetConeBoost":
                        {
                            int boostValue = Helpsters.ConvertJObjectValue(jObject, "BoostValue", 0);
                            if (boostValue > 0 && _starSystemProvider.CurrentShip != null)
                            {
                                _starSystemProvider.CurrentShip.JetConeBoost = true;
                                _starSystemProvider.CurrentShip.JetConeBoostValue = boostValue;
                            }
                            break;
                        }
                    case "StartJump":
                        if (!jObject.ContainsKey("StarSystem") || !jObject.ContainsKey("StarClass"))
                        {
                            break;
                        }
                        processJournalStartJumpEvent(jObject, _journalSystemMemory);
                        break;
                    case "CarrierStats":
                        processJournalCarrierStatsEvent(jObject);
                        break;
                    case "CarrierLocation":
                    case "CarrierJump":
                        processJournalCarrierLocationEvent(jObject);
                        break;
                    case "Docked":
                        if (Helpsters.ConvertJObjectValue<string>(jObject, "StationType") == "FleetCarrier")
                        {
                            _starSystemProvider.IsOnFleetCarrier = true;
                            _starSystemProvider.DockedCarrierId = Helpsters.ConvertJObjectValue(jObject, "MarketID", 0L);
                            _starSystemProvider.triggerGuiDataUpdateEvent();
                        }
                        break;
                    case "Undocked":
                        if (Helpsters.ConvertJObjectValue<string>(jObject, "StationType") == "FleetCarrier" &&
                            Helpsters.ConvertJObjectValue(jObject, "MarketID", 0L) == _starSystemProvider.DockedCarrierId)
                        {
                            _starSystemProvider.IsOnFleetCarrier = false;
                            _starSystemProvider.DockedCarrierId = 0L;
                            _starSystemProvider.triggerGuiDataUpdateEvent();
                        }
                        break;
                    case "Location":
                        if (Helpsters.ConvertJObjectValue(jObject, "Docked", false) &&
                            Helpsters.ConvertJObjectValue<string>(jObject, "StationType") == "FleetCarrier")
                        {
                            _starSystemProvider.IsOnFleetCarrier = true;
                            _starSystemProvider.DockedCarrierId = Helpsters.ConvertJObjectValue(jObject, "MarketID", 0L);
                            _starSystemProvider.triggerGuiDataUpdateEvent();
                        }
                        break;
                }
                switch (eventName)
                {
                    case "FSDJump":
                    case "Location":
                    case "CarrierJump":
                        _journalPlanetMemory.Clear();
                        if (_starSystemProvider.CurrentShip != null)
                        {
                            _starSystemProvider.CurrentShip.JetConeBoost = false;
                            _starSystemProvider.CurrentShip.JetConeBoostValue = 0;
                        }
                        if (providerSystem.Id != Helpsters.ConvertJObjectValue(jObject, "SystemAddress", 0L))
                        {
                            StarSystem? starSystem = processJournalFSDJumpEvent(jObject, _journalSystemMemory);
                            if (starSystem != null)
                            {
                                await _starSystemProvider.HandleCurrentSystemChange(starSystem);
                            }
                        }
                        break;
                    case "FSSDiscoveryScan":
                        processJournalFSSDiscoveryScanEvent(jObject, providerSystem);
                        break;
                    case "NavBeaconScan":
                        processJournalNavBeaconScanEvent(jObject, providerSystem);
                        break;
                    case "FSSAllBodiesFound":
                        processJournalFSSAllBodiesFoundEvent(jObject, providerSystem);
                        break;
                    case "Scan":
                        processJournalScanEvent(jObject, _journalPlanetMemory, providerSystem, _starSystemProvider, JournalFirstParse, JournalFirstParse);
                        break;
                    case "SAAScanComplete":
                        processJournalSAAScanCompleteEvent(jObject, providerSystem, _journalPlanetMemory);
                        break;
                    case "FSSBodySignals":
                    case "SAASignalsFound":
                        processJournalFSSBodySignalsEvent(jObject, _journalPlanetMemory, providerSystem, _starSystemProvider, JournalFirstParse);
                        break;
                    case "Touchdown":
                        processJournalTouchdownEvent(jObject, providerSystem);
                        break;
                    case "ScanOrganic":
                        processJournalScanOrganicEvent(jObject, providerSystem, _starSystemProvider, JournalFirstParse);
                        break;
                    case "CodexEntry":
                        processJournalCodexEntryEvent(jObject, providerSystem);
                        break;
                    case "Shutdown":
                        if (!JournalFirstParse)
                        {
                            journalShutdown = true;
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                log.Error($"Error parsing journal, JSON object: {jObject}", exception);
            }
        }
        ParsedJournalDataUpdated?.Invoke(this, journalShutdown);
        long elapsedMilliseconds = journalWatch.ElapsedMilliseconds;
        journalWatch.Stop();
        log.Debug($"parsing journal finished after {elapsedMilliseconds}ms");
        journalParseRunning = false;
        parseJournalTask = null;
    }

    /// <summary>Processes a CarrierStats journal event and updates the known carrier data.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    private void processJournalCarrierStatsEvent(JsonObject jObject)
    {
        var carrier = _starSystemProvider.CurrentCarrier ?? new FleetCarrier();
        carrier.CarrierId = Helpsters.ConvertJObjectValue(jObject, "CarrierID", 0L);
        carrier.Callsign = Helpsters.ConvertJObjectValue<string>(jObject, "Callsign") ?? string.Empty;
        carrier.Name = Helpsters.ConvertJObjectValue<string>(jObject, "Name") ?? string.Empty;
        carrier.CarrierType = Helpsters.ConvertJObjectValue<string>(jObject, "CarrierType") ?? string.Empty;
        carrier.FuelLevel = Helpsters.ConvertJObjectValue(jObject, "FuelLevel", 0);
        carrier.JumpRangeCurr = Helpsters.ConvertJObjectValue(jObject, "JumpRangeCurr", 0.0);
        carrier.JumpRangeMax = Helpsters.ConvertJObjectValue(jObject, "JumpRangeMax", 500.0);

        if (jObject["SpaceUsage"] is JsonObject spaceUsage)
        {
            carrier.TotalCapacity = Helpsters.ConvertJObjectValue(spaceUsage, "TotalCapacity", 25000);
            carrier.FreeSpace = Helpsters.ConvertJObjectValue(spaceUsage, "FreeSpace", 0);
        }

        _starSystemProvider.CurrentCarrier = carrier;
        _starSystemProvider.triggerGuiDataUpdateEvent();
        log.Debug($"Carrier stats updated: {carrier.Callsign} '{carrier.Name}', fuel {carrier.FuelLevel} t, capacity {carrier.CapacityUsed}/{carrier.TotalCapacity} t");
    }

    /// <summary>Processes a CarrierLocation or CarrierJump journal event and updates the carrier position.</summary>
    /// <param name="jObject">The JsonObject value of the jObject parameter.</param>
    private void processJournalCarrierLocationEvent(JsonObject jObject)
    {
        var carrier = _starSystemProvider.CurrentCarrier;
        long carrierId = Helpsters.ConvertJObjectValue(jObject, "CarrierID", 0L);
        if (carrier == null || (carrierId != 0L && carrierId != carrier.CarrierId))
        {
            return;
        }

        carrier.StarSystemId = Helpsters.ConvertJObjectValue(jObject, "SystemAddress", 0L);
        carrier.StarSystemName = Helpsters.ConvertJObjectValue<string>(jObject, "StarSystem") ?? string.Empty;
    }

    /// <summary>Performs the isProcessable operation.</summary>
    /// <param name="jObject">The JsonObject? value of the jObject parameter.</param>
    /// <param name="starSystem">The StarSystem? value of the starSystem parameter.</param>
    /// <param name="processEvents">The List<string> value of the processEvents parameter.</param>
    /// <param name="jSystemAddress">The long value of the jSystemAddress parameter.</param>
    /// <param name="jEvent">The string? value of the jEvent parameter.</param>
    /// <returns>A bool result.</returns>
    private bool isProcessable(JsonObject? jObject, StarSystem? starSystem, List<string> processEvents, out long jSystemAddress, out string? jEvent)
    {
        jEvent = Helpsters.ConvertJObjectValue<string>(jObject, "event");
        jSystemAddress = Helpsters.ConvertJObjectValue(jObject, "SystemAddress", 0L);
        if (jEvent == null || !processEvents.Contains(jEvent))
        {
            return false;
        }
        if (starSystem == null)
        {
            return jSystemAddress != 0;
        }
        if (jSystemAddress != 0L)
        {
            return jSystemAddress == starSystem.Id;
        }
        return false;
    }
}
