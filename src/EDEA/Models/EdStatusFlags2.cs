using System;

namespace EDEA.Models;

/// <summary>
/// Additional status flags reported by the Elite Dangerous status file.
/// </summary>
[Flags]
public enum EdStatusFlags2
{
    /// <summary>
    /// The player is on foot.
    /// </summary>
    OnFoot = 1,

    /// <summary>
    /// The player is in a taxi.
    /// </summary>
    InTaxi = 2,

    /// <summary>
    /// The player is in multicrew.
    /// </summary>
    InMulticrew = 4,

    /// <summary>
    /// The player is on foot inside a station.
    /// </summary>
    OnFootInStation = 8,

    /// <summary>
    /// The player is on foot on a planet.
    /// </summary>
    OnFootOnPlanet = 0x10,

    /// <summary>
    /// The player is aiming down sights.
    /// </summary>
    AimDownSight = 0x20,

    /// <summary>
    /// Oxygen is low.
    /// </summary>
    LowOxygen = 0x40,

    /// <summary>
    /// Health is low.
    /// </summary>
    LowHealth = 0x80,

    /// <summary>
    /// The player is cold.
    /// </summary>
    Cold = 0x100,

    /// <summary>
    /// The player is hot.
    /// </summary>
    Hot = 0x200,

    /// <summary>
    /// The player is very cold.
    /// </summary>
    VeryCold = 0x400,

    /// <summary>
    /// The player is very hot.
    /// </summary>
    VeryHot = 0x800,

    /// <summary>
    /// Glide mode is active.
    /// </summary>
    GlideMode = 0x1000,

    /// <summary>
    /// The player is on foot inside a hangar.
    /// </summary>
    OnFootInHangar = 0x2000,

    /// <summary>
    /// The player is on foot in a social space.
    /// </summary>
    OnFootSocialSpace = 0x4000,

    /// <summary>
    /// The player is on foot outside.
    /// </summary>
    OnFootExterior = 0x8000,

    /// <summary>
    /// The atmosphere is breathable.
    /// </summary>
    BreathableAtmosphere = 0x10000,

    /// <summary>
    /// The multicrew session uses telepresence.
    /// </summary>
    TelepresenceMulticrew = 0x20000,

    /// <summary>
    /// The multicrew session is physical.
    /// </summary>
    PhysicalMulticrew = 0x40000,

    /// <summary>
    /// The FSD hyperdrive is charging.
    /// </summary>
    FsdHyperdriveCharging = 0x80000
}
