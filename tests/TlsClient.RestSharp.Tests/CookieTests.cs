using FluentAssertions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TlsClient.Core.Models.Entities;
using TlsClient.RestSharp.Helpers.Builders;

namespace TlsClient.RestSharp.Tests
{
    public class CookieTests
    {
        static CookieTests()
        {
            Core.TlsClient.Initialize("D:\\Tools\\TlsClient\\tls-client-windows-64-1.10.0.dll");
        }

        [Fact]
        public void Should_Keep_Cookie()
        {
            using var client = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0")
            {
                WithDefaultCookieJar = true
            });
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();

            var setCookiesRequest= new RestRequest("/cookies/set?sessionid=123456");
            var setCookiesResponse = restClient.Execute(setCookiesRequest);

            var request = new RestRequest("/cookies");
            var response = restClient.Execute(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Contain("sessionid");
        }

        [Fact]
        public void Should_Not_Keep_Cookie()
        {
            using var client = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0")
            {
                WithoutCookieJar = true
            });
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();

            var setCookiesRequest = new RestRequest("/cookies/set?sessionid=123456");
            var setCookiesResponse = restClient.Execute(setCookiesRequest);

            var request = new RestRequest("/cookies");
            var response = restClient.Execute(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotContain("sessionid");
        }

        [Fact]
        public void Should_Set_Cookie_From_Client()
        {
            using var client = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0")
            {
                WithDefaultCookieJar = true
            });
            
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();

            // Init session with first request
            restClient.Execute(new RestRequest("/cookies"));

            var cookieResponse = client.AddCookies("https://httpbin.io", new List<TlsClientCookie>()
            {
                new TlsClientCookie("sessionid", "123456", "httpbin.io")
            });

            var request = new RestRequest("/cookies");
            var response = restClient.Execute(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Contain("sessionid");
        }

        [Fact]
        public void Should_Set_Cookie_From_Request()
        {
            using var client = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0")
            {
                WithoutCookieJar = true
            });
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("/cookies");
            request.AddCookie("sessionid", "123456", "/", "httpbin.io");
            request.AddCookie("othercookie", "654321", "/", "httpbin.io");
            var response = restClient.Execute(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Contain("sessionid");
            response.Content.Should().Contain("othercookie");
        }

        [Fact]
        public void Should_Set_Cookie_From_RestClient()
        {
            var cookieJar = new CookieContainer();
            cookieJar.Add(new Uri("https://httpbin.io"), new Cookie("sessionid", "123456"));

            using var client = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0")
            {
                WithDefaultCookieJar = true,
            });

            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .WithConfigureRestClient(c => c.CookieContainer = cookieJar)
                .Build();

            var request = new RestRequest("/cookies");
            var response = restClient.Execute(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Contain("sessionid");
        }

        [Fact]
        public void Should_Set_Cookie_From_Response_To_Container()
        {
            var cookieJar = new CookieContainer();
            
            using var client = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome133, "TlsClient.NET 1.0")
            {
                WithDefaultCookieJar = true,
            });

            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .WithConfigureRestClient(c => c.CookieContainer = cookieJar)
                .Build();

            var request = new RestRequest("/cookies/set?sessionid=123456");
            var response = restClient.Execute(request);

            var allCookies = cookieJar.GetAllCookies();

            response.StatusCode.Should().Be(HttpStatusCode.Found);
            allCookies.Should().NotBeNull();
            allCookies.Count.Should().Be(1);
            allCookies[0].Name.Should().Be("sessionid");
        }
    }
}
