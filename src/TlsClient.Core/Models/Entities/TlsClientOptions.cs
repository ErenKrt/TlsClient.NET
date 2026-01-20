using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Models.Entities
{
    public class TlsClientOptions
    {
        public CustomTlsClient? CustomTlsClient { get; set; } = null;
        public Dictionary<string, List<string>> DefaultHeaders { get; set; }
        public Guid SessionID { get; set; }
        public TlsClientIdentifier? TlsClientIdentifier { get; set; } = null;
        public string? ProxyURL { get; set; }
        public bool IsRotatingProxy { get; set; } = false;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
        public bool FollowRedirects { get; set; } = false;
        public bool InsecureSkipVerify { get; set; } = false;
        public bool DisableIPV4 { get; set; } = false;
        public bool DisableIPV6 { get; set; } = false;
        public bool DisableHttp3 { get; set; } = false;
        public bool WithProtocolRacing {  get; set; } = false;
        public bool WithDebug { get; set; } = false;
        public bool WithCustomCookieJar { get; set; } = false;
        public bool WithoutCookieJar { get; set; } = false;
        public bool CatchPanics { get; set; } = false; 
        public bool ForceHttp1 { get; set; } = false;
        public bool WithRandomTLSExtensionOrder { get; set; } = false;
        public bool EuckrResponse {  get; set; } = false;
        public List<string>? HeaderOrder { get; set; } = null;
        public Dictionary<string, List<string>>? CertificatePinningHosts { get; set; } = null;
        public Dictionary<string, List<string>>? ConnectHeaders { get; set; } = null;
        public string UserAgent => DefaultHeaders["User-Agent"][0];

        public TlsClientOptions(TlsClientIdentifier clientIdentifier, string userAgent) : this()
        {
            TlsClientIdentifier = clientIdentifier;
            DefaultHeaders["User-Agent"] = new List<string> { userAgent };
        }

        public TlsClientOptions()
        {
            DefaultHeaders = new Dictionary<string, List<string>>();
            SessionID = Guid.NewGuid();
        }

        public TlsClientOptions(CustomTlsClient customClient, string userAgent= "") : this()
        {
            if (!string.IsNullOrEmpty(userAgent))
            {
                DefaultHeaders["User-Agent"] = new List<string> { userAgent };
            }

            CustomTlsClient= customClient;
        }

    }
}
