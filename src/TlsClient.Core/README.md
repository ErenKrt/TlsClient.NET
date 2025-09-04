# TlsClient.Core

Core library for **TlsClient.NET**.
Provides primitives to configure and build a `TlsClient` with advanced TLS capabilities (fingerprinting, browser identifiers, user-agent control, redirect & timeout policies, certificate validation toggles, cookie handling, etc.).

This package is the foundation used by subpackages like:

* `TlsClient.HttpClient` (via `TlsClientHandler`)
* `TlsClient.RestSharp` (via `TlsRestClientBuilder`)

---

## 📦 Installation

Install from NuGet:

```bash
dotnet add package TlsClient.Core
```

---

## 🚀 Quick Start

### Example 1 – Direct Options

```csharp
using TlsClient.Core;

TlsClient.Initialize("{LIBRARY_PATH}");

var tlsClient = new TlsClient(
    new TlsClientOptions
    {
        TlsClientIdentifier = TlsClientIdentifier.Chrome132,
        FollowRedirects = true,
        Timeout = TimeSpan.FromSeconds(15),
        InsecureSkipVerify = true,
        DefaultHeaders = new()
        {
            { "User-Agent", new() { "TlsClient.NET 1.0" } }
        }
    }
);
```

### Example 2 – Fluent Builder

```csharp
using TlsClient.Core;

TlsClient.Initialize("{LIBRARY_PATH}");

var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("TestClient 1.0")
    .WithFollowRedirects()
    .WithTimeout(TimeSpan.FromSeconds(15))
    .WithDefaultCookieJar()
    .Build();
```

> ℹ️ Use `TlsClient.HttpClient` or `TlsClient.RestSharp` to integrate into existing stacks.

> ⚠️ Without `TlsClient.Initialize(path)` the library will not work.

---

## ⚙️ Options (`TlsClientOptions`)

| Property                  | Type                               | Default    | Description                                 |
| ------------------------- | ---------------------------------- | ---------- | ------------------------------------------- |
| `TlsClientIdentifier`     | `TlsClientIdentifier?`             | `null`     | Predefined browser/profile identifier.      |
| `ProxyURL`                | `string?`                          | `null`     | Proxy URL (e.g. `"http://127.0.0.1:8888"`). |
| `Timeout`                 | `TimeSpan`                         | `00:01:00` | Global timeout.                             |
| `FollowRedirects`         | `bool`                             | `false`    | Auto-follow HTTP redirects.                 |
| `InsecureSkipVerify`      | `bool`                             | `false`    | Skip cert validation.    |
| `WithDefaultCookieJar`    | `bool`                             | `false`    | Enable default cookie jar.                  |
| `WithoutCookieJar`        | `bool`                             | `false`    | Disable all cookies.                        |
| `WithDebug`               | `bool`                             | `false`    | Enable debug logging.                       |
| `CertificatePinningHosts` | `Dictionary<string,List<string>>?` | `null`     | Host → fingerprints.                        |
| `HeaderOrder`             | `List<string>?`                    | `null`     | Custom header ordering.                     |

---

## 🧰 Builder (`TlsClientBuilder`)

Fluent API to configure a `TlsClient`.

| Method                   | Parameters                          | Description             |
| ------------------------ | ----------------------------------- | ----------------------- |
| `WithIdentifier`         | `TlsClientIdentifier id`            | Sets client identifier. |
| `WithUserAgent`          | `string ua`                         | Sets `User-Agent`.      |
| `WithProxyUrl`           | `string url, bool isRotating=false` | Configures proxy.       |
| `WithTimeout`            | `TimeSpan t`                        | Sets timeout.           |
| `WithFollowRedirects`    | `bool enabled=true`                 | Enables redirects.      |
| `WithDefaultCookieJar`   | `bool enabled=true`                 | Enables cookie jar.     |
| `WithoutCookieJar`       | `bool enabled=true`                 | Disables cookie jar.    |
| `WithHeader`             | `string key, string value`          | Adds header.            |
| `WithCertificatePinning` | `string host, List<string> pins`    | Adds pinning.           |
| `Build`                  | —                                   | Creates `TlsClient`.    |

### Example 1 – Proxy + Headers

```csharp
TlsClient.Initialize("{LIBRARY_PATH}");

var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithProxyUrl("http://127.0.0.1:8080", isRotating: true)
    .WithHeader("Accept-Language", "en-US,en;q=0.9")
    .Build();
```

### Example 2 – With Pinning

```csharp
TlsClient.Initialize("{LIBRARY_PATH}");

var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithCertificatePinning("example.com", new List<string>
    {
        "sha256/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA="
    })
    .Build();
```

---

Tabii 👍 İşte sadece **`Request` kısmı** (tüm property’ler dahil, Markdown formatında döküm):

---

# 📦 Request (`Request`)

The `Request` class represents a single HTTP request configuration.
Below are all available properties:

| Property                      | Type                               | Default          | Description                                     |
| ----------------------------- | ---------------------------------- | ---------------- | ----------------------------------------------- |
| `TlsClientIdentifier`         | `TlsClientIdentifier?`             | `null`           | Overrides global TLS client identifier.         |
| `CustomTlsClient`             | `CustomTlsClient?`                 | `null`           | Custom TLS client for this request.             |
| `TransportOptions`            | `TransportOptions?`                | `null`           | Advanced transport options.                     |
| `Headers`                     | `Dictionary<string,string>`        | empty            | Request headers (single values).                |
| `DefaultHeaders`              | `Dictionary<string,List<string>>?` | `null`           | Multi-value default headers.                    |
| `ConnectHeaders`              | `Dictionary<string,List<string>>?` | `null`           | Headers for proxy `CONNECT`.                    |
| `CertificatePinningHosts`     | `Dictionary<string,List<string>>?` | `null`           | Host → SHA256 pins.                             |
| `LocalAddress`                | `string?`                          | `null`           | Local bind address.                             |
| `ServerNameOverwrite`         | `string?`                          | `null`           | Override SNI / TLS server name.                 |
| `ProxyUrl`                    | `string?`                          | `null`           | Per-request proxy URL.                          |
| `RequestBody`                 | `string?`                          | `null`           | Raw request body.                               |
| `RequestHostOverride`         | `string?`                          | `null`           | Override `Host` header.                         |
| `SessionId`                   | `Guid?`                            | `null`           | Session identifier override.                    |
| `StreamOutputBlockSize`       | `int?`                             | `null`           | Block size for streaming output.                |
| `StreamOutputEOFSymbol`       | `string?`                          | `null`           | EOF marker in stream.                           |
| `StreamOutputPath`            | `string?`                          | `null`           | Stream response to file path.                   |
| `RequestMethod`               | `HttpMethod`                       | `HttpMethod.Get` | HTTP method.                                    |
| `RequestUrl`                  | `string`                           | `""`             | Target request URL (**required**).              |
| `HeaderOrder`                 | `List<string>?`                    | `[]`             | Custom header order.                            |
| `RequestCookies`              | `List<TlsClientCookie>?`           | `null`           | Cookies for this request.                       |
| `TimeoutMilliseconds`         | `int?`                             | `null`           | Timeout in milliseconds.                        |
| `TimeoutSeconds`              | `int?`                             | `null`           | Timeout in seconds.                             |
| `CatchPanics`                 | `bool?`                            | `null`           | Catch panics from native TLS client.            |
| `FollowRedirects`             | `bool?`                            | `null`           | Request-level redirect policy.                  |
| `ForceHttp1`                  | `bool?`                            | `null`           | Force HTTP/1.1.                                 |
| `InsecureSkipVerify`          | `bool?`                            | `null`           | Skip certificate validation (**testing only**). |
| `IsByteRequest`               | `bool`                             | `false`          | Marks request body as byte-based.               |
| `IsByteResponse`              | `bool`                             | `false`          | Return response as raw bytes.                   |
| `IsRotatingProxy`             | `bool?`                            | `null`           | Indicates rotating proxy usage.                 |
| `DisableIPV6`                 | `bool?`                            | `null`           | Disable IPv6.                                   |
| `DisableIPV4`                 | `bool?`                            | `null`           | Disable IPv4.                                   |
| `WithDebug`                   | `bool?`                            | `null`           | Enable debug logging.                           |
| `WithDefaultCookieJar`        | `bool?`                            | `null`           | Enable default cookie jar.                      |
| `WithoutCookieJar`            | `bool?`                            | `null`           | Disable cookie jar.                             |
| `WithRandomTLSExtensionOrder` | `bool?`                            | `null`           | Randomize TLS extension order.                  |

---

## 🧰 Request Builder (`RequestBuilder`)

Simplifies building a `Request`.

| Method                 | Parameters           | Description              |
| ---------------------- | -------------------- | ------------------------ |
| `WithUrl`              | `string url`         | Sets target URL.         |
| `WithMethod`           | `HttpMethod m`       | Sets HTTP method.        |
| `WithHeader`           | `string k, string v` | Adds header.             |
| `WithBody`             | `string/json/bytes`  | Sets body.               |
| `WithCookie`           | `string n, string v` | Adds cookie.             |
| `WithStreamOutputPath` | `string path`        | Stream response to file. |
| `Build`                | —                    | Returns `Request`.       |

### Example 1 – JSON POST

```csharp
var req = new RequestBuilder()
    .WithUrl("https://httpbin.org/post")
    .WithMethod(HttpMethod.Post)
    .WithBody(new { email = "user@example.com", active = true })
    .WithHeader("Content-Type", "application/json")
    .Build();
```

### Example 2 – GET with Cookies

```csharp
var req = new RequestBuilder()
    .WithUrl("https://example.com/report")
    .WithMethod(HttpMethod.Get)
    .WithCookie("session", "abcdef")
    .WithStreamOutputPath("C:\\temp\\report.bin")
    .Build();
```

---

## 🧯 Error Handling

* Builders throw `ArgumentException` / `InvalidOperationException` for invalid inputs.
* Network/TLS issues bubble up as standard .NET exceptions.

---

## 🧪 Testing Tips

* Use `https://httpbin.org` to test headers, cookies, TLS behavior.
* Toggle `InsecureSkipVerify` **only in controlled test environments**.

---

## 📜 License

MIT License – see the root `LICENSE` file.