using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TlsClient.Core.Models.Responses
{
    // Reference: https://github.com/bogdanfinn/tls-client/blob/master/cffi_src/types.go#L188
    public class Response : BaseResponse
    {
        public Dictionary<string, string>? Cookies { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, List<string>>? Headers { get; set; } = new Dictionary<string, List<string>>();
        public string? SessionId { get; set; }
        public string Target { get; set; } = string.Empty;
        public string UsedProtocol { get; set; } = string.Empty;
        public HttpStatusCode Status { get; set; }
        public bool IsSuccessStatus => HttpStatusCode.OK == Status || (int)Status >= 200 && (int)Status < 300;
    }
}
