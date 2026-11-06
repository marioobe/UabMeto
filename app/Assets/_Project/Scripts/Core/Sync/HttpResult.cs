using System;

namespace UabMeto.Core.Sync
{
    /// <summary>
    /// Hasil satu permintaan HTTP. <see cref="StatusCode"/> bernilai 0 bila
    /// transport gagal sebelum menerima respons.
    /// </summary>
    public readonly struct HttpResult
    {
        public HttpResult(int statusCode, string body, string error)
        {
            StatusCode = statusCode;
            Body = body ?? string.Empty;
            Error = error;
        }

        public int StatusCode { get; }

        public string Body { get; }

        /// <summary>Pesan kegagalan transport; null bila respons diterima.</summary>
        public string Error { get; }

        public bool HasResponse => StatusCode > 0;

        public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

        public static HttpResult Failure(string error)
        {
            return new HttpResult(0, string.Empty, error);
        }

        public static HttpResult FromResponse(int statusCode, string body)
        {
            return new HttpResult(statusCode, body, null);
        }
    }
}
