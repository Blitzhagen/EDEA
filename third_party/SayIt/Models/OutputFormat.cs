namespace SayIt
{
    public record OutputFormat
    {
        public string Value { get; }

        private OutputFormat(string value) => Value = value;

        public static readonly OutputFormat Mp3_24Khz_96Kbps =
            new("audio-24khz-96kbitrate-mono-mp3");

        public static readonly OutputFormat Mp3_24Khz_48Kbps =
            new("audio-24khz-48kbitrate-mono-mp3");

        public static readonly OutputFormat Mp3_48Khz_96Kbps =
            new("audio-48khz-96kbitrate-mono-mp3");

        public static readonly OutputFormat Webm_24Khz_16Bit_Opus =
            new("webm-24khz-16bit-mono-opus");

        public static readonly OutputFormat Pcm_16Khz_16Bit =
            new("raw-16khz-16bit-mono-pcm");

        public static readonly OutputFormat Pcm_24Khz_16Bit =
            new("raw-24khz-16bit-mono-pcm");

        public static readonly OutputFormat Pcm_48Khz_16Bit =
            new("raw-48khz-16bit-mono-pcm");

        public static readonly OutputFormat Wav_16Khz_16Bit =
            new("riff-16khz-16bit-mono-pcm");

        public static readonly OutputFormat Wav_24Khz_16Bit =
            new("riff-24khz-16bit-mono-pcm");

        public static readonly OutputFormat Wav_48Khz_16Bit =
            new("riff-48khz-16bit-mono-pcm");

        public static readonly OutputFormat Default = Mp3_24Khz_96Kbps;

        public override string ToString() => Value;
    }
}
