using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TlsClient.Core;
using TlsClient.Core.Helpers;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;
using TlsClient.Core.Models.Responses;
using TlsClient.Native.Wrappers;

namespace TlsClient.Native
{
    public sealed class NativeTlsClient : BaseTlsClient
    {
        public NativeTlsClient(TlsClientOptions options) : base(options) { }
        public NativeTlsClient() : base() { }

        public static void Initialize(string? libraryPath) => TlsClientWrapper.Initialize(libraryPath);

        #region Sync Methods
        public override Response Request(Request request)
        {
            request= PrepareRequest(request);

            Response response;

            try
            {
                var payload = RequestHelpers.Prepare(request);
                var rawResponse = TlsClientWrapper.Request(payload);
                response = rawResponse.FromJson<Response>() ?? throw new Exception("Response is null, can't convert object from json.");
            }
            catch (Exception err)
            {
                response = new Response()
                {
                    Body = err.Message,
                    Status = 0,
                };
            }

            if (!string.IsNullOrEmpty(response.Id))
                TlsClientWrapper.FreeMemory(response.Id);

            if (response.Status == 0 && response.Body.Contains("Client.Timeout exceeded"))
            {
                response = new Response()
                {
                    Body = "Timeout",
                    Status = HttpStatusCode.RequestTimeout,
                };
            }

            return response;
        }
        public override GetCookiesFromSessionResponse GetCookies(string url)
        {
            var payload = PrepareGetCookies(url);
            var rawResponse = TlsClientWrapper.GetCookiesFromSession(RequestHelpers.Prepare(payload));
            
            return rawResponse.FromJson<GetCookiesFromSessionResponse>() ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override GetCookiesFromSessionResponse AddCookies(string url, List<TlsClientCookie> cookies)
        {
            var payload = PrepareAddCookies(url, cookies);
            var rawResponse = TlsClientWrapper.AddCookiesToSession(RequestHelpers.Prepare(payload));
            return rawResponse.FromJson<GetCookiesFromSessionResponse>() ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override DestroyResponse Destroy()
        {
            var payload = PrepareDestroy();
            var rawResponse = TlsClientWrapper.DestroySession(RequestHelpers.Prepare(payload));
            return rawResponse.FromJson<DestroyResponse>() ?? throw new Exception("Response is null, can't convert object from json.");
        }
        public override DestroyResponse DestroyAll()
        {
            var rawResponse = TlsClientWrapper.DestroyAll();
            return rawResponse.FromJson<DestroyResponse>() ?? throw new Exception("Response is null, can't convert object from json.");
        }
        #endregion

        #region Async Methods
        public override Task<Response> RequestAsync(Request request, CancellationToken ct = default) => AsyncHelpers.RunAsync(() => Request(request), ct);
        public override Task<GetCookiesFromSessionResponse> GetCookiesAsync(string url, CancellationToken ct = default) => AsyncHelpers.RunAsync(() => GetCookies(url), ct);
        public override Task<DestroyResponse> DestroyAsync(CancellationToken ct = default) => AsyncHelpers.RunAsync(() => Destroy(), ct);
        public override Task<DestroyResponse> DestroyAllAsync(CancellationToken ct = default) => AsyncHelpers.RunAsync(() => DestroyAll(), ct);
        public override Task<GetCookiesFromSessionResponse> AddCookiesAsync(string url, List<TlsClientCookie> cookies, CancellationToken ct = default) => AsyncHelpers.RunAsync(() => AddCookies(url, cookies), ct);
        #endregion

        #region Streaming
        /// <summary>
        /// Returns <c>true</c> when the loaded native library exposes the streaming exports.
        /// Older builds may not — in which case the streaming methods on this client throw
        /// on first use.
        /// </summary>
        public static bool IsStreamingSupported => TlsClientWrapper.IsStreamingSupported;

        /// <summary>
        /// Issues a request and returns once the response headers are available, without
        /// reading the body. Use the returned <see cref="StreamStartResponse.StreamId"/>
        /// to drive subsequent <see cref="ReadStream"/> / <see cref="ReadStreamAll"/> /
        /// <see cref="CancelStream"/> calls.
        /// </summary>
        /// <remarks>
        /// For server-sent events (Content-Type: text/event-stream) you must set
        /// <see cref="Request.TimeoutMilliseconds"/> to <c>0</c> (or a deliberately large value).
        /// The <see cref="Core.Models.Entities.TlsClientOptions.Timeout"/> bound applies to the
        /// whole request including body reads.
        /// </remarks>
        public StreamStartResponse RequestStream(Request request)
        {
            request = PrepareRequest(request);

            StreamStartResponse response;

            try
            {
                var payload = RequestHelpers.Prepare(request);
                var rawResponse = TlsClientWrapper.RequestStream(payload);
                response = rawResponse.FromJson<StreamStartResponse>() ?? throw new Exception("Response is null, can't convert object from json.");
            }
            catch (Exception err)
            {
                response = new StreamStartResponse
                {
                    Body = err.Message,
                    Status = 0,
                };
            }

            if (!string.IsNullOrEmpty(response.Id))
                TlsClientWrapper.FreeMemory(response.Id);

            return response;
        }

        /// <summary>
        /// Polls for the next chunk of an in-flight stream. Returns immediately on data,
        /// EOF, error, or after <paramref name="timeoutMs"/> when no chunk is available
        /// (heartbeat poll). Inspect <see cref="StreamChunkResponse.EOF"/>,
        /// <see cref="StreamChunkResponse.Timeout"/>, and <see cref="StreamChunkResponse.Error"/>
        /// to drive the read loop.
        /// </summary>
        /// <param name="timeoutMs">
        /// <c>&lt; 0</c>: block until the next chunk, EOF, or error.
        /// <c>= 0</c>: non-blocking poll.
        /// <c>&gt; 0</c>: block up to TimeoutMs, then return Timeout=true if no chunk arrived.
        /// </param>
        public StreamChunkResponse ReadStream(Guid streamId, int timeoutMs = 1000)
        {
            StreamChunkResponse response;

            try
            {
                var payload = RequestHelpers.Prepare(new ReadStreamRequest
                {
                    StreamId = streamId,
                    TimeoutMs = timeoutMs,
                });
                var rawResponse = TlsClientWrapper.ReadStream(payload);
                response = rawResponse.FromJson<StreamChunkResponse>() ?? throw new Exception("Response is null, can't convert object from json.");
            }
            catch (Exception err)
            {
                response = new StreamChunkResponse
                {
                    StreamId = streamId,
                    Error = err.Message,
                };
            }

            if (!string.IsNullOrEmpty(response.Id))
                TlsClientWrapper.FreeMemory(response.Id);

            return response;
        }

        /// <summary>
        /// Drains the rest of the stream's body in one call and returns it as a normal
        /// <see cref="Response"/> (charset-decoded for text bodies, base64+MIME prefix for
        /// byte responses). Use this when the response from <see cref="RequestStream"/>
        /// turns out to NOT be a streaming response (e.g. Content-Type isn't
        /// <c>text/event-stream</c>) and you just want the full body in one shot.
        /// After this returns, <paramref name="streamId"/> is invalid.
        /// </summary>
        public Response ReadStreamAll(Guid streamId)
        {
            Response response;

            try
            {
                var payload = RequestHelpers.Prepare(new ReadStreamAllRequest { StreamId = streamId });
                var rawResponse = TlsClientWrapper.ReadStreamAll(payload);
                response = rawResponse.FromJson<Response>() ?? throw new Exception("Response is null, can't convert object from json.");
            }
            catch (Exception err)
            {
                response = new Response
                {
                    Body = err.Message,
                    Status = 0,
                };
            }

            if (!string.IsNullOrEmpty(response.Id))
                TlsClientWrapper.FreeMemory(response.Id);

            return response;
        }

        /// <summary>
        /// Cancels an in-flight stream and releases the underlying connection. Idempotent —
        /// safe to call after a natural EOF, after an error, or with an unknown
        /// <paramref name="streamId"/>. Always call this in a finally block when consuming
        /// chunks via <see cref="ReadStream"/>.
        /// </summary>
        public DestroyResponse CancelStream(Guid streamId)
        {
            DestroyResponse response;

            try
            {
                var payload = RequestHelpers.Prepare(new CancelStreamRequest { StreamId = streamId });
                var rawResponse = TlsClientWrapper.CancelStream(payload);
                response = rawResponse.FromJson<DestroyResponse>() ?? throw new Exception("Response is null, can't convert object from json.");
            }
            catch (Exception err)
            {
                response = new DestroyResponse
                {
                    Body = err.Message,
                    Success = false,
                };
            }

            if (!string.IsNullOrEmpty(response.Id))
                TlsClientWrapper.FreeMemory(response.Id);

            return response;
        }

        public Task<StreamStartResponse> RequestStreamAsync(Request request, CancellationToken ct = default) => AsyncHelpers.RunAsync(() => RequestStream(request), ct);
        public Task<StreamChunkResponse> ReadStreamAsync(Guid streamId, int timeoutMs = 1000, CancellationToken ct = default) => AsyncHelpers.RunAsync(() => ReadStream(streamId, timeoutMs), ct);
        public Task<Response> ReadStreamAllAsync(Guid streamId, CancellationToken ct = default) => AsyncHelpers.RunAsync(() => ReadStreamAll(streamId), ct);
        public Task<DestroyResponse> CancelStreamAsync(Guid streamId, CancellationToken ct = default) => AsyncHelpers.RunAsync(() => CancelStream(streamId), ct);
        #endregion
    }
}
