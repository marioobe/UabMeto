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

        /// <summary>
        /// Pelajaran berstatus <c>in_review</c> yang hanya boleh tampil dengan
        /// label pratinjau. Tidak pernah dianggap materi terbit.
        /// </summary>
        public LessonContent[] PreviewLessons { get; set; } = Array.Empty<LessonContent>();

        public LessonContent DevelopmentFixture { get; set; }
        public string[] Warnings { get; set; } = Array.Empty<string>();

        public bool HasValidatedLessons => ValidatedLessons != null && ValidatedLessons.Length > 0;

        public bool HasPreviewLessons => PreviewLessons != null && PreviewLessons.Length > 0;

        public LessonContent FindLessonForTopic(string topicId)
        {
            return ValidatedLessons.FirstOrDefault(lesson => lesson.topicId == topicId);
        }

        public int LessonCountForTopic(string topicId)
        {
            return ValidatedLessons.Count(lesson => lesson.topicId == topicId);
        }

        public LessonContent FindPreviewLessonForTopic(string topicId)
        {
            return PreviewLessons.FirstOrDefault(lesson => lesson.topicId == topicId);
        }

        public int PreviewLessonCountForTopic(string topicId)
        {
            return PreviewLessons.Count(lesson => lesson.topicId == topicId);
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

        /// <summary>
        /// Pelajaran <c>in_review</c> hanya disalin ke folder ini oleh menu
        /// "Uab Meto/F1 Preview Setup". Build rilis menjalankan "F1 Setup"
        /// biasa sehingga folder ini kosong.
        /// </summary>
        public const string PreviewLessonsResourcePath = "Content/preview";

        /// <summary>
        /// Saklar tunggal untuk pratinjau <c>in_review</c>. Ubah menjadi
        /// <c>false</c> bila pratinjau tidak boleh tampil sama sekali.
        /// </summary>
        public const bool AllowInReviewPreview = true;

#if UNITY_EDITOR
        private const string FixtureRelativePath = "content/samples/lesson-001-salam.UNVALIDATED.json";
#endif

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

            var previewAssets = Resources.LoadAll<TextAsset>(PreviewLessonsResourcePath)
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
            return LoadFromText(
                topicsAsset.text,
                lessonAssets,
                fixtureJson,
                AllowInReviewPreview,
                previewAssets);
        }

        public static ContentCatalog LoadFromText(
            string topicsJson,
            IEnumerable<string> lessonJsons,
            string fixtureJson = null,
            bool allowInReviewPreview = AllowInReviewPreview,
            IEnumerable<string> previewLessonJsons = null)
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
            var preview = new List<LessonContent>();
            var validatedIds = new HashSet<string>(StringComparer.Ordinal);
            var previewIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var json in lessonJsons ?? Array.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    continue;
                }

                var lesson = ParseLesson(json, warnings);
                if (lesson == null)
                {
                    continue;
                }

                if (IsPublishable(lesson, json))
                {
                    validated.Add(lesson);
                    validatedIds.Add(lesson.id);
                }
                else if (allowInReviewPreview && IsPreviewable(lesson, json))
                {
                    AddPreview(lesson, preview, previewIds, warnings);
                }
                else
                {
                    warnings.Add($"Pelajaran '{lesson.id}' tidak lolos gate dan tidak ditampilkan.");
                }
            }

            if (allowInReviewPreview)
            {
                foreach (var json in previewLessonJsons ?? Array.Empty<string>())
                {
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        continue;
                    }

                    var lesson = ParseLesson(json, warnings);
                    if (lesson == null)
                    {
                        continue;
                    }

                    if (IsPreviewable(lesson, json) && !validatedIds.Contains(lesson.id))
                    {
                        AddPreview(lesson, preview, previewIds, warnings);
                    }
                    else
                    {
                        warnings.Add(
                            $"Pelajaran pratinjau '{lesson.id}' tidak lolos gate pratinjau dan tidak ditampilkan.");
                    }
                }
            }

            var orderedLessons = validated
                .OrderBy(lesson => TopicOrder(topics, lesson.topicId))
                .ThenBy(lesson => lesson.order)
                .ToArray();

            var orderedPreview = preview
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
                PreviewLessons = orderedPreview,
                DevelopmentFixture = fixture,
                Warnings = warnings.ToArray()
            };
        }

        private static LessonContent ParseLesson(string json, List<string> warnings)
        {
            try
            {
                var lesson = JsonUtility.FromJson<LessonContent>(json);
                if (lesson == null || string.IsNullOrWhiteSpace(lesson.id))
                {
                    warnings.Add("Pelajaran tanpa id diabaikan.");
                    return null;
                }

                return lesson;
            }
            catch (Exception exception)
            {
                warnings.Add($"Pelajaran gagal di-parse: {exception.Message}");
                return null;
            }
        }

        private static void AddPreview(
            LessonContent lesson,
            List<LessonContent> preview,
            HashSet<string> previewIds,
            List<string> warnings)
        {
            if (!previewIds.Add(lesson.id))
            {
                return;
            }

            preview.Add(lesson);
            warnings.Add(
                $"Pelajaran '{lesson.id}' berstatus in_review dan hanya masuk pratinjau, bukan materi terbit.");
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

        /// <summary>
        /// Gate pratinjau: hanya <c>in_review</c>, skema 1.1.0, metadata review
        /// dan sumber terisi, tanpa placeholder, dan semua contoh juga
        /// <c>in_review</c>. Tidak pernah meloloskan <c>draft</c> atau
        /// <c>UNVALIDATED</c>.
        /// </summary>
        public static bool IsPreviewable(LessonContent lesson, string rawJson = null)
        {
            if (lesson == null || lesson.validation == null)
            {
                return false;
            }

            if (lesson.schemaVersion != "1.1.0" || lesson.status != "in_review")
            {
                return false;
            }

            if (!string.IsNullOrEmpty(rawJson) &&
                rawJson.IndexOf("[PLACEHOLDER", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(lesson.validation.reviewedBy) ||
                string.IsNullOrWhiteSpace(lesson.validation.reviewedAt))
            {
                return false;
            }

            var sources = lesson.validation.sources ?? Array.Empty<SourceRef>();
            if (sources.Length == 0 ||
                sources.Any(source =>
                    source == null ||
                    string.IsNullOrWhiteSpace(source.id) ||
                    string.IsNullOrWhiteSpace(source.citation)))
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
                    example.status != "in_review")
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
