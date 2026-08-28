using System;
using System.Diagnostics;
using System.IO;
using log4net;

namespace EDEA.Commands;

public class OpenLogfileFolderCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(OpenLogfileFolderCommand));

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
