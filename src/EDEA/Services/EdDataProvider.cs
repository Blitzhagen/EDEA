using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EDEA.Models;
using log4net;

namespace EDEA.Services;

public static class EdDataProvider
{
    public static Dictionary<string, ModuleClassification> ModuleClassifications = new Dictionary<string, ModuleClassification>();

    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(EdDataProvider));

    private static string ReadEdDataFile(string fileName)
    {
        string rootPath = Path.Combine(Globals.ApplicationFolder, fileName);
        if (File.Exists(rootPath))
        {
            return File.ReadAllText(rootPath);
        }

        string resourcesPath = Path.Combine(Globals.ApplicationFolder, "Resources", fileName);
        if (File.Exists(resourcesPath))
        {
            return File.ReadAllText(resourcesPath);
        }

        throw new FileNotFoundException($"Could not find ED data file '{fileName}'", resourcesPath);
    }

    static EdDataProvider()
    {
        try
        {
            string mcDat = ReadEdDataFile("mc.dat");
            string mcJson = Encoding.UTF8.GetString(Convert.FromBase64String(mcDat));

            var moduleData = JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(mcJson);

            if (moduleData != null)
            {
                foreach (JsonObject moduleObject in moduleData.Values)
                {
                    string? type = Helpsters.ConvertJObjectValue<string>(moduleObject, "Type");
                    if (type == "cfsd" || type == "cfsdo")
                    {
                        HyperdriveClassification? hyperdrive = moduleObject.Deserialize<HyperdriveClassification>();
                        if (hyperdrive != null && !string.IsNullOrEmpty(hyperdrive.Id))
                        {
                            hyperdrive.JumpBoostMultiplier = hyperdrive.Id == "int_hyperdrive_overcharge_size8_class5_overchargebooster_mkii" ? 6.0 : 4.0;
                            ModuleClassifications[hyperdrive.Id] = hyperdrive;
                        }
                    }
                    else if (type == "ifsdb")
                    {
                        GuardianFsdBoosterClassification? guardian = moduleObject.Deserialize<GuardianFsdBoosterClassification>();
                        if (guardian != null && !string.IsNullOrEmpty(guardian.Id))
                        {
                            ModuleClassifications[guardian.Id] = guardian;
                        }
                    }
                }
            }
        }
        catch (Exception exception)
        {
            log.Error("Error reading Ed data!", exception);
        }
    }
}
