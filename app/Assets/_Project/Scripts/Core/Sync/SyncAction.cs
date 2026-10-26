namespace UabMeto.Core.Sync
{
    /// <summary>Tindakan yang harus diambil untuk satu operasi sinkronisasi.</summary>
    public enum SyncAction
    {
        /// <summary>Klien mengirim operasi (belum ada catatan server).</summary>
        SendOperation = 0,

        /// <summary>Server menulis catatan D1 lalu menerapkan efek ke Firestore.</summary>
        ReserveThenApply = 1,

        /// <summary>D1 sudah reserved, efek Firestore belum ada: terapkan sekarang.</summary>
        ApplyToFirestore = 2,

        /// <summary>Efek Firestore sudah ada: cukup konfirmasi D1.</summary>
        MarkConfirmed = 3,

        /// <summary>Duplikat setelah terkonfirmasi: kembalikan hasil tersimpan.</summary>
        ReturnCachedResult = 4,

        /// <summary>Status tidak konsisten: jangan menulis, jalankan rekonsiliasi.</summary>
        HaltForReconciliation = 5,

        /// <summary>Payload tidak valid: tolak permanen.</summary>
        Reject = 6,

        /// <summary>Batas percobaan habis: hentikan retry otomatis.</summary>
        Exhausted = 7
    }
}
