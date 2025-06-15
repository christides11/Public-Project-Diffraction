using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TightStuff
{
    public class FillControl : MonoBehaviour
    {
        [SerializeField]
        private Image _fillImage;

        [SerializeField]
        private float _fillSpd;

        public bool AutomaticFilling { get; set; }


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (AutomaticFilling) 
                _fillImage.fillAmount += _fillSpd * Time.deltaTime;
        }
    }
}
