using System;
using System.Collections;
using System.Collections.Generic;
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
    /// Menghasilkan tangkapan layar dengan merender canvas ke RenderTexture.
    /// Ini tidak bergantung pada Game View dan tetap gagal bila grafis tidak
    /// tersedia di lingkungan batch.
    /// </summary>
    public sealed class ScreenshotTests
    {
        private string _directory;
        private readonly List<GameObject> _roots = new List<GameObject>();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _directory = Path.Combine(
                Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..")),
                "Builds",
                "screenshots");
            Directory.CreateDirectory(_directory);
            foreach (var file in Directory.GetFiles(_directory, "shot-*.json"))
            {
                File.Delete(file);
            }

            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var root in _roots)
            {
                if (root != null)
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }

            _roots.Clear();
        }

        [UnityTest]
        public IEnumerator CaptureEmptyState_Portrait()
        {
            var shell = BuildShell(false, "EmptyPortrait", 360, 640);
            yield return Capture(shell, 360, 640, "f1-empty-360x640.png");
        }

        [UnityTest]
        public IEnumerator CaptureEmptyState_Compact()
        {
            var shell = BuildShell(false, "EmptyCompact", 412, 915);
            yield return Capture(shell, 412, 915, "f1-empty-412x915.png");
        }

        [UnityTest]
        public IEnumerator CaptureEmptyState_Landscape()
        {
            var shell = BuildShell(false, "EmptyLandscape", 915, 412);
            yield return Capture(shell, 915, 412, "f1-empty-915x412.png");
        }

        [UnityTest]
        public IEnumerator CaptureFixtureFlow_Portrait()
        {
            var shell = BuildShell(true, "FixturePortrait", 360, 640);
            yield return Capture(shell, 360, 640, "f1-fixture-home-360x640.png");

            Click("Start");
            yield return null;
            Click("Primary");
            yield return null;
            yield return Capture(shell, 360, 640, "f1-lesson-intro-360x640.png");

            Click("StartMaterial");
            yield return null;
            yield return Capture(shell, 360, 640, "f1-lesson-material-360x640.png");

            Click("ToExercise");
            yield return null;
            yield return Capture(shell, 360, 640, "f1-lesson-exercise-360x640.png");

            Click("Option_opt-a");
            yield return null;
            Click("Check");
            yield return null;
            yield return Capture(shell, 360, 640, "f1-lesson-feedback-360x640.png");

            Click("Continue");
            yield return null;
            yield return Capture(shell, 360, 640, "f1-lesson-summary-360x640.png");

            Click("Save");
            yield return null;
            yield return Capture(shell, 360, 640, "f1-after-completion-360x640.png");
        }

        [UnityTest]
        public IEnumerator CaptureFixtureFlow_Landscape()
        {
            var shell = BuildShell(true, "FixtureLandscape", 915, 412);
            yield return Capture(shell, 915, 412, "f1-fixture-home-915x412.png");

            Click("Start");
            yield return null;
            Click("Primary");
            yield return null;
            yield return Capture(shell, 915, 412, "f1-lesson-intro-915x412.png");
        }

        private ShellView BuildShell(bool includeFixture, string name, int width, int height)
        {
            Screen.SetResolution(width, height, false);

            var topics = Resources.Load<TextAsset>("Content/topics");
            Assert.IsNotNull(topics, "topics.json belum ada. Jalankan F1 Setup.");

            string fixtureJson = null;
            if (includeFixture)
            {
                var fixturePath = Path.GetFullPath(
                    Path.Combine(
                        Application.dataPath,
                        "..",
                        "..",
                        "content",
                        "samples",
                        "lesson-001-salam.UNVALIDATED.json"));
                Assert.IsTrue(File.Exists(fixturePath), "Fixture tidak ditemukan: " + fixturePath);
                fixtureJson = File.ReadAllText(fixturePath);
            }

            var catalog = ContentRepository.LoadFromText(
                topics.text,
                Array.Empty<string>(),
                fixtureJson);
            var profile = LocalProfile.Load(Path.Combine(_directory, "shot-profile.json"));
            Assert.IsTrue(profile.TrySetNickname("Penguji", out var error), error);
            var progress = ProgressStore.Load(Path.Combine(_directory, "shot-progress.json"));
            var outbox = OutboxStore.Load(Path.Combine(_directory, "shot-outbox.json"));

            var root = new GameObject("Screenshot_" + name);
            _roots.Add(root);
            var shell = root.AddComponent<ShellView>();
            shell.Initialize(ThemeTokens.Load(), catalog, profile, progress, outbox);
            return shell;
        }

        private IEnumerator Capture(ShellView shell, int width, int height, string fileName)
        {
            Screen.SetResolution(width, height, false);
            yield return null;
            yield return null;
            shell.RebuildLayout();

            var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            Assert.IsNotNull(canvas, "Canvas tidak ditemukan.");

            var path = Path.Combine(_directory, fileName);
            RenderCanvas(canvas, width, height, path);
            Assert.IsTrue(File.Exists(path), "Tangkapan layar tidak terbentuk: " + path);
        }

        private static void Click(string objectName)
        {
            var target = GameObject.Find(objectName);
            Assert.IsNotNull(target, "Tombol tidak ditemukan: " + objectName);
            var button = target.GetComponent<Button>();
            Assert.IsNotNull(button, "Komponen Button tidak ditemukan: " + objectName);
            button.onClick.Invoke();
        }

        private static void RenderCanvas(Canvas canvas, int width, int height, string path)
        {
            var cameraObject = new GameObject("ScreenshotCamera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = ThemeTokens.Hex("#0B1519");
            camera.orthographic = true;
            camera.orthographicSize = height / 2f;
            camera.aspect = (float)width / height;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;

            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            renderTexture.Create();
            camera.targetTexture = renderTexture;

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1f;

            Canvas.ForceUpdateCanvases();
            camera.Render();

            RenderTexture.active = renderTexture;
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());

            RenderTexture.active = null;
            camera.targetTexture = null;
            renderTexture.Release();

            UnityEngine.Object.DestroyImmediate(texture);
            UnityEngine.Object.DestroyImmediate(renderTexture);
            UnityEngine.Object.DestroyImmediate(cameraObject);
        }
    }
}
