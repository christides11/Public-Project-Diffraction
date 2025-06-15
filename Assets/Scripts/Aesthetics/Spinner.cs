namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Spinner : UpdateAbstract //CLASS SHOULD ONLY BE USED FOR AESTHETIC OBJECTS IN CASE OF PRESERVING ROLLBACK STATES
    {
        [SerializeField]
        private float _spinSpd;
    
        public float SpinSpd { get => _spinSpd; set => _spinSpd = value; }
    
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (MatchManager.rollingBack <= 0)
                transform.eulerAngles += _spinSpd * MatchManager.worldTime * Vector3.forward;
        }
    }
}
