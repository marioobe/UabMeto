using System.IO;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace UabMeto.Tests.EditMode
{
    /// <summary>
    /// Uji glif nyata pada font yang dipilih. Bukan klaim cakupan penuh:
    /// hanya karakter yang diwajibkan pada keputusan F1.
    /// </summary>
    public sealed class FontGlyphTests
    {
        private const string FontFolder = "Assets/_Project/Resources/Fonts";
        private static readonly char[] RequiredCharacters =
        {
            '\u0101', '\u0113', '\u012B', '\u014D', '\u016B',
            '\u02BC', '\u2019', '\u2018', '\u0294',
            '\u00E9', '\u00E8', '\u00EA', '\u00F1'
        };

        private static TMP_FontAsset LoadFont(string name)
        {
            var asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{FontFolder}/{name} SDF.asset");
            Assert.IsNotNull(asset, $"Font asset tidak ditemukan: {name}");
            return asset;
        }

        [Test]
        public void Inter_HasRequiredGlyphs()
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(
                "Assets/ThirdParty/Fonts/Inter/Inter-Regular.ttf");
            Assert.IsNotNull(font);

            foreach (var character in RequiredCharacters)
            {
                Assert.IsTrue(
                    font.HasCharacter(character),
                    $"Inter tidak memuat U+{(int)character:X4}");
            }
        }

        [Test]
        public void TmpInter_HasRequiredGlyphs()
        {
            var font = LoadFont("Inter-Regular");

            foreach (var character in RequiredCharacters)
            {
                Assert.IsTrue(
                    font.HasCharacter(character, false, true),
                    $"TMP Inter tidak dapat menambahkan U+{(int)character:X4}");
            }
        }

        [Test]
        public void TmpJakarta_RequiresFallbackForIpaGlyphs()
        {
            var source = AssetDatabase.LoadAssetAtPath<Font>(
                "Assets/ThirdParty/Fonts/PlusJakartaSans/PlusJakartaSans-Bold.ttf");
            var inter = LoadFont("Inter-Regular");
            var fresh = TMP_FontAsset.CreateFontAsset(source);

            try
            {
                foreach (var character in new[] { '\u0101', '\u2019', '\u02BC', '\u0294' })
                {
                    Assert.IsFalse(
                        fresh.HasCharacter(character, false, false),
                        $"PJS langsung memuat U+{(int)character:X4}; fallback tidak diuji");
                }

                fresh.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset>
                {
                    inter
                };

                foreach (var character in new[] { '\u0101', '\u2019', '\u02BC', '\u0294' })
                {
                    Assert.IsTrue(
                        fresh.HasCharacter(character, true, true),
                        $"Fallback TMP tidak menemukan U+{(int)character:X4}");
                }
            }
            finally
            {
                Object.DestroyImmediate(fresh);
            }
        }

        [Test]
        public void TmpJakarta_UsesInterBoldFallback()
        {
            var jakarta = LoadFont("PlusJakartaSans-Bold");
            var interBold = LoadFont("Inter-Bold");

            Assert.IsNotNull(jakarta.fallbackFontAssetTable);
            Assert.Contains(interBold, jakarta.fallbackFontAssetTable);
            Assert.IsTrue(
                jakarta.HasCharacter('\u0294', true, true),
                "Fallback TMP tidak menemukan U+0294");
        }

        [Test]
        public void TextMeshPro_RendersRequiredCharacters()
        {
            var font = LoadFont("Inter-Regular");
            var go = new GameObject("GlyphProbe", typeof(TextMeshPro));
            var text = go.GetComponent<TextMeshPro>();

            try
            {
                text.font = font;
                text.text = new string(RequiredCharacters);
                text.ForceMeshUpdate();

                Assert.AreEqual(RequiredCharacters.Length, text.textInfo.characterCount);

                for (var i = 0; i < RequiredCharacters.Length; i++)
                {
                    Assert.AreEqual(
                        RequiredCharacters[i],
                        text.textInfo.characterInfo[i].character,
                        $"Karakter ke-{i} tidak dirender sesuai harapan");
                }
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void TextMeshPro_UsesFallbackForJakartaHeading()
        {
            var font = LoadFont("PlusJakartaSans-Bold");
            var go = new GameObject("FallbackProbe", typeof(TextMeshPro));
            var text = go.GetComponent<TextMeshPro>();

            try
            {
                text.font = font;
                text.text = "\u0294";
                text.ForceMeshUpdate();

                Assert.AreEqual(1, text.textInfo.characterCount);
                Assert.AreEqual('\u0294', text.textInfo.characterInfo[0].character);
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void MaterialSymbols_HasHomeCodepoint()
        {
            var font = LoadFont("MaterialSymbolsOutlined");
            var codepointLine = File.ReadAllLines("Assets/ThirdParty/Icons/codepoints.txt")
                .FirstOrDefault(line => line.StartsWith("home "));

            Assert.IsNotNull(codepointLine, "Codepoint 'home' tidak ditemukan");
            var codepoint = System.Convert.ToInt32(codepointLine.Split(' ')[1], 16);
            Assert.LessOrEqual(codepoint, 0xFFFF, "Codepoint home di luar BMP");

            Assert.IsTrue(
                font.HasCharacter((char)codepoint, false, true),
                $"Material Symbols tidak memuat codepoint home U+{codepoint:X4}");
        }
    }
}
