using TMPro;
using UabMeto.App.Local;
using UnityEngine;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>Profil lokal F1. Tidak ada akun, logout, atau klaim server.</summary>
    public sealed class ProfileView
    {
        private readonly ShellView _shell;

        public ProfileView(ShellView shell)
        {
            _shell = shell;
        }

        public void Build(RectTransform parent)
        {
            var theme = _shell.Theme;
            var scroll = UIFactory.CreateScrollView("ProfileScroll", parent, theme);
            UIFactory.Stretch(scroll.GetComponent<RectTransform>());
            var content = scroll.content;

            BuildIdentity(content);
            BuildStats(content);
            BuildOutbox(content);
            BuildOnline(content);
            BuildAbout(content);
        }

        private void BuildIdentity(RectTransform parent)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("Identity", parent, theme.SurfaceCard, (int)theme.CornerRadius);

            // Kartu ini harus tumbuh mengikuti isi. Tinggi tetap membuat teks
            // bertumpuk saat lebar efektif berbeda dari tangkapan Editor.
            var cardLayout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            cardLayout.spacing = UIFactory.Px(theme.Space8);
            cardLayout.padding = UIFactory.ScalePadding(new RectOffset(20, 20, 20, 20));
            cardLayout.childControlWidth = true;
            cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = false;
            cardLayout.childForceExpandHeight = false;
            cardLayout.childAlignment = TextAnchor.MiddleCenter;

            var avatar = UIFactory.CreatePanel("Avatar", card.transform, theme.SurfaceHigh, 32);
            var avatarLayout = avatar.gameObject.AddComponent<LayoutElement>();
            avatarLayout.minWidth = UIFactory.Px(64f);
            avatarLayout.preferredWidth = UIFactory.Px(64f);
            avatarLayout.minHeight = UIFactory.Px(64f);
            avatarLayout.preferredHeight = UIFactory.Px(64f);

            var icon = UIFactory.CreateIcon(
                "Icon",
                avatar.transform,
                "person",
                theme.IconFont,
                30f,
                theme.Accent);
            UIFactory.Stretch(icon.rectTransform);

            var nickname = _shell.Profile.IsValid ? _shell.Profile.Nickname : "Pelajar";
            var nicknameText = UIFactory.CreateText(
                "Nickname",
                card.transform,
                nickname,
                theme.HeadingBoldFont,
                theme.Heading2Size,
                theme.TextPrimary,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(nicknameText.gameObject, flexibleWidth: 1f);

            var note = UIFactory.CreateText(
                "Note",
                card.transform,
                "Nama panggilan disimpan di perangkat. Belum ada akun server.",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(note.gameObject, flexibleWidth: 1f);

            var edit = UIFactory.CreateButton(
                "Edit",
                card.transform,
                "Ubah nama",
                theme,
                _shell.ShowNicknameDialog,
                primary: false);
            UIFactory.AddLayoutElement(edit.gameObject, flexibleWidth: 1f);
        }

        private void BuildStats(RectTransform parent)
        {
            var theme = _shell.Theme;
            var row = UIFactory.CreateHorizontalGroup("ProfileStats", parent, theme.Space8);
            UIFactory.AddLayoutElement(
                row.gameObject,
                minHeight: 84f,
                flexibleWidth: 1f);

            CreateStat(row, "Pelajaran selesai", _shell.Progress.Data.completedLessonIds.Count.ToString());
            CreateStat(row, "XP tertunda", _shell.Progress.PendingXp.ToString());
            CreateStat(row, "Antrean", _shell.Outbox.PendingCount.ToString());
        }

        private void CreateStat(Transform parent, string label, string value)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("Stat_" + label, parent, theme.SurfaceCard, (int)theme.Space12);
            var cardLayout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            cardLayout.spacing = UIFactory.Px(theme.Space4);
            cardLayout.padding = UIFactory.ScalePadding(new RectOffset(8, 8, 8, 8));
            cardLayout.childControlWidth = true;
            cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = true;
            cardLayout.childForceExpandHeight = false;
            cardLayout.childAlignment = TextAnchor.MiddleCenter;

            var layout = card.gameObject.AddComponent<LayoutElement>();
            layout.flexibleWidth = 1f;
            // Lebar ketiga kartu harus sama. Tanpa ini, lebar pilihan teks
            // (preferred width) membuat kartu 1 lebih lebar dan kartu 3
            // menyempit sampai label terpotong di tengah kata pada font besar.
            layout.minWidth = 0f;
            layout.preferredWidth = 0f;
            layout.minHeight = UIFactory.Px(84f);

            UIFactory.CreateText(
                "Value",
                card.transform,
                value,
                theme.HeadingBoldFont,
                theme.Heading2Size,
                theme.TextPrimary,
                TextAlignmentOptions.Center);

            var labelText = UIFactory.CreateText(
                "Label",
                card.transform,
                label,
                theme.BodyFont,
                10f,
                theme.TextMuted,
                TextAlignmentOptions.Center);
            UIFactory.AllowShrinkToFit(labelText, 8f);
        }

        private void BuildOutbox(RectTransform parent)
        {
            var theme = _shell.Theme;
            var group = UIFactory.CreateCard(
                "Outbox",
                parent,
                theme.SurfaceCard,
                theme,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                (int)theme.CornerRadius);

            UIFactory.CreateText(
                "Title",
                group,
                "Antrean sinkron",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            var pending = _shell.Outbox.PendingCount;
            UIFactory.CreateText(
                "Status",
                group,
                pending == 0
                    ? "Tidak ada hasil menunggu sinkron."
                    : $"{pending} hasil menunggu sinkron. Belum dikirim ke server.",
                theme.BodyFont,
                theme.BodySmallSize,
                pending == 0 ? theme.TextMuted : theme.Warning);

            UIFactory.CreateText(
                "Explanation",
                group,
                "Hasil pelajaran disimpan dulu di perangkat. Sinkronisasi hanya " +
                "berjalan saat kamu menekan tombol, tidak otomatis.",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted);
        }

        /// <summary>
        /// Kartu status mode online lokal. Tidak mengklaim akun server atau
        /// koneksi produksi.
        /// </summary>
        private void BuildOnline(RectTransform parent)
        {
            var theme = _shell.Theme;
            var group = UIFactory.CreateCard(
                "OnlineStatus",
                parent,
                theme.SurfaceCard,
                theme,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                (int)theme.CornerRadius);

            UIFactory.CreateText(
                "Title",
                group,
                "Mode online lokal",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            if (!_shell.OnlineConfigured)
            {
                UIFactory.CreateText(
                    "OnlineState",
                    group,
                    "Nonaktif — data hanya tersimpan di perangkat ini.",
                    theme.BodyFont,
                    theme.BodySmallSize,
                    theme.TextMuted);

                UIFactory.CreateText(
                    "OnlineExplanation",
                    group,
                    "Untuk pengujian, aktifkan lewat online.json di folder data aplikasi. " +
                    "Ini bukan akun server dan bukan backend produksi.",
                    theme.BodyFont,
                    theme.LabelSize,
                    theme.TextMuted);
                return;
            }

            UIFactory.CreateText(
                "OnlineState",
                group,
                "Aktif · " + _shell.Api.BaseUrl,
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextPrimary);

            UIFactory.CreateText(
                "OnlineIdentity",
                group,
                "Identitas pengembangan: " + _shell.Api.DevUser,
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted);

            var pending = _shell.Outbox.PendingCount;
            UIFactory.CreateText(
                "OnlinePending",
                group,
                pending == 0
                    ? "Tidak ada hasil menunggu sinkron."
                    : $"{pending} hasil menunggu sinkron.",
                theme.BodyFont,
                theme.BodySmallSize,
                pending == 0 ? theme.TextMuted : theme.Warning);

            UIFactory.CreateButton(
                "SyncNow",
                group,
                "Sinkronkan sekarang",
                theme,
                _shell.RequestSync,
                primary: true);

            UIFactory.CreateText(
                "OnlineSyncMessage",
                group,
                string.IsNullOrEmpty(_shell.LastSyncMessage)
                    ? "Belum ada sinkronisasi dijalankan."
                    : _shell.LastSyncMessage,
                theme.BodyFont,
                theme.LabelSize,
                _shell.LastSyncFailed ? theme.DangerText : theme.TextMuted);
        }

        private void BuildAbout(RectTransform parent)
        {
            var theme = _shell.Theme;
            var group = UIFactory.CreateCard(
                "About",
                parent,
                theme.SurfaceCard,
                theme,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                (int)theme.CornerRadius);

            UIFactory.CreateText(
                "Title",
                group,
                "Tentang",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Variant",
                group,
                "Bahasa Dawan · varian TTU/Kefamenanu",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            UIFactory.CreateText(
                "Version",
                group,
                $"Versi aplikasi {Application.version} · F1",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            UIFactory.CreateText(
                "Content",
                group,
                "Hanya pelajaran yang lolos validasi yang ditampilkan. " +
                "Audio belum tersedia sampai ada rekaman dengan izin penggunaan.",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted);
        }
    }
}
