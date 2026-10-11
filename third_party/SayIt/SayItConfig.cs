namespace SayIt
{
    public sealed class SayItConfig
    {
        public string VoiceName { get; }
        public string Rate { get; }
        public string Pitch { get; }
        public string Volume { get; }
        public OutputFormat Format { get; }
        public int TimeoutSeconds { get; }

        internal static readonly SayItConfig Default = new();

        public SayItConfig()
        {
            VoiceName = "zh-CN-XiaoxiaoNeural";
            Rate = "0%";
            Pitch = "0%";
            Volume = "100%";
            Format = OutputFormat.Default;
            TimeoutSeconds = 30;
        }

        private SayItConfig(
            string voiceName, string rate, string pitch,
            string volume, OutputFormat format, int timeoutSeconds)
        {
            VoiceName = voiceName;
            Rate = rate;
            Pitch = pitch;
            Volume = volume;
            Format = format;
            TimeoutSeconds = timeoutSeconds;
        }

        public SayItConfig WithVoice(string name) =>
            new(name, Rate, Pitch, Volume, Format, TimeoutSeconds);

        public SayItConfig WithVoice(VoiceId voice) =>
            new(voice.Name, Rate, Pitch, Volume, Format, TimeoutSeconds);

        public SayItConfig WithRate(string rate) =>
            new(VoiceName, rate, Pitch, Volume, Format, TimeoutSeconds);

        public SayItConfig WithPitch(string pitch) =>
            new(VoiceName, Rate, pitch, Volume, Format, TimeoutSeconds);

        public SayItConfig WithVolume(string volume) =>
            new(VoiceName, Rate, Pitch, volume, Format, TimeoutSeconds);

        public SayItConfig WithFormat(OutputFormat format) =>
            new(VoiceName, Rate, Pitch, Volume, format, TimeoutSeconds);

        public SayItConfig WithTimeout(int seconds) =>
            new(VoiceName, Rate, Pitch, Volume, Format, seconds);
    }
}
