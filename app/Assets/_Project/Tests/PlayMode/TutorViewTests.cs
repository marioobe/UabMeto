using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Menguji tab Tutor AI dengan transport palsu. Tidak ada jaringan dan
    /// tidak ada model: semua jawaban berasal dari respons server yang
    /// ditentukan tes.
    /// </summary>
    public sealed class TutorViewTests
    {
        private const string BaseUrl = "http://127.0.0.1:8787";
        private const string RefusalText =
            "Belum dapat dipastikan dari materi tervalidasi saat ini. " +
            "Coba tanyakan kosakata atau dialog yang ada di pelajaran Topik 1.";

        private string _directory;
        private GameObject _root;
        private ShellView _shell;
        private ContentCatalog _catalog;
        private LocalProfile _profile;
        private ProgressStore _progress;
        private OutboxStore _outbox;
        private TutorFakeTransport _transport;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _directory = Path.Combine(
                Path.GetTempPath(),
                "uabmeto-tutor-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);

            var topics = Resources.Load<TextAsset>("Content/topics");
            Assert.IsNotNull(topics, "topics.json belum ada. Jalankan F1 Setup.");

            _catalog = ContentRepository.LoadFromText(
                topics.text,
                Array.Empty<string>());
            _profile = LocalProfile.Load(Path.Combine(_directory, "profile.json"));
            Assert.IsTrue(_profile.TrySetNickname("Penguji", out var error), error);
            _progress = ProgressStore.Load(Path.Combine(_directory, "progress.json"));
            _outbox = OutboxStore.Load(Path.Combine(_directory, "outbox.json"));
            _transport = new TutorFakeTransport();

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
            _root = new GameObject("TutorUiTest");
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

        private static void QueueQuota(TutorFakeTransport transport, int used)
        {
            transport.Respond(
                200,
                "{\"dayWita\":\"2026-11-06\"," +
                "\"tutor\":{\"used\":" + used + ",\"limit\":10}," +
                "\"conversation\":{\"used\":0,\"limit\":3}," +
                "\"pronunciation\":{\"used\":0,\"limit\":10}}");
        }

        private static TextMeshProUGUI FindText(string name)
        {
            var go = GameObject.Find(name);
            Assert.IsNotNull(go, "Tidak menemukan objek " + name);
            var text = go.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(text, name + " bukan teks.");
            return text;
        }

        private static bool HasText(string fragment)
        {
            return UnityEngine.Object
                .FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None)
                .Any(
                    text => text != null &&
                            text.text != null &&
                            text.text.IndexOf(fragment, StringComparison.Ordinal) >= 0);
        }

        private void TypeAndSend(string question)
        {
            var input = GameObject.Find("TutorInput").GetComponent<TMP_InputField>();
            input.text = question;
            GameObject.Find("TutorSend").GetComponent<Button>().onClick.Invoke();
        }

        [UnityTest]
        public IEnumerator Tutor_Aktif_MenampilkanKuotaDanPertanyaanCepat()
        {
            QueueQuota(_transport, 2);
            BuildShell(true);
            _shell.ShowTab(ShellView.Tab.Tutor);
            yield return null;

            Assert.AreEqual("Tutor AI", FindText("Title").text);
            StringAssert.Contains("Sisa 8 dari 10", FindText("TutorQuota").text);
            Assert.IsNotNull(GameObject.Find("TutorChip_0"));
            Assert.IsNotNull(GameObject.Find("TutorChip_3"));
            Assert.IsTrue(GameObject.Find("TutorSend").GetComponent<Button>().interactable);
            Assert.IsTrue(HasText("belum dapat dipastikan"), "Penjelasan batas Tutor AI tidak tampil.");
        }

        [UnityTest]
        public IEnumerator Tutor_Chip_MengisiKolomPertanyaan()
        {
            QueueQuota(_transport, 0);
            BuildShell(true);
            _shell.ShowTab(ShellView.Tab.Tutor);
            yield return null;

            GameObject.Find("TutorChip_0").GetComponent<Button>().onClick.Invoke();
            yield return null;

            var input = GameObject.Find("TutorInput").GetComponent<TMP_InputField>();
            Assert.AreEqual("Apa arti kata Noka?", input.text);
        }

        [UnityTest]
        public IEnumerator Tutor_KirimPertanyaan_MenampilkanJawabanDanSumber()
        {
            QueueQuota(_transport, 2);
            _transport.Respond(
                200,
                "{\"answer\":\"Noka berarti pagi.\"," +
                "\"sources\":[{\"lessonId\":\"lesson-001-salam\"," +
                "\"title\":\"Sapaan waktu\",\"sourceRef\":\"SRC-SPK-001\"}]," +
                "\"quota\":{\"used\":3,\"limit\":10}," +
                "\"refused\":false,\"model\":\"mock\"}");
            BuildShell(true);
            _shell.ShowTab(ShellView.Tab.Tutor);
            yield return null;

            TypeAndSend("Apa arti kata Noka?");
            yield return null;

            Assert.IsTrue(HasText("Noka berarti pagi."), "Jawaban tidak tampil.");
            Assert.IsTrue(HasText("Berdasarkan: Sapaan waktu"), "Sumber tidak tampil.");
            Assert.IsTrue(HasText("Sisa 7 dari 10"), "Kuota tidak diperbarui.");
            StringAssert.Contains("materi tervalidasi", FindText("TutorStatus").text);
            Assert.AreEqual("Apa arti kata Noka?", _transport.LastCall.Question);
        }

        [UnityTest]
        public IEnumerator Tutor_PertanyaanDiLuarMateri_MenampilkanPenolakan()
        {
            QueueQuota(_transport, 0);
            _transport.Respond(
                200,
                "{\"answer\":\"" + RefusalText + "\",\"sources\":[]," +
                "\"quota\":{\"used\":1,\"limit\":10}," +
                "\"refused\":true,\"model\":\"none\"}");
            BuildShell(true);
            _shell.ShowTab(ShellView.Tab.Tutor);
            yield return null;

            TypeAndSend("Apa arti rumah adat?");
            yield return null;

            Assert.IsTrue(HasText("Belum dapat dipastikan"), "Penolakan tidak tampil.");
            Assert.IsFalse(HasText("Berdasarkan:"), "Penolakan tidak boleh membawa sumber.");
            StringAssert.Contains("tidak dikarang", FindText("TutorStatus").text);
        }

        [UnityTest]
        public IEnumerator Tutor_KuotaHabis_MenonaktifkanTombolKirim()
        {
            QueueQuota(_transport, 10);
            BuildShell(true);
            _shell.ShowTab(ShellView.Tab.Tutor);
            yield return null;

            StringAssert.Contains("Sisa 0 dari 10", FindText("TutorQuota").text);
            Assert.IsFalse(GameObject.Find("TutorSend").GetComponent<Button>().interactable);
            StringAssert.Contains("habis", FindText("TutorStatus").text);

            var calls = _transport.CallCount;
            GameObject.Find("TutorSend").GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.AreEqual(calls, _transport.CallCount, "Kuota habis tidak boleh memanggil server.");
        }

        [UnityTest]
        public IEnumerator Tutor_KuotaHabisSaatBertanya_MenampilkanPesanServer()
        {
            QueueQuota(_transport, 9);
            _transport.Respond(
                429,
                "{\"error\":\"quota_exceeded\",\"message\":\"Kuota Tutor AI hari ini habis.\"," +
                "\"quota\":{\"used\":10,\"limit\":10}}");
            BuildShell(true);
            _shell.ShowTab(ShellView.Tab.Tutor);
            yield return null;

            TypeAndSend("Apa arti kata Noka?");
            yield return null;

            StringAssert.Contains("Sisa 0 dari 10", FindText("TutorQuota").text);
            StringAssert.Contains("habis", FindText("TutorStatus").text);
            Assert.AreEqual(ThemeTokens.Load().DangerText, FindText("TutorStatus").color);
            Assert.IsFalse(GameObject.Find("TutorSend").GetComponent<Button>().interactable);
        }

        [UnityTest]
        public IEnumerator Tutor_GalatJaringan_MenampilkanPesanTanpaJawaban()
        {
            QueueQuota(_transport, 0);
            _transport.Fail("connection refused");
            BuildShell(true);
            _shell.ShowTab(ShellView.Tab.Tutor);
            yield return null;

            TypeAndSend("Apa arti kata Noka?");
            yield return null;

            StringAssert.Contains(
                "Tidak dapat menghubungi",
                FindText("TutorStatus").text);
            Assert.AreEqual(ThemeTokens.Load().DangerText, FindText("TutorStatus").color);
            Assert.IsTrue(GameObject.Find("TutorSend").GetComponent<Button>().interactable);
        }

        [UnityTest]
        public IEnumerator Tutor_TanpaModeOnline_MenampilkanStatusNonaktif()
        {
            BuildShell(false);
            _shell.ShowTab(ShellView.Tab.Tutor);
            yield return null;

            StringAssert.Contains("Mode online lokal nonaktif", FindText("TutorQuota").text);
            StringAssert.Contains("server lokal", FindText("TutorStatus").text);
            Assert.IsFalse(GameObject.Find("TutorInput").GetComponent<TMP_InputField>().interactable);
            Assert.IsFalse(GameObject.Find("TutorSend").GetComponent<Button>().interactable);
            Assert.AreEqual(0, _transport.CallCount);
        }

        private sealed class TutorFakeTransport : IHttpTransport
        {
            private readonly Queue<HttpResult> _responses = new Queue<HttpResult>();

            public int CallCount { get; private set; }

            public TutorCall LastCall { get; private set; }

            public TutorFakeTransport Respond(int statusCode, string body)
            {
                _responses.Enqueue(HttpResult.FromResponse(statusCode, body));
                return this;
            }

            public TutorFakeTransport Fail(string error)
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
                LastCall = new TutorCall(
                    method,
                    url,
                    jsonBody == null ? null : JsonUtility.FromJson<TutorAskRequest>(jsonBody));

                if (_responses.Count == 0)
                {
                    throw new InvalidOperationException(
                        "Tidak ada respons palsu tersisa.");
                }

                return Task.FromResult(_responses.Dequeue());
            }
        }

        public sealed class TutorCall
        {
            public TutorCall(string method, string url, TutorAskRequest body)
            {
                Method = method;
                Url = url;
                Body = body;
            }

            public string Method { get; }
            public string Url { get; }
            public TutorAskRequest Body { get; }
            public string Question => Body == null ? null : Body.question;
        }
    }
}
