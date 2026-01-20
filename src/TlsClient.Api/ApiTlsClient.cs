using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TlsClient.Api.Models.Entities;
using TlsClient.Core;
using TlsClient.Core.Helpers;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.Core.Models.Responses;

namespace TlsClient.Api
{
    public sealed class ApiTlsClient : BaseTlsClient
    {
        public RestClient RestClient { get; }
        public ApiTlsClient(ApiTlsClientOptions options) : base(options) {
            RestClient = new RestClient(new RestClientOptions()
            {
                BaseUrl= options.ApiBaseUri,
                Timeout= null,
            });
            RestClient.AddDefaultHeader("x-api-key", options.ApiKey);
        }

        public ApiTlsClient(Uri apiBaseUri, string apiKey) : this(new ApiTlsClientOptions(TlsClientIdentifier.Chrome133, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0", apiBaseUri, apiKey)){   }

        #region Sync Methods
        public override Response Request(Request request) => AsyncHelpers.RunSync(() => RequestAsync(request, CancellationToken.None));
        public override GetCookiesFromSessionResponse AddCookies(string url, List<TlsClientCookie> cookies) => throw new NotImplementedException();
        public override DestroyResponse Destroy() => AsyncHelpers.RunSync(() => DestroyAsync(CancellationToken.None));
        public override GetCookiesFromSessionResponse GetCookies(string url) => AsyncHelpers.RunSync(() => GetCookiesAsync(url, CancellationToken.None));
        public override DestroyResponse DestroyAll() => AsyncHelpers.RunSync(()=>DestroyAllAsync(CancellationToken.None));
        #endregion

        #region Async Methods
        public override async Task<Response> RequestAsync(Request request, CancellationToken ct = default)
        {
            request = PrepareRequest(request);


            Response response;

            try
            {
                var restRequest = new RestRequest("/api/forward", Method.Post);
                restRequest.AddJsonBody(request);
                var restResponse = await RestClient.ExecuteAsync<Response>(restRequest);

                response = restResponse.Data ?? throw new Exception("Response data is null, can't convert object from json."); ;
            }
            catch (Exception err)
            {
                response = new Response()
                {
                    Body = err.Message,
                    Status = 0,
                };
            }

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
        public override async Task<GetCookiesFromSessionResponse> AddCookiesAsync(string url, List<TlsClientCookie> cookies, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
        public override async Task<DestroyResponse> DestroyAsync(CancellationToken ct = default)
        {
            var payload = PrepareDestroy();
            var restRequest = new RestRequest("/api/free-session", Method.Post);
            restRequest.AddJsonBody(payload);
            var restResponse = await RestClient.ExecuteAsync<DestroyResponse>(restRequest, ct);
            return restResponse.Data ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override async Task<GetCookiesFromSessionResponse> GetCookiesAsync(string url, CancellationToken ct = default)
        {
            var payload = PrepareGetCookies(url);
            var restRequest = new RestRequest("/api/cookies", Method.Post);
            restRequest.AddJsonBody(payload);
            var restResponse = await RestClient.ExecuteAsync(restRequest, ct);
            if (string.IsNullOrEmpty(restResponse.Content))
            {
                throw new Exception(" Response content is null");
            }

            return RequestHelpers.ConvertObject<GetCookiesFromSessionResponse>(restResponse.Content.Replace("\"", string.Empty).FromBase64().ToStringFromBytes()) ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override async Task<DestroyResponse> DestroyAllAsync(CancellationToken ct = default)
        {
            var restRequest = new RestRequest("/api/free-all", Method.Get);
            var restResponse = await RestClient.ExecuteAsync<DestroyResponse>(restRequest, ct);
            return restResponse.Data ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override async ValueTask DisposeAsync()
        {
            try
            {
                await base.DisposeAsync();
            }
            finally
            {
                RestClient.Dispose();
            }
        }
        #endregion
    }
}
