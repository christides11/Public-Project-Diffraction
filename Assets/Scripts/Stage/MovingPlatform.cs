namespace TightStuff.Stage
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class MovingPlatform : Oscilator
    {
        [SerializeField]
        private Entity _entity;
    
        private void Start()
        {
            _entity = GetComponent<Entity>();
        }
    
        public override void GUpdate()
        {
            if (_entity == null)
                return;
    
            _timePassedX = _entity.stateVars.selfSpd.x;
            _timePassedY = _entity.stateVars.selfSpd.y;
    
            var prevX = transform.position.x;
            var prevY = transform.position.y;
    
            Oscilate();
    
            _entity.stateVars.selfSpd = new Vector2(_timePassedX, _timePassedY);
            _entity.stateVars.indieSpd = new Vector2(transform.position.x - prevX, transform.position.y - prevY) * (1 / (Time.fixedDeltaTime));
        }
    }
}
