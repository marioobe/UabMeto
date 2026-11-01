using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UabMeto.App.Content;
using UabMeto.App.Local;
using UabMeto.App.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace UabMeto.Tests.PlayMode
{
    /// <summary>
    /// Mengukur target sentuh yang benar-benar dibangun di layar F1.
    /// Ini bukan audit TalkBack dan bukan pengganti uji perangkat.
    /// </summary>
    public sealed class AccessibilityTests
    {
        private string _directory;
        private GameObject _root;
        private ShellView _shell;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _directory = Path.Combine(
                Path.GetTempPath(),
                "uabmeto-a11y-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);

            var topics = Resources.Load<TextAsset>("Content/topics");
            Assert.IsNotNull(topics);
            var fixturePath = Path.GetFullPath(
                Path.Combine(
                    Application.dataPath,
                    "..",
                    "..",
                    "content",
                    "samples",
                    "lesson-001-salam.UNVALIDATED.json"));
            Assert.IsTrue(File.Exists(fixturePath));
            var catalog = ContentRepository.LoadFromText(
                topics.text,
                Array.Empty<string>(),
                File.ReadAllText(fixturePath));
            var profile = LocalProfile.Load(Path.Combine(_directory, "profile.json"));
            Assert.IsTrue(profile.TrySetNickname("Penguji", out var error), error);

            _root = new GameObject("AccessibilityTest");
            _shell = _root.AddComponent<ShellView>();
            _shell.Initialize(
                ThemeTokens.Load(),
                catalog,
                profile,
                ProgressStore.Load(Path.Combine(_directory, "progress.json")),
                OutboxStore.Load(Path.Combine(_directory, "outbox.json")));
            yield return null;
            _shell.RebuildLayout();
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
        public IEnumerator HomeAndProfile_TargetsAreAtLeast48Dp()
        {
            AssertTargets("Beranda");
            _shell.ShowTab(ShellView.Tab.Profile);
            yield return null;
            _shell.RebuildLayout();
            AssertTargets("Profil");
        }

        [UnityTest]
        public IEnumerator LessonAndModals_TargetsAreAtLeast48Dp()
        {
            _shell.ShowLesson(
                _shell.Catalog.DevelopmentFixture,
                true);
            yield return null;
            _shell.RebuildLayout();
            AssertTargets("Pembuka pelajaran");

            var start = GameObject.Find("StartMaterial");
            Assert.IsNotNull(start);
            start.GetComponent<Button>().onClick.Invoke();
            yield return null;
            _shell.RebuildLayout();
            AssertTargets("Materi pelajaran");

            var toExercise = GameObject.Find("ToExercise");
            Assert.IsNotNull(toExercise);
            toExercise.GetComponent<Button>().onClick.Invoke();
            yield return null;
            _shell.RebuildLayout();
            AssertTargets("Latihan");

            _shell.ShowNicknameDialog();
            yield return null;
            _shell.RebuildLayout();
            AssertTargets("Dialog nama panggilan");
        }

        private static void AssertTargets(string screen)
        {
            var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            Assert.IsNotNull(canvas, "Canvas tidak ditemukan: " + screen);
            var failures = canvas
                .GetComponentsInChildren<Selectable>(true)
                .Where(selectable => selectable.isActiveAndEnabled)
                .Select(selectable => new
                {
                    Selectable = selectable,
                    Rect = selectable.transform as RectTransform
                })
                .Where(item => item.Rect != null)
                .Where(item =>
                    item.Rect.rect.width < 47.5f ||
                    item.Rect.rect.height < 47.5f)
                .Select(item =>
                    $"{item.Selectable.name} {item.Rect.rect.width:F1}x{item.Rect.rect.height:F1}")
                .ToArray();

            Assert.IsEmpty(
                failures,
                $"Target sentuh < 48 dp di {screen}: {string.Join(", ", failures)}");
        }
    }
}
