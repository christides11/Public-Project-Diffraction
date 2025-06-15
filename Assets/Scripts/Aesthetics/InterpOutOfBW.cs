namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;
    
    public class InterpOutOfBW : UpdateAbstract
    {
        [SerializeField]
        private SpriteRenderer _white;
    
        [SerializeField]
        private Light2D _light;
    
        [SerializeField]
        private float interpSpd = 0.25f;
    
        private float _timer;
    
        [SerializeField]
        private Color _bgColor;
        [SerializeField]
        private Color _darkenColor;
        [SerializeField]
        private float _initIntensity;
    
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (MatchManager.rollingBack > 0)
                return;
            if (_timer > 0)
            {
                _timer -= Time.fixedDeltaTime;
                return;
            }
    
            _white.color = Color.Lerp(_white.color, new Color(1, 1, 1, 0), interpSpd);
            _light.color = Color.Lerp(_light.color, new Color(1, 1, 1), interpSpd);
            _light.pointLightOuterRadius = Mathf.Lerp(_light.pointLightOuterRadius, 0, interpSpd * 5);
            _light.intensity = Mathf.Lerp(_light.intensity, 0, interpSpd);
        }
    
        public void StartBW()
        {
            _timer = 0.25f;
            _white.color = _bgColor;
            _light.color = _darkenColor;
            _light.pointLightOuterRadius = 100;
            _light.intensity = _initIntensity;
        }
    }
}
