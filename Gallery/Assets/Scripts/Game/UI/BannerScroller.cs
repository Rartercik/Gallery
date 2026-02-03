using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;

namespace Game.UI
{
    public class BannerScroller : MonoBehaviour
    {
        [SerializeField] private SimpleScrollSnap _scrollSnap;
        [SerializeField] private Image[] _pages;
        [SerializeField] private float _interval;
        [SerializeField] private Color _defaultPageColor;
        [SerializeField] private Color _selectedPageColor;

        private int _lastPanel;
        private float _time;

        private void Update()
        {
            _time += Time.deltaTime / _interval;

            if (_time >= 1f)
            {
                _scrollSnap.GoToNextPanel();
                _time = 0f;
            }
            if (_scrollSnap.SelectedPanel != _lastPanel)
            {
                _time = 0f;
                _lastPanel = _scrollSnap.SelectedPanel;
                SetColor(_scrollSnap.SelectedPanel);
            }
        }

        public void SetColor(int index)
        {
            for (int i = 0; i < _pages.Length; i++)
            {
                if (i == index)
                {
                    _pages[i].color = _selectedPageColor;
                }
                else
                {
                    _pages[i].color = _defaultPageColor;
                }
            }
        }
    }
}