using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SayIt.Internal
{
    internal sealed class WsConnection : IDisposable
    {
        private readonly ClientWebSocket _ws = new();
        private bool _disposed;

        internal WsConnection()
        {
            foreach (var (name, value) in Constants.WssHeaders)
                _ws.Options.SetRequestHeader(name, value);

            _ws.Options.SetRequestHeader("Cookie", $"muid={Drm.GenerateMuid()};");
        }

        internal async Task ConnectAsync(CancellationToken ct)
        {
            var url = Constants.WssUrl
                + $"&ConnectionId={Guid.NewGuid():N}"
                + $"&Sec-MS-GEC={Drm.GenerateSecMsGec()}"
                + $"&Sec-MS-GEC-Version={Constants.SecMsGecVersion}";

            await _ws.ConnectAsync(new Uri(url), ct);
        }

        internal async Task SendConfigAsync(OutputFormat format, CancellationToken ct)
        {
            var body = JsonSerializer.Serialize(new
            {
                context = new
                {
                    synthesis = new
                    {
                        audio = new
                        {
                            metadataoptions = new
                            {
                                sentenceBoundaryEnabled = false,
                                wordBoundaryEnabled = true
                            },
                            outputFormat = format.Value
                        }
                    }
                }
            });

            var headers = new[]
            {
                ("X-Timestamp", DateToJsString()),
                ("Content-Type", "application/json; charset=utf-8"),
                ("Path", "speech.config"),
            };

            await SendAsync(headers, body, ct);
        }

        internal async Task SendSsmlAsync(string ssml, CancellationToken ct)
        {
            var headers = new[]
            {
                ("X-RequestId", Guid.NewGuid().ToString("N")),
                ("Content-Type", "application/ssml+xml"),
                ("X-Timestamp", DateToJsString() + "Z"),
                ("Path", "ssml"),
            };

            await SendAsync(headers, ssml, ct);
        }

        internal async IAsyncEnumerable<byte[]> ReceiveAudioChunksAsync(
            [EnumeratorCancellation] CancellationToken ct)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(65536);

            try
            {
                while (!ct.IsCancellationRequested &&
                       _ws.State == WebSocketState.Open)
                {
                    var result = await _ws.ReceiveAsync(
                        new ArraySegment<byte>(buffer), ct);

                    if (result.MessageType == WebSocketMessageType.Close)
                        yield break;

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var text = await ReadFullTextAsync(result, buffer, ct);
                        if (text.Contains("Path:turn.end"))
                            yield break;

                        continue;
                    }

                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        using var message = new MemoryStream();
                        message.Write(buffer, 0, result.Count);

                        while (!result.EndOfMessage)
                        {
                            result = await _ws.ReceiveAsync(
                                new ArraySegment<byte>(buffer), ct);
                            message.Write(buffer, 0, result.Count);
                        }

                        var raw = message.ToArray();
                        var (audio, ok) = ExtractAudioFromBinary(raw);
                        if (ok && audio.Length > 0)
                            yield return audio;
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        internal async Task<byte[]> ReceiveAllAudioAsync(CancellationToken ct)
        {
            using var combined = new MemoryStream();
            await foreach (var chunk in ReceiveAudioChunksAsync(ct))
                combined.Write(chunk, 0, chunk.Length);
            return combined.ToArray();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_ws.State == WebSocketState.Open ||
                _ws.State == WebSocketState.CloseSent)
            {
                try
                {
                    _ws.CloseAsync(
                        WebSocketCloseStatus.NormalClosure, "",
                        CancellationToken.None)
                        .GetAwaiter().GetResult();
                }
                catch { }
            }

            _ws.Dispose();
        }

        private async Task SendAsync(
            (string Name, string Value)[] headers, string body, CancellationToken ct)
        {
            using var buffer = new MemoryStream();

            foreach (var (name, value) in headers)
            {
                var line = $"{name}:{value}\r\n";
                var lineBytes = Encoding.UTF8.GetBytes(line);
                buffer.Write(lineBytes, 0, lineBytes.Length);
            }

            buffer.WriteByte((byte)'\r');
            buffer.WriteByte((byte)'\n');

            var bodyBytes = Encoding.UTF8.GetBytes(body);
            buffer.Write(bodyBytes, 0, bodyBytes.Length);

            var message = buffer.ToArray();
            await _ws.SendAsync(
                new ArraySegment<byte>(message),
                WebSocketMessageType.Text, true, ct);
        }

        private async Task<string> ReadFullTextAsync(
            WebSocketReceiveResult first, byte[] buffer, CancellationToken ct)
        {
            if (first.EndOfMessage)
                return Encoding.UTF8.GetString(buffer, 0, first.Count);

            using var builder = new MemoryStream();
            builder.Write(buffer, 0, first.Count);
            var result = first;

            while (!result.EndOfMessage)
            {
                result = await _ws.ReceiveAsync(
                    new ArraySegment<byte>(buffer), ct);
                builder.Write(buffer, 0, result.Count);
            }

            return Encoding.UTF8.GetString(
                builder.GetBuffer(), 0, (int)builder.Length);
        }

        private static (byte[] audio, bool ok) ExtractAudioFromBinary(byte[] data)
        {
            if (data.Length < 2)
                return (Array.Empty<byte>(), false);

            var headerLen = (data[0] << 8) | data[1];

            if (headerLen > data.Length)
                return (Array.Empty<byte>(), false);

            var bodyStart = headerLen + 2;
            if (bodyStart >= data.Length)
                return (Array.Empty<byte>(), false);

            var audioLen = data.Length - bodyStart;
            var audio = new byte[audioLen];
            Buffer.BlockCopy(data, bodyStart, audio, 0, audioLen);
            return (audio, true);
        }

        internal static string DateToJsString()
        {
            return DateTime.UtcNow.ToString(
                "ddd MMM dd yyyy HH:mm:ss 'GMT+0000 (Coordinated Universal Time)'");
        }
    }
}
