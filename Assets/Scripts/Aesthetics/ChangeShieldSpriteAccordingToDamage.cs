namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ChangeShieldSpriteAccordingToDamage : MonoBehaviour
    {
        [SerializeField]
        private Entity _shield;
    
        [SerializeField]
        private ParticleSystem _crackParticle;
    
        [SerializeField]
        private SpriteRenderer _shieldSprite;
    
        [SerializeField]
        private Sprite _undamaged;
        [SerializeField]
        private Sprite _slightDamaged;
        [SerializeField]
        private Sprite _moderateDamaged;
        [SerializeField]
        private Sprite _veryDamaged;
    
        [SerializeField]
        private AudioSource _audio;
    
        // Update is called once per frame
        void Update()
        {
            if (_shield.stateVars.percent > 37.5f)
            {
                if (_shieldSprite.sprite != _veryDamaged && _shieldSprite.enabled)
                {
                    _crackParticle.Play();
                    _audio.Play();
                }
                _shieldSprite.sprite = _veryDamaged;
            }
            else if (_shield.stateVars.percent > 25f)
            {
                if (_shieldSprite.sprite != _moderateDamaged && _shieldSprite.enabled)
                {
                    _crackParticle.Play();
                    _audio.Play();
                }
                _shieldSprite.sprite = _moderateDamaged;
            }
            else if (_shield.stateVars.percent > 12.5f)
            {
                if (_shieldSprite.sprite != _slightDamaged && _shieldSprite.enabled)
                {
                    _crackParticle.Play();
                    _audio.Play();
                }
                _shieldSprite.sprite = _slightDamaged;
            }
            else
                _shieldSprite.sprite = _undamaged;
        }
    }
}
