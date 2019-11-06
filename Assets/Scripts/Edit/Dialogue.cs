using UnityEngine;

namespace roguelike{
    
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    [System.Serializable]
    public class Dialogue: ScriptableObject
    {
        public new string name;

        [TextArea(3, 10)]
        public string[] sentences;
    }
}