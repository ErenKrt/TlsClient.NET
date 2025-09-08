using System.Net;
using TlsClient.Api.Models.Entities;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;


namespace TlsClient.Api.Tests
{
    public class StatusTests
    {
        public static readonly string BaseURL = "https://httpbin.io";
 
        [Fact]
        public void Should_Return_200()
        {
            using var client = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");

            var request = new Request()
            {
                RequestUrl = BaseURL+"/status/200"
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public void Should_Return_500()
        {
            using var client = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");

            var request = new Request()
            {
                RequestUrl = BaseURL+"/status/500",
                RequestMethod= HttpMethod.Post
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.InternalServerError, response.Status);
        }

        [Fact]
        public void Should_Follow_Redirect()
        {
            using var client = new ApiTlsClient(new ApiTlsClientOptions(new Uri("http://127.0.0.1:8080"), "my-auth-key-1")
            {
                TlsClientIdentifier= TlsClientIdentifier.Chrome133,
                FollowRedirects= true
            });

            var request = new Request()
            {
                RequestUrl = BaseURL + "/redirect/1",
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public void Should_Not_Follow_Redirect()
        {
            using var client = new ApiTlsClient(new ApiTlsClientOptions(new Uri("http://127.0.0.1:8080"), "my-auth-key-1")
            {
                TlsClientIdentifier = TlsClientIdentifier.Chrome133,
                FollowRedirects = false
            });
            var request = new Request()
            {
                RequestUrl = BaseURL + "/redirect/1",
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.Found, response.Status);
        }

        [Fact]
        public void Should_Timeout()
        {
            using var client = new ApiTlsClient(new ApiTlsClientOptions(new Uri("http://127.0.0.1:8080"), "my-auth-key-1")
            {
                TlsClientIdentifier = TlsClientIdentifier.Chrome133,
                Timeout= TimeSpan.FromSeconds(5)
            });
            var request = new Request()
            {
                RequestUrl = BaseURL + "/delay/8",
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.RequestTimeout, response.Status);
        }

        [Fact]
        public void Should_Not_Timeout()
        {
            using var client = new ApiTlsClient(new ApiTlsClientOptions(new Uri("http://127.0.0.1:8080"), "my-auth-key-1")
            {
                TlsClientIdentifier = TlsClientIdentifier.Chrome133,
                Timeout = TimeSpan.FromSeconds(10)
            });
            var request = new Request()
            {
                RequestUrl = BaseURL + "/delay/5",
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

    }
}
