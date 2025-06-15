namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class LerpToColor : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private SpriteRenderer _sprite;
        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Color _targetColor;
        [SerializeField]
        public float _lerpSpeed;
    
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            var color = GetColor();
    
            color = Color.Lerp(color, _targetColor, _lerpSpeed);
    
            SetCurrentColor(color);
        }
        public void SetCurrentColor(Color color)
        {
            if (_sprite != null)
                _sprite.color = color;
            else if (_image != null)
                _image.color = color;
            else if (_text != null)
                _text.color = color;
        }
        public void SetCurrentColorToWhite()
        {
            if (_sprite != null)
                _sprite.color = Color.white;
            else if (_image != null)
                _image.color = Color.white;
            else if (_text != null)
                _text.color = Color.white;
        }
        private Color GetColor()
        {
            var color = Color.black;
            if (_sprite != null)
                color = _sprite.color;
            else if (_image != null)
                color = _image.color;
            else if (_text != null)
                color = _text.color;
            return color;
        }
    
        public void SetTargetColor(Color color)
        {
            _targetColor = color;
        }
        public void SetTargetColorToWhite()
        {
            _targetColor = Color.white;
        }
        public void SetTargetColorToBlack()
        {
            _targetColor = Color.black;
        }
        public void SetCurrentColorAlpha(float alpha)
        {
            var color = GetColor();
    
            color.a = alpha;
    
            SetCurrentColor(color);
        }
    
    }
}
