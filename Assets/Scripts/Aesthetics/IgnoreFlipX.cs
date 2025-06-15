namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class IgnoreFlipX : UpdateAbstract
    {
        // Update is called once per frame
        void GUpdate()
        {
            if (transform.lossyScale.x < 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
