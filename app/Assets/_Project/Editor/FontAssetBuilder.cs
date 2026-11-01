using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace UabMeto.Editor
{
    /// <summary>
    /// Membuat TMP font asset dari berkas TTF resmi yang disertakan di
    /// Assets/ThirdParty. Tidak ada unduhan saat runtime.
    /// </summary>
    public static class FontAssetBuilder
    {
        private const string OutputFolder = "Assets/_Project/Resources/Fonts";
        private const string InterFolder = "Assets/ThirdParty/Fonts/Inter";
        private const string JakartaFolder = "Assets/ThirdParty/Fonts/PlusJakartaSans";
        private const string NotoFolder = "Assets/ThirdParty/Fonts/NotoSans";
        private const string IconFolder = "Assets/ThirdParty/Icons";

        [MenuItem("Uab Meto/Build Font Assets")]
        public static void BuildAll()
        {
            EnsureTmpEssentials();
            Directory.CreateDirectory(OutputFolder);

            var interRegular = BuildFontAsset(
                $"{InterFolder}/Inter-Regular.ttf",
                $"{OutputFolder}/Inter-Regular SDF.asset");
            var interSemiBold = BuildFontAsset(
                $"{InterFolder}/Inter-SemiBold.ttf",
                $"{OutputFolder}/Inter-SemiBold SDF.asset");
            var interBold = BuildFontAsset(
                $"{InterFolder}/Inter-Bold.ttf",
                $"{OutputFolder}/Inter-Bold SDF.asset");
            var jakartaRegular = BuildFontAsset(
                $"{JakartaFolder}/PlusJakartaSans-Regular.ttf",
                $"{OutputFolder}/PlusJakartaSans-Regular SDF.asset");
            var jakartaBold = BuildFontAsset(
                $"{JakartaFolder}/PlusJakartaSans-Bold.ttf",
                $"{OutputFolder}/PlusJakartaSans-Bold SDF.asset");
            var notoRegular = BuildFontAsset(
                $"{NotoFolder}/NotoSans-Regular.ttf",
                $"{OutputFolder}/NotoSans-Regular SDF.asset");
            var icons = BuildFontAsset(
                $"{IconFolder}/MaterialSymbolsOutlined.ttf",
                $"{OutputFolder}/MaterialSymbolsOutlined SDF.asset");

            SetFallbacks(interRegular, new[] { notoRegular });
            SetFallbacks(interSemiBold, new[] { notoRegular });
            SetFallbacks(interBold, new[] { notoRegular });
            SetFallbacks(jakartaRegular, new[] { interRegular, notoRegular });
            SetFallbacks(jakartaBold, new[] { interBold, notoRegular });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                "[UabMeto] Font assets dibuat: Inter, Plus Jakarta Sans, Noto Sans, Material Symbols. " +
                $"Ikon={icons.name}");
        }

        private static void EnsureTmpEssentials()
        {
            const string resourcesFolder = "Assets/TextMesh Pro/Resources";
            const string settingsPath = resourcesFolder + "/TMP Settings.asset";

            if (AssetDatabase.LoadAssetAtPath<TMP_Settings>(settingsPath) != null)
            {
                return;
            }

            Directory.CreateDirectory(resourcesFolder);

            var settings = ScriptableObject.CreateInstance<TMP_Settings>();
            AssetDatabase.CreateAsset(settings, settingsPath);

            var serialized = new SerializedObject(settings);
            var version = serialized.FindProperty("assetVersion");
            if (version != null)
            {
                version.stringValue = "2";
            }

            var fontPath = serialized.FindProperty("m_defaultFontAssetPath");
            if (fontPath != null)
            {
                fontPath.stringValue = "Fonts/";
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (TMP_Settings.LoadDefaultSettings() == null)
            {
                throw new InvalidDataException(
                    "TMP Settings tidak dapat dimuat. Periksa Assets/TextMesh Pro/Resources/TMP Settings.asset.");
            }
        }

        private static TMP_FontAsset BuildFontAsset(string sourcePath, string outputPath)
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(sourcePath);
            if (font == null)
            {
                throw new FileNotFoundException($"Font tidak ditemukan: {sourcePath}");
            }

            var fontAsset = TMP_FontAsset.CreateFontAsset(
                font,
                90,
                9,
                GlyphRenderMode.SDFAA,
                1024,
                1024,
                AtlasPopulationMode.Dynamic,
                true);

            if (fontAsset == null)
            {
                throw new InvalidDataException($"Gagal membuat TMP font asset dari {sourcePath}");
            }

            fontAsset.name = Path.GetFileNameWithoutExtension(outputPath);

            if (File.Exists(outputPath))
            {
                AssetDatabase.DeleteAsset(outputPath);
            }

            AssetDatabase.CreateAsset(fontAsset, outputPath);

            if (fontAsset.material != null)
            {
                fontAsset.material.name = fontAsset.name + " Material";
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            }

            if (fontAsset.atlasTextures != null)
            {
                for (var i = 0; i < fontAsset.atlasTextures.Length; i++)
                {
                    var texture = fontAsset.atlasTextures[i];
                    if (texture == null)
                    {
                        continue;
                    }

                    texture.name = fontAsset.name + " Atlas " + i;
                    AssetDatabase.AddObjectToAsset(texture, fontAsset);
                }
            }

            EditorUtility.SetDirty(fontAsset);
            return fontAsset;
        }

        private static void SetFallbacks(TMP_FontAsset fontAsset, TMP_FontAsset[] fallbacks)
        {
            if (fontAsset == null)
            {
                return;
            }

            fontAsset.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset>();
            foreach (var fallback in fallbacks)
            {
                if (fallback != null)
                {
                    fontAsset.fallbackFontAssetTable.Add(fallback);
                }
            }

            EditorUtility.SetDirty(fontAsset);
        }
    }
}
