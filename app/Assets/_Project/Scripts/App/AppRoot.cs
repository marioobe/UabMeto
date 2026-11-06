using UabMeto.App.Content;
using UabMeto.App.Local;
using UabMeto.App.Net;
using UabMeto.App.UI;
using UnityEngine;

namespace UabMeto.App
{
    /// <summary>
    /// Titik masuk scene Bootstrap. Memuat token, konten, dan penyimpanan lokal
    /// lalu membangun shell. Tidak ada panel debug di build rilis.
    /// </summary>
    public sealed class AppRoot : MonoBehaviour
    {
        public ThemeTokens Theme { get; private set; }
        public ContentCatalog Catalog { get; private set; }
        public LocalProfile Profile { get; private set; }
        public ProgressStore Progress { get; private set; }
        public OutboxStore Outbox { get; private set; }
        public ShellView Shell { get; private set; }

        /// <summary>Null bila mode online lokal tidak diaktifkan.</summary>
        public ApiClient Api { get; private set; }

        /// <summary>Null bila mode online lokal tidak diaktifkan.</summary>
        public SyncService Sync { get; private set; }

        private void Start()
        {
            Theme = ThemeTokens.Load();
            Catalog = ContentRepository.LoadDefault();
            Profile = LocalProfile.LoadDefault();
            Progress = ProgressStore.LoadDefault();
            Outbox = OutboxStore.LoadDefault();

            ConfigureOnline();

            Shell = gameObject.AddComponent<ShellView>();
            Shell.Initialize(Theme, Catalog, Profile, Progress, Outbox);

            if (!Profile.IsValid)
            {
                Shell.ShowNicknameDialog();
            }
        }

        /// <summary>
        /// Menyiapkan klien sinkron hanya bila penguji mengaktifkannya lewat
        /// online.json. Tidak ada permintaan jaringan otomatis di sini.
        /// </summary>
        private void ConfigureOnline()
        {
            var settings = OnlineSettings.LoadDefault();
            if (!settings.enabled || string.IsNullOrWhiteSpace(settings.baseUrl))
            {
                return;
            }

            var identity = OnlineIdentity.LoadOrCreateDefault();
            Api = new ApiClient(new UnityHttpTransport(), settings.baseUrl, identity);
            Sync = new SyncService(Api, Outbox, () => Profile.Nickname);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && Shell != null)
            {
                Shell.HandleBack();
            }
        }
    }
}
