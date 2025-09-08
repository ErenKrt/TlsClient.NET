using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TlsClient.Core.Builders;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.Native;
using TlsClient.Native.Extensions;

namespace TlsClient.Core.Tests
{
    public class BuilderTests
    {
        static BuilderTests()
        {
            NativeTlsClient.Initialize("D:\\Tools\\TlsClient\\tls-client-windows-64-1.11.0.dll");
        }


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
        public void Should_Build_Request()
        {
            using var tlsClient = new NativeTlsClient();

            var request = new RequestBuilder()
                .WithUrl("https://httpbin.io/headers")
                .WithMethod(HttpMethod.Get)
                .WithHeader("show", "must go on")
                .WithCookie("hi", "there")
                .WithBody("hi")
                .Build();
            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("must go on");
            response.Body.Should().Contain("hi=there");
        }
    }
}