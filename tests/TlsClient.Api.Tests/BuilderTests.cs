using FluentAssertions;
using System.Net;
using TlsClient.Core.Builders;
using TlsClient.Core.Helpers;
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
                .WithCustomCookieJar()
                .WithInsecureSkipVerify()
                .WithHeader("show", "must go on")
                .WithDebug()
                .WithCatchPanics()
                .WithRandomTLSExtensionOrder()
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

        [Fact]
        public void Should_Configure_Disable_Session_Tickets()
        {
            var tlsClient = new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome133)
                .WithDisableSessionTickets()
                .WithApi(new Uri("http://127.0.0.1:8080"), "my-auth-key-1")
                .Build();

            var disabledRequest = new RequestBuilder()
                .WithDisableSessionTickets()
                .Build();
            var enabledRequest = new RequestBuilder()
                .WithDisableSessionTickets(false)
                .Build();

            tlsClient.Options.DisableSessionTickets.Should().BeTrue();
            disabledRequest.DisableSessionTickets.Should().BeTrue();
            enabledRequest.DisableSessionTickets.Should().BeFalse();

            tlsClient.HttpClient.Dispose();
        }

        [Fact]
        public void Should_Serialize_Trust_Anchors_Payload()
        {
            var request = new Request()
            {
                CustomTlsClient = new CustomTlsClient
                {
                    TrustAnchorsPayload = "test-trust-anchors"
                }
            };

            var payload = request.ToJson();

            payload.Should().Contain("\"trustAnchorsPayload\": \"test-trust-anchors\"");
        }
    }
}
