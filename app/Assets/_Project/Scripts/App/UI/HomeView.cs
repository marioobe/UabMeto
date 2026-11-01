using System;
using TMPro;
using UabMeto.App.Content;
using UabMeto.Core.Content;
using UnityEngine;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>Layar Beranda: sapaan, statistik lokal, pelajaran, dan 12 topik.</summary>
    public sealed class HomeView
    {
        private readonly ShellView _shell;

        public HomeView(ShellView shell)
        {
            _shell = shell;
        }

        public void Build(RectTransform parent)
        {
            var theme = _shell.Theme;
            var scroll = UIFactory.CreateScrollView("HomeScroll", parent, theme);
            UIFactory.Stretch(scroll.GetComponent<RectTransform>());
            var content = scroll.content;

            BuildGreeting(content);
            BuildStats(content);
            BuildLessonSection(content);
            BuildTopics(content);

            var note = UIFactory.CreateText(
                "ContentNote",
                content,
                "Materi Bahasa Dawan hanya muncul setelah lolos validasi penutur, sumber, dan izin audio. " +
                "Tidak ada teks Dawan yang dikarang untuk mengisi layar ini.",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted);
            UIFactory.AddLayoutElement(note.gameObject, preferredHeight: 54f);
        }

        private void BuildGreeting(RectTransform parent)
        {
            var theme = _shell.Theme;
            var group = UIFactory.CreateVerticalGroup("Greeting", parent, 2f, null, false);
            UIFactory.AddLayoutElement(group.gameObject, preferredHeight: 58f);

            UIFactory.CreateText(
                "Welcome",
                group,
                "Selamat datang,",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            var nickname = _shell.Profile.IsValid ? _shell.Profile.Nickname : "Pelajar";
            UIFactory.CreateText(
                "Nickname",
                group,
                nickname,
                theme.HeadingBoldFont,
                theme.Heading1Size,
                theme.TextPrimary);
        }

        private void BuildStats(RectTransform parent)
        {
            var theme = _shell.Theme;
            var row = UIFactory.CreateHorizontalGroup("Stats", parent, theme.Space8);
            UIFactory.AddLayoutElement(row.gameObject, preferredHeight: 84f);

            CreateStatCard(row, "local_fire_department", _shell.Progress.Streak.ToString(), "Streak hari");
            CreateStatCard(row, "bolt", _shell.Progress.PendingXp.ToString(), "XP tertunda");
            CreateStatCard(
                row,
                "check_circle",
                _shell.Progress.Data.completedLessonIds.Count.ToString(),
                "Selesai");
        }

        private void CreateStatCard(Transform parent, string iconName, string value, string label)
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

            var icon = UIFactory.CreateIcon(
                "Icon",
                group,
                iconName,
                theme.IconFont,
                18f,
                theme.Accent);
            UIFactory.AddLayoutElement(icon.gameObject, minHeight: 20f, preferredHeight: 20f);

            var valueText = UIFactory.CreateText(
                "Value",
                group,
                value,
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(valueText.gameObject, minHeight: 20f, preferredHeight: 20f);

            var labelText = UIFactory.CreateText(
                "Label",
                group,
                label,
                theme.BodyFont,
                10f,
                theme.TextMuted,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(labelText.gameObject, minHeight: 14f, preferredHeight: 14f);
        }

        private void BuildLessonSection(RectTransform parent)
        {
            var theme = _shell.Theme;
            UIFactory.CreateText(
                "LessonSection",
                parent,
                "Lanjutkan belajar",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            var catalog = _shell.Catalog;
            if (catalog.HasValidatedLessons)
            {
                var lesson = catalog.ValidatedLessons[0];
                BuildLessonCard(parent, lesson, false);
                return;
            }

            if (catalog.DevelopmentFixture != null)
            {
                BuildFixtureCard(parent, catalog.DevelopmentFixture);
            }

            BuildEmptyState(parent);
        }

        private void BuildLessonCard(RectTransform parent, LessonContent lesson, bool fixture)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("LessonCard", parent, theme.SurfaceCard, (int)theme.CornerRadius);
            var cardLayout = card.gameObject.AddComponent<LayoutElement>();
            cardLayout.minHeight = 190f;
            cardLayout.preferredHeight = 190f;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                false);
            UIFactory.Stretch(group);

            var badgeText = fixture
                ? "FIXTUR PENGEMBANGAN — BUKAN MATERI DAWAN"
                : "PELAJARAN TERVALIDASI";
            var badgeColor = fixture ? theme.Warning : theme.Success;
            var badge = UIFactory.CreateText(
                "Badge",
                group,
                badgeText,
                theme.BodyBoldFont,
                10f,
                badgeColor);
            UIFactory.AddLayoutElement(badge.gameObject, preferredHeight: 16f);

            var title = UIFactory.CreateText(
                "Title",
                group,
                lesson.title,
                theme.HeadingBoldFont,
                theme.Heading2Size,
                theme.TextPrimary);
            UIFactory.AddLayoutElement(title.gameObject, preferredHeight: 30f);

            var topicTitle = TopicTitle(lesson.topicId);
            var meta = UIFactory.CreateText(
                "Meta",
                group,
                $"{topicTitle} · ±{lesson.estimatedMinutes} menit · {lesson.xp} XP",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);
            UIFactory.AddLayoutElement(meta.gameObject, preferredHeight: 22f);

            if (fixture)
            {
                var warning = UIFactory.CreateText(
                    "FixtureWarning",
                    group,
                    "Teks di dalamnya masih placeholder dan tidak boleh dipakai sebagai materi nyata.",
                    theme.BodyFont,
                    theme.LabelSize,
                    theme.Warning);
                UIFactory.AddLayoutElement(warning.gameObject, preferredHeight: 34f);
            }

            UIFactory.CreateButton(
                "Start",
                group,
                fixture ? "Buka fixture" : "Mulai",
                theme,
                () =>
                {
                    if (fixture)
                    {
                        _shell.ShowModal(
                            "Fixture pengembangan",
                            "Ini bukan materi Bahasa Dawan. Semua teks masih placeholder dan " +
                            "hanya dipakai untuk menguji alur UI. Jangan menganggapnya sebagai pelajaran.",
                            "Buka fixture",
                            () => _shell.ShowLesson(lesson, true),
                            "Batal",
                            null);
                    }
                    else
                    {
                        _shell.ShowLesson(lesson, false);
                    }
                });
        }

        private void BuildFixtureCard(RectTransform parent, LessonContent fixture)
        {
            BuildLessonCard(parent, fixture, true);
        }

        private void BuildEmptyState(RectTransform parent)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("EmptyState", parent, theme.SurfaceCard, (int)theme.CornerRadius);
            var layout = card.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 210f;
            layout.preferredHeight = 210f;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                theme.Space8,
                new RectOffset(20, 20, 20, 20),
                false);
            UIFactory.Stretch(group);
            group.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

            var icon = UIFactory.CreateIcon(
                "Icon",
                group,
                "lock",
                theme.IconFont,
                36f,
                theme.Outline);
            UIFactory.AddLayoutElement(icon.gameObject, minHeight: 44f, preferredHeight: 44f);

            var title = UIFactory.CreateText(
                "Title",
                group,
                "Belum ada pelajaran yang bisa dipelajari",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(title.gameObject, preferredHeight: 46f);

            var body = UIFactory.CreateText(
                "Body",
                group,
                "Materi Bahasa Dawan sedang dalam proses validasi penutur dan pemeriksaan sumber. " +
                "Belum ada pelajaran yang boleh diterbitkan.",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(body.gameObject, preferredHeight: 58f);
        }

        private void BuildTopics(RectTransform parent)
        {
            var theme = _shell.Theme;
            var header = UIFactory.CreateVerticalGroup("TopicsHeader", parent, 2f, null, false);
            UIFactory.AddLayoutElement(header.gameObject, preferredHeight: 44f);

            UIFactory.CreateText(
                "Title",
                header,
                "12 topik",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Subtitle",
                header,
                "Urutan sesuai PRD §3.2 · varian TTU/Kefamenanu",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted);

            foreach (var topic in _shell.Catalog.Topics)
            {
                BuildTopicRow(parent, topic);
            }
        }

        private void BuildTopicRow(RectTransform parent, TopicEntry topic)
        {
            var theme = _shell.Theme;
            var lesson = _shell.Catalog.FindLessonForTopic(topic.id);
            var available = lesson != null;

            var panel = UIFactory.CreatePanel(
                "Topic_" + topic.id,
                parent,
                theme.SurfaceCard,
                (int)theme.Space12,
                true);
            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 64f;
            layout.preferredHeight = 64f;

            var button = panel.gameObject.AddComponent<Button>();
            button.targetGraphic = panel;
            button.transition = Selectable.Transition.ColorTint;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            button.colors = colors;
            button.onClick.AddListener(() =>
            {
                if (available)
                {
                    _shell.ShowLesson(lesson, false);
                    return;
                }

                _shell.ShowModal(
                    topic.title,
                    "Topik ini belum punya pelajaran yang lolos validasi. " +
                    "Tidak ada materi contoh yang ditampilkan sebagai pengganti.",
                    "Mengerti",
                    null,
                    dismissible: true);
            });

            var row = UIFactory.CreateHorizontalGroup(
                "Row",
                panel.transform,
                theme.Space12,
                new RectOffset(12, 12, 8, 8));
            UIFactory.Stretch(row);

            var iconPanel = UIFactory.CreatePanel("IconPanel", row, theme.SurfaceHigh, (int)theme.Space12);
            var iconLayout = iconPanel.gameObject.AddComponent<LayoutElement>();
            iconLayout.minWidth = 40f;
            iconLayout.preferredWidth = 40f;
            iconLayout.minHeight = 40f;
            iconLayout.preferredHeight = 40f;

            var icon = UIFactory.CreateIcon(
                "Icon",
                iconPanel.transform,
                topic.icon,
                theme.IconFont,
                22f,
                available ? theme.Accent : theme.Outline);
            UIFactory.Stretch(icon.rectTransform);

            var titles = UIFactory.CreateVerticalGroup("Titles", row, 1f, null, false);
            var titlesLayout = titles.gameObject.AddComponent<LayoutElement>();
            titlesLayout.flexibleWidth = 1f;

            UIFactory.CreateText(
                "Title",
                titles,
                topic.title,
                theme.BodyBoldFont,
                theme.BodySize,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Status",
                titles,
                available ? "Tersedia" : "Segera",
                theme.BodyFont,
                theme.LabelSize,
                available ? theme.Success : theme.TextMuted);

            var chevron = UIFactory.CreateIcon(
                "Chevron",
                row,
                "chevron_right",
                theme.IconFont,
                20f,
                theme.Outline);
            UIFactory.AddLayoutElement(
                chevron.gameObject,
                minWidth: 24f,
                preferredWidth: 24f,
                minHeight: 24f,
                preferredHeight: 24f);

            UIFactory.AddAccessibilityLabel(
                button.gameObject,
                $"{topic.title}, {(available ? "tersedia" : "belum tersedia")}");
        }

        private string TopicTitle(string topicId)
        {
            foreach (var topic in _shell.Catalog.Topics)
            {
                if (topic.id == topicId)
                {
                    return topic.title;
                }
            }

            return "Topik";
        }
    }
}
