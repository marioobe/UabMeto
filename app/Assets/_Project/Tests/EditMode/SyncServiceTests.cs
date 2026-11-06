using System;
using System.IO;
using NUnit.Framework;
using UabMeto.App.Local;
using UabMeto.App.Net;

namespace UabMeto.Tests.EditMode
{
    /// <summary>
    /// Menguji alur outbox → API → status. XP resmi selalu berasal dari
    /// respons server, dan operasi yang ditolak tidak dikirim ulang.
    /// </summary>
    public sealed class SyncServiceTests
    {
        private const string BaseUrl = "http://127.0.0.1:8787";

        private string _directory;
        private OutboxStore _outbox;

        [SetUp]
        public void SetUp()
        {
            _directory = Path.Combine(
                Path.GetTempPath(),
                "uabmeto-sync-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);
            _outbox = OutboxStore.Load(Path.Combine(_directory, "outbox.json"));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        private SyncService CreateService(FakeHttpTransport transport)
        {
            var api = new ApiClient(transport, BaseUrl, "dev-user");
            return new SyncService(api, _outbox, () => "Penguji");
        }

        [Test]
        public void Sukses_MenyimpanXpServerBukanKlaimKlien()
        {
            var item = _outbox.Enqueue("lesson-001-salam", "1.1.0", 999);
            var transport = new FakeHttpTransport().Respond(
                200,
                "{\"state\":\"confirmed\",\"awardedXp\":10,\"duplicate\":false,\"devMode\":true}");

            var report = CreateService(transport).SyncPendingAsync().GetAwaiter().GetResult();

            Assert.AreEqual(1, report.Confirmed);
            Assert.AreEqual(OutboxStore.ConfirmedState, item.state);
            Assert.AreEqual(10, item.awardedXp);
            Assert.IsNull(item.lastError);
        }

        [Test]
        public void Duplikat_TetapTerkonfirmasiTanpaXpTambahan()
        {
            var item = _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            var transport = new FakeHttpTransport().Respond(
                200,
                "{\"state\":\"confirmed\",\"awardedXp\":10,\"duplicate\":true,\"devMode\":true}");

            var report = CreateService(transport).SyncPendingAsync().GetAwaiter().GetResult();

            Assert.AreEqual(1, report.Confirmed);
            Assert.AreEqual(OutboxStore.ConfirmedState, item.state);
            Assert.AreEqual(10, item.awardedXp);
        }

        [Test]
        public void KontenBelumValid_DitolakDanTidakDikirimUlang()
        {
            var item = _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            var transport = new FakeHttpTransport().Respond(
                422,
                "{\"error\":\"content_not_validated\",\"message\":\"belum valid\"}");
            var service = CreateService(transport);

            var first = service.SyncPendingAsync().GetAwaiter().GetResult();
            var second = service.SyncPendingAsync().GetAwaiter().GetResult();

            Assert.AreEqual(1, first.Rejected);
            Assert.AreEqual(OutboxStore.RejectedState, item.state);
            Assert.AreEqual(0, second.Rejected);
            Assert.AreEqual(1, transport.Calls.Count);
        }

        [Test]
        public void KonflikOperasi_MenjadiFailed()
        {
            var item = _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            var transport = new FakeHttpTransport().Respond(
                409,
                "{\"error\":\"operation_conflict\",\"message\":\"payload berbeda\"}");

            var report = CreateService(transport).SyncPendingAsync().GetAwaiter().GetResult();

            Assert.AreEqual(1, report.Failed);
            Assert.AreEqual(OutboxStore.FailedState, item.state);
        }

        [Test]
        public void Offline_MenambahPercobaanDanBerhenti()
        {
            var first = _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            _outbox.Enqueue("lesson-002-salam-bertemu", "1.1.0", 10);
            var transport = new FakeHttpTransport().Fail("connection refused");

            var report = CreateService(transport).SyncPendingAsync().GetAwaiter().GetResult();

            Assert.IsTrue(report.Offline);
            Assert.AreEqual(1, report.StillPending);
            Assert.AreEqual(1, transport.Calls.Count);
            Assert.AreEqual(OutboxStore.PendingState, first.state);
            Assert.AreEqual(1, first.attemptCount);
        }

        [Test]
        public void Offline_SetelahPulih_DikirimUlangDanTerkonfirmasi()
        {
            var item = _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            var transport = new FakeHttpTransport()
                .Fail("connection refused")
                .Respond(
                    200,
                    "{\"state\":\"confirmed\",\"awardedXp\":10,\"duplicate\":false,\"devMode\":false}");
            var service = CreateService(transport);

            service.SyncPendingAsync().GetAwaiter().GetResult();
            var report = service.SyncPendingAsync().GetAwaiter().GetResult();

            Assert.AreEqual(1, report.Confirmed);
            Assert.AreEqual(OutboxStore.ConfirmedState, item.state);
            Assert.AreEqual(2, transport.Calls.Count);
        }

        [Test]
        public void TidakBerwenang_BerhentiTanpaMengubahStatus()
        {
            var first = _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            var second = _outbox.Enqueue("lesson-002-salam-bertemu", "1.1.0", 10);
            var transport = new FakeHttpTransport().Respond(
                401,
                "{\"error\":\"unauthorized\",\"message\":\"header wajib\"}");

            var report = CreateService(transport).SyncPendingAsync().GetAwaiter().GetResult();

            Assert.IsTrue(report.Unauthorized);
            Assert.AreEqual(1, transport.Calls.Count);
            Assert.AreEqual(OutboxStore.PendingState, first.state);
            Assert.AreEqual(OutboxStore.PendingState, second.state);
        }

        [Test]
        public void BatasPercobaanHabis_MenjadiFailed()
        {
            var item = _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            item.maxAttempts = 1;
            var transport = new FakeHttpTransport().Fail("timeout");

            var report = CreateService(transport).SyncPendingAsync().GetAwaiter().GetResult();

            Assert.AreEqual(1, report.Failed);
            Assert.AreEqual(OutboxStore.FailedState, item.state);
            Assert.AreEqual(1, item.attemptCount);
        }

        [Test]
        public void GalatServer_MenambahPercobaanTanpaMenolak()
        {
            var item = _outbox.Enqueue("lesson-001-salam", "1.1.0", 10);
            var transport = new FakeHttpTransport().Respond(
                500,
                "{\"error\":\"internal_error\"}");

            var report = CreateService(transport).SyncPendingAsync().GetAwaiter().GetResult();

            Assert.AreEqual(1, report.StillPending);
            Assert.AreEqual(OutboxStore.PendingState, item.state);
            Assert.AreEqual(1, item.attemptCount);
        }

        [Test]
        public void OutboxKosong_TidakMemanggilTransport()
        {
            var transport = new FakeHttpTransport();

            var report = CreateService(transport).SyncPendingAsync().GetAwaiter().GetResult();

            Assert.IsFalse(report.HasWork);
            Assert.AreEqual(0, transport.Calls.Count);
        }
    }
}
