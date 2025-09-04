using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Models.Requests
{
    public class GetCookiesFromSessionRequest
    {
        public Guid SessionID { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}
