using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using TlsClient.Core.Converters;
using TlsClient.Core.Models.Entities;

namespace TlsClient.Core.Models.Requests
{
    // Reference: https://github.com/bogdanfinn/tls-client/blob/master/cffi_src/types.go#L51
    public class Request
    {
        [JsonConverter(typeof(JsonStringConverter<TlsClientIdentifier>))]
        public TlsClientIdentifier? TlsClientIdentifier { get; set; } = null;
        public CustomTlsClient? CustomTlsClient { get; set; }
        public TransportOptions? TransportOptions { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, List<string>>? DefaultHeaders { get; set; } = null;
        public Dictionary<string, List<string>>? ConnectHeaders { get; set; } = null;
        public Dictionary<string, List<string>>? CertificatePinningHosts { get; set; } = null;
        public string? LocalAddress { get; set; } = null;
        public string? ServerNameOverwrite { get; set; } = null;
        public string? ProxyUrl { get; set; } = null;
        public string? RequestBody { get; set; } = null;
        public string? RequestHostOverride { get; set; } = null;
        public Guid? SessionId { get; set; }

        /// <summary>
        /// Per-Read buffer size used by the native side when writing to
        /// <see cref="StreamOutputPath"/> or when chunking the body for the
        /// streaming <c>RequestStream</c> / <c>ReadStream</c> API. Defaults to
        /// 1024 bytes for stream-to-file and 4096 bytes for the streaming API.
        /// </summary>
        public int? StreamOutputBlockSize { get; set; } = null;

        /// <summary>
        /// Optional sentinel string appended to <see cref="StreamOutputPath"/>
        /// after the body has been fully written, so consumers tailing the file
        /// can detect completion without polling for size. Ignored when
        /// <see cref="StreamOutputPath"/> is null.
        /// </summary>
        public string? StreamOutputEOFSymbol { get; set; } = null;

        /// <summary>
        /// When set, the response body is streamed to this file path on the
        /// native side instead of being returned in
        /// <see cref="Responses.Response.Body"/>;
        /// <see cref="Responses.Response.Body"/> is empty when this is non-null.
        /// Bytes are written as the body arrives (after any automatic
        /// decompression by the underlying HTTP transport), preserving the
        /// original byte stream — binary content such as images is byte-exact.
        /// </summary>
        /// <remarks>
        /// The file is opened with <c>O_APPEND | O_WRONLY | O_CREATE</c>; if
        /// the target already exists the new bytes are appended to it. The
        /// caller is responsible for truncating or removing a stale file
        /// before the request. This is independent of the streaming
        /// <c>RequestStream</c> / <c>ReadStream</c> API on
        /// <c>NativeTlsClient</c>; both can produce a file, but only the
        /// streaming API hands chunks back to the .NET caller incrementally.
        /// </remarks>
        public string? StreamOutputPath { get; set; } = null;
        [JsonConverter(typeof(JsonStringConverter<HttpMethod>))]
        public HttpMethod RequestMethod { get; set; } = HttpMethod.Get;
        public string RequestUrl { get; set; } = string.Empty;
        public List<string>? HeaderOrder { get; set; } = new List<string>();
        public List<TlsClientCookie>? RequestCookies { get; set; } = null;
        public int? TimeoutMilliseconds { get; set; } = null;
        public int? TimeoutSeconds { get; set; } = null;
        public bool? CatchPanics { get; set; } = null;
        public bool? FollowRedirects { get; set; } = null;
        public bool? ForceHttp1 { get; set; } = null;
        public bool? InsecureSkipVerify { get; set; } = null;
        public bool IsByteRequest { get; set; }
        public bool IsByteResponse { get; set; }
        public bool? IsRotatingProxy { get; set; } = null;
        public bool? DisableIPV6 { get; set; } = null;
        public bool? DisableIPV4 { get; set; } = null;
        public bool? DisableHttp3 { get; set; } = null;
        public bool? WithDebug { get; set; } = null;
        /* if is true creates cookie jar from tls-client-api, can be use withDebug */
        public bool? WithCustomCookieJar { get; set; } = null;
        /* if is true not using cookie jar on tls-client-api */
        public bool? WithoutCookieJar { get; set; } = null;
        public bool? WithRandomTLSExtensionOrder { get; set; } = null;
        public bool? WithProtocolRacing {  get; set; } = null;
        public bool? EuckrResponse { get; set; } = null;
    }
}
