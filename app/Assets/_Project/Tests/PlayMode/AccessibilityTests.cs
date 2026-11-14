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
        public IEnumerator Profile_IdentityCardFitsContentAndScrollIsDraggable()
        {
            _shell.ShowTab(ShellView.Tab.Profile);
            yield return null;
            _shell.RebuildLayout();

            var identity = GameObject.Find("Identity").GetComponent<RectTransform>();
            var avatar = GameObject.Find("Avatar").GetComponent<RectTransform>();
            var nickname = GameObject.Find("Nickname").GetComponent<RectTransform>();
            var note = GameObject.Find("Note").GetComponent<RectTransform>();
            var edit = GameObject.Find("Edit").GetComponent<RectTransform>();

            Assert.GreaterOrEqual(
                identity.rect.height,
                200f,
                "Kartu identitas terlalu pendek; teks akan bertumpuk.");
            Assert.LessOrEqual(
                avatar.rect.width,
                80f,
                "Avatar melebar mengikuti lebar kartu, bukan lingkaran 64 dp.");

            var nicknameBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(identity, nickname);
            var noteBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(identity, note);
            var editBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(identity, edit);
            Assert.GreaterOrEqual(
                nicknameBounds.min.y,
                noteBounds.max.y - 0.5f,
                "Nickname dan catatan bertumpuk.");
            Assert.GreaterOrEqual(
                noteBounds.min.y,
                editBounds.max.y - 0.5f,
                "Catatan dan tombol Ubah nama bertumpuk.");
            Assert.GreaterOrEqual(
                editBounds.min.y,
                identity.rect.yMin - 0.5f,
                "Tombol Ubah nama keluar dari kartu.");
            Assert.LessOrEqual(
                editBounds.max.y,
                identity.rect.yMax + 0.5f,
                "Tombol Ubah nama keluar dari kartu.");

            var viewport = GameObject.Find("ProfileScroll")
                .transform.Find("Viewport")
                .GetComponent<RectTransform>();
            var hitArea = viewport.GetComponent<Graphic>();
            Assert.IsNotNull(hitArea, "Viewport scroll tidak punya Graphic raycast target.");
            Assert.IsTrue(hitArea.raycastTarget, "Viewport scroll tidak menerima drag.");
        }

        [UnityTest]
        public IEnumerator NicknameDialog_FitsCardAndHasCancel()
        {
            _shell.ShowNicknameDialog();
            yield return null;
            _shell.RebuildLayout();

            var modal = GameObject.Find("NicknameModal");
            Assert.IsNotNull(modal);
            var card = modal.transform.Find("Card").GetComponent<RectTransform>();
            var content = card.Find("Viewport/Content");
            var title = content.Find("Title").GetComponent<RectTransform>();
            var body = content.Find("Body").GetComponent<RectTransform>();
            var input = content.Find("Nickname").GetComponent<RectTransform>();
            var error = content.Find("Error").GetComponent<RectTransform>();
            var cancel = content.Find("Cancel").GetComponent<RectTransform>();
            var save = content.Find("Save").GetComponent<RectTransform>();

            Assert.GreaterOrEqual(card.rect.height, 280f, "Kartu dialog terlalu pendek.");
            Assert.LessOrEqual(card.rect.height, 520f, "Kartu dialog keluar dari layar 360x640.");

            var titleBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(card, title);
            var bodyBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(card, body);
            var inputBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(card, input);
            var errorBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(card, error);
            var cancelBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(card, cancel);
            var saveBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(card, save);

            Assert.GreaterOrEqual(titleBounds.min.y, bodyBounds.max.y - 0.5f, "Judul dan isi dialog bertumpuk.");
            Assert.GreaterOrEqual(bodyBounds.min.y, inputBounds.max.y - 0.5f, "Isi dan input dialog bertumpuk.");
            Assert.GreaterOrEqual(inputBounds.min.y, errorBounds.max.y - 0.5f, "Input dan pesan galat bertumpuk.");
            Assert.GreaterOrEqual(errorBounds.min.y, cancelBounds.max.y - 0.5f, "Pesan galat dan Batal bertumpuk.");
            Assert.GreaterOrEqual(cancelBounds.min.y, saveBounds.max.y - 0.5f, "Batal dan Simpan bertumpuk.");
            Assert.GreaterOrEqual(saveBounds.min.y, card.rect.yMin - 0.5f, "Tombol Simpan keluar dari kartu.");
            Assert.LessOrEqual(saveBounds.max.y, card.rect.yMax + 0.5f, "Tombol Simpan keluar dari kartu.");
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
