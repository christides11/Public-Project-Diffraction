namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    
    public class Flash : UpdateAbstract
    {
        private SpriteRenderer _sprite;
    
        [SerializeField]
        private float _timeVisible;
        [SerializeField]
        private float _timeGone;
    
        [SerializeField]
        private float _fadeInTime;
        [SerializeField]
        private float _fadeOutTime;
    
        private float _timer;
    
        [SerializeField]
        private UnityEvent _AppearEvent;
        [SerializeField]
        private UnityEvent _DisappearEvent;
    
    
        // Start is called before the first frame update
        void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            base.GUpdate();
            if (!enabled)
                return;
            Color currentColor = _sprite.color;
    
            if (_timer > _fadeOutTime + _timeGone + _timeVisible && _fadeInTime != 0)
            {
                var time = _timer - _fadeOutTime - _timeGone - _timeVisible;
                currentColor.a = time / _fadeInTime;
            }
            else if (_timer > _fadeOutTime + _timeVisible && _timeGone != 0)
            {
                if (currentColor.a != 0)
                    _DisappearEvent.Invoke();
                currentColor.a = 0;
            }
            else if (_timer > _timeVisible && _fadeOutTime != 0)
            {
                var time = _timer - _timeVisible;
                currentColor.a = 1 - time / _fadeOutTime;
            }
            else
            {
                if (currentColor.a != 1)
                    _AppearEvent.Invoke();
                currentColor.a = 1;
            }
    
            _sprite.color = currentColor;
            _timer += Time.fixedDeltaTime * MatchManager.worldTime;
    
            if (_timer > _fadeOutTime + _timeGone + _timeVisible + _fadeInTime)
                _timer = 0;
        }
    
        private void OnEnable()
        {
            _timer = 0;
            _AppearEvent.Invoke();
        }
    }
}
