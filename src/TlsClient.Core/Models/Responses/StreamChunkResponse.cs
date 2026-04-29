using System;
using System.Text.Json.Serialization;

namespace TlsClient.Core.Models.Responses
{
    // Reference: https://github.com/bogdanfinn/tls-client/blob/master/cffi_src/types.go (StreamChunkResponse)
    /// <summary>
    /// Returned by <c>readStream</c>. Exactly one of <see cref="EOF"/>, <see cref="Timeout"/>,
    /// <see cref="Error"/>, or a non-empty <see cref="Chunk"/> indicates the meaningful state:
    /// <list type="bullet">
    /// <item><description><see cref="Chunk"/> (base64) holds the next slice of decompressed body bytes.</description></item>
    /// <item><description><see cref="EOF"/> = <c>true</c> means the stream completed naturally; the
    /// <see cref="StreamId"/> is invalid after this call.</description></item>
    /// <item><description><see cref="Timeout"/> = <c>true</c> means no data was available within the
    /// caller-provided timeout; the stream is still live and the caller may retry.</description></item>
    /// <item><description><see cref="Error"/> holds a non-empty message when the underlying read failed;
    /// the <see cref="StreamId"/> is invalid after this call.</description></item>
    /// </list>
    /// </summary>
    public class StreamChunkResponse : BaseResponse
    {
        public Guid StreamId { get; set; }

        /// <summary>Base64-encoded raw bytes. Empty when <see cref="EOF"/>, <see cref="Timeout"/>, or
        /// <see cref="Error"/> is set. Use <see cref="GetChunkBytes"/> for the decoded payload.</summary>
        public string? Chunk { get; set; }

        [JsonPropertyName("eof")]
        public bool EOF { get; set; }

        public bool Timeout { get; set; }

        public string? Error { get; set; }

        /// <summary>True when the chunk carries no payload (EOF/Timeout/Error or empty data).</summary>
        [JsonIgnore]
        public bool IsTerminal => EOF || !string.IsNullOrEmpty(Error);

        /// <summary>Decode the base64 <see cref="Chunk"/> into raw bytes. Returns an empty array if
        /// the chunk is missing.</summary>
        public byte[] GetChunkBytes()
        {
            if (string.IsNullOrEmpty(Chunk))
                return Array.Empty<byte>();
            return Convert.FromBase64String(Chunk);
        }
    }
}
