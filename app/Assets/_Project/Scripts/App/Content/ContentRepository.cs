using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UabMeto.Core.Content;
using UnityEngine;

namespace UabMeto.App.Content
{
    /// <summary>Katalog konten yang boleh dipakai UI.</summary>
    public sealed class ContentCatalog
    {
        public TopicEntry[] Topics { get; set; } = Array.Empty<TopicEntry>();
        public LessonContent[] ValidatedLessons { get; set; } = Array.Empty<LessonContent>();
        public LessonContent DevelopmentFixture { get; set; }
        public string[] Warnings { get; set; } = Array.Empty<string>();

        public bool HasValidatedLessons => ValidatedLessons.Length > 0;

        public LessonContent FindLessonForTopic(string topicId)
        {
            return ValidatedLessons.FirstOrDefault(lesson => lesson.topicId == topicId);
        }

        public int LessonCountForTopic(string topicId)
        {
            return ValidatedLessons.Count(lesson => lesson.topicId == topicId);
        }
    }

    /// <summary>
    /// Memuat topik dan pelajaran. Hanya pelajaran yang lolos gate yang masuk
    /// <see cref="ContentCatalog.ValidatedLessons"/>. Fixture UNVALIDATED hanya
    /// dimuat di Editor dan tidak pernah masuk build.
    /// </summary>
    public static class ContentRepository
    {
        public const string TopicsResourcePath = "Content/topics";
        public const string LessonsResourcePath = "Content/lessons";
        private const string FixtureRelativePath = "content/samples/lesson-001-salam.UNVALIDATED.json";

        public static ContentCatalog LoadDefault()
        {
            var topicsAsset = Resources.Load<TextAsset>(TopicsResourcePath);
            if (topicsAsset == null)
            {
                throw new InvalidDataException(
                    $"Resource '{TopicsResourcePath}' tidak ditemukan. Jalankan Uab Meto/F1 Setup.");
            }

            var lessonAssets = Resources.LoadAll<TextAsset>(LessonsResourcePath)
                .OrderBy(asset => asset.name)
                .Select(asset => asset.text)
                .ToArray();

            string fixtureJson = null;
#if UNITY_EDITOR
            var fixturePath = Path.GetFullPath(
                Path.Combine(Application.dataPath, "..", "..", FixtureRelativePath));
            if (File.Exists(fixturePath))
            {
                fixtureJson = File.ReadAllText(fixturePath);
            }
#endif
            return LoadFromText(topicsAsset.text, lessonAssets, fixtureJson);
        }

        public static ContentCatalog LoadFromText(
            string topicsJson,
            IEnumerable<string> lessonJsons,
            string fixtureJson = null)
        {
            var warnings = new List<string>();
            var registry = JsonUtility.FromJson<TopicRegistry>(topicsJson);
            if (registry == null || registry.topics == null)
            {
                throw new InvalidDataException("topics.json tidak valid.");
            }

            var topics = registry.topics
                .OrderBy(topic => topic.order)
                .ToArray();

            if (topics.Length != 12)
            {
                warnings.Add($"Registry topik berisi {topics.Length}, bukan 12.");
            }

            var validated = new List<LessonContent>();
            foreach (var json in lessonJsons ?? Array.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    continue;
                }

                LessonContent lesson;
                try
                {
                    lesson = JsonUtility.FromJson<LessonContent>(json);
                }
                catch (Exception exception)
                {
                    warnings.Add($"Pelajaran gagal di-parse: {exception.Message}");
                    continue;
                }

                if (lesson == null || string.IsNullOrWhiteSpace(lesson.id))
                {
                    warnings.Add("Pelajaran tanpa id diabaikan.");
                    continue;
                }

                if (IsPublishable(lesson, json))
                {
                    validated.Add(lesson);
                }
                else
                {
                    warnings.Add($"Pelajaran '{lesson.id}' tidak lolos gate dan tidak ditampilkan.");
                }
            }

            var orderedLessons = validated
                .OrderBy(lesson => TopicOrder(topics, lesson.topicId))
                .ThenBy(lesson => lesson.order)
                .ToArray();

            LessonContent fixture = null;
            if (!string.IsNullOrWhiteSpace(fixtureJson))
            {
                fixture = JsonUtility.FromJson<LessonContent>(fixtureJson);
                if (fixture != null)
                {
                    warnings.Add(
                        $"Fixture pengembangan '{fixture.id}' dimuat. Bukan materi Dawan dan tidak masuk build.");
                }
            }

            if (orderedLessons.Length == 0)
            {
                warnings.Add("Belum ada pelajaran validated. Beranda harus menampilkan empty state.");
            }

            return new ContentCatalog
            {
                Topics = topics,
                ValidatedLessons = orderedLessons,
                DevelopmentFixture = fixture,
                Warnings = warnings.ToArray()
            };
        }

        public static bool IsPublishable(LessonContent lesson, string rawJson = null)
        {
            if (lesson == null || lesson.validation == null)
            {
                return false;
            }

            if (lesson.schemaVersion != "1.1.0")
            {
                return false;
            }

            var containsPlaceholders =
                !string.IsNullOrEmpty(rawJson) &&
                rawJson.IndexOf("[PLACEHOLDER", StringComparison.OrdinalIgnoreCase) >= 0;

            var validation = new LessonValidation
            {
                SchemaVersion = lesson.schemaVersion,
                // 'published' sudah melewati gate; untuk pembacaan klien
                // diperlakukan setara 'validated'.
                Status = lesson.status == "published"
                    ? ValidationStatus.Validated
                    : ParseStatus(lesson.status),
                ReviewedBy = lesson.validation.reviewedBy ?? string.Empty,
                ReviewedAt = lesson.validation.reviewedAt ?? string.Empty,
                Sources = (lesson.validation.sources ?? Array.Empty<SourceRef>())
                    .Select(source => source?.id ?? string.Empty)
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .ToList(),
                AudioLicense = lesson.validation.audioLicense ?? string.Empty,
                ConsentRef = lesson.validation.consentRef ?? string.Empty,
                HasAudio = HasAudio(lesson),
                ContainsPlaceholders = containsPlaceholders
            };

            if (!PublishGate.Evaluate(validation).Allowed)
            {
                return false;
            }

            if (lesson.examples == null || lesson.examples.Length == 0)
            {
                return false;
            }

            foreach (var example in lesson.examples)
            {
                if (example == null ||
                    string.IsNullOrWhiteSpace(example.sourceRef) ||
                    (example.status != "validated" && example.status != "published"))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool HasAudio(LessonContent lesson)
        {
            return (lesson.vocabulary ?? Array.Empty<VocabularyItem>())
                       .Any(item => HasAudioFile(item?.audio)) ||
                   (lesson.dialogues ?? Array.Empty<Dialogue>())
                       .SelectMany(dialogue => dialogue?.lines ?? Array.Empty<DialogueLine>())
                       .Any(line => HasAudioFile(line?.audio)) ||
                   (lesson.exercises ?? Array.Empty<Exercise>())
                       .Any(exercise =>
                           exercise?.type == "listening" ||
                           (exercise?.content != null &&
                            (!string.IsNullOrWhiteSpace(exercise.content.audioRef) ||
                             !string.IsNullOrWhiteSpace(exercise.content.targetAudioRef)))) ||
                   (lesson.examples ?? Array.Empty<ExampleItem>())
                       .Any(example => HasAudioFile(example?.audio));
        }

        public static bool HasAudioFile(AudioRef audio)
        {
            return audio != null && !string.IsNullOrWhiteSpace(audio.file);
        }

        private static int TopicOrder(TopicEntry[] topics, string topicId)
        {
            var topic = topics.FirstOrDefault(entry => entry.id == topicId);
            return topic?.order ?? int.MaxValue;
        }

        private static ValidationStatus ParseStatus(string status)
        {
            switch (status)
            {
                case "UNVALIDATED":
                    return ValidationStatus.Unvalidated;
                case "draft":
                    return ValidationStatus.Draft;
                case "in_review":
                    return ValidationStatus.InReview;
                case "validated":
                    return ValidationStatus.Validated;
                case "published":
                    return ValidationStatus.Published;
                default:
                    return ValidationStatus.Draft;
            }
        }
    }
}
