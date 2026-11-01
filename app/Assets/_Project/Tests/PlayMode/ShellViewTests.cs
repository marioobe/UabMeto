using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using UabMeto.App.Content;
using UabMeto.App.Local;
using UabMeto.App.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace UabMeto.Tests.PlayMode
{
    public sealed class ShellViewTests
    {
        private string _directory;
        private GameObject _root;
        private ShellView _shell;
        private ProgressStore _progress;
        private OutboxStore _outbox;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _directory = Path.Combine(
                Path.GetTempPath(),
                "uabmeto-playmode-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);

            var topics = Resources.Load<TextAsset>("Content/topics");
            Assert.IsNotNull(topics, "topics.json belum ada. Jalankan F1 Setup.");

            var fixturePath = Path.GetFullPath(
                Path.Combine(
                    Application.dataPath,
                    "..",
                    "..",
                    "content",
                    "samples",
                    "lesson-001-salam.UNVALIDATED.json"));
            Assert.IsTrue(File.Exists(fixturePath), "Fixture tidak ditemukan: " + fixturePath);

            var catalog = ContentRepository.LoadFromText(
                topics.text,
                Array.Empty<string>(),
                File.ReadAllText(fixturePath));

            var profile = LocalProfile.Load(Path.Combine(_directory, "profile.json"));
            Assert.IsTrue(profile.TrySetNickname("Penguji", out var error), error);
            _progress = ProgressStore.Load(Path.Combine(_directory, "progress.json"));
            _outbox = OutboxStore.Load(Path.Combine(_directory, "outbox.json"));

            _root = new GameObject("ShellViewTest");
            _shell = _root.AddComponent<ShellView>();
            _shell.Initialize(ThemeTokens.Load(), catalog, profile, _progress, _outbox);

            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            if (_root != null)
            {
                UnityEngine.Object.DestroyImmediate(_root);
            }

            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        [UnityTest]
        public IEnumerator Shell_HasFourTabsAndTwelveTopics()
        {
            Assert.IsNotNull(GameObject.Find("Tab_Home"));
            Assert.IsNotNull(GameObject.Find("Tab_Tutor"));
            Assert.IsNotNull(GameObject.Find("Tab_Leaderboard"));
            Assert.IsNotNull(GameObject.Find("Tab_Profile"));

            var topics = new[]
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

            foreach (var topic in topics)
            {
                Assert.IsNotNull(
                    GameObject.Find("Topic_" + topic),
                    "Baris topik tidak ditemukan: " + topic);
            }

            Assert.IsNotNull(GameObject.Find("EmptyState"), "Empty state tidak tampil.");
            Assert.IsNotNull(GameObject.Find("LessonCard"), "Kartu fixture tidak tampil di Editor.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator FixtureFlow_EnqueuesCompletion()
        {
            Click("Start");
            yield return null;

            Assert.IsNotNull(GameObject.Find("Card"), "Modal fixture tidak muncul.");
            Click("Primary");
            yield return null;

            Assert.IsNotNull(GameObject.Find("StartMaterial"), "Pembuka pelajaran tidak muncul.");
            Click("StartMaterial");
            yield return null;

            Assert.IsNotNull(GameObject.Find("ToExercise"), "Materi tidak muncul.");
            Click("ToExercise");
            yield return null;

            Assert.IsNotNull(GameObject.Find("Option_opt-a"), "Opsi latihan tidak muncul.");
            Click("Option_opt-a");
            yield return null;

            Click("Check");
            yield return null;

            Assert.IsNotNull(GameObject.Find("Continue"), "Umpan balik tidak muncul.");
            Click("Continue");
            yield return null;

            Assert.IsNotNull(GameObject.Find("Save"), "Ringkasan tidak muncul.");
            Click("Save");
            yield return null;

            Assert.AreEqual(1, _outbox.Count);
            Assert.AreEqual(1, _outbox.PendingCount);
            Assert.AreEqual(10, _progress.PendingXp);
            Assert.IsNotNull(GameObject.Find("EmptyState"));
        }

        private static void Click(string objectName)
        {
            var target = GameObject.Find(objectName);
            Assert.IsNotNull(target, "Tombol tidak ditemukan: " + objectName);
            var button = target.GetComponent<Button>();
            Assert.IsNotNull(button, "Komponen Button tidak ditemukan: " + objectName);
            button.onClick.Invoke();
        }
    }
}
