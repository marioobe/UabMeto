using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UabMeto.Core.Sync;
using UnityEngine;

namespace UabMeto.App.Local
{
    [Serializable]
    public sealed class OutboxItem
    {
        public string operationId;
        public string lessonId;
        public string contentVersion;
        public int clientClaimedXp;
        public string createdAtUtc;
        public string state;
        public int attemptCount;
        public int maxAttempts;
    }

    [Serializable]
    public sealed class OutboxData
    {
        public List<OutboxItem> items = new List<OutboxItem>();
    }

    /// <summary>
    /// Antrean hasil pelajaran di perangkat. F1 hanya menyimpan dan
    /// merekonsiliasi status; transport jaringan belum ada.
    /// </summary>
    public sealed class OutboxStore
    {
        public const string PendingState = "Pending";
        public const string ReservedState = "Reserved";
        public const string ConfirmedState = "Confirmed";
        public const string RejectedState = "Rejected";
        public const string FailedState = "Failed";

        private readonly string _path;

        private OutboxStore(string path, OutboxData data)
        {
            _path = path;
            Data = data ?? new OutboxData();
            if (Data.items == null)
            {
                Data.items = new List<OutboxItem>();
            }
        }

        public OutboxData Data { get; }

        public int Count => Data.items.Count;

        public int PendingCount => Data.items.Count(item => item.state == PendingState);

        public static OutboxStore LoadDefault()
        {
            var directory = Path.Combine(Application.persistentDataPath, "uabmeto");
            return Load(Path.Combine(directory, "outbox.json"));
        }

        public static OutboxStore Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var data = JsonUtility.FromJson<OutboxData>(File.ReadAllText(path));
                    if (data != null)
                    {
                        return new OutboxStore(path, data);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("[UabMeto] Outbox rusak, dibuat baru: " + exception.Message);
                }
            }

            return new OutboxStore(path, new OutboxData());
        }

        public OutboxItem Enqueue(string lessonId, string contentVersion, int clientClaimedXp)
        {
            var item = new OutboxItem
            {
                operationId = Guid.NewGuid().ToString("N"),
                lessonId = lessonId,
                contentVersion = contentVersion,
                clientClaimedXp = Math.Max(0, clientClaimedXp),
                createdAtUtc = DateTime.UtcNow.ToString("O"),
                state = PendingState,
                attemptCount = 0,
                maxAttempts = 8
            };

            Data.items.Add(item);
            Save();
            return item;
        }

        public SyncAction Reconcile(string operationId, SyncOperationRecord record)
        {
            var item = Data.items.FirstOrDefault(entry => entry.operationId == operationId);
            if (item == null || record == null)
            {
                return SyncAction.HaltForReconciliation;
            }

            record.OperationId = operationId;
            record.AttemptCount = item.attemptCount;
            record.MaxAttempts = item.maxAttempts;

            var action = SyncReconciler.Decide(record);
            switch (action)
            {
                case SyncAction.Reject:
                    item.state = RejectedState;
                    break;
                case SyncAction.HaltForReconciliation:
                case SyncAction.Exhausted:
                    item.state = FailedState;
                    break;
                case SyncAction.MarkConfirmed:
                case SyncAction.ReturnCachedResult:
                    item.state = ConfirmedState;
                    break;
                case SyncAction.ApplyToFirestore:
                    item.state = ReservedState;
                    item.attemptCount += 1;
                    break;
                case SyncAction.ReserveThenApply:
                    item.state = ReservedState;
                    item.attemptCount += 1;
                    break;
                case SyncAction.SendOperation:
                    item.state = PendingState;
                    item.attemptCount += 1;
                    break;
            }

            Save();
            return action;
        }

        public void Save()
        {
            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(_path, JsonUtility.ToJson(Data, true));
        }
    }
}
