﻿// See https://aka.ms/new-console-template for more information
using RestSharp;
using System.Runtime.InteropServices;
using TlsClient.Core;
using TlsClient.Core.Helpers.Builders;
using TlsClient.Core.Models;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.HttpClient;
using TlsClient.RestSharp.Helpers.Builders;



string platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" :
                  RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
                  RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "darwin" :
                  throw new PlatformNotSupportedException("Unsupported OS platform");

string extension = platform switch
{
    "win" => "dll",
    "linux" => "so",
    "darwin" => "dylib",
    _ => throw new PlatformNotSupportedException("Unsupported OS platform")
};


Console.WriteLine($"Platform: {platform}");
Console.WriteLine($"Extension: {extension}");
Console.WriteLine($"Architecture: {RuntimeInformation.ProcessArchitecture}");

return;
var tlsClient31= new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("TestClient 1.0")
    .WithFollowRedirects(false)
    .Build();

var myRestClient = new TlsRestClientBuilder()
    .WithTlsClient(tlsClient31)
    .WithBaseUrl("https://httpbin.org")
    .Build();

var restReq21 = new RestRequest("/get", Method.Get);
var restResponse21 = await myRestClient.ExecuteAsync(restReq21);


return;
var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithProxyUrl("http://127.0.0.1:8080")
    .WithSkipTlsVerification(true)
    .WithFollowRedirects(true)
    .Build();

var tlsHandler = new TlsClientHandler(tlsClient);
RestClient restClient = new(tlsHandler, configureRestClient: (x) =>
{
    x.BaseUrl = new Uri("https://example.org");
    x.UserAgent = tlsClient.Options.UserAgent;
    x.Timeout= tlsClient.Options.Timeout;
    x.FollowRedirects= tlsClient.Options.FollowRedirects;
    x.CookieContainer = tlsHandler.CookieContainer;
    x.CookieContainer.Add(new Uri("https://example.org"), new System.Net.Cookie("hello", "eren"));
});

var restReq= new RestRequest("https://httpbin.org/cookies/set?MyCookie=MyValue", Method.Get);
var restResponse = await restClient.ExecuteAsync(restReq);

var restReq2 = new RestRequest("https://httpbin.org/cookies", Method.Get);
var restResponse2 = await restClient.ExecuteAsync(restReq2);

/*
var postReq = new RestRequest("https://httpbin.org/post", Method.Post);
postReq.AddParameter("eren", "30", ParameterType.GetOrPost);
postReq.AddParameter("erenn", "52", ParameterType.GetOrPost);
postReq.AddFile("myFile", "C:\\Users\\eren\\Desktop\\fotolar\\Untitled-1.png");
postReq.AddFile("yourFile", "C:\\Users\\eren\\Desktop\\fotolar\\Untitled-1.png");
//postReq.AddJsonBody(new { a = "b" });
var postResponse = await restClient.ExecuteAsync(postReq);
*/
return;
var clientOptions = new TlsClientOptions(TlsClientIdentifier.Chrome132, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0")
{
    ProxyURL = "",
    FollowRedirects = true,
};
var client = new TlsClient.Core.TlsClient(clientOptions);
var clientWithoutOptions= new TlsClient.Core.TlsClient();
var cc= await client.RequestAsync(new Request()
{
    RequestUrl= "https://google.com",
    RequestMethod= HttpMethod.Get,
});

var client4 = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("g0")
    .WithFollowRedirects(true)
    .WithSkipTlsVerification(true)
    .AddHeader("a", "b")
    .Build();

var request = new RequestBuilder()
    .WithUrl("https://httpbin.org/user-agent")
    .WithMethod(HttpMethod.Get)
    //.WithByteResponse()
    .Build();

var zz = await client4.RequestAsync(request);

var hh = await client.RequestAsync(request);

 var ll= await client.GetCookiesAsync("https://google.com");
await client.AddCookiesAsync("https://google.com", new List<TlsClientCookie>()
{
    new TlsClientCookie("a","b")
});

//await client.DestroyAsync();
//await client.DestroyAllAsync();
client.Dispose();

ll = await client.GetCookiesAsync("https://google.com");

var gg = "a";

using(var client2 = new TlsClient.Core.TlsClient())
{
    var cc2 = await client2.RequestAsync(new Request()
    {
        RequestUrl = "https://google.comr",
        RequestMethod = HttpMethod.Get,
        WithDebug = true,
    });
    var gg2 = "a";
}