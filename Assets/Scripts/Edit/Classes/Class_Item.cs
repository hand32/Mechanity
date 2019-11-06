using UnityEngine;

namespace roguelike
{
    [System.Serializable]
    public class Item
    {
        public GameObject itemPrefab;
        [Range(0, 100)]
        public float chance;
    }
}