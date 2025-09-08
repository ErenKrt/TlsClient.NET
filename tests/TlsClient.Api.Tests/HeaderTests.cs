using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TlsClient.Api.Models.Entities;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;


namespace TlsClient.Api.Tests
{
    public class HeaderTests
    {
        public static readonly string BaseURL = "https://httpbin.io";

        [Fact]
        public void Should_Add_UserAgent_Header()
        {
            var userAgent = "TlsClient.NET 1.0";
            using var tlsClient = new ApiTlsClient(new ApiTlsClientOptions(TlsClientIdentifier.Chrome133, userAgent, new Uri("http://127.0.0.1:8080"), "my-auth-key-1"));
            var request = new Request()
            {
                RequestUrl = BaseURL + "/get"
            };
            var response = tlsClient.Request(request);
            Assert.Contains(userAgent, response.Body);
        }

        [Fact]
        public void Should_Add_UserAgent_Header_Options()
        {
            var userAgent = "TlsClient.NET 1.0";
            var options = new ApiTlsClientOptions(new Uri("http://127.0.0.1:8080"), "my-auth-key-1")
            {
                TlsClientIdentifier = TlsClientIdentifier.Chrome133,
                DefaultHeaders = new Dictionary<string, List<string>>()
                {
                    { "User-Agent", new List<string> { userAgent } }
                }
            };
            using var tlsClient = new ApiTlsClient(options);
            var request = new Request()
            {
                RequestUrl = BaseURL + "/get"
            };
            var response = tlsClient.Request(request);
            Assert.Contains(userAgent, response.Body);
        }

        [Fact]
        public void Should_Add_UserAgent_Header_Default()
        {
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0";

            using var tlsClient = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            var request = new Request()
            {
                RequestUrl = BaseURL + "/get"
            };
            var response = tlsClient.Request(request);
            Assert.Contains(userAgent, response.Body);
        }

        [Fact]
        public void Should_Add_UserAgent_After_Ctor()
        {
            var userAgent = "TlsClient.NET 1.0";

            using var tlsClient = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            tlsClient.DefaultHeaders.Remove("User-Agent");
            tlsClient.DefaultHeaders.Add("User-Agent", new List<string> { userAgent });
            var request = new Request()
            {
                RequestUrl = BaseURL + "/get"
            };
            var response = tlsClient.Request(request);
            Assert.Contains(userAgent, response.Body);
        }

        [Fact]
        public void Should_Override_Host()
        {
            var baseHost = "example.com";
            var realIp = "http://23.220.75.245";

            using var tlsClient = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            var request = new Request()
            {
                RequestUrl = realIp,
                Headers = new Dictionary<string, string>()
                {
                    { "Host", baseHost }
                }
            };
            var response = tlsClient.Request(request);
            Assert.Contains($"Example Domain", response.Body);
        }
    }
}