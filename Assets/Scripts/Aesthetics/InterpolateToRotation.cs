namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class InterpolateToRotation : MonoBehaviour
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
        void FixedUpdate()
        {
            if (MatchManager.paused)
                return;
            _target.rotation = Quaternion.Lerp(_target.rotation, transform.rotation, _strength);
        }
    }
}
