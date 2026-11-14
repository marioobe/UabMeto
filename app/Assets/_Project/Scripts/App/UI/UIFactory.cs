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
        private static readonly Dictionary<int, Sprite> OutlineSprites = new Dictionary<int, Sprite>();

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
            // Ukuran dari ThemeTokens adalah ukuran desain 1.0x. Skala font
            // sistem diterapkan di satu tempat ini agar seluruh layar ikut.
            text.fontSize = UiScale.Px(size);
            text.color = color;
            text.text = value;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;

            return text;
        }

        /// <summary>
        /// Mengizinkan teks mengecil sampai <paramref name="minSize"/> (satuan
        /// desain 1.0x) bila ruangnya sempit. Dipakai untuk label statistik
        /// yang lebarnya tetap terbagi tiga, supaya kata tidak dipotong di
        /// tengah saat pengguna memakai font sistem besar.
        /// </summary>
        public static void AllowShrinkToFit(TextMeshProUGUI text, float minSize)
        {
            text.fontSizeMin = UiScale.Px(minSize);
            text.fontSizeMax = text.fontSize;
            text.enableAutoSizing = true;
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

            if (!primary && !ghost)
            {
                // design-tokens §6: tombol sekunder memakai garis tepi.
                AddRoundedBorder(panel.rectTransform, theme, (int)theme.CornerRadius, 2);
            }

            panel.gameObject.AddComponent<PressScale>();

            var layout = panel.gameObject.AddComponent<LayoutElement>();
            // Tanpa preferredHeight tetap: tinggi tombol mengikuti label yang
            // membungkus pada skala font besar. minHeight menjaga target 48 dp.
            layout.minHeight = UiScale.Px(theme.TouchTarget);

            var group = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            group.padding = ScalePadding(new RectOffset(16, 12, 16, 12));
            group.spacing = 0f;
            group.childControlWidth = true;
            group.childControlHeight = true;
            group.childForceExpandWidth = true;
            group.childForceExpandHeight = true;
            group.childAlignment = TextAnchor.MiddleCenter;

            var text = CreateText(
                "Label",
                panel.transform,
                label,
                primary ? theme.HeadingBoldFont : theme.BodyBoldFont,
                theme.ButtonSize,
                primary ? theme.OnAccent : theme.TextPrimary,
                TextAlignmentOptions.Center);

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
            layout.minWidth = UiScale.Px(theme.TouchTarget);
            layout.minHeight = UiScale.Px(theme.TouchTarget);
            layout.preferredWidth = UiScale.Px(theme.TouchTarget);
            layout.preferredHeight = UiScale.Px(theme.TouchTarget);

            var button = panel.gameObject.AddComponent<Button>();
            button.targetGraphic = panel;
            button.transition = Selectable.Transition.ColorTint;
            panel.gameObject.AddComponent<PressScale>();

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
            layout.minHeight = UiScale.Px(52f);
            layout.preferredHeight = UiScale.Px(52f);

            var input = panel.gameObject.AddComponent<TMP_InputField>();
            input.targetGraphic = panel;
            input.textViewport = panel.rectTransform;
            input.lineType = TMP_InputField.LineType.SingleLine;
            input.characterLimit = characterLimit;
            input.caretWidth = Mathf.Max(2, Mathf.RoundToInt(UiScale.Px(2f)));
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
            Stretch(
                text.rectTransform,
                UiScale.Px(14f),
                UiScale.Px(8f),
                UiScale.Px(14f),
                UiScale.Px(8f));
            text.raycastTarget = true;

            var placeholderText = CreateText(
                "Placeholder",
                panel.transform,
                placeholder,
                theme.BodyFont,
                theme.BodySize,
                theme.TextMuted,
                TextAlignmentOptions.MidlineLeft);
            Stretch(
                placeholderText.rectTransform,
                UiScale.Px(14f),
                UiScale.Px(8f),
                UiScale.Px(14f),
                UiScale.Px(8f));
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
            layout.spacing = UiScale.Px(spacing);
            layout.padding = ScalePadding(padding);
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
            var radius = cornerRadius >= 0 ? cornerRadius : (int)theme.Space12;
            var card = CreatePanel(name, parent, color, radius);
            AddRoundedBorder(card.rectTransform, theme, radius);

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
            layout.spacing = UiScale.Px(spacing);
            layout.padding = ScalePadding(padding);
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
            layout.spacing = UiScale.Px(theme.Space12);
            layout.padding = ScalePadding(new RectOffset(16, 16, 16, 24));
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
            layout.minHeight = UiScale.Px(height);
            layout.preferredHeight = UiScale.Px(height);
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
                layout.minHeight = UiScale.Px(minHeight);
            }

            if (preferredHeight >= 0f)
            {
                layout.preferredHeight = UiScale.Px(preferredHeight);
            }

            if (flexibleHeight >= 0f)
            {
                layout.flexibleHeight = flexibleHeight;
            }

            if (minWidth >= 0f)
            {
                layout.minWidth = UiScale.Px(minWidth);
            }

            if (preferredWidth >= 0f)
            {
                layout.preferredWidth = UiScale.Px(preferredWidth);
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
            rect.sizeDelta = new Vector2(0f, UiScale.Px(height));
        }

        /// <summary>Ukuran desain (dp) ke unit kanvas setelah skala font.</summary>
        public static float Px(float value) => UiScale.Px(value);

        /// <summary>Salinan padding yang sudah diskalakan.</summary>
        public static RectOffset ScalePadding(RectOffset padding)
        {
            if (padding == null)
            {
                return new RectOffset();
            }

            return new RectOffset(
                Mathf.RoundToInt(UiScale.Px(padding.left)),
                Mathf.RoundToInt(UiScale.Px(padding.right)),
                Mathf.RoundToInt(UiScale.Px(padding.top)),
                Mathf.RoundToInt(UiScale.Px(padding.bottom)));
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

        /// <summary>
        /// Garis tepi 1 dp di atas kartu. Dibuat sebagai anak dengan
        /// <c>ignoreLayout</c> agar hierarki <c>Body</c> dan ukuran kartu tidak
        /// berubah, sehingga tes layout yang membaca <c>Body</c> tetap sah.
        /// </summary>
        public static void AddRoundedBorder(
            RectTransform target,
            ThemeTokens theme,
            int radius,
            int thickness = 1)
        {
            if (target == null || target.Find("Border") != null)
            {
                return;
            }

            var border = CreatePanel("Border", target, theme.Border, 0);
            border.sprite = RoundedOutlineSprite(radius, thickness);
            border.type = Image.Type.Sliced;
            border.raycastTarget = false;
            Stretch(border.rectTransform);

            var layout = border.gameObject.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;

            border.transform.SetAsFirstSibling();
        }

        /// <summary>
        /// Bilah progres datar: lintasan <c>surface-high</c> dan isi <c>accent</c>.
        /// Nilai 0-1 dipetakan ke lebar lewat anchor, tanpa shader atau mask.
        /// </summary>
        public static RectTransform CreateProgressBar(
            string name,
            Transform parent,
            ThemeTokens theme,
            float value,
            float height = 6f)
        {
            value = Mathf.Clamp01(value);
            height = UiScale.Px(height);
            var radius = Mathf.Max(1, Mathf.RoundToInt(height / 2f));
            var track = CreatePanel(name, parent, theme.SurfaceHigh, radius);
            var layout = track.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = height;
            layout.preferredHeight = height;
            layout.flexibleWidth = 1f;

            if (value > 0f)
            {
                var fill = CreatePanel("Fill", track.transform, theme.Accent, radius);
                fill.raycastTarget = false;
                var rect = fill.rectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = new Vector2(value, 1f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }

            return track.rectTransform;
        }

        /// <summary>
        /// Pemisah motif belah ketupat 4 dp pada garis 1 dp
        /// (design-tokens §1 butir 4). Dekoratif, tanpa teks.
        /// </summary>
        public static RectTransform CreateMotifDivider(
            string name,
            Transform parent,
            ThemeTokens theme)
        {
            var row = CreateHorizontalGroup(name, parent, theme.Space8);
            row.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
            AddLayoutElement(row.gameObject, minHeight: 10f, preferredHeight: 10f);

            CreateHairline(row, theme);
            CreateDiamond(row, theme);
            CreateDiamond(row, theme);
            CreateDiamond(row, theme);
            CreateHairline(row, theme);

            return row;
        }

        private static void CreateHairline(Transform parent, ThemeTokens theme)
        {
            var line = CreatePanel("Line", parent, theme.Border, 0);
            line.raycastTarget = false;
            var layout = line.gameObject.AddComponent<LayoutElement>();
            layout.flexibleWidth = 1f;
            layout.minWidth = 8f;
            layout.minHeight = 1f;
            layout.preferredHeight = 1f;
        }

        private static void CreateDiamond(Transform parent, ThemeTokens theme)
        {
            var diamond = CreatePanel("Diamond", parent, theme.Border, 0);
            diamond.raycastTarget = false;
            diamond.rectTransform.localEulerAngles = new Vector3(0f, 0f, 45f);
            var layout = diamond.gameObject.AddComponent<LayoutElement>();
            layout.minWidth = 4f;
            layout.preferredWidth = 4f;
            layout.minHeight = 4f;
            layout.preferredHeight = 4f;
        }

        public static Sprite RoundedOutlineSprite(int radius, int thickness = 1)
        {
            radius = Mathf.Max(1, radius);
            thickness = Mathf.Clamp(thickness, 1, radius);
            var key = (radius * 16) + thickness;
            if (OutlineSprites.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var size = radius * 2 + 4;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = $"RoundedOutline_{radius}_{thickness}",
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
                    // Jarak bertanda terhadap tepi luar: 0 tepat di tepi.
                    var signed = Mathf.Sqrt(dx * dx + dy * dy) - radius;
                    var outer = Mathf.Clamp01(-signed + 0.5f);
                    var innerAlpha = Mathf.Clamp01(-signed - thickness + 0.5f);
                    pixels[y * size + x] = new Color32(
                        255,
                        255,
                        255,
                        (byte)(Mathf.Clamp01(outer - innerAlpha) * 255f));
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
            OutlineSprites[key] = sprite;
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
