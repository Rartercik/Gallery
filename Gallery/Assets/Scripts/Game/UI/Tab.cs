using UnityEngine;

namespace Game.UI
{
    public class Tab : MonoBehaviour
    {
        [SerializeField] private RectTransform _selectedObject;
        [SerializeField] private RectTransform _diselectedObject;
        [SerializeField] private RectTransform _floor;
        [SerializeField] private TabsGroup _tabsGroup;
        [SerializeField] private ContentAcquirer.Filter _filter;

        public void Select(bool select)
        {
            _selectedObject.gameObject.SetActive(select);
            _diselectedObject.gameObject.SetActive(!select);

            if (select)
            {
                _tabsGroup.SetFilter(_filter, _selectedObject.position.x);
            }
        }
    }
}
