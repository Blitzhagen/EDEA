using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SayIt.Internal
{
    internal static class VoiceList
    {
        internal static async Task<IReadOnlyList<VoiceInfo>> GetAsync(CancellationToken ct)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authority",
                "speech.platform.bing.com");
            client.DefaultRequestHeaders.Add("User-Agent",
                Constants.UserAgent);
            client.DefaultRequestHeaders.Add("Accept", "*/*");

            using var response = await client.GetAsync(Constants.VoiceListUrl, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var list = new List<VoiceInfo>(doc.RootElement.GetArrayLength());

            foreach (var item in doc.RootElement.EnumerateArray())
            {
                var voice = new VoiceInfo
                {
                    Name = TryGetString(item, "Name"),
                    ShortName = TryGetString(item, "ShortName"),
                    Gender = TryGetString(item, "Gender"),
                    Locale = TryGetString(item, "Locale"),
                    LocalName = TryGetString(item, "LocalName"),
                };

                if (item.TryGetProperty("SuggestedCodec", out var sc))
                    voice.SuggestedCodec = ParseStringList(sc);

                if (item.TryGetProperty("VoicePersonalities", out var vp))
                    voice.VoicePersonalities = ParseStringList(vp);

                list.Add(voice);
            }

            return list;
        }

        private static string TryGetString(JsonElement el, string key) =>
            el.TryGetProperty(key, out var p) ? p.GetString() ?? "" : "";

        private static IReadOnlyList<string> ParseStringList(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.String)
                return new[] { el.GetString() ?? "" };

            if (el.ValueKind == JsonValueKind.Array)
            {
                var list = new List<string>(el.GetArrayLength());
                foreach (var item in el.EnumerateArray())
                    list.Add(item.GetString() ?? "");
                return list;
            }

            return System.Array.Empty<string>();
        }
    }
}
