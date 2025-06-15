namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class SetTextInt : MonoBehaviour
    {
        [SerializeField]
        private float _maxVal;
    
        [SerializeField]
        private Text _text;
    
        // Start is called before the first frame update
        void Start()
        {
            _text = GetComponent<Text>();
        }
    
        public void ChangeText(float val)
        {
            var t = val - 1;
            if (t < 0)
                t = _maxVal;
            if (t > _maxVal)
                t = 0;
            _text.text = t.ToString();
            Debug.Log(t + " " + _maxVal);
            if (t == _maxVal)
            {
                _text.text = "\u221E";
            }
        }
    }
}
