# TlsClient.RestSharp

Integration of [TlsClient.NET](../README.md) with [RestSharp](https://restsharp.dev/).
This package allows you to make HTTP requests using RestSharp while benefiting from advanced TLS fingerprinting and browser emulation features provided by `TlsClient.NET`.

## 📦 Installation

Install from NuGet:

```bash
dotnet add package TlsClient.Provider.RestSharp
````

## 🚀 Usage

### Basic Example

>💡 `TlsRestClientBuilder` returns a standard `RestClient` instance.
> That means **you can continue using all features of RestSharp exactly the same way as before.**


```csharp
using RestSharp;
using TlsClient.Core;
using TlsClient.RestSharp.Helpers.Builders;

// initialize Wrapper
TlsClient.Initialize("{LIBRARY_PATH}");

// initialize TlsClient with desired options
var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome133)
    .WithUserAgent("TlsClient.NET 1.0")
    .Build();

// build RestClient with TlsRestClientBuilder
var restClient = new TlsRestClientBuilder()
    .WithTlsClient(tlsClient)
    .WithBaseUrl("https://httpbin.org")
    .Build();

// make request
var request = new RestRequest("/get", Method.Get);
var response = await restClient.ExecuteAsync(request);

Console.WriteLine(response.Content);
```

## 📜 License

This project is licensed under the MIT License.
See the [LICENSE](../../LICENSE) file for details.