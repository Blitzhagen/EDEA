using System;
using System.Security.Cryptography;
using System.Text;

namespace SayIt.Internal
{
    internal static class Drm
    {
        private const long WinEpoch = 11644473600;
        private const double SecTo100Ns = 10000000;

        internal static string GenerateSecMsGec()
        {
            var unixTs = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var ticks = (unixTs + WinEpoch);
            ticks -= ticks % 300;
            var ns100 = ticks * SecTo100Ns;
            var input = $"{ns100:F0}{Constants.TrustedClientToken}";

            using var algo = SHA256.Create();
            var hash = algo.ComputeHash(Encoding.ASCII.GetBytes(input));
            return ToHexUpper(hash);
        }

        internal static string GenerateMuid()
        {
            var bytes = new byte[16];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return ToHexUpper(bytes);
        }

        internal static string ToHexUpper(byte[] bytes)
        {
            var chars = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                chars[i * 2] = GetHexCharUpper(b >> 4);
                chars[i * 2 + 1] = GetHexCharUpper(b & 0x0F);
            }
            return new string(chars);
        }

        private static char GetHexCharUpper(int n) =>
            (char)(n < 10 ? '0' + n : 'A' + n - 10);
    }
}
