using System;

namespace TlsClient.Core.Models.Requests
{
    // Reference: https://github.com/bogdanfinn/tls-client/blob/master/cffi_src/types.go (ReadStreamInput)
    public class ReadStreamRequest
    {
        public Guid StreamId { get; set; }

        /// <summary>
        /// How long the native side should block waiting for the next chunk:
        /// <list type="bullet">
        /// <item><description><c>&lt; 0</c>: block until the next chunk, EOF, or error.</description></item>
        /// <item><description><c>= 0</c>: non-blocking poll. Returns Timeout=true immediately when no chunk is buffered.</description></item>
        /// <item><description><c>&gt; 0</c>: block up to TimeoutMs. Returns Timeout=true if no chunk arrived in time.</description></item>
        /// </list>
        /// </summary>
        public int TimeoutMs { get; set; }
    }
}
