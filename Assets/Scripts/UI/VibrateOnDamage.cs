namespace TightStuff.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class VibrateOnDamage : MonoBehaviour
    {
        [SerializeField]
        private float _offset;
        [SerializeField]
        private float _shakeAmplitudeMultiplier;
        [SerializeField]
        private float _shakeFrequency = 150;
    
        private Vector2 _initialPos;
        private RectTransform rectTransform;
    
        private float _shakeTimer;
        private float _shakeAmplitude;
    
        // Start is called before the first frame update
        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            _initialPos = rectTransform.anchoredPosition;
        }
    
        // Update is called once per frame
        void Update()
        {
            Vector2 displaceX = Mathf.Sin((Time.time + _offset) * _shakeFrequency * 1.25f) * _shakeTimer * Mathf.Clamp(_shakeAmplitude, 0, 50) * Vector2.right;
            Vector2 displaceY = Mathf.Cos((Time.time + _offset) * _shakeFrequency) * _shakeTimer * Mathf.Clamp(_shakeAmplitude, 0, 50) * Vector2.up;
            rectTransform.anchoredPosition = displaceX + displaceY + _initialPos;
            if (_shakeTimer > 0)
                _shakeTimer -= Time.deltaTime;
            else
            {
                rectTransform.anchoredPosition = _initialPos;
            }
        }
    
        public void ShakeRect(float value)
        {
            _shakeAmplitude = Mathf.Clamp(value, 0, 30) * _shakeAmplitudeMultiplier;
            _shakeTimer = Mathf.Clamp(value / 25f, 0.1f, 0.3f);
        }
    }
}
