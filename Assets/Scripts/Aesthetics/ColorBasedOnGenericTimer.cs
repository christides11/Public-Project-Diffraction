namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;
    
    public class ColorBasedOnGenericTimer : UpdateAbstract
    {
        [SerializeField]
        private Entity _timer;
    
        [SerializeField]
        private float _fadeStartTime;
        [SerializeField]
        private float _fadeInEndTime;
    
        [SerializeField]
        private Color _targetColor;
        private Color _initColor;
    
        private SpriteRenderer _spriteRenderer;
    
        // Start is called before the first frame update
        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
    
            if (_spriteRenderer != null)
                _initColor = _spriteRenderer.color;
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            base.GUpdate();
            float fadeInProgress = _fadeInEndTime != 0 ? (_timer.stateVars.genericTimer / _fadeInEndTime) : 1;
            fadeInProgress = Mathf.Clamp(fadeInProgress, 0, 1);
            float fadeOutProgress = (_timer.stateVars.genericTimer - _fadeStartTime) / 1;
            fadeOutProgress = fadeInProgress - Mathf.Clamp(fadeOutProgress, 0, 1);
            if (!_timer.stateVars.enabled)
                fadeOutProgress = 0;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _initColor * fadeOutProgress + _targetColor * (1 - fadeOutProgress);
            }
        }
    }
}
