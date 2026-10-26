using System;
using System.Collections.Generic;

namespace UabMeto.Core.Content
{
    /// <summary>Hasil evaluasi gerbang penerbitan.</summary>
    public readonly struct PublishGateResult
    {
        public PublishGateResult(bool allowed, IReadOnlyList<string> reasons)
        {
            Allowed = allowed;
            Reasons = reasons ?? Array.Empty<string>();
        }

        public bool Allowed { get; }

        public IReadOnlyList<string> Reasons { get; }
    }

    /// <summary>
    /// Aturan penerbitan konten. Aturan ini mencerminkan validator Node
    /// (<c>tools/content-validator</c>) agar pipeline dan klien tidak berbeda pendapat.
    /// Materi hanya boleh terbit bila sudah divalidasi penutur lokal dan lengkap
    /// metadata sumber serta izinnya.
    /// </summary>
    public static class PublishGate
    {
        public static PublishGateResult Evaluate(LessonValidation validation)
        {
            var reasons = new List<string>();

            if (validation == null)
            {
                reasons.Add("Metadata validasi tidak ada.");
                return new PublishGateResult(false, reasons);
            }

            if (validation.ContainsUnvalidatedMarker)
            {
                reasons.Add("Konten masih ditandai UNVALIDATED.");
            }

            if (validation.ContainsPlaceholders)
            {
                reasons.Add("Konten masih berisi placeholder.");
            }

            if (validation.Status != ValidationStatus.Validated)
            {
                reasons.Add(
                    $"Status harus '{ValidationStatus.Validated}', ditemukan '{validation.Status}'.");
            }

            if (string.IsNullOrWhiteSpace(validation.ReviewedBy))
            {
                reasons.Add("Peninjau (reviewedBy) belum diisi.");
            }

            if (!IsIsoDate(validation.ReviewedAt))
            {
                reasons.Add("Tanggal tinjauan (reviewedAt) harus ISO-8601 (YYYY-MM-DD).");
            }

            if (validation.Sources == null || validation.Sources.Count == 0)
            {
                reasons.Add("Minimal satu sumber referensi diperlukan.");
            }
            else
            {
                for (var i = 0; i < validation.Sources.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(validation.Sources[i]))
                    {
                        reasons.Add($"Sumber referensi pada indeks {i} kosong.");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(validation.AudioLicense))
            {
                reasons.Add("Lisensi audio belum diisi.");
            }

            if (validation.HasAudio && string.IsNullOrWhiteSpace(validation.ConsentRef))
            {
                reasons.Add("Bukti izin rekaman (consentRef) diperlukan untuk pelajaran beraudio.");
            }

            return new PublishGateResult(reasons.Count == 0, reasons);
        }

        private static bool IsIsoDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return DateTime.TryParseExact(
                value,
                "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out _);
        }
    }
}
