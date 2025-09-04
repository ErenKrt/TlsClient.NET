using System;
using System.Collections.Generic;
using System.Net;
using TlsClient.Core.Helpers;
using TlsClient.Core.Helpers.Wrappers;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.Core.Models.Responses;


namespace TlsClient.Core
{
    public class TlsClient : IDisposable
    {
        private bool _disposed;

        public TlsClientOptions Options { get; set; }
        public Dictionary<string, List<string>> DefaultHeaders => Options.DefaultHeaders;
        public bool IsDisposed => _disposed;

        public TlsClient(TlsClientOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public TlsClient() : this(new TlsClientOptions(TlsClientIdentifier.Chrome132, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0")) { }

        public static void Initialize(string? libraryPath) => TlsClientWrapper.Initialize(libraryPath);

        public Response Request(Request request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.RequestUrl))
                throw new ArgumentException("RequestUrl cannot be null or empty.", nameof(request));

            if ((request.TlsClientIdentifier == null && Options.TlsClientIdentifier == null) && (request.CustomTlsClient == null && Options.CustomTlsClient == null))
            {
                throw new ArgumentException("Either TlsClientIdentifier or CustomTlsClient must be set in Options or in Request.");
            }

            request.TlsClientIdentifier ??= Options.TlsClientIdentifier;
            request.SessionId ??= Options.SessionID;
            request.TimeoutMilliseconds ??= (int)Options.Timeout.TotalMilliseconds;
            request.ProxyUrl ??= Options.ProxyURL;
            request.IsRotatingProxy ??= Options.IsRotatingProxy;
            request.FollowRedirects ??= Options.FollowRedirects;
            request.InsecureSkipVerify ??= Options.InsecureSkipVerify;
            request.DisableIPV4 ??= Options.DisableIPV4;
            request.DisableIPV6 ??= Options.DisableIPV6;
            request.WithDebug ??= Options.WithDebug;
            request.WithDefaultCookieJar ??= Options.WithDefaultCookieJar;
            request.WithoutCookieJar ??= Options.WithoutCookieJar;
            request.CustomTlsClient ??= Options.CustomTlsClient;
            request.CatchPanics ??= Options.CatchPanics;
            request.CertificatePinningHosts ??= Options.CertificatePinningHosts;
            request.ForceHttp1 ??= Options.ForceHttp1;
            request.WithRandomTLSExtensionOrder ??= Options.WithRandomTLSExtensionOrder;
            request.HeaderOrder ??= Options.HeaderOrder;
            request.ConnectHeaders ??= Options.ConnectHeaders;

            if (request.CustomTlsClient != null) // If CustomTlsClient is not null, TlsClientIdentifier should be null
            {
                request.TlsClientIdentifier = default!;
            }

            // DefaultHeaders prop is not working
            request.DefaultHeaders ??= Options.DefaultHeaders;
            request.Headers ??= new Dictionary<string, string>();

            // Default headers prop is not working
            foreach (var header in request.DefaultHeaders)
            {
                if (!request.Headers.ContainsKey(header.Key))
                {
                    request.Headers.Add(header.Key, header.Value[0]);
                }
            }

            // Override if has Host header
            if (request.Headers != null && request.Headers.TryGetValue("Host", out var host) && !string.IsNullOrWhiteSpace(host))
                request.RequestHostOverride ??= host;

            Response response;

            try
            {
                var payload = RequestHelpers.Prepare(request);
                var rawResponse = TlsClientWrapper.Request(payload);
                response = RequestHelpers.ConvertObject<Response>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
            }
            catch(Exception err)
            {
                response = new Response()
                {
                    Body = err.Message,
                    Status = 0,
                };
            }

            if(!string.IsNullOrEmpty(response.Id))
                TlsClientWrapper.FreeMemory(response.Id);

            if (response.Status == 0 && response.Body.Contains("Client.Timeout exceeded"))
            {
                response = new Response()
                {
                    Body = "Timeout",
                    Status = HttpStatusCode.RequestTimeout,
                };
            }

            return response;
        }

        public GetCookiesFromSessionResponse GetCookies(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("url cannot be null or empty.", nameof(url));

            var payload = new GetCookiesFromSessionRequest()
            {
                SessionID = Options.SessionID,
                Url = url,
            };
            var rawResponse = TlsClientWrapper.GetCookiesFromSession(RequestHelpers.Prepare(payload));
            return RequestHelpers.ConvertObject<GetCookiesFromSessionResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }

        public DestroyResponse Destroy()
        {
            var payload = new DestroyRequest()
            {
                SessionID = Options.SessionID,
            };
            
            var rawResponse = TlsClientWrapper.DestroySession(RequestHelpers.Prepare(payload));
            return RequestHelpers.ConvertObject<DestroyResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }

        public static DestroyResponse DestroyAll()
        {
            var rawResponse = TlsClientWrapper.DestroyAll();
            return RequestHelpers.ConvertObject<DestroyResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }

        public GetCookiesFromSessionResponse AddCookies(string url, List<TlsClientCookie> cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var payload = new AddCookiesToSessionRequest
            {
                SessionID = Options.SessionID,
                Url = url,
                Cookies = cookies,
            };
            var rawResponse = TlsClientWrapper.AddCookiesToSession(RequestHelpers.Prepare(payload));
            return RequestHelpers.ConvertObject<GetCookiesFromSessionResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                try { Destroy(); }
                catch { }
            }
            _disposed = true;
        }
    }
}
