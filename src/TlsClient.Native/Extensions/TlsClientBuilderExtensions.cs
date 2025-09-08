using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TlsClient.Core.Builders;
using TlsClient.Native.Wrappers;

namespace TlsClient.Native.Extensions
{
    public static class TlsClientBuilderExtensions
    {
        public static TlsClientBuilder WithNative(this TlsClientBuilder builder, string? libraryPath)
        {
            TlsClientWrapper.Initialize(libraryPath);
            return builder;
        }
        public static NativeTlsClient Build(this TlsClientBuilder builder) => new NativeTlsClient(builder._options);
    }
}
