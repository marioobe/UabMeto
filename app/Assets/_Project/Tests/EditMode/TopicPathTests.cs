using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UabMeto.App.Content;
using UnityEngine;

namespace UabMeto.Tests.EditMode
{
    /// <summary>
    /// Mengunci jalur 12 topik PRD §3.2 dan memastikan label dialek di luar
    /// TTU/Kefamenanu tidak masuk ke UI atau registry.
    /// </summary>
    public sealed class TopicPathTests
    {
        private static readonly string[] ExpectedIds =
        {
            "topic-01-salam",
            "topic-02-perkenalan",
            "topic-03-keluarga",
            "topic-04-angka-waktu-hari",
            "topic-05-aktivitas",
            "topic-06-makanan",
            "topic-07-rumah",
            "topic-08-tempat-arah",
            "topic-09-belanja",
            "topic-10-budaya",
            "topic-11-kesehatan-darurat",
            "topic-12-sekolah-pekerjaan"
        };

        private static readonly string[] ForbiddenTerms =
        {
            "Amarasi",
            "Amanuban",
            "Sonbai",
            "Mutis",
            "Miomafo",
            "Dawan Netral"
        };

        [Test]
        public void CanonicalPath_IsExactlyTheTwelvePrdTopics()
        {
            var topics = Resources.Load<TextAsset>("Content/topics");
            Assert.IsNotNull(topics, "topics.json belum disalin ke Resources. Jalankan F1 Setup.");

            var catalog = ContentRepository.LoadFromText(
                topics.text,
                Array.Empty<string>());

            CollectionAssert.AreEqual(
                ExpectedIds,
                catalog.Topics.Select(topic => topic.id).ToArray());

            Assert.IsFalse(
                catalog.Topics.Any(topic =>
                    topic.id.IndexOf("exam", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    topic.title.IndexOf("ujian", StringComparison.OrdinalIgnoreCase) >= 0),
                "Ujian akhir tidak boleh menjadi topik jalur.");
        }

        [Test]
        public void UiSource_DoesNotContainOutOfScopeDialectLabels()
        {
            var uiDirectory = Path.Combine(
                Application.dataPath,
                "_Project",
                "Scripts",
                "App",
                "UI");
            Assert.IsTrue(Directory.Exists(uiDirectory));

            var hits = Directory
                .GetFiles(uiDirectory, "*.cs", SearchOption.AllDirectories)
                .SelectMany(path => File.ReadAllLines(path)
                    .Select((line, index) => new { path, line, index }))
                .Where(item => ForbiddenTerms.Any(term =>
                    item.line.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))
                .Select(item => $"{Path.GetFileName(item.path)}:{item.index + 1}: {item.line.Trim()}")
                .ToArray();

            Assert.IsEmpty(hits, "Label dialek di luar TTU ditemukan di UI: " + string.Join("; ", hits));
        }

        [Test]
        public void TopicRegistry_DoesNotContainForbiddenTerms()
        {
            var topics = Resources.Load<TextAsset>("Content/topics");
            Assert.IsNotNull(topics);
            var hits = ForbiddenTerms
                .Where(term => topics.text.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();
            Assert.IsEmpty(hits, "Istilah dialek terlarang di topics.json: " + string.Join(", ", hits));
        }
    }
}
