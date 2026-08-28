using System;
using System.IO;
using System.Threading.Tasks;

namespace EDEA.Models;

public class EdFileEvent
{
    private static int multipleEventTimer = 170;

    public string Id { get; }

    public string FileName { get; }

    public string FilePath { get; }

    public WatcherChangeTypes ChangeType { get; }

    public long EventTime { get; }

    public event FileSystemEventHandler? EdFileChanged = delegate
    {
    };

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

    public async Task waitForMultipleEventTimer(Action<EdFileEvent> eventCallBack)
    {
        await Task.Delay(multipleEventTimer);
        eventCallBack(this);
    }
}
