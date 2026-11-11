using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Gerak ringan untuk UI: hanya transform dan opacity, 150-200 ms, tanpa
    /// shader, bayangan, atau gradient (design-tokens §1 dan §7).
    /// Pada batchmode (tes otomatis) gerak dimatikan agar tangkapan layar dan
    /// pembacaan rect selalu melihat keadaan akhir.
    /// </summary>
    public static class UITween
    {
        public const float Fast = 0.15f;
        public const float Normal = 0.2f;
        public const float PressedScale = 0.97f;

        private static Runner _runner;

        public static bool Enabled => !Application.isBatchMode;

        public static float EaseOutCubic(float t)
        {
            t = Mathf.Clamp01(t);
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        /// <summary>
        /// Konten memudar masuk sambil naik <paramref name="distance"/> dp.
        /// Posisi akhir selalu dikembalikan ke posisi awal, sehingga layout
        /// tidak bergeser setelah animasi selesai.
        /// </summary>
        public static void SlideFade(
            CanvasGroup group,
            RectTransform target,
            float distance = 8f,
            float duration = Normal)
        {
            if (group == null)
            {
                return;
            }

            var start = target != null ? target.anchoredPosition : Vector2.zero;
            if (!Enabled || duration <= 0f || !Application.isPlaying)
            {
                group.alpha = 1f;
                if (target != null)
                {
                    target.anchoredPosition = start;
                }

                return;
            }

            var runner = RunnerInstance();
            runner.StopAllCoroutines();
            runner.StartCoroutine(Slide(group, target, start, distance, duration));
        }

        private static IEnumerator Slide(
            CanvasGroup group,
            RectTransform target,
            Vector2 start,
            float distance,
            float duration)
        {
            var from = start + new Vector2(0f, -distance);
            group.alpha = 0f;
            if (target != null)
            {
                target.anchoredPosition = from;
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var progress = EaseOutCubic(elapsed / duration);
                group.alpha = progress;
                if (target != null)
                {
                    target.anchoredPosition = Vector2.Lerp(from, start, progress);
                }

                yield return null;
            }

            group.alpha = 1f;
            if (target != null)
            {
                target.anchoredPosition = start;
            }
        }

        private static Runner RunnerInstance()
        {
            if (_runner != null)
            {
                return _runner;
            }

            var go = new GameObject("UITween") { hideFlags = HideFlags.DontSave };
            _runner = go.AddComponent<Runner>();
            return _runner;
        }

        private sealed class Runner : MonoBehaviour
        {
        }
    }

    /// <summary>
    /// Umpan balik tekan: tombol mengecil sedikit selama jari menekan.
    /// Hanya skala transform, tidak mengubah rect atau layout.
    /// </summary>
    public sealed class PressScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private Vector3 _rest = Vector3.one;
        private Coroutine _routine;

        private void Awake()
        {
            _rest = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData) => Animate(Pressed());

        public void OnPointerUp(PointerEventData eventData) => Animate(_rest);

        public void OnPointerExit(PointerEventData eventData) => Animate(_rest);

        private void OnDisable()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            transform.localScale = _rest;
        }

        private Vector3 Pressed()
        {
            var selectable = GetComponent<Selectable>();
            if (selectable != null && !selectable.IsInteractable())
            {
                return _rest;
            }

            return _rest * UITween.PressedScale;
        }

        private void Animate(Vector3 target)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            if (!UITween.Enabled || !isActiveAndEnabled)
            {
                transform.localScale = target;
                return;
            }

            _routine = StartCoroutine(Run(target));
        }

        private IEnumerator Run(Vector3 target)
        {
            var from = transform.localScale;
            var elapsed = 0f;
            while (elapsed < UITween.Fast)
            {
                elapsed += Time.unscaledDeltaTime;
                transform.localScale = Vector3.Lerp(
                    from,
                    target,
                    UITween.EaseOutCubic(elapsed / UITween.Fast));
                yield return null;
            }

            transform.localScale = target;
            _routine = null;
        }
    }
}
