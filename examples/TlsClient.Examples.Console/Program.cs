// See https://aka.ms/new-console-template for more information
using RestSharp;
using System.Runtime.InteropServices;
using TlsClient.Core;
using TlsClient.Core.Helpers.Builders;
using TlsClient.Core.Helpers.Wrappers;
using TlsClient.Core.Models;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.HttpClient;
using TlsClient.RestSharp.Helpers.Builders;

DotNetEnv.Env.Load();


TlsClientWrapper.Initialize(DotNetEnv.Env.GetString("TLS_CLIENT_PATH"));
/*
var tasks = new List<Task<RestResponse>>();
int threadCount = 1000;

var tlsClient = new TlsClientBuilder()
            .WithIdentifier(TlsClientIdentifier.Chrome133)
            .WithUserAgent("Mozilla/5.0 AppleWebKit/537.36 (KHTML, like Gecko; compatible; Googlebot/2.1; +http://www.google.com/bot.html) Chrome/135.0.0.0 Safari/537.36")
            .WithFollowRedirects(false)
            .WithTimeout(TimeSpan.FromSeconds(15))
            .WithHeader("Host", "gamermarkt.com")
            //.WithProxyUrl("http://127.0.0.1:8080")
            .WithInsecureSkipVerify(true)
            .WithoutCookieJar(true)
            .WithDebug(true)
            .Build();

var ggh = "a";

var RestClient = new TlsRestClientBuilder()
    .WithBaseUrl("https://57.129.18.169")
    .WithTlsClient(tlsClient)
    .Build();

RestClient.AddDefaultHeader("Host", "www.gamermarkt.com");

var restReq = new RestRequest("/listing/valorant-account/eu-server-ascendant-22-agents-52-skins-568944-1007343", Method.Get);
var test1= await RestClient.ExecuteAsync(restReq);

TlsClientWrapper.Destroy();

var test2 = await RestClient.ExecuteAsync(restReq);

var gg = "a";
*/

/* Cookie header test */
var tlsClient = new TlsClientBuilder()
            .WithIdentifier(TlsClientIdentifier.Chrome133)
            .WithUserAgent("Mozilla/5.0 AppleWebKit/537.36 (KHTML, like Gecko; compatible; Googlebot/2.1; +http://www.google.com/bot.html) Chrome/135.0.0.0 Safari/537.36")
            .WithFollowRedirects(false)
            .WithTimeout(TimeSpan.FromSeconds(15))
            .WithInsecureSkipVerify(true)
            .WithoutCookieJar(true)
            .WithHeader("Cookie", "test0=value0")
            .WithProxyUrl("http://127.0.0.1:8080")
            .WithDebug(true)
            .Build();

var response = tlsClient.Request(new Request()
{
    RequestUrl= "https://httpbin.org/cookies",
    Headers = new Dictionary<string, string>()
    {
        { "Cookie", "test1=value1;" },
    },
    RequestCookies= new List<TlsClientCookie>()
    {
        new TlsClientCookie("test5", "value5"),
        new TlsClientCookie("test6", "value6"),
    },
});

var gg = "a";
/*
var RestClient = new TlsRestClientBuilder()
    .WithBaseUrl("https://httpbin.io/")
    .WithTlsClient(tlsClient)
    .Build();

var restReq = new RestRequest("/cookies", Method.Get);
restReq.AddHeader("Cookie", "test1=value1; test2=value2;");
restReq.AddHeader("Cookie", "test3=value1; test4=value2;");
var test1 = await RestClient.ExecuteAsync(restReq);
var gg = "a";
*/