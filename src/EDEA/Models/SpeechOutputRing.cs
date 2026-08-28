using System;
using System.Linq;

namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for a ring.
/// </summary>
public class SpeechOutputRing : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputRing"/> class.
    /// </summary>
    /// <param name="ring">The ring to describe.</param>
    /// <param name="body">The parent body.</param>
    public SpeechOutputRing(Ring ring, Body body)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingName, ring.Name);
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingType, Helpsters.GetRingTypeName(ring.Type));
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingMass, ring.Mass.ToString("0.##"));
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingWidth, ring.Width.ToString("0.##"));
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingDensity, ring.Density.ToString("0.##"));
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingsReserveLevel, Helpsters.GetRingReserveLevelName(body.RingsReserveLevel));
    }
}
