using System;
using TMPro;
using UabMeto.App.Net;
using UnityEngine;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Tab Peringkat. Tidak menampilkan nama atau angka rekaan: data hanya
    /// dimuat dari Worker lokal saat pengguna menekan tombol.
    /// </summary>
    public sealed class LeaderboardView
    {
        private readonly ShellView _shell;

        public LeaderboardView(ShellView shell)
        {
            _shell = shell;
        }

        public void Build(RectTransform parent)
        {
            var theme = _shell.Theme;
            var scroll = UIFactory.CreateScrollView("LeaderboardScroll", parent, theme);
            UIFactory.Stretch(scroll.GetComponent<RectTransform>());
            var content = scroll.content;

            var card = UIFactory.CreateCard(
                "LeaderboardCard",
                content,
                theme.SurfaceCard,
                theme,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                (int)theme.CornerRadius);

            UIFactory.CreateText(
                "Title",
                card,
                "Peringkat mingguan",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            var status = UIFactory.CreateText(
                "LeaderboardStatus",
                card,
                string.Empty,
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            var entries = UIFactory.CreateVerticalGroup(
                "LeaderboardEntries",
                card,
                theme.Space4,
                null,
                true);

            if (!_shell.OnlineConfigured)
            {
                status.text =
                    "Mode online lokal nonaktif. Tidak ada data peringkat yang ditampilkan.";
                UIFactory.CreateText(
                    "Explanation",
                    card,
                    "Aktifkan mode online lokal lewat online.json hanya untuk pengujian. " +
                    "Peringkat ini bukan papan peringkat global.",
                    theme.BodyFont,
                    theme.LabelSize,
                    theme.TextMuted);
                return;
            }

            status.text = "Muat data dari server lokal untuk melihat peringkat minggu ini.";
            UIFactory.CreateButton(
                "LoadLeaderboard",
                card,
                "Muat peringkat",
                theme,
                () => LoadAsync(status, entries),
                primary: false);

            UIFactory.CreateText(
                "Explanation",
                card,
                "Peringkat dihitung server dari XP yang sudah dikonfirmasi. " +
                "Data berasal dari Worker lokal, bukan layanan produksi.",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted);
        }

        private async void LoadAsync(TextMeshProUGUI status, RectTransform entries)
        {
            var theme = _shell.Theme;
            status.color = theme.TextMuted;
            status.text = "Memuat peringkat…";
            UIFactory.DestroyChildren(entries);

            try
            {
                var board = await _shell.Api.GetLeaderboardAsync();
                var count = board.entries == null ? 0 : board.entries.Count;
                status.text = $"Minggu {board.weekKey} · {count} peserta";

                if (count == 0)
                {
                    UIFactory.CreateText(
                        "LeaderboardEmpty",
                        entries,
                        "Belum ada XP tercatat minggu ini.",
                        theme.BodyFont,
                        theme.BodySmallSize,
                        theme.TextMuted);
                }
                else
                {
                    for (var index = 0; index < count; index++)
                    {
                        CreateRow(entries, index + 1, board.entries[index]);
                    }
                }

                _shell.RebuildLayout();
            }
            catch (ApiException exception)
            {
                status.color = theme.DangerText;
                status.text = exception.Code == "network_error"
                    ? "Tidak dapat menghubungi server lokal."
                    : "Gagal memuat peringkat: " + exception.Message;
            }
            catch (Exception exception)
            {
                status.color = theme.DangerText;
                status.text = "Gagal memuat peringkat: " + exception.Message;
            }
        }

        private void CreateRow(RectTransform parent, int rank, LeaderboardEntry entry)
        {
            var theme = _shell.Theme;
            var isCurrentUser = _shell.Api != null && entry.userId == _shell.Api.DevUser;
            var nickname = string.IsNullOrWhiteSpace(entry.nickname)
                ? entry.userId
                : entry.nickname;

            var row = UIFactory.CreateHorizontalGroup(
                "Entry_" + rank,
                parent,
                theme.Space8);
            UIFactory.AddLayoutElement(row.gameObject, preferredHeight: 36f);

            var rankText = UIFactory.CreateText(
                "Rank",
                row,
                rank + ".",
                theme.BodyBoldFont,
                theme.BodySmallSize,
                theme.TextMuted);
            UIFactory.AddLayoutElement(
                rankText.gameObject,
                minWidth: 28f,
                preferredWidth: 28f);

            var nameText = UIFactory.CreateText(
                "Name",
                row,
                nickname,
                theme.BodyFont,
                theme.BodySmallSize,
                isCurrentUser ? theme.Accent : theme.TextPrimary);
            UIFactory.AddLayoutElement(nameText.gameObject, flexibleWidth: 1f);

            var xpText = UIFactory.CreateText(
                "Xp",
                row,
                entry.xp + " XP",
                theme.BodyBoldFont,
                theme.BodySmallSize,
                isCurrentUser ? theme.Accent : theme.TextMuted,
                TextAlignmentOptions.MidlineRight);
            UIFactory.AddLayoutElement(
                xpText.gameObject,
                minWidth: 72f,
                preferredWidth: 72f);

            UIFactory.AddAccessibilityLabel(
                row.gameObject,
                $"Peringkat {rank}, {nickname}, {entry.xp} XP");
        }
    }
}
