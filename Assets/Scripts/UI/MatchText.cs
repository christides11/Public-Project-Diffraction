namespace TightStuff.UI
{
    using UnityEngine.UI;
    using UnityEngine;
    using TMPro;
    
    public class MatchText : MonoBehaviour
    {
        [SerializeField]
        private Text _followText;
        [SerializeField]
        private Text _selfText;
        [SerializeField]
        private TextMeshProUGUI _selfTextPro;
        [SerializeField]
        private TextMeshProUGUI _followTextPro;
    
        private void Start()
        {
            _selfText = GetComponent<Text>();
            _selfTextPro = GetComponent<TextMeshProUGUI>();
        }
    
        // Update is called once per frame
        void LateUpdate()
        {
            if (_selfText != null)
                _selfText.text = _followText.text;
            if (_selfTextPro != null)
                _selfTextPro.text = _followTextPro.text;
        }
    }
}
