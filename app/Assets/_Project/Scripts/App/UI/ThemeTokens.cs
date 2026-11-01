using TMPro;
using UnityEngine;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Satu sumber token visual. Warna, font, dan ukuran dibaca dari aset ini;
    /// kode UI tidak boleh memuat hex mentah.
    /// </summary>
    [CreateAssetMenu(menuName = "Uab Meto/Theme Tokens", fileName = "ThemeTokens")]
    public sealed class ThemeTokens : ScriptableObject
    {
        [Header("Warna")]
        public Color Canvas = Hex("#0B1519");
        public Color SurfaceCard = Hex("#16242B");
        public Color SurfaceLow = Hex("#141D22");
        public Color Surface = Hex("#182126");
        public Color SurfaceHigh = Hex("#222B31");
        public Color SurfaceHighest = Hex("#2D363C");
        public Color Border = Hex("#223640");
        public Color Outline = Hex("#998F81");
        public Color TextPrimary = Hex("#F4EBDD");
        public Color TextMuted = Hex("#B8AEA0");
        public Color Accent = Hex("#F3D08F");
        public Color OnAccent = Hex("#0B1519");
        public Color AccentDeep = Hex("#C96F4A");
        public Color AccentDeepBg = Hex("#7B3313");
        public Color Success = Hex("#5FA97A");
        public Color Warning = Hex("#E39B5C");
        public Color DangerFill = Hex("#C6544C");
        public Color DangerText = Hex("#FFB4AB");
        public Color SuccessPanel = Hex("#132B20");
        public Color DangerPanel = Hex("#2C1415");
        public Color Scrim = new Color(0f, 0f, 0f, 0.8f);

        [Header("Font")]
        public TMP_FontAsset BodyFont;
        public TMP_FontAsset BodyBoldFont;
        public TMP_FontAsset HeadingFont;
        public TMP_FontAsset HeadingBoldFont;
        public TMP_FontAsset IconFont;

        [Header("Ukuran teks")]
        public float BodySize = 16f;
        public float BodySmallSize = 14f;
        public float LabelSize = 12f;
        public float Heading1Size = 24f;
        public float Heading2Size = 20f;
        public float Heading3Size = 16f;
        public float ButtonSize = 16f;
        public float IconSize = 24f;

        [Header("Spasi dan target sentuh")]
        public float Space4 = 4f;
        public float Space8 = 8f;
        public float Space12 = 12f;
        public float Space16 = 16f;
        public float Space24 = 24f;
        public float Space32 = 32f;
        public float TouchTarget = 48f;
        public float CornerRadius = 16f;

        public static ThemeTokens Load()
        {
            var theme = Resources.Load<ThemeTokens>("UI/ThemeTokens");
            if (theme == null)
            {
                throw new MissingReferenceException(
                    "ThemeTokens tidak ditemukan di Resources/UI/ThemeTokens.asset. " +
                    "Jalankan Uab Meto/F1 Setup.");
            }

            return theme;
        }

        public static Color Hex(string value)
        {
            return ColorUtility.TryParseHtmlString(value, out var color) ? color : Color.magenta;
        }
    }
}
