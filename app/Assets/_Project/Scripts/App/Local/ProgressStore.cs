using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UabMeto.App.Local
{
    [Serializable]
    public sealed class ProgressData
    {
        public int pendingXp;
        public int streak;
        public int hearts = 5;
        public List<string> completedLessonIds = new List<string>();
    }

    /// <summary>
    /// Progres lokal untuk tampilan F1. XP di sini adalah XP tertunda, bukan
    /// XP yang sudah diberikan server. Firestore tetap sumber kebenaran.
    /// </summary>
    public sealed class ProgressStore
    {
        private readonly string _path;

        private ProgressStore(string path, ProgressData data)
        {
            _path = path;
            Data = data ?? new ProgressData();
            if (Data.completedLessonIds == null)
            {
                Data.completedLessonIds = new List<string>();
            }
        }

        public ProgressData Data { get; }

        public int PendingXp => Data.pendingXp;

        public int Streak => Data.streak;

        public int Hearts => Data.hearts;

        public static ProgressStore LoadDefault()
        {
            var directory = Path.Combine(Application.persistentDataPath, "uabmeto");
            return Load(Path.Combine(directory, "progress.json"));
        }

        public static ProgressStore Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var data = JsonUtility.FromJson<ProgressData>(File.ReadAllText(path));
                    if (data != null)
                    {
                        return new ProgressStore(path, data);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("[UabMeto] Progres lokal rusak, dibuat baru: " + exception.Message);
                }
            }

            return new ProgressStore(path, new ProgressData());
        }

        public void RecordCompletion(string lessonId, int displayXp)
        {
            if (string.IsNullOrWhiteSpace(lessonId))
            {
                return;
            }

            // XP lokal hanya proyeksi tertunda. Jangan menambah dua kali untuk
            // pelajaran yang sama; server tetap yang menentukan hadiah final.
            if (Data.completedLessonIds.Contains(lessonId))
            {
                return;
            }

            Data.completedLessonIds.Add(lessonId);
            Data.pendingXp += Math.Max(0, displayXp);
            Save();
        }

        public bool IsCompleted(string lessonId)
        {
            return Data.completedLessonIds.Contains(lessonId);
        }

        /// <summary>
        /// Menyelaraskan XP tertunda dengan isi outbox. Dipanggil saat aplikasi
        /// dibuka dan setelah sinkron agar angka tidak basi setelah server
        /// mengonfirmasi hasil.
        /// </summary>
        public void ReconcilePendingXp(int pendingXp)
        {
            var value = Math.Max(0, pendingXp);
            if (Data.pendingXp == value)
            {
                return;
            }

            Data.pendingXp = value;
            Save();
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
