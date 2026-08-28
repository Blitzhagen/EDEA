using System;

namespace EDEA.Models;

[Flags]
public enum EdStatusFlags : long
{
    Docked = 1L,
    Landed = 2L,
    LandingGearDown = 4L,
    ShieldsUp = 8L,
    Supercruise = 0x10L,
    FlightAssistOff = 0x20L,
    HardpointsDeployed = 0x40L,
    InWing = 0x80L,
    LightsOn = 0x100L,
    CargoScoopDeployed = 0x200L,
    SilentRunning = 0x400L,
    ScoopingFuel = 0x800L,
    SrvHandbrake = 0x1000L,
    SrvUsingTurretView = 0x2000L,
    SrvTurretRetracted = 0x4000L,
    SrvDriveAssist = 0x8000L,
    FsdMassLocked = 0x10000L,
    FsdCharging = 0x20000L,
    FsdCooldown = 0x40000L,
    LowFuel = 0x80000L,
    OverHeating = 0x100000L,
    HasLatLong = 0x200000L,
    IsInDanger = 0x400000L,
    BeingInterdicted = 0x800000L,
    InMainShip = 0x1000000L,
    InFighter = 0x2000000L,
    InSRV = 0x4000000L,
    HudInAnalysisMode = 0x8000000L,
    NightVision = 0x10000000L,
    AltitudeFromAverageRadius = 0x20000000L,
    fsdJump = 0x40000000L,
    srvHighBeam = 0x80000000L
}
