using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TlsClient.Core;
using TlsClient.Core.Helpers;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.Core.Models.Responses;
using TlsClient.Native.Wrappers;

namespace TlsClient.Native
{
    public sealed class NativeTlsClient : BaseTlsClient
    {
        public NativeTlsClient(TlsClientOptions options) : base(options) { }
        public NativeTlsClient() : base() { }

        public static void Initialize(string? libraryPath) => TlsClientWrapper.Initialize(libraryPath);

        #region Sync Methods
        public override Response Request(Request request)
        {
            request= PrepareRequest(request);

            Response response;

            try
            {
                var payload = RequestHelpers.Prepare(request);
                var rawResponse = TlsClientWrapper.Request(payload);
                response = RequestHelpers.ConvertObject<Response>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
            }
            catch (Exception err)
            {
                response = new Response()
                {
                    Body = err.Message,
                    Status = 0,
                };
            }

            if (!string.IsNullOrEmpty(response.Id))
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
        public override GetCookiesFromSessionResponse GetCookies(string url)
        {
            var payload = PrepareGetCookies(url);
            var rawResponse = TlsClientWrapper.GetCookiesFromSession(RequestHelpers.Prepare(payload));
            return RequestHelpers.ConvertObject<GetCookiesFromSessionResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override GetCookiesFromSessionResponse AddCookies(string url, List<TlsClientCookie> cookies)
        {
            var payload = PrepareAddCookies(url, cookies);
            var rawResponse = TlsClientWrapper.AddCookiesToSession(RequestHelpers.Prepare(payload));
            return RequestHelpers.ConvertObject<GetCookiesFromSessionResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override DestroyResponse Destroy()
        {
            var payload = PrepareDestroy();
            var rawResponse = TlsClientWrapper.DestroySession(RequestHelpers.Prepare(payload));
            return RequestHelpers.ConvertObject<DestroyResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override DestroyResponse DestroyAll()
        {
            var rawResponse = TlsClientWrapper.DestroyAll();
            return RequestHelpers.ConvertObject<DestroyResponse>(rawResponse) ?? throw new Exception("Response is null, can't convert object from json.");
        }
        #endregion

        #region Async Methods
        public override Task<Response> RequestAsync(Request request, CancellationToken ct = default) => AsyncHelpers.RunAsync(() => Request(request), ct);
        public override Task<GetCookiesFromSessionResponse> GetCookiesAsync(string url, CancellationToken ct = default) => AsyncHelpers.RunAsync(() => GetCookies(url), ct);
        public override Task<DestroyResponse> DestroyAsync(CancellationToken ct = default) => AsyncHelpers.RunAsync(() => Destroy(), ct);
        public override Task<DestroyResponse> DestroyAllAsync(CancellationToken ct = default) => AsyncHelpers.RunAsync(() => DestroyAll(), ct);
        public override Task<GetCookiesFromSessionResponse> AddCookiesAsync(string url, List<TlsClientCookie> cookies, CancellationToken ct = default) => AsyncHelpers.RunAsync(() => AddCookies(url, cookies), ct);
        #endregion
    }
}
