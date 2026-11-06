using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UabMeto.App.Local;

namespace UabMeto.App.Net
{
    /// <summary>Ringkasan satu putaran sinkronisasi.</summary>
    public sealed class SyncReport
    {
        public int Confirmed { get; set; }

        public int Rejected { get; set; }

        public int Failed { get; set; }

        public int StillPending { get; set; }

        /// <summary>Transport gagal; putaran dihentikan sampai koneksi pulih.</summary>
        public bool Offline { get; set; }

        /// <summary>Identitas pengembangan ditolak server.</summary>
        public bool Unauthorized { get; set; }

        public bool Cancelled { get; set; }

        public string LastError { get; set; }

        public bool HasWork =>
            Confirmed > 0 || Rejected > 0 || Failed > 0 || StillPending > 0;
    }

    /// <summary>
    /// Mengirim isi outbox ke Worker lokal dan memperbarui statusnya.
    ///
    /// XP yang disimpan adalah <c>awardedXp</c> dari server, bukan
    /// <c>clientClaimedXp</c>. Operasi yang ditolak permanen tidak dikirim
    /// ulang; kegagalan sementara menambah hitungan percobaan.
    /// </summary>
    public sealed class SyncService
    {
        private readonly ApiClient _api;
        private readonly OutboxStore _outbox;
        private readonly Func<string> _nicknameProvider;

        public SyncService(ApiClient api, OutboxStore outbox, Func<string> nicknameProvider)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _outbox = outbox ?? throw new ArgumentNullException(nameof(outbox));
            _nicknameProvider = nicknameProvider ?? (() => string.Empty);
        }

        public ApiClient Api => _api;

        public async Task<SyncReport> SyncPendingAsync(
            CancellationToken cancellationToken = default)
        {
            var report = new SyncReport();

            foreach (var item in _outbox.GetSyncable().ToList())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    report.Cancelled = true;
                    break;
                }

                var request = new SyncRequest
                {
                    operationId = item.operationId,
                    lessonId = item.lessonId,
                    contentVersion = item.contentVersion,
                    clientClaimedXp = item.clientClaimedXp,
                    nickname = _nicknameProvider() ?? string.Empty
                };

                try
                {
                    var response = await _api.SyncAsync(request, cancellationToken);
                    _outbox.MarkConfirmed(item.operationId, response.awardedXp);
                    report.Confirmed += 1;
                }
                catch (ApiException exception)
                {
                    report.LastError = exception.Message;
                    if (!ApplyFailure(report, item.operationId, exception))
                    {
                        return report;
                    }
                }
                catch (OperationCanceledException)
                {
                    report.Cancelled = true;
                    break;
                }
            }

            return report;
        }

        /// <summary>
        /// Menerapkan satu kegagalan ke outbox. Mengembalikan false bila
        /// putaran harus berhenti (offline atau identitas ditolak).
        /// </summary>
        private bool ApplyFailure(SyncReport report, string operationId, ApiException exception)
        {
            switch (exception.Code)
            {
                case "unauthorized":
                    report.Unauthorized = true;
                    return false;

                case "network_error":
                    report.Offline = true;
                    if (_outbox.RecordAttempt(operationId, exception.Message))
                    {
                        report.StillPending += 1;
                    }
                    else
                    {
                        report.Failed += 1;
                    }

                    return false;

                case "operation_conflict":
                    // operationId sama dengan payload berbeda: jangan menimpa.
                    _outbox.MarkFailed(operationId, exception.Message);
                    report.Failed += 1;
                    return true;

                case "content_not_validated":
                case "unknown_content":
                case "invalid_body":
                case "invalid_json":
                case "invalid_operation_id":
                case "invalid_lesson_id":
                case "invalid_content_version":
                case "invalid_client_claimed_xp":
                    _outbox.MarkRejected(operationId, exception.Message);
                    report.Rejected += 1;
                    return true;

                default:
                    if (exception.StatusCode >= 500)
                    {
                        if (_outbox.RecordAttempt(operationId, exception.Message))
                        {
                            report.StillPending += 1;
                        }
                        else
                        {
                            report.Failed += 1;
                        }
                    }
                    else
                    {
                        _outbox.MarkFailed(operationId, exception.Message);
                        report.Failed += 1;
                    }

                    return true;
            }
        }
    }
}
