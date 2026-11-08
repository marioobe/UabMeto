using UabMeto.App.Content;
using UabMeto.Core.Content;
using UnityEngine;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Daftar pelajaran untuk satu topik. Hanya pelajaran yang lolos gate
    /// (validated) yang boleh memberi progres; pratinjau tetap berlabel dan
    /// tidak mencatat XP.
    /// </summary>
    public sealed class LessonListView
    {
        private readonly ShellView _shell;
        private readonly TopicEntry _topic;

        public LessonListView(ShellView shell, TopicEntry topic)
        {
            _shell = shell;
            _topic = topic;
        }

        public void Build(RectTransform parent)
        {
            var theme = _shell.Theme;
            var scroll = UIFactory.CreateScrollView("LessonListScroll", parent, theme);
            UIFactory.Stretch(scroll.GetComponent<RectTransform>());
            var content = scroll.content;

            var validated = _shell.Catalog.ValidatedLessonsForTopic(_topic.id);
            var preview = _shell.Catalog.PreviewLessonsForTopic(_topic.id);

            BuildHeader(content, validated.Length, preview.Length);

            foreach (var lesson in validated)
            {
                BuildLessonCard(content, lesson, false);
            }

            foreach (var lesson in preview)
            {
                BuildLessonCard(content, lesson, true);
            }

            if (validated.Length == 0 && preview.Length == 0)
            {
                BuildEmptyState(content);
            }
        }

        private void BuildHeader(RectTransform parent, int validatedCount, int previewCount)
        {
            var theme = _shell.Theme;
            var header = UIFactory.CreateVerticalGroup("LessonListHeader", parent, 2f, null, false);

            UIFactory.CreateText(
                "Title",
                header,
                _topic.title,
                theme.HeadingBoldFont,
                theme.Heading1Size,
                theme.TextPrimary);

            string subtitle;
            if (validatedCount == 0 && previewCount == 0)
            {
                subtitle = "Belum ada pelajaran yang lolos validasi.";
            }
            else if (previewCount > 0)
            {
                subtitle = $"{validatedCount} pelajaran tervalidasi · {previewCount} pratinjau";
            }
            else
            {
                subtitle = $"{validatedCount} pelajaran tervalidasi";
            }

            UIFactory.CreateText(
                "Subtitle",
                header,
                subtitle,
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);
        }

        private void BuildLessonCard(RectTransform parent, LessonContent lesson, bool preview)
        {
            var theme = _shell.Theme;
            var completed = !preview && _shell.Progress.IsCompleted(lesson.id);

            var body = UIFactory.CreateCard(
                "Lesson_" + lesson.id,
                parent,
                theme.SurfaceCard,
                theme,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                (int)theme.CornerRadius);

            UIFactory.CreateText(
                "Status",
                body,
                preview
                    ? "PRATINJAU — BELUM DIVALIDASI"
                    : completed
                        ? "SELESAI"
                        : "TERVALIDASI",
                theme.BodyBoldFont,
                10f,
                preview ? theme.Warning : completed ? theme.Success : theme.Accent);

            UIFactory.CreateText(
                "Title",
                body,
                lesson.title,
                theme.HeadingBoldFont,
                theme.Heading2Size,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Meta",
                body,
                $"Pelajaran {lesson.order} · ±{lesson.estimatedMinutes} menit · {lesson.xp} XP",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            if (!string.IsNullOrWhiteSpace(lesson.summary))
            {
                UIFactory.CreateText(
                    "Summary",
                    body,
                    lesson.summary,
                    theme.BodyFont,
                    theme.BodySmallSize,
                    theme.TextPrimary);
            }

            if (preview)
            {
                UIFactory.CreateText(
                    "PreviewWarning",
                    body,
                    "Materi ini masih berstatus in_review dan belum diperiksa reviewer independen. " +
                    "Hasil pratinjau tidak masuk progres atau XP.",
                    theme.BodyFont,
                    theme.LabelSize,
                    theme.Warning);
            }
            else if (completed)
            {
                UIFactory.CreateText(
                    "CompletedNote",
                    body,
                    "Sudah diselesaikan. Bisa diulang untuk latihan tanpa menambah XP.",
                    theme.BodyFont,
                    theme.LabelSize,
                    theme.TextMuted);
            }

            UIFactory.CreateButton(
                "Start_" + lesson.id,
                body,
                preview ? "Buka pratinjau" : completed ? "Ulangi latihan" : "Mulai",
                theme,
                () =>
                {
                    if (preview)
                    {
                        _shell.ShowModal(
                            "Pratinjau materi",
                            "Materi ini masih berstatus in_review dan belum divalidasi reviewer " +
                            "independen. Pratinjau tidak mencatat progres atau XP.",
                            "Buka pratinjau",
                            () => _shell.ShowPreviewLesson(lesson, ReturnToList),
                            "Batal",
                            null);
                        return;
                    }

                    _shell.ShowLesson(lesson, false, ReturnToList);
                });
        }

        private void BuildEmptyState(RectTransform parent)
        {
            var theme = _shell.Theme;
            var body = UIFactory.CreateCard(
                "LessonListEmpty",
                parent,
                theme.SurfaceCard,
                theme,
                theme.Space8,
                new RectOffset(20, 20, 20, 20),
                (int)theme.CornerRadius);

            UIFactory.CreateText(
                "Title",
                body,
                "Belum ada pelajaran",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Body",
                body,
                "Topik ini belum punya pelajaran yang lolos validasi. " +
                "Tidak ada materi contoh yang ditampilkan sebagai pengganti.",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);
        }

        private void ReturnToList()
        {
            _shell.ShowLessonList(_topic);
        }
    }
}
