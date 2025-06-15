namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ApplyGravity : MonoBehaviour
    {
        private Rigidbody2D _rb;
        [SerializeField] private float _gravity;
        // Start is called before the first frame update
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            _rb.velocity -= _gravity * Vector2.up * MatchManager.worldTime;
        }
    }
}
