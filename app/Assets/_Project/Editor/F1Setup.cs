using System.IO;
using TMPro;
using UabMeto.App.UI;
using UnityEditor;
using UnityEngine;

namespace UabMeto.Editor
{
    /// <summary>
    /// Menyiapkan aset F1: token tema, konten Resources, dan scene Bootstrap.
    /// Tidak menyalin samples/UNVALIDATED ke build.
    /// </summary>
    public static class F1Setup
    {
        public const string ThemeAssetPath = "Assets/_Project/Resources/UI/ThemeTokens.asset";
        public const string ContentResourceFolder = "Assets/_Project/Resources/Content";
        public const string LessonResourceFolder = ContentResourceFolder + "/lessons";

        [MenuItem("Uab Meto/F1 Setup")]
        public static void Prepare()
        {
            EnsureFontAssets();
            SyncContent();
            CreateThemeAsset();
            BootstrapSceneBuilder.Rebuild();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[UabMeto] F1 setup selesai.");
        }

        public static void EnsureFontAssets()
        {
            var probe = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/_Project/Resources/Fonts/Inter-Regular SDF.asset");
            if (probe == null)
            {
                FontAssetBuilder.BuildAll();
            }
        }

        public static void SyncContent()
        {
            var repositoryRoot = Path.GetFullPath(
                Path.Combine(Application.dataPath, "..", ".."));
            var sourceTopics = Path.Combine(repositoryRoot, "content", "path", "topics.json");
            var sourceLessons = Path.Combine(repositoryRoot, "content", "lessons");

            if (!File.Exists(sourceTopics))
            {
                throw new FileNotFoundException("topics.json tidak ditemukan", sourceTopics);
            }

            Directory.CreateDirectory(ContentResourceFolder);
            Directory.CreateDirectory(LessonResourceFolder);

            File.Copy(
                sourceTopics,
                Path.Combine(ContentResourceFolder, "topics.json"),
                true);
            File.WriteAllText(
                Path.Combine(LessonResourceFolder, ".gitkeep"),
                "Folder ini menampung pelajaran validated yang disalin dari content/lessons.\n");

            var sourceFiles = Directory.Exists(sourceLessons)
                ? Directory.GetFiles(sourceLessons, "*.json", SearchOption.TopDirectoryOnly)
                : System.Array.Empty<string>();

            var sourceNames = new System.Collections.Generic.HashSet<string>();
            foreach (var sourceFile in sourceFiles)
            {
                var fileName = Path.GetFileName(sourceFile);
                sourceNames.Add(fileName);
                File.Copy(sourceFile, Path.Combine(LessonResourceFolder, fileName), true);
            }

            foreach (var existing in Directory.GetFiles(LessonResourceFolder, "*.json"))
            {
                if (!sourceNames.Contains(Path.GetFileName(existing)))
                {
                    File.Delete(existing);
                }
            }

            AssetDatabase.Refresh();
        }

        public static void CreateThemeAsset()
        {
            Directory.CreateDirectory("Assets/_Project/Resources/UI");

            var theme = AssetDatabase.LoadAssetAtPath<ThemeTokens>(ThemeAssetPath);
            if (theme == null)
            {
                theme = ScriptableObject.CreateInstance<ThemeTokens>();
                AssetDatabase.CreateAsset(theme, ThemeAssetPath);
            }

            theme.Canvas = ThemeTokens.Hex("#0B1519");
            theme.SurfaceCard = ThemeTokens.Hex("#16242B");
            theme.SurfaceLow = ThemeTokens.Hex("#141D22");
            theme.Surface = ThemeTokens.Hex("#182126");
            theme.SurfaceHigh = ThemeTokens.Hex("#222B31");
            theme.SurfaceHighest = ThemeTokens.Hex("#2D363C");
            theme.Border = ThemeTokens.Hex("#223640");
            theme.Outline = ThemeTokens.Hex("#998F81");
            theme.TextPrimary = ThemeTokens.Hex("#F4EBDD");
            theme.TextMuted = ThemeTokens.Hex("#B8AEA0");
            theme.Accent = ThemeTokens.Hex("#F3D08F");
            theme.OnAccent = ThemeTokens.Hex("#0B1519");
            theme.AccentDeep = ThemeTokens.Hex("#C96F4A");
            theme.AccentDeepBg = ThemeTokens.Hex("#7B3313");
            theme.Success = ThemeTokens.Hex("#5FA97A");
            theme.Warning = ThemeTokens.Hex("#E39B5C");
            theme.DangerFill = ThemeTokens.Hex("#C6544C");
            theme.DangerText = ThemeTokens.Hex("#FFB4AB");
            theme.SuccessPanel = ThemeTokens.Hex("#132B20");
            theme.DangerPanel = ThemeTokens.Hex("#2C1415");
            theme.Scrim = new Color(0f, 0f, 0f, 0.8f);

            theme.BodyFont = LoadFont("Inter-Regular SDF");
            theme.BodyBoldFont = LoadFont("Inter-SemiBold SDF");
            theme.HeadingFont = LoadFont("PlusJakartaSans-Regular SDF");
            theme.HeadingBoldFont = LoadFont("PlusJakartaSans-Bold SDF");
            theme.IconFont = LoadFont("MaterialSymbolsOutlined SDF");

            EditorUtility.SetDirty(theme);
            AssetDatabase.SaveAssets();

            var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(
                "Assets/TextMesh Pro/Resources/TMP Settings.asset");
            if (settings != null && theme.BodyFont != null)
            {
                var serialized = new SerializedObject(settings);
                var defaultFont = serialized.FindProperty("m_defaultFontAsset");
                if (defaultFont != null)
                {
                    defaultFont.objectReferenceValue = theme.BodyFont;
                    serialized.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(settings);
                }
            }

            AssetDatabase.SaveAssets();
        }

        private static TMP_FontAsset LoadFont(string name)
        {
            var asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                $"Assets/_Project/Resources/Fonts/{name}.asset");
            if (asset == null)
            {
                throw new FileNotFoundException($"TMP font asset tidak ditemukan: {name}");
            }

            return asset;
        }
    }
}
