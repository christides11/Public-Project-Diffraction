namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
    
    public class DreamEffectControl : MonoBehaviour
    {
        public Volume volume;
        public Vignette vignette;
        public ChromaticAberration aberration;
    
        public float vigValue = 0.3f;
        public float abVal = 0.2f;
    
        // Start is called before the first frame update
        void Start()
        {
            volume = GetComponent<Volume>();
        }
    
        // Update is called once per frame
        void Update()
        {
            volume.profile.TryGet(out vignette);
            volume.profile.TryGet(out aberration);
    
            vignette.intensity.Override(vigValue);
            aberration.intensity.Override(abVal);
        }
    }
}
