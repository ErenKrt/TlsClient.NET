using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TlsClient.Api.Models.Entities;
using TlsClient.Core.Helpers;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Api.Tests
{
    public class BodyTests
    {

        [Fact]
        public void Should_Download_Image()
        {
            using var tlsClient = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");

            var tmpFile= Path.GetTempFileName();

            var request = new Request()
            {
                RequestUrl = "https://avatars.githubusercontent.com/u/26926124?v=4",
                StreamOutputPath = tmpFile,
            };
            var response = tlsClient.Request(request);
            var contentLength = response.Headers?.FirstOrDefault(x => x.Key == "Content-Length").Value.FirstOrDefault();
            Assert.NotNull(contentLength);

            var fileInfo = new FileInfo(tmpFile);
            var fileLength = fileInfo.Length;
            File.Delete(tmpFile);

            response.Status.Should().Be(HttpStatusCode.OK);
            fileLength.Should().Be(long.Parse(contentLength));
        }

        [Fact]
        public void Should_Send_Json()
        {
            using var tlsClient = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            var jsonBody = "{\"title\":\"foo\",\"body\":\"bar\",\"userId\":1}";
            var request = new Request()
            {
                RequestUrl = "https://jsonplaceholder.typicode.com/posts",
                RequestMethod = HttpMethod.Post,
                RequestBody = RequestHelpers.PrepareBody(Encoding.UTF8.GetBytes(jsonBody)),
                IsByteRequest = true,
                Headers = new Dictionary<string, string>()
                {
                    { "Content-Type", "application/json; charset=UTF-8" }
                }
            };
            var response = tlsClient.Request(request);

            response.Status.Should().Be(HttpStatusCode.Created);
            response.Body.Should().Contain("foo");
            response.Body.Should().Contain("bar");
            response.Body.Should().Contain("1");
        }

        [Fact]
        public void Should_Send_Form()
        {
            using var tlsClient = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
            var request = new Request()
            {
                RequestUrl = "https://postman-echo.com/post",
                RequestMethod = HttpMethod.Post,
                RequestBody = "foo1=bar1&foo2=bar2",
                Headers = new Dictionary<string, string>()
                {
                    { "Content-Type", "application/x-www-form-urlencoded" }
                }
            };

            var response = tlsClient.Request(request);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("foo1");
            response.Body.Should().Contain("bar1");
            response.Body.Should().Contain("foo2");

        }

        [Fact]
        public async Task Should_Send_File()
        {
            using var tlsClient = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");

            var filePath = "testfile.txt";
            File.WriteAllText(filePath, "This is a test file.");
            var fileBytes = File.ReadAllBytes(filePath);

            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new ByteArrayContent(fileBytes), "file", "testfile.txt");
            
            var request = new Request()
            {
                RequestUrl = "https://postman-echo.com/post",
                RequestMethod = HttpMethod.Post,
                RequestBody = RequestHelpers.PrepareBody(await requestContent.ReadAsByteArrayAsync()),
                IsByteRequest = true,
                Headers = new Dictionary<string, string>()
                {
                    { "Content-Type", requestContent.Headers.ContentType?.ToString() }
                }
            };
            var response = tlsClient.Request(request);
            File.Delete(filePath);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("testfile.txt");
        }
    }
}