// See https://aka.ms/new-console-template for more information
using TlsClient.Core.Models.Entities;
using TlsClient.Api.Extensions;
using TlsClient.RestSharp.Helpers.Builders;
using RestSharp;
using TlsClient.Core.Builders;

Console.WriteLine("Hello, World!");

var baseUri = new Uri("http://127.0.0.1:8080/");

/*
var baseUri = new Uri("http://127.0.0.1:8080/");
var client = new ApiTlsClient(new ApiTlsClientOptions(TlsClientIdentifier.Chrome133, "TestClient", baseUri, "my-auth-key-1"));
var response = client.Request(new Request()
{
    RequestUrl = "https://httpbin.io/cookies",
    RequestMethod = HttpMethod.Get,
});

var cookies = client.GetCookies("https://httpbin.io/cookies");

var addCookies = client.AddCookies("https://httpbin.io/cookies", new List<TlsClientCookie>() {
    new TlsClientCookie("analinyo","firarinyo", "httpbin.io")
});

var response2 = client.Request(new Request()
{
    RequestUrl = "https://httpbin.io/cookies",
    RequestMethod = HttpMethod.Get,
});
var cookies2 = client.GetCookies("https://httpbin.io/cookies");

var bb= client.DestroyAll();

var ll = "a";
*/

var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome133)
    .WithUserAgent("TlsClient-Example")
    .WithProxyUrl("http://127.0.0.1:8086")
    .WithInsecureSkipVerify(true)
    .WithApi(baseUri, "my-auth-key-1")
    .Build();

var restClient= new TlsRestClientBuilder()
    .WithTlsClient(tlsClient)
    .WithBaseUrl("https://httpbin.io")
    .Build();

var request = new RestRequest("/cookies/set?name=31", Method.Get);
var response = restClient.Execute(request);

var ll = "a";