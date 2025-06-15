namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;
    
    public class ColorBasedOnProjectileLife : UpdateAbstract
    {
        [SerializeField]
        private BaseProjectileBehaviour _proj;
    
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
            float fadeInProgress = _fadeInEndTime != 0 ? (_proj.LifeTime / _fadeInEndTime) : 1;
            fadeInProgress = Mathf.Clamp(fadeInProgress, 0, 1);
            float fadeOutProgress = (_proj.LifeTime - _fadeStartTime) / (_proj.MaxLifeTime - _fadeStartTime);
            fadeOutProgress = fadeInProgress - Mathf.Clamp(fadeOutProgress, 0, 1);
            if (!_proj.entity.enabled)
                fadeOutProgress = 0;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _initColor * fadeOutProgress + _targetColor * (1 - fadeOutProgress);
            }
        }
    }
}
