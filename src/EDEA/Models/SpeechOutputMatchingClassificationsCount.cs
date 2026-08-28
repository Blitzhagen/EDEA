using System;

namespace EDEA.Models;

public class SpeechOutputMatchingClassificationsCount : SpeechOutput
{
    public SpeechOutputMatchingClassificationsCount(int classificationsCount)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.PoiCriteriaSetsCount, Convert.ToString(classificationsCount));
    }
}
