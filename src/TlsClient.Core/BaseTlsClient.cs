using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.Core.Models.Responses;

namespace TlsClient.Core
{
    public abstract class BaseTlsClient : IDisposable, IAsyncDisposable
    {
        public TlsClientOptions Options { get; }
        public Dictionary<string, List<string>> DefaultHeaders => Options.DefaultHeaders;

        protected BaseTlsClient(TlsClientOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected BaseTlsClient(): this(new TlsClientOptions(TlsClientIdentifier.Chrome132, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0")){}

        public abstract Response Request(Request request);
        public abstract GetCookiesFromSessionResponse GetCookies(string url);
        public abstract DestroyResponse Destroy();
        public abstract DestroyResponse DestroyAll();
        public abstract GetCookiesFromSessionResponse AddCookies(string url, List<TlsClientCookie> cookies);

        #region Async Methods
        public abstract Task<Response> RequestAsync(Request request, CancellationToken ct = default);
        public abstract Task<GetCookiesFromSessionResponse> GetCookiesAsync(string url, CancellationToken ct = default);
        public abstract Task<DestroyResponse> DestroyAsync(CancellationToken ct = default);
        public abstract Task<DestroyResponse> DestroyAllAsync(CancellationToken ct = default);
        public abstract Task<GetCookiesFromSessionResponse> AddCookiesAsync(string url, List<TlsClientCookie> cookies, CancellationToken ct = default);
        #endregion

        private bool _disposed;
        public bool IsDisposed => _disposed;
        internal Request PrepareRequest(Request request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.RequestUrl))
                throw new ArgumentException("RequestUrl cannot be null or empty.", nameof(request));

            if ((request.TlsClientIdentifier == null && Options.TlsClientIdentifier == null) &&
                (request.CustomTlsClient == null && Options.CustomTlsClient == null))
                throw new ArgumentException("Either TlsClientIdentifier or CustomTlsClient must be set in Options or in Request.");

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

            if (request.CustomTlsClient != null)
                request.TlsClientIdentifier = default!;

            request.DefaultHeaders ??= Options.DefaultHeaders;
            request.Headers ??= new Dictionary<string, string>();

            foreach (var header in request.DefaultHeaders)
                if (!request.Headers.ContainsKey(header.Key) && header.Value != null && header.Value.Count > 0)
                    request.Headers.Add(header.Key, header.Value[0]);

            if (request.Headers.TryGetValue("Host", out var host) && !string.IsNullOrWhiteSpace(host))
                request.RequestHostOverride ??= host;

            return request;
        }
        internal AddCookiesToSessionRequest PrepareAddCookies(string url, List<TlsClientCookie> cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            if (cookies == null || cookies.Count == 0)
            {
                throw new ArgumentNullException(nameof(cookies));
            }

            return new AddCookiesToSessionRequest()
            {
                SessionID = Options.SessionID,
                Url = url,
                Cookies = cookies,
            };
        }
        internal DestroyRequest PrepareDestroy()
        {
            return new DestroyRequest()
            {
                SessionID = Options.SessionID,
            };
        }

        internal GetCookiesFromSessionRequest PrepareGetCookies(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            return new GetCookiesFromSessionRequest()
            {
                SessionID = Options.SessionID,
                Url = url,
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                try
                {
                    Destroy();
                }catch{}
            }
            _disposed = true;
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            try
            {
                await DestroyAsync().ConfigureAwait(false);
            }
            catch{}
            _disposed = true;
        }
    }
}
