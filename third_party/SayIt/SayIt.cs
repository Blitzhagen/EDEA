using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SayIt.Internal;

namespace SayIt
{
    public static class SayIt
    {
        public static Task<Stream> StreamAsync(
            string text,
            SayItConfig? config = null,
            CancellationToken ct = default)
        {
            var speaker = new SayItSpeaker(config ?? SayItConfig.Default);
            return speaker.StreamAsync(text, ct);
        }

        public static Task<Stream> StreamAsync(
            IAsyncEnumerable<string> textStream,
            SayItConfig? config = null,
            CancellationToken ct = default)
        {
            var speaker = new SayItSpeaker(config ?? SayItConfig.Default);
            return speaker.StreamAsync(textStream, ct);
        }

        public static Task SaveAsync(
            string text,
            string path,
            SayItConfig? config = null,
            CancellationToken ct = default)
        {
            var speaker = new SayItSpeaker(config ?? SayItConfig.Default);
            return speaker.SaveAsync(text, path, ct);
        }

        public static Task<IReadOnlyList<VoiceInfo>> ListVoicesAsync(
            CancellationToken ct = default)
        {
            return VoiceList.GetAsync(ct);
        }
    }
}
