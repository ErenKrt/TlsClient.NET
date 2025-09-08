using RestSharp;
using System.Net;
using TlsClient.RestSharp.Helpers.Builders;

namespace TlsClient.Api.RestSharp.Tests
{
    public class MethodTests
    {

        [Fact]
        public void Get_Should_Return_200()
        {
            using var client = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            using var restClient= new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("/get", Method.Get);
            var response = restClient.Execute(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Get_Should_Return_405()
        {
            using var client = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("/get", Method.Post);
            var response = restClient.Execute(request);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public void Post_Should_Return_200()
        {
            using var client = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("/post", Method.Post);
            var response = restClient.Execute(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Delete_Should_Return_200()
        {
            using var client = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("/delete", Method.Delete);
            var response = restClient.Execute(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Put_Should_Return_200()
        {
            using var client = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("/put", Method.Put);
            var response = restClient.Execute(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Patch_Should_Return_200()
        {
            using var client = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("/patch", Method.Patch);
            var response = restClient.Execute(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
