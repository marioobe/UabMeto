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
            BuildAbout(content);
        }

        private void BuildIdentity(RectTransform parent)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("Identity", parent, theme.SurfaceCard, (int)theme.CornerRadius);
            var layout = card.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 170f;
            layout.preferredHeight = 170f;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                theme.Space8,
                new RectOffset(20, 20, 20, 20),
                false);
            UIFactory.Stretch(group);
            group.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

            var avatar = UIFactory.CreatePanel("Avatar", group, theme.SurfaceHigh, 32);
            var avatarLayout = avatar.gameObject.AddComponent<LayoutElement>();
            avatarLayout.minWidth = 64f;
            avatarLayout.preferredWidth = 64f;
            avatarLayout.minHeight = 64f;
            avatarLayout.preferredHeight = 64f;

            var icon = UIFactory.CreateIcon(
                "Icon",
                avatar.transform,
                "person",
                theme.IconFont,
                30f,
                theme.Accent);
            UIFactory.Stretch(icon.rectTransform);

            var nickname = _shell.Profile.IsValid ? _shell.Profile.Nickname : "Pelajar";
            UIFactory.CreateText(
                "Nickname",
                group,
                nickname,
                theme.HeadingBoldFont,
                theme.Heading2Size,
                theme.TextPrimary,
                TextAlignmentOptions.Center);

            UIFactory.CreateText(
                "Note",
                group,
                "Nama panggilan disimpan di perangkat. Belum ada akun server.",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted,
                TextAlignmentOptions.Center);

            UIFactory.CreateButton(
                "Edit",
                group,
                "Ubah nama",
                theme,
                _shell.ShowNicknameDialog,
                primary: false);
        }

        private void BuildStats(RectTransform parent)
        {
            var theme = _shell.Theme;
            var row = UIFactory.CreateHorizontalGroup("ProfileStats", parent, theme.Space8);
            UIFactory.AddLayoutElement(row.gameObject, preferredHeight: 84f);

            CreateStat(row, "Pelajaran selesai", _shell.Progress.Data.completedLessonIds.Count.ToString());
            CreateStat(row, "XP tertunda", _shell.Progress.PendingXp.ToString());
            CreateStat(row, "Antrean", _shell.Outbox.Count.ToString());
        }

        private void CreateStat(Transform parent, string label, string value)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("Stat_" + label, parent, theme.SurfaceCard, (int)theme.Space12);
            var layout = card.gameObject.AddComponent<LayoutElement>();
            layout.flexibleWidth = 1f;
            layout.minHeight = 84f;
            layout.preferredHeight = 84f;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                theme.Space4,
                new RectOffset(8, 8, 8, 8),
                false);
            UIFactory.Stretch(group);
            group.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

            UIFactory.CreateText(
                "Value",
                group,
                value,
                theme.HeadingBoldFont,
                theme.Heading2Size,
                theme.TextPrimary,
                TextAlignmentOptions.Center);

            UIFactory.CreateText(
                "Label",
                group,
                label,
                theme.BodyFont,
                10f,
                theme.TextMuted,
                TextAlignmentOptions.Center);
        }

        private void BuildOutbox(RectTransform parent)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("Outbox", parent, theme.SurfaceCard, (int)theme.CornerRadius);
            var layout = card.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 150f;
            layout.preferredHeight = 150f;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                false);
            UIFactory.Stretch(group);

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
                "Hasil pelajaran disimpan dulu di perangkat. Sinkronisasi server belum aktif pada F1.",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted);
        }

        private void BuildAbout(RectTransform parent)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("About", parent, theme.SurfaceCard, (int)theme.CornerRadius);
            var layout = card.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 190f;
            layout.preferredHeight = 190f;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                false);
            UIFactory.Stretch(group);

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
