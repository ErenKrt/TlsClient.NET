# TlsClient.Native

A native (in-process) client for **TlsClient.NET**.

---

## 📦 Installation

```bash
dotnet add package TlsClient.Native
```

Initialize the native library once at process start:

```csharp
using TlsClient.Native;

NativeTlsClient.Initialize("{PATH_TO_NATIVE_LIBRARY}");
// e.g. Windows: "C:\\tools\\tls-client\\tls-client-windows-64-1.11.0.dll"
```

---

## 🚀 Quick Start

```csharp
using TlsClient.Native;
using TlsClient.Core.Models.Requests;

NativeTlsClient.Initialize("{PATH_TO_NATIVE_LIBRARY}");

using var client = new NativeTlsClient(); // or pass TlsClientOptions

var res = client.Request(new Request {
    RequestUrl = "https://httpbin.io/get"
});

Console.WriteLine($"{(int)res.Status} {res.Status}");
Console.WriteLine(res.Body);
```

> For advanced configuration, construct with `new NativeTlsClient(new TlsClientOptions(...))`.

---

## 🧱 API Interface

```csharp
public sealed class NativeTlsClient : BaseTlsClient
{
    // Static bootstrap
    public static void Initialize(string? libraryPath);

    // Constructors
    public NativeTlsClient(TlsClientOptions options);
    public NativeTlsClient();

    // Sync
    public override Response Request(Request request);
    public override GetCookiesFromSessionResponse GetCookies(string url);
    public override GetCookiesFromSessionResponse AddCookies(string url, List<TlsClientCookie> cookies);
    public override DestroyResponse Destroy();
    public override DestroyResponse DestroyAll();

    // Async (wrapping sync)
    public override Task<Response> RequestAsync(Request request, CancellationToken ct = default);
    public override Task<GetCookiesFromSessionResponse> GetCookiesAsync(string url, CancellationToken ct = default);
    public override Task<GetCookiesFromSessionResponse> AddCookiesAsync(string url, List<TlsClientCookie> cookies, CancellationToken ct = default);
    public override Task<DestroyResponse> DestroyAsync(CancellationToken ct = default);
    public override Task<DestroyResponse> DestroyAllAsync(CancellationToken ct = default);
}
```

## 🧱 Native Client — Builder Usage

Use the shared `TlsClientBuilder` to configure common options, then switch to the native transport with `WithNative(...)` and call `Build()` to get a `NativeTlsClient`.

```csharp
    // 1) Initialize native library once
NativeTlsClient.Initialize("C:\\tools\\tls-client\\tls-client-windows-64-1.11.0.dll");
    // 2) Create client
using var client = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome133)
    .WithUserAgent("MyApp/1.0")
    .WithFollowRedirects()
    .WithTimeout(TimeSpan.FromSeconds(15))
    .WithDefaultCookieJar()
    .WithHeader("Accept-Language", "en-US,en;q=0.9")
    .WithProxyUrl("http://127.0.0.1:8086", isRotating: false);
    .withNative() // OPTIONAL
    .build()

    // 3) Make a request
var response = await client.RequestAsync(new Request
{
     RequestUrl = "https://httpbin.io/get"
});
```
---

> ⚠️ **Important:**
>
> * Call `NativeTlsClient.Initialize(path)` **once per project** before creating clients.
> * You **can** call `.WithNative(path)` if you dont want use `NativeTlsClient.Initialize(path)`.
> * Due to C# ↔ Go interop, native mode can be less stable under heavy concurrency. If you run into issues, prefer the API mode.

---

### Under the Hood

* `Initialize(path)` loads the native library via `TlsClientWrapper.Initialize(...)`.
* `Request(request)`:

  * Merges client defaults via `PrepareRequest(...)`.
  * Serializes with `RequestHelpers.Prepare(...)`, calls `TlsClientWrapper.Request(...)`.
  * Deserializes to `Response` and frees native memory with `TlsClientWrapper.FreeMemory(response.Id)` when present.
  * Normalizes timeouts: if `Status == 0` and `Body` contains `"Client.Timeout exceeded"`, returns `408 RequestTimeout`.
* Cookie and destroy operations map to corresponding wrapper functions and return strongly-typed results.

---

## ⚙️ Requests & Responses (same as Core)

* **Request** highlights: `RequestUrl` (required), `RequestMethod`, `Headers`, `RequestCookies`, `RequestBody` (string or bytes via `RequestHelpers.PrepareBody(...)`), `IsByteRequest/IsByteResponse`, `StreamOutputPath`, TLS/transport flags (`TlsClientIdentifier`, `WithDefaultCookieJar`, `WithoutCookieJar`, `FollowRedirects`, `Timeout*`, etc.).
* **Response**: `Status` (`HttpStatusCode`), `Body` (`string?`, empty if streamed to file), `Headers` (`Dictionary<string, List<string>>`), `Id` (internal handle used to free native buffers).

---

## 🧯 Error Handling

* Native failures set `Response.Status = 0` and include the error message in `Response.Body`.
* `"Client.Timeout exceeded"` is normalized to `408 RequestTimeout`.
* Always call `Initialize(...)` **before** the first request.
* Ensure `StreamOutputPath` targets a writable location.

---

## 🔬 Learn More

This page focuses on the API surface and basic usage.
For **additional scenarios**, please refer to the test projects (e.g., `BodyTests`, `CookieTests`).

---

## 📜 License

MIT — see the root `LICENSE` file.