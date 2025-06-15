namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ReadyFlash : MonoBehaviour
    {
        [SerializeField]
        private Color _colorPositive;
        [SerializeField]
        private Color _colorNegative;
    
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _lerpSpeed = 5;
    
        private float _alpha;
    
        private Color _desiredColor;
        private Color _initColor;
        [SerializeField]
        private Image _image;
    
        // Start is called before the first frame update
        void Start()
        {
            _image = GetComponent<Image>();
            _initColor = _image.color;
        }
    
        // Update is called once per frame
        void Update()
        {
            var timeSined = Mathf.Sin(Time.time * _speed);
            var timeSinedAbs = Mathf.Abs(timeSined);
    
            if (timeSined > 0)
                _desiredColor = timeSinedAbs * _colorPositive + (1 - timeSinedAbs) * _initColor;
            else
                _desiredColor = timeSinedAbs * _colorNegative + (1 - timeSinedAbs) * _initColor;
            _desiredColor.a = _alpha;
            _image.color = Color.Lerp(_image.color, _desiredColor, Time.deltaTime * _lerpSpeed);
        }
    
        public void SetAlpha(float i)
        {
            _alpha = i;
        }
        public void SetLerpSpeed(float i)
        {
            _lerpSpeed = i;
        }
        public void SetAlphaInstantly(float i)
        {
            _alpha = i;
            var col = Color.white;
            col.a = i;
            _image.color = col;
        }
        public void SetBothFlashColors(Color color)
        {
            _colorPositive = color;
            _colorNegative = color;
        }
    }
}
