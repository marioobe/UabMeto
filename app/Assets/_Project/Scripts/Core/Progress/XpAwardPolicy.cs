using System;

namespace UabMeto.Core.Progress
{
    /// <summary>
    /// Definisi XP dari materi yang diterbitkan. Ini adalah satu-satunya sumber
    /// nilai hadiah yang boleh dipakai saat menyelesaikan pelajaran.
    /// </summary>
    public sealed class LessonXpDefinition
    {
        public string LessonId { get; set; } = string.Empty;

        /// <summary>Versi materi terbit, misalnya hash konten atau nomor versi.</summary>
        public string ContentVersion { get; set; } = string.Empty;

        public int Xp { get; set; }
    }

    /// <summary>
    /// Permintaan penyelesaian pelajaran dari klien. Nilai XP dari klien tidak
    /// pernah dipakai sebagai hadiah; hanya dicatat sebagai klaim untuk deteksi
    /// penyimpangan.
    /// </summary>
    public sealed class XpAwardRequest
    {
        public string LessonId { get; set; } = string.Empty;

        public string ContentVersion { get; set; } = string.Empty;

        public int ClientClaimedXp { get; set; }
    }

    public readonly struct XpAwardResult
    {
        public XpAwardResult(bool accepted, int awardedXp, string reason)
        {
            Accepted = accepted;
            AwardedXp = awardedXp;
            Reason = reason ?? string.Empty;
        }

        public bool Accepted { get; }

        /// <summary>XP yang benar-benar diberikan. Selalu berasal dari server.</summary>
        public int AwardedXp { get; }

        public string Reason { get; }
    }

    /// <summary>
    /// Kebijakan hadiah XP yang berotoritas server.
    ///
    /// Aturan:
    /// - Hadiah selalu diambil dari <see cref="LessonXpDefinition.Xp"/>.
    /// - Nilai <see cref="XpAwardRequest.ClientClaimedXp"/> diabaikan.
    /// - Pelajaran atau versi yang tidak cocok ditolak.
    /// - Nilai server negatif ditolak.
    /// </summary>
    public static class XpAwardPolicy
    {
        public static XpAwardResult Resolve(
            LessonXpDefinition serverDefinition,
            XpAwardRequest request)
        {
            if (serverDefinition == null)
            {
                return Reject("Definisi XP server tidak ada.");
            }

            if (request == null)
            {
                return Reject("Permintaan penyelesaian tidak ada.");
            }

            if (serverDefinition.Xp < 0)
            {
                return Reject("Nilai XP server tidak valid.");
            }

            if (string.IsNullOrWhiteSpace(serverDefinition.LessonId) ||
                !string.Equals(serverDefinition.LessonId, request.LessonId, StringComparison.Ordinal))
            {
                return Reject("Pelajaran tidak cocok dengan definisi server.");
            }

            if (string.IsNullOrWhiteSpace(serverDefinition.ContentVersion) ||
                !string.Equals(serverDefinition.ContentVersion, request.ContentVersion, StringComparison.Ordinal))
            {
                return Reject("Versi materi tidak cocok dengan definisi server.");
            }

            return new XpAwardResult(true, serverDefinition.Xp, "Hadiah XP dari server.");
        }

        private static XpAwardResult Reject(string reason)
        {
            return new XpAwardResult(false, 0, reason);
        }
    }
}
