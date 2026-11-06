using System;
using System.IO;
using UnityEngine;

namespace UabMeto.App.Net
{
    [Serializable]
    public sealed class OnlineIdentityData
    {
        public string userId;
    }

    /// <summary>
    /// Identitas pengembangan lokal untuk header <c>X-Dev-User</c>.
    /// Ini <b>bukan</b> autentikasi dan bukan akun pengguna sungguhan.
    /// </summary>
    public static class OnlineIdentity
    {
        public const string FileName = "online-identity.json";

        public static string LoadOrCreateDefault()
        {
            var directory = Path.Combine(Application.persistentDataPath, "uabmeto");
            return LoadOrCreate(Path.Combine(directory, FileName));
        }

        public static string LoadOrCreate(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var data = JsonUtility.FromJson<OnlineIdentityData>(
                        File.ReadAllText(path));

                    if (data != null && IsValid(data.userId))
                    {
                        return data.userId;
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(
                        "[UabMeto] Identitas online rusak, dibuat baru: " +
                        exception.Message);
                }
            }

            var created = "dev-" + Guid.NewGuid().ToString("N").Substring(0, 20);
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(
                path,
                JsonUtility.ToJson(new OnlineIdentityData { userId = created }, true));

            return created;
        }

        public static bool IsValid(string userId)
        {
            var trimmed = (userId ?? string.Empty).Trim();
            if (trimmed.Length < 3 || trimmed.Length > 64)
            {
                return false;
            }

            foreach (var character in trimmed)
            {
                var allowed =
                    (character >= 'A' && character <= 'Z') ||
                    (character >= 'a' && character <= 'z') ||
                    (character >= '0' && character <= '9') ||
                    character == '.' ||
                    character == '_' ||
                    character == '-';

                if (!allowed)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
