using System;
using System.Collections.Generic;
using System.Text;
using TlsClient.Core.Models.Entities;

namespace TlsClient.Core.Models.Requests
{
    public class AddCookiesToSessionRequest
    {
        public Guid SessionID { get; set; }
        public string Url { get; set; } = string.Empty;
        public List<TlsClientCookie> Cookies { get; set; } = new List<TlsClientCookie>();
    }
}
