// See https://aka.ms/new-console-template for more information
using TlsClient.Core.Builders;
using TlsClient.Core.Models.Requests;
using TlsClient.Native.Extensions;

DotNetEnv.Env.Load();


//TlsClientWrapper.Initialize(DotNetEnv.Env.GetString("TLS_CLIENT_PATH"));
var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClient.Core.Models.Entities.TlsClientIdentifier.Chrome133)
    .WithUserAgent("TlsClient-Example")
    .WithNative(DotNetEnv.Env.GetString("TLS_CLIENT_PATH"))
    .Build();

Console.WriteLine("Hello, Friend !");

// parallel for 10000 request
await Parallel.ForAsync(0, 10000, async (i, cancellationToken) =>
{
    var request = new Request()
    {
        RequestUrl = "https://httpbin.io/get",
        RequestMethod = HttpMethod.Get,
        Headers = new Dictionary<string, string>()
       {
           { "User-Agent", "TlsClient-Example" }
       }
    };
    var response = await tlsClient.RequestAsync(request);
    Console.WriteLine($"Response {i + 1}: {response.Status} - {response.Body.Substring(0, Math.Min(100, response.Body.Length))}...");
});
