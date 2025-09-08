# TlsClient.Net

A **.NET wrapper** around [bogdanfinn/tls-client](https://github.com/bogdanfinn/tls-client).
Brings advanced TLS fingerprinting, browser emulation, User-Agent control, header ordering, cookie handling, certificate validation toggles, and proxy support to .NET.

---

## 📦 Packages

* **Native**

  * **[`TlsClient.Native`](./src/TlsClient.Native/README.md)** — calls the native `tls-client` library directly (no remote service).
* **Remote Service**

  * **[`TlsClient.Api`](./src/TlsClient.Api/README.md)** — talks to a running `tls-client` HTTP service.

> ℹ️ The shared models and builders commonly referred to as “Core” (e.g., `Request`, `Response`, `TlsClientOptions`, `TlsClientIdentifier`, helpers) are **included within each package**. You do **not** install a separate `TlsClient.Core` package.

---

## 🧭 Which one should I use?

* **Use `TlsClient.Native`** if you want everything **in-process** and can ship the native library with your app.
  **Note:** Due to C# ↔ Go native interop, some environments may experience instability or edge-case issues (GC/pinning, P/Invoke marshalling, native memory management). If you hit odd crashes or hangs, prefer the API mode below.
* **Use `TlsClient.Api`** if you want a **separate service** (local Docker/remote host) and a thin .NET client in your app. This avoids Go interop inside your process and is generally more robust operationally.

Both packages expose the same request/response shapes.

---

## ⚙️ Installation

### Option A — Native (in-process)

1. Add the package:

```bash
dotnet add package TlsClient.Native
```

2. Add the **runtime-specific native binary** for your target platform:

| OS             | Arch  | NuGet package                         |
| -------------- | ----- | ------------------------------------- |
| Windows        | x64   | `TlsClient.Native.win-x64`            |
| Windows        | x86   | `TlsClient.Native.win-x32`            |
| Linux (Ubuntu) | amd64 | `TlsClient.Native.linux-ubuntu-amd64` |
| Linux          | arm64 | `TlsClient.Native.linux-arm64`        |
| Linux          | armv7 | `TlsClient.Native.linux-armv7`        |
| Linux (Alpine) | amd64 | `TlsClient.Native.linux-alpine-amd64` |
| macOS (Apple)  | arm64 | `TlsClient.Native.darwin-arm64`       |
| macOS (Intel)  | amd64 | `TlsClient.Native.darwin-amd64`       |

3. Initialize once at startup:

```csharp
using TlsClient.Native;

NativeTlsClient.Initialize("{PATH_TO_NATIVE_LIBRARY}");
// e.g. "C:\\tools\\tls-client\\tls-client-windows-64-1.11.0.dll"
```

> ⚠️ Without `NativeTlsClient.Initialize(...)`, native mode won’t work.

**Interop note (important):** C# calling into the Go native library can be less stable on some systems (especially under heavy concurrency). If you encounter flakiness (random SIG… signals, heap issues, or unexplained timeouts), switch to `TlsClient.Api`.

---

### Option B — API (service)

If you run a `tls-client` HTTP service (local/remote):

```bash
dotnet add package TlsClient.Api
```

No native init required; just point to the service URL and provide an API key.

---

## 🚀 Quick Start

### Native (in-process)

```csharp
using TlsClient.Native;
using TlsClient.Core.Models.Requests; // bundled inside package

NativeTlsClient.Initialize("{PATH_TO_NATIVE_LIBRARY}");

using var client = new NativeTlsClient();
var res = client.Request(new Request { RequestUrl = "https://httpbin.io/get" });
Console.WriteLine(res.Status);
```

### API (service)

```csharp
using TlsClient.Api;
using TlsClient.Core.Models.Requests; // bundled inside package

using var client = new ApiTlsClient(new Uri("http://127.0.0.1:8080"), "my-auth-key-1");
var res = client.Request(new Request { RequestUrl = "https://httpbin.io/get" });
Console.WriteLine(res.Status);
```

> For additional scenarios, please refer to the test projects in each package.

---

## 🧯 Support & Issues

* Wrapper/packaging issues → open an issue **here**.
* Native TLS engine issues → [bogdanfinn/tls-client](https://github.com/bogdanfinn/tls-client).

---

## 🔬 Learn More

This focuses on setup.
For **additional scenarios**, please refer to the test projects.

---

## 📜 License

Licensed under the **MIT License**.
See [LICENSE](./LICENSE) for details.

---

## ©️ Copyright

**© 2025 TlsClient.NET** — Maintained by **Eren Kurt**

* GitHub: [@ErenKrt](https://github.com/ErenKrt)
* Instagram: [@ep.eren](https://instagram.com/ep.eren)
