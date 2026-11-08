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
    /// Mengunci jalur belajar Topik 1: daftar tiga pelajaran, kartu lanjutan
    /// yang maju ke pelajaran berikutnya, dan latihan ulang yang tidak
    /// menggandakan XP.
    /// </summary>
    public sealed class LessonListShellViewTests
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
                "uabmeto-lesson-list-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);

            var topics = Resources.Load<TextAsset>("Content/topics");
            Assert.IsNotNull(topics, "topics.json belum ada. Jalankan F1 Setup.");

            var catalog = ContentRepository.LoadFromText(
                topics.text,
                new[]
                {
                    TestContent.LoadLesson("lesson-001-salam.json"),
                    TestContent.LoadLesson("lesson-002-salam-bertemu.json"),
                    TestContent.LoadLesson("lesson-003-salam-sopan.json")
                });

            Assert.AreEqual(3, catalog.ValidatedLessons.Length, "Topik 1 harus punya 3 pelajaran.");

            var profile = LocalProfile.Load(Path.Combine(_directory, "profile.json"));
            Assert.IsTrue(profile.TrySetNickname("Penguji", out var error), error);
            _progress = ProgressStore.Load(Path.Combine(_directory, "progress.json"));
            _outbox = OutboxStore.Load(Path.Combine(_directory, "outbox.json"));

            _root = new GameObject("LessonListShellViewTest");
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
        public IEnumerator Home_ContinueCardShowsFirstLesson()
        {
            var title = GameObject.Find("LessonCard/Body/Title")
                .GetComponent<TextMeshProUGUI>();
            Assert.AreEqual("Sapaan waktu", title.text);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TopicRow_OpensListWithThreeLessons()
        {
            Click("Topic_topic-01-salam");
            yield return null;

            Assert.IsNotNull(GameObject.Find("LessonListScroll"), "Layar daftar pelajaran tidak muncul.");
            Assert.IsNotNull(GameObject.Find("Lesson_lesson-001-salam"));
            Assert.IsNotNull(GameObject.Find("Lesson_lesson-002-salam-bertemu"));
            Assert.IsNotNull(GameObject.Find("Lesson_lesson-003-salam-sopan"));

            var status = GameObject.Find("Lesson_lesson-001-salam/Body/Status")
                .GetComponent<TextMeshProUGUI>();
            Assert.AreEqual("TERVALIDASI", status.text);

            var start = GameObject.Find("Start_lesson-002-salam-bertemu");
            Assert.IsNotNull(start, "Tombol mulai pelajaran 2 tidak ada.");
        }

        [UnityTest]
        public IEnumerator ContinueCard_AdvancesToFirstIncompleteLesson()
        {
            _progress.RecordCompletion("lesson-001-salam", 10);
            _shell.ShowTab(ShellView.Tab.Home);
            yield return null;

            var title = GameObject.Find("LessonCard/Body/Title")
                .GetComponent<TextMeshProUGUI>();
            Assert.AreEqual("Bertemu dan berpisah", title.text);
        }

        [UnityTest]
        public IEnumerator Replay_DoesNotEnqueueAnotherResultOrXp()
        {
            CompleteLessonFromHome();
            Assert.AreEqual(1, _outbox.Count, "Penyelesaian pertama harus masuk outbox.");
            Assert.AreEqual(10, _progress.PendingXp);

            var nextTitle = GameObject.Find("LessonCard/Body/Title")
                .GetComponent<TextMeshProUGUI>();
            Assert.AreEqual("Bertemu dan berpisah", nextTitle.text);

            Click("Topic_topic-01-salam");
            yield return null;

            var status = GameObject.Find("Lesson_lesson-001-salam/Body/Status")
                .GetComponent<TextMeshProUGUI>();
            Assert.AreEqual("SELESAI", status.text);

            Click("Start_lesson-001-salam");
            yield return null;
            Click("StartMaterial");
            CompleteLessonSteps();

            Assert.IsNotNull(
                GameObject.Find("FinishRepeat"),
                "Ringkasan latihan ulang harus memakai tombol Selesai, bukan Simpan hasil.");
            Assert.IsNull(GameObject.Find("Save"), "Latihan ulang tidak boleh menawarkan simpan hasil.");

            Click("FinishRepeat");
            yield return null;

            Assert.AreEqual(1, _outbox.Count, "Latihan ulang tidak boleh menambah antrean.");
            Assert.AreEqual(10, _progress.PendingXp, "Latihan ulang tidak boleh menambah XP tertunda.");
            Assert.IsNotNull(
                GameObject.Find("LessonListScroll"),
                "Setelah selesai, pengguna harus kembali ke daftar pelajaran.");
        }

        private void CompleteLessonFromHome()
        {
            Click("Start");
            Click("StartMaterial");
            CompleteLessonSteps();
            Click("Save");
        }

        private static void CompleteLessonSteps()
        {
            Click("ToExercise");
            Click("Option_opt-a");
            Click("Check");
            Click("Continue");
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
