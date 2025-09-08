using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Models.Entities
{
    public class TlsClientCookie
    {
        public long? Expires { get; set; } = 0;
        public string? Domain { get; set; } = null;
        public string? Name { get; set; } = null;
        public string? Path { get; set; } = null;
        public string? Value { get; set; } = null;
        public long? MaxAge { get; set; } = null;

        public TlsClientCookie() { }
        public TlsClientCookie(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public TlsClientCookie(string name, string value, string domain) : this(name, value)
        {
            Domain = domain;
        }
    }
}
