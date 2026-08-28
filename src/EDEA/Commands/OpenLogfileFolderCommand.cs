using System;
using System.Diagnostics;
using System.IO;
using log4net;

namespace EDEA.Commands;

/// <summary>
/// Command that opens the application log folder in Windows Explorer.
/// </summary>
public class OpenLogfileFolderCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(OpenLogfileFolderCommand));

    /// <summary>
    /// Opens the log folder located under the application data folder.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        try
        {
            string path = Path.Combine(Globals.AppDataFolder, "log");
            Process.Start("explorer.exe", path);
        }
        catch (Exception exception)
        {
            log.Error("Error on opening log file folder.", exception);
        }
    }
}
