using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

public delegate void JournalUpdatedEventHandler(object? sender, bool sequelRead, IEnumerable<string> lastJournalAddition);

public class JournalStore
{
    private static JournalStore? instance;

    private static readonly ILog log = LogManager.GetLogger(typeof(JournalStore));

    private readonly EDFileWatcher _fileWatcher;

    private readonly List<byte> _journal;

    private readonly List<byte> _lastJournalAddition;

    private readonly UTF8Encoding _encoding;

    private readonly int _readPufferSize;

    private readonly System.Timers.Timer _touchJournalFileTimer;

    private readonly int _touchJournalFileTimerInterval = 5000;

    private long journalPosition;

    private bool readingJournalFile;

    public event JournalUpdatedEventHandler JournalUpdated = delegate
    {
    };

    private JournalStore(EDFileWatcher eDFileWatcher)
    {
        _fileWatcher = eDFileWatcher;
        _fileWatcher.JournalFileChanged += _fileWatcher_JournalFileChanged;
        _journal = new List<byte>();
        _lastJournalAddition = new List<byte>();
        _encoding = new UTF8Encoding();
        _readPufferSize = 1024;
        journalPosition = 0L;
        readingJournalFile = false;
        _touchJournalFileTimer = new System.Timers.Timer();
        _touchJournalFileTimer.Elapsed += _touchJournalFileTimer_Elapsed;
        _touchJournalFileTimer.Interval = _touchJournalFileTimerInterval;
        _touchJournalFileTimer.Enabled = true;
    }

    public static JournalStore Instance(EDFileWatcher eDFileWatcher)
    {
        if (instance == null)
        {
            instance = new JournalStore(eDFileWatcher);
        }
        return instance;
    }

    private async void _fileWatcher_JournalFileChanged(object? sender, EdFileEvent e, bool isOldFile)
    {
        try
        {

            _touchJournalFileTimer.Stop();
            _touchJournalFileTimer.Start();
            if ((e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Deleted) | isOldFile)
            {
                _journal.Clear();
                journalPosition = 0L;
                log.Info($"Using {(isOldFile ? "last modified" : ((e.ChangeType == WatcherChangeTypes.Created) ? "new" : "existing"))} file {_fileWatcher.JournalFileName} as journal file");
            }
            await readJournalFile();

        }
        catch (Exception exception)
        {
            log.Error("Error in _fileWatcher_JournalFileChanged", exception);
        }
    }

    private void _touchJournalFileTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        _ = readJournalFile();
    }

    public async Task<IEnumerable<string>> GetJournal()
    {
        if (journalPosition == 0L)
        {
            await readJournalFile();
        }
        return convertBytesToStrings(_journal);
    }

    private IEnumerable<string> convertBytesToStrings(List<byte> byteList)
    {
        using var stringReader = new StringReader(_encoding.GetString(byteList.ToArray()));
        List<string> lines = new List<string>();
        string? line;
        while ((line = stringReader.ReadLine()) != null)
        {
            lines.Add(line);
        }
        return lines;
    }

    private async Task readJournalFile()
    {
        if (_fileWatcher.JournalFilePath == null)
        {
            return;
        }
        if (readingJournalFile)
        {
            log.Debug("Journal file " + _fileWatcher.JournalFilePath + " is currently being read, aborting parallel read request");
            return;
        }
        readingJournalFile = true;
        Stopwatch journalWatch = Stopwatch.StartNew();
        for (int i = 0; i < 10; i++)
        {
            try
            {
                bool sequelRead = false;
                if (journalPosition != 0L)
                {
                    sequelRead = true;
                }
                using (FileStream fs = File.Open(_fileWatcher.JournalFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fs.Position = journalPosition;
                    _lastJournalAddition.Clear();
                    byte[] puffer = new byte[_readPufferSize];
                    bool eof = false;
                    while (!eof)
                    {
                        int bytesRead = await fs.ReadAsync(puffer, 0, puffer.Length);
                        if (bytesRead < puffer.Length)
                        {
                            eof = true;
                        }
                        byte[] collection = puffer.Take(bytesRead).ToArray();
                        _journal.AddRange(collection);
                        if (sequelRead)
                        {
                            _lastJournalAddition.AddRange(collection);
                        }
                        journalPosition = fs.Position;
                    }
                    fs.Close();
                }
                long elapsedMilliseconds = journalWatch.ElapsedMilliseconds;
                journalWatch.Stop();
                if (!sequelRead || _lastJournalAddition.Count > 0)
                {
                    log.Debug($"Journal file {_fileWatcher.JournalFileName} read in {elapsedMilliseconds}ms: total {_journal.Count} bytes, last addition {_lastJournalAddition.Count} bytes, sequel read {sequelRead}");
                    JournalUpdated?.Invoke(this, sequelRead, convertBytesToStrings(_lastJournalAddition));
                }
                break;
            }
            catch (Exception exception)
            {
                if (10 - i > 1)
                {
                    log.Warn($"Journal file {_fileWatcher.JournalFilePath} missing or locked, waiting for {500}ms, retry {i + 1} of {10}", exception);
                }
                else
                {
                    log.Error($"Journal file {_fileWatcher.JournalFilePath} missing or locked, giving up after {i + 1} retries", exception);
                }
                await Task.Delay(500);
                continue;
            }
        }
        readingJournalFile = false;
    }
}
