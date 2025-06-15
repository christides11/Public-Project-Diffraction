namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SoundAdjustToHealth : MonoBehaviour
    {
        [SerializeField]
        private Entity _entity;
        private AudioSource _audio;
    
        [SerializeField]
        private float _noiseMaxHealth = 50;
        [SerializeField]
        private float _minPitch = 1;
        [SerializeField]
        private float _lerpSpeed = 5f;
        [SerializeField]
        private float _minVolume = 1;
    
        private float _desiredHealth;
    
        // Start is called before the first frame update
        void Start()
        {
            _audio = GetComponent<AudioSource>();
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            _desiredHealth = Mathf.Lerp(_desiredHealth, _entity.stateVars.percent, Time.deltaTime * _lerpSpeed);
            _audio.pitch = _minPitch + (3 - _minPitch) * Mathf.Clamp(_desiredHealth, 0, _noiseMaxHealth) / (_noiseMaxHealth);
            _audio.volume = _minVolume + Mathf.Clamp(_desiredHealth, 0, _noiseMaxHealth) / (_noiseMaxHealth) * _minVolume;
        }
    }
}
