using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Content", menuName = "Data/Content", order = 1)]
    public class Content : ScriptableObject
    {
        [SerializeField] private Sprite[] _sprites;

        public int ContentLength => _sprites.Length;

        public bool CheckAvailability(int index)
        {
            return index >= 0 && index < _sprites.Length;
        }

        public Sprite GetSprite(int index)
        {
            if (CheckAvailability(index) == false) throw new ArgumentOutOfRangeException();

            return _sprites[index];
        }
    }
}
