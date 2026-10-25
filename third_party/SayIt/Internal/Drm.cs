using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SayIt.Internal
{
    /// <summary>
    /// Sec-MS-GEC token generation. Ported from edge-tts drm.py (v2 scheme):
    /// the token combines a custom CS64 digest and a SHA-256 variant with a
    /// pre-baked initial state, formatted as "&lt;16 hex&gt;-&lt;64 hex&gt;".
    /// All arithmetic is 32-bit unsigned (natural uint wraparound).
    /// </summary>
    internal static class Drm
    {
        private const long WinEpoch = 11644473600;

        // FIPS 180-4 initial hash state.
        private static readonly uint[] Sha256Initial =
        {
            0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A,
            0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19,
        };

        // Non-standard working state used to precompute the first block.
        private static readonly uint[] Sha256State =
        {
            0xFD9FBEA2, 0x91243CE7, 0xBE499094, 0xCD2E1E0A,
            0x6C006F49, 0x04BD35AB, 0xFEB0A7E4, 0x03EFFBD2,
        };

        // Extra schedule values used when building the synthetic first block.
        private static readonly uint[] Sha256Schedule =
        {
            0xEB24BF9E, 0x1558691F, 0xA0C09EE2, 0x0CFA32BA,
            0xA622EE8C, 0xD1DD9491, 0xC0DD25A0, 0xF9E70AB6,
            0xF0C31573, 0x7058CE4E, 0x7188E606, 0x00E5AB33,
            0xD258B060, 0x72766564,
        };

        // Standard SHA-256 round constants.
        private static readonly uint[] Sha256RoundConstants =
        {
            0x428A2F98, 0x71374491, 0xB5C0FBCF, 0xE9B5DBA5,
            0x3956C25B, 0x59F111F1, 0x923F82A4, 0xAB1C5ED5,
            0xD807AA98, 0x12835B01, 0x243185BE, 0x550C7DC3,
            0x72BE5D74, 0x80DEB1FE, 0x9BDC06A7, 0xC19BF174,
            0xE49B69C1, 0xEFBE4786, 0x0FC19DC6, 0x240CA1CC,
            0x2DE92C6F, 0x4A7484AA, 0x5CB0A9DC, 0x76F988DA,
            0x983E5152, 0xA831C66D, 0xB00327C8, 0xBF597FC7,
            0xC6E00BF3, 0xD5A79147, 0x06CA6351, 0x14292967,
            0x27B70A85, 0x2E1B2138, 0x4D2C6DFC, 0x53380D13,
            0x650A7354, 0x766A0ABB, 0x81C2C92E, 0x92722C85,
            0xA2BFE8A1, 0xA81A664B, 0xC24B8B70, 0xC76C51A3,
            0xD192E819, 0xD6990624, 0xF40E3585, 0x106AA070,
            0x19A4C116, 0x1E376C08, 0x2748774C, 0x34B0BCB5,
            0x391C0CB3, 0x4ED8AA4A, 0x5B9CCA4F, 0x682E6FF3,
            0x748F82EE, 0x78A5636F, 0x84C87814, 0x8CC70208,
            0x90BEFFFA, 0xA4506CEB, 0xBEF9A3F7, 0xC67178F2,
        };

        private const uint Sha256Adjustment = 0xC9DC06A7;
        private const uint Sigma0Adjustment = 0x059C0B80;
        private const uint Sigma1Adjustment = 0x000B92C0;

        // Round constants where index 14 is replaced by the adjustment value;
        // used when the synthetic first block is compressed starting at round 14.
        private static readonly uint[] Sha256BlockConstants = BuildBlockConstants();

        private static readonly (uint Coefficient, uint Offset)[] Cs64Coefficients =
        {
            (0xE82E91B1, 0xB32700D0), (0x44DE24C6, 0x88DFF51D),
            (0x3BEB65BC, 0x29514038), (0xC89C5933, 0xB328CBDF),
            (0x8D1EC211, 0xD405B4D5), (0x314EF5D5, 0x26E55F3C),
            (0xE92491B1, 0x607801C6), (0xF0A5B9D5, 0x82B2C1FB),
            (0x06064CF3, 0xFA51A3DA), (0xDA79FA60, 0x207CC464),
            (0xBED20560, 0x3C3C74F3), (0x89F9C9F8, 0xD2D14B61),
            (0x35FA38C6, 0xBB04A2C1), (0xCB49E3C6, 0xC5626167),
        };

        private const uint Cs64Factor = 0x82893933;
        private const uint Cs64Adjustment0 = 0xF2F90B1A;
        private const uint Cs64Adjustment1 = 0x0B1A0000;

        /// <summary>
        /// Generates the Sec-MS-GEC token for the current time (v2 digest format).
        /// </summary>
        internal static string GenerateSecMsGec() => GenerateSecMsGecV2(null);

        /// <summary>
        /// Generates a v2 Sec-MS-GEC token: ticks rounded to 5 minutes in Windows
        /// file-time format plus the trusted client token, hashed into
        /// "&lt;cs64&gt;-&lt;sha256&gt;" form.
        /// </summary>
        internal static string GenerateSecMsGecV2(double? unixTimestamp)
        {
            // The reference implementation computes in float64; replicate that
            // (values exceed 2^53, but the server-side reference rounds the same).
            double ticks = unixTimestamp ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
            ticks += WinEpoch;
            ticks -= ticks % 300;
            ticks *= 1e7;
            byte[] input = Encoding.ASCII.GetBytes($"{ticks:F0}{Constants.TrustedClientToken}");
            byte[] digest = Sha256Digest(input);
            (uint cs64a, uint cs64b) = Cs64Digest(input, digest);
            var cs64Bytes = new byte[8];
            BitConverter.TryWriteBytes(cs64Bytes.AsSpan(0, 4), cs64a);
            BitConverter.TryWriteBytes(cs64Bytes.AsSpan(4, 4), cs64b);
            return $"{ToHexUpper(cs64Bytes)}-{ToHexUpper(digest)}";
        }

        /// <summary>Rotates a 32-bit value left by 16 bits.</summary>
        private static uint Rol16(uint value) => (value << 16) | (value >> 16);

        private static uint RotR(uint value, int bits) => (value >> bits) | (value << (32 - bits));

        /// <summary>Splits data into little-endian 32-bit words, zero-padded to an even count.</summary>
        private static List<uint> WordsLe4Even(byte[] data)
        {
            int padded = data.Length + ((-data.Length) & 3);
            var words = new List<uint>(padded / 4);
            for (int i = 0; i < padded; i += 4)
            {
                uint w = 0;
                for (int j = 0; j < 4 && i + j < data.Length; j++)
                {
                    w |= (uint)data[i + j] << (8 * j);
                }
                words.Add(w);
            }
            if (words.Count % 2 != 0)
            {
                words.Add(0);
            }
            return words;
        }

        private static uint Sigma0(uint value) =>
            (value >> 7) ^ (value << 25) ^ (value >> 18) ^ (value << 14) ^ (value >> 3);

        private static uint Sigma1(uint value) =>
            (value >> 17) ^ (value << 15) ^ (value >> 19) ^ (value << 13) ^ (value >> 10);

        private static uint[] BuildBlockConstants()
        {
            var constants = (uint[])Sha256RoundConstants.Clone();
            constants[14] = Sha256Adjustment;
            return constants;
        }

        /// <summary>Runs SHA-256 rounds from firstRound..63.</summary>
        private static uint[] Sha256Rounds(uint[] state, uint[] words, int firstRound = 0)
        {
            uint a = state[0], b = state[1], c = state[2], d = state[3],
                 e = state[4], f = state[5], g = state[6], h = state[7];
            var constants = firstRound == 14 ? Sha256BlockConstants : Sha256RoundConstants;
            for (int i = firstRound; i < 64; i++)
            {
                uint t1 = h
                    + (RotR(e, 6) ^ RotR(e, 11) ^ RotR(e, 25))
                    + (g ^ (e & (f ^ g)))
                    + constants[i]
                    + words[i];
                uint t2 = (RotR(a, 2) ^ RotR(a, 13) ^ RotR(a, 22))
                    + ((a & b) | (c & (a | b)));
                h = g; g = f; f = e; e = d + t1;
                d = c; c = b; b = a; a = t1 + t2;
            }
            return new[] { a, b, c, d, e, f, g, h };
        }

        /// <summary>Expands the message schedule from firstWord to 64 words.</summary>
        private static void ExpandSha256Schedule(uint[] words, int firstWord = 16)
        {
            for (int i = firstWord; i < 64; i++)
            {
                uint x = words[i - 2], y = words[i - 15];
                words[i] = Sigma1(x) + words[i - 7] + Sigma0(y) + words[i - 16];
            }
        }

        /// <summary>
        /// Precomputes the compression state of a synthetic first block whose last
        /// eight bytes carry the first seven input bytes (data[0..7]).
        /// </summary>
        private static uint[] Sha256Init(byte[] data)
        {
            var words = new uint[64];
            words[14] = ((uint)data[0] << 16) | ((uint)data[1] << 8) | data[2];
            words[15] = ((uint)data[3] << 24) | ((uint)data[4] << 16)
                | ((uint)data[5] << 8) | data[6];
            for (int i = 16; i < 30; i++)
            {
                uint value = Sha256Schedule[i - 16];
                if (i - 2 >= 14)
                {
                    value += i == 16
                        ? Sigma1(words[i - 2]) ^ Sigma1Adjustment
                        : Sigma1(words[i - 2]);
                }
                if (i - 7 >= 14)
                {
                    value += words[i - 7];
                }
                if (i - 15 >= 14)
                {
                    value += i == 29
                        ? Sigma0(words[i - 15]) ^ Sigma0Adjustment
                        : Sigma0(words[i - 15]);
                }
                words[i] = value;
            }
            words[30] = Sigma1(words[28]) + words[23] + Sigma0(words[15])
                + words[14] + Sha256Adjustment - Sha256RoundConstants[14];
            ExpandSha256Schedule(words, 31);
            uint[] working = Sha256Rounds(Sha256State, words, 14);
            var state = new uint[8];
            for (int i = 0; i < 8; i++)
            {
                state[i] = Sha256Initial[i] + working[i];
            }
            return state;
        }

        /// <summary>
        /// Computes the SHA-256 digest variant used for the token: a synthetic
        /// first block seeded with data[0..7], followed by data[7..] with the
        /// standard padding where the declared length includes the prefix.
        /// </summary>
        private static byte[] Sha256Digest(byte[] data)
        {
            if (data.Length < 7)
            {
                throw new ArgumentException("Token salt must contain at least seven bytes", nameof(data));
            }
            int length = 57 + data.Length;
            int padZeros = ((55 - length) % 64 + 64) % 64;
            var padded = new byte[data.Length - 7 + 1 + padZeros + 8];
            Buffer.BlockCopy(data, 7, padded, 0, data.Length - 7);
            padded[data.Length - 7] = 0x80;
            ulong bitLength = (ulong)length * 8;
            for (int i = 0; i < 8; i++)
            {
                padded[padded.Length - 1 - i] = (byte)(bitLength >> (8 * i));
            }

            uint[] state = Sha256Init(data);
            var block = new uint[64];
            for (int offset = 0; offset < padded.Length; offset += 64)
            {
                for (int i = 0; i < 16; i++)
                {
                    int p = offset + i * 4;
                    block[i] = ((uint)padded[p] << 24) | ((uint)padded[p + 1] << 16)
                        | ((uint)padded[p + 2] << 8) | padded[p + 3];
                }
                ExpandSha256Schedule(block);
                uint[] working = Sha256Rounds(state, block);
                for (int i = 0; i < 8; i++)
                {
                    state[i] += working[i];
                }
            }

            var digest = new byte[32];
            for (int i = 0; i < 8; i++)
            {
                digest[i * 4] = (byte)(state[i] >> 24);
                digest[i * 4 + 1] = (byte)(state[i] >> 16);
                digest[i * 4 + 2] = (byte)(state[i] >> 8);
                digest[i * 4 + 3] = (byte)state[i];
            }
            return digest;
        }

        /// <summary>Computes the CS64 digest words for the token data.</summary>
        private static (uint, uint) Cs64Digest(byte[] data, byte[] digest)
        {
            uint keyA = BitConverter.ToUInt32(digest, 0) | 1;
            uint keyB = BitConverter.ToUInt32(digest, 4) | 1;
            uint factorA = keyA * Cs64Factor;
            uint factorB = keyB * Cs64Factor;

            var terms = new List<uint>();
            var offsets = new List<uint>();
            for (int i = 0; i < Cs64Coefficients.Length; i++)
            {
                var (coefficient, offset) = Cs64Coefficients[i];
                terms.Add(coefficient * (i % 2 == 0 ? factorA : factorB));
                offsets.Add(offset);
            }

            var prefixed = new byte[data.Length + 1]; // leading 0x00 byte
            Buffer.BlockCopy(data, 0, prefixed, 1, data.Length);
            var words = WordsLe4Even(prefixed);
            for (int i = 0; i < words.Count; i++)
            {
                uint word = words[i];
                uint term = word * (i % 2 == 0 ? keyA : keyB);
                uint offset = Rol16(word) * (i % 2 == 0 ? 0xEF0569FBu : 0xC31713DBu);
                if (i == 0)
                {
                    term += Cs64Adjustment0 * factorA;
                    offset += Cs64Adjustment1;
                }
                terms.Add(term);
                offsets.Add(offset);
            }

            uint swapAcc = 0, swapLast = 0, state = 0, acc = 0;
            for (int i = 0; i < terms.Count; i += 2)
            {
                uint x = terms[i], y = terms[i + 1];
                uint t = Rol16(offsets[i] + x);
                uint q = t * 0x76F50F34;
                swapAcc += Rol16(q) * 0xC3EFEA97 + q;
                t = Rol16(offsets[i + 1] + y);
                q = t * 0x1881CE3C;
                swapLast = Rol16(q) * 0x35BD1EC9 + q;
                swapAcc += swapLast;

                state = state * keyA + x;
                state = Rol16(state) * 0xCF98B111;
                state = Rol16(state) * 0x87085B9F;
                state = Rol16(state) * 0x12CEB96D;
                state = Rol16(state) * 0x257E1D83;
                acc += state;
                t = state * keyB + y;
                t = Rol16(t) * 0xA27416F5;
                t = Rol16(t) * 0xD38396FF;
                state = Rol16(Rol16(t) * 0x7C932B89) * 0xBFA49F69;
                acc += state;
            }
            return (swapLast ^ state, swapAcc ^ acc);
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
