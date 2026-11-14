using UnityEngine;
using UnityEngine.UI;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Membatasi tinggi kartu dialog mengikuti isinya. Pada font sistem 2.0x
    /// isi dialog tidak muat dalam satu layar; kartu tidak boleh lebih tinggi
    /// dari viewport karena tombol bawah akan keluar layar. Isi yang lebih
    /// panjang tetap dapat dicapai lewat ScrollRect di kartu.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    internal sealed class ModalCardFitter : MonoBehaviour
    {
        private RectTransform _card;
        private RectTransform _content;
        private float _maxHeight;
        private float _applied = -1f;

        public void Configure(
            RectTransform card,
            RectTransform content,
            float maxHeight)
        {
            _card = card;
            _content = content;
            _maxHeight = Mathf.Max(0f, maxHeight);
            Apply();
        }

        private void LateUpdate()
        {
            Apply();
        }

        private void Apply()
        {
            if (_card == null || _content == null)
            {
                return;
            }

            var preferred = LayoutUtility.GetPreferredHeight(_content);
            if (preferred <= 0f)
            {
                preferred = _content.rect.height;
            }

            var target = Mathf.Clamp(preferred, 0f, _maxHeight);
            if (Mathf.Abs(target - _applied) < 0.5f)
            {
                return;
            }

            _applied = target;
            _card.sizeDelta = new Vector2(_card.sizeDelta.x, target);
        }
    }
}
