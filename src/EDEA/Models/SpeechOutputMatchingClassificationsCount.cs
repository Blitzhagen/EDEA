using System;

namespace EDEA.Models;

/// <summary>
/// Provides speech output placeholders for the number of matching classifications.
/// </summary>
public class SpeechOutputMatchingClassificationsCount : SpeechOutput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputMatchingClassificationsCount"/> class.
    /// </summary>
    /// <param name="classificationsCount">The number of matching classifications.</param>
    public SpeechOutputMatchingClassificationsCount(int classificationsCount)
    {
        Placeholders.Add(SpeechOutputPlaceholderKeys.PoiCriteriaSetsCount, Convert.ToString(classificationsCount));
    }
}
