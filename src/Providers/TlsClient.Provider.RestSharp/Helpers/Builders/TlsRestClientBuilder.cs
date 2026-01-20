using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TlsClient.Core;
using TlsClient.Core.Models.Entities;
using TlsClient.HttpClient;

namespace TlsClient.RestSharp.Helpers.Builders
{
    public class TlsRestClientBuilder
    {
        private BaseTlsClient? _tlsClient;
        private Uri? _baseUrl;
        private CookieContainer? _cookieContainer;

        private Action<RestClientOptions>? _configureRestClient;

        public TlsRestClientBuilder WithCookieContainer(CookieContainer? cookieContainer= null)
        {
            if (_tlsClient == null) throw new ArgumentNullException("TlsClient is required");

            if(cookieContainer == null) cookieContainer = new CookieContainer();
            _cookieContainer = cookieContainer;

            // We disabled cookie jar for tls-client, its will manage restSharp if is enable
            _tlsClient.Options.WithoutCookieJar = true;
            return this;
        }
        public TlsRestClientBuilder WithTlsClient(BaseTlsClient tlsClient)
        {
            _tlsClient = tlsClient ?? throw new ArgumentNullException(nameof(tlsClient));
            return this;
        }

        public TlsRestClientBuilder WithBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be null or empty", nameof(baseUrl));

            _baseUrl = new Uri(baseUrl, UriKind.Absolute);
            return this;
        }

        public TlsRestClientBuilder WithConfigureRestClient(Action<RestClientOptions> configureRestClient)
        {
            _configureRestClient = configureRestClient ?? throw new ArgumentNullException(nameof(configureRestClient));
            return this;
        }

        public RestClient Build()
        {
            if (_tlsClient is null)
                throw new InvalidOperationException("TlsClient must be provided before building.");

            if (_baseUrl is null)
                throw new InvalidOperationException("BaseUrl must be provided before building.");

            var tlsHandler = new TlsClientHandler(_tlsClient, true);

            return new RestClient(
                handler: tlsHandler,
                configureRestClient: options =>
                {
                    options.BaseUrl = _baseUrl;
                    options.UserAgent = _tlsClient.Options.UserAgent;
                    options.FollowRedirects = _tlsClient.Options.FollowRedirects;

                    if (_tlsClient.Options.InsecureSkipVerify)
                        options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    options.CookieContainer = _cookieContainer;
                    _configureRestClient?.Invoke(options);
                });
        }
    }
}
