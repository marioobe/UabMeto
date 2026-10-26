namespace UabMeto.Core.Sync
{
    /// <summary>
    /// Status operasi sinkronisasi seperti tercatat di D1 (server).
    /// Status ini bukan bukti bahwa efek di Firestore sudah tertulis.
    /// </summary>
    public enum SyncOperationState
    {
        /// <summary>Belum ada catatan D1; klien masih menyimpan di outbox.</summary>
        Pending = 0,

        /// <summary>D1 sudah mencatat operationId, Firestore belum tentu tertulis.</summary>
        Reserved = 1,

        /// <summary>Efek Firestore terverifikasi dan D1 sudah dikonfirmasi.</summary>
        Confirmed = 2,

        /// <summary>Ditolak validasi; tidak boleh di-retry otomatis.</summary>
        Rejected = 3,

        /// <summary>Gagal permanen; butuh rekonsiliasi manual.</summary>
        Failed = 4
    }
}
