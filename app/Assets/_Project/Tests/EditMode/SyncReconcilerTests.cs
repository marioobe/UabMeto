using System.Collections.Generic;
using NUnit.Framework;
using UabMeto.Core.Sync;

namespace UabMeto.Tests.EditMode
{
    /// <summary>
    /// Menguji keputusan rekonsiliasi dan skenario kegagalan antara penulisan D1
    /// dan Firestore. Fokus: operasi yang dikirim berulang tidak menggandakan progres.
    /// </summary>
    public sealed class SyncReconcilerTests
    {
        private const string OperationId = "op-0001";
        private const int XpReward = 10;

        [Test]
        public void PendingWithoutEffect_ReservesThenApplies()
        {
            var record = new SyncOperationRecord
            {
                OperationId = OperationId,
                State = SyncOperationState.Pending,
                FirestoreHasEffect = false
            };

            Assert.AreEqual(SyncAction.ReserveThenApply, SyncReconciler.Decide(record));
        }

        [Test]
        public void ReservedWithoutEffect_AppliesToFirestore()
        {
            var record = new SyncOperationRecord
            {
                OperationId = OperationId,
                State = SyncOperationState.Reserved,
                FirestoreHasEffect = false
            };

            Assert.AreEqual(SyncAction.ApplyToFirestore, SyncReconciler.Decide(record));
        }

        [Test]
        public void ReservedWithEffect_OnlyConfirms()
        {
            var record = new SyncOperationRecord
            {
                OperationId = OperationId,
                State = SyncOperationState.Reserved,
                FirestoreHasEffect = true
            };

            Assert.AreEqual(SyncAction.MarkConfirmed, SyncReconciler.Decide(record));
        }

        [Test]
        public void ConfirmedWithEffect_ReturnsCachedResult()
        {
            var record = new SyncOperationRecord
            {
                OperationId = OperationId,
                State = SyncOperationState.Confirmed,
                FirestoreHasEffect = true
            };

            Assert.AreEqual(SyncAction.ReturnCachedResult, SyncReconciler.Decide(record));
        }

        [Test]
        public void ConfirmedWithoutEffect_HaltsForReconciliation()
        {
            var record = new SyncOperationRecord
            {
                OperationId = OperationId,
                State = SyncOperationState.Confirmed,
                FirestoreHasEffect = false
            };

            Assert.AreEqual(SyncAction.HaltForReconciliation, SyncReconciler.Decide(record));
        }

        [Test]
        public void PayloadMismatch_HaltsForReconciliation()
        {
            var record = new SyncOperationRecord
            {
                OperationId = OperationId,
                State = SyncOperationState.Reserved,
                FirestoreHasEffect = true,
                PayloadMatchesAppliedEffect = false
            };

            Assert.AreEqual(SyncAction.HaltForReconciliation, SyncReconciler.Decide(record));
        }

        [Test]
        public void InvalidPayload_IsRejected()
        {
            var record = new SyncOperationRecord
            {
                OperationId = OperationId,
                PayloadValid = false
            };

            Assert.AreEqual(SyncAction.Reject, SyncReconciler.Decide(record));
        }

        [Test]
        public void AttemptsExhausted_StopsRetry()
        {
            var record = new SyncOperationRecord
            {
                OperationId = OperationId,
                State = SyncOperationState.Reserved,
                AttemptCount = 8,
                MaxAttempts = 8
            };

            Assert.AreEqual(SyncAction.Exhausted, SyncReconciler.Decide(record));
        }

        [Test]
        public void CrashAfterD1Reserve_DoesNotDuplicateXp()
        {
            var backend = new FakeSyncBackend();

            // Jendela kegagalan 1: D1 reserved, proses berhenti sebelum Firestore.
            backend.Reserve(OperationId);

            ExecuteOnce(backend, OperationId, XpReward);

            Assert.AreEqual(XpReward, backend.Xp);
            Assert.AreEqual(SyncOperationState.Confirmed, backend.StateOf(OperationId));
        }

        [Test]
        public void CrashAfterFirestoreWrite_DoesNotDuplicateXp()
        {
            var backend = new FakeSyncBackend();

            // Jendela kegagalan 2: Firestore sudah tertulis, D1 belum dikonfirmasi.
            var firstAction = ExecuteOnce(backend, OperationId, XpReward, crashAfterFirestoreWrite: true);
            Assert.AreEqual(SyncAction.ReserveThenApply, firstAction);
            Assert.AreEqual(XpReward, backend.Xp);

            var retryAction = ExecuteOnce(backend, OperationId, XpReward);

            Assert.AreEqual(SyncAction.MarkConfirmed, retryAction);
            Assert.AreEqual(XpReward, backend.Xp, "XP tidak boleh bertambah pada retry.");
            Assert.AreEqual(SyncOperationState.Confirmed, backend.StateOf(OperationId));
        }

        [Test]
        public void DuplicateDeliveryAfterConfirm_ReturnsCachedWithoutReward()
        {
            var backend = new FakeSyncBackend();

            ExecuteOnce(backend, OperationId, XpReward);
            var duplicateAction = ExecuteOnce(backend, OperationId, XpReward);

            Assert.AreEqual(SyncAction.ReturnCachedResult, duplicateAction);
            Assert.AreEqual(XpReward, backend.Xp);
        }

        [Test]
        public void LostD1RecordButEffectExists_ConfirmsInsteadOfReapplying()
        {
            var backend = new FakeSyncBackend();
            backend.ApplyDirectly(OperationId, XpReward);

            var action = ExecuteOnce(backend, OperationId, XpReward);

            Assert.AreEqual(SyncAction.MarkConfirmed, action);
            Assert.AreEqual(XpReward, backend.Xp);
        }

        private static SyncAction ExecuteOnce(
            FakeSyncBackend backend,
            string operationId,
            int xp,
            bool crashAfterFirestoreWrite = false)
        {
            var record = new SyncOperationRecord
            {
                OperationId = operationId,
                State = backend.StateOf(operationId),
                FirestoreHasEffect = backend.HasEffect(operationId),
                PayloadValid = true,
                PayloadMatchesAppliedEffect = true,
                AttemptCount = 0,
                MaxAttempts = 8
            };

            var action = SyncReconciler.Decide(record);

            switch (action)
            {
                case SyncAction.ReserveThenApply:
                    backend.Reserve(operationId);
                    backend.Apply(operationId, xp);
                    if (!crashAfterFirestoreWrite)
                    {
                        backend.Confirm(operationId);
                    }
                    break;

                case SyncAction.ApplyToFirestore:
                    backend.Apply(operationId, xp);
                    if (!crashAfterFirestoreWrite)
                    {
                        backend.Confirm(operationId);
                    }
                    break;

                case SyncAction.MarkConfirmed:
                    backend.Confirm(operationId);
                    break;
            }

            return action;
        }

        /// <summary>
        /// Tiruan backend dua sistem: himpunan reserved/confirmed (D1) dan
        /// efek yang diterapkan (Firestore).
        /// </summary>
        private sealed class FakeSyncBackend
        {
            private readonly HashSet<string> reserved = new HashSet<string>();
            private readonly HashSet<string> confirmed = new HashSet<string>();
            private readonly HashSet<string> applied = new HashSet<string>();

            public int Xp { get; private set; }

            public SyncOperationState StateOf(string operationId)
            {
                if (confirmed.Contains(operationId))
                {
                    return SyncOperationState.Confirmed;
                }

                return reserved.Contains(operationId)
                    ? SyncOperationState.Reserved
                    : SyncOperationState.Pending;
            }

            public bool HasEffect(string operationId)
            {
                return applied.Contains(operationId);
            }

            public void Reserve(string operationId)
            {
                reserved.Add(operationId);
            }

            public void Apply(string operationId, int xp)
            {
                if (!applied.Add(operationId))
                {
                    return;
                }

                Xp += xp;
            }

            public void ApplyDirectly(string operationId, int xp)
            {
                Apply(operationId, xp);
            }

            public void Confirm(string operationId)
            {
                reserved.Remove(operationId);
                confirmed.Add(operationId);
            }
        }
    }
}
