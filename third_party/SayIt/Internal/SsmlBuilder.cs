using System.Security;
using System.Text;

namespace SayIt.Internal
{
    internal static class SsmlBuilder
    {
        internal static string Build(
            string text,
            string voiceName,
            string rate,
            string pitch,
            string volume)
        {
            var escaped = SecurityElement.Escape(text) ?? text;
            var lang = ExtractLang(voiceName);

            return new StringBuilder()
                .Append("<speak")
                .Append(" version=\"1.0\"")
                .Append(" xmlns=\"http://www.w3.org/2001/10/synthesis\"")
                .Append(" xmlns:mstts=\"https://www.w3.org/2001/mstts\"")
                .Append(" xml:lang=\"").Append(lang).Append('"')
                .Append('>')
                .Append("<voice name=\"").Append(voiceName).Append("\">")
                .Append("<prosody")
                .Append(" rate=\"").Append(rate).Append('"')
                .Append(" pitch=\"").Append(pitch).Append('"')
                .Append(" volume=\"").Append(volume).Append('"')
                .Append('>')
                .Append(escaped)
                .Append("</prosody>")
                .Append("</voice>")
                .Append("</speak>")
                .ToString();
        }

        private static string ExtractLang(string voiceName)
        {
            var parts = voiceName.Split('-');
            return parts.Length >= 2 ? $"{parts[0]}-{parts[1]}" : "en-US";
        }
    }
}
