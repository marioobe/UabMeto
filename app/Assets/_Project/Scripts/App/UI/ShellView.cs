using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UabMeto.App.Content;
using UabMeto.App.Local;
using UabMeto.Core.Content;
using UnityEngine;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Shell empat tab. F1 hanya Beranda dan alur pelajaran yang berfungsi;
    /// tab lain menampilkan status jujur "belum tersedia".
    /// </summary>
    public sealed class ShellView : MonoBehaviour
    {
        public enum Tab
        {
            Home,
            Tutor,
            Leaderboard,
            Profile
        }

        private ThemeTokens _theme;
        private ContentCatalog _catalog;
        private LocalProfile _profile;
        private ProgressStore _progress;
        private OutboxStore _outbox;

        private Canvas _canvas;
        private RectTransform _safeRoot;
        private RectTransform _contentHost;
        private TextMeshProUGUI _title;
        private TextMeshProUGUI _subtitle;
        private Button _backButton;
        private Image _offlineBanner;
        private TextMeshProUGUI _offlineText;
        private RectTransform _modalRoot;
        private LessonView _lessonView;
        private readonly List<TabButton> _tabButtons = new List<TabButton>();
        private Tab _tab = Tab.Home;
        private bool _built;

        public ThemeTokens Theme => _theme;
        public ContentCatalog Catalog => _catalog;
        public LocalProfile Profile => _profile;
        public ProgressStore Progress => _progress;
        public OutboxStore Outbox => _outbox;
        public RectTransform ContentHost => _contentHost;

        public void Initialize(
            ThemeTokens theme,
            ContentCatalog catalog,
            LocalProfile profile,
            ProgressStore progress,
            OutboxStore outbox)
        {
            _theme = theme;
            _catalog = catalog;
            _profile = profile;
            _progress = progress;
            _outbox = outbox;
            Build();
        }

        public void Build()
        {
            if (_built)
            {
                return;
            }

            _built = true;
            _canvas = UIFactory.CreateCanvas("UabMetoCanvas", _theme);
            _canvas.transform.SetParent(transform, false);
            var canvasRect = _canvas.GetComponent<RectTransform>();

            var background = UIFactory.CreatePanel("Background", canvasRect, _theme.Canvas);
            UIFactory.Stretch(background.rectTransform);

            _safeRoot = UIFactory.CreateRect("SafeArea", canvasRect);
            UIFactory.Stretch(_safeRoot);
            _safeRoot.gameObject.AddComponent<SafeAreaFitter>();

            var rootLayout = _safeRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            rootLayout.spacing = 0f;
            rootLayout.padding = new RectOffset();
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = true;
            rootLayout.childForceExpandHeight = false;
            rootLayout.childAlignment = TextAnchor.UpperCenter;

            BuildHeader();
            BuildOfflineBanner();
            BuildContentHost();
            BuildBottomNavigation();
            ShowTab(Tab.Home);
        }

        private void BuildHeader()
        {
            var header = UIFactory.CreatePanel("Header", _safeRoot, _theme.Surface);
            var headerLayout = header.gameObject.AddComponent<LayoutElement>();
            headerLayout.minHeight = 72f;
            headerLayout.preferredHeight = 72f;

            var row = UIFactory.CreateHorizontalGroup(
                "HeaderRow",
                header.transform,
                8f,
                new RectOffset(8, 12, 8, 8));
            UIFactory.Stretch(row);

            _backButton = UIFactory.CreateIconButton(
                "Back",
                row,
                "arrow_back",
                "Kembali",
                _theme,
                OnBack);
            _backButton.gameObject.SetActive(false);

            var titles = UIFactory.CreateVerticalGroup("Titles", row, 0f, null, false);
            var titlesLayout = titles.gameObject.AddComponent<LayoutElement>();
            titlesLayout.flexibleWidth = 1f;

            _title = UIFactory.CreateText(
                "Title",
                titles,
                "Uab Meto",
                _theme.HeadingBoldFont,
                _theme.Heading2Size,
                _theme.TextPrimary);
            _subtitle = UIFactory.CreateText(
                "Subtitle",
                titles,
                "Bahasa Dawan · TTU",
                _theme.BodyFont,
                _theme.LabelSize,
                _theme.TextMuted);

            UIFactory.CreateIconButton(
                "Avatar",
                row,
                "person",
                "Buka profil",
                _theme,
                () => ShowTab(Tab.Profile),
                _theme.SurfaceHigh);
        }

        private void BuildOfflineBanner()
        {
            _offlineBanner = UIFactory.CreatePanel("OfflineBanner", _safeRoot, _theme.Warning);
            var layout = _offlineBanner.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 44f;
            layout.preferredHeight = 44f;

            var row = UIFactory.CreateHorizontalGroup(
                "BannerRow",
                _offlineBanner.transform,
                8f,
                new RectOffset(16, 16, 4, 4));
            UIFactory.Stretch(row);

            var icon = UIFactory.CreateIcon(
                "Icon",
                row,
                "wifi_off",
                _theme.IconFont,
                20f,
                _theme.OnAccent);
            UIFactory.AddLayoutElement(icon.gameObject, minWidth: 24f, preferredWidth: 24f);

            _offlineText = UIFactory.CreateText(
                "Text",
                row,
                string.Empty,
                _theme.BodyFont,
                _theme.LabelSize,
                _theme.OnAccent,
                TextAlignmentOptions.MidlineLeft);
            UIFactory.AddLayoutElement(_offlineText.gameObject, flexibleHeight: 0f);

            _offlineBanner.gameObject.SetActive(false);
        }

        private void BuildContentHost()
        {
            _contentHost = UIFactory.CreateRect("Content", _safeRoot);
            var layout = _contentHost.gameObject.AddComponent<LayoutElement>();
            layout.flexibleHeight = 1f;
            layout.minHeight = 0f;
        }

        private void BuildBottomNavigation()
        {
            var nav = UIFactory.CreatePanel("BottomNav", _safeRoot, _theme.Surface);
            var layout = nav.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 64f;
            layout.preferredHeight = 64f;

            var row = UIFactory.CreateHorizontalGroup(
                "Tabs",
                nav.transform,
                0f,
                new RectOffset(8, 8, 4, 4));
            UIFactory.Stretch(row);

            AddTab(row, Tab.Home, "home", "Beranda");
            AddTab(row, Tab.Tutor, "menu_book", "Tutor AI");
            AddTab(row, Tab.Leaderboard, "leaderboard", "Peringkat");
            AddTab(row, Tab.Profile, "person", "Profil");
        }

        private void AddTab(Transform parent, Tab tab, string iconName, string label)
        {
            var panel = UIFactory.CreatePanel(
                "Tab_" + tab,
                parent,
                new Color(_theme.Surface.r, _theme.Surface.g, _theme.Surface.b, 0f),
                (int)_theme.Space12,
                true);
            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.flexibleWidth = 1f;
            layout.minWidth = _theme.TouchTarget;
            layout.minHeight = 56f;
            layout.preferredHeight = 56f;

            var button = panel.gameObject.AddComponent<Button>();
            button.targetGraphic = panel;
            button.transition = Selectable.Transition.ColorTint;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            button.colors = colors;
            button.onClick.AddListener(() => ShowTab(tab));

            var group = UIFactory.CreateVerticalGroup(
                "TabContent",
                panel.transform,
                0f,
                new RectOffset(0, 0, 6, 4),
                false);
            UIFactory.Stretch(group);

            var icon = UIFactory.CreateIcon(
                "Icon",
                group,
                iconName,
                _theme.IconFont,
                22f,
                _theme.TextMuted);
            UIFactory.AddLayoutElement(icon.gameObject, minHeight: 24f, preferredHeight: 24f);

            var text = UIFactory.CreateText(
                "Label",
                group,
                label,
                _theme.BodyFont,
                11f,
                _theme.TextMuted,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(text.gameObject, minHeight: 16f, preferredHeight: 16f);

            UIFactory.AddAccessibilityLabel(button.gameObject, label);
            _tabButtons.Add(new TabButton
            {
                Tab = tab,
                Background = panel,
                Icon = icon,
                Label = text
            });
        }

        public void ShowTab(Tab tab)
        {
            _tab = tab;
            _lessonView = null;
            if (_backButton != null)
            {
                _backButton.gameObject.SetActive(false);
            }

            UIFactory.DestroyChildren(_contentHost);
            switch (tab)
            {
                case Tab.Home:
                    SetHeader("Uab Meto", "Bahasa Dawan · TTU");
                    new HomeView(this).Build(_contentHost);
                    break;
                case Tab.Tutor:
                    SetHeader("Tutor AI", "Latihan percakapan");
                    BuildPlaceholder(
                        "Tutor AI belum tersedia",
                        "Fitur Tutor AI akan dikerjakan setelah alur pelajaran dan konten tervalidasi siap. " +
                        "Tidak ada percakapan yang dikarang untuk mengisi layar ini.",
                        "menu_book");
                    break;
                case Tab.Leaderboard:
                    SetHeader("Peringkat", "Mingguan · reset Senin 00.00 WITA");
                    BuildPlaceholder(
                        "Peringkat belum tersedia",
                        "Papan peringkat memerlukan data server yang belum ada. F1 tidak menampilkan " +
                        "angka atau nama peserta rekaan.",
                        "leaderboard");
                    break;
                case Tab.Profile:
                    SetHeader("Profil", "Data lokal perangkat");
                    new ProfileView(this).Build(_contentHost);
                    break;
            }

            UpdateTabStyles();
            RebuildLayout();
        }

        public void ShowLesson(LessonContent lesson, bool fixture)
        {
            if (lesson == null)
            {
                return;
            }

            UIFactory.DestroyChildren(_contentHost);
            _backButton.gameObject.SetActive(true);
            SetHeader("Pelajaran", lesson.title);

            _lessonView = new LessonView(
                this,
                lesson,
                fixture,
                () => ShowTab(Tab.Home),
                OnLessonCompleted);
            _lessonView.Build(_contentHost);
            RebuildLayout();
        }

        public void RebuildLayout()
        {
            Canvas.ForceUpdateCanvases();
            if (_canvas != null)
            {
                // Rebuild dari akar ke daun agar ukuran induk sudah benar
                // sebelum grup anak menghitung lebar teks.
                var groups = _canvas.GetComponentsInChildren<LayoutGroup>(true)
                    .OrderBy(group => Depth(group.transform))
                    .ToArray();
                // Dua lintasan: induk dapat mengubah ukuran anak setelah anak
                // sempat dihitung pada lintasan pertama.
                for (var pass = 0; pass < 2; pass++)
                {
                    foreach (var group in groups)
                    {
                        var rect = (RectTransform)group.transform;
                        LayoutRebuilder.MarkLayoutForRebuild(rect);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
                    }
                }
            }
        }

        private static int Depth(Transform transform)
        {
            var depth = 0;
            while (transform.parent != null)
            {
                depth++;
                transform = transform.parent;
            }

            return depth;
        }

        private void OnLessonCompleted(LessonContent lesson)
        {
            var item = _outbox.Enqueue(lesson.id, lesson.schemaVersion, lesson.xp);
            _progress.RecordCompletion(lesson.id, lesson.xp);
            Debug.Log(
                $"[UabMeto] Hasil pelajaran masuk outbox: {lesson.id} " +
                $"operation={item.operationId} xpPending={lesson.xp}");
            ShowTab(Tab.Home);
        }

        private void SetHeader(string title, string subtitle)
        {
            if (_title != null)
            {
                _title.text = title;
            }

            if (_subtitle != null)
            {
                _subtitle.text = subtitle;
            }
        }

        private void UpdateTabStyles()
        {
            foreach (var tabButton in _tabButtons)
            {
                var selected = tabButton.Tab == _tab;
                tabButton.Icon.color = selected ? _theme.Accent : _theme.TextMuted;
                tabButton.Label.color = selected ? _theme.Accent : _theme.TextMuted;
                tabButton.Background.color = selected
                    ? new Color(_theme.SurfaceHigh.r, _theme.SurfaceHigh.g, _theme.SurfaceHigh.b, 1f)
                    : new Color(_theme.Surface.r, _theme.Surface.g, _theme.Surface.b, 0f);
            }
        }

        private void BuildPlaceholder(string title, string body, string iconName)
        {
            var scroll = UIFactory.CreateScrollView("PlaceholderScroll", _contentHost, _theme);
            UIFactory.Stretch(scroll.GetComponent<RectTransform>());
            var content = scroll.content;

            var card = UIFactory.CreatePanel("PlaceholderCard", content, _theme.SurfaceCard, (int)_theme.CornerRadius);
            UIFactory.AddLayoutElement(card.gameObject, preferredHeight: 260f);

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                _theme.Space12,
                new RectOffset(20, 20, 24, 24),
                false);
            UIFactory.Stretch(group);

            var icon = UIFactory.CreateIcon(
                "Icon",
                group,
                iconName,
                _theme.IconFont,
                40f,
                _theme.Outline);
            UIFactory.AddLayoutElement(icon.gameObject, minHeight: 48f, preferredHeight: 48f);

            var heading = UIFactory.CreateText(
                "Title",
                group,
                title,
                _theme.HeadingBoldFont,
                _theme.Heading2Size,
                _theme.TextPrimary,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(heading.gameObject, preferredHeight: 30f);

            var bodyText = UIFactory.CreateText(
                "Body",
                group,
                body,
                _theme.BodyFont,
                _theme.BodySmallSize,
                _theme.TextMuted,
                TextAlignmentOptions.Center);
            UIFactory.AddLayoutElement(bodyText.gameObject, preferredHeight: 80f);
        }

        public void ShowModal(
            string title,
            string body,
            string primaryLabel,
            Action primary,
            string secondaryLabel = null,
            Action secondary = null,
            bool dismissible = false)
        {
            CloseModal();

            _modalRoot = UIFactory.CreateRect("Modal", _canvas.transform);
            UIFactory.Stretch(_modalRoot);

            var scrim = UIFactory.CreatePanel("Scrim", _modalRoot, _theme.Scrim, 0, true);
            UIFactory.Stretch(scrim.rectTransform);
            if (dismissible)
            {
                var scrimButton = scrim.gameObject.AddComponent<Button>();
                scrimButton.targetGraphic = scrim;
                scrimButton.transition = Selectable.Transition.None;
                scrimButton.onClick.AddListener(CloseModal);
            }

            var card = UIFactory.CreatePanel("Card", _modalRoot, _theme.SurfaceCard, (int)_theme.CornerRadius);
            var cardRect = card.rectTransform;
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.sizeDelta = new Vector2(320f, 0f);

            var fitter = card.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                _theme.Space12,
                new RectOffset(20, 20, 20, 20),
                false);
            UIFactory.Stretch(group);

            var heading = UIFactory.CreateText(
                "Title",
                group,
                title,
                _theme.HeadingBoldFont,
                _theme.Heading2Size,
                _theme.TextPrimary);
            UIFactory.AddLayoutElement(heading.gameObject, preferredHeight: 30f);

            var bodyText = UIFactory.CreateText(
                "Body",
                group,
                body,
                _theme.BodyFont,
                _theme.BodySmallSize,
                _theme.TextMuted);
            UIFactory.AddLayoutElement(bodyText.gameObject, preferredHeight: 60f);

            if (!string.IsNullOrWhiteSpace(secondaryLabel))
            {
                UIFactory.CreateButton(
                    "Secondary",
                    group,
                    secondaryLabel,
                    _theme,
                    () =>
                    {
                        CloseModal();
                        secondary?.Invoke();
                    },
                    primary: false);
            }

            UIFactory.CreateButton(
                "Primary",
                group,
                primaryLabel,
                _theme,
                () =>
                {
                    CloseModal();
                    primary?.Invoke();
                });
        }

        public void ShowNicknameDialog()
        {
            CloseModal();

            _modalRoot = UIFactory.CreateRect("NicknameModal", _canvas.transform);
            UIFactory.Stretch(_modalRoot);

            var scrim = UIFactory.CreatePanel("Scrim", _modalRoot, _theme.Scrim, 0, true);
            UIFactory.Stretch(scrim.rectTransform);

            var card = UIFactory.CreatePanel("Card", _modalRoot, _theme.SurfaceCard, (int)_theme.CornerRadius);
            var cardRect = card.rectTransform;
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.sizeDelta = new Vector2(320f, 0f);

            var fitter = card.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var group = UIFactory.CreateVerticalGroup(
                "Body",
                card.transform,
                _theme.Space12,
                new RectOffset(20, 20, 20, 20),
                false);
            UIFactory.Stretch(group);

            var heading = UIFactory.CreateText(
                "Title",
                group,
                "Siapa nama panggilanmu?",
                _theme.HeadingBoldFont,
                _theme.Heading2Size,
                _theme.TextPrimary);
            UIFactory.AddLayoutElement(heading.gameObject, preferredHeight: 30f);

            var body = UIFactory.CreateText(
                "Body",
                group,
                "Nama ini disimpan di perangkat dan dipakai untuk menyapa. " +
                "Belum ada akun server pada F1.",
                _theme.BodyFont,
                _theme.BodySmallSize,
                _theme.TextMuted);
            UIFactory.AddLayoutElement(body.gameObject, preferredHeight: 56f);

            var input = UIFactory.CreateInputField(
                "Nickname",
                group,
                "3–20 karakter",
                _theme,
                LocalProfile.MaximumNicknameLength);
            var error = UIFactory.CreateText(
                "Error",
                group,
                string.Empty,
                _theme.BodyFont,
                _theme.LabelSize,
                _theme.DangerText);
            UIFactory.AddLayoutElement(error.gameObject, preferredHeight: 18f);

            UIFactory.CreateButton(
                "Save",
                group,
                "Simpan",
                _theme,
                () =>
                {
                    if (_profile.TrySetNickname(input.text, out var message))
                    {
                        CloseModal();
                        ShowTab(Tab.Home);
                    }
                    else
                    {
                        error.text = message;
                    }
                });
        }

        public void CloseModal()
        {
            if (_modalRoot != null)
            {
                Destroy(_modalRoot.gameObject);
                _modalRoot = null;
            }
        }

        public void HandleBack()
        {
            if (_modalRoot != null)
            {
                CloseModal();
                return;
            }

            OnBack();
        }

        private void OnBack()
        {
            if (_lessonView != null)
            {
                _lessonView.Back();
                return;
            }

            ShowTab(Tab.Home);
        }

        private void Update()
        {
            if (!_built || _offlineBanner == null)
            {
                return;
            }

            var offline = Application.internetReachability == NetworkReachability.NotReachable;
            var pending = _outbox == null ? 0 : _outbox.PendingCount;
            var visible = offline || pending > 0;
            if (_offlineBanner.gameObject.activeSelf != visible)
            {
                _offlineBanner.gameObject.SetActive(visible);
            }

            if (!visible)
            {
                return;
            }

            if (offline && pending > 0)
            {
                _offlineText.text =
                    $"Offline — {pending} hasil menunggu sinkron. Pembelajaran tetap bisa dilanjutkan.";
            }
            else if (offline)
            {
                _offlineText.text = "Offline — pembelajaran tetap bisa dilanjutkan. Hasil akan disinkronkan saat online.";
            }
            else
            {
                _offlineText.text = $"{pending} hasil menunggu sinkron.";
            }
        }

        private sealed class TabButton
        {
            public Tab Tab;
            public Image Background;
            public TextMeshProUGUI Icon;
            public TextMeshProUGUI Label;
        }
    }
}
