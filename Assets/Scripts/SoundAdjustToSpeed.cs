namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SoundAdjustToSpeed : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rb;
        private AudioSource _audio;
    
        [SerializeField]
        private float _noiseStartingSpeed = 5;
        [SerializeField]
        private float _noiseMaxSpeed = 30;
        [SerializeField]
        private float _minPitch = 0.75f;
        [SerializeField]
        private float _lerpSpeed = 5f;
    
        [SerializeField]
        private float _volumeMultiplier = 1;
    
        private float _desiredSpeed;
    
        // Start is called before the first frame update
        void Start()
        {
            _audio = GetComponent<AudioSource>();
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            _desiredSpeed = Mathf.Lerp(_desiredSpeed, _rb.velocity.magnitude, Time.deltaTime * _lerpSpeed);
            _audio.pitch = _minPitch + (3 - _minPitch) * Mathf.Clamp(_desiredSpeed - _noiseStartingSpeed, 0, _noiseMaxSpeed - _noiseStartingSpeed) / (_noiseMaxSpeed - _noiseStartingSpeed);
            _audio.volume = Mathf.Clamp(_desiredSpeed - _noiseStartingSpeed, 0, _noiseMaxSpeed - _noiseStartingSpeed) / (_noiseMaxSpeed - _noiseStartingSpeed) * _volumeMultiplier;
        }
    }
}
