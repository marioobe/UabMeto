using System;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UabMeto.Editor
{
    /// <summary>
    /// Entry point build APK lokal (F1). Dijalankan via:
    /// Unity.exe -batchmode -quit -projectPath app -executeMethod UabMeto.Editor.BuildScript.BuildAndroid
    /// BuildAndroid selalu menyalin hanya pelajaran validated/published.
    /// BuildAndroidPreview dipakai khusus demo/UAS dan memuat in_review.
    /// </summary>
    public static class BuildScript
    {
        private const string OutputFolderName = "Builds/Android";
        private const string OutputFileName = "uabmeto-f1.apk";
        private const string PreviewOutputFileName = "uabmeto-f1-preview.apk";
        private const string SchoolUniversalOutputFileName = "uabmeto-sekolah-universal.apk";

        private static string RepositoryRoot =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));

        private static string OutputDirectory =>
            Path.Combine(RepositoryRoot, OutputFolderName);

        public static void BuildAndroid()
        {
            BuildAndroidInternal(includePreview: false, OutputFileName, universal: false);
        }

        /// <summary>
        /// Build cadangan untuk uji sekolah: ARMv7 + ARM64. Versi dan konten
        /// sama dengan build rilis; hanya ABI yang diperluas agar dapat
        /// dipasang di perangkat 32-bit. Bukan build rilis.
        /// </summary>
        public static void BuildAndroidSchoolUniversal()
        {
            BuildAndroidInternal(
                includePreview: false,
                SchoolUniversalOutputFileName,
                universal: true);
        }

        /// <summary>
        /// Build demo/UAS dengan materi in_review. Jangan dipakai sebagai
        /// build rilis.
        /// </summary>
        public static void BuildAndroidPreview()
        {
            BuildAndroidInternal(includePreview: true, PreviewOutputFileName, universal: false);
        }

        private static void BuildAndroidInternal(
            bool includePreview,
            string fileName,
            bool universal)
        {
            try
            {
                F1Setup.SyncContent(includePreview);
                ProjectConfigurator.Apply();

                if (universal)
                {
                    PlayerSettings.Android.targetArchitectures =
                        AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
                    Debug.Log("[UabMeto] Build uji sekolah: ARMv7 + ARM64");
                }

                var scenePath = BootstrapSceneBuilder.EnsureScene();
                EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };

                Directory.CreateDirectory(OutputDirectory);
                var outputPath = Path.Combine(OutputDirectory, fileName);
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }

                var options = new BuildPlayerOptions
                {
                    scenes = new[] { scenePath },
                    locationPathName = outputPath,
                    target = BuildTarget.Android,
                    options = BuildOptions.None
                };

                var startedAt = DateTime.UtcNow;
                var report = BuildPipeline.BuildPlayer(options);
                var elapsed = DateTime.UtcNow - startedAt;
                var summary = report.summary;
                var actualSize = File.Exists(outputPath) ? new FileInfo(outputPath).Length : 0;

                Debug.Log(
                    $"[UabMeto] Build result={summary.result} " +
                    $"apkSize={actualSize} bytes " +
                    $"duration={elapsed.TotalSeconds.ToString("F1", CultureInfo.InvariantCulture)}s " +
                    $"errors={summary.totalErrors} warnings={summary.totalWarnings}");

                WriteSummary(summary, elapsed, outputPath, actualSize);

                EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 1);
            }
            catch (Exception exception)
            {
                Debug.LogError("[UabMeto] Build gagal: " + exception);
                EditorApplication.Exit(1);
            }
        }

        private static void WriteSummary(
            BuildSummary summary,
            TimeSpan elapsed,
            string outputPath,
            long actualSize)
        {
            var json =
                "{\n" +
                $"  \"result\": \"{summary.result}\",\n" +
                $"  \"outputPath\": \"{outputPath.Replace("\\", "/")}\",\n" +
                $"  \"apkSizeBytes\": {actualSize},\n" +
                $"  \"reportedTotalSizeBytes\": {summary.totalSize},\n" +
                $"  \"totalErrors\": {summary.totalErrors},\n" +
                $"  \"totalWarnings\": {summary.totalWarnings},\n" +
                $"  \"durationSeconds\": {elapsed.TotalSeconds.ToString("F1", CultureInfo.InvariantCulture)},\n" +
                $"  \"unityVersion\": \"{Application.unityVersion}\",\n" +
                $"  \"builtAtUtc\": \"{DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)}\"\n" +
                "}\n";

            File.WriteAllText(Path.Combine(OutputDirectory, "build-summary.json"), json);
        }
    }
}
