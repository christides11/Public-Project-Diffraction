using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TightStuff
{
    public class AnimateLegacyText : MonoBehaviour
    {
        public float sizeMultiplier;
        private Text _text;

        private int _ogFontSize;

        // Start is called before the first frame update
        void Start()
        {
            _text = GetComponent<Text>();
            _ogFontSize = _text.fontSize;
        }

        // Update is called once per frame
        void Update()
        {
            _text.fontSize = (int)(_ogFontSize * sizeMultiplier);  
        }
    }
}
