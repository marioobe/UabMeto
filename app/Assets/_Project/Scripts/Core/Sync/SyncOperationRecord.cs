namespace UabMeto.Core.Sync
{
    /// <summary>
    /// Potret satu operasi sinkronisasi saat hendak diproses ulang.
    /// Nilai <see cref="FirestoreHasEffect"/> berasal dari pemeriksaan penanda
    /// idempotensi di Firestore (bukan diasumsikan dari status D1).
    /// </summary>
    public sealed class SyncOperationRecord
    {
        public string OperationId { get; set; } = string.Empty;

        public SyncOperationState State { get; set; } = SyncOperationState.Pending;

        /// <summary>Firestore sudah memuat efek untuk operationId ini.</summary>
        public bool FirestoreHasEffect { get; set; }

        /// <summary>
        /// Payload operasi cocok dengan efek yang tersimpan di Firestore.
        /// Jika tidak cocok, jangan menimpa: hentikan untuk rekonsiliasi.
        /// </summary>
        public bool PayloadMatchesAppliedEffect { get; set; } = true;

        /// <summary>Payload lolos validasi skema dan aturan bisnis.</summary>
        public bool PayloadValid { get; set; } = true;

        public int AttemptCount { get; set; }

        public int MaxAttempts { get; set; } = 8;
    }
}
