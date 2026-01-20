using System;
using System.Collections.Generic;
using System.Text;
using TlsClient.Core.Models.Entities;

namespace TlsClient.Core.Builders
{
    public class TlsClientBuilder
    {
        internal TlsClientOptions _options= new TlsClientOptions();

        public TlsClientBuilder WithIdentifier(TlsClientIdentifier clientIdentifier)
        {
            _options.TlsClientIdentifier = clientIdentifier;
            return this;
        }

        public TlsClientBuilder WithUserAgent(string userAgent)
        {
            if(string.IsNullOrWhiteSpace(userAgent))
                throw new ArgumentException("User-Agent cannot be null or empty", nameof(userAgent));

            if (_options.DefaultHeaders.ContainsKey("User-Agent"))
                _options.DefaultHeaders["User-Agent"].Add(userAgent);
            else
                _options.DefaultHeaders["User-Agent"] = new List<string> { userAgent };
            return this;
        }

        public TlsClientBuilder WithProxyUrl(string proxyUrl, bool isRotating = false)
        {
            if(string.IsNullOrWhiteSpace(proxyUrl))
                throw new ArgumentException("Proxy URL cannot be null or empty", nameof(proxyUrl));

            _options.ProxyURL = proxyUrl;
            _options.IsRotatingProxy = isRotating;
            return this;
        }

        public TlsClientBuilder WithTimeout(TimeSpan timeout)
        {
            _options.Timeout = timeout;
            return this;
        }

        public TlsClientBuilder WithDebug(bool enabled = true)
        {
            _options.WithDebug = enabled;
            return this;
        }

        public TlsClientBuilder WithFollowRedirects(bool enabled = true)
        {
            _options.FollowRedirects = enabled;
            return this;
        }

        public TlsClientBuilder WithInsecureSkipVerify(bool skip = true)
        {
            _options.InsecureSkipVerify = skip;
            return this;
        }

        public TlsClientBuilder WithDisableIPV4(bool disabled = true)
        {
            _options.DisableIPV4 = disabled;
            return this;
        }

        public TlsClientBuilder WithDisableIPV6(bool disabled = true)
        {
            _options.DisableIPV6 = disabled;
            return this;
        }

        public TlsClientBuilder WithCustomCookieJar(bool enabled = true)
        {
            _options.WithCustomCookieJar = enabled;
            _options.WithoutCookieJar = !enabled;
            return this;
        }

        public TlsClientBuilder WithoutCookieJar(bool enabled = true)
        {
            _options.WithoutCookieJar = enabled;
            _options.WithCustomCookieJar = !enabled;
            return this;
        }

        public TlsClientBuilder WithHeader(string key, string value)
        {
            if(string.IsNullOrWhiteSpace(key) || string.IsNullOrEmpty(value))
                throw new ArgumentException("Key and value must be non-empty strings.", nameof(key));

            if (_options.DefaultHeaders.ContainsKey(key))
                _options.DefaultHeaders[key].Add(value);
            else
                _options.DefaultHeaders[key] = new List<string> { value };

            return this;
        }

        public TlsClientBuilder WithHeaders(Dictionary<string, List<string>> headers)
        {
            if (headers is null || headers.Count == 0)
                throw new ArgumentException("Header list cannot be null or empty", nameof(headers));

            foreach (var header in headers)
            {
                if (_options.DefaultHeaders.ContainsKey(header.Key))
                    _options.DefaultHeaders[header.Key].AddRange(header.Value);
                else
                    _options.DefaultHeaders[header.Key] = new List<string>(header.Value);
            }
            return this;
        }

        public TlsClientBuilder WithCustomTlsClient(CustomTlsClient customTlsClient)
        {
            if (customTlsClient is null)
                throw new ArgumentNullException(nameof(customTlsClient));

            _options.CustomTlsClient = customTlsClient;
            return this;
        }

        public TlsClientBuilder WithHeaderOrder(List<string> headerOrder)
        {
            if (headerOrder is null || headerOrder.Count == 0)
                throw new ArgumentException("Header order list cannot be null or empty", nameof(headerOrder));

            _options.HeaderOrder = headerOrder;
            return this;
        }

        public TlsClientBuilder WithCatchPanics(bool enabled = true)
        {
            _options.CatchPanics = enabled;
            return this;
        }

        public TlsClientBuilder WithForceHttp1(bool enabled = true)
        {
            _options.ForceHttp1 = enabled;
            return this;
        }

        public TlsClientBuilder WithRandomTLSExtensionOrder(bool enabled = true)
        {
            _options.WithRandomTLSExtensionOrder = enabled;
            return this;
        }

        public TlsClientBuilder WithConnectHeader(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrEmpty(value))
                throw new ArgumentException("Key and value must be non-empty strings.", nameof(key));

            _options.ConnectHeaders ??= new Dictionary<string, List<string>>();

            if (_options.ConnectHeaders.TryGetValue(key, out var values))
            {
                values.Add(value);
            }
            else
            {
                _options.ConnectHeaders[key] = new List<string> { value };
            }

            return this;
        }


        public TlsClientBuilder WithConnectHeaders(Dictionary<string, List<string>> headers)
        {
            if (headers is null)
                throw new ArgumentNullException(nameof(headers));

            _options.ConnectHeaders ??= new Dictionary<string, List<string>>();

            foreach (var (key, values) in headers)
            {
                if (values is null || values.Count == 0)
                    continue;

                if (_options.ConnectHeaders.TryGetValue(key, out var existingValues))
                {
                    existingValues.AddRange(values);
                }
                else
                {
                    _options.ConnectHeaders[key] = new List<string>(values);
                }
            }

            return this;
        }


        public TlsClientBuilder WithCertificatePinning(string host, List<string> sha256Fingerprints)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Host cannot be null or empty", nameof(host));

            if (sha256Fingerprints is null || sha256Fingerprints.Count == 0)
                throw new ArgumentException("At least one fingerprint must be provided", nameof(sha256Fingerprints));

            _options.CertificatePinningHosts ??= new Dictionary<string, List<string>>();

            if (_options.CertificatePinningHosts.TryGetValue(host, out var existingPins))
            {
                existingPins.AddRange(sha256Fingerprints);
            }
            else
            {
                _options.CertificatePinningHosts[host] = new List<string>(sha256Fingerprints);
            }

            return this;
        }
    }
}
