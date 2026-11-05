using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Pembuat komponen uGUI + TextMeshPro. Semua warna dan ukuran berasal
    /// dari ThemeTokens; tidak ada hex di sini.
    /// </summary>
    public static class UIFactory
    {
        private static readonly Dictionary<int, Sprite> RoundedSprites = new Dictionary<int, Sprite>();

        public static Canvas CreateCanvas(string name, ThemeTokens theme)
        {
            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(360f, 640f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            // Aplikasi F1 dirancang portrait dengan lebar acuan 360. Mengunci
            // ke lebar membuat jumlah unit horizontal selalu 360 di semua
            // perangkat, sehingga teks tidak tiba-tiba membungkus lebih rapat
            // pada layar 20:9. ResponsiveCanvasScaler menukar acuan saat
            // perangkat landscape.
            scaler.matchWidthOrHeight = 0f;
            go.AddComponent<ResponsiveCanvasScaler>();

            return canvas;
        }

        public static RectTransform CreateRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        public static Image CreatePanel(
            string name,
            Transform parent,
            Color color,
            int cornerRadius = 0,
            bool raycast = false)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);

            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = raycast;

            if (cornerRadius > 0)
            {
                image.sprite = RoundedSprite(cornerRadius);
                image.type = Image.Type.Sliced;
            }

            return image;
        }

        public static TextMeshProUGUI CreateText(
            string name,
            Transform parent,
            string value,
            TMP_FontAsset font,
            float size,
            Color color,
            TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);

            var text = go.GetComponent<TextMeshProUGUI>();
            text.font = font;
            text.fontSize = size;
            text.color = color;
            text.text = value;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;

            return text;
        }

        public static TextMeshProUGUI CreateIcon(
            string name,
            Transform parent,
            string iconName,
            TMP_FontAsset iconFont,
            float size,
            Color color)
        {
            var text = CreateText(
                name,
                parent,
                IconCodepoints.Glyph(iconName),
                iconFont,
                size,
                color,
                TextAlignmentOptions.Center);
            text.characterSpacing = 0f;
            text.lineSpacing = 0f;
            return text;
        }

        public static Button CreateButton(
            string name,
            Transform parent,
            string label,
            ThemeTokens theme,
            UnityAction onClick,
            bool primary = true,
            bool ghost = false)
        {
            var background = ghost
                ? new Color(theme.SurfaceHigh.r, theme.SurfaceHigh.g, theme.SurfaceHigh.b, 0f)
                : primary
                    ? theme.Accent
                    : theme.SurfaceHigh;

            var panel = CreatePanel(name, parent, background, (int)theme.CornerRadius, true);
            var button = panel.gameObject.AddComponent<Button>();
            button.targetGraphic = panel;
            button.transition = Selectable.Transition.ColorTint;

            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.45f);
            button.colors = colors;

            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = theme.TouchTarget;
            layout.preferredHeight = 52f;

            var text = CreateText(
                "Label",
                panel.transform,
                label,
                primary ? theme.HeadingBoldFont : theme.BodyBoldFont,
                theme.ButtonSize,
                primary ? theme.OnAccent : theme.TextPrimary,
                TextAlignmentOptions.Center);
            Stretch(text.rectTransform, 16f, 12f, 16f, 12f);

            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            AddAccessibilityLabel(button.gameObject, label);
            return button;
        }

        public static Button CreateIconButton(
            string name,
            Transform parent,
            string iconName,
            string accessibilityLabel,
            ThemeTokens theme,
            UnityAction onClick,
            Color? background = null)
        {
            var panel = CreatePanel(
                name,
                parent,
                background ?? new Color(theme.SurfaceHigh.r, theme.SurfaceHigh.g, theme.SurfaceHigh.b, 0f),
                (int)(theme.TouchTarget / 2f),
                true);

            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.minWidth = theme.TouchTarget;
            layout.minHeight = theme.TouchTarget;
            layout.preferredWidth = theme.TouchTarget;
            layout.preferredHeight = theme.TouchTarget;

            var button = panel.gameObject.AddComponent<Button>();
            button.targetGraphic = panel;
            button.transition = Selectable.Transition.ColorTint;

            var icon = CreateIcon(
                "Icon",
                panel.transform,
                iconName,
                theme.IconFont,
                theme.IconSize,
                theme.TextPrimary);
            Stretch(icon.rectTransform);

            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            AddAccessibilityLabel(button.gameObject, accessibilityLabel);
            return button;
        }

        public static TMP_InputField CreateInputField(
            string name,
            Transform parent,
            string placeholder,
            ThemeTokens theme,
            int characterLimit = 20)
        {
            var panel = CreatePanel(name, parent, theme.SurfaceLow, (int)theme.Space12, true);
            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 52f;
            layout.preferredHeight = 52f;

            var input = panel.gameObject.AddComponent<TMP_InputField>();
            input.targetGraphic = panel;
            input.textViewport = panel.rectTransform;
            input.lineType = TMP_InputField.LineType.SingleLine;
            input.characterLimit = characterLimit;
            input.caretWidth = 2;
            input.caretColor = theme.Accent;
            input.selectionColor = new Color(theme.Accent.r, theme.Accent.g, theme.Accent.b, 0.35f);

            var text = CreateText(
                "Text",
                panel.transform,
                string.Empty,
                theme.BodyFont,
                theme.BodySize,
                theme.TextPrimary,
                TextAlignmentOptions.MidlineLeft);
            Stretch(text.rectTransform, 14f, 8f, 14f, 8f);
            text.raycastTarget = true;

            var placeholderText = CreateText(
                "Placeholder",
                panel.transform,
                placeholder,
                theme.BodyFont,
                theme.BodySize,
                theme.TextMuted,
                TextAlignmentOptions.MidlineLeft);
            Stretch(placeholderText.rectTransform, 14f, 8f, 14f, 8f);
            placeholderText.raycastTarget = false;

            input.textComponent = text;
            input.placeholder = placeholderText;

            return input;
        }

        public static RectTransform CreateVerticalGroup(
            string name,
            Transform parent,
            float spacing,
            RectOffset padding = null,
            bool fitHeight = true)
        {
            var rect = CreateRect(name, parent);
            var layout = rect.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childAlignment = TextAnchor.UpperLeft;

            if (fitHeight)
            {
                var fitter = rect.gameObject.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            return rect;
        }

        /// <summary>
        /// Kartu yang tingginya mengikuti isi, bukan angka tetap. Panel latar
        /// menampung satu grup vertikal bernama <c>Body</c>; tinggi panel
        /// dihitung dari preferred height isi Body oleh layout induk.
        /// </summary>
        public static RectTransform CreateCard(
            string name,
            Transform parent,
            Color color,
            ThemeTokens theme,
            float spacing = -1f,
            RectOffset padding = null,
            int cornerRadius = -1)
        {
            var card = CreatePanel(
                name,
                parent,
                color,
                cornerRadius >= 0 ? cornerRadius : (int)theme.Space12);

            var cardLayout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            cardLayout.spacing = 0f;
            cardLayout.padding = new RectOffset();
            cardLayout.childControlWidth = true;
            cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = true;
            cardLayout.childForceExpandHeight = false;
            cardLayout.childAlignment = TextAnchor.UpperLeft;

            var element = card.gameObject.AddComponent<LayoutElement>();
            element.flexibleWidth = 1f;
            element.minHeight = 0f;

            return CreateVerticalGroup(
                "Body",
                card.transform,
                spacing >= 0f ? spacing : theme.Space8,
                padding ?? new RectOffset(16, 16, 16, 16),
                true);
        }

        public static RectTransform CreateHorizontalGroup(
            string name,
            Transform parent,
            float spacing,
            RectOffset padding = null)
        {
            var rect = CreateRect(name, parent);
            var layout = rect.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.childAlignment = TextAnchor.MiddleLeft;
            return rect;
        }

        public static ScrollRect CreateScrollView(string name, Transform parent, ThemeTokens theme)
        {
            var root = CreateRect(name, parent);
            var scroll = root.gameObject.AddComponent<ScrollRect>();

            var viewport = CreateRect("Viewport", root);
            Stretch(viewport);
            viewport.gameObject.AddComponent<RectMask2D>();

            // ScrollRect hanya menerima drag jika ada Graphic yang menjadi
            // raycast target. Tanpa ini, kartu yang raycastTarget=false membuat
            // seluruh area konten tidak bisa digeser di perangkat.
            var viewportHitArea = viewport.gameObject.AddComponent<Image>();
            viewportHitArea.color = new Color(0f, 0f, 0f, 0f);
            viewportHitArea.raycastTarget = true;

            var content = CreateRect("Content", viewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;

            var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = theme.Space12;
            layout.padding = new RectOffset(16, 16, 16, 24);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.inertia = true;
            scroll.scrollSensitivity = 24f;

            return scroll;
        }

        public static RectTransform CreateSpacer(string name, Transform parent, float height)
        {
            var rect = CreateRect(name, parent);
            var layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = height;
            layout.preferredHeight = height;
            return rect;
        }

        public static Image CreateDivider(string name, Transform parent, ThemeTokens theme)
        {
            var panel = CreatePanel(name, parent, theme.Border, 0);
            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 1f;
            layout.preferredHeight = 1f;
            return panel;
        }

        public static void AddLayoutElement(
            GameObject target,
            float minHeight = -1f,
            float preferredHeight = -1f,
            float flexibleHeight = -1f,
            float minWidth = -1f,
            float preferredWidth = -1f,
            float flexibleWidth = -1f)
        {
            var layout = target.GetComponent<LayoutElement>() ?? target.AddComponent<LayoutElement>();
            if (minHeight >= 0f)
            {
                layout.minHeight = minHeight;
            }

            if (preferredHeight >= 0f)
            {
                layout.preferredHeight = preferredHeight;
            }

            if (flexibleHeight >= 0f)
            {
                layout.flexibleHeight = flexibleHeight;
            }

            if (minWidth >= 0f)
            {
                layout.minWidth = minWidth;
            }

            if (preferredWidth >= 0f)
            {
                layout.preferredWidth = preferredWidth;
            }

            if (flexibleWidth >= 0f)
            {
                layout.flexibleWidth = flexibleWidth;
            }
        }

        public static void Stretch(
            RectTransform rect,
            float left = 0f,
            float bottom = 0f,
            float right = 0f,
            float top = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        public static void SetTopStretch(RectTransform rect, float height)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0f, height);
        }

        public static Sprite RoundedSprite(int radius)
        {
            radius = Mathf.Max(1, radius);
            if (RoundedSprites.TryGetValue(radius, out var cached))
            {
                return cached;
            }

            var size = radius * 2 + 4;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = $"RoundedRect_{radius}",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };

            var pixels = new Color32[size * size];
            var half = size / 2f;
            var inner = half - radius;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = Mathf.Max(0f, Mathf.Abs(x - half + 0.5f) - inner);
                    var dy = Mathf.Max(0f, Mathf.Abs(y - half + 0.5f) - inner);
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);
                    var alpha = Mathf.Clamp01(radius - distance + 0.5f);
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);

            var sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(radius, radius, radius, radius));
            sprite.name = texture.name;
            sprite.hideFlags = HideFlags.HideAndDontSave;
            RoundedSprites[radius] = sprite;
            return sprite;
        }

        public static void DestroyChildren(Transform parent)
        {
            for (var index = parent.childCount - 1; index >= 0; index--)
            {
                var child = parent.GetChild(index).gameObject;
                // Nonaktifkan lebih dulu agar layout tidak menghitung konten
                // lama pada frame yang sama ketika layar dibangun ulang.
                child.SetActive(false);
                if (Application.isPlaying)
                {
                    Object.Destroy(child);
                }
                else
                {
                    Object.DestroyImmediate(child);
                }
            }
        }

        public static void AddAccessibilityLabel(GameObject target, string label)
        {
            var accessible = target.GetComponent<AccessibleLabel>() ?? target.AddComponent<AccessibleLabel>();
            accessible.Label = label;
        }
    }
}
