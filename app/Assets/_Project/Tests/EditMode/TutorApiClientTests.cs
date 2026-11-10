using System;
using NUnit.Framework;
using UabMeto.App.Net;

namespace UabMeto.Tests.EditMode
{
    /// <summary>
    /// Menguji kontrak Tutor AI pada klien: pertanyaan, riwayat, sumber, dan
    /// kode galat kuota/provider. Tidak ada jaringan; semua respons berasal
    /// dari <see cref="FakeHttpTransport"/>.
    /// </summary>
    public sealed class TutorApiClientTests
    {
        private const string BaseUrl = "http://127.0.0.1:8787";

        [Test]
        public void AskTutor_MengirimPertanyaanDanMembacaSumber()
        {
            var transport = new FakeHttpTransport().Respond(
                200,
                "{\"answer\":\"Noka berarti pagi.\"," +
                "\"sources\":[{\"lessonId\":\"lesson-001-salam\"," +
                "\"title\":\"Sapaan waktu\",\"sourceRef\":\"SRC-SPK-001\"}]," +
                "\"quota\":{\"used\":3,\"limit\":10}," +
                "\"refused\":false,\"model\":\"mock\"}");
            var api = new ApiClient(transport, BaseUrl + "/", "dev-user");

            var response = api.AskTutorAsync(
                    "Apa arti Noka?",
                    new[]
                    {
                        new TutorMessage { role = "user", content = "Halo" },
                        new TutorMessage { role = "tutor", content = "Hai" }
                    })
                .GetAwaiter()
                .GetResult();

            Assert.AreEqual("Noka berarti pagi.", response.answer);
            Assert.IsFalse(response.refused);
            Assert.AreEqual("mock", response.model);
            Assert.AreEqual(1, response.sources.Length);
            Assert.AreEqual("lesson-001-salam", response.sources[0].lessonId);
            Assert.AreEqual("SRC-SPK-001", response.sources[0].sourceRef);
            Assert.AreEqual(3, response.quota.used);
            Assert.AreEqual(10, response.quota.limit);

            Assert.AreEqual(1, transport.Calls.Count);
            Assert.AreEqual("POST", transport.Calls[0].Method);
            Assert.AreEqual(BaseUrl + "/v1/tutor/ask", transport.Calls[0].Url);
            Assert.AreEqual("dev-user", transport.Calls[0].Headers["X-Dev-User"]);

            var sent = UnityEngine.JsonUtility.FromJson<TutorAskRequest>(
                transport.Calls[0].Body);
            Assert.AreEqual("Apa arti Noka?", sent.question);
            Assert.AreEqual(2, sent.history.Length);
            Assert.AreEqual("user", sent.history[0].role);
        }

        [Test]
        public void AskTutor_TanpaRiwayat_MengirimDaftarKosong()
        {
            var transport = new FakeHttpTransport().Respond(
                200,
                "{\"answer\":\"Belum dapat dipastikan.\",\"sources\":[]," +
                "\"quota\":{\"used\":1,\"limit\":10},\"refused\":true,\"model\":\"none\"}");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            api.AskTutorAsync("Apa itu rumah adat?").GetAwaiter().GetResult();

            var sent = UnityEngine.JsonUtility.FromJson<TutorAskRequest>(
                transport.Calls[0].Body);
            Assert.AreEqual("Apa itu rumah adat?", sent.question);
            Assert.IsNotNull(sent.history);
            Assert.AreEqual(0, sent.history.Length);
        }

        [Test]
        public void AskTutor_PertanyaanKosong_DitolakSebelumJaringan()
        {
            var transport = new FakeHttpTransport();
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            Assert.Throws<ArgumentException>(
                () => api.AskTutorAsync("   ").GetAwaiter().GetResult());
            Assert.AreEqual(0, transport.Calls.Count);
        }

        [Test]
        public void AskTutor_KuotaHabis_MembawaKodeKuota()
        {
            var transport = new FakeHttpTransport().Respond(
                429,
                "{\"error\":\"quota_exceeded\",\"message\":\"Kuota habis.\"," +
                "\"quota\":{\"used\":10,\"limit\":10}}");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            var exception = Assert.Throws<ApiException>(
                () => api.AskTutorAsync("Apa arti Noka?").GetAwaiter().GetResult());

            Assert.AreEqual(429, exception.StatusCode);
            Assert.AreEqual("quota_exceeded", exception.Code);
            Assert.AreEqual("Kuota habis.", exception.Message);
        }

        [Test]
        public void AskTutor_ProviderGagal_MembawaKodeProvider()
        {
            var transport = new FakeHttpTransport().Respond(
                503,
                "{\"error\":\"provider_error\",\"message\":\"Workers AI gagal.\"}");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            var exception = Assert.Throws<ApiException>(
                () => api.AskTutorAsync("Apa arti Noka?").GetAwaiter().GetResult());

            Assert.AreEqual(503, exception.StatusCode);
            Assert.AreEqual("provider_error", exception.Code);
        }

        [Test]
        public void AskTutor_KegagalanTransport_MenjadiNetworkError()
        {
            var transport = new FakeHttpTransport().Fail("connection refused");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            var exception = Assert.Throws<ApiException>(
                () => api.AskTutorAsync("Apa arti Noka?").GetAwaiter().GetResult());

            Assert.AreEqual(0, exception.StatusCode);
            Assert.AreEqual("network_error", exception.Code);
        }
    }
}
