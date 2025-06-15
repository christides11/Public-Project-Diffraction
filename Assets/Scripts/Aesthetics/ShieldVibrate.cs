namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ShieldVibrate : UpdateAbstract
    {
        [SerializeField]
        private ShieldBox _shield;
        [SerializeField]
        private float _offset;
        [SerializeField]
        private float _shakeAmplitudeMultiplier;
        [SerializeField]
        private float _shakeFrequency = 150;
    
        private Vector2 _initialPos;
    
        private float _shakeTimer;
        private float _shakeAmplitude;
    
        // Start is called before the first frame update
        void Start()
        {
            _initialPos = transform.localPosition;
            _shield.OnHitted.AddListener(ShakeTransform);
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            Vector2 displaceX = Mathf.Sin((Time.time + _offset) * _shakeFrequency * 1.25f) * _shakeTimer * Mathf.Clamp(_shakeAmplitude + _shield.entity.stateVars.percent / 75, 0, 40) * Vector2.right;
            Vector2 displaceY = Mathf.Cos((Time.time + _offset) * _shakeFrequency) * _shakeTimer * Mathf.Clamp(_shakeAmplitude + _shield.entity.stateVars.percent / 75, 0, 50) * Vector2.up;
            transform.localPosition = displaceX + displaceY + _initialPos;
            if (_shakeTimer > 0.05f)
                _shakeTimer -= Time.fixedDeltaTime;
            else
            {
                _shakeAmplitude = 0;
                if (_shield.owner.stateVars.freezeTime <= 0)
                    _shakeTimer = 0.05f;
            }
        }
    
        public void ShakeTransform(HitObject hit)
        {
            if (hit.box.owner.TryGetComponent(out BaseFighterBehavior fighter))
                if (fighter.ActionState.GetState() is GroundedFighterPerfectShieldState || fighter.ActionState.GetState() is GroundedFighterPerfectShieldEndState)
                {
                    _shakeTimer = 0;
                    return;
                }
            _shakeAmplitude = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, 20) * _shakeAmplitudeMultiplier;
            _shakeTimer = Mathf.Clamp(hit.hitbox.hitProperties.damage / 30f, 0.1f, 0.2f);
        }
    }
}
