# TlsClient.Net

A **.NET wrapper** for [bogdanfinn/tls-client](https://github.com/bogdanfinn/tls-client).
Provides advanced TLS fingerprinting, browser emulation, header ordering, cookie handling, certificate validation control, and proxy support for .NET applications.

---

## 📦 Subpackages

* **[TlsClient.Core](./src/TlsClient.Core/README.md)** → Core library (options, builders, primitives).
* **[TlsClient.HttpClient](./src/TlsClient.HttpClient/README.md)** → Drop-in `HttpClientHandler` support.
* **[TlsClient.RestSharp](./src/TlsClient.RestSharp/README.md)** → RestSharp client integration.

---

## ⚙️ Installation

Install the core package:

```bash
dotnet add package TlsClient.Core
```

Then, depending on your OS/architecture, install the appropriate **native library**:

| Operating System | Architecture          | Package to Install                    |
| ---------------- | --------------------- | ------------------------------------- |
| Windows          | x64 (64-bit)          | `TlsClient.Native.win-x64`            |
| Windows          | x86 (32-bit)          | `TlsClient.Native.win-x32`            |
| Linux (Ubuntu)   | AMD64 (64-bit)        | `TlsClient.Native.linux-ubuntu-amd64` |
| Linux            | ARM64                 | `TlsClient.Native.linux-arm64`        |
| Linux            | ARMv7                 | `TlsClient.Native.linux-armv7`        |
| Linux (Alpine)   | AMD64 (64-bit)        | `TlsClient.Native.linux-alpine-amd64` |
| macOS            | ARM64 (Apple Silicon) | `TlsClient.Native.darwin-arm64`       |
| macOS            | AMD64 (Intel)         | `TlsClient.Native.darwin-amd64`       |

👉 Use **NuGet** to add the correct native package for your target platform.

---

## 🔑 Initialization

Before creating any `TlsClient`, you **must** initialize the wrapper with the path to the native library (`tls-client`):

```csharp
using TlsClient.Core;

// initialize once at app startup
TlsClient.Initialize("{LIBRARY_PATH}");

// then create clients normally
var client = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("TlsClient.NET/1.0")
    .Build();
```

> ⚠️ Without `TlsClient.Initialize(path)` the library will not work.

---

## 🚀 Quick Start

### Example 1 – Core client

```csharp
using TlsClient.Core;

TlsClient.Initialize("{LIBRARY_PATH}");

var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("TlsClient.NET/1.0")
    .WithFollowRedirects()
    .WithTimeout(TimeSpan.FromSeconds(10))
    .Build();
```

### Example 2 – With HttpClient

```csharp
using TlsClient.Core;
using TlsClient.HttpClient;

TlsClient.Initialize("{LIBRARY_PATH}");

var handler = new TlsClientHandler(
    new TlsClientOptions(TlsClientIdentifier.Chrome132, "CustomUA/1.0")
);

using var http = new System.Net.Http.HttpClient(handler);
var html = await http.GetStringAsync("https://httpbin.org/get");
Console.WriteLine(html);
```

---

## 🧯 Support & Issues

* For **.NET wrapper issues** → open an issue here.
* For **native TLS client issues** → open an issue at [bogdanfinn/tls-client](https://github.com/bogdanfinn/tls-client).

---

## 📜 License

Licensed under the **MIT License**.
See [LICENSE](./LICENSE) for details.

---
## ©️ Copyright & Contact

**© 2025 TlsClient.NET**

Maintained by **Eren Kurt**

* 🐙 GitHub → [@ErenKrt](https://github.com/ErenKrt)
* 📷 Instagram → [@ep.eren](https://instagram.com/ep.eren)