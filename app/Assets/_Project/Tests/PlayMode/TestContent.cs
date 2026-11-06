using System;
using System.IO;
using UnityEngine;

namespace UabMeto.Tests.PlayMode
{
    /// <summary>
    /// Memuat pelajaran nyata lalu menurunkannya ke status in_review hanya di
    /// memori. Dipakai tes pratinjau agar tidak bergantung pada status berkas
    /// konten yang dapat berubah setelah validasi. Tidak mengarang materi.
    /// </summary>
    internal static class TestContent
    {
        public static string LoadLessonAsInReview(string fileName = "lesson-001-salam.json")
        {
            var path = Path.GetFullPath(
                Path.Combine(
                    Application.dataPath,
                    "..",
                    "..",
                    "content",
                    "lessons",
                    fileName));

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    "Berkas pelajaran tidak ditemukan: " + path,
                    path);
            }

            return File.ReadAllText(path)
                .Replace("\"status\": \"validated\"", "\"status\": \"in_review\"")
                .Replace("\"status\": \"published\"", "\"status\": \"in_review\"");
        }
    }
}
