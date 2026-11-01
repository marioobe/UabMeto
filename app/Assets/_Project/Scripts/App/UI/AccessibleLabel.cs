using UnityEngine;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Metadata label aksesibilitas untuk F1. Ini belum merupakan integrasi
    /// API aksesibilitas Android; pengujian dan integrasi penuh belum selesai.
    /// </summary>
    public sealed class AccessibleLabel : MonoBehaviour
    {
        [SerializeField] private string label = string.Empty;

        public string Label
        {
            get => label;
            set => label = value ?? string.Empty;
        }
    }
}
