using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TlsClient.Core.Helpers.Wrappers;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Core.Tests
{
    public class ClientTests
    {
        static ClientTests()
        {
            TlsClient.Initialize("D:\\Tools\\TlsClient\\tls-client-windows-64-1.10.0.dll");
        }

        [Fact]
        public void Should_Initialize_Client()
        {
            using var tlsClient = new TlsClient();
            Assert.NotNull(tlsClient);
        }

        [Fact]
        public void Should_Dispose_Client()
        {
            var client = new TlsClient();
            client.Dispose();
            Assert.True(client.IsDisposed);
        }

        [Fact]
        public void Should_Error_Without_Identifier()
        {
            var client = new TlsClient(new TlsClientOptions()
            {
                Timeout = TimeSpan.FromSeconds(5)
            });

            var request = new Request()
            {
                RequestUrl = "https://www.example.com"
            };

            var exception = Assert.Throws<ArgumentException>(() => client.Request(request));
            Assert.Equal("Either TlsClientIdentifier or CustomTlsClient must be set in Options or in Request.", exception.Message);
        }

        [Fact]
        public void Should_Dispose_Wrapper()
        {
            using var client = new TlsClient();
            

            var request= new Request()
            {
                RequestUrl = "https://www.example.com"
            };
            var response = client.Request(request);
            response.Status.Equals(HttpStatusCode.OK);

            TlsClientWrapper.Destroy();

            var exception = Assert.Throws<InvalidOperationException>(() => client.Request(request));
            Assert.Equal("TlsClientStaticWrapper not initialized. Call Initialize(string libraryPath) first.", exception.Message);
        }
    }
}
