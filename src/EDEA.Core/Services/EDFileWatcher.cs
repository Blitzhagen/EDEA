using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

/// <summary>Represents a method that handles the NavRouteFileChanged event.</summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">The event data.</param>
public delegate void NavRouteFileChangedEventHandler(object? sender, EdFileEvent e);
/// <summary>Represents a method that handles the StatusFileChanged event.</summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">The event data.</param>
public delegate void StatusFileChangedEventHandler(object? sender, EdFileEvent e);
/// <summary>Represents a method that handles the JournalFileChanged event.</summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">The event data.</param>
/// <param name="isOldFile">A value indicating whether old file.</param>
public delegate void JournalFileChangedEventHandler(object? sender, EdFileEvent e, bool isOldFile);

/// <summary>Represents the EDFileWatcher class.</summary>
public class EDFileWatcher : FileSystemWatcher
{
    /// <summary>The instance field.</summary>
    private static EDFileWatcher? instance;

    /// <summary>The log field.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(EDFileWatcher));

    /// <summary>The _edFileEvents field.</summary>
    private readonly ConcurrentDictionary<string, EdFileEvent> _edFileEvents;

    /// <summary>Gets or sets the NavRouteFilePath.</summary>
    /// <value>A string value.</value>
    public string NavRouteFilePath { get; private set; } = string.Empty;

    /// <summary>Gets the NavRouteFileName.</summary>
    /// <value>A string value.</value>
    public string NavRouteFileName => System.IO.Path.GetFileName(NavRouteFilePath);

    /// <summary>Gets or sets the JournalFilePath.</summary>
    /// <value>A string value.</value>
    public string JournalFilePath { get; private set; } = string.Empty;

    /// <summary>Gets the JournalFileName.</summary>
    /// <value>A string value.</value>
    public string JournalFileName => System.IO.Path.GetFileName(JournalFilePath);

    /// <summary>Gets or sets the StatusFilePath.</summary>
    /// <value>A string value.</value>
    public string StatusFilePath { get; private set; } = string.Empty;

    /// <summary>Gets the StatusFileName.</summary>
    /// <value>A string value.</value>
    public string StatusFileName => System.IO.Path.GetFileName(StatusFilePath);

    /// <summary>Occurs when the NavRouteFileChanged event is raised.</summary>
    public event NavRouteFileChangedEventHandler NavRouteFileChanged = delegate
    {
    };

    /// <summary>Occurs when the StatusFileChanged event is raised.</summary>
    public event StatusFileChangedEventHandler StatusFileChanged = delegate
    {
    };

    /// <summary>Occurs when the JournalFileChanged event is raised.</summary>
    public event JournalFileChangedEventHandler JournalFileChanged = delegate
    {
    };

    /// <summary>Initializes a new instance of the EDFileWatcher class.</summary>
    private EDFileWatcher()
    {
        _edFileEvents = new ConcurrentDictionary<string, EdFileEvent>();
        try
        {
            var latestJournal = Helpsters.GetLatestJournalFile(Preferences.Other.EdSavedGamePath);
            if (latestJournal != null)
            {
                log.Info("Using " + Preferences.Other.EdSavedGamePath + " as path for ED journal files");
                base.Path = Preferences.Other.EdSavedGamePath;
                NavRouteFilePath = System.IO.Path.Combine(base.Path, "NavRoute.json");
                JournalFilePath = latestJournal.FullName;
                StatusFilePath = System.IO.Path.Combine(base.Path, "Status.json");
                log.Debug("Using " + NavRouteFileName + " as route file");
                log.Info("Using " + JournalFileName + " as journal file");
                log.Debug("Using " + StatusFileName + " as status file");
                base.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                base.IncludeSubdirectories = false;
                base.EnableRaisingEvents = true;
                base.Created += eDFileWatcher_EventRaised;
                base.Changed += eDFileWatcher_EventRaised;
                base.Deleted += eDFileWatcher_EventRaised;
                return;
            }
            throw new Exception("Folder does not contain any ED journal files");
        }
        catch (Exception exception)
        {
            log.Fatal("Path for ED journal files is invalid", exception);
            throw new InvalidOperationException("Could not determine the Elite Dangerous Saved Games folder where the Journal*.log files are stored. Please set the appropriate folder in the Configuration section in the preferences so that EDEA works correctly.");
        }
    }

    /// <summary>Performs the Instance operation.</summary>
    /// <returns>A EDFileWatcher result.</returns>
    public static EDFileWatcher Instance()
    {
        if (instance == null)
        {
            instance = new EDFileWatcher();
        }
        return instance;
    }

    /// <summary>Performs the eDFileWatcher_EventRaised operation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void eDFileWatcher_EventRaised(object? sender, FileSystemEventArgs e)
    {
        EdFileEvent edFileEvent = new EdFileEvent(e);
        log.Debug($"New event raised: {edFileEvent.Id}, {edFileEvent.EventTime}, {edFileEvent.ChangeType}");
        if (_edFileEvents.TryAdd(edFileEvent.Id, edFileEvent))
        {
            _ = edFileEvent.waitForMultipleEventTimer(handleEdFileEvent);
            log.Debug($"Added {edFileEvent.Id} to edFileEvents, count: {_edFileEvents.Count}");
        }
        else
        {
            log.Debug(edFileEvent.Id + " already exists in dFileEvents and was not added");
        }
    }

    /// <summary>Performs the handleEdFileEvent operation.</summary>
    /// <param name="edFileEvent">The EdFileEvent value of the edFileEvent parameter.</param>
    private void handleEdFileEvent(EdFileEvent edFileEvent)
    {
        if (edFileEvent.FileName == StatusFileName)
        {
            log.Debug($"{StatusFileName} was {edFileEvent.ChangeType}");
            if (edFileEvent.ChangeType == WatcherChangeTypes.Changed || edFileEvent.ChangeType == WatcherChangeTypes.Created)
            {
                StatusFileChanged?.Invoke(this, edFileEvent);
            }
            else if (edFileEvent.ChangeType == WatcherChangeTypes.Deleted)
            {
                log.Error(StatusFileName + " was deleted! EDEA won't be able to get status information from ED and will not work correctly anymore");
            }
        }
        else if (edFileEvent.FileName == JournalFileName)
        {
            log.Debug($"{JournalFileName} was {edFileEvent.ChangeType}");
            if (edFileEvent.ChangeType == WatcherChangeTypes.Changed)
            {
                JournalFileChanged?.Invoke(this, edFileEvent, isOldFile: false);
            }
            else if (edFileEvent.ChangeType == WatcherChangeTypes.Deleted)
            {
                log.Warn(JournalFileName + " was deleted! EDEA won't be able to get journal information from ED, trying to find another journal file ...");
                var latestJournal = Helpsters.GetLatestJournalFile(base.Path);
                if (latestJournal != null)
                {
                    JournalFilePath = latestJournal.FullName;
                    JournalFileChanged?.Invoke(this, edFileEvent, isOldFile: false);
                }
                else
                {
                    log.Error("No journal file could be found! EDEA will not work correctly anymore!");
                }
            }
        }
        else if (edFileEvent.FileName == NavRouteFileName)
        {
            log.Debug($"{NavRouteFileName} was {edFileEvent.ChangeType}");
            if (edFileEvent.ChangeType == WatcherChangeTypes.Changed || edFileEvent.ChangeType == WatcherChangeTypes.Created)
            {
                NavRouteFileChanged?.Invoke(this, edFileEvent);
            }
            else if (edFileEvent.ChangeType == WatcherChangeTypes.Deleted)
            {
                log.Error(NavRouteFileName + " was deleted! EDEA won't be able to get route information from ED and will not work correctly anymore");
            }
        }
        else if (Helpsters.GetLatestJournalFile(base.Path)?.FullName == edFileEvent.FilePath)
        {
            JournalFilePath = edFileEvent.FilePath;
            if (edFileEvent.ChangeType == WatcherChangeTypes.Created)
            {
                log.Info("New journal file " + edFileEvent.FileName + " was created, using it as current one");
                JournalFileChanged?.Invoke(this, edFileEvent, isOldFile: false);
            }
            else if (edFileEvent.ChangeType == WatcherChangeTypes.Changed)
            {
                log.Info("Old journal file " + edFileEvent.FileName + " was changed, using it as current one");
                JournalFileChanged?.Invoke(this, edFileEvent, isOldFile: true);
            }
        }
        else
        {
            log.Debug("Event " + edFileEvent.Id + " has no consequences and will be igenored");
        }
        if (_edFileEvents.TryRemove(edFileEvent.Id, out var removedEvent))
        {
            log.Debug($"Removed {removedEvent.Id} from edFileEvents, count: {_edFileEvents.Count}");
        }
        else
        {
            log.Warn("Could not remove " + edFileEvent.Id + " from edFileEvents, this may lead to inconsistencies");
        }
    }
}
