using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TlsClient.Core.Helpers;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Core.Tests
{
    public class PerformanceTests
    {
        static PerformanceTests()
        {
            TlsClient.Initialize("D:\\Tools\\TlsClient\\tls-client-windows-64-1.11.0.dll");
        }

        [Fact]
        public async Task Should_Perform_Multiple_Requests()
        {
            const int parallelCount = 20;
            const int requestsPerThread = 10;
            string url = "https://tls.peet.ws/api/all";

            var tasks = new List<Task>();
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < parallelCount; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    using var tlsClient = new TlsClient(new TlsClientOptions
                    {
                        TlsClientIdentifier = TlsClientIdentifier.Chrome132,
                        Timeout = TimeSpan.FromSeconds(10)
                    });

                    for (int j = 0; j < requestsPerThread; j++)
                    {
                        var request = new Request
                        {
                            RequestUrl = url
                        };
                        var response = tlsClient.Request(request);
                        response.Status.Should().Be(HttpStatusCode.OK);
                    }
                }));
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();
            Console.WriteLine($"Total time for {parallelCount * requestsPerThread} requests: {stopwatch.Elapsed.TotalSeconds} seconds");
        }

        [Fact]
        public async Task Should_Not_Leak_Memory()
        {
            const int parallelCount = 10;
            const int requestsPerThread = 20;
            string url = "https://tls.peet.ws/api/all";

            // Warmup
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long memoryBefore = GC.GetTotalMemory(forceFullCollection: true);

            var tasks = new List<Task>();

            for (int i = 0; i < parallelCount; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    using var tlsClient = new TlsClient(new TlsClientOptions
                    {
                        TlsClientIdentifier = TlsClientIdentifier.Chrome132,
                        Timeout = TimeSpan.FromSeconds(10)
                    });

                    for (int j = 0; j < requestsPerThread; j++)
                    {
                        var request = new Request
                        {
                            RequestUrl = url
                        };
                        var response = tlsClient.Request(request);
                        response.Status.Should().Be(HttpStatusCode.OK);
                    }
                }));
            }

            await Task.WhenAll(tasks);

            // Force GC and measure again
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long memoryAfter = GC.GetTotalMemory(forceFullCollection: true);

            Console.WriteLine($"Memory before: {memoryBefore / 1024 / 1024} MB");
            Console.WriteLine($"Memory after : {memoryAfter / 1024 / 1024} MB");
            Console.WriteLine($"Memory diff  : {(memoryAfter - memoryBefore) / 1024 / 1024} MB");
 
            (memoryAfter - memoryBefore).Should().BeLessThan(50 * 1024 * 1024, "memory usage should not grow excessively");
        }
    }
}