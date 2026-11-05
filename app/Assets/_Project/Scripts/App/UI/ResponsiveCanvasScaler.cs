using UnityEngine;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Menyesuaikan acuan kanvas dengan orientasi. Desain F1 portrait
    /// memakai 360×640; saat perangkat landscape acuan ditukar menjadi
    /// 640×360 agar tinggi kanvas tidak menyusut sampai hanya menyisakan
    /// header dan bottom bar.
    /// </summary>
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class ResponsiveCanvasScaler : MonoBehaviour
    {
        private static readonly Vector2 PortraitReference = new Vector2(360f, 640f);
        private static readonly Vector2 LandscapeReference = new Vector2(640f, 360f);

        private CanvasScaler _scaler;
        private int _lastWidth;
        private int _lastHeight;

        private void Awake()
        {
            _scaler = GetComponent<CanvasScaler>();
            Apply();
        }

        private void Update()
        {
            if (Screen.width == _lastWidth && Screen.height == _lastHeight)
            {
                return;
            }

            Apply();
        }

        private void Apply()
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;

            var landscape = Screen.width > Screen.height;
            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _scaler.referenceResolution = landscape ? LandscapeReference : PortraitReference;
            _scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            // Selalu kunci ke lebar acuan supaya jumlah unit horizontal
            // konsisten dan teks tidak membungkus lebih rapat dari desain.
            _scaler.matchWidthOrHeight = 0f;
        }
    }
}
