using UabMeto.App.Content;
using UabMeto.App.Local;
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

        private void Start()
        {
            Theme = ThemeTokens.Load();
            Catalog = ContentRepository.LoadDefault();
            Profile = LocalProfile.LoadDefault();
            Progress = ProgressStore.LoadDefault();
            Outbox = OutboxStore.LoadDefault();

            Shell = gameObject.AddComponent<ShellView>();
            Shell.Initialize(Theme, Catalog, Profile, Progress, Outbox);

            if (!Profile.IsValid)
            {
                Shell.ShowNicknameDialog();
            }
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
