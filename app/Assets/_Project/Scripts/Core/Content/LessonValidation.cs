using System;
using System.Collections.Generic;

namespace UabMeto.Core.Content
{
    /// <summary>
    /// Metadata validasi yang menyertai satu pelajaran.
    /// Nilai-nilai ini wajib terisi sebelum materi boleh diterbitkan.
    /// </summary>
    public sealed class LessonValidation
    {
        public ValidationStatus Status { get; set; } = ValidationStatus.Draft;

        /// <summary>Identitas peninjau (penutur lokal / ahli varian TTU).</summary>
        public string ReviewedBy { get; set; } = string.Empty;

        /// <summary>Tanggal tinjauan, format ISO-8601: YYYY-MM-DD.</summary>
        public string ReviewedAt { get; set; } = string.Empty;

        /// <summary>Daftar ID sumber referensi yang dipakai.</summary>
        public IReadOnlyList<string> Sources { get; set; } = Array.Empty<string>();

        /// <summary>Lisensi/persetujuan penggunaan audio pelajaran.</summary>
        public string AudioLicense { get; set; } = string.Empty;

        /// <summary>Referensi bukti izin rekaman (consent). Wajib bila pelajaran beraudio.</summary>
        public string ConsentRef { get; set; } = string.Empty;

        /// <summary>Apakah pelajaran memuat audio.</summary>
        public bool HasAudio { get; set; }

        /// <summary>Konten masih mengandung penanda placeholder.</summary>
        public bool ContainsPlaceholders { get; set; }

        /// <summary>Konten membawa penanda UNVALIDATED.</summary>
        public bool ContainsUnvalidatedMarker { get; set; }
    }
}
