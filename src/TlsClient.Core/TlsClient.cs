using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TlsClient.Core.Helpers;
using TlsClient.Core.Helpers.Natives;
using TlsClient.Core.Helpers.Wrappers;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.Core.Models.Responses;


namespace TlsClient.Core
{
    public class TlsClient : IDisposable
    {
        private IntPtr LoadedLibrary { get; set; } = IntPtr.Zero;
        public TlsClientOptions Options { get; set; }
        public Dictionary<string, List<string>> DefaultHeaders => Options.DefaultHeaders;
        private TlsClientWrapper _wrapper { get; set; }
        public TlsClient(TlsClientOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            LoadedLibrary = NativeLoader.LoadNativeAssembly(options.LibraryPath);
            _wrapper = new TlsClientWrapper(LoadedLibrary);
        }

        public TlsClient() : this(new TlsClientOptions(TlsClientIdentifier.Chrome132, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0")) { }
        public async Task<Response> RequestAsync(Request request, CancellationToken cancellationToken = default)
        {
            if (request.RequestUrl == null)
            {
                throw new ArgumentNullException(nameof(request.RequestUrl));
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
           
            if(request.CustomTlsClient != null) // If CustomTlsClient is not null, TlsClientIdentifier should be null
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
            request.RequestHostOverride ??= request.Headers.ContainsKey("Host") ? request.Headers["Host"] : null;

            Response response;

            try
            {
                var rawResponse = await _wrapper.RequestAsync(RequestHelpers.Prepare(request), cancellationToken);
                response = JsonConvert.DeserializeObject<Response>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
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
                await _wrapper.FreeMemoryAsync(response.Id, cancellationToken);

            return response;
        }

        public async Task<GetCookiesFromSessionResponse> GetCookiesAsync(string url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var payload = new GetCookiesFromSessionRequest()
            {
                SessionID = Options.SessionID,
                Url = url,
            };

            var rawResponse = await _wrapper.GetCookiesFromSessionAsync(RequestHelpers.Prepare(payload), cancellationToken);
            return JsonConvert.DeserializeObject<GetCookiesFromSessionResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }

        public async Task<DestroyResponse> DestroyAsync(CancellationToken cancellationToken = default)
        {
            var payload = new DestroyRequest()
            {
                SessionID = Options.SessionID,
            };

            var rawResponse = await _wrapper.DestroySessionAsync(RequestHelpers.Prepare(payload), cancellationToken);
            return JsonConvert.DeserializeObject<DestroyResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }

        public async Task<DestroyResponse> DestroyAllAsync(CancellationToken cancellationToken = default)
        {
            var rawResponse = await _wrapper.DestroyAllAsync(cancellationToken);
            return JsonConvert.DeserializeObject<DestroyResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }

        public async Task<GetCookiesFromSessionResponse> AddCookiesAsync(string url, List<TlsClientCookie> cookies, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var payload = new AddCookiesToSessionRequest()
            {
                SessionID = Options.SessionID,
                Url = url,
                Cookies = cookies,
            };
            var rawResponse = await _wrapper.AddCookiesToSessionAsync(RequestHelpers.Prepare(payload), cancellationToken);
            return JsonConvert.DeserializeObject<GetCookiesFromSessionResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }
  

        public void Dispose()
        {
            if(LoadedLibrary == IntPtr.Zero) return;

            DestroyAsync().GetAwaiter().GetResult();
            _wrapper.Dispose();

            LoadedLibrary = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }
}
