// See https://aka.ms/new-console-template for more information
using TlsClient.Core.Models.Entities;
using TlsClient.Api.Extensions;
using TlsClient.RestSharp.Helpers.Builders;
using RestSharp;
using TlsClient.Core.Builders;

Console.WriteLine("Hello, World!");

var baseUri = new Uri("http://127.0.0.1:8080/");

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