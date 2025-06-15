using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TightStuff.Menu
{
    public class KeypadButton : MonoBehaviour
    {
        private Text _keycode;
        [SerializeField]
        private TMP_InputField _inputField;

        // Start is called before the first frame update
        void Start()
        {
            _keycode = GetComponentInChildren<Text>();
        }

        public void AddKey()
        {
            if (_inputField.text.Length < 5)
                _inputField.text += _keycode.text;
        }
        public void RemoveKey()
        {
            if (_inputField.text.Length >= 1)
                _inputField.text = _inputField.text.Remove(_inputField.text.Length - 1);
        }
    }
}
