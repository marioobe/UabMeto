using System.Collections;
using NUnit.Framework;
using UabMeto.App.UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace UabMeto.Tests.PlayMode
{
    /// <summary>
    /// Mengunci perilaku gerak: easing monoton, dan pada batchmode gerak
    /// dimatikan sehingga konten langsung berada pada keadaan akhir
    /// (dipakai tes layout dan tangkapan layar).
    /// </summary>
    public sealed class TweenTests
    {
        [Test]
        public void EaseOutCubic_IsClampedAndMonotonic()
        {
            Assert.AreEqual(0f, UITween.EaseOutCubic(0f), 0.0001f);
            Assert.AreEqual(1f, UITween.EaseOutCubic(1f), 0.0001f);
            Assert.AreEqual(1f, UITween.EaseOutCubic(2f), 0.0001f);

            var previous = -1f;
            for (var step = 0; step <= 10; step++)
            {
                var value = UITween.EaseOutCubic(step / 10f);
                Assert.GreaterOrEqual(value, previous, "Easing harus tidak menurun.");
                previous = value;
            }
        }

        [UnityTest]
        public IEnumerator SlideFade_WhenMotionDisabled_SnapsToFinalState()
        {
            Assert.IsFalse(
                UITween.Enabled,
                "Batchmode harus mematikan gerak agar tangkapan layar deterministik.");

            var go = new GameObject("TweenTarget", typeof(RectTransform), typeof(CanvasGroup));
            var group = go.GetComponent<CanvasGroup>();
            var rect = (RectTransform)go.transform;
            rect.anchoredPosition = new Vector2(0f, 0f);
            group.alpha = 0f;

            UITween.SlideFade(group, rect, 8f, UITween.Normal);
            yield return null;

            Assert.AreEqual(1f, group.alpha, 0.0001f, "Alpha harus langsung penuh.");
            Assert.AreEqual(
                Vector2.zero,
                rect.anchoredPosition,
                "Posisi harus kembali ke keadaan akhir tanpa geser.");

            Object.Destroy(go);
        }
    }
}
