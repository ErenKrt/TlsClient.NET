using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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
        public HttpClient HttpClient { get; }
        public ApiTlsClient(ApiTlsClientOptions options) : base(options) {
            HttpClient = new HttpClient
            {
                BaseAddress = options.ApiBaseUri,
                Timeout = Timeout.InfiniteTimeSpan
            };

            HttpClient.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
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
                string jsonPayload = request.ToJson();
                using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                using var httpResponse = await HttpClient.PostAsync("/api/forward", content, ct);
                var responseString = await httpResponse.Content.ReadAsStringAsync();

                response = responseString.FromJson<Response>() ?? throw new Exception("Response data is null, can't convert object from json.");
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
            string jsonPayload = payload.ToJson();

            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            using var httpResponse = await HttpClient.PostAsync("/api/free-session", content, ct);

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return responseString.FromJson<DestroyResponse>()
                   ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override async Task<GetCookiesFromSessionResponse> GetCookiesAsync(string url, CancellationToken ct = default)
        {
            var payload = PrepareGetCookies(url);
            string jsonPayload = payload.ToJson();

            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            using var httpResponse = await HttpClient.PostAsync("/api/cookies", content, ct);

            var responseString = await httpResponse.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseString))
            {
                throw new Exception("Response content is null");
            }

            string cleanString = responseString.Replace("\"", string.Empty);
            byte[] decodedBytes = cleanString.FromBase64();
            string decodedJson = decodedBytes.ToStringFromBytes();

            return decodedJson.FromJson<GetCookiesFromSessionResponse>() ?? throw new Exception("Response is null, can't convert object from json.");
        }

        public override async Task<DestroyResponse> DestroyAllAsync(CancellationToken ct = default)
        {
            using var httpResponse = await HttpClient.GetAsync("/api/free-all", ct);
            var responseString = await httpResponse.Content.ReadAsStringAsync();

            return responseString.FromJson<DestroyResponse>() ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override async ValueTask DisposeAsync()
        {
            try
            {
                await base.DisposeAsync();
            }
            finally
            {
                HttpClient.Dispose();
            }
        }
        #endregion
    }
}
