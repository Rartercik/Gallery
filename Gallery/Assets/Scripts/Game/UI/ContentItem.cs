using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ContentItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] public Canvas _showWindow;
        [SerializeField] private Canvas _premiumWindow;
        [SerializeField] private FullContent _fullContent;
        [SerializeField] private Image _main;
        [SerializeField] private Image _mask;
        [SerializeField] private Image _content;
        [SerializeField] private Image _premiumFlag;

        private bool _free;

        public RectTransform RectTransform => _rectTransform;

        public void SetVisible(bool visible)
        {
            var color = visible ? Color.white : new Color(1f, 1f, 1f, 0f);
            _main.color = color;
            _mask.color = color;
            _content.color = color;
            _premiumFlag.color = color;
        }

        public void SetImage(Sprite sprite, bool free)
        {
            _content.sprite = sprite;
            _free = free;
            _premiumFlag.enabled = !free;
        }

        public void Show()
        {
            if (_free)
            {
                ShowContent();
                return;
            }
            ShowPremium();
        }

        private void ShowContent()
        {
            _showWindow.enabled = true;
            _fullContent.SetContent(_content.sprite);
        }

        private void ShowPremium()
        {
            _premiumWindow.enabled = true;
        }
    }
}