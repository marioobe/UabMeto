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

            UIFactory.CreateText(
                "ContentNote",
                content,
                "Materi tervalidasi muncul setelah lolos pemeriksaan penutur dan sumber. " +
                "Materi berstatus in_review hanya tampil sebagai pratinjau berlabel dan tidak " +
                "dihitung sebagai progres. Tidak ada teks Dawan yang dikarang untuk mengisi layar ini.",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted);
        }

        private void BuildGreeting(RectTransform parent)
        {
            var theme = _shell.Theme;
            var group = UIFactory.CreateVerticalGroup("Greeting", parent, 2f, null, false);

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
            UIFactory.AddLayoutElement(icon.gameObject, minHeight: 24f, preferredHeight: 24f);

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

            if (catalog.HasPreviewLessons)
            {
                BuildLessonCard(parent, catalog.PreviewLessons[0], false, true);
                return;
            }

            if (catalog.DevelopmentFixture != null)
            {
                BuildFixtureCard(parent, catalog.DevelopmentFixture);
            }

            BuildEmptyState(parent);
        }

        private void BuildLessonCard(
            RectTransform parent,
            LessonContent lesson,
            bool fixture,
            bool preview = false)
        {
            var theme = _shell.Theme;
            var group = UIFactory.CreateCard(
                preview ? "PreviewCard" : "LessonCard",
                parent,
                theme.SurfaceCard,
                theme,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                (int)theme.CornerRadius);

            var badgeText = fixture
                ? "FIXTUR PENGEMBANGAN — BUKAN MATERI DAWAN"
                : preview
                    ? "PRATINJAU — BELUM DIVALIDASI"
                    : "PELAJARAN TERVALIDASI";
            var badgeColor = fixture || preview ? theme.Warning : theme.Success;
            UIFactory.CreateText(
                "Badge",
                group,
                badgeText,
                theme.BodyBoldFont,
                10f,
                badgeColor);

            UIFactory.CreateText(
                "Title",
                group,
                lesson.title,
                theme.HeadingBoldFont,
                theme.Heading2Size,
                theme.TextPrimary);

            var topicTitle = TopicTitle(lesson.topicId);
            UIFactory.CreateText(
                "Meta",
                group,
                $"{topicTitle} · ±{lesson.estimatedMinutes} menit · {lesson.xp} XP",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            if (fixture || preview)
            {
                UIFactory.CreateText(
                    "FixtureWarning",
                    group,
                    fixture
                        ? "Teks di dalamnya masih placeholder dan tidak boleh dipakai sebagai materi nyata."
                        : "Materi ini masih berstatus in_review. Belum diperiksa reviewer independen; " +
                          "hasil pratinjau tidak masuk progres atau XP.",
                    theme.BodyFont,
                    theme.LabelSize,
                    theme.Warning);
            }

            UIFactory.CreateButton(
                "Start",
                group,
                fixture ? "Buka fixture" : preview ? "Buka pratinjau" : "Mulai",
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
                    else if (preview)
                    {
                        _shell.ShowModal(
                            "Pratinjau materi",
                            "Materi ini masih berstatus in_review dan belum divalidasi reviewer " +
                            "independen. Pratinjau tidak mencatat progres atau XP.",
                            "Buka pratinjau",
                            () => _shell.ShowPreviewLesson(lesson),
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
            var group = UIFactory.CreateCard(
                "EmptyState",
                parent,
                theme.SurfaceCard,
                theme,
                theme.Space8,
                new RectOffset(20, 20, 20, 20),
                (int)theme.CornerRadius);
            group.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

            var icon = UIFactory.CreateIcon(
                "Icon",
                group,
                "lock",
                theme.IconFont,
                36f,
                theme.Outline);
            UIFactory.AddLayoutElement(
                icon.gameObject,
                minHeight: 44f,
                preferredHeight: 44f,
                minWidth: 44f,
                preferredWidth: 44f);

            UIFactory.CreateText(
                "Title",
                group,
                "Belum ada pelajaran yang bisa dipelajari",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary,
                TextAlignmentOptions.Center);

            UIFactory.CreateText(
                "Body",
                group,
                "Materi Bahasa Dawan sedang dalam proses validasi penutur dan pemeriksaan sumber. " +
                "Belum ada pelajaran yang boleh diterbitkan.",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted,
                TextAlignmentOptions.Center);
        }

        private void BuildTopics(RectTransform parent)
        {
            var theme = _shell.Theme;
            var header = UIFactory.CreateVerticalGroup("TopicsHeader", parent, 2f, null, false);

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
            var previewLesson = _shell.Catalog.FindPreviewLessonForTopic(topic.id);
            var available = lesson != null;
            var preview = !available && previewLesson != null;

            var panel = UIFactory.CreatePanel(
                "Topic_" + topic.id,
                parent,
                theme.SurfaceCard,
                (int)theme.Space12,
                true);
            var panelLayout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            panelLayout.spacing = 0f;
            panelLayout.padding = new RectOffset();
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = true;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;
            panelLayout.childAlignment = TextAnchor.UpperLeft;

            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 64f;

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

                if (preview)
                {
                    _shell.ShowModal(
                        "Pratinjau " + topic.title,
                        "Topik ini punya pelajaran berstatus in_review. Materi belum divalidasi " +
                        "reviewer independen dan tidak mencatat progres atau XP.",
                        "Buka pratinjau",
                        () => _shell.ShowPreviewLesson(previewLesson),
                        "Batal",
                        null);
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
            row.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

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
                available ? theme.Accent : preview ? theme.Warning : theme.Outline);
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
                available ? "Tersedia" : preview ? "Pratinjau — belum divalidasi" : "Segera",
                theme.BodyFont,
                theme.LabelSize,
                available ? theme.Success : preview ? theme.Warning : theme.TextMuted);

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
                $"{topic.title}, {(available ? "tersedia" : preview ? "pratinjau belum divalidasi" : "belum tersedia")}");
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
