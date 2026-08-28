using System;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using EDEA.Services;
using log4net;

namespace EDEA.Models;

public class Ship
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Ship));

    public string Id { get; protected set; } = string.Empty;
    public string Name { get; protected set; } = string.Empty;
    public string Identification { get; protected set; } = string.Empty;
    public double BaseMass { get; protected set; }
    public int CargoCapacity { get; protected set; }
    public double MaxJumpRange { get; protected set; }
    public double CurrentJumpRange { get; protected set; }
    public double MainFuelCapacity { get; protected set; }
    public double ReserveFuelCapacity { get; protected set; }
    public HyperdriveClassification? FrameShiftDrive { get; protected set; }
    public GuardianFsdBoosterClassification? GuardianFsdBooster { get; protected set; }
    public JsonArray? SLEF { get; protected set; }
    public int CargoCount { get; set; }
    public double MainFuel { get; set; }
    public double ReserveFuel { get; set; }
    public bool JetConeBoost { get; set; }
    public int JetConeBoostValue { get; set; }

    public Ship()
    {
    }

    public Ship(JsonNode? loadout)
    {
        if (loadout is null)
            return;

        try
        {
            Id = Helpsters.ConvertJObjectValue<string>(loadout, "Ship").ToLower();
            Name = Helpsters.ConvertJObjectValue<string>(loadout, "ShipName");
            Identification = Helpsters.ConvertJObjectValue<string>(loadout, "ShipIdent");
            BaseMass = Helpsters.ConvertJObjectValue(loadout, "UnladenMass", 0.0);
            CargoCapacity = Helpsters.ConvertJObjectValue(loadout, "CargoCapacity", 0);
            MaxJumpRange = Helpsters.ConvertJObjectValue(loadout, "MaxJumpRange", 0.0);
            var fuelCapacity = Helpsters.ConvertJObjectValue<JsonObject?>(loadout, "FuelCapacity");
            MainFuelCapacity = fuelCapacity != null ? Helpsters.ConvertJObjectValue(fuelCapacity, "Main", 0.0) : 0.0;
            ReserveFuelCapacity = fuelCapacity != null ? Helpsters.ConvertJObjectValue(fuelCapacity, "Reserve", 0.0) : 0.0;

            var modules = Helpsters.ConvertJObjectValue<JsonArray?>(loadout, "Modules");
            if (modules != null)
            {
                foreach (var module in modules.OfType<JsonObject>())
                {
                    string slot = Helpsters.ConvertJObjectValue<string>(module, "Slot");
                    string item = Helpsters.ConvertJObjectValue<string>(module, "Item");

                    if (slot == "FrameShiftDrive")
                    {
                        if (EdDataProvider.ModuleClassifications.TryGetValue(item, out var moduleClassification) && moduleClassification is HyperdriveClassification hyperdrive)
                        {
                            FrameShiftDrive = new HyperdriveClassification
                            {
                                Id = hyperdrive.Id,
                                Type = hyperdrive.Type,
                                Name = hyperdrive.Name,
                                Class = hyperdrive.Class,
                                Rating = hyperdrive.Rating,
                                FuelPower = hyperdrive.FuelPower,
                                FuelMultiplier = hyperdrive.FuelMultiplier,
                                OptimalMass = hyperdrive.OptimalMass,
                                MaxFuelPerJump = hyperdrive.MaxFuelPerJump,
                                JumpBoostMultiplier = hyperdrive.JumpBoostMultiplier
                            };
                        }
                        else
                        {
                            log.Error($"Found no matching frame shift drive for {(item)}! Is this a brand new module?");
                        }

                        var engineering = Helpsters.ConvertJObjectValue<JsonObject?>(module, "Engineering");
                        if (FrameShiftDrive != null && engineering != null)
                        {
                            var modifiers = Helpsters.ConvertJObjectValue<JsonArray?>(engineering, "Modifiers");
                            if (modifiers != null)
                            {
                                foreach (var modifier in modifiers.OfType<JsonObject>())
                                {
                                    if (Helpsters.ConvertJObjectValue<string>(modifier, "Label") == "FSDOptimalMass")
                                    {
                                        double parsedValue = Helpsters.ConvertJObjectValue<double>(modifier, "Value", 0.0);
                                        if (parsedValue > 0.0)
                                        {
                                            FrameShiftDrive.OptimalMass = parsedValue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (item.ToLowerInvariant().Contains("guardianfsdbooster") && EdDataProvider.ModuleClassifications.TryGetValue(item, out var moduleValue) && moduleValue is GuardianFsdBoosterClassification guardian)
                    {
                        GuardianFsdBooster = new GuardianFsdBoosterClassification
                        {
                            Id = guardian.Id,
                            Type = guardian.Type,
                            Name = guardian.Name,
                            Class = guardian.Class,
                            Rating = guardian.Rating,
                            JumpBoost = guardian.JumpBoost
                        };
                    }
                }
            }

            SLEF = new JsonArray
            {
                new JsonObject
                {
                    ["header"] = new JsonObject
                    {
                        ["appName"] = "EDEA",
                        ["appVersion"] = Globals.AppVersionString,
                        ["appUrl"] = "https://edea.app/EDEA/"
                    },
                    ["data"] = loadout
                }
            };

            UpdateJumpRange();
        }
        catch
        {
            throw;
        }
    }

    public void UpdateJumpRange()
    {
        if (FrameShiftDrive == null || BaseMass <= 0.0 || MaxJumpRange <= 0.0)
        {
            CurrentJumpRange = MaxJumpRange;
            return;
        }

        double maxFuelPerJump = FrameShiftDrive.MaxFuelPerJump;
        if (maxFuelPerJump <= 0.0 || MainFuel <= 0.0)
        {
            CurrentJumpRange = MaxJumpRange;
            return;
        }

        double fuelForJump = Math.Min(MainFuel, maxFuelPerJump);
        double massForMaxJump = BaseMass + maxFuelPerJump;
        double currentMass = BaseMass + CargoCount + MainFuel;
        if (massForMaxJump <= 0.0 || currentMass <= 0.0)
        {
            CurrentJumpRange = MaxJumpRange;
            return;
        }

        double fuelRatio = fuelForJump / maxFuelPerJump;
        double massRatio = massForMaxJump / currentMass;
        double exponent = 1.0 / FrameShiftDrive.FuelPower;

        CurrentJumpRange = MaxJumpRange * massRatio * Math.Pow(fuelRatio, exponent);
        log.Debug($"Updated current jump range for '{Name}': {CurrentJumpRange:F6} LY (Max: {MaxJumpRange:F6} LY, BaseMass: {BaseMass}, Fuel: {MainFuel}, Cargo: {CargoCount})");
    }
}
