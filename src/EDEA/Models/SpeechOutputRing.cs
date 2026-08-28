using System;
using System.Linq;

namespace EDEA.Models;

public class SpeechOutputRing : SpeechOutput
{
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
