using Data;
using Game.UI;
using UnityEngine;

namespace Game
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private Content _contentData;
        [SerializeField] private DynamicScroll _scroll;
        [SerializeField] private TabsGroup _tabsGroup;

        private ContentAcquirer _contentAcquirer;

        private void Awake()
        {
            _tabsGroup.Initialize(SetFilter);
            _contentAcquirer = new ContentAcquirer(_contentData, _tabsGroup.Filter);
            _scroll.Initialize(_contentAcquirer.SetContent, _contentAcquirer.GetFilteredContentLength());
        }

        private void SetFilter()
        {
            _contentAcquirer.SetFilter(_tabsGroup.Filter);
            _scroll.Initialize(_contentAcquirer.SetContent, _contentAcquirer.GetFilteredContentLength());
        }
    }
}