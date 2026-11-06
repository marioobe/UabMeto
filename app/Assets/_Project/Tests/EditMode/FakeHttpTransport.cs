using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UabMeto.Core.Sync;

namespace UabMeto.Tests.EditMode
{
    /// <summary>Transport palsu untuk menguji klien API tanpa jaringan.</summary>
    public sealed class FakeHttpTransport : IHttpTransport
    {
        public sealed class Call
        {
            public string Method;
            public string Url;
            public string Body;
            public IReadOnlyDictionary<string, string> Headers;
        }

        private readonly Queue<Func<Call, HttpResult>> _responses =
            new Queue<Func<Call, HttpResult>>();

        public List<Call> Calls { get; } = new List<Call>();

        public FakeHttpTransport Respond(int statusCode, string body)
        {
            _responses.Enqueue(_ => HttpResult.FromResponse(statusCode, body));
            return this;
        }

        public FakeHttpTransport Fail(string error)
        {
            _responses.Enqueue(_ => HttpResult.Failure(error));
            return this;
        }

        public Task<HttpResult> SendAsync(
            string method,
            string url,
            string jsonBody,
            IReadOnlyDictionary<string, string> headers,
            CancellationToken cancellationToken)
        {
            var call = new Call
            {
                Method = method,
                Url = url,
                Body = jsonBody,
                Headers = headers
            };
            Calls.Add(call);

            if (_responses.Count == 0)
            {
                throw new InvalidOperationException("Tidak ada respons palsu tersisa.");
            }

            return Task.FromResult(_responses.Dequeue()(call));
        }
    }
}
