using System;

namespace EDEA.Models;

/// <summary>
/// Status flags reported by the Elite Dangerous status file.
/// </summary>
[Flags]
public enum EdStatusFlags : long
{
    /// <summary>
    /// The ship is docked.
    /// </summary>
    Docked = 1L,

    /// <summary>
    /// The ship is landed.
    /// </summary>
    Landed = 2L,

    /// <summary>
    /// The landing gear is down.
    /// </summary>
    LandingGearDown = 4L,

    /// <summary>
    /// The shields are up.
    /// </summary>
    ShieldsUp = 8L,

    /// <summary>
    /// The ship is in supercruise.
    /// </summary>
    Supercruise = 0x10L,

    /// <summary>
    /// Flight assist is off.
    /// </summary>
    FlightAssistOff = 0x20L,

    /// <summary>
    /// Hardpoints are deployed.
    /// </summary>
    HardpointsDeployed = 0x40L,

    /// <summary>
    /// The ship is in a wing.
    /// </summary>
    InWing = 0x80L,

    /// <summary>
    /// The lights are on.
    /// </summary>
    LightsOn = 0x100L,

    /// <summary>
    /// The cargo scoop is deployed.
    /// </summary>
    CargoScoopDeployed = 0x200L,

    /// <summary>
    /// Silent running is active.
    /// </summary>
    SilentRunning = 0x400L,

    /// <summary>
    /// The ship is scooping fuel.
    /// </summary>
    ScoopingFuel = 0x800L,

    /// <summary>
    /// The SRV handbrake is on.
    /// </summary>
    SrvHandbrake = 0x1000L,

    /// <summary>
    /// The SRV turret view is in use.
    /// </summary>
    SrvUsingTurretView = 0x2000L,

    /// <summary>
    /// The SRV turret is retracted.
    /// </summary>
    SrvTurretRetracted = 0x4000L,

    /// <summary>
    /// SRV drive assist is active.
    /// </summary>
    SrvDriveAssist = 0x8000L,

    /// <summary>
    /// The frame shift drive is mass locked.
    /// </summary>
    FsdMassLocked = 0x10000L,

    /// <summary>
    /// The frame shift drive is charging.
    /// </summary>
    FsdCharging = 0x20000L,

    /// <summary>
    /// The frame shift drive is on cooldown.
    /// </summary>
    FsdCooldown = 0x40000L,

    /// <summary>
    /// Fuel is low.
    /// </summary>
    LowFuel = 0x80000L,

    /// <summary>
    /// The ship is overheating.
    /// </summary>
    OverHeating = 0x100000L,

    /// <summary>
    /// Latitude and longitude are available.
    /// </summary>
    HasLatLong = 0x200000L,

    /// <summary>
    /// The ship is in danger.
    /// </summary>
    IsInDanger = 0x400000L,

    /// <summary>
    /// The ship is being interdicted.
    /// </summary>
    BeingInterdicted = 0x800000L,

    /// <summary>
    /// The player is in the main ship.
    /// </summary>
    InMainShip = 0x1000000L,

    /// <summary>
    /// The player is in a fighter.
    /// </summary>
    InFighter = 0x2000000L,

    /// <summary>
    /// The player is in an SRV.
    /// </summary>
    InSRV = 0x4000000L,

    /// <summary>
    /// The HUD is in analysis mode.
    /// </summary>
    HudInAnalysisMode = 0x8000000L,

    /// <summary>
    /// Night vision is active.
    /// </summary>
    NightVision = 0x10000000L,

    /// <summary>
    /// Altitude is calculated from the average radius.
    /// </summary>
    AltitudeFromAverageRadius = 0x20000000L,

    /// <summary>
    /// The FSD jump is in progress.
    /// </summary>
    fsdJump = 0x40000000L,

    /// <summary>
    /// The SRV high beam is on.
    /// </summary>
    srvHighBeam = 0x80000000L
}
