using FluentAssertions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TlsClient.RestSharp.Helpers.Builders;

namespace TlsClient.RestSharp.Tests
{
    public class BodyTests
    {
        static BodyTests()
        {
            Core.TlsClient.Initialize("D:\\Tools\\TlsClient\\tls-client-windows-64-1.10.0.dll");
        }

        [Fact]
        public void Should_Send_Json_Body()
        {
            using var client = new Core.TlsClient();
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();

            var request = new RestRequest("https://httpbin.org/post",Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new{ 
                key = "value"
            });
            var response = restClient.Execute(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Contain("\"key\": \"value\"");
        }

        [Fact]
        public void Should_Send_Form_Body()
        {
            using var client = new Core.TlsClient();
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("https://httpbin.org/post", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("key", "value");
            var response = restClient.Execute(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Contain("\"key\": \"value\"");
        }

        [Fact]
        public void Should_Send_File_Body()
        {
            File.WriteAllLines("testfile.txt", new[] { "This is a test file." });

            using var client = new Core.TlsClient();
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("https://httpbin.org/post", Method.Post);
            request.AddFile("myTestFile", "testfile.txt");
            var response = restClient.Execute(request);

            File.Delete("testfile.txt");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Contain("myTestFile");
        }

        [Fact]
        public void Should_Send_Multipart_Body()
        {
            File.WriteAllLines("testfile.txt", new[] { "This is a test file." });
            using var client = new Core.TlsClient();
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("https://httpbin.org/post", Method.Post);
            request.AlwaysMultipartFormData = true;
            request.AddFile("myTestFile", "testfile.txt");
            request.AddParameter("key", "value");
            var response = restClient.Execute(request);
            File.Delete("testfile.txt");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Contain("myTestFile");
            response.Content.Should().Contain("\"key\": \"value\"");
        }

        [Fact]
        public void Should_Send_Query_Parameters()
        {
            using var client = new Core.TlsClient();
            using var restClient = new TlsRestClientBuilder()
                .WithTlsClient(client)
                .WithBaseUrl("https://httpbin.io")
                .Build();
            var request = new RestRequest("https://httpbin.org/get", Method.Get);
            request.AddQueryParameter("key", "value");
            var response = restClient.Execute(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Contain("\"key\": \"value\"");
        }
    }
}
