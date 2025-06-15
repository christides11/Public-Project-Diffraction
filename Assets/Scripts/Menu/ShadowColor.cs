namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ShadowColor : MonoBehaviour
    {
        [SerializeField]
        private List<Shadow> shadow;
    
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        public void SetShadowColor(Color color)
        {
            foreach (var shadow in shadow)
                shadow.effectColor = color;
        }
    }
}
