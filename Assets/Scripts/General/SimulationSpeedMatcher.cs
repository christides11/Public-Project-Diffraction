namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SimulationSpeedMatcher : UpdateAbstract
    {
        private ParticleSystem _particleSystem;
        private Animator _animator;
    
        [SerializeField]
        private Entity _entity;
    
        // Start is called before the first frame update
        void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _animator = GetComponent<Animator>();
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (_particleSystem != null)
            {
                var main = _particleSystem.main;
                main.simulationSpeed = MatchManager.worldTime;
                if (_entity != null)
                    main.simulationSpeed = _entity.TrueTimeScale;
                if (MatchManager.paused)
                    main.simulationSpeed = 0;
            }
            if (_animator != null)
            {
                _animator.speed = MatchManager.worldTime;
                if (_entity != null)
                    _animator.speed = _entity.TrueTimeScale;
                if (MatchManager.paused)
                    _animator.speed = 0;
            }
        }
    
        public void SetEntity(Entity entity)
        {
            _entity = entity;
        }
    }
}
