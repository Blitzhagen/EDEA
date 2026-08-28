using System;
using System.Linq;

namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for the ring count of a body.
/// </summary>
public class SpeechOutputRingsCount : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputRingsCount"/> class.
    /// </summary>
    /// <param name="body">The body to describe.</param>
    public SpeechOutputRingsCount(Body body)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingsCount, Convert.ToString(body.Rings.Count));
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingsReserveLevel, Helpsters.GetRingReserveLevelName(body.RingsReserveLevel));
        Placeholders.Add(SpeechOutputPlaceholderKeys.RingsTotalWidth, (body.Rings.Sum(r => r.Value.Width) / 1000.0).ToString("0.##"));
    }
}
