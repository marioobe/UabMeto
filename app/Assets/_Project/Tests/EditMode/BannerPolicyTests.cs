using NUnit.Framework;
using UabMeto.App.UI;

namespace UabMeto.Tests.EditMode
{
    /// <summary>
    /// Kebijakan banner: mode online lokal tidak boleh disebut "Offline"
    /// hanya karena internetReachability tidak melihat jaringan, sebab server
    /// lokal bisa tetap terjangkau lewat adb reverse.
    /// </summary>
    public sealed class BannerPolicyTests
    {
        [Test]
        public void OnlineMode_Reachable_ShowsNoBanner()
        {
            Assert.IsNull(ShellView.BannerMessage(false, false, string.Empty, true, true, 0));
        }

        [Test]
        public void OnlineMode_Pending_ShowsPendingWithoutOfflineClaim()
        {
            var message = ShellView.BannerMessage(false, false, string.Empty, true, true, 2);

            StringAssert.Contains("2 hasil menunggu sinkron", message);
            StringAssert.DoesNotContain("Offline", message);
        }

        [Test]
        public void NoOnlineMode_OfflineStillReported()
        {
            var message = ShellView.BannerMessage(false, false, string.Empty, true, false, 0);

            StringAssert.Contains("Offline", message);
        }

        [Test]
        public void SyncFailureMessageTakesPriority()
        {
            var message = ShellView.BannerMessage(
                false,
                true,
                "Server lokal tidak dapat dihubungi. Hasil tetap tersimpan.",
                true,
                true,
                1);

            StringAssert.Contains("Server lokal tidak dapat dihubungi", message);
        }

        [Test]
        public void SyncingMessageTakesPriority()
        {
            Assert.AreEqual(
                "Menyinkronkan hasil…",
                ShellView.BannerMessage(true, true, "gagal", true, true, 1));
        }
    }
}
