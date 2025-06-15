namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;
    
    public class LightColorBasedOnGenericTimer : UpdateAbstract
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
    
        private Light2D _light;
    
        // Start is called before the first frame update
        void Start()
        {
            _light = GetComponent<Light2D>();
    
            if (_light != null)
                _initColor = _light.color;
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
            if (_light != null)
                _light.color = _initColor * fadeOutProgress + _targetColor * (1 - fadeOutProgress);
        }
    }
}
