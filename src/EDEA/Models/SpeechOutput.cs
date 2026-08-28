using System.Collections.Generic;

namespace EDEA.Models;

public class SpeechOutput
{
    public Dictionary<SpeechOutputPlaceholderKeys, string> Placeholders { get; set; } = new();
}
