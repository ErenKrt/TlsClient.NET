using System;

namespace TlsClient.Core.Models.Requests
{
    // Reference: https://github.com/bogdanfinn/tls-client/blob/master/cffi_src/types.go (CancelStreamInput)
    public class CancelStreamRequest
    {
        public Guid StreamId { get; set; }
    }
}
