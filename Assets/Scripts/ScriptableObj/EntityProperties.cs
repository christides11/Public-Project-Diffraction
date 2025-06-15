namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "New Entity", menuName = "Scriptable Object/Entity")]
    public class EntityProperties : ScriptableObject
    {
        [Header("Entity Grounded Properties")]
        [SerializeField]
        private float _groundRes = 0.1f;
    
        [SerializeField]
        private float _runSpd = 4.5f;
        [SerializeField]
        private float _runAcl = 6;
        [SerializeField]
        private float _dashSpd = 8;
    
        [Header("Entity Airboene Properties")]
        [SerializeField]
        private float _airAcl = 0.375f;
        [SerializeField]
        private float _airSpd = 5;
    
        [SerializeField]
        private float _fallSpd = 0.4f;
        [SerializeField]
        private float _termVelo = 6.8f;
        [SerializeField]
        private float _airRes = 0.03f;
    
    
        [Header("Entity Misc Properties")]
        [SerializeField]
        private float _mass = 1f;
        [SerializeField]
        private float _health = 10000000f;
        [SerializeField]
        private float _launchRes = 0.04f;
    
        public float groundRes { get { return _groundRes; } }
    
        public float runSpd { get { return _runSpd; } }
        public float runAcl { get { return _runAcl; } }
        public float dashSpd { get { return _dashSpd; } }
    
        public float airAcl { get { return _airAcl; } }
        public float airSpd { get { return _airSpd; } }
    
        public float fallSpd { get { return _fallSpd; } }
        public float termVelo { get { return _termVelo; } }
        public float airRes { get { return _airRes; } }
    
        public float mass { get { return _mass; } }
        public float health { get { return _health; } }
        public float launchRes { get { return _launchRes; } }
    }
}
