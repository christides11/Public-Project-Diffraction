using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TightStuff.Menu
{
    public class SliderDropDown : MonoBehaviour
    {
        public List<string> items;
        public Text textField;

        private Slider _slider;

        // Start is called before the first frame update
        void Start()
        {
            _slider = GetComponent<Slider>();
            _slider.maxValue = items.Count - 1;
            UpdateTextField();
        }

        public void UpdateTextField()
        {
            textField.text = items[(int)_slider.value];
        }
    }
}
