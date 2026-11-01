using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UabMeto.Editor
{
    /// <summary>
    /// Membuat scene bootstrap secara terprogram agar scene dapat diregenerasi
    /// dan tidak bergantung pada file .unity yang disunting manual.
    /// </summary>
    public static class BootstrapSceneBuilder
    {
        public const string SceneDirectory = "Assets/_Project/Scenes";
        public const string ScenePath = SceneDirectory + "/Bootstrap.unity";

        [MenuItem("Uab Meto/Rebuild Bootstrap Scene")]
        public static void Rebuild()
        {
            CreateScene();
        }

        public static string EnsureScene()
        {
            if (!File.Exists(ScenePath))
            {
                CreateScene();
            }

            return ScenePath;
        }

        private static void CreateScene()
        {
            Directory.CreateDirectory(SceneDirectory);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = UabMeto.App.UI.ThemeTokens.Hex("#0B1519");
            camera.orthographic = true;
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            var eventSystem = new GameObject(
                "EventSystem",
                typeof(EventSystem),
                typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(null);

            var bootstrapObject = new GameObject("AppRoot");
            bootstrapObject.AddComponent<UabMeto.App.AppRoot>();

            EditorSceneManager.MarkSceneDirty(scene);

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                throw new IOException("Gagal menyimpan scene bootstrap: " + ScenePath);
            }

            AssetDatabase.Refresh();
            Debug.Log("[UabMeto] Scene bootstrap dibuat: " + ScenePath);
        }
    }
}
