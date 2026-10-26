using System;

namespace UabMeto.Core.Sync
{
    /// <summary>
    /// Menentukan langkah aman untuk operasi sinkronisasi.
    ///
    /// Unique constraint pada D1 <b>tidak</b> menjamin exactly-once, karena
    /// penulisan D1 dan Firestore berada di dua sistem berbeda. Karena itu
    /// keputusan di sini selalu memeriksa apakah efek Firestore sudah ada
    /// sebelum menerapkan ulang.
    /// </summary>
    public static class SyncReconciler
    {
        public static SyncAction Decide(SyncOperationRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (!record.PayloadValid || record.State == SyncOperationState.Rejected)
            {
                return SyncAction.Reject;
            }

            if (record.State == SyncOperationState.Failed)
            {
                return SyncAction.HaltForReconciliation;
            }

            if (record.FirestoreHasEffect && !record.PayloadMatchesAppliedEffect)
            {
                // operationId sama tetapi isi berbeda: jangan menimpa data.
                return SyncAction.HaltForReconciliation;
            }

            if (record.State == SyncOperationState.Confirmed)
            {
                // Konfirmasi tanpa efek berarti data Firestore hilang/berbeda.
                return record.FirestoreHasEffect
                    ? SyncAction.ReturnCachedResult
                    : SyncAction.HaltForReconciliation;
            }

            if (record.AttemptCount >= record.MaxAttempts)
            {
                return SyncAction.Exhausted;
            }

            if (record.State == SyncOperationState.Reserved)
            {
                // Jendela kegagalan: D1 tertulis, Firestore belum.
                return record.FirestoreHasEffect
                    ? SyncAction.MarkConfirmed
                    : SyncAction.ApplyToFirestore;
            }

            // Pending: catatan D1 belum ada (atau hilang).
            return record.FirestoreHasEffect
                ? SyncAction.MarkConfirmed
                : SyncAction.ReserveThenApply;
        }
    }
}
