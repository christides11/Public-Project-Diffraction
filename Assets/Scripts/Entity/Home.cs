namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Home : UpdateAbstract
    {
        private Entity _entity;
        [SerializeField]
        private Grazer _grazer;
    
        [SerializeField]
        private float _slowHomeSpeed = 5;
        [SerializeField]
        private float _followAngleAgainst;
    
        // Start is called before the first frame update
        void Start()
        {
            order = 4000;
            _entity = GetComponent<Entity>();
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (!enabled)
                return;
            Transform hitobject = null;
            hitobject = DetectClosestTarget(hitobject);
            HomeToTarget(hitobject);
        }
    
        private void HomeToTarget(Transform hitobject)
        {
            if (hitobject == null)
                _entity.stateVars.indieSpd += _entity.et.airAcl * _entity.TrueTimeScale * _entity.stateVars.indieSpd.normalized;
            else
            {
                var dir = (Vector2)(hitobject.position - transform.position).normalized;
                var multiplier = 1 - Mathf.Clamp(Vector2.Dot(dir, _entity.stateVars.indieSpd.normalized), -Mathf.Clamp(_followAngleAgainst, -1, 0), 1);
                if (_entity.stateVars.indieSpd.magnitude < _slowHomeSpeed)
                    multiplier = 0;
                _entity.stateVars.indieSpd += _entity.et.airAcl * _entity.TrueTimeScale * (dir + multiplier * _entity.stateVars.indieSpd.normalized).normalized;
            }
        }
    
        private Transform DetectClosestTarget(Transform hitobject)
        {
            float closestDistance = 1000;
            foreach (HitObject hit in _grazer.hitObjects)
            {
                if (!hit.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
                    continue;
                var difference = hit.box.entity.transform.position - _entity.transform.position;
                float angleDifference = Vector2.Dot(difference.normalized, _entity.stateVars.indieSpd.normalized);
                if (difference.magnitude < closestDistance && (angleDifference > _followAngleAgainst || _entity.stateVars.indieSpd.magnitude < _slowHomeSpeed))
                {
                    hitobject = hit.box.entity.transform;
                    closestDistance = difference.magnitude;
                }
            }
    
            return hitobject;
        }
    }
}
