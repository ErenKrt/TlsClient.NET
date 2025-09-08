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
        public int? StreamOutputBlockSize { get; set; } = null;
        public string? StreamOutputEOFSymbol { get; set; } = null;
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
        public bool? WithDebug { get; set; } = null;
        public bool? WithDefaultCookieJar { get; set; } = null;
        public bool? WithoutCookieJar { get; set; } = null;
        public bool? WithRandomTLSExtensionOrder { get; set; } = null;
    }
}
