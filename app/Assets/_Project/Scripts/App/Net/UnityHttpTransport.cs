using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UabMeto.Core.Sync;
using UnityEngine.Networking;

namespace UabMeto.App.Net
{
    /// <summary>
    /// Transport HTTP memakai UnityWebRequest. Harus dipanggil dari main thread.
    /// Batas waktu mencegah permintaan menggantung saat server lokal mati.
    /// </summary>
    public sealed class UnityHttpTransport : IHttpTransport
    {
        private readonly int _timeoutSeconds;

        public UnityHttpTransport(int timeoutSeconds = 10)
        {
            _timeoutSeconds = Math.Max(1, timeoutSeconds);
        }

        public async Task<HttpResult> SendAsync(
            string method,
            string url,
            string jsonBody,
            IReadOnlyDictionary<string, string> headers,
            CancellationToken cancellationToken)
        {
            using (var request = new UnityWebRequest(url, method))
            {
                if (jsonBody != null)
                {
                    request.uploadHandler = new UploadHandlerRaw(
                        Encoding.UTF8.GetBytes(jsonBody));
                }

                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = _timeoutSeconds;
                request.SetRequestHeader("Content-Type", "application/json");

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        if (string.IsNullOrEmpty(header.Key))
                        {
                            continue;
                        }

                        request.SetRequestHeader(header.Key, header.Value ?? string.Empty);
                    }
                }

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        request.Abort();
                        throw new OperationCanceledException(cancellationToken);
                    }

                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.responseCode == 0)
                {
                    return HttpResult.Failure(request.error);
                }

                var body = request.downloadHandler == null
                    ? string.Empty
                    : request.downloadHandler.text;

                return HttpResult.FromResponse((int)request.responseCode, body);
            }
        }
    }
}
