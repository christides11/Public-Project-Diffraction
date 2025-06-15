namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ActivateHitboxSpeedBased : UpdateAbstract
    {
        [SerializeField]
        private float _speedThreshold;
        [SerializeField]
        private Entity _entity;
    
        private Collider2D[] _colliders;
    
        // Start is called before the first frame update
        void Start()
        {
            _colliders = GetComponents<Collider2D>();
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            foreach (var collider in _colliders)
            {
                collider.enabled = _entity.stateVars.indieSpd.magnitude > _speedThreshold && _entity.enabled;
            }
        }
    }
}
