namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
    
    public class InterpDeathPredictionColorAdjustment : MonoBehaviour
    {
        public Volume volume;
        public ColorAdjustments colorAdjustments;
    
        // Start is called before the first frame update
        void Start()
        {
            volume = GetComponent<Volume>();
        }
    
        // Update is called once per frame
        void Update()
        {
            volume.profile.TryGet(out colorAdjustments);
    
            colorAdjustments.hueShift.Override(Mathf.Lerp(colorAdjustments.hueShift.value, 0, Time.deltaTime * 10));
        }
    
        public void InvertColor()
        {
            colorAdjustments.hueShift.Override(180);
        }
    }
}
