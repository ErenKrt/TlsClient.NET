using FluentAssertions;
using RestSharp;
using System.Net;
using TlsClient.Api.Extensions;
using TlsClient.Core.Builders;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.RestSharp.Helpers.Builders;

namespace TlsClient.Api.RestSharp.Tests
{
    public class BuilderTests
    {

        [Fact]
        public void Should_Build_Client()
        {
            using var tlsClient = new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome133)
                .WithTimeout(TimeSpan.FromSeconds(10))
                .WithCustomCookieJar()
                .WithInsecureSkipVerify()
                .WithHeader("show", "must go on")
                .WithDebug()
                .WithCatchPanics()
                .WithRandomTLSExtensionOrder()
                .WithUserAgent("TlsClient.NET 1.0")
                .WithApi(new Uri("http://127.0.0.1:8080"), "my-auth-key-1")
                .Build();

            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(tlsClient)
                .WithBaseUrl("https://httpbin.io")
                .Build();

            var request = new RestRequest("/get", Method.Get);
            var response = restClient.Execute(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}