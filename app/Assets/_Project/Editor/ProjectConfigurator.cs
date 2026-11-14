using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace UabMeto.Editor
{
    /// <summary>
    /// Konfigurasi proyek Android yang dapat diulang (reproducible).
    /// Nilai di sini adalah baseline F0, bukan keputusan rilis final.
    /// </summary>
    public static class ProjectConfigurator
    {
        public const string CompanyName = "Uab Meto Project";
        public const string ProductName = "Uab Meto";
        public const string ApplicationIdentifier = "id.uabmeto.app";
        public const string BundleVersion = "0.5.0";

        private const int MinimumApiLevel = 26;  // Android 8.0
        private const int TargetApiLevel = 36;   // Google Play requirement (2026)

        [MenuItem("Uab Meto/Configure Project (Android)")]
        public static void Apply()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(
                    BuildTargetGroup.Android,
                    BuildTarget.Android);
            }

            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;
            PlayerSettings.bundleVersion = BundleVersion;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationIdentifier);

            // ARM64 memerlukan IL2CPP. Mono tidak dipakai untuk build Android
            // karena kombinasi Mono+ARM64 ditolak oleh pipeline build.
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetIl2CppCompilerConfiguration(
                NamedBuildTarget.Android,
                Il2CppCompilerConfiguration.Release);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.bundleVersionCode = 6;
            PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)MinimumApiLevel;
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)TargetApiLevel;

            // F0 memakai debug keystore. Keystore rilis tidak dibuat pada tahap ini.
            PlayerSettings.Android.useCustomKeystore = false;

            // F1 dikunci portrait-only atas keputusan pemilik 2026-11-12.
            // Uji rotasi landscape D1-05 dicatat sebagai waiver, bukan lulus.
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;

            PlayerSettings.colorSpace = ColorSpace.Linear;
            EditorUserBuildSettings.buildAppBundle = false;

            AssetDatabase.SaveAssets();
            Debug.Log(
                $"[UabMeto] Konfigurasi Android: id={ApplicationIdentifier}, " +
                $"minSdk={MinimumApiLevel}, targetSdk={TargetApiLevel}, " +
                $"arch={PlayerSettings.Android.targetArchitectures}, IL2CPP");
        }
    }
}
