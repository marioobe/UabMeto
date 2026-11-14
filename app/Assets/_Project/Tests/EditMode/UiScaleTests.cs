using NUnit.Framework;
using UabMeto.App.UI;

namespace UabMeto.Tests.EditMode
{
    /// <summary>
    /// Mengunci pembacaan dan pembatasan skala font sistem (F1-21/D1-06).
    /// Tes ini tidak membuktikan tampilan perangkat; uji 1.3x/2.0x tetap
    /// dijalankan di PlayMode dan di OPPO.
    /// </summary>
    public sealed class UiScaleTests
    {
        [TearDown]
        public void TearDown()
        {
            UiScale.ClearOverride();
        }

        [Test]
        public void DefaultFactorIsOne()
        {
            Assert.AreEqual(1f, UiScale.Factor, 0.001f);
            Assert.AreEqual(16f, UiScale.Px(16f), 0.001f);
        }

        [Test]
        public void FactorBelowMinimumIsClamped()
        {
            UiScale.SetForTests(0.85f);
            Assert.AreEqual(1f, UiScale.Factor, 0.001f);
            Assert.AreEqual(16f, UiScale.Px(16f), 0.001f);
        }

        [Test]
        public void FactorAboveMaximumIsClamped()
        {
            UiScale.SetForTests(3f);
            Assert.AreEqual(2f, UiScale.Factor, 0.001f);
            Assert.AreEqual(32f, UiScale.Px(16f), 0.001f);
        }

        [Test]
        public void FactorIsAppliedToTypographyAndSpacing()
        {
            UiScale.SetForTests(1.3f);
            Assert.AreEqual(1.3f, UiScale.Factor, 0.001f);
            Assert.AreEqual(20.8f, UiScale.Px(16f), 0.001f);
            Assert.AreEqual(31.2f, UiScale.Px(24f), 0.001f);
        }

        [Test]
        public void InvalidValuesFallBackToDesignScale()
        {
            UiScale.SetForTests(0f);
            Assert.AreEqual(1f, UiScale.Factor, 0.001f);

            UiScale.SetForTests(float.NaN);
            Assert.AreEqual(1f, UiScale.Factor, 0.001f);
        }

        [Test]
        public void ClearOverrideReturnsToDesignScale()
        {
            UiScale.SetForTests(2f);
            Assert.AreEqual(2f, UiScale.Factor, 0.001f);

            UiScale.ClearOverride();
            Assert.AreEqual(1f, UiScale.Factor, 0.001f);
        }
    }
}
