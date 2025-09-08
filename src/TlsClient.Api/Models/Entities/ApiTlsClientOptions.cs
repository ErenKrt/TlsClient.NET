using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TlsClient.Core.Models.Entities;

namespace TlsClient.Api.Models.Entities
{
    public class ApiTlsClientOptions : TlsClientOptions
    {
        public Uri ApiBaseUri { get; internal set; }
        public string ApiKey { get; internal set; }

        public ApiTlsClientOptions(Uri apiBaseUri, string apiKey) : base(TlsClientIdentifier.Chrome133, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0")
        {
            ApiBaseUri = ValidateBaseUri(apiBaseUri, nameof(apiBaseUri));
            ApiKey = ValidateApiKey(apiKey, nameof(apiKey));
        }

        public ApiTlsClientOptions(TlsClientIdentifier clientIdentifier, string userAgent, Uri apiBaseUri, string apiKey) : base(clientIdentifier, userAgent)
        {
            ApiBaseUri = ValidateBaseUri(apiBaseUri, nameof(apiBaseUri));
            ApiKey = ValidateApiKey(apiKey, nameof(apiKey));
        }

        public ApiTlsClientOptions(CustomTlsClient customClient, string userAgent, Uri apiBaseUri, string apiKey) : base(customClient, userAgent)
        {
            ApiBaseUri = ValidateBaseUri(apiBaseUri, nameof(apiBaseUri));
            ApiKey = ValidateApiKey(apiKey, nameof(apiKey));
        }

        private static Uri ValidateBaseUri(Uri baseUri, string paramName)
        {
            if (!baseUri.IsAbsoluteUri)
                throw new ArgumentException("ApiBaseUri must be absolute.", paramName);

            if (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("ApiBaseUri must use HTTP or HTTPS.", paramName);

            return baseUri;
        }

        private static string ValidateApiKey(string apiKey, string paramName)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("ApiKey cannot be null or empty.", paramName);
            return apiKey;
        }
    }
}
