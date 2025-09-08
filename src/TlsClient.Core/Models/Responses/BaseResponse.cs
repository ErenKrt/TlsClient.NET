using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Models.Responses
{
    public class BaseResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
