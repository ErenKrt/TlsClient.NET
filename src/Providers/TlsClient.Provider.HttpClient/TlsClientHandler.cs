using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using TlsClient.Core;
using TlsClient.Core.Builders;
using System.Linq;

using System.Collections.Generic;
using System.Net;
using TlsClient.HttpClient.Helpers;
using System.Net.Http.Headers;

namespace TlsClient.HttpClient
{
    public class TlsClientHandler : HttpClientHandler
    {
        private readonly BaseTlsClient _client;
        private bool _isRestClient;

        public TlsClientHandler(BaseTlsClient client, bool isRestClient= false)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _isRestClient = isRestClient;

            // Always true, cookies will manage by httpClientHandler
            _client.Options.WithoutCookieJar = true;
        }
   
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tlsRequestBuilder = new RequestBuilder()
                .WithUrl(request.RequestUri.AbsoluteUri.ToString())
                .WithMethod(request.Method)
                .WithByteRequest()
                .WithByteResponse();

            if (request.Content != null)
            {
                var content = await request.Content.ReadAsByteArrayAsync();
                tlsRequestBuilder.WithBody(content);

                if (request.Content?.Headers?.ContentType is MediaTypeHeaderValue contentType)
                {
                    tlsRequestBuilder.WithHeader("Content-Type", contentType.ToString());
                }
            }

            // Add headers
            foreach (var header in request.GetHeaderDictionary())
            {
                tlsRequestBuilder.WithHeader(header.Key, header.Value);
            }

            // For raw http-client
            if (!_isRestClient && UseCookies)
            {
                var requestCokies = CookieContainer.GetCookies(request.RequestUri);
                foreach (Cookie requestCookie in requestCokies)
                {
                    tlsRequestBuilder.WithCookie(requestCookie.Name, requestCookie.Value);
                }
            }

            var tlsRequest= tlsRequestBuilder.Build();
            var response = await _client.RequestAsync(tlsRequest, cancellationToken) ?? throw new Exception("Response was returned null from Native Tls Client");
            
            if (response.Status == 0 && !response.Body.Contains("Timeout"))
            {
                throw new Exception(response.Body);
            }

            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = response.Status,
                Version = HttpVersionHelper.Map(response.UsedProtocol),
                RequestMessage = request,
            };

            if (!string.IsNullOrWhiteSpace(response.Body))
            {
                var parsed = response.Body.ToParsedBase64();
                if(!string.IsNullOrEmpty(parsed.Item1) && !string.IsNullOrEmpty(parsed.Item2))
                {
                    var contentType= response.Headers?.FirstOrDefault(h => h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)).Value?.FirstOrDefault()?.Split(';')[0] ?? parsed.Item1;

                    httpResponseMessage.Content = new ByteArrayContent(Convert.FromBase64String(parsed.Item2));
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }
            }
            else
            {
                httpResponseMessage.Content = new ByteArrayContent(Array.Empty<byte>());
            }

            // Add headers to response
            var headers = response?.Headers ?? Enumerable.Empty<KeyValuePair<string, List<string>>>();
            foreach (var header in headers)
            {
                httpResponseMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // for raw http-client
            if (!_isRestClient && UseCookies)
            {
                var setCookieHeader = response!.Headers.FirstOrDefault(h => string.Equals(h.Key, "Set-Cookie", StringComparison.OrdinalIgnoreCase));

                if (setCookieHeader.Value != null)
                {
                    foreach (var cookieString in setCookieHeader.Value)
                    {
                        try
                        {
                            CookieContainer.SetCookies(request.RequestUri, cookieString);
                        }
                        catch {

                        }
                    }
                }
            }

            return httpResponseMessage;
        }
    }
}
