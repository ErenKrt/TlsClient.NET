# TlsClient.HttpClient

Integration of [TlsClient.NET](../README.md) with the built-in [.NET HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient).
This package provides a `TlsClientHandler` that plugs directly into `HttpClient`, enabling advanced TLS fingerprinting and browser emulation in any .NET application.

---

## 📦 Installation

Install from NuGet:

```bash
dotnet add package TlsClient.HttpClient
````

## 🚀 Usage

> 💡 `TlsClientHandler` is a standard `HttpMessageHandler`.
> That means **you can continue using all features of HttpClient exactly the same way as before.**

### Basic Example

```csharp
using System;
using System.Net.Http;
using TlsClient.Core;
using TlsClient.HttpClient;

// initialize Wrapper
TlsClient.Initialize("{LIBRARY_PATH}");

// create a TlsClient instance
var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("TestClient 1.0")
    .Build();

// create a handler using the TlsClient
var handler = new TlsClientHandler(tlsClient);

// create a standard HttpClient with the TlsClient handler
var httpClient = new HttpClient(handler);

// make request
var response = await httpClient.GetAsync("https://httpbin.org/get");
var content = await response.Content.ReadAsStringAsync();

Console.WriteLine(content);
```

## 📜 License

This project is licensed under the MIT License.
See the [LICENSE](../../LICENSE) file for details.