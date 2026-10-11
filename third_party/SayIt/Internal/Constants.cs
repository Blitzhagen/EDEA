namespace SayIt.Internal
{
    internal static class Constants
    {
        internal const string TrustedClientToken = "6A5AA1D4EAFF4E9FB37E23D68491D6F4";
        internal const string BaseUrl = "speech.platform.bing.com/consumer/speech/synthesize/readaloud";
        internal const string ChromiumFullVersion = "143.0.3650.75";

        internal static string ChromiumMajorVersion => ChromiumFullVersion.Split('.')[0];

        internal static string WssUrl =>
            $"wss://{BaseUrl}/edge/v1?TrustedClientToken={TrustedClientToken}";

        internal static string VoiceListUrl =>
            $"https://{BaseUrl}/voices/list?trustedclienttoken={TrustedClientToken}";

        internal static string SecMsGecVersion => $"1-{ChromiumFullVersion}";

        internal static string UserAgent =>
            $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
            + $" (KHTML, like Gecko) Chrome/{ChromiumMajorVersion}.0.0.0"
            + $" Safari/537.36 Edg/{ChromiumMajorVersion}.0.0.0";

        internal static readonly (string Name, string Value)[] WssHeaders =
        {
            ("Pragma", "no-cache"),
            ("Cache-Control", "no-cache"),
            ("Origin", "chrome-extension://jdiccldimpdaibmpdkjnbmckianbfold"),
            ("User-Agent", UserAgent),
            ("Accept-Encoding", "gzip, deflate, br, zstd"),
            ("Accept-Language", "en-US,en;q=0.9"),
        };
    }
}
