using NUnit.Framework;
using UabMeto.Core.Progress;

namespace UabMeto.Tests.EditMode
{
    /// <summary>
    /// Memastikan XP yang dikirim klien tidak dapat mengubah hadiah server.
    /// </summary>
    public sealed class XpAwardPolicyTests
    {
        private static LessonXpDefinition ServerLesson(int xp = 25)
        {
            return new LessonXpDefinition
            {
                LessonId = "lesson-001-salam",
                ContentVersion = "v1",
                Xp = xp
            };
        }

        private static XpAwardRequest Request(int clientXp)
        {
            return new XpAwardRequest
            {
                LessonId = "lesson-001-salam",
                ContentVersion = "v1",
                ClientClaimedXp = clientXp
            };
        }

        [Test]
        public void ClientClaim_IsIgnored()
        {
            var result = XpAwardPolicy.Resolve(ServerLesson(25), Request(9999));

            Assert.IsTrue(result.Accepted);
            Assert.AreEqual(25, result.AwardedXp);
        }

        [Test]
        public void NegativeClientClaim_DoesNotReduceAward()
        {
            var result = XpAwardPolicy.Resolve(ServerLesson(25), Request(-100));

            Assert.IsTrue(result.Accepted);
            Assert.AreEqual(25, result.AwardedXp);
        }

        [Test]
        public void ZeroClientClaim_DoesNotChangeAward()
        {
            var result = XpAwardPolicy.Resolve(ServerLesson(25), Request(0));

            Assert.IsTrue(result.Accepted);
            Assert.AreEqual(25, result.AwardedXp);
        }

        [Test]
        public void ServerXp_IsAuthoritative_EvenWhenClientMatches()
        {
            var result = XpAwardPolicy.Resolve(ServerLesson(40), Request(40));

            Assert.IsTrue(result.Accepted);
            Assert.AreEqual(40, result.AwardedXp);
        }

        [Test]
        public void UnknownLesson_IsRejected()
        {
            var request = Request(25);
            request.LessonId = "lesson-999";

            var result = XpAwardPolicy.Resolve(ServerLesson(25), request);

            Assert.IsFalse(result.Accepted);
            Assert.AreEqual(0, result.AwardedXp);
        }

        [Test]
        public void VersionMismatch_IsRejected()
        {
            var request = Request(25);
            request.ContentVersion = "v2";

            var result = XpAwardPolicy.Resolve(ServerLesson(25), request);

            Assert.IsFalse(result.Accepted);
            Assert.AreEqual(0, result.AwardedXp);
        }

        [Test]
        public void NegativeServerXp_IsRejected()
        {
            var result = XpAwardPolicy.Resolve(ServerLesson(-5), Request(25));

            Assert.IsFalse(result.Accepted);
            Assert.AreEqual(0, result.AwardedXp);
        }

        [Test]
        public void NullDefinition_IsRejected()
        {
            var result = XpAwardPolicy.Resolve(null, Request(25));

            Assert.IsFalse(result.Accepted);
            Assert.AreEqual(0, result.AwardedXp);
        }

        [Test]
        public void NullRequest_IsRejected()
        {
            var result = XpAwardPolicy.Resolve(ServerLesson(25), null);

            Assert.IsFalse(result.Accepted);
            Assert.AreEqual(0, result.AwardedXp);
        }
    }
}
