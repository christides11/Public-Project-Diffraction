namespace TightStuff
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "New Assist", menuName = "Scriptable Object/Assist")]
    public class AssistProperties : ScriptableObject
    {
        [Header("Fighter Grounded Properties")]
        [SerializeField]
        private int _coolDown = 30;
        [SerializeField]
        private Sprite _icon;
        [SerializeField]
        private Color _iconColor;
        [SerializeField]
        private Sprite _mascot;
        [SerializeField]
        private GameObject _assistPrefab;
    
        public int CoolDown => _coolDown;
        public Sprite Icon => _icon;
        public Color IconColor => _iconColor;
        public Sprite Mascot => _mascot;
        public GameObject AssistPrefab => _assistPrefab;
    }
}
