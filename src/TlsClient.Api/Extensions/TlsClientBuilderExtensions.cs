using System;
using System.Collections.Generic;
using System.Text;
using TlsClient.Api.Models.Entities;
using TlsClient.Core.Builders;

namespace TlsClient.Api.Extensions
{
    public static class TlsClientBuilderExtensions
    {
        public static TlsClientBuilder WithApi(this TlsClientBuilder builder, Uri apiBaseUri, string apiKey)
        {
            builder._options = new ApiTlsClientOptions(apiBaseUri, apiKey)
            {
                CatchPanics = builder._options.CatchPanics,
                CustomTlsClient = builder._options.CustomTlsClient,
                DefaultHeaders = builder._options.DefaultHeaders,
                DisableIPV4 = builder._options.DisableIPV4,
                DisableIPV6 = builder._options.DisableIPV6,
                FollowRedirects = builder._options.FollowRedirects,
                ForceHttp1 = builder._options.ForceHttp1,
                HeaderOrder = builder._options.HeaderOrder,
                InsecureSkipVerify = builder._options.InsecureSkipVerify,
                IsRotatingProxy = builder._options.IsRotatingProxy,
                ProxyURL = builder._options.ProxyURL,
                SessionID = builder._options.SessionID,
                TlsClientIdentifier = builder._options.TlsClientIdentifier,
                Timeout = builder._options.Timeout,
                WithDebug = builder._options.WithDebug,
                CertificatePinningHosts = builder._options.CertificatePinningHosts,
                ConnectHeaders = builder._options.ConnectHeaders,
                WithCustomCookieJar = builder._options.WithCustomCookieJar,
                WithoutCookieJar = builder._options.WithoutCookieJar,
                WithRandomTLSExtensionOrder = builder._options.WithRandomTLSExtensionOrder,
                EuckrResponse = builder._options.EuckrResponse,
                WithProtocolRacing = builder._options.WithProtocolRacing,
                DisableHttp3 = builder._options.DisableHttp3,
            };

            return builder;
        }
        public static ApiTlsClient Build(this TlsClientBuilder builder)
        {
            var options = builder._options as ApiTlsClientOptions;
            if (options == null)
            {
                throw new InvalidOperationException("WithApi must be called before Build");
            }
            return new ApiTlsClient(options);
        }
    }
}
