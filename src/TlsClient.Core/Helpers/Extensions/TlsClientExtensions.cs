using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TlsClient.Core.Models.Requests;
using TlsClient.Core.Models.Responses;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Helpers.Wrappers;

namespace TlsClient.Core.Helpers.Extensions
{
    public static class TlsClientExtensions
    {

        public static Task<Response> RequestAsync( this TlsClient client, Request request, CancellationToken cancellationToken = default)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            return Task.Run(() => client.Request(request), cancellationToken);
        }

        public static Task<GetCookiesFromSessionResponse> GetCookiesAsync(this TlsClient client, string url, CancellationToken cancellationToken = default)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            return Task.Run(() => client.GetCookies(url), cancellationToken);
        }

        public static Task<GetCookiesFromSessionResponse> AddCookiesAsync(this TlsClient client, string url, List<TlsClientCookie> cookies, CancellationToken cancellationToken = default)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            return Task.Run(() => client.AddCookies(url, cookies), cancellationToken);
        }


        public static Task<DestroyResponse> DestroyAsync(this TlsClient client, CancellationToken cancellationToken = default)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            return Task.Run(() => client.Destroy(), cancellationToken);
        }

        public static Task<DestroyResponse> DestroyAllAsync(this TlsClient client, CancellationToken cancellationToken = default)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            return Task.Run(() => TlsClient.DestroyAll(), cancellationToken);
        }

        public static Task DisposeAsync(this TlsClient client, CancellationToken cancellationToken = default)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            return Task.Run(() => client.Dispose(), cancellationToken);
        }
    }
}
