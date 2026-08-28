using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EDEA.Services;
using log4net;

namespace EDEA.Models;

public class StarSystem
{
    private static readonly ILog log = LogManager.GetLogger(typeof(StarSystem));

    private static readonly HashSet<string> _scoopableStarClasses = new(StringComparer.OrdinalIgnoreCase)
    {
        "O", "B", "A", "F", "G", "K", "M", "Refuel"
    };

    private readonly ConcurrentDictionary<int, Body> _bodies;

    public long Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public double? StarPositionX { get; set; }

    public double? StarPositionY { get; set; }

    public double? StarPositionZ { get; set; }

    public string? StarClass { get; set; }

    public string? PrimaryStarName { get; set; }

    public bool PrimaryStarIsScoopable
    {
        get
        {
            if (!CheckStarClassForScoopable(StarClass))
            {
                return EdsmPrimaryStarIsScoopable;
            }
            return true;
        }
    }

    public IReadOnlyDictionary<int, Body> Bodies => _bodies;

    public int JumpDistance { get; set; }

    public double JumpDistanceLy { get; set; }

    public int TotalBodyCount { get; set; }

    public int TotalNonBodyCount { get; set; }

    public int NavBeaconScanBodyCount { get; set; }

    public bool AllBodiesFound { get; set; }

    public bool WasReadFromJournal { get; set; }

    public bool WasReadFromEdsm { get; set; }

    public bool WasRequestedFromEdsm { get; set; }

    public bool WasReadFromEdsmOnly
    {
        get
        {
            if (WasReadFromEdsm)
            {
                return !WasReadFromJournal;
            }
            return false;
        }
    }

    public bool NeedsEdsmSystemUpdate => !WasReadFromEdsm;

    public bool NeedsEdsmBodiesUpdate
    {
        get
        {
            if (!_bodies.IsEmpty)
            {
                return Bodies.Any(body => body.Value.IsPlanetOrStar && !body.Value.WasReadFromEdsm);
            }
            return true;
        }
    }

    public bool IsPastSystemInRoute { get; set; }

    public bool IsCurrentSystemInRoute { get; set; }

    public bool IsJumpDestinationSystemInRoute { get; set; }

    public bool IsSystemInRouteAhead
    {
        get
        {
            if (!IsPastSystemInRoute)
            {
                return !IsCurrentSystemInRoute;
            }
            return false;
        }
    }

    public string? EdsmName { get; set; }

    public string? EdsmPrimaryStarType { get; set; }

    public string? EdsmPrimaryStarName { get; set; }

    public bool EdsmPrimaryStarIsScoopable { get; set; }

    public int? EdsmTotalBodyCount { get; set; }

    public bool IsTripHistory { get; set; }

    public long Population { get; set; }

    public StarSystem()
    {
        _bodies = new ConcurrentDictionary<int, Body>();
    }

    public StarSystem(long id, string name)
    {
        _bodies = new ConcurrentDictionary<int, Body>();
        Id = id;
        Name = name;
        IsTripHistory = true;
    }

    public StarSystem(long id, string name, long isTripHistory)
        : this(id, name)
    {
        IsTripHistory = Convert.ToBoolean(isTripHistory);
    }

    public StarSystem(long id, string name, string starClass, string primaryStarName, long totalBodyCount, long totalNonBodyCount, long wasReadFromJournal, long wasReadFromEdsm, long wasRequestedFromEdsm, string edsmName, string edsmPrimaryStarType, string edsmPrimaryStarName, long edsmPrimaryStarIsScoopable, long? edsmTotalBodyCount, double starPositionX, double starPositionY, double starPositionZ, long isTripHistory, long allBodiesFound, long population)
        : this(id, name, isTripHistory)
    {
        StarPositionX = Convert.ToDouble(starPositionX);
        StarPositionY = Convert.ToDouble(starPositionY);
        StarPositionZ = Convert.ToDouble(starPositionZ);
        StarClass = starClass;
        PrimaryStarName = primaryStarName;
        TotalBodyCount = Convert.ToInt32(totalBodyCount);
        TotalNonBodyCount = Convert.ToInt32(totalNonBodyCount);
        WasReadFromJournal = Convert.ToBoolean(wasReadFromJournal);
        WasReadFromEdsm = Convert.ToBoolean(wasReadFromEdsm);
        WasRequestedFromEdsm = Convert.ToBoolean(wasRequestedFromEdsm);
        EdsmName = edsmName;
        EdsmPrimaryStarType = edsmPrimaryStarType;
        EdsmPrimaryStarName = edsmPrimaryStarName;
        EdsmPrimaryStarIsScoopable = Convert.ToBoolean(edsmPrimaryStarIsScoopable);
        EdsmTotalBodyCount = (int?)edsmTotalBodyCount;
        AllBodiesFound = Convert.ToBoolean(allBodiesFound);
        Population = population;
    }

    public int TryAddOrUpdateBody(Body body, bool ignoreSpeechOutput, DataSource dataSource, out Body addedOrUpdatedBody)
    {
        if (Bodies.TryGetValue(body.Id, out var existingBody))
        {
            if (dataSource != DataSource.Journal || !checkAndRemoveInvalidEdsmBody(existingBody))
            {
                if (!ignoreSpeechOutput)
                {
                    checkBodyAndSpeak(body, existingBody);
                }
                if (body.GetType() == typeof(Planet))
                {
                    ((Planet)existingBody).UpdatePlanet((Planet)body, dataSource);
                }
                if (body.GetType() == typeof(Star))
                {
                    ((Star)existingBody).UpdateStar((Star)body, dataSource);
                }
                if (body.GetType() == typeof(Body))
                {
                    existingBody.UpdateBody(body, dataSource);
                }
                log.Debug($"Body '{existingBody.Name}' ({existingBody.Id}) updated in system '{Name}' ({Id})");
                addedOrUpdatedBody = existingBody;
                return 2;
            }
            log.Info($"Invalid body to be updated, will replace existing body '{existingBody.Name}' with body '{body.Name}' from journal instead");
        }
        else if (Bodies.Values.FirstOrDefault((Body existing) => existing.Name == body.Name) is { } existingBodyByName)
        {
            log.Info($"Found a body with the same name '{body.Name}' and different ids ({body.Id}/{existingBodyByName.Id}) on updating");
            if (checkAndRemoveInvalidEdsmBody(existingBodyByName))
            {
                log.Info("Invalid body to be updated, will replace body '" + body.Name + "' with data from journal instead");
            }
            if (dataSource == DataSource.Edsm && body.Id == 0)
            {
                log.Error($"Could not add nor update body '{body.Name}' ({body.Id}) in system '{Name}' ({Id})");
                addedOrUpdatedBody = null!;
                return 0;
            }
        }
        if (_bodies.TryAdd(body.Id, body))
        {
            switch (dataSource)
            {
                case DataSource.Journal:
                    body.WasReadFromJournal = true;
                    break;
                case DataSource.Edsm:
                    body.WasReadFromEdsm = true;
                    break;
            }
            if (!ignoreSpeechOutput)
            {
                checkBodyAndSpeak(body);
            }
            log.Debug($"New body '{body.Name}' ({body.Id}) added to system '{Name}' ({Id})");
            addedOrUpdatedBody = body;
            return 1;
        }
        log.Error($"Could not add nor update body '{body.Name}' ({body.Id}) in system '{Name}' ({Id})");
        addedOrUpdatedBody = null!;
        return 0;
    }

    private bool checkAndRemoveInvalidEdsmBody(Body body)
    {
        if (!body.WasReadFromEdsmOnly || body.Id > 0)
        {
            return false;
        }
        if (_bodies.TryRemove(body.Id, out var removedBody))
        {
            log.Info($"Removed body '{removedBody.Name}' with id {removedBody.Id} - probably early data from EDSM with incorrect id");
            return true;
        }
        log.Error($"Could not remove body '{body.Name}' with id {body.Id} - probably early data from EDSM with incorrect id - might lead to incorrect data!");
        return false;
    }

    private void checkBodyAndSpeak(Body body, Body? existingBody = null)
    {
        if (existingBody == null && !body.WasDiscovered && body.Type != BodyType.Unknown)
        {
            SpeechOutputBody speechOutputBody = new SpeechOutputBody(body);
            if (body.Name == PrimaryStarName && body.Type == BodyType.Star)
            {
                SpeechProvider.SpeakFirstDiscoverySystem(new SpeechOutputSystem(this));
            }
            else
            {
                SpeechProvider.SpeakFirstDiscoveryBody(speechOutputBody);
                if (body.Type == BodyType.Planet)
                {
                    Planet obj = (Planet)body;
                    SpeechOutputPlanet speechOutputPlanet = new SpeechOutputPlanet(obj);
                    if (obj.IsTerraformable)
                    {
                        SpeechProvider.SpeakTerraformable(speechOutputPlanet);
                    }
                    if (obj.IsLandable)
                    {
                        SpeechProvider.SpeakLandable(speechOutputPlanet);
                    }
                }
            }
            if (body.HasRings)
            {
                SpeechProvider.SpeakRingCount(speechOutputBody, new SpeechOutputRingsCount(body));
                foreach (Ring ring in body.Rings.Values)
                {
                    SpeechProvider.SpeakRing(new SpeechOutputRing(ring, body), speechOutputBody);
                }
            }
        }
        if (body.Type == BodyType.Planet)
        {
            Planet newPlanet = (Planet)body;
            Planet? planet = null;
            if (existingBody != null && existingBody.Type == BodyType.Planet)
            {
                planet = (Planet)existingBody;
            }
            if (newPlanet.GeologicalCount > 0 && (planet == null || planet.GeologicalCount == 0))
            {
                SpeechProvider.SpeakGeologicalSignals(new SpeechOutputPlanet(newPlanet));
            }
            if (newPlanet.BiologicalCount > 0 && (planet == null || planet.BiologicalCount == 0))
            {
                SpeechProvider.SpeakBiologicalSignals(new SpeechOutputPlanet(newPlanet));
            }
        }
    }

    private static bool CheckStarClassForScoopable(string? starClass)
    {
        if (!string.IsNullOrEmpty(starClass))
        {
            string[] starClassParts = starClass.Split(' ');
            if (starClassParts.Length != 0 && _scoopableStarClasses.Contains(starClassParts[0]))
            {
                return true;
            }
        }
        return false;
    }
}
