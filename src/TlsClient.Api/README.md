# TlsClient.Api

A .NET client for **TlsClient.NET** that connects to a remote **TlsClient Service**.

This package builds on top of `TlsClient.Core` models (`Request`, `Response`, `TlsClientCookie`, etc.), but handles transport via the remote API.

---

## 📦 Installation

```bash
dotnet add package TlsClient.Api
```

> Unlike `TlsClient.Native`, you **do not** need `TlsClient.Initialize(...)` here.
> Simply provide the service base URL and an API key.

---

## 🚀 Quick Start

### Minimal Example

```csharp
using TlsClient.Api;
using TlsClient.Core.Models.Requests;

using var client = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");

var response = client.Request(new Request {
    RequestUrl = "https://httpbin.io/get"
});

Console.WriteLine($"{(int)response.Status} {response.Status}");
Console.WriteLine(response.Body);
```

### With Options

```csharp
using TlsClient.Api;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

var options = new ApiTlsClientOptions(
    TlsClientIdentifier.Chrome133,
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0",
    new Uri("http://127.0.0.1:8080"),
    "my-auth-key-1"
);

using var client = new ApiTlsClient(options);

var response = await client.RequestAsync(new Request { RequestUrl = "https://example.com" });
```

---

## 🧱 API Interface

The public interface of `ApiTlsClient`:

```csharp
public sealed class ApiTlsClient : BaseTlsClient, IAsyncDisposable
{
    public RestClient RestClient { get; }

    public ApiTlsClient(ApiTlsClientOptions options);
    public ApiTlsClient(Uri apiBaseUri, string apiKey);

    // Sync methods
    public override Response Request(Request request);
    public override GetCookiesFromSessionResponse AddCookies(string url, List<TlsClientCookie> cookies);
    public override DestroyResponse Destroy();
    public override GetCookiesFromSessionResponse GetCookies(string url);
    public override DestroyResponse DestroyAll();

    // Async methods
    public override Task<Response> RequestAsync(Request request, CancellationToken ct = default);
    public override Task<GetCookiesFromSessionResponse> AddCookiesAsync(string url, List<TlsClientCookie> cookies, CancellationToken ct = default);
    public override Task<DestroyResponse> DestroyAsync(CancellationToken ct = default);
    public override Task<GetCookiesFromSessionResponse> GetCookiesAsync(string url, CancellationToken ct = default);
    public override Task<DestroyResponse> DestroyAllAsync(CancellationToken ct = default);

    public override ValueTask DisposeAsync();
}
```

---

## 🧱 API Client — Builder Usage

Use the shared builder to configure common options, then switch to the API transport with `WithApi(...)` and call `Build()` to get an `ApiTlsClient`.

```csharp
    // 1) Create client
    using var client = new TlsClientBuilder()
        .WithIdentifier(TlsClientIdentifier.Chrome133)
        .WithUserAgent("MyApp/1.0")
        .WithFollowRedirects()
        .WithTimeout(TimeSpan.FromSeconds(15))
        .WithDefaultCookieJar()
        .WithHeader("Accept-Language", "en-US,en;q=0.9")
        .WithProxyUrl("http://127.0.0.1:8086", isRotating: false);
        .WithApi(new Uri("http://127.0.0.1:8080"), "my-auth-key-1")
        .Build();

    // 2) Make a request
    var response = await client.RequestAsync(new Request
    {
        RequestUrl = "https://httpbin.io/get"
    });

    Console.WriteLine($"{(int)response.Status} {response.Status}");
    Console.WriteLine(response.Body);
```

> Note:
> * You **must** call `.WithApi(...)` **before** `.Build()`; otherwise an `InvalidOperationException` is thrown (by design).
> * All options you set on the builder (headers, proxy, cookie jar, timeouts, etc.) are forwarded into `ApiTlsClientOptions` inside `WithApi(...)`.

---

## 🧯 Error Handling

* If serialization or service issues occur, `Response.Status = 0` and `Body` contains the error message.
* If the error contains `"Client.Timeout exceeded"`, the client normalizes it to `408 RequestTimeout`.
* When using `StreamOutputPath`, make sure the file path is writable.

---

## 🔬 Learn More

This documentation focuses on the API interface and basic usage.
For more detailed **usage scenarios and real-world examples**, check the test suite:
`TlsClient.Api.Tests` → `BodyTests` and `CookieTests`.
These cover JSON requests, form-data, multipart uploads, streaming to files, and cookie management in depth.

---

## 📜 License

MIT — see the root `LICENSE` file.
