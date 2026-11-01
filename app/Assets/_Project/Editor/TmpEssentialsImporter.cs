using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace UabMeto.Editor
{
    /// <summary>
    /// Mengimpor TMP Essential Resources (shader, settings, style sheet) secara
    /// non-interaktif. Dijalankan sekali sebelum font asset dibuat.
    ///
    /// Pemakaian batchmode (tanpa -quit, metode ini keluar sendiri):
    ///   Unity.exe -batchmode -nographics -projectPath app \
    ///     -executeMethod UabMeto.Editor.TmpEssentialsImporter.Import
    /// </summary>
    public static class TmpEssentialsImporter
    {
        private const string ShaderPath = "Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile.shader";
        private static double s_StartedAt;

        public static void Import()
        {
            if (File.Exists(ShaderPath))
            {
                Debug.Log("[UabMeto] TMP Essential Resources sudah ada.");
                EditorApplication.Exit(0);
                return;
            }

            var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                typeof(TMP_Settings).Assembly);
            if (package == null)
            {
                Debug.LogError("[UabMeto] Paket com.unity.ugui tidak ditemukan.");
                EditorApplication.Exit(1);
                return;
            }

            var packagePath = Path.Combine(
                package.resolvedPath,
                "Package Resources",
                "TMP Essential Resources.unitypackage");

            if (!File.Exists(packagePath))
            {
                Debug.LogError($"[UabMeto] Berkas tidak ditemukan: {packagePath}");
                EditorApplication.Exit(1);
                return;
            }

            s_StartedAt = EditorApplication.timeSinceStartup;
            UnityEditor.AssetPackage.Package.Import(packagePath, false);
            EditorApplication.update += WaitForImport;
        }

        private static void WaitForImport()
        {
            if (File.Exists(ShaderPath))
            {
                EditorApplication.update -= WaitForImport;
                AssetDatabase.Refresh();
                Debug.Log("[UabMeto] TMP Essential Resources berhasil diimpor.");
                EditorApplication.Exit(0);
                return;
            }

            if (EditorApplication.timeSinceStartup - s_StartedAt > 120)
            {
                EditorApplication.update -= WaitForImport;
                Debug.LogError("[UabMeto] Impor TMP Essential Resources melewati batas waktu.");
                EditorApplication.Exit(1);
            }
        }
    }
}
