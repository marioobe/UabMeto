using System;
using TMPro;
using UabMeto.App.Content;
using UabMeto.Core.Content;
using UnityEngine;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Layar Beranda: hero lanjutkan-belajar (sapaan + progres), satu baris
    /// statistik, dan 12 topik dengan progres masing-masing.
    /// </summary>
    public sealed class HomeView
    {
        /// <summary>Target jalur belajar PRD §3.2: 12 topik × 3 pelajaran.</summary>
        private const int TargetLessonCount = 36;

        private const int LessonsPerTopic = TargetLessonCount / 12;

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

            BuildHero(content);
            BuildStats(content);
            BuildTopics(content);

            UIFactory.CreateText(
                "ContentNote",
                content,
                "Materi tervalidasi muncul setelah lolos pemeriksaan penutur dan sumber. " +
                "Materi berstatus in_review hanya tampil sebagai pratinjau berlabel dan tidak " +
                "dihitung sebagai progres. Tidak ada teks Dawan yang dikarang untuk mengisi layar ini.",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);
        }

        private void BuildHero(RectTransform parent)
        {
            var catalog = _shell.Catalog;
            if (catalog.HasValidatedLessons)
            {
                BuildLessonHero(parent, FirstIncompleteLesson(catalog.ValidatedLessons), false, false);
                return;
            }

            if (catalog.HasPreviewLessons)
            {
                BuildLessonHero(parent, catalog.PreviewLessons[0], false, true);
                return;
            }

            if (catalog.DevelopmentFixture != null)
            {
                BuildLessonHero(parent, catalog.DevelopmentFixture, true, false);
                BuildCompactEmptyState(parent);
                return;
            }

            BuildEmptyHero(parent);
        }

        /// <summary>
        /// Pelajaran berikutnya yang belum selesai. Bila semua sudah selesai,
        /// kembalikan pelajaran pertama agar kartu tetap punya isi.
        /// </summary>
        private LessonContent FirstIncompleteLesson(LessonContent[] lessons)
        {
            foreach (var lesson in lessons)
            {
                if (!_shell.Progress.IsCompleted(lesson.id))
                {
                    return lesson;
                }
            }

            return lessons[0];
        }

        private void BuildLessonHero(
            RectTransform parent,
            LessonContent lesson,
            bool fixture,
            bool preview)
        {
            var theme = _shell.Theme;
            var group = UIFactory.CreateCard(
                preview ? "PreviewCard" : "LessonCard",
                parent,
                theme.SurfaceCard,
                theme,
                theme.Space12,
                new RectOffset(20, 20, 20, 20),
                (int)theme.CornerRadius);

            BuildGreeting(group);
            UIFactory.CreateMotifDivider("Motif", group, theme);

            var badgeText = fixture
                ? "FIXTUR PENGEMBANGAN — BUKAN MATERI DAWAN"
                : preview
                    ? "PRATINJAU — BELUM DIVALIDASI"
                    : "PELAJARAN TERVALIDASI";
            var badge = UIFactory.CreateText(
                "Badge",
                group,
                badgeText,
                theme.BodyBoldFont,
                theme.LabelSize,
                fixture || preview ? theme.Warning : theme.Success);
            badge.characterSpacing = 4f;

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
                    theme.BodySmallSize,
                    theme.Warning);
            }

            if (!fixture && !preview)
            {
                BuildProgressBlock(group);
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

        private void BuildGreeting(Transform parent)
        {
            var theme = _shell.Theme;
            var row = UIFactory.CreateHorizontalGroup("Greeting", parent, theme.Space12);
            UIFactory.AddLayoutElement(row.gameObject, flexibleWidth: 1f);

            var avatar = UIFactory.CreatePanel(
                "GreetingIcon",
                row,
                new Color(theme.Accent.r, theme.Accent.g, theme.Accent.b, 0.12f),
                22);
            UIFactory.AddLayoutElement(
                avatar.gameObject,
                minWidth: 44f,
                preferredWidth: 44f,
                minHeight: 44f,
                preferredHeight: 44f);

            var icon = UIFactory.CreateIcon(
                "Icon",
                avatar.transform,
                "waving_hand",
                theme.IconFont,
                22f,
                theme.Accent);
            UIFactory.Stretch(icon.rectTransform);

            var titles = UIFactory.CreateVerticalGroup("Titles", row, 0f, null, false);
            UIFactory.AddLayoutElement(titles.gameObject, flexibleWidth: 1f);

            UIFactory.CreateText(
                "Welcome",
                titles,
                "Selamat datang,",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            var nickname = _shell.Profile.IsValid ? _shell.Profile.Nickname : "Pelajar";
            UIFactory.CreateText(
                "Nickname",
                titles,
                nickname,
                theme.HeadingBoldFont,
                theme.Heading1Size,
                theme.TextPrimary);
        }

        private void BuildProgressBlock(Transform parent)
        {
            var theme = _shell.Theme;
            var completed = _shell.Progress.Data.completedLessonIds.Count;
            UIFactory.CreateText(
                "ProgressLabel",
                parent,
                $"{completed} dari {TargetLessonCount} pelajaran",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted);

            UIFactory.CreateProgressBar(
                "ProgressBar",
                parent,
                theme,
                completed / (float)TargetLessonCount,
                6f);
        }

        private void BuildEmptyHero(RectTransform parent)
        {
            var theme = _shell.Theme;
            var group = UIFactory.CreateCard(
                "EmptyState",
                parent,
                theme.SurfaceCard,
                theme,
                theme.Space12,
                new RectOffset(20, 20, 20, 20),
                (int)theme.CornerRadius);

            BuildGreeting(group);
            UIFactory.CreateMotifDivider("Motif", group, theme);

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
                theme.Heading2Size,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Body",
                group,
                "Materi Bahasa Dawan sedang dalam proses validasi penutur dan pemeriksaan sumber. " +
                "Belum ada pelajaran yang boleh diterbitkan.",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);
        }

        /// <summary>
        /// Catatan ringkas saat yang tampil hanya fixture pengembangan. Nama
        /// <c>EmptyState</c> dipertahankan karena membawa arti yang sama:
        /// belum ada pelajaran tervalidasi.
        /// </summary>
        private void BuildCompactEmptyState(RectTransform parent)
        {
            var theme = _shell.Theme;
            var group = UIFactory.CreateCard(
                "EmptyState",
                parent,
                theme.SurfaceLow,
                theme,
                theme.Space4,
                new RectOffset(16, 16, 16, 16),
                (int)theme.Space12);

            UIFactory.CreateText(
                "Title",
                group,
                "Belum ada pelajaran tervalidasi",
                theme.BodyBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Body",
                group,
                "Kartu di atas hanya fixture pengembangan dan tidak dihitung sebagai materi atau progres.",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);
        }

        private void BuildStats(RectTransform parent)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreateCard(
                "Stats",
                parent,
                theme.SurfaceLow,
                theme,
                theme.Space8,
                new RectOffset(12, 12, 12, 12),
                (int)theme.Space12);

            var row = UIFactory.CreateHorizontalGroup("StatRow", card, theme.Space12);
            UIFactory.AddLayoutElement(row.gameObject, flexibleWidth: 1f);

            CreateStat(row, "Streak", "local_fire_department", _shell.Progress.Streak.ToString(), "Streak hari");
            CreateStatDivider(row);
            CreateStat(row, "Xp", "bolt", _shell.Progress.PendingXp.ToString(), "XP tertunda");
            CreateStatDivider(row);
            CreateStat(
                row,
                "Selesai",
                "check_circle",
                _shell.Progress.Data.completedLessonIds.Count.ToString(),
                "Selesai");
        }

        private void CreateStat(
            Transform parent,
            string name,
            string iconName,
            string value,
            string label)
        {
            var theme = _shell.Theme;
            var cell = UIFactory.CreateVerticalGroup("Stat_" + name, parent, 2f, null, false);
            UIFactory.AddLayoutElement(cell.gameObject, flexibleWidth: 1f);
            cell.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

            var head = UIFactory.CreateHorizontalGroup("Head", cell, theme.Space4);
            head.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
            UIFactory.AddLayoutElement(head.gameObject, flexibleWidth: 1f);

            var icon = UIFactory.CreateIcon(
                "Icon",
                head,
                iconName,
                theme.IconFont,
                16f,
                theme.Accent);
            UIFactory.AddLayoutElement(
                icon.gameObject,
                minWidth: 20f,
                preferredWidth: 20f,
                minHeight: 20f,
                preferredHeight: 20f);

            UIFactory.CreateText(
                "Value",
                head,
                value,
                theme.HeadingBoldFont,
                theme.Heading2Size,
                theme.TextPrimary,
                TextAlignmentOptions.MidlineLeft);

            UIFactory.CreateText(
                "Label",
                cell,
                label,
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted,
                TextAlignmentOptions.Center);
        }

        private void CreateStatDivider(Transform parent)
        {
            var theme = _shell.Theme;
            var divider = UIFactory.CreatePanel("Divider", parent, theme.Border, 0);
            divider.raycastTarget = false;
            UIFactory.AddLayoutElement(
                divider.gameObject,
                minWidth: 1f,
                preferredWidth: 1f,
                minHeight: 32f,
                preferredHeight: 32f,
                flexibleHeight: 0f);
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
                theme.Heading2Size,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Subtitle",
                header,
                "Urutan sesuai PRD §3.2 · varian TTU/Kefamenanu",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            foreach (var topic in _shell.Catalog.Topics)
            {
                BuildTopicRow(parent, topic);
            }
        }

        private void BuildTopicRow(RectTransform parent, TopicEntry topic)
        {
            var theme = _shell.Theme;
            var lessons = _shell.Catalog.ValidatedLessonsForTopic(topic.id);
            var previewLessons = _shell.Catalog.PreviewLessonsForTopic(topic.id);
            var lesson = lessons.Length > 0 ? lessons[0] : null;
            var previewLesson = previewLessons.Length > 0 ? previewLessons[0] : null;
            var available = lesson != null;
            var preview = !available && previewLesson != null;
            var totalLessons = lessons.Length + previewLessons.Length;
            var completed = CountCompleted(lessons);

            var panel = UIFactory.CreatePanel(
                "Topic_" + topic.id,
                parent,
                theme.SurfaceCard,
                (int)theme.Space12,
                true);
            UIFactory.AddRoundedBorder(panel.rectTransform, theme, (int)theme.Space12);

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
            panel.gameObject.AddComponent<PressScale>();
            button.onClick.AddListener(() =>
            {
                if (totalLessons > 1)
                {
                    _shell.ShowLessonList(topic);
                    return;
                }

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
                new RectOffset(12, 12, 10, 10));
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

            var titles = UIFactory.CreateVerticalGroup("Titles", row, 2f, null, false);
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
                available
                    ? lessons.Length > 1
                        ? $"{lessons.Length} pelajaran"
                        : "Tersedia"
                    : preview
                        ? "Pratinjau — belum divalidasi"
                        : "Segera",
                theme.BodyFont,
                theme.LabelSize,
                available ? theme.Success : preview ? theme.Warning : theme.TextMuted);

            var progress = UIFactory.CreateHorizontalGroup("Progress", titles, theme.Space8);
            UIFactory.AddLayoutElement(progress.gameObject, flexibleWidth: 1f);

            UIFactory.CreateProgressBar(
                "Bar",
                progress,
                theme,
                completed / (float)LessonsPerTopic,
                4f);

            UIFactory.CreateText(
                "Count",
                progress,
                $"{completed}/{LessonsPerTopic}",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted,
                TextAlignmentOptions.MidlineRight);

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
                $"{topic.title}, {(available ? "tersedia" : preview ? "pratinjau belum divalidasi" : "belum tersedia")}, " +
                $"progres {completed} dari {LessonsPerTopic}");
        }

        private int CountCompleted(LessonContent[] lessons)
        {
            var completed = 0;
            foreach (var lesson in lessons)
            {
                if (_shell.Progress.IsCompleted(lesson.id))
                {
                    completed++;
                }
            }

            return completed;
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
