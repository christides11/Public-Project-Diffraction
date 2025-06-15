namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Gohei : MonoBehaviour
    {
        private Rigidbody2D rb;
    
        public float oscilateRate = 6;
        [SerializeField]
        private float _oscilateAmplitude;
    
        [SerializeField]
        private Vector2 _windDirection;
        [SerializeField]
        private float _windStrength;
    
        public Vector2 gravityDirection;
        [SerializeField]
        private float _gravityStrength;
    
        [SerializeField]
        private float _offset = 1;
    
        private float _timer;
    
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            _timer += _offset;
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            if (MatchManager.paused)
                return;
            var windDir = Mathf.Sin(_timer) * _oscilateAmplitude;
            _timer += oscilateRate * Time.fixedDeltaTime * MatchManager.worldTime;
    
            rb.velocity += gravityDirection * _gravityStrength * MatchManager.worldTime;
            rb.velocity += Mathf.Sign(windDir) * _windStrength * Vector2.Perpendicular(gravityDirection) * MatchManager.worldTime;
    
            if (_timer > Mathf.PI * 2)
                _timer = 0;
        }
    
        public void Flick(Vector2 force)
        {
            rb.velocity = force;
        }
    }
}
