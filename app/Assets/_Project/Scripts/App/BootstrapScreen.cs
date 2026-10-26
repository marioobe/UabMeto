using UnityEngine;
using UnityEngine.UI;

namespace UabMeto.App
{
    /// <summary>
    /// Layar bootstrap F0. Hanya menampilkan identitas aplikasi dan status build.
    /// Bukan UI final: arah visual menunggu hasil desain Stitch yang disetujui.
    /// </summary>
    public sealed class BootstrapScreen : MonoBehaviour
    {
        private static readonly Color BackgroundColor = new Color(0.055f, 0.090f, 0.110f, 1f);
        private static readonly Color PrimaryTextColor = new Color(0.953f, 0.863f, 0.620f, 1f);
        private static readonly Color SecondaryTextColor = new Color(0.780f, 0.820f, 0.800f, 1f);
        private static readonly Color WarningColor = new Color(0.950f, 0.620f, 0.400f, 1f);

        private void Start()
        {
            BuildInterface();
        }

        private void BuildInterface()
        {
            var canvasObject = new GameObject(
                "BootstrapCanvas",
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            CreatePanel(canvasObject.transform, BackgroundColor);

            CreateLabel(canvasObject.transform, "Title", "Uab Meto", 84, FontStyle.Bold,
                new Vector2(0f, 380f), PrimaryTextColor);
            CreateLabel(canvasObject.transform, "Subtitle", "Aplikasi Pembelajaran Bahasa Dawan", 36,
                FontStyle.Normal, new Vector2(0f, 280f), SecondaryTextColor);
            CreateLabel(canvasObject.transform, "Stage", "F0 - Fondasi Lokal", 44, FontStyle.Bold,
                new Vector2(0f, 60f), SecondaryTextColor);
            CreateLabel(canvasObject.transform, "BuildInfo",
                $"Versi {Application.version}\nUnity {Application.unityVersion}",
                30, FontStyle.Normal, new Vector2(0f, -40f), SecondaryTextColor);
            CreateLabel(canvasObject.transform, "Warning",
                "KONTEN BELUM TERVALIDASI\nTidak untuk dipublikasikan",
                30, FontStyle.Bold, new Vector2(0f, -260f), WarningColor);
        }

        private static void CreatePanel(Transform parent, Color color)
        {
            var panelObject = new GameObject("Background", typeof(Image));
            panelObject.transform.SetParent(parent, false);

            var image = panelObject.GetComponent<Image>();
            image.color = color;

            var rect = panelObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void CreateLabel(
            Transform parent,
            string name,
            string content,
            int fontSize,
            FontStyle style,
            Vector2 anchoredPosition,
            Color color)
        {
            var labelObject = new GameObject(name, typeof(Text));
            labelObject.transform.SetParent(parent, false);

            var text = labelObject.GetComponent<Text>();
            text.text = content;
            text.font = LoadFont();
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            var rect = labelObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(900f, 160f);
            rect.anchoredPosition = anchoredPosition;
        }

        private static Font LoadFont()
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Font.CreateDynamicFontFromOSFont("Arial", 32);
            }

            return font;
        }
    }
}
