using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UabMeto.Core.Sync
{
    /// <summary>
    /// Abstraksi transport HTTP agar klien API bisa diuji tanpa jaringan.
    /// Implementasi Unity berada di assembly App.
    /// </summary>
    public interface IHttpTransport
    {
        Task<HttpResult> SendAsync(
            string method,
            string url,
            string jsonBody,
            IReadOnlyDictionary<string, string> headers,
            CancellationToken cancellationToken);
    }
}
