using System;
using System.IO;
using UnityEngine;

namespace UabMeto.App.Local
{
    [Serializable]
    public sealed class ProfileData
    {
        public string nickname;
        public string createdAtUtc;
    }

    /// <summary>
    /// Profil lokal F1. Nickname disimpan di perangkat; belum ada akun server.
    /// </summary>
    public sealed class LocalProfile
    {
        public const int MinimumNicknameLength = 3;
        public const int MaximumNicknameLength = 20;

        private readonly string _path;

        private LocalProfile(string path, ProfileData data)
        {
            _path = path;
            Data = data ?? new ProfileData();
        }

        public ProfileData Data { get; }

        public string Nickname => Data.nickname ?? string.Empty;

        public bool IsValid => ValidateNickname(Nickname) == null;

        public static LocalProfile LoadDefault()
        {
            var directory = Path.Combine(Application.persistentDataPath, "uabmeto");
            return Load(Path.Combine(directory, "profile.json"));
        }

        public static LocalProfile Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var data = JsonUtility.FromJson<ProfileData>(File.ReadAllText(path));
                    if (data != null)
                    {
                        return new LocalProfile(path, data);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("[UabMeto] Profil lokal rusak, dibuat baru: " + exception.Message);
                }
            }

            return new LocalProfile(path, new ProfileData());
        }

        public bool TrySetNickname(string nickname, out string error)
        {
            var trimmed = (nickname ?? string.Empty).Trim();
            error = ValidateNickname(trimmed);
            if (error != null)
            {
                return false;
            }

            Data.nickname = trimmed;
            if (string.IsNullOrWhiteSpace(Data.createdAtUtc))
            {
                Data.createdAtUtc = DateTime.UtcNow.ToString("O");
            }

            Save();
            return true;
        }

        public void Save()
        {
            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(_path, JsonUtility.ToJson(Data, true));
        }

        public static string ValidateNickname(string nickname)
        {
            var trimmed = (nickname ?? string.Empty).Trim();
            if (trimmed.Length < MinimumNicknameLength)
            {
                return $"Nama panggilan minimal {MinimumNicknameLength} karakter.";
            }

            if (trimmed.Length > MaximumNicknameLength)
            {
                return $"Nama panggilan maksimal {MaximumNicknameLength} karakter.";
            }

            foreach (var character in trimmed)
            {
                if (char.IsControl(character))
                {
                    return "Nama panggilan tidak boleh memuat karakter kontrol.";
                }
            }

            return null;
        }
    }
}
