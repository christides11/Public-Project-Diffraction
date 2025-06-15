namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class InterpolateTo : UpdateAbstract
    {
        [SerializeField]
        private float _strength = 0.1f;
    
        [SerializeField]
        private Transform _target;
    
        [SerializeField]
        private bool _unparent;
    
        // Start is called before the first frame update
        void Start()
        {
            if (_unparent)
                _target.parent = null;
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (MatchManager.paused)
                return;
            _target.position = Vector2.Lerp(_target.position, transform.position, _strength);
        }
    
        public void ReturnToObject()
        {
            _target.position = transform.position;
        }
    }
}
