using System;
using System.IO;
using NUnit.Framework;
using UabMeto.App.Local;
using UabMeto.Core.Sync;

namespace UabMeto.Tests.EditMode
{
    public sealed class LocalStoreTests
    {
        private string _directory;

        [SetUp]
        public void SetUp()
        {
            _directory = Path.Combine(
                Path.GetTempPath(),
                "uabmeto-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        [Test]
        public void Nickname_RequiresThreeToTwentyCharacters()
        {
            Assert.IsNotNull(LocalProfile.ValidateNickname("ab"));
            Assert.IsNull(LocalProfile.ValidateNickname("abc"));
            Assert.IsNull(LocalProfile.ValidateNickname("Nama Pelajar"));
            Assert.IsNotNull(LocalProfile.ValidateNickname(new string('a', 21)));
            Assert.IsNotNull(LocalProfile.ValidateNickname("a\tb"));
        }

        [Test]
        public void Nickname_IsPersistedLocally()
        {
            var path = Path.Combine(_directory, "profile.json");
            var profile = LocalProfile.Load(path);
            Assert.IsFalse(profile.IsValid);

            Assert.IsTrue(profile.TrySetNickname("  Maria  ", out var error));
            Assert.IsNull(error);
            Assert.AreEqual("Maria", profile.Nickname);

            var reloaded = LocalProfile.Load(path);
            Assert.AreEqual("Maria", reloaded.Nickname);
            Assert.IsTrue(reloaded.IsValid);
        }

        [Test]
        public void Progress_RecordsPendingXpNotAwardedXp()
        {
            var path = Path.Combine(_directory, "progress.json");
            var progress = ProgressStore.Load(path);
            Assert.AreEqual(0, progress.PendingXp);

            progress.RecordCompletion("lesson-001", 10);
            progress.RecordCompletion("lesson-001", 10);
            progress.RecordCompletion("lesson-002", 15);

            Assert.AreEqual(25, progress.PendingXp);
            Assert.AreEqual(2, progress.Data.completedLessonIds.Count);

            var reloaded = ProgressStore.Load(path);
            Assert.AreEqual(25, reloaded.PendingXp);
        }

        [Test]
        public void Outbox_EnqueuesBeforeSync()
        {
            var path = Path.Combine(_directory, "outbox.json");
            var outbox = OutboxStore.Load(path);
            var item = outbox.Enqueue("lesson-001", "1.1.0", 10);

            Assert.AreEqual(1, outbox.Count);
            Assert.AreEqual(1, outbox.PendingCount);
            Assert.AreEqual(OutboxStore.PendingState, item.state);
            Assert.AreEqual("1.1.0", item.contentVersion);
            Assert.AreEqual(10, item.clientClaimedXp);
            Assert.IsFalse(string.IsNullOrWhiteSpace(item.operationId));

            var reloaded = OutboxStore.Load(path);
            Assert.AreEqual(1, reloaded.Count);
        }

        [Test]
        public void Outbox_ReconcilesPendingToConfirmed()
        {
            var outbox = OutboxStore.Load(Path.Combine(_directory, "outbox.json"));
            var item = outbox.Enqueue("lesson-001", "1.1.0", 10);

            var pending = new SyncOperationRecord
            {
                State = SyncOperationState.Pending,
                PayloadValid = true,
                FirestoreHasEffect = false
            };
            Assert.AreEqual(SyncAction.ReserveThenApply, outbox.Reconcile(item.operationId, pending));
            Assert.AreEqual(OutboxStore.ReservedState, item.state);
            Assert.AreEqual(1, item.attemptCount);

            var reserved = new SyncOperationRecord
            {
                State = SyncOperationState.Reserved,
                PayloadValid = true,
                FirestoreHasEffect = true,
                PayloadMatchesAppliedEffect = true
            };
            Assert.AreEqual(SyncAction.MarkConfirmed, outbox.Reconcile(item.operationId, reserved));
            Assert.AreEqual(OutboxStore.ConfirmedState, item.state);
        }

        [Test]
        public void Outbox_RejectsInvalidPayload()
        {
            var outbox = OutboxStore.Load(Path.Combine(_directory, "outbox.json"));
            var item = outbox.Enqueue("lesson-001", "1.1.0", 10);

            var invalid = new SyncOperationRecord
            {
                PayloadValid = false,
                FirestoreHasEffect = false
            };
            Assert.AreEqual(SyncAction.Reject, outbox.Reconcile(item.operationId, invalid));
            Assert.AreEqual(OutboxStore.RejectedState, item.state);
        }

        [Test]
        public void Outbox_HaltsOnFailedState()
        {
            var outbox = OutboxStore.Load(Path.Combine(_directory, "outbox.json"));
            var item = outbox.Enqueue("lesson-001", "1.1.0", 10);

            var failed = new SyncOperationRecord
            {
                State = SyncOperationState.Failed,
                PayloadValid = true,
                FirestoreHasEffect = false
            };
            Assert.AreEqual(SyncAction.HaltForReconciliation, outbox.Reconcile(item.operationId, failed));
            Assert.AreEqual(OutboxStore.FailedState, item.state);
        }
    }
}
