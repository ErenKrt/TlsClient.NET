using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using TlsClient.Core;
using TlsClient.Core.Helpers.Builders;
using System.Linq;

using System.Collections.Generic;
using System.Net;
using TlsClient.HttpClient.Helpers;
using System.Net.Http.Headers;
using TlsClient.Core.Helpers.Extensions;

namespace TlsClient.HttpClient
{
    public class TlsClientHandler : HttpClientHandler
    {
        private readonly Core.TlsClient _client;

        public TlsClientHandler(Core.TlsClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            if (_client.Options.WithoutCookieJar)
            {
                this.UseCookies = false;
            }
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

            // Adding cookies with header, native tls client not tested with cookiejar
            var cookies = this.CookieContainer?.GetAllCookies();
            if (cookies?.Any() == true)
            {
                var cookieHeader = string.Join("; ", cookies.Select(cookie => $"{cookie.Name}={cookie.Value}"));
                tlsRequestBuilder.WithHeader("Cookie", cookieHeader);
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
                    httpResponseMessage.Content = new ByteArrayContent(Convert.FromBase64String(parsed.Item2));
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(parsed.Item1);
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

            return httpResponseMessage;
        }
    }
}
