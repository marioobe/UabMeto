using System;
using NUnit.Framework;
using UabMeto.App.Content;
using UnityEngine;

namespace UabMeto.Tests.EditMode
{
    public sealed class ContentRepositoryTests
    {
        private static string TopicsJson()
        {
            var asset = Resources.Load<TextAsset>("Content/topics");
            Assert.IsNotNull(asset, "topics.json belum disalin ke Resources. Jalankan F1 Setup.");
            return asset.text;
        }

        private static string ValidLessonJson(string status = "validated")
        {
            return
                "{" +
                "\"schemaVersion\":\"1.1.0\"," +
                "\"id\":\"lesson-test-001\"," +
                "\"topicId\":\"topic-01-salam\"," +
                "\"order\":1," +
                "\"xp\":10," +
                "\"estimatedMinutes\":5," +
                "\"title\":\"Test lesson\"," +
                "\"summary\":\"Test only\"," +
                "\"status\":\"" + status + "\"," +
                "\"validation\":{" +
                "\"reviewedBy\":\"reviewer-test\"," +
                "\"reviewedAt\":\"2026-10-26\"," +
                "\"sources\":[{\"id\":\"src-test\",\"citation\":\"Test source\"}]," +
                "\"audioLicense\":\"CC0\"," +
                "\"consentRef\":\"consent-test\"}," +
                "\"objectives\":[\"Test objective\"]," +
                "\"vocabulary\":[]," +
                "\"dialogues\":[]," +
                "\"exercises\":[]," +
                "\"examples\":[{" +
                "\"id\":\"ex-test-1\"," +
                "\"dawan\":\"x\"," +
                "\"indonesian\":\"y\"," +
                "\"context\":\"test\"," +
                "\"status\":\"validated\"," +
                "\"sourceRef\":\"src-test\"}]," +
                "\"notes\":\"Test only\"" +
                "}";
        }

        private static string FixtureJson()
        {
            var path = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    Application.dataPath,
                    "..",
                    "..",
                    "content",
                    "samples",
                    "lesson-001-salam.UNVALIDATED.json"));
            Assert.IsTrue(System.IO.File.Exists(path), "Fixture tidak ditemukan: " + path);
            return System.IO.File.ReadAllText(path);
        }

        [Test]
        public void EmptyLessons_ProducesEmptyStateCatalog()
        {
            var catalog = ContentRepository.LoadFromText(
                TopicsJson(),
                Array.Empty<string>());

            Assert.IsFalse(catalog.HasValidatedLessons);
            Assert.AreEqual(12, catalog.Topics.Length);
            Assert.AreEqual(0, catalog.ValidatedLessons.Length);
            Assert.IsNull(catalog.DevelopmentFixture);
            Assert.That(catalog.Warnings, Has.Some.Contains("empty state"));
        }

        [Test]
        public void ValidatedLesson_IsPublishedToCatalog()
        {
            var catalog = ContentRepository.LoadFromText(
                TopicsJson(),
                new[] { ValidLessonJson() });

            Assert.IsTrue(catalog.HasValidatedLessons);
            Assert.AreEqual(1, catalog.ValidatedLessons.Length);
            Assert.AreEqual("lesson-test-001", catalog.ValidatedLessons[0].id);
            Assert.AreEqual(1, catalog.LessonCountForTopic("topic-01-salam"));
        }

        [Test]
        public void PublishedLesson_IsStillReadable()
        {
            var catalog = ContentRepository.LoadFromText(
                TopicsJson(),
                new[] { ValidLessonJson("published") });

            Assert.IsTrue(catalog.HasValidatedLessons);
            Assert.AreEqual(1, catalog.ValidatedLessons.Length);
        }

        [Test]
        public void UnvalidatedLesson_IsRejected()
        {
            var catalog = ContentRepository.LoadFromText(
                TopicsJson(),
                new[] { FixtureJson() });

            Assert.IsFalse(catalog.HasValidatedLessons);
            Assert.IsNull(catalog.DevelopmentFixture);
            Assert.That(catalog.Warnings, Has.Some.Contains("tidak lolos gate"));
        }

        [Test]
        public void Fixture_IsExposedOnlyAsDevelopmentData()
        {
            var catalog = ContentRepository.LoadFromText(
                TopicsJson(),
                Array.Empty<string>(),
                FixtureJson());

            Assert.IsFalse(catalog.HasValidatedLessons);
            Assert.IsNotNull(catalog.DevelopmentFixture);
            Assert.AreEqual("UNVALIDATED", catalog.DevelopmentFixture.status);
            Assert.That(catalog.Warnings, Has.Some.Contains("Fixture pengembangan"));
        }

        [Test]
        public void LessonWithPlaceholder_IsRejectedEvenIfStatusValidated()
        {
            var json = ValidLessonJson().Replace(
                "\"dawan\":\"x\"",
                "\"dawan\":\"[PLACEHOLDER-DWN-999]\"");

            var catalog = ContentRepository.LoadFromText(
                TopicsJson(),
                new[] { json });

            Assert.IsFalse(catalog.HasValidatedLessons);
            Assert.That(catalog.Warnings, Has.Some.Contains("tidak lolos gate"));
        }

        [Test]
        public void LessonWithoutExamples_IsRejected()
        {
            var json = ValidLessonJson().Replace(
                "\"examples\":[{\"id\":\"ex-test-1\",\"dawan\":\"x\",\"indonesian\":\"y\",\"context\":\"test\",\"status\":\"validated\",\"sourceRef\":\"src-test\"}]",
                "\"examples\":[]");

            var catalog = ContentRepository.LoadFromText(
                TopicsJson(),
                new[] { json });

            Assert.IsFalse(catalog.HasValidatedLessons);
        }
    }
}
