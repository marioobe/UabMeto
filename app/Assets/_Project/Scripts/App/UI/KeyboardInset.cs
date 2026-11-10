using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Menggeser kolom tulis ke atas keyboard layar dan menyusutkan area gulir.
    /// Unity Android (Game Activity) tidak melakukan <c>adjustResize</c>, dan
    /// <see cref="TouchScreenKeyboard.area"/> kosong di perangkat ini. Karena
    /// itu tinggi keyboard dibaca dari window insets Android, dengan
    /// <see cref="TouchScreenKeyboard.area"/> sebagai jalur utama bila tersedia.
    /// </summary>
    internal sealed class KeyboardInset : MonoBehaviour
    {
        private const float ProbeInterval = 0.15f;
        private const float MinimumKeyboardFraction = 0.15f;

        private RectTransform _composer;
        private RectTransform _scroll;
        private TMP_InputField _input;
        private Canvas _canvas;
        private float _baseBottom;
        private float _applied = -1f;
        private float _lastProbe;

        public void Initialize(
            RectTransform composer,
            RectTransform scroll,
            TMP_InputField input,
            float baseBottom)
        {
            _composer = composer;
            _scroll = scroll;
            _input = input;
            _baseBottom = baseBottom;

            var graphic = GetComponent<Graphic>();
            _canvas = graphic == null ? null : graphic.canvas;
            Apply(0f);
        }

        private void Update()
        {
            if (_composer == null)
            {
                return;
            }

            // Menggeser panel saat jari masih menekan tombol membuat pointer-up
            // jatuh di luar tombol, sehingga tap Kirim dibatalkan. Tunggu jari
            // diangkat sebelum mengubah posisi.
            if (IsPointerDown())
            {
                return;
            }

            var focused = _input != null && _input.isFocused;
            if (!focused &&
                (TouchScreenKeyboard.visible || _applied > 0f))
            {
                Apply(0f);
                return;
            }

            if (!focused)
            {
                return;
            }

            if (Time.unscaledTime - _lastProbe < ProbeInterval)
            {
                return;
            }

            _lastProbe = Time.unscaledTime;

            var pixels = GetKeyboardHeightPixels();
            var scale = _canvas == null ? 1f : _canvas.scaleFactor;
            if (scale <= 0f)
            {
                scale = 1f;
            }

            Apply(pixels / scale);
        }

        private static bool IsPointerDown()
        {
            return Input.touchCount > 0 || Input.GetMouseButton(0);
        }

        private static float GetKeyboardHeightPixels()
        {
            if (TouchScreenKeyboard.visible &&
                TouchScreenKeyboard.area.height > 0f)
            {
                return TouchScreenKeyboard.area.height;
            }

            if (Application.platform != RuntimePlatform.Android)
            {
                return 0f;
            }

            AndroidJavaObject activity = null;
            AndroidJavaObject window = null;
            AndroidJavaObject decor = null;
            AndroidJavaObject rect = null;
            try
            {
                using (var player = new AndroidJavaClass(
                    "com.unity3d.player.UnityPlayer"))
                {
                    activity = player.GetStatic<AndroidJavaObject>(
                        "currentActivity");
                }

                if (activity == null)
                {
                    return 0f;
                }

                window = activity.Call<AndroidJavaObject>("getWindow");
                decor = window.Call<AndroidJavaObject>("getDecorView");
                rect = new AndroidJavaObject("android.graphics.Rect");
                decor.Call("getWindowVisibleDisplayFrame", rect);

                var bottom = rect.Get<int>("bottom");
                var inset = Screen.height - bottom;

                // Abaikan selisih kecil (status/navigation bar); hanya keyboard
                // yang menutupi sebagian besar layar yang dianggap keyboard.
                return inset > Screen.height * MinimumKeyboardFraction
                    ? inset
                    : 0f;
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[UabMeto] Tinggi keyboard tidak dapat dibaca: " +
                    exception.Message);
                return 0f;
            }
            finally
            {
                rect?.Dispose();
                decor?.Dispose();
                window?.Dispose();
                activity?.Dispose();
            }
        }

        private void Apply(float offset)
        {
            if (Mathf.Abs(offset - _applied) < 0.5f)
            {
                return;
            }

            _applied = offset;

            if (_composer != null)
            {
                var position = _composer.anchoredPosition;
                position.y = offset;
                _composer.anchoredPosition = position;
            }

            if (_scroll != null)
            {
                var min = _scroll.offsetMin;
                min.y = _baseBottom + offset;
                _scroll.offsetMin = min;
            }
        }
    }
}
