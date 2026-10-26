using System.Collections.Generic;
using NUnit.Framework;
using UabMeto.Core.Content;

namespace UabMeto.Tests.EditMode
{
    /// <summary>
    /// Menguji aturan gerbang penerbitan konten (F0).
    /// Aturan ini harus konsisten dengan tools/content-validator.
    /// </summary>
    public sealed class PublishGateTests
    {
        private static LessonValidation ValidLesson()
        {
            return new LessonValidation
            {
                Status = ValidationStatus.Validated,
                ReviewedBy = "reviewer-ttu-01",
                ReviewedAt = "2026-10-26",
                Sources = new List<string> { "SRC-0001" },
                AudioLicense = "CC-BY-4.0",
                ConsentRef = "CONSENT-0001",
                HasAudio = true
            };
        }

        [Test]
        public void ValidatedContent_IsAllowed()
        {
            var result = PublishGate.Evaluate(ValidLesson());

            Assert.IsTrue(result.Allowed, string.Join(" | ", result.Reasons));
            Assert.IsEmpty(result.Reasons);
        }

        [Test]
        public void UnvalidatedMarker_IsBlocked()
        {
            var lesson = ValidLesson();
            lesson.ContainsUnvalidatedMarker = true;

            var result = PublishGate.Evaluate(lesson);

            Assert.IsFalse(result.Allowed);
            Assert.That(result.Reasons, Has.Some.Contains("UNVALIDATED"));
        }

        [Test]
        public void PlaceholderContent_IsBlocked()
        {
            var lesson = ValidLesson();
            lesson.ContainsPlaceholders = true;

            var result = PublishGate.Evaluate(lesson);

            Assert.IsFalse(result.Allowed);
            Assert.That(result.Reasons, Has.Some.Contains("placeholder"));
        }

        [Test]
        public void DraftStatus_IsBlocked()
        {
            var lesson = ValidLesson();
            lesson.Status = ValidationStatus.Draft;

            var result = PublishGate.Evaluate(lesson);

            Assert.IsFalse(result.Allowed);
            Assert.That(result.Reasons, Has.Some.Contains("Status harus"));
        }

        [Test]
        public void MissingReviewer_IsBlocked()
        {
            var lesson = ValidLesson();
            lesson.ReviewedBy = "  ";

            var result = PublishGate.Evaluate(lesson);

            Assert.IsFalse(result.Allowed);
            Assert.That(result.Reasons, Has.Some.Contains("reviewedBy"));
        }

        [Test]
        public void InvalidReviewDate_IsBlocked()
        {
            var lesson = ValidLesson();
            lesson.ReviewedAt = "26/10/2026";

            var result = PublishGate.Evaluate(lesson);

            Assert.IsFalse(result.Allowed);
            Assert.That(result.Reasons, Has.Some.Contains("reviewedAt"));
        }

        [Test]
        public void MissingSources_IsBlocked()
        {
            var lesson = ValidLesson();
            lesson.Sources = new List<string>();

            var result = PublishGate.Evaluate(lesson);

            Assert.IsFalse(result.Allowed);
            Assert.That(result.Reasons, Has.Some.Contains("sumber referensi"));
        }

        [Test]
        public void AudioLessonWithoutConsent_IsBlocked()
        {
            var lesson = ValidLesson();
            lesson.ConsentRef = string.Empty;

            var result = PublishGate.Evaluate(lesson);

            Assert.IsFalse(result.Allowed);
            Assert.That(result.Reasons, Has.Some.Contains("consentRef"));
        }

        [Test]
        public void NonAudioLesson_DoesNotRequireConsent()
        {
            var lesson = ValidLesson();
            lesson.HasAudio = false;
            lesson.ConsentRef = string.Empty;

            var result = PublishGate.Evaluate(lesson);

            Assert.IsTrue(result.Allowed, string.Join(" | ", result.Reasons));
        }

        [Test]
        public void NullMetadata_IsBlocked()
        {
            var result = PublishGate.Evaluate(null);

            Assert.IsFalse(result.Allowed);
            Assert.IsNotEmpty(result.Reasons);
        }
    }
}
