namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class GrazeParticle : UpdateAbstract
    {
        [SerializeField]
        private Entity _entity;
        private Transform _homeTarget;
    
        [SerializeField]
        private ParticleSystem ps;
        [SerializeField]
        private TrailRenderer tr;
        [SerializeField]
        private AudioSource destroyaudio;
        [SerializeField]
        private AudioSource receiveaudio;
    
        private void Start()
        {
            _entity = GetComponent<Entity>();
            destroyaudio.transform.parent = null;
            receiveaudio.transform.parent = null;
        }
    
        public override void LateGUpdate()
        {
            base.LateGUpdate();
            tr.emitting = _entity.stateVars.trueRbSpd.magnitude > 2 || _homeTarget != null;
            if (_homeTarget == null)
                return;
            _entity.transform.position = Vector2.Lerp(_entity.transform.position, _homeTarget.position, _entity.et.airAcl * _entity.TrueTimeScale);
            if (Vector3.Distance(_entity.transform.position, _homeTarget.position) < 0.5f)
            {
                _homeTarget = null;
                _entity.SetEntityActive(false);
            }
        }
        public void SetHomeTarget(GrazePoint gz)
        {
            destroyaudio.transform.position = transform.position;
            receiveaudio.transform.position = transform.position;
            _homeTarget = gz.fighter.transform;
            receiveaudio.Play();
        }
        public void DestroyParticle()
        {
            if (!_entity.enabled)
                return;
            destroyaudio.transform.position = transform.position;
            receiveaudio.transform.position = transform.position;
            _entity.SetEntityActive(false);
            ps.Play();
            destroyaudio.Play();
        }
        public void ResetHomeTarget()
        {
            _homeTarget = null;
        }
    }
}
