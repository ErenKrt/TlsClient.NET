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
    public class CookieTests
    {
        public static readonly string BaseURL = "https://httpbin.io";

        static CookieTests()
        {
            NativeTlsClient.Initialize("D:\\Tools\\tls-client-windows-64-1.13.1.dll");
        }

        [Fact]
        public void Should_Include_Cookie()
        {
            using var tlsClient = new NativeTlsClient();
            tlsClient.Options.WithCustomCookieJar= true;

            var request = new Request()
            {
                RequestUrl = BaseURL + "/cookies",
                RequestCookies = new List<TlsClientCookie>
                {
                    new TlsClientCookie("sessionid", "123456")
                }
            };
            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("sessionid");
        }

        [Fact]
        public void Should_Keep_Cookie()
        {
            using var tlsClient = new NativeTlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0")
            {
                WithCustomCookieJar = true
            });
            var request = new Request()
            {
                RequestUrl = BaseURL + "/cookies/set?sessionid=123456",
            };
            var response = tlsClient.Request(request);

            var secondRequest = new Request()
            {
                RequestUrl = BaseURL + "/cookies",
            };

            var secondResponse = tlsClient.Request(secondRequest);
            secondResponse.Status.Should().Be(HttpStatusCode.OK);
            secondResponse.Body.Should().Contain("sessionid");
        }

        [Fact]
        public void Should_Not_Keep_Cookie_By_Request()
        {
            using var tlsClient = new NativeTlsClient();
            var request = new Request()
            {
                RequestUrl = BaseURL + "/cookies/set?sessionid=123456",
                WithoutCookieJar = true
            };
            var response = tlsClient.Request(request);
            var secondRequest = new Request()
            {
                RequestUrl = BaseURL + "/cookies",
                WithoutCookieJar = true
            };
            var secondResponse = tlsClient.Request(secondRequest);
            secondResponse.Status.Should().Be(HttpStatusCode.OK);
            secondResponse.Body.Should().NotContain("sessionid");
        }

        [Fact]
        public void Should_Not_Keep_Cookie_By_Client()
        {
            using var tlsClient = new NativeTlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0")
            {
                WithoutCookieJar = true
            });
            var request = new Request()
            {
                RequestUrl = BaseURL + "/cookies/set?sessionid=123456",
            };
            var response = tlsClient.Request(request);
            var secondRequest = new Request()
            {
                RequestUrl = BaseURL + "/cookies",
            };
            var secondResponse = tlsClient.Request(secondRequest);
            secondResponse.Status.Should().Be(HttpStatusCode.OK);
            secondResponse.Body.Should().NotContain("sessionid");
        }

        [Fact]
        public void Should_Include_Cookie_By_Header()
        {
            using var tlsClient = new NativeTlsClient();
            tlsClient.DefaultHeaders.Add("Cookie", new List<string> { "sessionid=123456" });

            var request = new Request()
            {
                RequestUrl = BaseURL + "/cookies",
            };
            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("sessionid");
        }

        [Fact]
        public void Should_Set_Cookie()
        {
            using var tlsClient = new NativeTlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0"));
            tlsClient.Options.WithCustomCookieJar = true;

            // First request for init session
            var firstRequest = new Request()
            {
                RequestUrl = BaseURL + "/cookies",
            };
            var firstResponse = tlsClient.Request(firstRequest);

            var addCookiesResponse= tlsClient.AddCookies(BaseURL, new List<TlsClientCookie>()
            {
                new TlsClientCookie("sessionid", "123456", "httpbin.io")
            });

            addCookiesResponse.Cookies.Should().NotBeNull();
            addCookiesResponse.Cookies.First().Name.Should().Be("sessionid");

            var request = new Request()
            {
                RequestUrl = BaseURL + "/cookies",
            };
            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("sessionid");
        }

        [Fact]
        public void Should_Get_Cookie()
        {
            using var tlsClient = new NativeTlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0"));

            var request = new Request()
            {
                RequestUrl = BaseURL + "/cookies/set?sessionid=123456",
            };

            var response = tlsClient.Request(request);

            var cookiesResponse = tlsClient.GetCookies(BaseURL + "/cookies");
            cookiesResponse.Should().NotBeNull();
            cookiesResponse.Cookies.Should().NotBeNull();
            cookiesResponse.Cookies.First().Name.Should().Be("sessionid");

        }
    }
}
