using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Net;
using TlsClient.Core.Models;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Tests
{
    public class HeaderTests
    {
        [Fact]
        public async Task ShouldUserAgent()
        {
            var tlsClient = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome132, "TestClient 1.0"));
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/user-agent",
                RequestMethod = HttpMethod.Get,
            });
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("TestClient 1.0");
        }

        [Fact]
        public async Task ShouldIncludeAuthorizationHeader()
        {
            var tlsClient = new Core.TlsClient();
            tlsClient.DefaultHeaders.Add("Authorization", new List<string>() { "Bearer my-token" });
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/headers",
                RequestMethod = HttpMethod.Get,
            });

            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Contains($"\"Authorization\": \"my-token\"");
        }

        [Fact]
        public async Task ShouldRespectHeaderOrder()
        {
            var tlsClient = new Core.TlsClient();
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/headers",
                RequestMethod = HttpMethod.Get,
                Headers= new Dictionary<string, string>()
                {
                    { "X-Custom-A", "1" },
                    { "X-Custom-B", "2" },
                    { "X-Custom-C", "3" },
                },
                HeaderOrder = new List<string>()
                {
                    "X-Custom-C",
                    "X-Custom-B",
                    "X-Custom-A",
                }
            });

            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("\"X-Custom-C\": \"3\"")
               .And.Contain("\"X-Custom-B\": \"2\"")
               .And.Contain("\"X-Custom-A\": \"1\"");
        }

        [Fact]
        public async Task ShouldApplyCycledHeader()
        {
            var tlsClient = new Core.TlsClient();
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/headers",
                RequestMethod = HttpMethod.Get,
            });

            response.Body.Should().NotContain("X-Cycled-Token");

            tlsClient.DefaultHeaders["X-Cycled-Token"] = new List<string> { "abc123" };

            var response2 = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/headers",
                RequestMethod = HttpMethod.Get,
            });

            response2.Status.Should().Be(HttpStatusCode.OK);
            response2.Body.Should().Contain("X-Cycled-Token");
        }

        [Fact] 
        public async Task ShouldUseServerNameOverwrite()
        {

        }
    }
}