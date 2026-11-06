using NUnit.Framework;
using UabMeto.App.Net;
using UnityEngine;

namespace UabMeto.Tests.EditMode
{
    /// <summary>
    /// Menguji pemetaan kontrak Worker lokal ke DTO klien. Tidak ada jaringan;
    /// semua respons datang dari <see cref="FakeHttpTransport"/>.
    /// </summary>
    public sealed class ApiClientTests
    {
        private const string BaseUrl = "http://127.0.0.1:8787";

        [Test]
        public void Sync_MengirimOperasiDanMembacaXpServer()
        {
            var transport = new FakeHttpTransport().Respond(
                200,
                "{\"state\":\"confirmed\",\"awardedXp\":10,\"duplicate\":false,\"devMode\":true}");
            var api = new ApiClient(transport, BaseUrl + "/", "dev-user");

            var response = api.SyncAsync(
                    new SyncRequest
                    {
                        operationId = "op-1",
                        lessonId = "lesson-001-salam",
                        contentVersion = "1.1.0",
                        clientClaimedXp = 999,
                        nickname = "Penguji"
                    })
                .GetAwaiter()
                .GetResult();

            Assert.AreEqual(10, response.awardedXp);
            Assert.AreEqual("confirmed", response.state);
            Assert.IsTrue(response.devMode);
            Assert.AreEqual(1, transport.Calls.Count);
            Assert.AreEqual("POST", transport.Calls[0].Method);
            Assert.AreEqual(BaseUrl + "/v1/sync", transport.Calls[0].Url);
            Assert.AreEqual("dev-user", transport.Calls[0].Headers["X-Dev-User"]);

            var sent = JsonUtility.FromJson<SyncRequest>(transport.Calls[0].Body);
            Assert.AreEqual("op-1", sent.operationId);
            Assert.AreEqual(999, sent.clientClaimedXp);
        }

        [Test]
        public void Sync_KontenBelumValid_MembawaKodeServer()
        {
            var transport = new FakeHttpTransport().Respond(
                422,
                "{\"error\":\"content_not_validated\",\"status\":\"in_review\"," +
                "\"message\":\"XP hanya boleh diberikan untuk konten validated.\"}");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            var exception = Assert.Throws<ApiException>(
                () =>
                {
                    api.SyncAsync(new SyncRequest()).GetAwaiter().GetResult();
                });

            Assert.AreEqual(422, exception.StatusCode);
            Assert.AreEqual("content_not_validated", exception.Code);
        }

        [Test]
        public void Sync_KonflikOperasi_MembawaKodeKonflik()
        {
            var transport = new FakeHttpTransport().Respond(
                409,
                "{\"error\":\"operation_conflict\",\"message\":\"operationId sudah dipakai.\"}");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            var exception = Assert.Throws<ApiException>(
                () =>
                {
                    api.SyncAsync(new SyncRequest()).GetAwaiter().GetResult();
                });

            Assert.AreEqual(409, exception.StatusCode);
            Assert.AreEqual("operation_conflict", exception.Code);
        }

        [Test]
        public void Quota_MembacaTigaCounter()
        {
            var transport = new FakeHttpTransport().Respond(
                200,
                "{\"dayWita\":\"2026-11-06\"," +
                "\"tutor\":{\"used\":4,\"limit\":10}," +
                "\"conversation\":{\"used\":1,\"limit\":3}," +
                "\"pronunciation\":{\"used\":2,\"limit\":10}}");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            var quota = api.GetQuotaAsync().GetAwaiter().GetResult();

            Assert.AreEqual("2026-11-06", quota.dayWita);
            Assert.AreEqual(4, quota.tutor.used);
            Assert.AreEqual(10, quota.tutor.limit);
            Assert.AreEqual(3, quota.conversation.limit);
            Assert.AreEqual(2, quota.pronunciation.used);
        }

        [Test]
        public void Leaderboard_MembacaEntriBerurutan()
        {
            var transport = new FakeHttpTransport().Respond(
                200,
                "{\"weekKey\":\"2026-11-02\",\"entries\":[" +
                "{\"userId\":\"dev-user-2\",\"nickname\":\"Aina\",\"xp\":20}," +
                "{\"userId\":\"dev-user\",\"nickname\":\"Penguji\",\"xp\":10}]}");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            var board = api.GetLeaderboardAsync().GetAwaiter().GetResult();

            Assert.AreEqual("2026-11-02", board.weekKey);
            Assert.AreEqual(2, board.entries.Count);
            Assert.AreEqual("dev-user-2", board.entries[0].userId);
            Assert.AreEqual(20, board.entries[0].xp);
            Assert.AreEqual("dev-user", board.entries[1].userId);
        }

        [Test]
        public void StatusOperasi_MemakaiPathTerenkode()
        {
            var transport = new FakeHttpTransport().Respond(
                200,
                "{\"operationId\":\"op-1\",\"lessonId\":\"lesson-001-salam\"," +
                "\"state\":\"confirmed\",\"awardedXp\":10}");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            var status = api.GetOperationAsync("op-1").GetAwaiter().GetResult();

            Assert.AreEqual("confirmed", status.state);
            Assert.AreEqual(10, status.awardedXp);
            Assert.AreEqual(BaseUrl + "/v1/sync/op-1", transport.Calls[0].Url);
            Assert.AreEqual("GET", transport.Calls[0].Method);
            Assert.IsNull(transport.Calls[0].Body);
        }

        [Test]
        public void KegagalanTransport_MenjadiNetworkError()
        {
            var transport = new FakeHttpTransport().Fail("connection refused");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            var exception = Assert.Throws<ApiException>(
                () =>
                {
                    api.GetQuotaAsync().GetAwaiter().GetResult();
                });

            Assert.AreEqual(0, exception.StatusCode);
            Assert.AreEqual("network_error", exception.Code);
        }

        [Test]
        public void ResponsRusak_MenjadiInvalidResponse()
        {
            var transport = new FakeHttpTransport().Respond(200, "bukan json");
            var api = new ApiClient(transport, BaseUrl, "dev-user");

            var exception = Assert.Throws<ApiException>(
                () =>
                {
                    api.GetQuotaAsync().GetAwaiter().GetResult();
                });

            Assert.AreEqual("invalid_response", exception.Code);
        }

        [Test]
        public void DevUser_TidakValid_Ditolak()
        {
            var transport = new FakeHttpTransport();

            Assert.Throws<System.ArgumentException>(
                () => new ApiClient(transport, BaseUrl, "ab"));
            Assert.Throws<System.ArgumentException>(
                () => new ApiClient(transport, BaseUrl, "nama dengan spasi"));
            Assert.Throws<System.ArgumentException>(
                () => new ApiClient(transport, BaseUrl, string.Empty));
        }
    }
}
