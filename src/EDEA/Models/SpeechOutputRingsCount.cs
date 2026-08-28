using System;
using System.Linq;

namespace EDEA.Models;

public class SpeechOutputRingsCount : SpeechOutput
{
    public SpeechOutputRingsCount(Body body)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingsCount, Convert.ToString(body.Rings.Count));
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingsReserveLevel, Helpsters.GetRingReserveLevelName(body.RingsReserveLevel));
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingsTotalWidth, (body.Rings.Sum(r => r.Value.Width) / 1000.0).ToString("0.##"));
    }
}
