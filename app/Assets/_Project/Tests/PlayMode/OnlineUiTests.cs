using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using TMPro;
using UabMeto.App.Content;
using UabMeto.App.Local;
using UabMeto.App.Net;
using UabMeto.App.UI;
using UabMeto.Core.Sync;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace UabMeto.Tests.PlayMode
{
    /// <summary>
    /// Menguji UI mode online lokal: kartu profil, banner, dan tab Peringkat.
    /// Semua respons server berasal dari transport palsu; tidak ada jaringan.
    /// </summary>
    public sealed class OnlineUiTests
    {
        private const string BaseUrl = "http://127.0.0.1:8787";

        private string _directory;
        private GameObject _root;
        private ShellView _shell;
        private ContentCatalog _catalog;
        private LocalProfile _profile;
        private ProgressStore _progress;
        private OutboxStore _outbox;
        private PlayModeFakeTransport _transport;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _directory = Path.Combine(
                Path.GetTempPath(),
                "uabmeto-online-ui-" + Guid.NewGuid().ToString("N"));
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
            Assert.IsTrue(
                File.Exists(fixturePath),
                "Fixture tidak ditemukan: " + fixturePath);

            _catalog = ContentRepository.LoadFromText(
                topics.text,
                Array.Empty<string>(),
                File.ReadAllText(fixturePath));

            _profile = LocalProfile.Load(Path.Combine(_directory, "profile.json"));
            Assert.IsTrue(
                _profile.TrySetNickname("Penguji", out var error),
                error);

            _progress = ProgressStore.Load(Path.Combine(_directory, "progress.json"));
            _outbox = OutboxStore.Load(Path.Combine(_directory, "outbox.json"));
            _transport = new PlayModeFakeTransport();

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

        private void BuildShell(bool online)
        {
            _root = new GameObject("OnlineUiTest");
            _shell = _root.AddComponent<ShellView>();

            if (online)
            {
                var api = new ApiClient(_transport, BaseUrl, "dev-user");
                var sync = new SyncService(api, _outbox, () => "Penguji");
                _shell.Initialize(
                    ThemeTokens.Load(),
                    _catalog,
                    _profile,
                    _progress,
                    _outbox,
                    api,
                    sync);
            }
            else
            {
                _shell.Initialize(
                    ThemeTokens.Load(),
                    _catalog,
                    _profile,
                    _progress,
                    _outbox);
            }
        }

        private static TextMeshProUGUI FindText(string name)
        {
            var go = GameObject.Find(name);
            Assert.IsNotNull(go, "Tidak menemukan objek " + name);
            var text = go.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(text, name + " bukan teks.");
            return text;
        }

        private static TextMeshProUGUI FindChildText(string parent, string child)
        {
            var parentObject = GameObject.Find(parent);
            Assert.IsNotNull(parentObject, "Tidak menemukan objek " + parent);
            var childTransform = parentObject.transform.Find(child);
            Assert.IsNotNull(childTransform, "Tidak menemukan " + parent + "/" + child);
            return childTransform.GetComponent<TextMeshProUGUI>();
        }

        [UnityTest]
        public IEnumerator Profil_Nonaktif_MenampilkanKartuTanpaTombolSinkron()
        {
            BuildShell(false);
            _shell.ShowTab(ShellView.Tab.Profile);
            yield return null;

            Assert.IsNotNull(GameObject.Find("OnlineStatus"));
            StringAssert.Contains("Nonaktif", FindText("OnlineState").text);
            Assert.IsNull(GameObject.Find("SyncNow"));
        }

        [UnityTest]
        public IEnumerator Profil_Aktif_MenampilkanAlamatDanTombolSinkron()
        {
            BuildShell(true);
            _shell.ShowTab(ShellView.Tab.Profile);
            yield return null;

            StringAssert.Contains("Aktif", FindText("OnlineState").text);
            StringAssert.Contains(BaseUrl, FindText("OnlineState").text);
            Assert.IsNotNull(GameObject.Find("SyncNow"));
        }

        [UnityTest]
        public IEnumerator Sinkron_MengirimOutboxDanMenampilkanHasil()
        {
            BuildShell(true);
            var item = _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            _transport.Respond(
                200,
                "{\"state\":\"confirmed\",\"awardedXp\":10," +
                "\"duplicate\":false,\"devMode\":false}");

            _shell.ShowTab(ShellView.Tab.Profile);
            yield return null;

            GameObject.Find("SyncNow").GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.AreEqual(OutboxStore.ConfirmedState, item.state);
            Assert.AreEqual(10, item.awardedXp);
            Assert.AreEqual(0, _outbox.PendingXpTotal);
            Assert.AreEqual(0, _progress.PendingXp);
            StringAssert.Contains(
                "Sinkron selesai",
                FindText("OnlineSyncMessage").text);
        }

        [UnityTest]
        public IEnumerator Peringkat_Nonaktif_TidakMenampilkanEntriReaan()
        {
            BuildShell(false);
            _shell.ShowTab(ShellView.Tab.Leaderboard);
            yield return null;

            StringAssert.Contains(
                "nonaktif",
                FindText("LeaderboardStatus").text);
            Assert.IsNull(GameObject.Find("LoadLeaderboard"));
            Assert.IsNull(GameObject.Find("Entry_1"));
        }

        [UnityTest]
        public IEnumerator Peringkat_MemuatEntriDariServerLokal()
        {
            BuildShell(true);
            _transport.Respond(
                200,
                "{\"weekKey\":\"2026-11-02\",\"entries\":[" +
                "{\"userId\":\"dev-user-2\",\"nickname\":\"Aina\",\"xp\":20}," +
                "{\"userId\":\"dev-user\",\"nickname\":\"Penguji\",\"xp\":10}]}");

            _shell.ShowTab(ShellView.Tab.Leaderboard);
            yield return null;

            GameObject.Find("LoadLeaderboard").GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.IsNotNull(GameObject.Find("Entry_1"));
            Assert.IsNotNull(GameObject.Find("Entry_2"));
            Assert.AreEqual("Aina", FindChildText("Entry_1", "Name").text);
            Assert.AreEqual("20 XP", FindChildText("Entry_1", "Xp").text);
            StringAssert.Contains("2026-11-02", FindText("LeaderboardStatus").text);
        }

        [UnityTest]
        public IEnumerator Peringkat_GalatKoneksi_MenampilkanPesanBahasaManusia()
        {
            BuildShell(true);
            _transport.Fail("connection refused");

            _shell.ShowTab(ShellView.Tab.Leaderboard);
            yield return null;

            GameObject.Find("LoadLeaderboard").GetComponent<Button>().onClick.Invoke();
            yield return null;

            var status = FindText("LeaderboardStatus");
            StringAssert.Contains("Tidak dapat menghubungi", status.text);
            Assert.AreEqual(ThemeTokens.Load().DangerText, status.color);
            Assert.IsNull(GameObject.Find("Entry_1"));
        }

        [UnityTest]
        public IEnumerator Banner_MenampilkanHasilMenungguSinkron()
        {
            BuildShell(false);
            _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            yield return null;

            Assert.IsTrue(_shell.BannerVisible);
            StringAssert.Contains("menunggu sinkron", _shell.BannerText);
        }

        [UnityTest]
        public IEnumerator Banner_MenampilkanKegagalanSinkron()
        {
            BuildShell(true);
            _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            _transport.Fail("connection refused");

            _shell.ShowTab(ShellView.Tab.Profile);
            yield return null;

            GameObject.Find("SyncNow").GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.IsTrue(_shell.BannerVisible);
            StringAssert.Contains("tidak dapat dihubungi", _shell.BannerText);
        }

        private sealed class PlayModeFakeTransport : IHttpTransport
        {
            private readonly Queue<HttpResult> _responses = new Queue<HttpResult>();

            public int CallCount { get; private set; }

            public PlayModeFakeTransport Respond(int statusCode, string body)
            {
                _responses.Enqueue(HttpResult.FromResponse(statusCode, body));
                return this;
            }

            public PlayModeFakeTransport Fail(string error)
            {
                _responses.Enqueue(HttpResult.Failure(error));
                return this;
            }

            public Task<HttpResult> SendAsync(
                string method,
                string url,
                string jsonBody,
                IReadOnlyDictionary<string, string> headers,
                CancellationToken cancellationToken)
            {
                CallCount += 1;
                if (_responses.Count == 0)
                {
                    throw new InvalidOperationException(
                        "Tidak ada respons palsu tersisa.");
                }

                return Task.FromResult(_responses.Dequeue());
            }
        }
    }
}
