namespace TightStuff.Stage
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.U2D.Animation;
    
    public class Oscilator : UpdateAbstract, IBlowable
    {
        private static bool animated = true;
    
        private Vector2 _initPos;
        protected float _timePassedY;
        protected float _timePassedX;
    
        [SerializeField]
        private float _oscilateAmplitudeX;
        [SerializeField]
        private float _oscilateRateX;
        [SerializeField]
        private float _offsetX;
    
    
        [SerializeField]
        private float _oscilateAmplitudeY;
        [SerializeField]
        private float _oscilateRateY;
        [SerializeField]
        private float _offsetY;
    
        private float _oscilateRateExtraX;
        private float _oscilateAmplitudeExtraX;
        private float _oscilateRateExtraY;
        private float _oscilateAmplitudeExtraY;
    
        [SerializeField]
        private UnityEvent BlowEvent;
    
        // Start is called before the first frame update
        void Awake()
        {
            if (!animated)
                if (transform.parent != null)
                    if (transform.parent.TryGetComponent(out SpriteSkin ss))
                        ss.enabled = false;
    
            _initPos = transform.localPosition;
            _timePassedY += _offsetY;
            _timePassedX += _offsetX;
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (MatchManager.paused)
                return;
            Oscilate();
        }
    
        protected virtual void Oscilate()
        {
            var y = CalculatePosition(_oscilateAmplitudeY + _oscilateAmplitudeExtraY, _oscilateRateY + _oscilateRateExtraY, ref _timePassedY);
            var x = CalculatePosition(_oscilateAmplitudeX + _oscilateAmplitudeExtraX, _oscilateRateX + _oscilateRateExtraX, ref _timePassedX);
    
            transform.localPosition = new Vector2(x, y) + _initPos;
        }
    
        protected virtual float CalculatePosition(float oscilateAmplitude, float oscilateRate, ref float timePassed)
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
                _oscilateAmplitudeExtraX = 0;
                _oscilateAmplitudeExtraY = 0;
    
                _oscilateRateExtraX = 0;
                _oscilateRateExtraY = 0;
                return;
            }
            strength *= 0.0075f;
            if (strength > 0)
                BlowEvent.Invoke();
            _oscilateAmplitudeExtraX += strength;
            _oscilateAmplitudeExtraY += strength;
    
            _oscilateRateExtraX += extraRate;
            _oscilateRateExtraY += extraRate;
        }
    }
}
