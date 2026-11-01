using System;
using System.Collections.Generic;
using TMPro;
using UabMeto.App.Content;
using UabMeto.Core.Content;
using UnityEngine;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Alur pelajaran F1: pembuka → materi → latihan → umpan balik → ringkasan.
    /// F1 tidak memakai timer atau penalti. Pengurangan hati dari PRD §3.8
    /// belum diterapkan di F1 dan tidak boleh diklaim sudah aktif.
    /// XP di ringkasan adalah XP tertunda, bukan hadiah final server.
    /// </summary>
    public sealed class LessonView
    {
        private enum Step
        {
            Intro,
            Material,
            Exercise,
            Feedback,
            Summary
        }

        private readonly ShellView _shell;
        private readonly LessonContent _lesson;
        private readonly bool _fixture;
        private readonly Action _onExit;
        private readonly Action<LessonContent> _onCompleted;

        private RectTransform _host;
        private Step _step = Step.Intro;
        private Exercise _mcq;
        private string _selectedOptionId;
        private bool _answeredCorrectly;
        private Button _checkButton;
        private readonly List<OptionButton> _optionButtons = new List<OptionButton>();

        public LessonView(
            ShellView shell,
            LessonContent lesson,
            bool fixture,
            Action onExit,
            Action<LessonContent> onCompleted)
        {
            _shell = shell;
            _lesson = lesson;
            _fixture = fixture;
            _onExit = onExit;
            _onCompleted = onCompleted;
        }

        public void Build(RectTransform parent)
        {
            _host = parent;
            _mcq = FindMultipleChoice();
            Render();
        }

        public void Back()
        {
            switch (_step)
            {
                case Step.Intro:
                    _onExit?.Invoke();
                    break;
                case Step.Material:
                    _step = Step.Intro;
                    Render();
                    break;
                case Step.Exercise:
                    _step = Step.Material;
                    Render();
                    break;
                case Step.Feedback:
                    _step = Step.Exercise;
                    Render();
                    break;
                case Step.Summary:
                    _shell.ShowModal(
                        "Keluar dari pelajaran?",
                        "Hasil belum disimpan ke antrean. Keluar sekarang?",
                        "Keluar",
                        _onExit,
                        "Tetap di sini",
                        null,
                        true);
                    break;
            }
        }

        private void Render()
        {
            UIFactory.DestroyChildren(_host);
            var theme = _shell.Theme;
            var scroll = UIFactory.CreateScrollView("LessonScroll", _host, theme);
            UIFactory.Stretch(scroll.GetComponent<RectTransform>());
            var content = scroll.content;

            BuildBadge(content);
            BuildStepIndicator(content);

            switch (_step)
            {
                case Step.Intro:
                    BuildIntro(content);
                    break;
                case Step.Material:
                    BuildMaterial(content);
                    break;
                case Step.Exercise:
                    BuildExercise(content);
                    break;
                case Step.Feedback:
                    BuildFeedback(content);
                    break;
                case Step.Summary:
                    BuildSummary(content);
                    break;
            }

            _shell.RebuildLayout();
        }

        private void BuildBadge(RectTransform parent)
        {
            var theme = _shell.Theme;
            var text = _fixture
                ? "FIXTUR PENGEMBANGAN — BUKAN MATERI DAWAN"
                : "PELAJARAN TERVALIDASI";
            var color = _fixture ? theme.Warning : theme.Success;
            var badge = UIFactory.CreateText(
                "Badge",
                parent,
                text,
                theme.BodyBoldFont,
                10f,
                color);
            UIFactory.AddLayoutElement(badge.gameObject, preferredHeight: 16f);
        }

        private void BuildStepIndicator(RectTransform parent)
        {
            var theme = _shell.Theme;
            var labels = new[] { "Pembuka", "Materi", "Latihan", "Umpan balik", "Ringkasan" };
            var index = (int)_step;
            var label = UIFactory.CreateText(
                "Step",
                parent,
                $"Langkah {index + 1} dari {labels.Length} · {labels[index]}",
                theme.BodyFont,
                11f,
                theme.Accent,
                TextAlignmentOptions.MidlineLeft);
            UIFactory.AddLayoutElement(label.gameObject, preferredHeight: 20f);
        }

        private void BuildIntro(RectTransform parent)
        {
            var theme = _shell.Theme;
            var title = UIFactory.CreateText(
                "Title",
                parent,
                _lesson.title,
                theme.HeadingBoldFont,
                theme.Heading1Size,
                theme.TextPrimary);
            UIFactory.AddLayoutElement(title.gameObject, preferredHeight: 34f);

            var summary = UIFactory.CreateText(
                "Summary",
                parent,
                string.IsNullOrWhiteSpace(_lesson.summary)
                    ? "Ringkasan pelajaran belum tersedia."
                    : _lesson.summary,
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);
            UIFactory.AddLayoutElement(summary.gameObject, preferredHeight: 54f);

            var meta = UIFactory.CreateVerticalGroup("Meta", parent, 4f, null, false);
            UIFactory.AddLayoutElement(meta.gameObject, preferredHeight: 92f);
            CreateChip(meta, "schedule", $"±{_lesson.estimatedMinutes} menit");
            CreateChip(meta, "bolt", $"{_lesson.xp} XP (menunggu server)");
            CreateChip(meta, "volume_up", "Audio: belum tersedia");

            var objectivesTitle = UIFactory.CreateText(
                "ObjectivesTitle",
                parent,
                "Tujuan pelajaran",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);
            UIFactory.AddLayoutElement(objectivesTitle.gameObject, preferredHeight: 24f);

            if (_lesson.objectives != null)
            {
                foreach (var objective in _lesson.objectives)
                {
                    var row = UIFactory.CreateHorizontalGroup("Objective", parent, theme.Space8);
                    var icon = UIFactory.CreateIcon(
                        "Icon",
                        row,
                        "check_circle",
                        theme.IconFont,
                        18f,
                        theme.Success);
                    UIFactory.AddLayoutElement(icon.gameObject, minWidth: 24f, preferredWidth: 24f);
                    var text = UIFactory.CreateText(
                        "Text",
                        row,
                        objective,
                        theme.BodyFont,
                        theme.BodySmallSize,
                        theme.TextPrimary);
                    UIFactory.AddLayoutElement(
                        text.gameObject,
                        preferredHeight: 40f,
                        preferredWidth: 0f,
                        flexibleWidth: 1f);
                }
            }

            UIFactory.CreateSpacer("Spacer", parent, theme.Space8);

            UIFactory.CreateButton(
                "StartMaterial",
                parent,
                "Mulai materi",
                theme,
                () =>
                {
                    _step = Step.Material;
                    Render();
                });

            UIFactory.CreateButton(
                "Exit",
                parent,
                "Kembali",
                theme,
                () => _onExit?.Invoke(),
                primary: false,
                ghost: true);
        }

        private void CreateChip(Transform parent, string iconName, string label)
        {
            var theme = _shell.Theme;
            var panel = UIFactory.CreatePanel("Chip", parent, theme.SurfaceHigh, 14);
            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 28f;
            layout.preferredHeight = 28f;

            var row = UIFactory.CreateHorizontalGroup(
                "Row",
                panel.transform,
                4f,
                new RectOffset(10, 10, 4, 4));
            UIFactory.Stretch(row);

            var icon = UIFactory.CreateIcon(
                "Icon",
                row,
                iconName,
                theme.IconFont,
                14f,
                theme.TextMuted);
            UIFactory.AddLayoutElement(icon.gameObject, minWidth: 16f, preferredWidth: 16f);

            var text = UIFactory.CreateText(
                "Label",
                row,
                label,
                theme.BodyFont,
                10f,
                theme.TextMuted,
                TextAlignmentOptions.MidlineLeft);
            UIFactory.AddLayoutElement(
                text.gameObject,
                preferredWidth: 0f,
                flexibleWidth: 1f);
        }

        private void BuildMaterial(RectTransform parent)
        {
            var theme = _shell.Theme;

            if (_lesson.vocabulary != null && _lesson.vocabulary.Length > 0)
            {
                SectionTitle(parent, "Kosakata");
                foreach (var item in _lesson.vocabulary)
                {
                    BuildVocabularyCard(parent, item);
                }
            }

            if (_lesson.examples != null && _lesson.examples.Length > 0)
            {
                SectionTitle(parent, "Contoh kalimat");
                foreach (var example in _lesson.examples)
                {
                    BuildExampleCard(parent, example);
                }
            }

            if (_lesson.dialogues != null && _lesson.dialogues.Length > 0)
            {
                SectionTitle(parent, "Dialog");
                foreach (var dialogue in _lesson.dialogues)
                {
                    BuildDialogueCard(parent, dialogue);
                }
            }

            UIFactory.CreateSpacer("Spacer", parent, theme.Space8);

            if (_mcq != null)
            {
                UIFactory.CreateButton(
                    "ToExercise",
                    parent,
                    "Lanjut ke latihan",
                    theme,
                    () =>
                    {
                        _step = Step.Exercise;
                        Render();
                    });
            }
            else
            {
                var unsupported = UIFactory.CreateText(
                    "Unsupported",
                    parent,
                    "Jenis latihan pada pelajaran ini belum didukung di F1. " +
                    "Tidak ada penilaian yang dibuat-buat.",
                    theme.BodyFont,
                    theme.BodySmallSize,
                    theme.Warning);
                UIFactory.AddLayoutElement(unsupported.gameObject, preferredHeight: 54f);
            }

            UIFactory.CreateButton(
                "Back",
                parent,
                "Kembali ke pembuka",
                theme,
                () =>
                {
                    _step = Step.Intro;
                    Render();
                },
                primary: false,
                ghost: true);
        }

        private void SectionTitle(Transform parent, string title)
        {
            var theme = _shell.Theme;
            var text = UIFactory.CreateText(
                "Section_" + title,
                parent,
                title,
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);
            UIFactory.AddLayoutElement(text.gameObject, preferredHeight: 24f);
        }

        private void BuildVocabularyCard(Transform parent, VocabularyItem item)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("Vocab_" + item.id, parent, theme.SurfaceCard, (int)theme.Space12);
            var layout = card.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 92f;
            layout.preferredHeight = 92f;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                theme.Space4,
                new RectOffset(14, 14, 12, 12),
                false);
            UIFactory.Stretch(group);

            UIFactory.CreateText(
                "Dawan",
                group,
                item.dawan,
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Indonesian",
                group,
                item.indonesian,
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            if (ContentRepository.HasAudioFile(item.audio))
            {
                UIFactory.CreateText(
                    "Audio",
                    group,
                    "Audio tersedia — pemutaran belum diuji di F1.",
                    theme.BodyFont,
                    theme.LabelSize,
                    theme.Warning);
            }
            else
            {
                var audioRow = UIFactory.CreateHorizontalGroup("NoAudio", group, 4f);
                var icon = UIFactory.CreateIcon(
                    "Icon",
                    audioRow,
                    "volume_up",
                    theme.IconFont,
                    14f,
                    theme.Outline);
                UIFactory.AddLayoutElement(icon.gameObject, minWidth: 18f, preferredWidth: 18f);
                UIFactory.CreateText(
                    "Text",
                    audioRow,
                    "Audio Belum Tersedia",
                    theme.BodyFont,
                    theme.LabelSize,
                    theme.Outline);
            }
        }

        private void BuildExampleCard(Transform parent, ExampleItem example)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("Example_" + example.id, parent, theme.SurfaceCard, (int)theme.Space12);
            var layout = card.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 124f;
            layout.preferredHeight = 124f;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                theme.Space4,
                new RectOffset(14, 14, 12, 12),
                false);
            UIFactory.Stretch(group);

            UIFactory.CreateText(
                "Dawan",
                group,
                example.dawan,
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Indonesian",
                group,
                example.indonesian,
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            var context = UIFactory.CreateText(
                "Context",
                group,
                string.IsNullOrWhiteSpace(example.context) ? "Konteks belum tersedia." : example.context,
                theme.BodyFont,
                theme.LabelSize,
                theme.Outline);
            UIFactory.AddLayoutElement(context.gameObject, preferredHeight: 18f);

            var status = example.status == "validated" || example.status == "published"
                ? "Tervalidasi"
                : "Belum tervalidasi";
            var statusColor = example.status == "validated" || example.status == "published"
                ? theme.Success
                : theme.Warning;
            UIFactory.CreateText(
                "Status",
                group,
                string.IsNullOrWhiteSpace(example.sourceRef)
                    ? status
                    : $"{status} · {example.sourceRef}",
                theme.BodyFont,
                10f,
                statusColor);
        }

        private void BuildDialogueCard(Transform parent, Dialogue dialogue)
        {
            var theme = _shell.Theme;
            var card = UIFactory.CreatePanel("Dialogue_" + dialogue.id, parent, theme.SurfaceCard, (int)theme.Space12);
            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                theme.Space8,
                new RectOffset(14, 14, 12, 12),
                true);
            UIFactory.Stretch(group);

            UIFactory.CreateText(
                "Title",
                group,
                string.IsNullOrWhiteSpace(dialogue.title) ? "Dialog" : dialogue.title,
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            foreach (var line in dialogue.lines ?? Array.Empty<DialogueLine>())
            {
                UIFactory.CreateText(
                    "Speaker",
                    group,
                    line.speaker,
                    theme.BodyBoldFont,
                    theme.LabelSize,
                    theme.Accent);
                UIFactory.CreateText(
                    "Dawan",
                    group,
                    line.dawan,
                    theme.BodyFont,
                    theme.BodySize,
                    theme.TextPrimary);
                UIFactory.CreateText(
                    "Indonesian",
                    group,
                    line.indonesian,
                    theme.BodyFont,
                    theme.BodySmallSize,
                    theme.TextMuted);
            }
        }

        private void BuildExercise(RectTransform parent)
        {
            var theme = _shell.Theme;
            _optionButtons.Clear();
            _checkButton = null;

            if (_mcq == null || _mcq.content?.options == null)
            {
                UIFactory.CreateText(
                    "NoExercise",
                    parent,
                    "Latihan belum tersedia untuk pelajaran ini.",
                    theme.BodyFont,
                    theme.BodySize,
                    theme.Warning);
                UIFactory.CreateButton("Back", parent, "Kembali", theme, Back, primary: false);
                return;
            }

            var prompt = UIFactory.CreateText(
                "Prompt",
                parent,
                _mcq.prompt,
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);
            UIFactory.AddLayoutElement(prompt.gameObject, preferredHeight: 54f);

            foreach (var option in _mcq.content.options)
            {
                BuildOption(parent, option);
            }

            _checkButton = UIFactory.CreateButton(
                "Check",
                parent,
                "Periksa jawaban",
                theme,
                CheckAnswer);
            _checkButton.interactable = !string.IsNullOrWhiteSpace(_selectedOptionId);
        }

        private void BuildOption(RectTransform parent, ExerciseOption option)
        {
            var theme = _shell.Theme;
            var selected = option.id == _selectedOptionId;
            var panel = UIFactory.CreatePanel(
                "Option_" + option.id,
                parent,
                selected ? theme.AccentDeepBg : theme.SurfaceHigh,
                (int)theme.Space12,
                true);
            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 56f;
            layout.preferredHeight = 56f;

            var button = panel.gameObject.AddComponent<Button>();
            button.targetGraphic = panel;
            button.transition = Selectable.Transition.ColorTint;

            var label = UIFactory.CreateText(
                "Label",
                panel.transform,
                option.text,
                theme.BodyFont,
                theme.BodySize,
                selected ? theme.TextPrimary : theme.TextPrimary,
                TextAlignmentOptions.MidlineLeft);
            UIFactory.Stretch(label.rectTransform, 16f, 8f, 16f, 8f);

            button.onClick.AddListener(() =>
            {
                _selectedOptionId = option.id;
                foreach (var entry in _optionButtons)
                {
                    var isSelected = entry.Id == option.id;
                    entry.Background.color = isSelected ? theme.AccentDeepBg : theme.SurfaceHigh;
                    entry.Label.color = theme.TextPrimary;
                }

                if (_checkButton != null)
                {
                    _checkButton.interactable = true;
                }
            });

            UIFactory.AddAccessibilityLabel(button.gameObject, option.text);
            _optionButtons.Add(new OptionButton
            {
                Id = option.id,
                Background = panel,
                Label = label
            });
        }

        private void CheckAnswer()
        {
            _answeredCorrectly = _selectedOptionId == _mcq.content.correctOptionId;
            _step = Step.Feedback;
            Render();
        }

        private void BuildFeedback(RectTransform parent)
        {
            var theme = _shell.Theme;
            var panel = UIFactory.CreatePanel(
                "Feedback",
                parent,
                _answeredCorrectly ? theme.SuccessPanel : theme.DangerPanel,
                (int)theme.CornerRadius);
            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 160f;
            layout.preferredHeight = 160f;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                panel.transform,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                false);
            UIFactory.Stretch(group);

            var row = UIFactory.CreateHorizontalGroup("Status", group, theme.Space8);
            var icon = UIFactory.CreateIcon(
                "Icon",
                row,
                _answeredCorrectly ? "check_circle" : "cancel",
                theme.IconFont,
                28f,
                _answeredCorrectly ? theme.Success : theme.DangerText);
            UIFactory.AddLayoutElement(icon.gameObject, minWidth: 32f, preferredWidth: 32f);

            UIFactory.CreateText(
                "Title",
                row,
                _answeredCorrectly ? "Benar" : "Belum tepat",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                _answeredCorrectly ? theme.Success : theme.DangerText,
                TextAlignmentOptions.MidlineLeft);

            var explanation = UIFactory.CreateText(
                "Explanation",
                group,
                string.IsNullOrWhiteSpace(_mcq.explanation)
                    ? "Penjelasan belum tersedia."
                    : _mcq.explanation,
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextPrimary);
            UIFactory.AddLayoutElement(explanation.gameObject, preferredHeight: 44f);

            if (!_answeredCorrectly)
            {
                var correct = FindOption(_mcq.content.correctOptionId);
                if (correct != null)
                {
                    UIFactory.CreateText(
                        "CorrectAnswer",
                        group,
                        "Jawaban benar: " + correct.text,
                        theme.BodyFont,
                        theme.LabelSize,
                        theme.TextMuted);
                }
            }

            UIFactory.CreateSpacer("Spacer", parent, theme.Space8);

            UIFactory.CreateButton(
                "Continue",
                parent,
                "Lanjut",
                theme,
                () =>
                {
                    _step = Step.Summary;
                    Render();
                });
        }

        private void BuildSummary(RectTransform parent)
        {
            var theme = _shell.Theme;
            var icon = UIFactory.CreateIcon(
                "Icon",
                parent,
                "workspace_premium",
                theme.IconFont,
                48f,
                theme.Accent);
            UIFactory.AddLayoutElement(icon.gameObject, minHeight: 56f, preferredHeight: 56f);

            var title = UIFactory.CreateText(
                "Title",
                parent,
                "Pelajaran selesai",
                theme.HeadingBoldFont,
                theme.Heading1Size,
                theme.TextPrimary,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(title.gameObject, preferredHeight: 34f);

            var accuracy = UIFactory.CreateText(
                "Accuracy",
                parent,
                _answeredCorrectly ? "Latihan dijawab benar." : "Latihan belum dijawab benar.",
                theme.BodyFont,
                theme.BodySmallSize,
                _answeredCorrectly ? theme.Success : theme.Warning,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(accuracy.gameObject, preferredHeight: 22f);

            var xp = UIFactory.CreateText(
                "Xp",
                parent,
                $"{_lesson.xp} XP menunggu verifikasi server.",
                theme.BodyBoldFont,
                theme.BodySize,
                theme.TextPrimary,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(xp.gameObject, preferredHeight: 24f);

            var note = UIFactory.CreateText(
                "Note",
                parent,
                "Hasil akan disimpan di antrean lokal dan belum dianggap sebagai XP final. " +
                "Server menentukan hadiah XP dari versi materi terbit.",
                theme.BodyFont,
                theme.LabelSize,
                theme.TextMuted,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(note.gameObject, preferredHeight: 54f);

            UIFactory.CreateButton(
                "Save",
                parent,
                "Simpan hasil",
                theme,
                () => _onCompleted?.Invoke(_lesson));

            UIFactory.CreateButton(
                "ExitWithoutSave",
                parent,
                "Keluar tanpa menyimpan",
                theme,
                () => _onExit?.Invoke(),
                primary: false,
                ghost: true);
        }

        private Exercise FindMultipleChoice()
        {
            foreach (var exercise in _lesson.exercises ?? Array.Empty<Exercise>())
            {
                if (exercise.type == "multiple_choice" &&
                    exercise.content?.options != null &&
                    exercise.content.options.Length >= 2)
                {
                    return exercise;
                }
            }

            return null;
        }

        private ExerciseOption FindOption(string optionId)
        {
            foreach (var option in _mcq.content.options)
            {
                if (option.id == optionId)
                {
                    return option;
                }
            }

            return null;
        }

        private sealed class OptionButton
        {
            public string Id;
            public Image Background;
            public TextMeshProUGUI Label;
        }
    }
}
