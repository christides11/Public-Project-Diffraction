namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class LaunchAndHurtOwner : UpdateAbstract
    {
        [SerializeField]
        private Entity _entity;
        [SerializeField]
        private HitBox _hitBox;
        // Start is called before the first frame update
        void Start()
        {
            order = 3;
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (_entity.stateVars.indieSpd.magnitude < _entity.et.airSpd + 1)
            {
                _hitBox.hitProperties.hitOwner = false;
            }
        }
        public void SemiReflectBasedOnLaunchSpeed(HitObject hit)
        {
            if (hit.hitbox.owner == _hitBox.owner || _entity.stateVars.indieSpd.magnitude < _entity.et.airSpd + 1)
                return;
            _hitBox.hitProperties.hitOwner = true;
        }
        public void ResetHitOwner()
        {
            _hitBox.hitProperties.hitOwner = false;
        }
    }
}
