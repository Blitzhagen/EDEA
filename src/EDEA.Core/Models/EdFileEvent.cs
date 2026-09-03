using System;
using System.IO;
using System.Threading.Tasks;

namespace EDEA.Models;

/// <summary>
/// Represents a file system event for an Elite Dangerous journal file.
/// </summary>
public class EdFileEvent
{
    /// <summary>
    /// The debounce timer in milliseconds for multiple events.
    /// </summary>
    private static int multipleEventTimer = 170;

    /// <summary>
    /// Gets the event identifier.
    /// </summary>
    /// <value>The event identifier.</value>
    public string Id { get; }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    /// <value>The file name.</value>
    public string FileName { get; }

    /// <summary>
    /// Gets the file path.
    /// </summary>
    /// <value>The file path.</value>
    public string FilePath { get; }

    /// <summary>
    /// Gets the type of change that occurred.
    /// </summary>
    /// <value>The type of change.</value>
    public WatcherChangeTypes ChangeType { get; }

    /// <summary>
    /// Gets the event time in ticks.
    /// </summary>
    /// <value>The event time in ticks.</value>
    public long EventTime { get; }

    /// <summary>
    /// Occurs when the file content has changed.
    /// </summary>
    public event FileSystemEventHandler? EdFileChanged = delegate
    {
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="EdFileEvent"/> class.
    /// </summary>
    /// <param name="fileSystemEventArgs">The file system event arguments.</param>
    public EdFileEvent(FileSystemEventArgs fileSystemEventArgs)
    {
        EventTime = DateTime.UtcNow.Ticks;
        FileName = fileSystemEventArgs.Name ?? string.Empty;
        FilePath = fileSystemEventArgs.FullPath ?? string.Empty;
        if (!File.Exists(FilePath) && File.Exists(FileName))
        {
            FilePath = FileName;
            FileName = Path.GetFileName(FilePath);
        }
        ChangeType = fileSystemEventArgs.ChangeType;
        Id = $"{FileName}_{ChangeType}";
    }

    /// <summary>
    /// Waits for the multiple event timer and then invokes the callback.
    /// </summary>
    /// <param name="eventCallBack">The callback to invoke when the timer elapses.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task waitForMultipleEventTimer(Action<EdFileEvent> eventCallBack)
    {
        await Task.Delay(multipleEventTimer);
        eventCallBack(this);
    }
}
