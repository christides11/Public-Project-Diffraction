namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class OscilateZ : MonoBehaviour
    {
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _amplitude;
        [SerializeField]
        private float _offset;
    
        private Vector3 _initPosition;
    
        private void Start()
        {
            _initPosition = transform.position;
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            transform.position = _initPosition + Vector3.forward * Mathf.Sin(Time.time * _speed + _offset) * _amplitude;
        }
    }
}
