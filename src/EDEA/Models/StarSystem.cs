using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EDEA.Services;
using log4net;

namespace EDEA.Models;

/// <summary>
/// Represents a star system with its bodies and metadata.
/// </summary>
public class StarSystem
{
    /// <summary>
    /// The logger for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(StarSystem));

    /// <summary>
    /// The set of star classes considered scoopable.
    /// </summary>
    private static readonly HashSet<string> _scoopableStarClasses = new(StringComparer.OrdinalIgnoreCase)
    {
        "O", "B", "A", "F", "G", "K", "M", "Refuel"
    };

    /// <summary>
    /// The bodies in this system, keyed by body identifier.
    /// </summary>
    private readonly ConcurrentDictionary<int, Body> _bodies;

    /// <summary>
    /// Gets the system identifier.
    /// </summary>
    /// <value>The system identifier.</value>
    public long Id { get; init; }

    /// <summary>
    /// Gets the system name.
    /// </summary>
    /// <value>The system name.</value>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the X coordinate of the star position.
    /// </summary>
    /// <value>The X coordinate, or <see langword="null"/> if not specified.</value>
    public double? StarPositionX { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of the star position.
    /// </summary>
    /// <value>The Y coordinate, or <see langword="null"/> if not specified.</value>
    public double? StarPositionY { get; set; }

    /// <summary>
    /// Gets or sets the Z coordinate of the star position.
    /// </summary>
    /// <value>The Z coordinate, or <see langword="null"/> if not specified.</value>
    public double? StarPositionZ { get; set; }

    /// <summary>
    /// Gets or sets the primary star class.
    /// </summary>
    /// <value>The star class, or <see langword="null"/> if not specified.</value>
    public string? StarClass { get; set; }

    /// <summary>
    /// Gets or sets the primary star name.
    /// </summary>
    /// <value>The primary star name, or <see langword="null"/> if not specified.</value>
    public string? PrimaryStarName { get; set; }

    /// <summary>
    /// Gets a value indicating whether the primary star is scoopable.
    /// </summary>
    /// <value><see langword="true"/> if the primary star is scoopable; otherwise, <see langword="false"/>.</value>
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

    /// <summary>
    /// Gets the bodies in this system.
    /// </summary>
    /// <value>A read-only dictionary of bodies keyed by identifier.</value>
    public IReadOnlyDictionary<int, Body> Bodies => _bodies;

    /// <summary>
    /// Gets or sets the jump distance.
    /// </summary>
    /// <value>The jump distance.</value>
    public int JumpDistance { get; set; }

    /// <summary>
    /// Gets or sets the jump distance in light years.
    /// </summary>
    /// <value>The jump distance in light years.</value>
    public double JumpDistanceLy { get; set; }

    /// <summary>
    /// Gets or sets the total known body count.
    /// </summary>
    /// <value>The total body count.</value>
    public int TotalBodyCount { get; set; }

    /// <summary>
    /// Gets or sets the total non-body count.
    /// </summary>
    /// <value>The total non-body count.</value>
    public int TotalNonBodyCount { get; set; }

    /// <summary>
    /// Gets or sets the number of bodies from a nav beacon scan.
    /// </summary>
    /// <value>The nav beacon scan body count.</value>
    public int NavBeaconScanBodyCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether all bodies were found.
    /// </summary>
    /// <value><see langword="true"/> if all bodies found; otherwise, <see langword="false"/>.</value>
    public bool AllBodiesFound { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this system was read from a journal.
    /// </summary>
    /// <value><see langword="true"/> if read from a journal; otherwise, <see langword="false"/>.</value>
    public bool WasReadFromJournal { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this system was read from EDSM.
    /// </summary>
    /// <value><see langword="true"/> if read from EDSM; otherwise, <see langword="false"/>.</value>
    public bool WasReadFromEdsm { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this system was requested from EDSM.
    /// </summary>
    /// <value><see langword="true"/> if requested from EDSM; otherwise, <see langword="false"/>.</value>
    public bool WasRequestedFromEdsm { get; set; }

    /// <summary>
    /// Gets a value indicating whether this system was read from EDSM only.
    /// </summary>
    /// <value><see langword="true"/> if read from EDSM and not from a journal; otherwise, <see langword="false"/>.</value>
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

    /// <summary>
    /// Gets a value indicating whether this system needs an EDSM system update.
    /// </summary>
    /// <value><see langword="true"/> if EDSM update is needed; otherwise, <see langword="false"/>.</value>
    public bool NeedsEdsmSystemUpdate => !WasReadFromEdsm;

    /// <summary>
    /// Gets a value indicating whether this system needs an EDSM body update.
    /// </summary>
    /// <value><see langword="true"/> if EDSM body update is needed; otherwise, <see langword="false"/>.</value>
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

    /// <summary>
    /// Gets or sets a value indicating whether this system is a past system in the route.
    /// </summary>
    /// <value><see langword="true"/> if past; otherwise, <see langword="false"/>.</value>
    public bool IsPastSystemInRoute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this system is the current system in the route.
    /// </summary>
    /// <value><see langword="true"/> if current; otherwise, <see langword="false"/>.</value>
    public bool IsCurrentSystemInRoute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this system is the jump destination in the route.
    /// </summary>
    /// <value><see langword="true"/> if jump destination; otherwise, <see langword="false"/>.</value>
    public bool IsJumpDestinationSystemInRoute { get; set; }

    /// <summary>
    /// Gets a value indicating whether this system is ahead in the route.
    /// </summary>
    /// <value><see langword="true"/> if ahead in route; otherwise, <see langword="false"/>.</value>
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

    /// <summary>
    /// Gets or sets the EDSM name.
    /// </summary>
    /// <value>The EDSM name, or <see langword="null"/> if not specified.</value>
    public string? EdsmName { get; set; }

    /// <summary>
    /// Gets or sets the EDSM primary star type.
    /// </summary>
    /// <value>The EDSM primary star type, or <see langword="null"/> if not specified.</value>
    public string? EdsmPrimaryStarType { get; set; }

    /// <summary>
    /// Gets or sets the EDSM primary star name.
    /// </summary>
    /// <value>The EDSM primary star name, or <see langword="null"/> if not specified.</value>
    public string? EdsmPrimaryStarName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the EDSM primary star is scoopable.
    /// </summary>
    /// <value><see langword="true"/> if the EDSM primary star is scoopable; otherwise, <see langword="false"/>.</value>
    public bool EdsmPrimaryStarIsScoopable { get; set; }

    /// <summary>
    /// Gets or sets the EDSM total body count.
    /// </summary>
    /// <value>The EDSM total body count, or <see langword="null"/> if not specified.</value>
    public int? EdsmTotalBodyCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this system is part of the trip history.
    /// </summary>
    /// <value><see langword="true"/> if part of trip history; otherwise, <see langword="false"/>.</value>
    public bool IsTripHistory { get; set; }

    /// <summary>
    /// Gets or sets the population.
    /// </summary>
    /// <value>The population.</value>
    public long Population { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StarSystem"/> class.
    /// </summary>
    public StarSystem()
    {
        _bodies = new ConcurrentDictionary<int, Body>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StarSystem"/> class.
    /// </summary>
    /// <param name="id">The system identifier.</param>
    /// <param name="name">The system name.</param>
    public StarSystem(long id, string name)
    {
        _bodies = new ConcurrentDictionary<int, Body>();
        Id = id;
        Name = name;
        IsTripHistory = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StarSystem"/> class.
    /// </summary>
    /// <param name="id">The system identifier.</param>
    /// <param name="name">The system name.</param>
    /// <param name="isTripHistory">Whether this system is part of the trip history.</param>
    public StarSystem(long id, string name, long isTripHistory)
        : this(id, name)
    {
        IsTripHistory = Convert.ToBoolean(isTripHistory);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StarSystem"/> class from persisted data.
    /// </summary>
    /// <param name="id">The system identifier.</param>
    /// <param name="name">The system name.</param>
    /// <param name="starClass">The primary star class.</param>
    /// <param name="primaryStarName">The primary star name.</param>
    /// <param name="totalBodyCount">The total body count.</param>
    /// <param name="totalNonBodyCount">The total non-body count.</param>
    /// <param name="wasReadFromJournal">Whether this system was read from a journal.</param>
    /// <param name="wasReadFromEdsm">Whether this system was read from EDSM.</param>
    /// <param name="wasRequestedFromEdsm">Whether this system was requested from EDSM.</param>
    /// <param name="edsmName">The EDSM name.</param>
    /// <param name="edsmPrimaryStarType">The EDSM primary star type.</param>
    /// <param name="edsmPrimaryStarName">The EDSM primary star name.</param>
    /// <param name="edsmPrimaryStarIsScoopable">Whether the EDSM primary star is scoopable.</param>
    /// <param name="edsmTotalBodyCount">The EDSM total body count, or <see langword="null"/> if unspecified.</param>
    /// <param name="starPositionX">The X coordinate of the star position.</param>
    /// <param name="starPositionY">The Y coordinate of the star position.</param>
    /// <param name="starPositionZ">The Z coordinate of the star position.</param>
    /// <param name="isTripHistory">Whether this system is part of the trip history.</param>
    /// <param name="allBodiesFound">Whether all bodies were found.</param>
    /// <param name="population">The population.</param>
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

    /// <summary>
    /// Adds or updates a body in this system.
    /// </summary>
    /// <param name="body">The body to add or update.</param>
    /// <param name="ignoreSpeechOutput">Whether to skip speech output.</param>
    /// <param name="dataSource">The data source that provided the body.</param>
    /// <param name="addedOrUpdatedBody">The body that was added or updated.</param>
    /// <returns>1 if added, 2 if updated, 0 on failure.</returns>
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

    /// <summary>
    /// Removes an invalid EDSM body if it was added without a valid identifier.
    /// </summary>
    /// <param name="body">The body to check and possibly remove.</param>
    /// <returns><see langword="true"/> if the body was removed; otherwise, <see langword="false"/>.</returns>
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

    /// <summary>
    /// Checks the body and triggers speech output for relevant events.
    /// </summary>
    /// <param name="body">The new or updated body.</param>
    /// <param name="existingBody">The existing body, or <see langword="null"/> if the body is new.</param>
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

    /// <summary>
    /// Checks whether the specified star class is scoopable.
    /// </summary>
    /// <param name="starClass">The star class to check.</param>
    /// <returns><see langword="true"/> if the star class is scoopable; otherwise, <see langword="false"/>.</returns>
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
