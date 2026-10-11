using System.Collections.Generic;

namespace SayIt
{
    public class VoiceInfo
    {
        public string Name { get; set; } = "";
        public string ShortName { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Locale { get; set; } = "";
        public string LocalName { get; set; } = "";
        public IReadOnlyList<string> SuggestedCodec { get; set; } = System.Array.Empty<string>();
        public IReadOnlyList<string> VoicePersonalities { get; set; } = System.Array.Empty<string>();
    }
}
