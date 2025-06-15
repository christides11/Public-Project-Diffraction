namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class InterpBackToColor : MonoBehaviour
    {
        [SerializeField]
        private float _lerpSpd = 1;
    
        private SpriteRenderer _sprite;
        private Color _initColor;
    
        // Start is called before the first frame update
        void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _initColor = _sprite.color;
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            _sprite.color = Color.Lerp(_sprite.color, _initColor, _lerpSpd * Time.deltaTime);
        }
    }
}
