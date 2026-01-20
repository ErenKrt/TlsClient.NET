using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.Native;

namespace TlsClient.Core.Tests
{
    public class TlsTests
    {
        static TlsTests()
        {
            NativeTlsClient.Initialize("D:\\Tools\\tls-client-windows-64-1.13.1.dll");
        }

        [Fact]
        public void Should_Fail_Ssl()
        {
            using var tlsClient = new NativeTlsClient();
            var request = new Request()
            {
                RequestUrl = "https://self-signed.badssl.com/",
            };
            var response = tlsClient.Request(request);
            response.Status.Should().Be(0);
            response.Body.Should().Contain("certificate signed by unknown authority");
        }

        [Fact]
        public void Should_Skip_Verify_Ssl()
        {
            using var tlsClient = new NativeTlsClient(new TlsClientOptions()
            {
                InsecureSkipVerify = true,
                TlsClientIdentifier = TlsClientIdentifier.Chrome132,
            });
            var request = new Request()
            {
                RequestUrl = "https://self-signed.badssl.com/",
            };
            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_Force_Http1()
        {
            using var tlsClient = new NativeTlsClient(new TlsClientOptions()
            {
                ForceHttp1 = true,
                TlsClientIdentifier = TlsClientIdentifier.Chrome132,
            });
            var request = new Request()
            {
                RequestUrl = "https://tls.peet.ws/api/all",
            };
            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("\"http_version\": \"HTTP/1.1\"");
        }

        [Fact]
        public void Should_Not_Force_Http1()
        {
            using var tlsClient = new NativeTlsClient(new TlsClientOptions()
            {
                TlsClientIdentifier = TlsClientIdentifier.Chrome132,
            });
            var request = new Request()
            {
                RequestUrl = "https://tls.peet.ws/api/all",
            };
            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("\"http_version\": \"h2\"");
        }

        [Fact]
        public void Should_Be_Http3()
        {
            using var tlsClient = new NativeTlsClient(new TlsClientOptions()
            {
                TlsClientIdentifier = TlsClientIdentifier.Chrome133,
            });
            var request = new Request()
            {
                RequestUrl = "https://http3.is/",
            };
            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("support HTTP/3!");
        }

        [Fact]
        public void Should_Random_Extension_Order()
        {
            using var tlsClient = new NativeTlsClient(new TlsClientOptions()
            {
                TlsClientIdentifier = TlsClientIdentifier.Chrome107,
                WithRandomTLSExtensionOrder = true,
            });
            var request = new Request()
            {
                RequestUrl = "https://tls.peet.ws/api/all",
            };
            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);

            var json = System.Text.Json.JsonDocument.Parse(response.Body);
            var ja3String = json.RootElement.GetProperty("tls").GetProperty("ja3").GetString();
            ja3String.Should().NotBeNull();

            // Extract extensions part
            var ja3StringParts = ja3String.Split(',');
            ja3StringParts.Length.Should().BeGreaterThan(2);
            var returnedExtensions = ja3StringParts[2];

            var extensions = "5-0-35-16-18-10-23-65281-43-51-27-17513-45-13-11-21".Split('-');

            foreach (var extension in extensions)
            {
                returnedExtensions.Should().Contain(extension, $"extension {extension} is not part of {returnedExtensions}");
            }

            var returnedExtensionParts = returnedExtensions.Split('-');
            returnedExtensionParts[^1].Should().Be("21");
        }

        /*
         * Certificate pinning test broke.
        [Fact]
        public void Should_Certificate_Pinning()
        {
            // Sertifika pinleri
            var pins = new Dictionary<string, List<string>>
            {
                ["example.com"] = new List<string>
                {
                    "iMMpIJdSf5VlClHaxZReyhaLxLsmZMMNAiA2pMR8/M4=",
                    "qBRjZmOmkSNJL0p70zek7odSIzqs/muR4Jk9xYyCP+E="
                }
            };

            var options = new TlsClientOptions
            {
                Timeout = TimeSpan.FromSeconds(60),
                TlsClientIdentifier = TlsClientIdentifier.Chrome107,
                WithRandomTLSExtensionOrder = true,
                CertificatePinningHosts = pins,
                ProxyURL = "http://127.0.0.1:8080",
                ForceHttp1 = true,
                HeaderOrder = new List<string>
                {
                    "accept",
                    "accept-encoding",
                    "accept-language",
                    "sec-ch-ua",
                    "sec-ch-ua-mobile",
                    "sec-ch-ua-platform",
                    "sec-fetch-dest",
                    "user-agent"
                }
            };

            using var tlsClient = new NativeTlsClient(options);

            var request = new Request
            {
                RequestUrl = "https://example.com",
                Headers = new Dictionary<string, string>
                {
                    ["accept-encoding"] = "gzip, deflate, br",
                    ["accept-language"] = "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7",
                    ["sec-ch-ua"] = "\"Google Chrome\";v=\"107\", \"Chromium\";v=\"107\", \"Not=A?Brand\";v=\"24\"",
                    ["sec-ch-ua-mobile"] = "?0",
                    ["sec-ch-ua-platform"] = "\"macOS\"",
                    ["sec-fetch-dest"] = "empty",
                    ["user-agent"] = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36"
                }
            };

            var response = tlsClient.Request(request);

            response.Status.Should().Be(HttpStatusCode.OK, $"certificate pinning should succeed for example.com with provided pins. Response: {response.Body}");
        }
        */

    }
}
