namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class StrictFollowObject : UpdateAbstract
    {
        [SerializeField]
        private Transform _target;
        // Start is called before the first frame update
        void Start()
        {
            transform.parent = null;
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            transform.position = _target.position;
        }
    }
}
