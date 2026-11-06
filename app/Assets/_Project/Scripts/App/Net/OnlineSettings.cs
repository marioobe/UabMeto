using System;
using System.IO;
using UnityEngine;

namespace UabMeto.App.Net
{
    [Serializable]
    public sealed class OnlineSettingsData
    {
        public bool enabled;
        public string baseUrl = OnlineSettings.DefaultBaseUrl;
    }

    /// <summary>
    /// Pengaturan mode online lokal. Nonaktif secara bawaan agar build tidak
    /// menghubungi jaringan tanpa persetujuan penguji.
    /// </summary>
    public static class OnlineSettings
    {
        public const string DefaultBaseUrl = "http://127.0.0.1:8787";
        public const string FileName = "online.json";

        public static OnlineSettingsData LoadDefault()
        {
            var directory = Path.Combine(Application.persistentDataPath, "uabmeto");
            return Load(Path.Combine(directory, FileName));
        }

        public static OnlineSettingsData Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var data = JsonUtility.FromJson<OnlineSettingsData>(
                        File.ReadAllText(path));

                    if (data != null)
                    {
                        if (string.IsNullOrWhiteSpace(data.baseUrl))
                        {
                            data.baseUrl = DefaultBaseUrl;
                        }

                        return data;
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(
                        "[UabMeto] Pengaturan online rusak, memakai bawaan: " +
                        exception.Message);
                }
            }

            return new OnlineSettingsData();
        }

        public static void Save(string path, OnlineSettingsData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, JsonUtility.ToJson(data, true));
        }
    }
}
