using FluentAssertions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TlsClient.Core.Models.Entities;
using TlsClient.Native;
using TlsClient.RestSharp.Helpers.Builders;

namespace TlsClient.RestSharp.Tests
{
    public class StatusTests
    {
        static StatusTests()
        {
            NativeTlsClient.Initialize("D:\\Tools\\TlsClient\\tls-client-windows-64-1.10.0.dll");
        }

        [Fact]
        public void Should_Timeout()
        {
            using var client = new NativeTlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0")
            {
                Timeout = TimeSpan.FromSeconds(5)
            });
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();

            var request = new RestRequest("https://httpbin.org/delay/10");
            var response = restClient.Execute(request);
            Assert.Equal(HttpStatusCode.RequestTimeout, response.StatusCode);
        }

        [Fact]
        public void Should_Error()
        {
            using var client = new NativeTlsClient();
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin1.io")
                .Build();

            var request = new RestRequest("/get");
            var response = restClient.Execute(request);
            response.StatusCode.Should().Be(0);
        }
    }
}
