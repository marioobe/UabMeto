using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UabMeto.App.Content;
using UabMeto.App.Local;
using UabMeto.App.Net;
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
        private ApiClient _api;
        private SyncService _sync;

        private Canvas _canvas;
        private RectTransform _safeRoot;
        private CanvasGroup _contentGroup;
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
        private bool _syncing;
        private string _lastSyncMessage = string.Empty;
        private bool _lastSyncFailed;

        public ThemeTokens Theme => _theme;
        public ContentCatalog Catalog => _catalog;
        public LocalProfile Profile => _profile;
        public ProgressStore Progress => _progress;
        public OutboxStore Outbox => _outbox;
        public RectTransform ContentHost => _contentHost;

        /// <summary>Null bila mode online lokal tidak diaktifkan.</summary>
        public ApiClient Api => _api;

        /// <summary>Null bila mode online lokal tidak diaktifkan.</summary>
        public SyncService Sync => _sync;

        public bool OnlineConfigured => _api != null && _sync != null;

        public bool IsSyncing => _syncing;

        public string LastSyncMessage => _lastSyncMessage;

        public bool LastSyncFailed => _lastSyncFailed;

        /// <summary>Teks banner status saat ini; kosong bila tidak tampil.</summary>
        public string BannerText =>
            _offlineText == null ? string.Empty : _offlineText.text;

        public bool BannerVisible =>
            _offlineBanner != null && _offlineBanner.gameObject.activeSelf;

        public void Initialize(
            ThemeTokens theme,
            ContentCatalog catalog,
            LocalProfile profile,
            ProgressStore progress,
            OutboxStore outbox,
            ApiClient api = null,
            SyncService sync = null)
        {
            _theme = theme;
            _catalog = catalog;
            _profile = profile;
            _progress = progress;
            _outbox = outbox;
            _api = api;
            _sync = sync;
            // Dibaca sebelum UI dibangun. Setelan font Android biasanya
            // memicu pembuatan ulang activity, jadi pembacaan saat mulai
            // cukup untuk F1; perubahan langsung ditangani dengan restart.
            UiScale.Refresh();
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
            headerLayout.minHeight = UIFactory.Px(72f);
            headerLayout.preferredHeight = UIFactory.Px(72f);

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

            // Tinggi banner mengikuti isi. Teks panjang akan membungkus ke
            // baris berikutnya, bukan meluber menimpa konten di bawahnya.
            var bannerLayout = _offlineBanner.gameObject.AddComponent<VerticalLayoutGroup>();
            bannerLayout.spacing = 0f;
            bannerLayout.padding = new RectOffset();
            bannerLayout.childControlWidth = true;
            bannerLayout.childControlHeight = true;
            bannerLayout.childForceExpandWidth = true;
            bannerLayout.childForceExpandHeight = false;
            bannerLayout.childAlignment = TextAnchor.UpperLeft;

            var row = UIFactory.CreateHorizontalGroup(
                "BannerRow",
                _offlineBanner.transform,
                8f,
                new RectOffset(16, 16, 8, 8));
            row.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var icon = UIFactory.CreateIcon(
                "Icon",
                row,
                "wifi_off",
                _theme.IconFont,
                20f,
                _theme.OnAccent);
            UIFactory.AddLayoutElement(
                icon.gameObject,
                minWidth: 24f,
                preferredWidth: 24f,
                minHeight: 24f,
                preferredHeight: 24f);

            _offlineText = UIFactory.CreateText(
                "Text",
                row,
                string.Empty,
                _theme.BodyFont,
                _theme.LabelSize,
                _theme.OnAccent,
                TextAlignmentOptions.MidlineLeft);
            UIFactory.AddLayoutElement(
                _offlineText.gameObject,
                preferredWidth: 0f,
                flexibleWidth: 1f);

            _offlineBanner.gameObject.SetActive(false);
        }

        private void BuildContentHost()
        {
            _contentHost = UIFactory.CreateRect("Content", _safeRoot);
            var layout = _contentHost.gameObject.AddComponent<LayoutElement>();
            layout.flexibleHeight = 1f;
            layout.minHeight = 0f;

            // Dipakai untuk transisi masuk antar-tab. Alpha saja; layout tidak
            // terpengaruh sehingga rect tetap bisa diukur tes.
            _contentGroup = _contentHost.gameObject.AddComponent<CanvasGroup>();
            _contentGroup.interactable = true;
            _contentGroup.blocksRaycasts = true;
        }

        private void BuildBottomNavigation()
        {
            var nav = UIFactory.CreatePanel("BottomNav", _safeRoot, _theme.Surface);
            var layout = nav.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = UIFactory.Px(68f);
            layout.preferredHeight = UIFactory.Px(68f);

            // Garis rambut di tepi atas memisahkan bar dari konten tanpa bayangan.
            var hairline = UIFactory.CreatePanel("Hairline", nav.transform, _theme.Border, 0);
            hairline.raycastTarget = false;
            var hairlineRect = hairline.rectTransform;
            hairlineRect.anchorMin = new Vector2(0f, 1f);
            hairlineRect.anchorMax = new Vector2(1f, 1f);
            hairlineRect.pivot = new Vector2(0.5f, 1f);
            hairlineRect.anchoredPosition = Vector2.zero;
            hairlineRect.sizeDelta = new Vector2(0f, 1f);
            hairline.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;

            var row = UIFactory.CreateHorizontalGroup(
                "Tabs",
                nav.transform,
                0f,
                new RectOffset(8, 8, 6, 6));
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
                Transparent(_theme.Surface),
                (int)_theme.Space24,
                true);
            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.flexibleWidth = 1f;
            layout.minWidth = _theme.TouchTarget;
            layout.minHeight = UIFactory.Px(56f);
            layout.preferredHeight = UIFactory.Px(56f);

            var button = panel.gameObject.AddComponent<Button>();
            button.targetGraphic = panel;
            button.transition = Selectable.Transition.ColorTint;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            button.colors = colors;
            button.onClick.AddListener(() => ShowTab(tab));
            panel.gameObject.AddComponent<PressScale>();

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
            UIFactory.AddLayoutElement(icon.gameObject, minHeight: 28f, preferredHeight: 28f);

            var text = UIFactory.CreateText(
                "Label",
                group,
                label,
                _theme.BodyFont,
                _theme.LabelSize,
                _theme.TextMuted,
                TextAlignmentOptions.Center);
            text.characterSpacing = 4f;
            // Label tab tidak boleh turun menjadi dua baris. Autosize hanya
            // mengecilkan bila "Peringkat" tidak muat pada font 2.0x.
            text.enableAutoSizing = true;
            text.fontSizeMin = 10f;
            text.fontSizeMax = text.fontSize;
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
                    SetHeader("Tutor AI", "Tanya materi tervalidasi");
                    new TutorView(this).Build(_contentHost);
                    break;
                case Tab.Leaderboard:
                    SetHeader("Peringkat", "Mingguan · reset Senin 00.00 WITA");
                    new LeaderboardView(this).Build(_contentHost);
                    break;
                case Tab.Profile:
                    SetHeader("Profil", "Data lokal perangkat");
                    new ProfileView(this).Build(_contentHost);
                    break;
            }

            UpdateTabStyles();
            RebuildLayout();
            ResetScrollToTop();
            PlayContentEntrance();
        }

        /// <summary>Konten tab memudar masuk sambil naik 8 dp (200 ms).</summary>
        private void PlayContentEntrance()
        {
            if (_contentGroup == null || _contentHost == null || _contentHost.childCount == 0)
            {
                return;
            }

            var target = _contentHost.GetChild(0) as RectTransform;
            UITween.SlideFade(_contentGroup, target, 8f, UITween.Normal);
        }

        private static Color Transparent(Color color) =>
            new Color(color.r, color.g, color.b, 0f);

        public void ShowLesson(LessonContent lesson, bool fixture, Action onExit = null)
        {
            ShowLessonInternal(lesson, fixture, false, onExit);
        }

        /// <summary>
        /// Membuka pelajaran <c>in_review</c> dalam mode pratinjau. Tidak
        /// pernah mencatat progres atau XP.
        /// </summary>
        public void ShowPreviewLesson(LessonContent lesson, Action onExit = null)
        {
            ShowLessonInternal(lesson, false, true, onExit);
        }

        /// <summary>
        /// Menampilkan daftar pelajaran satu topik. Dipakai bila topik punya
        /// lebih dari satu pelajaran sehingga pengguna bisa memilih sendiri.
        /// </summary>
        public void ShowLessonList(TopicEntry topic)
        {
            if (topic == null)
            {
                return;
            }

            _lessonView = null;
            UIFactory.DestroyChildren(_contentHost);
            _backButton.gameObject.SetActive(true);
            SetHeader(topic.title, "Daftar pelajaran");
            new LessonListView(this, topic).Build(_contentHost);
            RebuildLayout();
            ResetScrollToTop();
        }

        private void ShowLessonInternal(LessonContent lesson, bool fixture, bool preview, Action onExit)
        {
            if (lesson == null)
            {
                return;
            }

            UIFactory.DestroyChildren(_contentHost);
            _backButton.gameObject.SetActive(true);
            SetHeader(preview ? "Pratinjau pelajaran" : "Pelajaran", lesson.title);

            var alreadyCompleted =
                !preview &&
                !fixture &&
                _progress != null &&
                _progress.IsCompleted(lesson.id);

            _lessonView = new LessonView(
                this,
                lesson,
                fixture,
                preview,
                alreadyCompleted,
                onExit ?? (() => ShowTab(Tab.Home)),
                OnLessonCompleted);
            _lessonView.Build(_contentHost);
            RebuildLayout();
            ResetScrollToTop();
        }

        /// <summary>
        /// Mengembalikan semua daftar gulir ke posisi teratas. Dipanggil
        /// setelah layar dibangun ulang supaya pengguna tidak mendarat di
        /// tengah materi.
        /// </summary>
        public void ResetScrollToTop()
        {
            if (_contentHost == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();
            foreach (var scroll in _contentHost.GetComponentsInChildren<ScrollRect>(true))
            {
                scroll.StopMovement();
                scroll.verticalNormalizedPosition = 1f;
            }
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
                // Tiga lintasan: induk dapat mengubah ukuran anak setelah anak
                // sempat dihitung pada lintasan sebelumnya, terutama karena
                // kartu memakai ContentSizeFitter bersarang.
                for (var pass = 0; pass < 3; pass++)
                {
                    foreach (var group in groups)
                    {
                        var rect = (RectTransform)group.transform;
                        LayoutRebuilder.MarkLayoutForRebuild(rect);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
                    }

                    Canvas.ForceUpdateCanvases();
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
            // PRD §523: sinkronisasi tidak boleh menggandakan XP. Pelajaran
            // tetap boleh diulang untuk latihan, tetapi hasil ulangan tidak
            // diantrekan lagi setelah pelajaran tercatat selesai.
            if (_progress != null && _progress.IsCompleted(lesson.id))
            {
                Debug.Log(
                    $"[UabMeto] Pelajaran {lesson.id} sudah selesai; " +
                    "hasil latihan ulang tidak diantrekan agar XP tidak berlipat.");
                ShowTab(Tab.Home);
                return;
            }

            var item = _outbox.Enqueue(lesson.id, lesson.schemaVersion, lesson.xp);
            _progress.RecordCompletion(lesson.id, lesson.xp);
            Debug.Log(
                $"[UabMeto] Hasil pelajaran masuk outbox: {lesson.id} " +
                $"operation={item.operationId} xpPending={lesson.xp}");
            ShowTab(Tab.Home);
        }

        /// <summary>
        /// Dipanggil tombol UI. Aman dipanggil saat mode online nonaktif.
        /// </summary>
        public void RequestSync()
        {
            _ = SyncNowAsync();
        }

        /// <summary>
        /// Satu putaran sinkron. Tidak pernah berjalan otomatis saat aplikasi
        /// dibuka; hanya dari aksi pengguna.
        /// </summary>
        public async Task SyncNowAsync(CancellationToken cancellationToken = default)
        {
            if (!OnlineConfigured || _syncing)
            {
                return;
            }

            _syncing = true;
            _lastSyncMessage = "Menyinkronkan hasil…";
            _lastSyncFailed = false;

            try
            {
                var report = await _sync.SyncPendingAsync(cancellationToken);
                ApplySyncReport(report);
            }
            catch (Exception exception)
            {
                _lastSyncMessage = "Sinkron gagal: " + exception.Message;
                _lastSyncFailed = true;
                Debug.LogWarning("[UabMeto] Sinkron gagal: " + exception);
            }
            finally
            {
                _syncing = false;
                if (_tab == Tab.Profile)
                {
                    ShowTab(Tab.Profile);
                }
            }
        }

        private void ApplySyncReport(SyncReport report)
        {
            // Server sudah mengonfirmasi sebagian hasil; XP tertunda harus
            // turun mengikuti sisa outbox, bukan tetap pada angka lama.
            _progress?.ReconcilePendingXp(_outbox.PendingXpTotal);

            _lastSyncFailed =
                report.Unauthorized ||
                report.Offline ||
                report.Rejected > 0 ||
                report.Failed > 0;

            if (report.Unauthorized)
            {
                _lastSyncMessage = "Sinkron ditolak server: identitas pengembangan tidak sah.";
            }
            else if (report.Offline)
            {
                _lastSyncMessage = "Server lokal tidak dapat dihubungi. Hasil tetap tersimpan.";
            }
            else if (report.Confirmed > 0)
            {
                _lastSyncMessage = $"Sinkron selesai: {report.Confirmed} hasil terkirim.";
            }
            else if (report.Rejected > 0)
            {
                _lastSyncMessage = $"{report.Rejected} hasil ditolak: konten belum tervalidasi.";
            }
            else if (report.Failed > 0)
            {
                _lastSyncMessage = $"{report.Failed} hasil butuh rekonsiliasi.";
            }
            else if (report.StillPending > 0)
            {
                _lastSyncMessage = $"{report.StillPending} hasil menunggu percobaan berikutnya.";
            }
            else
            {
                _lastSyncMessage = "Tidak ada hasil menunggu sinkron.";
            }
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
            var activeBackground = new Color(_theme.Accent.r, _theme.Accent.g, _theme.Accent.b, 0.12f);
            var inactiveBackground = Transparent(_theme.Surface);
            foreach (var tabButton in _tabButtons)
            {
                var selected = tabButton.Tab == _tab;
                tabButton.Icon.color = selected ? _theme.Accent : _theme.TextMuted;
                tabButton.Label.color = selected ? _theme.Accent : _theme.TextMuted;
                tabButton.Label.font = selected ? _theme.BodyBoldFont : _theme.BodyFont;
                tabButton.Background.color = selected ? activeBackground : inactiveBackground;
            }
        }

        private void BuildPlaceholder(string title, string body, string iconName)
        {
            var scroll = UIFactory.CreateScrollView("PlaceholderScroll", _contentHost, _theme);
            UIFactory.Stretch(scroll.GetComponent<RectTransform>());
            var content = scroll.content;

            var group = UIFactory.CreateCard(
                "PlaceholderCard",
                content,
                _theme.SurfaceCard,
                _theme,
                _theme.Space12,
                new RectOffset(20, 20, 24, 24),
                (int)_theme.CornerRadius);
            group.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

            var icon = UIFactory.CreateIcon(
                "Icon",
                group,
                iconName,
                _theme.IconFont,
                40f,
                _theme.Outline);
            UIFactory.AddLayoutElement(
                icon.gameObject,
                minHeight: 48f,
                preferredHeight: 48f,
                minWidth: 48f,
                preferredWidth: 48f);

            UIFactory.CreateText(
                "Title",
                group,
                title,
                _theme.HeadingBoldFont,
                _theme.Heading2Size,
                _theme.TextPrimary,
                TextAlignmentOptions.Center);

            UIFactory.CreateText(
                "Body",
                group,
                body,
                _theme.BodyFont,
                _theme.BodySmallSize,
                _theme.TextMuted,
                TextAlignmentOptions.Center);
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
            var card = CreateModalCard("Modal", dismissible);

            UIFactory.CreateText(
                "Title",
                card,
                title,
                _theme.HeadingBoldFont,
                _theme.Heading2Size,
                _theme.TextPrimary);

            UIFactory.CreateText(
                "Body",
                card,
                body,
                _theme.BodyFont,
                _theme.BodySmallSize,
                _theme.TextMuted);

            if (!string.IsNullOrWhiteSpace(secondaryLabel))
            {
                UIFactory.CreateButton(
                    "Secondary",
                    card,
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
                card,
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
            var card = CreateModalCard("NicknameModal", false);

            UIFactory.CreateText(
                "Title",
                card,
                "Siapa nama panggilanmu?",
                _theme.HeadingBoldFont,
                _theme.Heading2Size,
                _theme.TextPrimary);

            UIFactory.CreateText(
                "Body",
                card,
                "Nama ini disimpan di perangkat dan dipakai untuk menyapa. " +
                "Belum ada akun server pada F1.",
                _theme.BodyFont,
                _theme.BodySmallSize,
                _theme.TextMuted);

            var input = UIFactory.CreateInputField(
                "Nickname",
                card,
                "3–20 karakter",
                _theme,
                LocalProfile.MaximumNicknameLength);
            var error = UIFactory.CreateText(
                "Error",
                card,
                string.Empty,
                _theme.BodyFont,
                _theme.LabelSize,
                _theme.DangerText);
            UIFactory.AddLayoutElement(error.gameObject, preferredHeight: 18f);

            UIFactory.CreateButton(
                "Cancel",
                card,
                "Batal",
                _theme,
                CloseModal,
                primary: false);
            UIFactory.CreateButton(
                "Save",
                card,
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

        private RectTransform CreateModalCard(string name, bool dismissible)
        {
            CloseModal();

            _modalRoot = UIFactory.CreateRect(name, _canvas.transform);
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
            var canvasRect = (RectTransform)_canvas.transform;
            var canvasWidth = canvasRect.rect.width;
            var canvasHeight = canvasRect.rect.height;
            cardRect.sizeDelta = new Vector2(
                Mathf.Min(UIFactory.Px(320f), canvasWidth - UIFactory.Px(32f)),
                0f);

            // Dialog harus tetap bisa dipakai pada font 2.0x. Isinya diletakkan
            // di dalam ScrollRect; tinggi kartu dibatasi oleh ModalCardFitter
            // agar tombol tidak pernah keluar layar.
            var viewport = UIFactory.CreateRect("Viewport", card.transform);
            UIFactory.Stretch(viewport);
            viewport.gameObject.AddComponent<RectMask2D>();
            var hitArea = viewport.gameObject.AddComponent<Image>();
            hitArea.color = new Color(0f, 0f, 0f, 0f);
            hitArea.raycastTarget = true;

            var content = UIFactory.CreateRect("Content", viewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;

            var cardLayout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            cardLayout.spacing = UIFactory.Px(_theme.Space12);
            cardLayout.padding = UIFactory.ScalePadding(new RectOffset(20, 20, 20, 20));
            cardLayout.childControlWidth = true;
            cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = true;
            cardLayout.childForceExpandHeight = false;

            var contentFitter = content.gameObject.AddComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = card.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.inertia = true;
            scroll.scrollSensitivity = UIFactory.Px(24f);

            var fitter = card.gameObject.AddComponent<ModalCardFitter>();
            fitter.Configure(cardRect, content, canvasHeight - UIFactory.Px(32f));

            return content;
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

            SetBanner(
                BannerMessage(
                    _syncing,
                    _lastSyncFailed,
                    _lastSyncMessage,
                    offline,
                    OnlineConfigured,
                    pending));
        }

        /// <summary>
        /// Menentukan teks banner. Mode online lokal tidak boleh disebut
        /// "Offline" hanya karena <see cref="Application.internetReachability"/>
        /// tidak melihat jaringan: server lokal bisa tetap terjangkau lewat
        /// <c>adb reverse</c>. Kegagalan server dilaporkan dari hasil sinkron.
        /// </summary>
        public static string BannerMessage(
            bool syncing,
            bool lastSyncFailed,
            string lastSyncMessage,
            bool offline,
            bool onlineConfigured,
            int pending)
        {
            if (syncing)
            {
                return "Menyinkronkan hasil…";
            }

            if (lastSyncFailed && !string.IsNullOrEmpty(lastSyncMessage))
            {
                return lastSyncMessage;
            }

            if (offline && !onlineConfigured)
            {
                return pending > 0
                    ? $"Offline — {pending} hasil menunggu sinkron. " +
                      "Pembelajaran tetap bisa dilanjutkan."
                    : "Offline — pembelajaran tetap bisa dilanjutkan. " +
                      "Hasil akan disinkronkan saat online.";
            }

            if (pending > 0)
            {
                return $"{pending} hasil menunggu sinkron.";
            }

            return null;
        }

        private void SetBanner(string message)
        {
            var visible = !string.IsNullOrEmpty(message);
            if (_offlineBanner.gameObject.activeSelf != visible)
            {
                _offlineBanner.gameObject.SetActive(visible);
            }

            if (visible)
            {
                _offlineText.text = message;
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
