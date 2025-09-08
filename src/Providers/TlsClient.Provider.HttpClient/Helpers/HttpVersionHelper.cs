using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TlsClient.HttpClient.Helpers
{
    public static class HttpVersionHelper
    {
        public static Version Map(string? usedProtocol)
        {
            if (string.IsNullOrWhiteSpace(usedProtocol))
                return HttpVersion.Unknown;

            return usedProtocol switch
            {
                "HTTP/1.0" => HttpVersion.Version10,
                "HTTP/1.1" => HttpVersion.Version11,
                "HTTP/2.0" => HttpVersion.Version20,
                _ => HttpVersion.Unknown
            };
        }
    }
}
