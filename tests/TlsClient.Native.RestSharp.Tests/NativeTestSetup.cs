using System;

namespace TlsClient.RestSharp.Tests
{
    /// <summary>
    /// Resolves the path to the native tls-client library used by the Native
    /// RestSharp test fixtures. Reads the <c>TLS_CLIENT_NATIVE_DLL</c>
    /// environment variable when set; otherwise falls back to a developer-machine
    /// default that the upstream maintainer uses locally. CI / contributors
    /// should set the env var to point at the DLL produced for their platform.
    /// </summary>
    internal static class NativeTestSetup
    {
        public const string EnvVar = "TLS_CLIENT_NATIVE_DLL";
        public const string DefaultDllPath = "D:\\Tools\\tls-client-windows-64-1.13.1.dll";

        public static string DllPath =>
            Environment.GetEnvironmentVariable(EnvVar) ?? DefaultDllPath;
    }
}
