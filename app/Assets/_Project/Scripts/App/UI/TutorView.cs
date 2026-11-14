using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UabMeto.App.Net;
using UnityEngine;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Tab Tutor AI (teks). Jawaban hanya boleh berasal dari korpus tervalidasi
    /// yang disiapkan Worker; klien tidak pernah mengarang kosakata Dawan atau
    /// memberi skor. Kuota harian ditampilkan dari server.
    /// </summary>
    public sealed class TutorView
    {
        private const int MaximumHistoryMessages = 6;
        private const float ComposerHeight = 80f;

        private static readonly string[] QuickQuestions =
        {
            "Apa arti kata Noka?",
            "Bagaimana menyapa pada pagi hari?",
            "Bagaimana mengatakan terima kasih?",
            "Apa arti selamat tinggal?"
        };

        private readonly ShellView _shell;
        private readonly List<TutorMessage> _history = new List<TutorMessage>();

        private TMP_InputField _input;
        private Button _send;
        private TextMeshProUGUI _status;
        private TextMeshProUGUI _quota;
        private RectTransform _messages;
        private ScrollRect _scroll;
        private int _messageIndex;
        private bool _sending;
        private bool _quotaExhausted;

        public TutorView(ShellView shell)
        {
            _shell = shell ?? throw new ArgumentNullException(nameof(shell));
        }

        public void Build(RectTransform parent)
        {
            var theme = _shell.Theme;
            var composerHeight = UIFactory.Px(ComposerHeight);
            var root = UIFactory.CreateRect("TutorRoot", parent);
            UIFactory.Stretch(root);

            _scroll = UIFactory.CreateScrollView("TutorScroll", root, theme);
            UIFactory.Stretch(
                _scroll.GetComponent<RectTransform>(),
                0f,
                composerHeight);

            var content = _scroll.content;
            var intro = UIFactory.CreateCard(
                "TutorIntro",
                content,
                theme.SurfaceCard,
                theme,
                theme.Space8,
                new RectOffset(16, 16, 16, 16),
                (int)theme.CornerRadius);

            UIFactory.CreateText(
                "Title",
                intro,
                "Tanya materi Topik 1",
                theme.HeadingBoldFont,
                theme.Heading3Size,
                theme.TextPrimary);

            UIFactory.CreateText(
                "Body",
                intro,
                "Tutor AI menjelaskan dalam Bahasa Indonesia hanya dari pelajaran " +
                "yang sudah tervalidasi. Jika materinya tidak ada, Tutor AI akan " +
                "mengatakan belum dapat dipastikan. Tidak ada audio atau penilaian " +
                "pelafalan pada versi ini.",
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            _quota = UIFactory.CreateText(
                "TutorQuota",
                intro,
                "Memeriksa kuota…",
                theme.BodyBoldFont,
                theme.LabelSize,
                theme.TextMuted);

            var chips = UIFactory.CreateVerticalGroup(
                "TutorChips",
                intro,
                theme.Space8,
                null,
                true);
            for (var index = 0; index < QuickQuestions.Length; index++)
            {
                var question = QuickQuestions[index];
                var chip = UIFactory.CreateButton(
                    "TutorChip_" + index,
                    chips,
                    question,
                    theme,
                    () =>
                    {
                        if (_input != null)
                        {
                            _input.text = question;
                            _input.ActivateInputField();
                        }
                    },
                    primary: false);
                UIFactory.AddLayoutElement(chip.gameObject, minHeight: 56f);
            }

            _messages = UIFactory.CreateVerticalGroup(
                "TutorMessages",
                content,
                theme.Space8,
                null,
                true);

            _status = UIFactory.CreateText(
                "TutorStatus",
                content,
                string.Empty,
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextMuted);

            var composer = BuildComposer(root, composerHeight);
            composer.gameObject.AddComponent<KeyboardInset>().Initialize(
                composer,
                _scroll.GetComponent<RectTransform>(),
                _input,
                composerHeight);

            if (!_shell.OnlineConfigured)
            {
                ShowOfflineState();
                return;
            }

            _ = LoadQuotaAsync();
        }

        private RectTransform BuildComposer(RectTransform root, float composerHeight)
        {
            var theme = _shell.Theme;
            var panel = UIFactory.CreatePanel("TutorComposer", root, theme.Surface, 0);
            var rect = panel.rectTransform;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(0f, composerHeight);
            rect.anchoredPosition = Vector2.zero;

            var row = UIFactory.CreateHorizontalGroup(
                "Row",
                panel.transform,
                theme.Space8,
                new RectOffset(16, 16, 12, 12));
            UIFactory.Stretch(row);

            _input = UIFactory.CreateInputField(
                "TutorInput",
                row,
                "Tulis pertanyaan…",
                theme,
                300);
            UIFactory.AddLayoutElement(_input.gameObject, flexibleWidth: 1f);
            _input.onSubmit.AddListener(_ => Send());

            _send = UIFactory.CreateButton(
                "TutorSend",
                row,
                "Kirim",
                theme,
                Send);
            UIFactory.AddLayoutElement(
                _send.gameObject,
                minWidth: 84f,
                preferredWidth: 84f,
                flexibleWidth: 0f);

            return rect;
        }

        private async Task LoadQuotaAsync()
        {
            try
            {
                var quota = await _shell.Api.GetQuotaAsync();
                UpdateQuota(quota.tutor);
            }
            catch (Exception)
            {
                if (_quota != null)
                {
                    _quota.text = "Kuota tidak dapat dimuat dari server lokal.";
                }
            }
        }

        private void Send()
        {
            if (_sending)
            {
                return;
            }

            if (!_shell.OnlineConfigured)
            {
                ShowOfflineState();
                return;
            }

            if (_quotaExhausted)
            {
                SetStatus(
                    "Kuota Tutor AI hari ini habis. Coba lagi besok atau lanjutkan pelajaran.",
                    danger: true);
                return;
            }

            var question = _input == null ? string.Empty : _input.text.Trim();
            if (question.Length < 2)
            {
                SetStatus("Tulis pertanyaan minimal 2 karakter.", danger: false);
                return;
            }

            var history = _history
                .Skip(Math.Max(0, _history.Count - MaximumHistoryMessages))
                .ToArray();

            _sending = true;
            if (_send != null)
            {
                _send.interactable = false;
            }

            _input.text = string.Empty;
            _input.DeactivateInputField();
            SetStatus("Tutor sedang menyusun jawaban…", danger: false);

            AddBubble("user", question, null);
            AddHistory("user", question);
            var pending = AddBubble("tutor", "Menunggu jawaban…", null);

            _ = AskAsync(question, history, pending);
        }

        private async Task AskAsync(
            string question,
            TutorMessage[] history,
            GameObject pending)
        {
            try
            {
                var response = await _shell.Api.AskTutorAsync(question, history);
                RemoveBubble(pending);

                AddBubble("tutor", response.answer, BuildSourceText(response));
                AddHistory("tutor", response.answer);
                UpdateQuota(response.quota);

                SetStatus(
                    response.refused
                        ? "Tidak ada materi tervalidasi yang cocok. Jawaban tidak dikarang."
                        : "Jawaban berasal dari materi tervalidasi.",
                    danger: false);
            }
            catch (ApiException exception)
            {
                RemoveBubble(pending);
                HandleApiError(exception);
            }
            catch (Exception exception)
            {
                RemoveBubble(pending);
                SetStatus("Gagal meminta jawaban: " + exception.Message, danger: true);
            }
            finally
            {
                _sending = false;
                if (_send != null)
                {
                    _send.interactable = !_quotaExhausted;
                }

                _shell.RebuildLayout();
                ScrollToBottom();
            }
        }

        private void HandleApiError(ApiException exception)
        {
            switch (exception.Code)
            {
                case "quota_exceeded":
                    _quotaExhausted = true;
                    if (_send != null)
                    {
                        _send.interactable = false;
                    }

                    if (_quota != null)
                    {
                        _quota.color = _shell.Theme.DangerText;
                        _quota.text = "Sisa 0 dari 10 pertanyaan hari ini";
                    }

                    SetStatus(exception.Message, danger: true);
                    break;
                case "network_error":
                    SetStatus(
                        "Tidak dapat menghubungi server lokal. Tutor AI membutuhkan " +
                        "server lokal yang aktif.",
                        danger: true);
                    break;
                case "provider_error":
                case "provider_empty":
                case "ai_binding_missing":
                    SetStatus(
                        "Tutor AI sedang tidak tersedia. Coba lagi nanti.",
                        danger: true);
                    break;
                default:
                    SetStatus("Gagal meminta jawaban: " + exception.Message, danger: true);
                    break;
            }
        }

        private void UpdateQuota(QuotaCounter quota)
        {
            if (quota == null || quota.limit <= 0)
            {
                return;
            }

            var remaining = Mathf.Max(0, quota.limit - quota.used);
            _quotaExhausted = remaining <= 0;
            if (_quota != null)
            {
                _quota.color = _quotaExhausted
                    ? _shell.Theme.DangerText
                    : _shell.Theme.TextMuted;
                _quota.text = $"Sisa {remaining} dari {quota.limit} pertanyaan hari ini";
            }

            if (_send != null)
            {
                _send.interactable = !_quotaExhausted;
            }

            if (_quotaExhausted)
            {
                SetStatus(
                    "Kuota Tutor AI hari ini habis. Coba lagi besok atau lanjutkan pelajaran.",
                    danger: true);
            }
        }

        private void ShowOfflineState()
        {
            if (_input != null)
            {
                _input.interactable = false;
            }

            if (_send != null)
            {
                _send.interactable = false;
            }

            if (_quota != null)
            {
                _quota.text = "Mode online lokal nonaktif";
            }

            SetStatus(
                "Tutor AI membutuhkan server lokal yang aktif. Aktifkan mode online " +
                "lokal lewat online.json untuk pengujian.",
                danger: false);
        }

        private GameObject AddBubble(string role, string text, string source)
        {
            var theme = _shell.Theme;
            _messageIndex += 1;

            var card = UIFactory.CreateCard(
                "Message_" + role + "_" + _messageIndex,
                _messages,
                role == "user" ? theme.SurfaceHigh : theme.SurfaceCard,
                theme,
                theme.Space4,
                new RectOffset(14, 14, 12, 12),
                (int)theme.Space12);

            UIFactory.CreateText(
                "Role",
                card,
                role == "user" ? "Kamu" : "Tutor AI",
                theme.BodyBoldFont,
                theme.LabelSize,
                role == "user" ? theme.TextMuted : theme.Accent);

            UIFactory.CreateText(
                "Text",
                card,
                text,
                theme.BodyFont,
                theme.BodySmallSize,
                theme.TextPrimary);

            if (!string.IsNullOrWhiteSpace(source))
            {
                UIFactory.CreateText(
                    "Source",
                    card,
                    source,
                    theme.BodyFont,
                    theme.LabelSize,
                    theme.TextMuted);
            }

            var panel = card.parent.gameObject;
            UIFactory.AddAccessibilityLabel(
                panel,
                (role == "user" ? "Pesan kamu: " : "Jawaban Tutor AI: ") + text);
            return panel;
        }

        private static void RemoveBubble(GameObject bubble)
        {
            if (bubble == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(bubble);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(bubble);
            }
        }

        private void AddHistory(string role, string content)
        {
            _history.Add(new TutorMessage { role = role, content = content });
            while (_history.Count > MaximumHistoryMessages)
            {
                _history.RemoveAt(0);
            }
        }

        private static string BuildSourceText(TutorAskResponse response)
        {
            if (response.sources == null || response.sources.Length == 0)
            {
                return null;
            }

            var titles = response.sources
                .Where(source => source != null && !string.IsNullOrWhiteSpace(source.title))
                .Select(source => source.title)
                .Distinct()
                .ToArray();

            return titles.Length == 0 ? null : "Berdasarkan: " + string.Join(", ", titles);
        }

        private void SetStatus(string message, bool danger)
        {
            if (_status == null)
            {
                return;
            }

            _status.color = danger ? _shell.Theme.DangerText : _shell.Theme.TextMuted;
            _status.text = message;
        }

        private void ScrollToBottom()
        {
            if (_scroll == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();
            _scroll.StopMovement();
            _scroll.verticalNormalizedPosition = 0f;
        }
    }
}
