namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;
    
    public class OpacityBasedOnProjectileLife : UpdateAbstract
    {
        [SerializeField]
        private BaseProjectileBehaviour _proj;
    
        [SerializeField]
        private float _fadeStartTime;
        [SerializeField]
        private float _fadeInEndTime;
    
        private float _initAlpha;
        private float _initIntensity;
    
        private SpriteRenderer _spriteRenderer;
        private Light2D _light;
    
        // Start is called before the first frame update
        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _light = GetComponent<Light2D>();
    
            if (_spriteRenderer != null)
                _initAlpha = _spriteRenderer.color.a;
            if (_light != null)
                _initIntensity = _light.intensity;
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            base.GUpdate();
            float fadeInProgress = _fadeInEndTime != 0 ? (_proj.LifeTime / _fadeInEndTime) : 1;
            fadeInProgress = Mathf.Clamp(fadeInProgress, 0, 1);
            float fadeOutProgress = (_proj.LifeTime - _fadeStartTime) / (_proj.MaxLifeTime - _fadeStartTime);
            fadeOutProgress = fadeInProgress - Mathf.Clamp(fadeOutProgress, 0, 1);
            if (!_proj.entity.enabled)
                fadeOutProgress = 0;
            if (_spriteRenderer != null)
            {
                var currentColor = _spriteRenderer.color;
                currentColor.a = _initAlpha * fadeOutProgress;
                _spriteRenderer.color = currentColor;
            }
            if (_light != null)
                _light.intensity = _initIntensity * fadeOutProgress;
        }
    }
}
