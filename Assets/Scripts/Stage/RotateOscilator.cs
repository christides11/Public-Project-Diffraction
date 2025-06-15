namespace TightStuff.Stage
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.U2D.Animation;
    
    public class RotateOscilator : UpdateAbstract, IBlowable
    {
        private static bool animated = true;
    
        private Quaternion _initRot;
        private float _timePassed;
    
        [SerializeField]
        private float _oscilateAmplitude;
        [SerializeField]
        private float _oscilateRate;
        [SerializeField]
        private float _offset;
    
        private float _oscilateAmplitudeExtra;
        private float _oscilateRateExtra;
    
        [SerializeField]
        private UnityEvent _BlowEvent;
    
        [SerializeField]
        private UnityEvent _AmplitudeReachEvent;
        [SerializeField]
        private float _AmplitudeEventThreshold;
    
        void Start()
        {
            if (!animated)
                if (transform.parent != null)
                    if (transform.parent.TryGetComponent(out SpriteSkin ss))
                        ss.enabled = false;
    
            _initRot = transform.localRotation;
            _timePassed += _offset;
        }
    
        public override void GUpdate()
        {
            if (MatchManager.paused)
                return;
            var zRot = Oscilate(_oscilateAmplitude + _oscilateAmplitudeExtra, _oscilateRate + _oscilateRateExtra, ref _timePassed);
            if (Mathf.Abs(transform.eulerAngles.z - _initRot.eulerAngles.z) < _AmplitudeEventThreshold && Mathf.Abs(zRot) >= _AmplitudeEventThreshold)
                _AmplitudeReachEvent.Invoke();
            transform.localRotation = Quaternion.Euler(0, 0, _initRot.eulerAngles.z + zRot);
        }
    
        private float Oscilate(float oscilateAmplitude, float oscilateRate, ref float timePassed)
        {
            timePassed += Time.fixedDeltaTime * oscilateRate * MatchManager.worldTime;
    
            if (timePassed > Mathf.PI * 2)
                timePassed = 0;
            return Mathf.Sin(timePassed) * oscilateAmplitude;
        }
    
        public void Blown(float strength, float extraRate)
        {
            if (strength <= -90)
            {
                _oscilateAmplitudeExtra = 0;
    
                _oscilateRateExtra = 0;
                return;
            }
            if (strength > 0)
                _BlowEvent.Invoke();
            _oscilateAmplitudeExtra += strength;
    
            _oscilateRateExtra += extraRate;
        }
    }
}
