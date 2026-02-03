using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class TabsGroup : MonoBehaviour
    {
        [SerializeField] private Toggle _all;
        [SerializeField] private Toggle _odd;
        [SerializeField] private Toggle _even;
        [SerializeField] private RectTransform _slider;
        
        private Action _onFilterSet;
        private ContentAcquirer.Filter _filter;

        public ContentAcquirer.Filter Filter => _filter;

        public void Initialize(Action onFilterSet)
        {
            _all.onValueChanged?.Invoke(_all.isOn);
            _odd.onValueChanged?.Invoke(_odd.isOn);
            _even.onValueChanged?.Invoke(_even.isOn);
            _onFilterSet = onFilterSet;
        }

        public void SetFilter(ContentAcquirer.Filter filter, float sliderX)
        {
            _filter = filter;
            SetSlider(sliderX);
            _onFilterSet?.Invoke();
        }

        private void SetSlider(float x)
        {
            var position = _slider.position;
            position.x = x;
            _slider.position = position;
        }
    }
}