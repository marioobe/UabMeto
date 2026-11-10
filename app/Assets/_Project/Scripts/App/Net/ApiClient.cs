using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UabMeto.Core.Sync;
using UnityEngine;

namespace UabMeto.App.Net
{
    /// <summary>Kesalahan dari klien API, termasuk kode error server.</summary>
    public sealed class ApiException : Exception
    {
        public ApiException(int statusCode, string code, string message)
            : base(message)
        {
            StatusCode = statusCode;
            Code = code ?? "unknown_error";
        }

        /// <summary>Kode status HTTP; 0 untuk kegagalan transport.</summary>
        public int StatusCode { get; }

        /// <summary>Nilai field <c>error</c> dari server.</summary>
        public string Code { get; }
    }

    [Serializable]
    public sealed class SyncRequest
    {
        public string operationId;
        public string lessonId;
        public string contentVersion;
        public int clientClaimedXp;
        public string nickname;
    }

    [Serializable]
    public sealed class SyncResponse
    {
        public string state;
        public int awardedXp;
        public bool duplicate;
        public bool devMode;
        public string error;
        public string message;
    }

    [Serializable]
    public sealed class SyncStatusResponse
    {
        public string operationId;
        public string lessonId;
        public string state;
        public int awardedXp;
        public string updatedAt;
    }

    [Serializable]
    public sealed class QuotaCounter
    {
        public int used;
        public int limit;
    }

    [Serializable]
    public sealed class QuotaResponse
    {
        public string dayWita;
        public QuotaCounter tutor;
        public QuotaCounter conversation;
        public QuotaCounter pronunciation;
    }

    [Serializable]
    public sealed class TutorMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    public sealed class TutorAskRequest
    {
        public string question;
        public TutorMessage[] history;
    }

    [Serializable]
    public sealed class TutorSource
    {
        public string lessonId;
        public string title;
        public string sourceRef;
    }

    [Serializable]
    public sealed class TutorAskResponse
    {
        public string answer;
        public TutorSource[] sources;
        public QuotaCounter quota;
        public bool refused;
        public string model;
        public string error;
        public string message;
    }

    [Serializable]
    public sealed class LeaderboardEntry
    {
        public string userId;
        public string nickname;
        public int xp;
    }

    [Serializable]
    public sealed class LeaderboardResponse
    {
        public string weekKey;
        public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    }

    [Serializable]
    internal sealed class ApiErrorResponse
    {
        public string error;
        public string message;
    }

    /// <summary>
    /// Klien HTTP untuk Worker mode online lokal.
    /// Tidak menyimpan token; identitas pengembangan dikirim lewat X-Dev-User.
    /// </summary>
    public sealed class ApiClient
    {
        private readonly IHttpTransport _transport;
        private readonly string _baseUrl;
        private readonly string _devUser;

        public ApiClient(IHttpTransport transport, string baseUrl, string devUser)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _baseUrl = NormalizeBaseUrl(baseUrl);
            _devUser = NormalizeDevUser(devUser);
        }

        public string BaseUrl => _baseUrl;

        public string DevUser => _devUser;

        public Task<SyncResponse> SyncAsync(
            SyncRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendAsync<SyncResponse>("POST", "/v1/sync", request, cancellationToken);
        }

        public Task<SyncStatusResponse> GetOperationAsync(
            string operationId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(operationId))
            {
                throw new ArgumentException("operationId wajib diisi.", nameof(operationId));
            }

            return SendAsync<SyncStatusResponse>(
                "GET",
                "/v1/sync/" + Uri.EscapeDataString(operationId.Trim()),
                null,
                cancellationToken);
        }

        public Task<QuotaResponse> GetQuotaAsync(CancellationToken cancellationToken = default)
        {
            return SendAsync<QuotaResponse>("GET", "/v1/quota", null, cancellationToken);
        }

        public Task<LeaderboardResponse> GetLeaderboardAsync(
            string weekKey = null,
            CancellationToken cancellationToken = default)
        {
            var path = "/v1/leaderboard";
            if (!string.IsNullOrWhiteSpace(weekKey))
            {
                path += "?week=" + Uri.EscapeDataString(weekKey.Trim());
            }

            return SendAsync<LeaderboardResponse>("GET", path, null, cancellationToken);
        }

        /// <summary>
        /// Mengirim satu pertanyaan ke Tutor AI. Riwayat hanya dipakai sebagai
        /// konteks percakapan; server tetap membatasi jawaban pada korpus
        /// tervalidasi dan kuota harian.
        /// </summary>
        public Task<TutorAskResponse> AskTutorAsync(
            string question,
            IReadOnlyList<TutorMessage> history = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Pertanyaan wajib diisi.", nameof(question));
            }

            var messages = new List<TutorMessage>();
            if (history != null)
            {
                for (var index = 0; index < history.Count; index++)
                {
                    var message = history[index];
                    if (message != null && !string.IsNullOrWhiteSpace(message.content))
                    {
                        messages.Add(message);
                    }
                }
            }

            var request = new TutorAskRequest
            {
                question = question.Trim(),
                history = messages.ToArray()
            };

            return SendAsync<TutorAskResponse>(
                "POST",
                "/v1/tutor/ask",
                request,
                cancellationToken);
        }

        private async Task<T> SendAsync<T>(
            string method,
            string path,
            object body,
            CancellationToken cancellationToken)
            where T : class
        {
            var json = body == null ? null : JsonUtility.ToJson(body);
            var result = await _transport.SendAsync(
                method,
                _baseUrl + path,
                json,
                BuildHeaders(),
                cancellationToken);

            if (!result.HasResponse)
            {
                throw new ApiException(
                    0,
                    "network_error",
                    string.IsNullOrWhiteSpace(result.Error)
                        ? "Tidak dapat menghubungi server."
                        : result.Error);
            }

            if (!result.IsSuccess)
            {
                var error = TryParse<ApiErrorResponse>(result.Body);
                throw new ApiException(
                    result.StatusCode,
                    error != null && !string.IsNullOrWhiteSpace(error.error)
                        ? error.error
                        : "http_error",
                    error != null && !string.IsNullOrWhiteSpace(error.message)
                        ? error.message
                        : result.Body);
            }

            var parsed = TryParse<T>(result.Body);
            if (parsed == null)
            {
                throw new ApiException(
                    result.StatusCode,
                    "invalid_response",
                    "Respons server tidak dapat dibaca.");
            }

            return parsed;
        }

        private Dictionary<string, string> BuildHeaders()
        {
            return new Dictionary<string, string>
            {
                { "X-Dev-User", _devUser }
            };
        }

        private static T TryParse<T>(string json)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string NormalizeBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("baseUrl wajib diisi.", nameof(baseUrl));
            }

            return baseUrl.Trim().TrimEnd('/');
        }

        private static string NormalizeDevUser(string devUser)
        {
            var trimmed = (devUser ?? string.Empty).Trim();
            if (trimmed.Length < 3 || trimmed.Length > 64)
            {
                throw new ArgumentException(
                    "X-Dev-User harus 3-64 karakter.",
                    nameof(devUser));
            }

            foreach (var character in trimmed)
            {
                var allowed =
                    (character >= 'A' && character <= 'Z') ||
                    (character >= 'a' && character <= 'z') ||
                    (character >= '0' && character <= '9') ||
                    character == '.' ||
                    character == '_' ||
                    character == '-';

                if (!allowed)
                {
                    throw new ArgumentException(
                        "X-Dev-User hanya boleh memuat A-Z a-z 0-9 . _ -",
                        nameof(devUser));
                }
            }

            return trimmed;
        }
    }
}
