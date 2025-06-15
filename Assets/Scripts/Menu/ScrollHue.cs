namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ScrollHue : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        public Color currentColor;
        public float alpha;
    
        [SerializeField]
        private float _speed = 0.1f;
    
        // Start is called before the first frame update
        void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            currentColor = _sprite.color;
            alpha = currentColor.a;
        }
    
        // Update is called once per frame
        void Update()
        {
            Color color = currentColor;
            float[] hsv = new float[3];
            Color.RGBToHSV(currentColor, out hsv[0], out hsv[1], out hsv[2]);
            color = Color.HSVToRGB(hsv[0] + Time.deltaTime * _speed, hsv[1], hsv[2]);
            currentColor = color;
            currentColor.a = alpha;
            _sprite.color = currentColor;
        }
    }
}
