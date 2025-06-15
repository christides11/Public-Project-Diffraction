namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ChangeCrystalMaterial : MonoBehaviour
    {
        private MeshRenderer[] _renderers;
        // Start is called before the first frame update
        void Start()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>();
        }
    
        public void ChangeCrystalMat(Material mat)
        {
            if (_renderers == null)
                return;
            foreach (var renderer in _renderers)
                renderer.material = mat;
        }
    }
}
