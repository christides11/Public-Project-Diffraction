namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class CharacterSwitchFlash : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        [SerializeField]
        private Color _targetColor;
        [SerializeField]
        private Color _darkColor;
    
        [SerializeField]
        private float _flashSpeed = 0.1f;
        [SerializeField]
        private Vector2 _p3existencePos;
        private Vector2 _initPos;
    
        // Start is called before the first frame update
        void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _initPos = transform.position;
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            if (_sprite == null)
                return;
            var alpha = _sprite.color;
            alpha = Color.Lerp(alpha, _targetColor, _flashSpeed);
            _sprite.color = alpha;
        }
    
        public void Flash(float val)
        {
            var alpha = _sprite.color;
            alpha.a = val;
            _sprite.color = alpha;
        }
        public void P3Mode(bool val)
        {
            if (val)
                transform.position = _p3existencePos;
            else
                transform.position = _initPos;
        }
        public void SwitchToBlack()
        {
            var alpha = _darkColor;
            _targetColor = alpha;
            var c = Color.white;
            c.a = 0.5f;
            _sprite.color = c;
        }
        public void SwitchToWhite()
        {
            var alpha = Color.white;
            alpha.a = 0;
            _targetColor = alpha;
        }
        public void SwitchToFullWhite()
        {
            var alpha = Color.white;
            _targetColor = alpha;
        }
    }
}
