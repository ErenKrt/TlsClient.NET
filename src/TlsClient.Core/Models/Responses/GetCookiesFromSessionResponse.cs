using System;
using System.Collections.Generic;
using System.Text;
using TlsClient.Core.Models.Entities;

namespace TlsClient.Core.Models.Responses
{
    public class GetCookiesFromSessionResponse : BaseResponse
    {
        public List<TlsClientCookie> Cookies { get; set; } = new List<TlsClientCookie>();
        
    }
}
