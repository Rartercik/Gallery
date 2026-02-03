using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class FullContent : MonoBehaviour
    {
        [SerializeField] private Image _content;

        public void SetContent(Sprite sprite)
        {
            _content.sprite = sprite;
        }
    }
}