using System;
using System.Collections;
using System.IO;
using System.Linq;
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
    /// Mengunci perbaikan layout F1: daftar selalu mulai dari atas, teks
    /// multi-baris tidak terpotong, dan kartu tumbuh mengikuti isinya.
    /// </summary>
    public sealed class LayoutTests
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
                "uabmeto-layout-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);

            var topics = Resources.Load<TextAsset>("Content/topics");
            Assert.IsNotNull(topics, "topics.json belum ada. Jalankan F1 Setup.");

            var catalog = ContentRepository.LoadFromText(
                topics.text,
                new[] { TestContent.LoadLessonAsInReview() });

            var profile = LocalProfile.Load(Path.Combine(_directory, "profile.json"));
            Assert.IsTrue(profile.TrySetNickname("Penguji", out var error), error);
            _progress = ProgressStore.Load(Path.Combine(_directory, "progress.json"));
            _outbox = OutboxStore.Load(Path.Combine(_directory, "outbox.json"));

            _root = new GameObject("LayoutTest");
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
        public IEnumerator Home_ScrollStartsAtTop()
        {
            var scroll = FindScroll("HomeScroll");
            scroll.verticalNormalizedPosition = 0f;
            yield return null;

            _shell.ShowTab(ShellView.Tab.Home);
            yield return null;

            scroll = FindScroll("HomeScroll");
            Assert.AreEqual(
                1f,
                scroll.verticalNormalizedPosition,
                0.001f,
                "Beranda harus kembali ke posisi paling atas setelah dibangun ulang.");
        }

        [UnityTest]
        public IEnumerator Lesson_ScrollStartsAtTopOnEveryStep()
        {
            Click("Start");
            yield return null;
            Click("Primary");
            yield return null;

            var scroll = FindScroll("LessonScroll");
            Assert.AreEqual(
                1f,
                scroll.verticalNormalizedPosition,
                0.001f,
                "Pelajaran harus terbuka dari langkah pembuka, bukan dari tengah.");

            scroll.verticalNormalizedPosition = 0f;
            yield return null;

            Click("StartMaterial");
            yield return null;

            scroll = FindScroll("LessonScroll");
            Assert.AreEqual(
                1f,
                scroll.verticalNormalizedPosition,
                0.001f,
                "Pindah langkah harus menggulir kembali ke atas.");
        }

        [UnityTest]
        public IEnumerator HomeAndLesson_TextFitsInsideItsRect()
        {
            _shell.RebuildLayout();
            yield return null;
            var checkedOnHome = AssertNoClippedText("Beranda");
            Assert.Greater(checkedOnHome, 5, "Tidak cukup teks yang diperiksa di Beranda.");

            Click("Start");
            yield return null;
            Click("Primary");
            yield return null;
            Click("StartMaterial");
            yield return null;
            _shell.RebuildLayout();
            yield return null;

            var checkedInLesson = AssertNoClippedText("Materi");
            Assert.Greater(checkedInLesson, 5, "Tidak cukup teks yang diperiksa di materi.");
        }

        [UnityTest]
        public IEnumerator VocabularyCards_GrowWithTheirContent()
        {
            Click("Start");
            yield return null;
            Click("Primary");
            yield return null;
            Click("StartMaterial");
            yield return null;
            _shell.RebuildLayout();
            yield return null;

            var cards = _shell.ContentHost
                .GetComponentsInChildren<RectTransform>(true)
                .Where(rect => rect.name.StartsWith("Vocab_"))
                .ToArray();
            Assert.Greater(cards.Length, 0, "Kartu kosakata tidak ditemukan.");

            foreach (var card in cards)
            {
                var body = card.Find("Body") as RectTransform;
                Assert.IsNotNull(body, "Body kartu kosakata tidak ditemukan: " + card.name);
                var preferred = LayoutUtility.GetPreferredHeight(body);
                Assert.GreaterOrEqual(
                    card.rect.height + 1.5f,
                    preferred,
                    $"Kartu {card.name} lebih pendek dari isinya " +
                    $"(tinggi {card.rect.height:F1}, butuh {preferred:F1}).");
            }
        }

        [UnityTest]
        public IEnumerator NicknameModal_FitsReferenceViewport()
        {
            _shell.ShowNicknameDialog();
            yield return null;
            _shell.RebuildLayout();
            yield return null;

            var card = GameObject.Find("NicknameModal/Card");
            Assert.IsNotNull(card, "Kartu dialog nama tidak ditemukan.");
            var rect = card.GetComponent<RectTransform>();
            Assert.LessOrEqual(rect.rect.width, 360f, "Dialog lebih lebar dari acuan 360.");
            Assert.LessOrEqual(rect.rect.height, 640f, "Dialog lebih tinggi dari acuan 640.");
            Assert.Greater(rect.rect.height, 100f, "Dialog tidak mendapat tinggi dari isinya.");
        }

        private int AssertNoClippedText(string context)
        {
            var checkedCount = 0;
            var texts = _shell.ContentHost.GetComponentsInChildren<TextMeshProUGUI>(false);
            foreach (var text in texts)
            {
                if (string.IsNullOrEmpty(text.text))
                {
                    continue;
                }

                var rect = text.rectTransform.rect;
                if (rect.width < 1f || rect.height < 1f)
                {
                    continue;
                }

                checkedCount++;
                var preferred = text.GetPreferredValues(text.text, rect.width, 0f);
                Assert.LessOrEqual(
                    preferred.y,
                    rect.height + 1.5f,
                    $"{context}: teks terpotong \"{text.text}\" " +
                    $"(butuh {preferred.y:F1}, tersedia {rect.height:F1}) di {PathOf(text.transform)}.");
            }

            return checkedCount;
        }

        private static ScrollRect FindScroll(string name)
        {
            var target = GameObject.Find(name);
            Assert.IsNotNull(target, "Scroll view tidak ditemukan: " + name);
            var scroll = target.GetComponent<ScrollRect>();
            Assert.IsNotNull(scroll, "ScrollRect tidak ditemukan: " + name);
            return scroll;
        }

        private static string PathOf(Transform transform)
        {
            var path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
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
