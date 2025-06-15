namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TightStuff.Aesthetics;
    
    public class FracturedTransienceReadyGlow : MonoBehaviour
    {
        [SerializeField]
        private ScrollHue _yinColor;
        [SerializeField]
        private ScrollHue _flashColor;
    
        [SerializeField]
        private Color _scrollStartColor;
    
        [SerializeField]
        private Color _ogyinColor;
        [SerializeField]
        private float _ogFlashAlpha;
    
        [SerializeField]
        private List<Spinner> _spinner;
        [SerializeField]
        private List<SpriteRenderer> _spinnerSprites;
        private List<float> _ogSpeeds;
    
        private float _circleAlpha;
    
        [SerializeField]
        private SpriteRenderer _yinSprite;
    
        private float _spinMultiplier;
    
        // Start is called before the first frame update
        void Start()
        {
            _ogFlashAlpha = _flashColor.alpha;
            _ogyinColor = _yinSprite.color;
            _ogSpeeds = new List<float>();
            foreach (var spinner in _spinner)
                _ogSpeeds.Add(spinner.SpinSpd);
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            if (PlayerManager.instance.currentMode == 2)
            {
                _yinColor.enabled = true;
                if (_yinSprite.color == _ogyinColor)
                    _yinColor.currentColor = _scrollStartColor;
                _flashColor.alpha = Mathf.Lerp(_flashColor.alpha, _ogFlashAlpha, 0.2f);
                for (int i = 0; i < _spinner.Count; i++)
                {
                    Spinner spinner = _spinner[i];
                    var c = _spinnerSprites[i].color;
                    c.a = Mathf.Lerp(c.a, 1, 0.1f);
                    _spinnerSprites[i].color = c;
                    spinner.SpinSpd = Mathf.Lerp(spinner.SpinSpd, _ogSpeeds[i] * 10, 0.01f);
                }
            }
            else if (PlayerManager.instance.currentMode == 1)
            {
                _yinColor.enabled = false;
                _yinSprite.color = Color.Lerp(_yinSprite.color, _ogyinColor, 0.5f);
                _yinColor.currentColor = _ogyinColor;
                _flashColor.alpha = Mathf.Lerp(_flashColor.alpha, 0, 0.05f);
                for (int i = 0; i < _spinner.Count; i++)
                {
                    Spinner spinner = _spinner[i];
                    var c = _spinnerSprites[i].color;
                    c.a = Mathf.Lerp(c.a, 0.6f, 0.1f);
                    _spinnerSprites[i].color = c;
                    spinner.SpinSpd = Mathf.Lerp(spinner.SpinSpd, _ogSpeeds[i], 0.05f);
                }
            }
            if (PlayerManager.instance.currentMode == 0)
            {
                var trans = _ogyinColor;
                trans.a = 0;
                _yinColor.enabled = false;
                _yinSprite.color = Color.Lerp(_yinSprite.color, trans, 0.5f);
                _yinColor.currentColor = _ogyinColor;
                _flashColor.alpha = Mathf.Lerp(_flashColor.alpha, 0, 0.05f);
                for (int i = 0; i < _spinner.Count; i++)
                {
                    Spinner spinner = _spinner[i];
                    var c = _spinnerSprites[i].color;
                    c.a = Mathf.Lerp(c.a, 0, 0.1f);
                    _spinnerSprites[i].color = c;
                    spinner.SpinSpd = Mathf.Lerp(spinner.SpinSpd, _ogSpeeds[i], 0.05f);
                }
            }
        }
    }
}
