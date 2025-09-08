using FluentAssertions;
using System.Net;
using TlsClient.Core.Builders;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.Api.Extensions;

namespace TlsClient.Api.Tests
{
    public class BuilderTests
    {

        [Fact]
        public void Should_Build_Client()
        {
            using var tlsClient = new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome133)
                .WithTimeout(TimeSpan.FromSeconds(10))
                .WithDefaultCookieJar()
                .WithInsecureSkipVerify()
                .WithHeader("show", "must go on")
                .WithDebug()
                .WithCatchPanics()
                .WithRandomTLSExtensionOrder()
                .WithDefaultCookieJar()
                .WithUserAgent("TlsClient.NET 1.0")
                .WithApi(new Uri("http://127.0.0.1:8080"), "my-auth-key-1")
                .Build();

            var request = new Request()
            {
                RequestUrl = "https://httpbin.io/headers",
            };
            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("TlsClient.NET 1.0");
            response.Body.Should().Contain("must go on");
        }
    }
}