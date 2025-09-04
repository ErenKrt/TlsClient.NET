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
var tlsClient = new TlsClient.Core.TlsClient();

Console.WriteLine("Hello, Friend !");