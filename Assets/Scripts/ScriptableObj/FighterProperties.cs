namespace TightStuff
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "New Fighter", menuName = "Scriptable Object/Fighter")]
    public class FighterProperties : EntityProperties
    {
        [Header("Fighter Grounded Properties")]
        [SerializeField]
        private float _jumpHeight = 13.75f;
        [SerializeField]
        private float _decelerate = 6;
        [SerializeField]
        private float _jumpTime = 0.06f;
        [SerializeField]
        private float _shorthopTime = 0.02f;
    
        [SerializeField]
        private float _rollSpd = 6f;
    
    
        [Header("Fighter Airboene Properties")]
        [SerializeField]
        private float _airdashTime = 0.2f;
        [SerializeField]
        private float _airdashSpd = 8f;
    
    
        [SerializeField]
        private float _upAcl = 0.2f;
        [SerializeField]
        private float _downAcl = 0.2f;
    
        [SerializeField]
        private float _focusAcl = 0.5f;
        [SerializeField]
        private float _focusSpd = 2.5f;
        [SerializeField]
        private float _focusTime = 1f;
    
        [SerializeField]
        private float _focusRes = 0.035f;
    
        [Header(" ")]
        [SerializeField]
        private bool _walljumpEnable = false;
    
    
        public List<ColorPalettes> palette;
    
        [Header("UI")]
    
        public Sprite nameJap;
        public Sprite nameJapBG;
        public GameObject fighterExtraUI;
    
        public Vector2 CharSelectPortraitOffset;

        public GameObject fighterPrefab;
    
        public float jumpHeight { get { return _jumpHeight; } }
        public float decelerate { get { return _decelerate; } }
        public float jumpTime { get { return _jumpTime; } }
        public float shorthopTime { get { return _shorthopTime; } }
    
        public float rollSpd { get { return _rollSpd; } }
    
        public float airdashTime { get { return _airdashTime; } }
        public float airdashSpd { get { return _airdashSpd; } }
    
        public float upAcl { get { return _upAcl; } }
        public float downAcl { get { return _downAcl; } }
    
        public float focusAcl { get { return _focusAcl; } }
        public float focusSpd { get { return _focusSpd; } }
        public float focusTime { get { return _focusTime; } }
    
        public bool walljumpEnable { get { return _walljumpEnable; } }
    
        public float focusRes { get { return _focusRes; } }
    
        [System.Serializable]
        public class ColorPalettes
        {
            public Material colorMat;
            public Sprite portrait;
            public Sprite portraitFull;
            public GameObject stockIcon;
            public int defaultPlayerColorID;
            public int backupPlayerColorID;
        }
    }
}
