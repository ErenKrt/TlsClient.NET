using System;

namespace TlsClient.Core.Models.Responses
{
    // Reference: https://github.com/bogdanfinn/tls-client/blob/master/cffi_src/types.go (StreamStartResponse)
    /// <summary>
    /// Returned by <c>requestStream</c>. Carries the same fields as <see cref="Response"/>
    /// (status, headers, cookies, target, used protocol) plus a <see cref="StreamId"/>
    /// that identifies the open stream for subsequent ReadStream / ReadStreamAll /
    /// CancelStream calls. <see cref="Response.Body"/> is always empty here.
    /// </summary>
    public class StreamStartResponse : Response
    {
        public Guid StreamId { get; set; }
    }
}
