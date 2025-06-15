namespace TightStuff.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    
    public class TranslucentOnTriggerEnter : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TextMeshProUGUI _text;
    
        private float _currentAlpha = 1;
        public float initAlpha = 1;
        private float _desiredAlpha = 1;
        [SerializeField]
        private float _lowAlpha = 0.2f;
        [SerializeField]
        private float _interpSpd = 4;
    
        private float _speed = 0;
    
        private void Start()
        {
            if (_image != null)
                initAlpha = _image.color.a;
            if (_text != null)
                initAlpha = _text.color.a;
            GetComponent<RectTransform>().SetParent(null);
        }
    
        private void OnTriggerStay2D(Collider2D collision)
        {
            _desiredAlpha = _lowAlpha;
        }
    
        private void OnTriggerExit2D(Collider2D collision)
        {
            _desiredAlpha = 1;
        }
    
        private void LateUpdate()
        {
            if (_image != null)
            {
                var color = _image.color;
                color.a = _currentAlpha * initAlpha;
                _image.color = color;
            }
            if (_text != null)
            {
                var color = _text.color;
                color.a = _currentAlpha * initAlpha;
                _text.color = color;
            }
            _currentAlpha = Mathf.SmoothDamp(_currentAlpha, _desiredAlpha, ref _speed, _interpSpd);
        }
    }
}
