using System;

namespace EDEA.Models;

[Flags]
public enum EdStatusFlags2
{
    OnFoot = 1,
    InTaxi = 2,
    InMulticrew = 4,
    OnFootInStation = 8,
    OnFootOnPlanet = 0x10,
    AimDownSight = 0x20,
    LowOxygen = 0x40,
    LowHealth = 0x80,
    Cold = 0x100,
    Hot = 0x200,
    VeryCold = 0x400,
    VeryHot = 0x800,
    GlideMode = 0x1000,
    OnFootInHangar = 0x2000,
    OnFootSocialSpace = 0x4000,
    OnFootExterior = 0x8000,
    BreathableAtmosphere = 0x10000,
    TelepresenceMulticrew = 0x20000,
    PhysicalMulticrew = 0x40000,
    FsdHyperdriveCharging = 0x80000
}
