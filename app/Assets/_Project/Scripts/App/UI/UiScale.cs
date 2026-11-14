using System;
using UnityEngine;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Skala font sistem untuk F1-21/D1-06. uGUI dan TextMeshPro tidak
    /// menerapkan skala font Android secara otomatis, jadi ukuran teks,
    /// ikon, target sentuh, dan jarak dibaca dari satu faktor di sini.
    ///
    /// Faktor dibatasi 1.0-2.0: pengguna yang mengecilkan font tidak boleh
    /// mendapat teks lebih kecil dari ukuran desain, dan pembesaran di atas
    /// 2.0 tidak diuji sehingga tidak diklaim didukung.
    /// </summary>
    public static class UiScale
    {
        public const float Minimum = 1f;
        public const float Maximum = 2f;

        private static float _factor = 1f;
        private static bool _overrideActive;
        private static bool _logged;

        /// <summary>Faktor aktif setelah pembacaan dan pembatasan.</summary>
        public static float Factor => _factor;

        /// <summary>
        /// Membaca skala font sistem. Aman dipanggil berkali-kali; hasil
        /// override tes tidak ditimpa.
        /// </summary>
        public static void Refresh()
        {
            if (_overrideActive)
            {
                return;
            }

            var raw = ReadSystemScale();
            _factor = Clamp(raw);

            if (!_logged && !Mathf.Approximately(_factor, 1f))
            {
                // Berguna untuk bukti D1-06 di logcat: nilai mentah dan nilai
                // yang benar-benar diterapkan.
                Debug.Log(
                    $"[UabMeto] fontScale sistem {raw:F2} diterapkan sebagai {_factor:F2}.");
                _logged = true;
            }
        }

        /// <summary>Menetapkan faktor untuk tes tanpa menyentuh setelan sistem.</summary>
        public static void SetForTests(float value)
        {
            _overrideActive = true;
            _factor = Clamp(value);
        }

        /// <summary>Menghapus override tes dan kembali ke faktor desain.</summary>
        public static void ClearOverride()
        {
            _overrideActive = false;
            _factor = 1f;
            _logged = false;
        }

        public static float Clamp(float value)
        {
            if (float.IsNaN(value) || value <= 0.01f)
            {
                return 1f;
            }

            return Mathf.Clamp(value, Minimum, Maximum);
        }

        /// <summary>Menskalakan satu ukuran desain (dp) ke unit kanvas.</summary>
        public static float Px(float value) => value * _factor;

        private static float ReadSystemScale()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var android = ReadAndroidFontScale();
            if (android > 0.01f)
            {
                return android;
            }
#endif

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            try
            {
                return UnityEngine.Accessibility.AccessibilitySettings.fontScale;
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[UabMeto] Skala font sistem tidak dapat dibaca: " + exception.Message);
            }
#endif

            return 1f;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        /// <summary>
        /// Android 8.1 (API 27) sudah menyimpan fontScale di Configuration,
        /// tetapi jalur JNI dipakai lebih dulu agar nilai yang dibaca sama
        /// dengan yang dilihat pengguna di Setelan.
        /// </summary>
        private static float ReadAndroidFontScale()
        {
            AndroidJavaObject activity = null;
            AndroidJavaObject resources = null;
            AndroidJavaObject configuration = null;
            try
            {
                using (var player = new AndroidJavaClass(
                    "com.unity3d.player.UnityPlayer"))
                {
                    activity = player.GetStatic<AndroidJavaObject>("currentActivity");
                }

                if (activity == null)
                {
                    return 0f;
                }

                resources = activity.Call<AndroidJavaObject>("getResources");
                configuration = resources.Call<AndroidJavaObject>("getConfiguration");
                return configuration.Get<float>("fontScale");
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[UabMeto] fontScale Android tidak dapat dibaca lewat JNI: " +
                    exception.Message);
                return 0f;
            }
            finally
            {
                configuration?.Dispose();
                resources?.Dispose();
                activity?.Dispose();
            }
        }
#endif
    }
}
