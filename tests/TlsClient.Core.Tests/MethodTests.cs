using System.Net;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Core.Tests
{
    public class MethodTests
    {
        public static readonly string BaseURL = "https://httpbin.io";
        static MethodTests()
        {
            TlsClient.Initialize("D:\\Tools\\TlsClient\\tls-client-windows-64-1.10.0.dll");
        }

        [Fact]
        public void Get_Should_Return_200()
        {
            using var client = new TlsClient();

            var request = new Request()
            {
                RequestUrl = BaseURL+"/get"
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public void Get_Should_Return_405()
        {
            using var client = new TlsClient();

            var request = new Request()
            {
                RequestUrl = BaseURL+"/get",
                RequestMethod= HttpMethod.Post
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.Status);
        }

        [Fact]
        public void Post_Should_Return_200()
        {
            using var client = new TlsClient();

            var request = new Request()
            {
                RequestUrl = BaseURL+"/post",
                RequestMethod = HttpMethod.Post,
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public void Delete_Should_Return_200()
        {
            using var client = new TlsClient();

            var request = new Request()
            {
                RequestUrl = BaseURL+"/delete",
                RequestMethod = HttpMethod.Delete,
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public void Put_Should_Return_200()
        {
            using var client = new TlsClient();

            var request = new Request()
            {
                RequestUrl = BaseURL+"/put",
                RequestMethod = HttpMethod.Put,
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public void Patch_Should_Return_200()
        {
            using var client = new TlsClient();

            var request = new Request()
            {
                RequestUrl = BaseURL+"/patch",
                RequestMethod = HttpMethod.Patch,
            };
            var response = client.Request(request);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }
    }
}
