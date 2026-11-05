using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using TMPro;
using UabMeto.App.Content;
using UabMeto.App.Local;
using UabMeto.App.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace UabMeto.Tests.PlayMode
{
    /// <summary>
    /// Mengunci perilaku pratinjau: label jelas, alur bisa dibuka, dan tidak
    /// ada progres/XP yang tercatat.
    /// </summary>
    public sealed class PreviewShellViewTests
    {
        private string _directory;
        private GameObject _root;
        private ProgressStore _progress;
        private OutboxStore _outbox;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _directory = Path.Combine(
                Path.GetTempPath(),
                "uabmeto-preview-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);

            var topics = Resources.Load<TextAsset>("Content/topics");
            Assert.IsNotNull(topics, "topics.json belum ada. Jalankan F1 Setup.");

            var lessonPath = Path.GetFullPath(
                Path.Combine(
                    Application.dataPath,
                    "..",
                    "..",
                    "content",
                    "lessons",
                    "lesson-001-salam.json"));
            Assert.IsTrue(File.Exists(lessonPath), "Pelajaran pratinjau tidak ditemukan: " + lessonPath);

            var catalog = ContentRepository.LoadFromText(
                topics.text,
                new[] { File.ReadAllText(lessonPath) });

            var profile = LocalProfile.Load(Path.Combine(_directory, "profile.json"));
            Assert.IsTrue(profile.TrySetNickname("Penguji", out var error), error);
            _progress = ProgressStore.Load(Path.Combine(_directory, "progress.json"));
            _outbox = OutboxStore.Load(Path.Combine(_directory, "outbox.json"));

            _root = new GameObject("PreviewShellViewTest");
            var shell = _root.AddComponent<ShellView>();
            shell.Initialize(ThemeTokens.Load(), catalog, profile, _progress, _outbox);

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
        public IEnumerator Home_ShowsPreviewBadgeAndTopicStatus()
        {
            Assert.IsNotNull(GameObject.Find("PreviewCard"), "Kartu pratinjau tidak tampil.");
            Assert.IsNull(GameObject.Find("EmptyState"), "Empty state tidak boleh menutupi pratinjau.");

            var badge = GameObject.Find("PreviewCard/Body/Badge")
                .GetComponent<TextMeshProUGUI>();
            StringAssert.Contains("PRATINJAU — BELUM DIVALIDASI", badge.text);

            var status = GameObject.Find("Topic_topic-01-salam/Row/Titles/Status")
                .GetComponent<TextMeshProUGUI>();
            StringAssert.Contains("Pratinjau", status.text);

            yield return null;
        }

        [UnityTest]
        public IEnumerator PreviewFlow_DoesNotRecordProgressOrXp()
        {
            Click("Start");
            yield return null;
            Assert.IsNotNull(GameObject.Find("Card"), "Modal pratinjau tidak muncul.");
            Click("Primary");
            yield return null;

            Assert.IsNotNull(GameObject.Find("PreviewNotice"), "Pemberitahuan pratinjau tidak muncul.");
            var badge = GameObject.Find("Badge").GetComponent<TextMeshProUGUI>();
            StringAssert.Contains("PRATINJAU — BELUM DIVALIDASI", badge.text);

            Click("StartMaterial");
            yield return null;
            Click("ToExercise");
            yield return null;

            Assert.IsNotNull(GameObject.Find("Option_opt-a"), "Opsi latihan tidak muncul.");
            Click("Option_opt-a");
            yield return null;
            Click("Check");
            yield return null;
            Click("Continue");
            yield return null;

            Assert.IsNotNull(GameObject.Find("PreviewClose"), "Tombol tutup pratinjau tidak muncul.");
            Assert.IsNull(GameObject.Find("Save"), "Pratinjau tidak boleh punya tombol simpan.");

            Click("PreviewClose");
            yield return null;

            Assert.AreEqual(0, _outbox.Count, "Pratinjau tidak boleh masuk outbox.");
            Assert.AreEqual(0, _progress.PendingXp, "Pratinjau tidak boleh menambah XP.");
            Assert.AreEqual(0, _progress.Data.completedLessonIds.Count, "Pratinjau tidak boleh menandai selesai.");
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
