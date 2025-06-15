namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class DeactivateSelfOnFinishFreeze : UpdateAbstract
    {
        private Entity _entity;
        void Start()
        {
            _entity = GetComponent<Entity>();
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (_entity.stateVars.freezeTime <= 0)
                _entity.SetEntityActive(false);
        }
    }
}
